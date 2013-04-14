using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections;
using MessageFormat;
using System.Diagnostics;

namespace MainTracker
{
    class Server
    {
        private TcpListener tcpListener;
        private Thread listenThread;

        private ArrayList peers;
        private ArrayList rooms;
        const int cx = 0;
        private int lastPeerID = 110;
        public int max_peer = 100;
        public int max_room = 100;
        public bool log = true;
        private int findPeer(int peer_id)
        {
            for (int i = 0; i < peers.Count; i++)
            {
                Peer p = peers[i] as Peer;
                if (p.peer_id == peer_id)
                    Console.WriteLine("peerID: " + peer_id);
                    return i;
            }
            return -1;
        }

        private int findRoom(String room_id)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                Room r = rooms[i] as Room;
                if (r.room_id == room_id)
                    Console.WriteLine("roomID: " + room_id);
                    return i;
            }
            return -1;
        }

        private int findRoom(int peer_id)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                Room r = rooms[i] as Room;
                foreach (Peer p in r.neighbor)
                {
                    Debug.WriteLine("peer_id room{0}: " + p.peer_id, i);
                        
                    if (p.peer_id == peer_id)
                    {
                        //Debug.WriteLine("peer_id: " + peer_id);
                        Debug.WriteLine("i: " + i);
                        return i;
                    }
                }
            }

            return -1;
        }

        private int countRoom()
        {
            Console.WriteLine("room count: " + rooms.Count);
            return rooms.Count;
        }

        private bool joinRoom(String room_id, int peer_id)
        {
            int pindex = findPeer(peer_id);
            int rindex = findRoom(room_id);
            Console.WriteLine("peerindex: " + pindex + "\nroomindex: " + rindex);
            if (pindex == -1 || rindex == -1)
                return false;
            else
            {
                Room r = rooms[rindex] as Room;
                Peer p = peers[pindex] as Peer;
                if (r.neighbor.Count == r.maxplayer)
                    return false;
                if (r.findNeighbor(peer_id) != -1)
                    return false;
                r.neighbor.Add(p);
                return true;
            }
        }

        public Server()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, 3000);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            Console.WriteLine("Thread created");
            this.listenThread.Start();
            peers = new ArrayList();
            rooms = new ArrayList();
        }
        private void ListenForClients()
        {
            this.tcpListener.Start();

            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = this.tcpListener.AcceptTcpClient();

                //create a thread to handle communication 
                //with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                Console.WriteLine("Client connected");
                clientThread.Start(client);
            }
        }
        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            tcpClient.ReceiveTimeout = 10000000;

            byte[] message = new byte[4096];
            int bytesRead;
            //clientStream.ReadTimeout = 1000;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();
                switch (Message.getCode(message))
                {
                    case Message.HANDSHAKE_CODE: // 135
                        if (peers.Count == max_peer)
                        {
                            if (log)
                                Console.WriteLine("Handshake failed : max_peer reached");
                            byte[] buff = Message.Failed();
                            clientStream.Write(buff, 0, buff.Length);
                            clientStream.Flush();
                        }
                        else
                        {
                            Peer p = new Peer();
                            p.peer_id = lastPeerID;
                            peers.Add(p);
                            byte[] byte_peer_id = BitConverter.GetBytes(lastPeerID);
                            lastPeerID++;
                            if (log)
                                Console.WriteLine("Handshake success : peer " + p.peer_id);
                            byte[] buff = Message.HandShakeResponse(p.peer_id);
                            clientStream.Write(buff, 0, buff.Length);
                            clientStream.Flush();
                        }
                        break;
                    case Message.CREATE_CODE: // 255
                        int peerid = Message.getPeerId(message);
                        int maxplayer = Message.getMaxPlayer(message);
                        String roomid = Message.getRoomId2(message);
                        if (findPeer(peerid) != -1 && findRoom(roomid) == -1 && rooms.Count < max_room)
                        {
                            Room room = new Room();
                            room.peer_id = peerid;
                            room.room_id = roomid;
                            room.maxplayer = maxplayer;
                            room.neighbor = new ArrayList();
                            Peer p2 = peers[findPeer(peerid)] as Peer;
                            room.neighbor.Add(p2);
                            rooms.Add(room);
                            if (log)
                                Console.WriteLine("Room creation success : peer " + peerid + " room " + roomid);
                            byte[] buff = Message.Success();
                            clientStream.Write(buff, 0, buff.Length);
                            clientStream.Flush();
                        }
                        else
                        {
                            if (log)
                                Console.WriteLine("Room creation failed");
                            byte[] buff = Message.Failed();
                            clientStream.Write(buff, 0, buff.Length);
                            clientStream.Flush();
                        }
                        break;
                    case Message.LIST_CODE:// 254
                        int peerid2 = Message.getPeerId(message);
                        if (log)
                            Console.WriteLine("Room listed for : peer " + peerid2);
                        byte[] buff2 = Message.Room(rooms);
                        clientStream.Write(buff2, 0, buff2.Length);
                        clientStream.Flush();
                        break;
                    case Message.JOIN_CODE:// 253
                        int peerid3 = Message.getPeerId(message);
                        String roomid3 = Message.getRoomId(message);
                        if (joinRoom(roomid3, peerid3))
                        {
                            if (log)
                                Console.WriteLine("Room join success : peer " + peerid3 + " room " + roomid3);
                            byte[] buff = Message.Success();
                            clientStream.Write(buff, 0, buff.Length);
                            clientStream.Flush();
                        }
                        else
                        {
                            if (log)
                                Console.WriteLine("Room join failed : peer " + peerid3 + " room " + roomid3);
                            byte[] buff = Message.Failed();
                            clientStream.Write(buff, 0, buff.Length);
                            clientStream.Flush();
                        }
                        break;
                    case Message.START_CODE:// 252
                        int peerid4 = Message.getPeerId(message);
                        String roomid4 = Message.getRoomId(message);
                        int roomidx = findRoom(roomid4);
                        if (roomidx == -1)
                        {
                            if (log)
                                Console.WriteLine("Room start failed : room not found. peer " + peerid4 + " room " + roomid4);
                            byte[] buff = Message.Failed();
                            clientStream.Write(buff, 0, buff.Length);
                            clientStream.Flush();
                        }
                        else
                        {
                            Room r4 = rooms[roomidx] as Room;
                            if (r4.peer_id != peerid4)
                            {
                                if (log)
                                    Console.WriteLine("Room start failed : peer is not master. peer " + peerid4 + " room " + roomid4);
                                byte[] buff = Message.Failed();
                                clientStream.Write(buff, 0, buff.Length);
                                clientStream.Flush();
                            }
                            else
                            {
                                rooms.RemoveAt(roomidx);
                                if (log)
                                    Console.WriteLine("Room start success peer " + peerid4 + " room " + roomid4);
                                byte[] buff = Message.Success();
                                clientStream.Write(buff, 0, buff.Length);
                                clientStream.Flush();
                            }
                        }
                        break;
                    case Message.QUIT_CODE:// 235
                        int peerid5 = Message.getPeerId(message);
                        int roomidx5 = findRoom(peerid5);
                        if (roomidx5 == -1)
                        {
                            if (log)
                                Console.WriteLine("Room quit failed : room not found. peer " + peerid5);// + " room " + roomid5);
                            byte[] buff = Message.Failed();
                            clientStream.Write(buff, 0, buff.Length);
                            clientStream.Flush();
                        }
                        else
                        {
                            Room r5 = rooms[roomidx5] as Room;
                            if (r5.findNeighbor(peerid5) == -1)
                            {
                                if (log)
                                    Console.WriteLine("Room quit failed : peer hasn't joined yet. peer " + peerid5);// + " room " + roomid5);
                                byte[] buff = Message.Failed();
                                clientStream.Write(buff, 0, buff.Length);
                                clientStream.Flush();
                            }
                            else
                            {
                                r5.neighbor.RemoveAt(r5.findNeighbor(peerid5));
                                if (log)
                                    Console.WriteLine("Room quit success. peer " + peerid5);// + " room " + roomid5);
                                byte[] buff = Message.Success();
                                clientStream.Write(buff, 0, buff.Length);
                                clientStream.Flush();
                            }
                        }
                        break;
                }
                if (log)
                    Console.WriteLine("Message = " + encoder.GetString(message, 0, bytesRead));
                byte[] buff7 = Message.KeepAlive(Message.getPeerId(message));
                clientStream.Write(buff7, 0, buff7.Length);
                clientStream.Flush();
            }

            tcpClient.Close();
        }
    }
}
