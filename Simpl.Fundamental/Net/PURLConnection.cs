using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Simpl.Fundamental.PlatformSpecifics;

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

        private object _file;

        //public object File { get { return null; } }

        public object File
        {
            get { return _file ?? (_file = FundamentalPlatformSpecifics.Get().CreateFile(PURL.PathAndQuery.Replace('/', '\\'))); }
        }


        private string _mimeType;

        /// <summary>
        /// Find the mime type returned by the web server to the URLConnection, in its header.
        /// Thus, if there is no URLConnection (as for local file system), this always returns null.
        /// </summary>
        public string MimeType
        {
            get
            {
                if (_mimeType == null)
                {
                    string contentType = Response.ContentType;
                    int i = contentType.IndexOf(';');
                    if (i > 0)
                        _mimeType = contentType.Substring(0, i);
                }
                return _mimeType;
            }
            private set { _mimeType = value; }
        }

        public Stream Stream { get; set; }

        public HttpWebRequest Request { get; set; }

        public HttpWebResponse Response { get; set; }

        /// <summary>
        /// If true, a timeout occurred during Connect().
        /// </summary>
        public bool Timeout { get; private set; }

        public bool Good { get; set; }

        private ParsedUri _responsePurl;

        public ParsedUri ResponsePURL
        {
            get
            {
                if (_responsePurl == null && Response != null)
                    _responsePurl = new ParsedUri(Response.ResponseUri.AbsoluteUri);
                return _responsePurl;
            }
        }


        public void Connect(IConnectionHelper connectionHelper, String userAgent,
                            int connectionTimeout, int readTimeout)
        {
            // get an InputStream, and set the mimeType, if not bad
            FundamentalPlatformSpecifics.Get().Connect(connectionHelper, userAgent, connectionTimeout, readTimeout, this);
        }

        public void FileConnect()
        {
            Stream = FundamentalPlatformSpecifics.Get().OpenFileReadStream(File);
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
            FundamentalPlatformSpecifics.Get().NetworkConnect(connectionHelper, userAgent, this, connectionTimeout, readTimeout);
        }

        private void CleanUp(Exception e)
        {
            Debug.WriteLine("PURLConnection.CleanUp(): " + e.Message);
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
                    Stream = FundamentalPlatformSpecifics.Get().OpenFileReadStream(File);
                }
                catch (FileNotFoundException e)
                {
                    Debug.WriteLine("ERROR: " + e);
                }
            }
        }

        public void Close()
        {
            // parsing done. now free resources asap to avert leaking and memory fragmentation
            // (this is a known problem w java.net.HttpURLConnection)
            if (Stream != null)
            {
                Stream.Dispose();
                Stream = null;
            }
            if (Request != null)
            {
                Request.Abort();
                Request = null;
            }
            if (Response != null)
            {
                Response.Dispose();
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
            return MimeType != null && NoAlphaMimeSet.Contains(_mimeType);
        }
    }
}
