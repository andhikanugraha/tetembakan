using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Client
    {
        private Socket soc;
        private IPAddress remoteIP;
        private IPEndPoint remoteEndpoint;

        private string _serverIP;
        public string ServerIP
        {
            get
            {
                return _serverIP;
            }
            set
            {
                if (value != _serverIP)
                {
                    _serverIP = value;
                    remoteIP = IPAddress.Parse(value);
                }
            }
        }

        public Client()
        {
            try
            {
                soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (SocketException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void Connect()
        {
            remoteEndpoint = new IPEndPoint(remoteIP, Convert.ToInt16("8081"));
            soc.Connect(remoteEndpoint);
        }

        public void Close()
        {
            soc.Close();
        }

        public void Send(byte[] data)
        {
            soc.Send(data);
        }
    }
}
