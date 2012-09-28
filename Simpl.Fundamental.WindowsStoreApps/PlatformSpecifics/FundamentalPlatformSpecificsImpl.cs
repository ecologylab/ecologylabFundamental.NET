using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Simpl.Fundamental.Net;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Simpl.Fundamental.PlatformSpecifics
{
    class FundamentalPlatformSpecificsImpl : IFundamentalPlatformSpecifics
    {
        public object CreateFile(string uri)
        {
            try
            {
                var operation =  StorageFile.GetFileFromPathAsync(uri);
                return operation.GetResults();
            }
            catch(IOException e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        public string GetDirFullNameFromFile(object file)
        {
            var storageFile = file as StorageFile;
            if (storageFile != null)
            {
                return storageFile.Path;
            }
            Debug.WriteLine("Error: not a storage file");
            throw new IOException();
        }

        public string[] GetFilesFromDirectory(string dir, string fileType)
        {
            throw new NotImplementedException();
        }

        public string[] GetDirectoriesFromDirectory(string dir)
        {
            throw new NotImplementedException();
        }

        public void Connect(IConnectionHelper connectionHelper, string userAgent, int connectionTimeout, int readTimeout, PURLConnection purlConnection)
        {
            if (purlConnection.PURL.IsFile)
            {
                FileAttributes attributes = ((StorageFile) purlConnection.File).Attributes;
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

        public Stream OpenFileReadStream(object file)
        {
            return OpenFileReadStreamAsync(file).Result;
        }

        private async Task<Stream> OpenFileReadStreamAsync(object file)
        {
            var storageFile = file as StorageFile;
            if (storageFile != null)
            {
                return await storageFile.OpenStreamForReadAsync();
            }
            Debug.WriteLine("Error: not a storage file for windows store app");
            throw new IOException();
        }

        public Stream OpenFileReadStream(object file, Encoding encoding)
        {
            var stream = OpenFileReadStreamAsync(file).Result;
            if (stream != null)
            {
                StreamReader fileStream = new StreamReader(stream, encoding);
                return fileStream.BaseStream;
            }
            Debug.WriteLine("Error: stream is null");
            throw new IOException();
        }

        private async Task<Stream> OpenFileWriteStreamAsync(object file)
        {
            var storageFile = file as StorageFile;
            if (storageFile != null)
            {
                return await storageFile.OpenStreamForWriteAsync();
            }
            Debug.WriteLine("Error: not a storage file for windows store app");
            throw new IOException();
        }

        public Stream OpenFileWriteStream(object file)
        {
            return OpenFileWriteStreamAsync(file).Result;
        }

        public StreamReader GenerateStreamReaderFromFile(string url)
        {
            var file = CreateFile(url);
            var stream = OpenFileReadStream(file);
            return new StreamReader(stream);
        }

        public void NetworkConnect(IConnectionHelperJustRemote connectionHelper, string userAgent, PURLConnection purlConnection, int connectionTimeout = ParsedUri.CONNECT_TIMEOUT, int readTimeout = ParsedUri.READ_TIMEOUT)
        {
            Uri url = purlConnection.PURL;
            purlConnection.Request = WebRequest.CreateHttp(url);
            if (purlConnection.Request != null)
            {
                //            purlConnection.Request.UserAgent = userAgent;
                //            purlConnection.Request.Timeout = connectionTimeout;
                //            purlConnection.Request.ReadWriteTimeout = readTimeout;
                try
                {
                    purlConnection.Response = (HttpWebResponse) purlConnection.Request.GetResponseAsync().Result;
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

            var urlWithQuery = parsedUri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.UriEscaped);
            var query = parsedUri.GetComponents(UriComponents.Query, UriFormat.UriEscaped);
            var index = urlWithQuery.IndexOf(query);
            string urlWithoutQuery = urlWithQuery;
            if (index != -1)
            {
                urlWithoutQuery = urlWithQuery.Remove(index);
            }
            return urlWithoutQuery;
        }
    }
}
