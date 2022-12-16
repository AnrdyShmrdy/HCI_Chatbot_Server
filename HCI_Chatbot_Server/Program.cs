using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

// Socket Listener acts as a server and listens to the incoming
// messages on the specified port and protocol.
namespace HCI_Chatbot_Server
{
    public class Program
    {
        public static void Main()
        {
            ChatbotServer chatbotServer = new ChatbotServer();
            chatbotServer.StartServer();
        }
    }
}