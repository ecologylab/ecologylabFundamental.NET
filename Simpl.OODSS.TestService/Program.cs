using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simpl.OODSS.TestService.Server;

namespace Simpl.OODSS.TestService
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 2018;
            string serverAddress = "localhost";
            TestServer server = new TestServer(port);
            server.StartService();
        }
    }
}
