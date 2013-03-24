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
        private byte[] pstr;
        private byte[] reserved = { 0, 0, 0, 0, 0, 0, 0, 0 };

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
            pstr = Encoding.ASCII.GetBytes("GunbondGame");

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

        public void Handshake()
        {
            byte[] message = new byte[20];
            pstr.CopyTo(message, 0);
            reserved.CopyTo(message, pstr.Length);

            byte[] append = { Convert.ToByte(135) };
            append.CopyTo(message, pstr.Length + reserved.Length);

            Send(message);
        }

        public void KeepAlive(int peerID)
        {
            byte[] message = new byte[20];
            pstr.CopyTo(message, 0);
            reserved.CopyTo(message, pstr.Length);

            byte[] append = { Convert.ToByte(135) };
            append.CopyTo(message, pstr.Length + reserved.Length);

            byte[] append2 = BitConverter.GetBytes(peerID);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(append2);
            append2.CopyTo(message, pstr.Length + reserved.Length + append.Length + append2.Length);

            Send(message);
        }
    }
}
