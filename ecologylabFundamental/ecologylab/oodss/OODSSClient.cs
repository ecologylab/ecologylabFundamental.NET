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
    public Task StartClient(string host,int port) 
    {
        try 
        {
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
    
    public ElementState GetResponse(ElementState request,TranslationScope ts=null)
    {
        TranslationScope translationScope = OODSSMessages.Get();
        if (ts != null)
            translationScope.AddTranslations(ts); 
        ElementState responseElementState = null;
        try
        {

            //push
            StringBuilder rq = new StringBuilder();
            request.serializeToXML(rq);
            String sendMessage = message_for_xml(rq.ToString());
            Console.WriteLine("Trying to send the string:" + sendMessage);
            clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
            

            //pull
            byte[] data = new byte[1024*1024];
            int receivedDataLength = clientSocket.Receive(data);
            string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
            

            stringData = stringData.Substring(stringData.IndexOf('<')-1);
            Console.WriteLine(stringData);
            responseElementState = translationScope.deserializeString(stringData, Format.XML);
            return responseElementState;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        return responseElementState;
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
