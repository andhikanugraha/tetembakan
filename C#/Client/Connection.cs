using P2PTracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Connection
    {
        Socket m_socClient;
        AsyncCallback pfnWorkerCallBack;
        
        public Connection(string IPAddress)
        {
            //create a new client socket ...
            m_socClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            String szIPSelected = IPAddress;
            String szPort = "3000";
            int alPort = System.Convert.ToInt16(szPort, 10);

            System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(szIPSelected);
            System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, alPort);
            m_socClient.Connect(remoteEndPoint);
            //String szData = "Hello There";
            //Console.WriteLine("Sending message");
            //byte[] byData = System.Text.Encoding.UTF8.GetBytes(szData);
            //m_socClient.Send(byData);
            //Console.WriteLine("Sent");
            //m_socClient.Close();
        }

        public class CSocketPacket
        {
            public System.Net.Sockets.Socket thisSocket;
            public byte[] dataBuffer = new byte[500];
        }

        public void WaitForData(Socket soc)
        {
            try
            {
                if (pfnWorkerCallBack == null)
                {
                    pfnWorkerCallBack = new AsyncCallback(OnDataReceived);
                }
                CSocketPacket theSocPkt = new CSocketPacket();
                theSocPkt.thisSocket = soc;
                // now start to listen for any data...
                soc.BeginReceive(theSocPkt.dataBuffer, 0, theSocPkt.dataBuffer.Length, SocketFlags.None, pfnWorkerCallBack, theSocPkt);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }

        }

        public void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                CSocketPacket theSockId = (CSocketPacket)asyn.AsyncState;
                //end receive...
                int iRx = 0;
                iRx = theSockId.thisSocket.EndReceive(asyn);
                char[] chars = new char[iRx + 1];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(theSockId.dataBuffer, 0, iRx, chars, 0);
                System.String szData = new System.String(chars);
                if (szData != "\0")
                {
                    Console.WriteLine(szData);
                }
                WaitForData(m_socClient);
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nOnDataReceived: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }

        public void sendMessage(Message message)
        {
            m_socClient.Send(message.Payload);
        }

        public void receive()
        {

        }
    }
}
