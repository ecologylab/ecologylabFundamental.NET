using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using ecologylab.serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32.SafeHandles;
using ecologylab.oodss.messages;
/* 
 * 
 *  This is the initial OODSS for C#.  It uses the async library.
 *  Use the connect function to connect to an OODSS client and the GetResponse to
 *  get and send data.
 */
 
public class OODSSClient {
    Socket clientSocket;
    public Task StartClient(string host,int port) {
        try {
            IPHostEntry ipHostInfo = Dns.Resolve(host);//"achilles.cse.tamu.edu");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
             clientSocket = new Socket(AddressFamily.InterNetwork,  SocketType.Stream, ProtocolType.Tcp);
             clientSocket.Connect(host, port);            
        } catch (Exception e) {
            Console.WriteLine(e.ToString());
        }
        return null;
    }
    
    async public Task<ElementState> GetResponse(ElementState request,TranslationScope ts=null)
    {
        TranslationScope tsf = OODSSMessages.Get();
        if (ts != null)
            tsf.AddTranslations(ts); 
        ElementState ret = null;
        try
        {

        //push
        StringBuilder rq = new StringBuilder();
        request.serializeToXML(rq);
        clientSocket.Send(Encoding.ASCII.GetBytes(message_for_xml(rq.ToString())));
        Console.WriteLine("Trying to send the string:");

        //pull
        byte[] data = new byte[1024*1024];
        int receivedDataLength = clientSocket.Receive(data);
        string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
        Console.WriteLine(stringData);
        
            //This is hacky but it works right now
        FileStream fs = new FileStream(@"c:\temp\mcb.txt", FileMode.OpenOrCreate, FileAccess.Write);
        //strip header info
        int index = 0;
        for (int i = 0; i < data.Length; i++)
            if (data[i] == '<')
            {
                index = i;
                break;
            }
        byte[] shorter = new byte[data.Length - index];
        for (int i = 0; i < shorter.Length; i++)
        {
            shorter[i] = data[i + index];
        }
        fs.Write(shorter, 0, shorter.Length);
        fs.Close();
        
        ret = tsf.deserialize(@"c:\temp\mcb.txt", Format.XML);
        
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        return ret;
    }
    
    static int n=1;
    public string message_for_xml(string x)
    {
        string ret = "content-length:" + x.Length + "\r\nuid:" + n + "\r\n\r\n" + x;
        n++;
        return ret;
    }

    public OODSSClient()
    {
    }

    public OODSSClient(String host, int port)
    {
        StartClient(host, port);
        GetResponse(new InitConnectionRequest());
    }


}
