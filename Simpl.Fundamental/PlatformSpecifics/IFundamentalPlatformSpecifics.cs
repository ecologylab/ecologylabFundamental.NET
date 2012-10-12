using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpl.Fundamental.Net;

namespace Simpl.Fundamental.PlatformSpecifics
{
    public interface IFundamentalPlatformSpecifics
    {
        Task<object> CreateFile(string uri);

        Task<object> CreateDirectory(string uri);

        string GetDirFullNameFromFile(object file);

        Task<String[]> GetFilesFromDirectory(String dir, String fileType);

        Task<string[]> GetDirectoriesFromDirectory(String dir);

        void Connect(IConnectionHelper connectionHelper, String userAgent,
                     int connectionTimeout, int readTimeout, PURLConnection purlConnection);

        Task<Stream> OpenFileReadStream(object file);

        Task<Stream> OpenFileReadStream(object file, Encoding encoding);

        Task<Stream> OpenFileWriteStream(object file);

        Task<StreamReader> GenerateStreamReaderFromFile(String url);

        void NetworkConnect(IConnectionHelperJustRemote connectionHelper, String userAgent,
                            PURLConnection purlConnection,
                            int connectionTimeout = ParsedUri.CONNECT_TIMEOUT,
                            int readTimeout = ParsedUri.READ_TIMEOUT);

        string GetUriLeftPart(ParsedUri parsedUri);
    }
}
