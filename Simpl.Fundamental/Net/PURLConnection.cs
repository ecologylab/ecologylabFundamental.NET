using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Simpl.Fundamental.Net
{
    /// <summary>
    /// Combines URLConnection with InputStream, providing convenience.
    /// </summary>
    public class PURLConnection
    {
        private static readonly string[] NoAlphaMimeStrings = { "image/jpeg", "image/bmp", };

        private static readonly HashSet<string> NoAlphaMimeSet = new HashSet<string>(NoAlphaMimeStrings);

        
        public PURLConnection(ParsedUri purl)
        {
            PURL = purl;
        }

        public PURLConnection(ParsedUri purl, HttpWebRequest request, Stream inputStream)
        {
            PURL = purl;
            Stream = inputStream;
            Request = request;
            Good = true;
        }

        public ParsedUri PURL { get; private set; }

        private FileInfo file;

        public FileInfo File
        {
            get { return file ?? (file = new FileInfo(PURL.PathAndQuery.Replace('/', Path.DirectorySeparatorChar))); }
        }


        private string mimeType;

        /// <summary>
        /// Find the mime type returned by the web server to the URLConnection, in its header.
        /// Thus, if there is no URLConnection (as for local file system), this always returns null.
        /// </summary>
        public string MimeType
        {
            get
            {
                if (mimeType == null)
                {
                    string contentType = Response.ContentType;
                    int i = contentType.IndexOf(';');
                    if (i > 0)
                        mimeType = contentType.Substring(0, i);
                }
                return mimeType;
            }
            private set { mimeType = value; }
        }

        public Stream Stream { get; private set; }

        public HttpWebRequest Request { get; private set; }

        public HttpWebResponse Response { get; private set; }

        /// <summary>
        /// If true, a timeout occurred during Connect().
        /// </summary>
        public bool Timeout { get; private set; }

        public bool Good { get; private set; }

        private ParsedUri responsePURL;

        public ParsedUri ResponsePURL
        {
            get
            {
                if (responsePURL == null && Response != null)
                    responsePURL = new ParsedUri(Response.ResponseUri.AbsoluteUri);
                return responsePURL;
            }
        }


        public void Connect(IConnectionHelper connectionHelper, String userAgent,
                            int connectionTimeout, int readTimeout)
        {
            // get an InputStream, and set the mimeType, if not bad
            if (PURL.IsFile)
            {
                FileAttributes attributes = File.Attributes;
                if (attributes.HasFlag(FileAttributes.Directory))
                {
                    connectionHelper.HandleFileDirectory(File);
                }
                else
                {
                    string suffix = PURL.Suffix;
                    if (suffix != null)
                    {
                        if (connectionHelper.ParseFilesWithSuffix(suffix))
                        {
                            try
                            {
                                FileConnect();
                            }
                            catch (FileNotFoundException e)
                            {
                                Console.WriteLine("ERROR: Can't open because FileNotFoundException");
                            }
                        }
                    }
                }
            }
            else
            {
                NetworkConnectAndCatch(connectionHelper, userAgent, connectionTimeout, readTimeout);
            }
        }

        public void FileConnect()
        {
            Stream = File.Open(FileMode.Open, FileAccess.Read);
            Good = true;
        }

        public void NetworkConnectAndCatch(IConnectionHelper connectionHelper, String userAgent,
                                           int connectionTimeout = ParsedUri.CONNECT_TIMEOUT,
                                           int readTimeout = ParsedUri.READ_TIMEOUT)
        {
            NetworkConnect(connectionHelper, userAgent, connectionTimeout, readTimeout);
            try
            {
                NetworkConnect(connectionHelper, userAgent, connectionTimeout, readTimeout);
            }
            catch (WebException e)
            {
                Timeout = true;
                CleanUp(e);
            }
            catch (Exception e) // catch all exceptions, including security
            {
                CleanUp(e);
            }
        }

        public void NetworkConnect(IConnectionHelperJustRemote connectionHelper, String userAgent,
                                   int connectionTimeout = ParsedUri.CONNECT_TIMEOUT,
                                   int readTimeout = ParsedUri.READ_TIMEOUT)
        {
            Uri url = PURL;
            Request = WebRequest.CreateDefault(url) as HttpWebRequest;
            if (Request != null)
            {
                Request.UserAgent = userAgent;
                Request.Timeout = connectionTimeout;
                Request.ReadWriteTimeout = readTimeout;

                try
                {
                    Response = (HttpWebResponse)Request.GetResponse();
                }
                catch (WebException e)
                {
                    Console.WriteLine("Web Exception ::" + e.Message);
                }
                if (Response != null)
                {
                    // TODO check charset (using mime type) and display error message if charset not supported.

                    Uri responseUrl = Response.ResponseUri;
                    if (responseUrl != url) // follow redirects!
                    {
                        string requestPath = url.AbsolutePath;
                        string responsePath = responseUrl.AbsolutePath;
                        if (requestPath.IndexOf("http://") < 0 && responsePath.IndexOf("http://") < 0)
                        {
                            if (connectionHelper.ProcessRedirect(responseUrl))
                                Stream = Response.GetResponseStream();
                            Good = true;
                        }
                        else
                        {
                            Console.WriteLine("WEIRD: skipping double stuffed URL: " + responseUrl);
                        }
                    }
                    else
                    {
                        Stream = Response.GetResponseStream();
                        Good = true;
                    }
                }
                else
                {
                    Console.WriteLine("ERROR: failure to get response from " + url);
                }
            }
            else
            {
                Console.WriteLine("ERROR: cannot create a connection to " + url);
            }
        }

        private void CleanUp(Exception e)
        {
            Console.WriteLine("PURLConnection.CleanUp(): " + e.Message);
            Close();
        }

        public void Recycle()
        {
            Close();
            //		purl.recycle();
            //		purl							= null;
        }

        public void Reconnect()
        {
            if (PURL != null && PURL.IsFile && Stream == null)
            {
                try
                {
                    Stream = File.Open(FileMode.Open, FileAccess.Read);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("ERROR: " + e);
                }
            }
        }

        public void Close()
        {
            // parsing done. now free resources asap to avert leaking and memory fragmentation
            // (this is a known problem w java.net.HttpURLConnection)
            if (Stream != null)
            {
                Stream.Close();
                Stream = null;
            }
            if (Request != null)
            {
                Request.Abort();
                Request = null;
            }
            if (Response != null)
            {
                Response.Close();
                Response = null;
            }
            MimeType = null;
        }

        public override string ToString()
        {
            return "PURLConnection[" + PURL + "]";
        }

        public bool IsNoAlpha()
        {
            return MimeType != null && NoAlphaMimeSet.Contains(mimeType);
        }
    }
}
