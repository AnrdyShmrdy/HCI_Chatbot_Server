using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HCI_Chatbot_Server
{
    public class ChatbotServer
    {
        private readonly int port;
        private readonly IPHostEntry host;
        private readonly IPAddress ipAddress;
        private readonly IPEndPoint localEndPoint;
        bool userExit = false;
        string userName = string.Empty;
        string userEmail = string.Empty;
        string userPhone = string.Empty;
        string ticketNo = string.Empty;
        public ChatbotServer(string ipAddress = "127.0.0.1", int port = 11000)
        {
            this.port = port;
            host = Dns.GetHostEntry("localhost");
            /*ipAddress = host.AddressList[0]*/;
            this.ipAddress = IPAddress.Parse(ipAddress);
            localEndPoint = new IPEndPoint(this.ipAddress, port);
        }

        public void ifMessageIsThis(string message)
        {
            if (message != null)
            {
                if (message.StartsWith("name:") || message.StartsWith("Name:"))
                {
                    string[] subs = message.Split(':');
                    userName= subs[1];
                }
                else if (message.StartsWith("phone:") || message.StartsWith("Phone:"))
                {
                    string[] subs = message.Split(':');
                    userPhone = subs[1];
                }
                else if (message.StartsWith("email:") || message.StartsWith("Email:"))
                {
                    string[] subs = message.Split(':');
                    userEmail = subs[1];
                }
                else if (message.StartsWith("ticket:") || message.StartsWith("Ticket:"))
                {
                    string[] subs = message.Split(':');
                    ticketNo = subs[1];
                }
                else if (message.StartsWith("exit") || message.StartsWith("Exit"))
                {
                    userExit = true;
                }
            }
        }
        public void GenerateTicketForm(string userName, string userPhone, string ticketNo, string userEmail)
        {
            string filePath = @"c:\temp\MyForm.html";

            // Delete the file if it exists.
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            static void AddText(FileStream fs, string value)
            {
                byte[] info = new UTF8Encoding(true).GetBytes(value);
                fs.Write(info, 0, info.Length);
            }

            //Create the file.
            using (FileStream fs = File.Create(filePath))
            {
                AddText(fs,
                    "<!DOCTYPE html>\n" +
                    "<html>\n" +
                    "<body>\n" +
                    "<h2>Ticket Submission Review</h2>\n" +
                    "<p>Please review the information below before submitting your ticket. " +
                    "A help desk representative will be in touch with you within 24-48 hours</p>\n" +
                    "<form action=\"\">\n" +
                    "<label for=\"uname\">Name:</label><br>\n" +
                    "<input type=\"text\" id=\"uname\" name=\"uname\" value=\"" + userName + "\"><br>\n" +
                    "<label for=\"phoneno\">Phone:</label><br>\n" +
                    "<input type=\"text\" id=\"phoneno\" name=\"phoneno\" value=\"" + userPhone + "\"><br>\n" +
                    "<label for=\"email\">Email:</label><br>\n" +
                    "<input type=\"text\" id=\"email\" name=\"email\" value=\"" + userEmail + "\"><br>\n" +
                    "<label for=\"ticketno\">Ticket Number: "+ticketNo+"</label><br><br>\n" +
                    "<input type=\"submit\" value=\"Submit\">\n" +
                    "</form>\n" +
                    "</body>\n" +
                    "</html>");
            }
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
                while (userExit == false)
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
                        break;
                    }

                    Console.WriteLine("Text received : {0}", data);
                    ifMessageIsThis(data);
                    byte[] msg = Encoding.ASCII.GetBytes(data);
                    handler.Send(msg);
                }
                Console.WriteLine("UserPhone: " + userPhone);
                Console.WriteLine("UserEmail: " + userEmail);
                Console.WriteLine("UserName: " + userName);
                Console.WriteLine("Ticket: " + ticketNo);
                GenerateTicketForm(userName, userPhone, ticketNo, userEmail);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
