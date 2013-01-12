using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Simpl.Fundamental.Net;

namespace Simpl.Fundamental.PlatformSpecifics
{
    class FundamentalPlatformSpecificsImpl : IFundamentalPlatformSpecifics
    {
        public async Task<object> CreateFile(string uri)
        {
            return new FileInfo(uri);
        }

        public async Task<object> CreateDirectory(string uri)
        {
            return Directory.CreateDirectory(uri);
        }

        public string GetDirFullNameFromFile(object file)
        {
            var fileInfo = file as FileInfo;
            if (fileInfo != null)
                return fileInfo.FullName;
            Debug.WriteLine("Warning: can not obtain file name");
            throw new FileNotFoundException();
        }

        public async Task<string[]> GetFilesFromDirectory(string dir, string fileTypePostfix)
        {
            return Directory.GetFiles(dir, fileTypePostfix);
        }

        public async Task<string[]> GetDirectoriesFromDirectory(string dir)
        {
            return Directory.GetDirectories(dir);
        }

        public void Connect(IConnectionHelper connectionHelper, string userAgent, int connectionTimeout, int readTimeout, PURLConnection purlConnection)
        {
            if (purlConnection.PURL.IsFile)
            {
                FileAttributes attributes = ((FileInfo) purlConnection.File).Attributes;
                if (attributes.HasFlag(FileAttributes.Directory))
                {
                    connectionHelper.HandleFileDirectory(purlConnection.File);
                }
                else
                {
                    string suffix = purlConnection.PURL.Suffix;
                    if (suffix != null)
                    {
                        if (connectionHelper.ParseFilesWithSuffix(suffix))
                        {
                            try
                            {
                                purlConnection.FileConnect();
                            }
                            catch (FileNotFoundException e)
                            {
                                Debug.WriteLine("ERROR: Can't open because FileNotFoundException");
                            }
                        }
                    }
                }
            }
            else
            {
                purlConnection.NetworkConnectAndCatch(connectionHelper, userAgent, connectionTimeout, readTimeout);
            }
        }

        public async Task<Stream> OpenFileReadStream(object file)
        {
            var fileinfo = file as FileInfo;
            if (fileinfo != null)
                return fileinfo.Open(FileMode.Open, FileAccess.Read);
            Debug.WriteLine("Error: not a file");
            throw new IOException();
        }

        public async Task<Stream> OpenFileReadStream(object file, Encoding encoding)
        {
            var fileinfo = file as FileInfo;
            if (fileinfo != null)
            {
                StreamReader fileStream = new StreamReader(((FileInfo)file).OpenRead(), encoding);
                return fileStream.BaseStream;
            }
            Debug.WriteLine("Error: not a file");
            throw new IOException();
        }

        public async Task<Stream> OpenFileWriteStream(object file)
        {
            var fileinfo = file as FileInfo;
            if (fileinfo != null)
                return fileinfo.OpenWrite();
            Debug.WriteLine("Error: not a file");
            throw new IOException();
        }

        public async Task<StreamReader> GenerateStreamReaderFromFile(string url)
        {
            return new StreamReader(url);
        }

        public void NetworkConnect(IConnectionHelperJustRemote connectionHelper, string userAgent, PURLConnection purlConnection, int connectionTimeout = ParsedUri.CONNECT_TIMEOUT, int readTimeout = ParsedUri.READ_TIMEOUT)
        {
            Uri url = purlConnection.PURL;
            purlConnection.Request = WebRequest.CreateDefault(url) as HttpWebRequest;
            if (purlConnection.Request != null)
            {
                purlConnection.Request.UserAgent = userAgent;
                purlConnection.Request.Timeout = connectionTimeout;
                purlConnection.Request.ReadWriteTimeout = readTimeout;

                try
                {
                    purlConnection.Response = (HttpWebResponse)purlConnection.Request.GetResponse();
                }
                catch (WebException e)
                {
                    Debug.WriteLine("Web Exception ::" + e.Message);
                }
                if (purlConnection.Response != null)
                {
                    // TODO check charset (using mime type) and display error message if charset not supported.

                    Uri responseUrl = purlConnection.Response.ResponseUri;
                    if (responseUrl != url) // follow redirects!
                    {
                        string requestPath = url.AbsolutePath;
                        string responsePath = responseUrl.AbsolutePath;
                        if (requestPath.IndexOf("http://") < 0 && responsePath.IndexOf("http://") < 0)
                        {
                            if (connectionHelper.ProcessRedirect(responseUrl))
                                purlConnection.Stream = purlConnection.Response.GetResponseStream();
                            purlConnection.Good = true;
                        }
                        else
                        {
                            Debug.WriteLine("WEIRD: skipping double stuffed URL: " + responseUrl);
                        }
                    }
                    else
                    {
                        purlConnection.Stream = purlConnection.Response.GetResponseStream();
                        purlConnection.Good = true;
                    }
                }
                else
                {
                    Debug.WriteLine("ERROR: failure to get response from " + url);
                }
            }
            else
            {
                Debug.WriteLine("ERROR: cannot create a connection to " + url);
            }
        }



        public string GetUriLeftPart(ParsedUri parsedUri)
        {
            return parsedUri.GetLeftPart(UriPartial.Path);
        }
    }
}
