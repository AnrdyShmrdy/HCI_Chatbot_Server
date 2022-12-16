using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Chatbot_Server
{
    public class ChatbotServer
    {
        private readonly int port;
        private readonly IPHostEntry host;
        private readonly IPAddress ipAddress;
        private readonly IPEndPoint localEndPoint;
        public ChatbotServer(string ipAddress = "127.0.0.1", int port = 11000)
        {
            this.port = port;
            host = Dns.GetHostEntry("localhost");
            /*ipAddress = host.AddressList[0]*/;
            this.ipAddress = IPAddress.Parse(ipAddress);
            localEndPoint = new IPEndPoint(this.ipAddress, port);
        }

        public void StartServer()
        {

            try
            {
                // Create a Socket that will use Tcp protocol
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // A Socket must be associated with an endpoint using the Bind method
                listener.Bind(localEndPoint);
                // Specify how many requests a Socket can listen before it gives Server busy response.
                // We will listen 10 requests at a time
                listener.Listen(10);
                while (true)
                {
                Console.WriteLine("Waiting for a connection...");
                Socket handler = listener.Accept();
                    // Incoming data from the client.
                    string? data = null;
                    byte[]? bytes = null;

                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    Console.WriteLine("Text received : {0}", data);

                    byte[] msg = Encoding.ASCII.GetBytes(data);
                    handler.Send(msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
