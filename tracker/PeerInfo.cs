using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace GunbondGame
{
    class PeerInfo
    {
        string peerIP;
        int peerID;
        int port;
        string username;

        public PeerInfo(string peerIP, int peerID, int port, string username)
        {
            this.peerIP = peerIP;
            this.peerID = peerID;
            this.port = port;
            this.username = username;
        }

        public PeerInfo(string peerIP, int peerID)
        {
            this.peerIP = peerIP;
            this.peerID = peerID;
            this.port = 13245;
            this.username = "default";
        }

        public PeerInfo(byte[] peerByte)
        {
            int offset = 0;

            byte[] IPAddress = new byte[4];
            Buffer.BlockCopy(peerByte, offset, IPAddress, 0, 4); offset += 4;

            this.peerIP = (int)IPAddress[0] + "." + (int)IPAddress[1] + "." + (int)IPAddress[2] + "." + (int)IPAddress[3];

            this.peerID = BitConverter.ToInt32(peerByte, offset);
            offset += GameConstant.peerIdSize;

            this.port = BitConverter.ToInt32(peerByte, offset);
            offset += GameConstant.portSize;

            this.username = Encoding.ASCII.GetString(peerByte, offset, GameConstant.usernameSize).Replace("\0", String.Empty).Trim();
        }

        public PeerInfo(PeerInfo peer)
        {
            this.peerIP = peer.peerIP; this.peerID = peer.peerID; this.port = peer.port; this.username = peer.username;
        }

        public string getIP() { return peerIP; }
        public int getID() { return peerID; }
        public int getPort() { return port; }
        public string getUsername() { return username; }

        public void setIP(string peerIP) { this.peerIP = peerIP; }
        public void setID(int peerID) { this.peerID = peerID; }
        public void setPort(int port) { this.port = port; }
        public void setUsername(string username) { this.username = username; }

        public bool Equals(PeerInfo peer)
        {
            if ((this.peerID == peer.peerID) && (this.peerIP == peer.peerIP) && (this.port == peer.port))
            {
                return true;
            }
            return false;
        }

        public byte[] toByte()
        {
            List<byte> peerByte = new List<byte>();
            string[] IPAddress = peerIP.Split('.');
            foreach (string s in IPAddress)
            {
                peerByte.Add((byte)Int16.Parse(s));
            }
            peerByte.AddRange(BitConverter.GetBytes(peerID));
            peerByte.AddRange(BitConverter.GetBytes(port));
            byte[] usernameByte = Encoding.ASCII.GetBytes(username); Array.Resize(ref usernameByte, GameConstant.usernameSize);
            peerByte.AddRange(usernameByte);

            return peerByte.ToArray();
        }

        public override string ToString()
        {
            return this.peerIP + ":" + this.port + ":" + this.peerID + ":" + this.username;
        }
    }
}
