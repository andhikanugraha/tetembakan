using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using System.Timers;

namespace GunbondGame
{
    class Tracker
    {
        struct PeerHandler
        {
            public Tracker tracker;
            public Socket socket;
            public PeerInfo info;
            public bool isAlive;
            public System.Timers.Timer isAliveTimer;

            public PeerHandler(Socket socket, PeerInfo info, Tracker tracker)
            {
                this.tracker = tracker;
                this.socket = socket;
                this.info = info;
                this.isAlive = true;
                this.isAliveTimer = new System.Timers.Timer(GameConstant.connectionTimeOut);
                this.isAliveTimer.Elapsed += new ElapsedEventHandler(dead);
                this.isAliveTimer.Start();
            }

            private void dead(object source, ElapsedEventArgs e)
            {
                tracker.removeDeadPeer(this.info.getID());
                this.isAliveTimer.Stop();
                this.socket = null; this.info = null; this.isAliveTimer = null; this.tracker = null;
            }
        }

        List<PeerHandler> peerList;
        List<Room> roomList;
        Hashtable roomTable; // Key : roomID, Value : handler of creator
        Socket trackerSocket;
        byte[] msgByte = new Byte[1024];
        int maxPeer;
        int maxRoom;
        bool log; bool logKeepAlive;
        peerInfoPools peerInfoPooler;


        public Tracker(int port)
        {
            this.peerList = new List<PeerHandler>();
            this.roomList = new List<Room>();
            this.roomTable = new Hashtable();
            this.maxPeer = GameConstant.defaultMaxPeer;
            this.maxRoom = GameConstant.defaultMaxRoom;
            this.log = true; this.logKeepAlive = true;
            this.peerInfoPooler = new peerInfoPools();
            initialize(port);
        }

        private void initialize(int port)
        {
            try
            {
                trackerSocket = new Socket(AddressFamily.InterNetwork,
                                           SocketType.Stream,
                                           ProtocolType.Tcp);

                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, port);

                trackerSocket.Bind(ipEndPoint);
                trackerSocket.Listen(GameConstant.maxConnectionQueue);
            }
            catch (Exception e)
            {
                Console.WriteLine("initialize exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        public void start()
        {
            try
            {
                trackerSocket.BeginAccept(new AsyncCallback(onAccept), null);
            }
            catch (Exception e)
            {
                Console.WriteLine("start exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        private void onAccept(IAsyncResult ar)
        {
            try
            {
                Socket peerSocket = trackerSocket.EndAccept(ar);

                // Begin accept another incoming connection request
                if (peerList.Count <= maxPeer)
                    trackerSocket.BeginAccept(new AsyncCallback(onAccept), null);

                peerSocket.BeginReceive(msgByte, 0, msgByte.Length, SocketFlags.None,
                    new AsyncCallback(onReceive), peerSocket);
            }
            catch (Exception e)
            {
                Console.WriteLine("onAccept exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        private void onReceive(IAsyncResult ar)
        {
            try
            {
                Socket peerSocket = (Socket)ar.AsyncState;
                peerSocket.EndReceive(ar);

                Message msgReceived = new Message(msgByte);
                Message msgResponse = new Message();
                string logMsg = String.Empty;
                switch (msgReceived.msgType)
                {
                    case MessageType.Handshake:
                        logMsg = MessageType.Handshake.ToString();
                        if (peerList.Count < maxPeer)
                        {
                            string peerIP = ((IPEndPoint)peerSocket.RemoteEndPoint).Address.ToString();
                            int peerID = peerInfoPooler.getAvailableID();
                            int port = peerInfoPooler.getAvailablePort();
                            PeerInfo peerInfo = new PeerInfo(peerIP, peerID, port, msgReceived.username);
                            peerList.Add(new PeerHandler(peerSocket, peerInfo, this));

                            msgResponse = new Message(MessageType.HandshakeResponse, peerInfo);
                        }
                        else
                        {
                            msgResponse = new Message(MessageType.Failed);
                        }
                        break;

                    case MessageType.KeepAlive:
                        logMsg = MessageType.KeepAlive.ToString();
                        msgResponse = new Message(MessageType.KeepAlive, msgReceived.peerID);
                        PeerHandler handler = getHandler(msgReceived.peerID);
                        handler.isAlive = true;
                        handler.isAliveTimer.Stop();
                        handler.isAliveTimer.Start();
                        break;

                    case MessageType.List:
                        logMsg = MessageType.List.ToString();
                        msgResponse = new Message(MessageType.Room, roomList);
                        break;

                    case MessageType.Create:
                        logMsg = MessageType.Create.ToString();
                        if ((roomList.Count < maxRoom) && (!roomTable.ContainsKey(msgReceived.roomID)))
                        {
                            PeerInfo creator = getPeerInfo(peerSocket);
                            this.roomList.Add(new Room(creator, msgReceived.roomID, msgReceived.maxPlayer));
                            this.roomTable.Add(msgReceived.roomID, getHandler(msgReceived.peerID));
                            msgResponse = new Message(MessageType.Success);
                        }
                        else
                        {
                            msgResponse = new Message(MessageType.Failed);
                        }
                        break;

                    case MessageType.Join:
                        logMsg = MessageType.Join.ToString();
                        if (roomTable.ContainsKey(msgReceived.roomID))
                        {
                            if (!isRoomFull(msgReceived.roomID))
                            {
                                msgResponse = new Message(MessageType.JoinSuccess, getRoomByID(msgReceived.roomID));
                            }
                            else
                            {
                                msgResponse = new Message(MessageType.Failed);
                            }
                        }
                        else
                        {
                            msgResponse = new Message(MessageType.Failed);
                        }
                        break;

                    case MessageType.CheckResponse:
                        logMsg = MessageType.CheckResponse.ToString();
                        if (msgReceived.isJoinAccepted)
                        {
                            Room infoSent = getRoomByCreator(getPeerInfo(peerSocket).getID());
                            infoSent.getCreator().setPort(msgReceived.port);
                            msgResponse = new Message(MessageType.JoinSuccess, infoSent);
                        } 
                        else
                            msgResponse = new Message(MessageType.Failed);
                        break;

                    case MessageType.CreatorQuit:
                        logMsg = MessageType.CreatorQuit.ToString();
                        msgResponse = new Message(MessageType.Success);
                        PeerInfo oldCreator = getPeerInfo(peerSocket);
                        if (oldCreator.getID() == msgReceived.peer.getID())
                        {
                            removeRoom(oldCreator);
                        }
                        else
                        {
                            setNewCreator(oldCreator, msgReceived.peer);
                        }
                        break;

                    case MessageType.RoomInfo:
                        logMsg = MessageType.RoomInfo.ToString();
                        updateRoomInfo(msgReceived.room);
                        break;

                    case MessageType.SelfPromoteCreator:
                        logMsg = MessageType.SelfPromoteCreator.ToString();
                        setNewCreator(msgReceived.peer, msgReceived.peer2);
                        break;

                }

                byte[] message = msgResponse.toByte();
                
                if (msgReceived.msgType == MessageType.CheckResponse)
                {
                    Socket joinSocket = getSocket(msgReceived.peerIDJoinRoom);
                    if (joinSocket != null)
                    {
                        joinSocket.BeginSend(message, 0, message.Length, SocketFlags.None,
                                                new AsyncCallback(onSend), joinSocket);
                    }
                }
                else if (msgReceived.msgType != MessageType.RoomInfo)
                {
                    peerSocket.BeginSend(message, 0, message.Length, SocketFlags.None,
                                        new AsyncCallback(onSend), peerSocket);
                }
                
                Array.Clear(msgByte, 0, msgByte.Length);
                peerSocket.BeginReceive(msgByte, 0, msgByte.Length, SocketFlags.None, 
                                            new AsyncCallback(onReceive), peerSocket);

                PeerInfo logInfo = getPeerInfo(peerSocket);

                if (logMsg == MessageType.KeepAlive.ToString() && !logKeepAlive) { }
                else
                if (logInfo != null && log)
                {
                    if (logMsg != MessageType.RoomInfo.ToString())
                        if (logMsg == MessageType.Handshake.ToString())
                            System.Console.WriteLine("{0} has been connected to tracker", logInfo);
                        System.Console.WriteLine("{0} : {1}", logInfo, logMsg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("onReceive exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        private void onSend(IAsyncResult ar)
        {
            try
            {
                Socket peer = (Socket)ar.AsyncState;
                peer.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine("onSend exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        private PeerInfo getPeerInfo(Socket search)
        {
            foreach (PeerHandler handler in peerList)
            {
                if (handler.socket == search)
                {
                    return handler.info;
                }
            }
            return null;
        }

        private Socket getSocket(int peerID)
        {
            foreach (PeerHandler handler in peerList)
            {
                if (handler.info.getID() == peerID)
                    return handler.socket;
            }
            return null;
        }
        
        private Room getRoomByCreator(int creatorID)
        {
            foreach (Room room in roomList)
            {
                if (room.getCreator().getID() == creatorID)
                {
                    return room;
                }
            }
            return null;
        }

        private Room getRoomByID(string roomID)
        {
            foreach (Room room in roomList)
            {
                if (room.getRoomID() == roomID)
                {
                    return room;
                }
            }
            return null;
        }

        private PeerHandler getHandler(PeerInfo peer)
        {
            foreach (PeerHandler handler in peerList)
            {
                if (handler.info.getID() == peer.getID())
                {
                    return handler;
                }
            }
            return new PeerHandler();
        }

        private PeerHandler getHandler(int peerID)
        {
            foreach (PeerHandler handler in peerList)
            {
                if (handler.info.getID() == peerID)
                {
                    return handler;
                }
            }
            return new PeerHandler();
        }

        private void removeRoom(PeerInfo creator)
        {
            foreach (Room room in roomList)
            {
                if (room.getCreator().getID() == creator.getID())
                {
                    roomList.Remove(room);
                    roomTable.Remove(room.getRoomID());
                    return;
                }
            }
        }

        private void setNewCreator(PeerInfo oldCreator, PeerInfo newCreator)
        {
            foreach (Room room in roomList)
            {
                if (room.getCreator().getID() == oldCreator.getID())
                {
                    room.setCreator(new PeerInfo(newCreator));
                    room.getConnectedPeers().Clear();
                    room.addPlayer(room.getCreator());
                    roomTable[room.getRoomID()] = getHandler(room.getCreator());
                    return;
                }
            }
        }

        private void removeDeadPeer(int deadPeer)
        {
            if (peerList.Count < maxPeer)
            {
                trackerSocket.BeginAccept(new AsyncCallback(onAccept), null);
            }

            foreach (PeerHandler peer in peerList)
            {
                if (peer.info.getID() == deadPeer)
                {
                    System.Console.WriteLine("{0} has been disconnected from tracker", peer.info);
                    peerList.Remove(peer);
                    peerInfoPooler.releaseID(deadPeer, peer.info.getPort());
                    break;
                }
            }

            foreach (Room room in roomList)
            {
                if (room.getCreator().getID() == deadPeer)
                {
                    if (room.getConnectedPeers().Count == 1)
                    {
                        roomTable.Remove(room.getRoomID());
                        roomList.Remove(room);
                        return;
                    }
                    else
                    {
                        setNewCreator(room.getCreator(), room.getConnectedPeers()[1]);
                        byte[] msg = new Message(MessageType.CreatorQuit, room.getConnectedPeers()[1]).toByte();
                        for (int i = 1; i < room.getConnectedPeers().Count; i++)
                        {
                            getSocket(room.getConnectedPeers()[i].getID()).
                                BeginSend(msg, 0, msg.Length, SocketFlags.None, 
                                new AsyncCallback(onSend), getSocket(room.getConnectedPeers()[i].getID()));
                        }
                    }
                }
            }

        }

        private void updateRoomInfo(Room room)
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].getRoomID() == room.getRoomID())
                {
                    roomList[i] = new Room(room);
                    return;
                }
            }
        }

        private bool isRoomFull(string roomID)
        {
            foreach (Room room in roomList)
            {
                if (room.getRoomID() == roomID)
                {
                    if (room.getConnectedPeers().Count < room.getMaxPlayer())
                        return false;
                    else
                        return true;
                }
            }
            return true;
        }

        public List<Room> getRoomList() { return roomList; }
        public int getMaxPeer() { return maxPeer; }
        public int getMaxRoom() { return maxRoom; }
        public void setMaxPeer(int maxPeer) 
        {
            if (peerList.Count < maxPeer)
            {
                if (peerList.Count + 1 == this.maxPeer)
                {
                    trackerSocket.BeginAccept(new AsyncCallback(onAccept), null);
                }
                this.maxPeer = maxPeer;
                System.Console.WriteLine("max_peer set to {0}", maxPeer);
            }
            else if (peerList.Count == maxPeer)
            {
                this.maxPeer = maxPeer;
                System.Console.WriteLine("max_peer set to {0}", maxPeer);
            }
            else
            {
                System.Console.WriteLine("Failed set max peer to {0}", maxPeer);
                System.Console.WriteLine("{0} peers have already been connected to tracker", peerList.Count);
            }
        }
        public void setMaxRoom(int maxRoom)
        {
            if (roomList.Count <= maxRoom)
            {
                this.maxRoom = maxRoom;
                System.Console.WriteLine("max_room set to {0}", maxRoom);
            }
            else
            {
                System.Console.WriteLine("Failed set max room to {0}", maxRoom);
                System.Console.WriteLine("{0} rooms currently active now", roomList.Count);
            }
            
        }
        public void setLog(bool log) { this.log = log; }
        public void setLogKeepAlive(bool logKeepAlive) { this.logKeepAlive = logKeepAlive; }

        static void Main(string[] args)
        {
            Tracker tracker = new Tracker(GameConstant.trackerPort);
            tracker.start();

            string cmd = String.Empty;
            string[] cmds;
            System.Console.WriteLine("GunbondGame Tracker v1.0");
            while (cmd != "shutdown")
            {
                System.Console.Write(">");
                cmd = System.Console.ReadLine();
                cmds = Regex.Split(cmd, " ");
                if (cmds[0] == "max_peer")
                {
                    tracker.setMaxPeer(Convert.ToInt32(cmds[1]));
                }
                if (cmds[0] == "max_room")
                {
                    tracker.setMaxRoom(Convert.ToInt32(cmds[1]));
                }
                if (cmds[0] == "log")
                {
                    if (cmds[1] == "on") tracker.setLog(true);
                    else if (cmds[1] == "off") tracker.setLog(false);
                    else if (cmds[1] == "keepalive")
                    {
                        if (cmds[2] == "on") tracker.setLogKeepAlive(true);
                        else if (cmds[2] == "off") tracker.setLogKeepAlive(false);
                    }
                }
                if (cmds[0] == "room_list")
                {
                    System.Console.WriteLine("ROOM LIST");
                    foreach (Room room in tracker.getRoomList())
                    {
                        System.Console.WriteLine(room.toStringWithoutPeers());
                    }
                }
                if (cmds[0] == "peer_list")
                {
                    System.Console.WriteLine("PEER LIST");
                    foreach (PeerHandler peer in tracker.peerList)
                    {
                        System.Console.WriteLine(peer.info);
                    }
                }
                if (cmds[0] == "room_list_full")
                {
                    System.Console.WriteLine("ROOM LIST");
                    foreach (Room room in tracker.getRoomList())
                    {
                        System.Console.WriteLine(room);
                    }
                }
            }
        }
    }
}
