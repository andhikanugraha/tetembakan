using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GunbondGame
{
    class Room
    {
        PeerInfo creator;
        string roomID;
        int maxPlayer;
        List<PeerInfo> connectedPeers;

        public Room(PeerInfo creator, string roomID, int maxPlayer)
        {
            this.creator = creator;
            this.roomID = roomID;
            this.maxPlayer = maxPlayer;
            this.connectedPeers = new List<PeerInfo>(maxPlayer);
            addPlayer(creator);
        }

        public Room(PeerInfo creator, string roomID, int maxPlayer, List<PeerInfo> connectedPeers)
        {
            this.creator = creator;
            this.roomID = roomID;
            this.maxPlayer = maxPlayer;
            this.connectedPeers = connectedPeers;
        }

        // Concstruct Room Object from ByteArray Data
        public Room(byte[] roomByte)
        {
            int offset = 0;
            int roomIDLength = BitConverter.ToInt32(roomByte, offset); offset += 4;
            this.roomID = Encoding.ASCII.GetString(roomByte, offset, roomIDLength); offset += roomIDLength;
            byte[] creator = new byte[GameConstant.peerInfoSize];
            Buffer.BlockCopy(roomByte, offset, creator, 0, GameConstant.peerInfoSize);
            this.creator = new PeerInfo(creator);
            offset += GameConstant.peerInfoSize;
            int maxPlayer = BitConverter.ToInt32(roomByte, offset); offset += GameConstant.maxPlayerSize;
            this.maxPlayer = maxPlayer;
            int connectedPeersCount = BitConverter.ToInt32(roomByte, offset); offset += 4;
            this.connectedPeers = new List<PeerInfo>(connectedPeersCount);
            for (int i = 0; i < connectedPeersCount; i++)
            {
                byte[] peer = new byte[GameConstant.peerInfoSize];
                Buffer.BlockCopy(roomByte, offset, peer, 0, GameConstant.peerInfoSize);
                offset += GameConstant.peerInfoSize;
                this.connectedPeers.Add(new PeerInfo(peer));
            }

        }

        // Copy constructor
        public Room(Room room)
        {
            this.creator = new PeerInfo(room.creator.getIP(), room.creator.getID(), room.creator.getPort(), room.creator.getUsername());
            this.roomID = room.roomID;
            this.maxPlayer = room.maxPlayer;
            this.connectedPeers = new List<PeerInfo>();
            foreach (PeerInfo peer in room.connectedPeers)
            {
                this.connectedPeers.Add(new PeerInfo(peer.getIP(), peer.getID(), peer.getPort(), peer.getUsername()));
            }
        }

        public PeerInfo getCreator() { return creator; }
        public string getRoomID() { return roomID; }
        public int getMaxPlayer() { return maxPlayer; }
        public List<PeerInfo> getConnectedPeers() { return connectedPeers; }

        public void setCreator(PeerInfo creator) { this.creator = creator; }
        public void setRoomID(string roomID) { this.roomID = roomID; }
        public void setMaxPlayer(int maxPlayer) { this.maxPlayer = maxPlayer; }

        public void addPlayer(PeerInfo newPlayer)
        {
            connectedPeers.Add(newPlayer);
        }

        public void removePlayer(int playerID)
        {
            foreach (PeerInfo peer in connectedPeers)
            {
                if (peer.getID() == playerID)
                {
                    connectedPeers.Remove(peer);
                    return;
                }
            }
        }

        public void removeAllPlayer() { this.connectedPeers.Clear(); }

        // Convert this ROOM to ByteArray
        public byte[] toByte()
        {
            List<byte> roomByte = new List<byte>();
            byte[] roomIDByte = Encoding.ASCII.GetBytes(this.roomID);
            roomByte.AddRange(BitConverter.GetBytes(roomIDByte.Length));
            roomByte.AddRange(roomIDByte);
            roomByte.AddRange(this.creator.toByte());
            roomByte.AddRange(BitConverter.GetBytes(maxPlayer));
            roomByte.AddRange(BitConverter.GetBytes(connectedPeers.Count));
            foreach (PeerInfo info in connectedPeers)
            {
                roomByte.AddRange(info.toByte());
            }

            return roomByte.ToArray();
        }

        public override string ToString()
        {
            string s = "\n------------------------------\n";
            s += "RoomID = " + roomID + " [Player = " + connectedPeers.Count + "/" +  maxPlayer + "]\n";
            s += "Creator = " + creator.ToString() + "\n\n";
            s += "Connected Peers:\n";
            int i = 1;
            foreach (PeerInfo peer in connectedPeers)
            {
                s += "Peer #" + i + ": " + peer.ToString() + "\n";
                i++;
            }
            s += "------------------------------";
            return s;
        }

        public string toStringWithoutPeers()
        {
            string s = "------------------------------\n";
            s += "RoomID = " + roomID + " [MaxPlayer = " + maxPlayer + "]\n";
            s += "Creator = " + creator.ToString() + "\n";
            s += "------------------------------";
            return s;
        }
    }
}
