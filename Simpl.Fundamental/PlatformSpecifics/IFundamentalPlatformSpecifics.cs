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
        object CreateFile(string uri);

        string GetDirFullNameFromFile(object file);

        String[] GetFilesFromDirectory(String dir, String fileType);

        String[] GetDirectoriesFromDirectory(String dir);

        void Connect(IConnectionHelper connectionHelper, String userAgent,
                     int connectionTimeout, int readTimeout, PURLConnection purlConnection);

        Stream OpenFileReadStream(object file);

        Stream OpenFileReadStream(object file, Encoding encoding);

        Stream OpenFileWriteStream(object file);

        StreamReader GenerateStreamReaderFromFile(String url);

        void NetworkConnect(IConnectionHelperJustRemote connectionHelper, String userAgent,
                            PURLConnection purlConnection,
                            int connectionTimeout = ParsedUri.CONNECT_TIMEOUT,
                            int readTimeout = ParsedUri.READ_TIMEOUT);

        string GetUriLeftPart(ParsedUri parsedUri);
    }
}
