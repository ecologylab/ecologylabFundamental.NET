using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using ecologylab.serialization;
using ecologylab.oodss.messages;
using System.Collections.Concurrent;

/* 
 * 
 *  This is the initial OODSS for C#.  It uses the async library.
 *  Use the connect function to connect to an OODSS client and the GetResponse to
 *  get and send data.
 */
 
namespace Simpl.OODSS
{
    public class OODSSClient {
        public Socket clientSocket;

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
                F
                //push
                StringBuilder rq = new StringBuilder();
                request.serialize(rq, Format.XML);
                //request.serializeToXML(rq);
                String sendMessage = message_for_xml(rq.ToString());
                Console.WriteLine("Trying to send the string:" + sendMessage);
                clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
            

                //pull
                byte[] data = new byte[1024*1024];
                int receivedDataLength = clientSocket.Receive(data);
                string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);
            

                stringData = stringData.Substring(stringData.IndexOf('<')-1);
                Console.WriteLine(stringData);
                responseElementState = translationScope.deserializeString(stringData, new ecologylabFundamental.ecologylab.serialization.TranslationContext(),Format.XML);
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

        Thread workingThread;
        static int callNumber = 0;    
        public OODSSClient(String host, int port)
        {
            StartClient(host, port);
            GetResponse(new InitConnectionRequest());
            workingThread = new Thread(new ThreadStart(messageWorker));
            blockingQueue = new BlockingCollection<QueueObject>();
            blockingResponse = new BlockingCollection<ElementState>();
            workingThread.Start();
        
        }

        int lastServed = -1;
        public async Task<ElementState> GetRequestAsync(ElementState elementState, TranslationScope translationScope)
        {
            int callNum = callNumber;
        
            //queue.Add(new QueueObject(elementState,translationScope,callNumber));
            callNumber+=1;
            await TaskEx.Run(() => pauseTillServedBlah(callNum));
            QueueObject toTheQueue = new QueueObject(elementState, translationScope, callNumber);
            blockingQueue.Add(toTheQueue);
            ElementState elementStateResponse = null;
            await TaskEx.Run(() => elementStateResponse=blockingResponse.Take());
            lastServed = callNum;
            return elementStateResponse;
        }

        void pauseTillServedBlah(int i)
        {
            while (true)
            {
                if (lastServed == i - 1)
                    break;
                Thread.Sleep(50);
            }
        }

        struct QueueObject
        {
            public ElementState elementState;
            public TranslationScope translationScope;
            public int callNumber;
            public ElementState response;
            public QueueObject(ElementState elementState, TranslationScope translationScope, int callNumber)
            {
                this.elementState = elementState;
                this.translationScope = translationScope;
                this.callNumber = callNumber;
                response = null;
            }
        }

        BlockingCollection<QueueObject> blockingQueue;
        BlockingCollection<ElementState> blockingResponse;
        public void messageWorker()
        {
            while (true)
            {
                QueueObject q = blockingQueue.Take();
                blockingResponse.Add( GetResponse(q.elementState,q.translationScope));
            }
        }

        public void doWork()
        {
            //while (true)
            for (int i = 0; i < 5; i++ )
            {
                Console.WriteLine("derp");
                Thread.Sleep(500);
                Console.WriteLine("dop");
            }
            // return 0;

        }
    }
}
