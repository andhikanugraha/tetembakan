using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net.Sockets;

namespace Sister2
{
    class Client
    {
        Socket m_socClient;

        static void Main(string[] args)
        {
            Client c = new Client();

            try
            {
                //create a new client socket ...
                c.m_socClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                String szIPSelected = "167.205.86.253";
                String szPort = "8221";
                int alPort = System.Convert.ToInt16(szPort, 10);

                System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(szIPSelected);
                System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, alPort);
                c.m_socClient.Connect(remoteEndPoint);
                String szData = "Hello There";
                Console.WriteLine("Sending message");
                byte[] byData = System.Text.Encoding.UTF8.GetBytes(szData);
                c.m_socClient.Send(byData);
                Console.WriteLine("Sent");
                c.m_socClient.Close();
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }
    }
}
