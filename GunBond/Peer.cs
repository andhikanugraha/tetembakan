using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
//using System.Windows.Threading;
using System.Timers;

namespace GunBond
{

    enum GameState
    {
        Connect,
        Lobby,
        Room,
        Reconnect
    }
    class Peer
    {
        /* THIS peer information */
        bool isCreator;
        PeerInfo peerInfo;
        Room currentRoom; // Peer draw room method variable
        GameState state;
		Game1 game;

        /* Peer to Tracker communication */
        Socket peerToTrackerSocket;
        MessageType lastMessage;
        byte[] msgByte = new byte[1024];
        System.Timers.Timer keepAliveSender;
        System.Timers.Timer keepAliveResponse;

        List<int>[] cubenode = new List<int>[9];
        
        /* Peer to peer communication */
        struct PeerHandler
        {
            public Peer peer;
            public Socket socket;
            public PeerInfo info;
            public bool isAlive;
            public System.Timers.Timer isAliveTimer;

            public PeerHandler(Socket socket, PeerInfo info, Peer peer)
            {
                this.peer = peer;
                this.socket = socket;
                this.info = info;
                this.isAlive = true;
                this.isAliveTimer = new System.Timers.Timer(GameConstant.connectionTimeOut);
                this.isAliveTimer.Elapsed += new ElapsedEventHandler(dead);
                this.isAliveTimer.Start();
            }

            private void dead(object source, ElapsedEventArgs e)
            {
                peer.removeDeadPeer(this.info.getID());
                this.isAliveTimer.Stop();
                this.socket = null; this.info = null; this.isAliveTimer = null; this.peer = null;
            }
        }
        List<PeerHandler> peerList;
        Queue<PeerInfo> roomRequester;
        Socket peerToPeerSocketServer;
        Socket peerToPeerSocketClient;
        byte[] msgByteP2PServer = new byte[1024];
        byte[] msgByteP2PClient = new byte[1024];
        MessageType lastMessageClient;
        System.Timers.Timer keepAliveSenderCreator;
        System.Timers.Timer keepAliveResponseCreator;

        public Peer()
        {
            this.state = GameState.Connect;
            this.isCreator = false;
            this.peerInfo = new PeerInfo(IPAddress.Loopback.ToString(), -1);
            this.lastMessage = MessageType.Null;
            this.currentRoom = null;
            this.roomRequester = new Queue<PeerInfo>();
            this.lastMessageClient = MessageType.Null;
            this.keepAliveSender = new System.Timers.Timer(GameConstant.keepAliveInterval);
            this.keepAliveSender.Elapsed += new ElapsedEventHandler(sendKeepAlive);
            this.keepAliveSenderCreator = new System.Timers.Timer(GameConstant.keepAliveInterval);
            this.keepAliveSenderCreator.Elapsed += new ElapsedEventHandler(sendKeepAliveCreator);
            this.keepAliveResponse = new System.Timers.Timer(GameConstant.connectionTimeOut);
            this.keepAliveResponse.Elapsed += new ElapsedEventHandler(trackerDown);
            this.keepAliveResponseCreator = new System.Timers.Timer(GameConstant.connectionTimeOut);
            this.keepAliveResponseCreator.Elapsed += new ElapsedEventHandler(creatorDown);

            for (int i = 1; i <= 8; i++ )
            {
                cubenode[i] = new List<int>();
            }
            cubenode[1].Add(2); cubenode[1].Add(4); cubenode[1].Add(5); cubenode[1].Add(7);
            cubenode[2].Add(1); cubenode[2].Add(3); cubenode[2].Add(6); cubenode[2].Add(8);
            cubenode[3].Add(2); cubenode[3].Add(4); cubenode[3].Add(5); cubenode[3].Add(7);
            cubenode[4].Add(1); cubenode[4].Add(3); cubenode[4].Add(6); cubenode[4].Add(8);
            cubenode[5].Add(1); cubenode[5].Add(3); cubenode[5].Add(6); cubenode[5].Add(8);
            cubenode[6].Add(2); cubenode[6].Add(4); cubenode[6].Add(5); cubenode[6].Add(7);
            cubenode[7].Add(1); cubenode[7].Add(3); cubenode[7].Add(6); cubenode[7].Add(8);
            cubenode[8].Add(2); cubenode[8].Add(4); cubenode[8].Add(5); cubenode[8].Add(7);
            
            // there is a reason that takes long sentences to explain why I passed random object here
            // instead of creating new random object when needed.
        }

        public void connectToTracker(string trackerIPAddress, string username)
        {
            try
            {
                peerToTrackerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(trackerIPAddress), GameConstant.trackerPort);

                peerToTrackerSocket.BeginConnect(ipEndPoint, new AsyncCallback(onConnectTracker), username);
            }
            catch (Exception e)
            {
                Console.WriteLine("connectToTracker exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        public void connectToCreator(string creatorIPAddress, int creatorPort)
        {
            try
            {
                peerToPeerSocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(creatorIPAddress), creatorPort);

                peerToPeerSocketClient.BeginConnect(ipEndPoint, new AsyncCallback(onConnectPeerClient), null);
            }
            catch (Exception e)
            {
                Console.WriteLine("connectToCreator exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        private void onConnectTracker(IAsyncResult ar)
        {
            try
            {
                string username = (string)ar.AsyncState;
                peerToTrackerSocket.EndConnect(ar);

                byte[] msg = new Message(MessageType.Handshake, username).toByte();

                peerToTrackerSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(onSendTracker), null);
                peerToTrackerSocket.BeginReceive(msgByte, 0, msgByte.Length, SocketFlags.None, new AsyncCallback(onReceiveTracker), null);
            }
            catch (Exception e)
            {
                Console.WriteLine("onConnectTracker exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        private void onConnectPeerClient(IAsyncResult ar)
        {
            try
            {
                peerToPeerSocketClient.EndConnect(ar);
                Message msg = new Message(MessageType.HandshakeCreator, this.peerInfo);
                byte[] message = msg.toByte();

                peerToPeerSocketClient.BeginSend(message, 0, message.Length, SocketFlags.None,
                                                    new AsyncCallback(onSendPeerClient), null);

                peerToPeerSocketClient.BeginReceive(msgByteP2PClient, 0, msgByteP2PClient.Length, SocketFlags.None,
                                                        new AsyncCallback(onReceivePeerClient), null);
                this.keepAliveSenderCreator.Start();
                this.keepAliveResponseCreator.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("onConnectPeerClient exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        private void onSendTracker(IAsyncResult ar)
        {
            try
            {
                peerToTrackerSocket.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine("onSendTracker exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        private void onSendPeerClient(IAsyncResult ar)
        {
            try
            {
                peerToPeerSocketClient.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine("onSendPeerClient exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        private void onReceiveTracker(IAsyncResult ar)
        {
            try
            {
                peerToTrackerSocket.EndReceive(ar);

                Message msgReceived = new Message(msgByte);
                Message msgResponse = new Message();
                byte[] message;
                switch (msgReceived.msgType)
                {
                    case MessageType.HandshakeResponse:
                        this.peerInfo = new PeerInfo(msgReceived.peer);
                        this.keepAliveSender.Start();
                        this.keepAliveResponse.Start();
                        gotoLobby();
                        break;

                    case MessageType.KeepAlive:
                        this.keepAliveResponse.Stop();
                        this.keepAliveResponse.Start();
                        break;

                    case MessageType.Room:
                        drawLobby(msgReceived.roomList);
                        this.state = GameState.Lobby;
                        break;

                    case MessageType.Success:
                        switch (lastMessage)
                        {
                            case MessageType.Create:
                                isCreator = true;
                                drawRoom();
                                this.state = GameState.Room;
                                break;

                            case MessageType.CreatorQuit:
                                gotoLobby();
                                break;
                        }
                        break;

                    case MessageType.Failed:
                        switch (lastMessage)
                        {
                            case MessageType.Create:
                                System.Console.WriteLine("Failed to create room");
                                System.Console.WriteLine("Maximum number of room has been reached or roomID already exists");
                                break;

                            case MessageType.Join:
                                System.Console.WriteLine("Failed to join room");
                                System.Console.WriteLine("Maximum number of player has been reached or room not found");
                                break;
                        }
                        break;

                    case MessageType.Check:
                        if (currentRoom.getConnectedPeers().Count < currentRoom.getMaxPlayer())
                        {
                            msgResponse = new Message(MessageType.CheckResponse, msgReceived.peerIDJoinRoom, true, this.peerInfo.getPort());
                        }
                        else
                        {
                            msgResponse = new Message(MessageType.CheckResponse, msgReceived.peerIDJoinRoom, false, this.peerInfo.getPort());
                        }
                        message = msgResponse.toByte();
                        peerToTrackerSocket.BeginSend(message, 0, message.Length, SocketFlags.None,
                                                        new AsyncCallback(onSendTracker), null);
                        break;

                    case MessageType.JoinSuccess:
                        connectToCreator(msgReceived.room.getCreator().getIP(), msgReceived.room.getCreator().getPort());
                        break;
                    
                    case MessageType.CreatorQuit:
                        if (this.peerInfo.getID() == msgReceived.peer.getID())
                        {
                            this.currentRoom.setCreator(this.peerInfo);
                            this.currentRoom.getConnectedPeers().Clear();
                            this.currentRoom.getConnectedPeers().Add(this.peerInfo);
                            this.isCreator = true;
                            this.peerList = new List<PeerHandler>();
                            initializeP2PConnection();
                        }
                        else
                        {
                            this.currentRoom.setCreator(new PeerInfo(msgReceived.peer));
                            connectToCreator(msgReceived.peer.getIP(), msgReceived.peer.getPort());
                        }
                        return;
                }
                Array.Clear(msgByte, 0, msgByte.Length);
                peerToTrackerSocket.BeginReceive(msgByte, 0, msgByte.Length, SocketFlags.None,
                                                    new AsyncCallback(onReceiveTracker), null);
            }
            catch (Exception e)
            {
                Console.WriteLine("onReceiveTracker exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        public void initializeP2PConnection()
        {
            try
            {
                Console.WriteLine("helloooo");
                peerToPeerSocketServer = new Socket(AddressFamily.InterNetwork,
                                              SocketType.Stream,
                                              ProtocolType.Tcp);

                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, peerInfo.getPort());
                peerToPeerSocketServer.Bind(ipEndPoint);
                peerToPeerSocketServer.Listen(GameConstant.maxConnectionQueue);

                peerToPeerSocketServer.BeginAccept(new AsyncCallback(onAcceptPeerServer), null);
            }
            catch (Exception e)
            {
                Console.WriteLine("initializeP2PConnection exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        public void onAcceptPeerServer(IAsyncResult ar)
        {
            try
            {
                Socket peerSocket = peerToPeerSocketServer.EndAccept(ar);

                peerToPeerSocketServer.BeginAccept(new AsyncCallback(onAcceptPeerServer), null);

                peerSocket.BeginReceive(msgByteP2PServer, 0, msgByteP2PServer.Length, SocketFlags.None,
                                            new AsyncCallback(onReceivePeerServer), peerSocket);

            }
            catch (Exception e)
            {
                Console.WriteLine("onAcceptPeerServer exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        private void onReceivePeerServer(IAsyncResult ar)
        {
            try
            {
                Socket peerSocket = (Socket)ar.AsyncState;
                peerSocket.EndReceive(ar);

                Message msgReceived = new Message(msgByteP2PServer);
                Message msgResponse = new Message();

                switch (msgReceived.msgType)
                {
                    case MessageType.HandshakeCreator:
                        PeerInfo newPlayer = new PeerInfo(msgReceived.peer);
                        this.peerList.Add(new PeerHandler(peerSocket, newPlayer, this));
                        this.currentRoom.addPlayer(newPlayer);
                        
                        msgResponse = new Message(MessageType.RoomInfo, this.currentRoom);
                        break;

                    case MessageType.KeepAlive:
                        msgResponse = new Message(MessageType.KeepAlive, msgReceived.peerID);
                        PeerHandler handler = getHandler(msgReceived.peerID);
                        handler.isAlive = true;
                        handler.isAliveTimer.Stop();
                        handler.isAliveTimer.Start();
                        break;

                    case MessageType.GameUpdate:
                        msgResponse = new Message(MessageType.GameUpdate, msgReceived.timestamp, msgReceived.gameUpdate);
                        foreach (PeerInfo peer in this.currentRoom.getConnectedPeers())
                        {
							if (cubenode[this.currentRoom.getIDOnRoom(this.peerInfo.getID())].Contains(this.currentRoom.getIDOnRoom(peer.getID())))
                                System.Console.WriteLine(peer.ToString() + " -> " + this.peerInfo.ToString() + " : " + msgReceived.timestamp + msgReceived.gameUpdate.ToString());
                        }
                        break;

                    case MessageType.Quit:
                        msgResponse = new Message(MessageType.Success);
                        this.currentRoom.removePlayer(msgReceived.peerID);
                        removePeerHandler(msgReceived.peerID);
                        break;
                }

                byte[] message = msgResponse.toByte();
                peerSocket.BeginSend(message, 0, message.Length, SocketFlags.None,
                                        new AsyncCallback(onSendPeerServer), peerSocket);
                
                if (msgReceived.msgType != MessageType.Quit) {
                    peerSocket.BeginReceive(msgByteP2PServer, 0, msgByteP2PServer.Length, SocketFlags.None,
                                                new AsyncCallback(onReceivePeerServer), peerSocket);
                }

                if (msgReceived.msgType == MessageType.Quit || msgReceived.msgType == MessageType.HandshakeCreator)
                {
                    byte[] m = new Message(MessageType.RoomInfo, this.currentRoom).toByte();
                    foreach (PeerHandler peer in peerList)
                    {
                        if (peer.socket != peerSocket)
                        {
                            peer.socket.BeginSend(m, 0, m.Length, SocketFlags.None, new AsyncCallback(onSendPeerServer), peer.socket);
                        }
                    }
                    
                    byte[] msgTracker = new Message(MessageType.RoomInfo, this.currentRoom).toByte();
                    peerToTrackerSocket.BeginSend(msgTracker, 0, msgTracker.Length, SocketFlags.None,
                                                    new AsyncCallback(onSendTracker), null);
                    drawRoom();
                }
                if (msgReceived.msgType == MessageType.GameUpdate)
                {
                    byte[] msg = new Message(MessageType.GameUpdate, msgReceived.timestamp, msgReceived.gameUpdate).toByte();
                    foreach (PeerHandler peer in peerList)
                    {
                        if (peer.socket != peerSocket)
                        {
                            peer.socket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(onSendPeerServer), peer.socket);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("onReceivePeerServer exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        private void onReceivePeerClient(IAsyncResult ar)
        {
            try
            {
                peerToPeerSocketClient.EndReceive(ar);

                Message msgReceived = new Message(msgByteP2PClient);
                switch (msgReceived.msgType)
                {
                    case MessageType.RoomInfo:
                        this.currentRoom = new Room(msgReceived.room);
                        drawRoom();
                        this.state = GameState.Room;
                        break;

                    case MessageType.KeepAlive:
                        this.keepAliveResponseCreator.Stop();
                        this.keepAliveResponseCreator.Start();
                        break;

                    case MessageType.Success:
                        if (this.lastMessageClient == MessageType.Quit)
                        {
                            gotoLobby();
                            this.currentRoom = null;
                            this.state = GameState.Lobby;
                        }
                        break;

                    case MessageType.CreatorQuit:
                        if (this.peerInfo.getID() == msgReceived.peer.getID())
                        {
                            this.currentRoom.setCreator(this.peerInfo);
                            bool hitoriBochi = false;
                            if (this.currentRoom.getConnectedPeers().Count == 2) hitoriBochi = true;
                            this.currentRoom.getConnectedPeers().Clear();
                            this.currentRoom.getConnectedPeers().Add(this.peerInfo);
                            this.isCreator = true;
                            this.peerList = new List<PeerHandler>();
                            if (hitoriBochi) drawRoom();
                        }
                        else
                        {
                            this.currentRoom.setCreator(new PeerInfo(msgReceived.peer));
                            connectToCreator(msgReceived.peer.getIP(), msgReceived.peer.getPort());
                        }
                        break;

                    case MessageType.Start:
                        Console.WriteLine("Starting game...\nMembers : \n");
                        foreach(PeerInfo peer in this.currentRoom.getConnectedPeers()) {
                            Console.WriteLine(peer.getID());
                            Console.WriteLine(peer.getIP());
                            Console.WriteLine(peer.getUsername());
						}
						Thread t = new Thread( () => {
						Thread.CurrentThread.IsBackground = true;
						game = new Game1 (this.currentRoom.getIDOnRoom(this.peerInfo.getID ()),
						                  this.currentRoom.getConnectedPeers().Count, msgReceived.turn);
						game.Run ();
						});
						t.Start();
                        break;
                    case MessageType.GameUpdate:
                        foreach (PeerInfo peer in this.currentRoom.getConnectedPeers())
                        {
							if (cubenode[this.currentRoom.getIDOnRoom(this.peerInfo.getID())].Contains(this.currentRoom.getIDOnRoom(peer.getID())))
                                System.Console.WriteLine(peer.ToString() + " -> " + this.peerInfo.ToString() + " : " + msgReceived.timestamp + msgReceived.gameUpdate.ToString());
                        }
                        break;
                }
                if (msgReceived.msgType != MessageType.CreatorQuit)
                    peerToPeerSocketClient.BeginReceive(msgByteP2PClient, 0, msgByteP2PClient.Length, SocketFlags.None,
                                                            new AsyncCallback(onReceivePeerClient), null);
            }
            catch (Exception e)
            {
                Console.WriteLine("onreceivepeerclient exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        private void onSendPeerServer(IAsyncResult ar)
        {
            try
            {
                Socket peerSocket = (Socket)ar.AsyncState;
                peerSocket.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine("onReceivePeerClient exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        public void gotoLobby()
        {
            try
            {
                byte[] msg = new Message(MessageType.List, this.peerInfo.getID()).toByte();
                peerToTrackerSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None,
                                                new AsyncCallback(onSendTracker), null);
            }
            catch (Exception e)
            {
                Console.WriteLine("goToLobby exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        public void createRoom(string roomID, int maxPlayer)
        {
            try
            {
                initializeP2PConnection();
                this.lastMessage = MessageType.Create;
                this.peerList = new List<PeerHandler>();
                byte[] msg = new Message(MessageType.Create, this.peerInfo.getID(), maxPlayer, roomID).toByte();

                this.currentRoom = new Room(this.peerInfo, roomID, maxPlayer);

                peerToTrackerSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None,
                                                new AsyncCallback(onSendTracker), null);
            }
            catch (Exception e)
            {
                Console.WriteLine("createRoom exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        public void joinRoom(string roomID)
        {
            try
            {
                this.lastMessage = MessageType.Join;
                byte[] msg = new Message(MessageType.Join, this.peerInfo.getID(), roomID).toByte();

                peerToTrackerSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None,
                                                new AsyncCallback(onSendTracker), null);
            }
            catch (Exception e)
            {
                Console.WriteLine("joinRoom exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        public void quitRoom()
        {
            try
            {
                if (isCreator)
                {
                    this.lastMessage = MessageType.CreatorQuit;
                    this.isCreator = false;
                    if (this.currentRoom.getConnectedPeers().Count == 1)
                    {
                        byte[] msg = new Message(MessageType.CreatorQuit, this.peerInfo).toByte();
                        peerToTrackerSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(onSendTracker), null);
                    }
                    else
                    {
                        byte[] msg = new Message(MessageType.CreatorQuit, this.currentRoom.getConnectedPeers()[1]).toByte();
                        peerToTrackerSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(onSendTracker), null);
                        foreach (PeerHandler peer in peerList)
                        {
                            peer.socket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(onSendPeerServer), peer.socket);
                        }
                    }
                }
                else
                {
                    this.lastMessageClient = MessageType.Quit;
                    byte[] msg = new Message(MessageType.Quit, this.peerInfo.getID()).toByte();

                    peerToPeerSocketClient.BeginSend(msg, 0, msg.Length, SocketFlags.None,
                                                    new AsyncCallback(onSendPeerClient), null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("quitRoom exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        public void updateRoom()
        {
            try
            {
                if (isCreator)
                {
                    this.lastMessage = MessageType.GameUpdate;
                    Message m = new Message(MessageType.GameUpdate, getCurrentTime(), new GameUpdate(1, 2, 3));
                    byte[] msg = m.toByte();
                    foreach (PeerHandler peer in peerList)
                    {
                        peer.socket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(onSendPeerServer), peer.socket);
                    }
                    foreach (PeerInfo peer in this.currentRoom.getConnectedPeers())
                    {
						if (cubenode[this.currentRoom.getIDOnRoom(this.peerInfo.getID())].Contains(this.currentRoom.getIDOnRoom(peer.getID())))
                            System.Console.WriteLine(peer.ToString() + " -> " + this.peerInfo.ToString() + " : " + m.timestamp + m.gameUpdate.ToString());
                    }
                }
                else
                {
                    this.lastMessageClient = MessageType.GameUpdate;
                    byte[] msg = new Message(MessageType.GameUpdate, getCurrentTime(), new GameUpdate(1,2,3)).toByte();

                    peerToPeerSocketClient.BeginSend(msg, 0, msg.Length, SocketFlags.None,
                                                    new AsyncCallback(onSendPeerClient), null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("quitRoom exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        public void startRoom()
        {
            try
            {
                if (isCreator)
                {
                    this.lastMessage = MessageType.Start;
                    if (this.currentRoom.getConnectedPeers().Count % 2 == 1)
                    {
                        Console.WriteLine("cannot start ! unbalance team");
                    }
                    else
					{
						Random r = new Random();
						int turn = r.Next(this.currentRoom.getConnectedPeers().Count);
                        byte[] msg = new Message(MessageType.Start, this.peerInfo.getID(), turn, this.currentRoom.getRoomID()).toByte();
                        foreach (PeerHandler peer in peerList)
                        {
                            peer.socket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(onSendPeerServer), peer.socket);
						}
						Thread t = new Thread( () => {
							Thread.CurrentThread.IsBackground = true;
							game = new Game1 (this.currentRoom.getIDOnRoom(this.peerInfo.getID ()),
							                  this.currentRoom.getConnectedPeers().Count, turn);
							game.Run ();
						});
						t.Start();
                    }
                }
                else
                {
                    Console.WriteLine("start failed : not a room master !");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("quitRoom exception : {0}", e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.TargetSite);
            }
        }

        public void drawLobby(List<Room> roomList)
        {
            System.Console.WriteLine("\nLobby Screen");
            System.Console.WriteLine("------------");
            int i = 1;       
            System.Console.WriteLine("ROOM LIST\n");
            foreach (Room room in roomList)
            {
                System.Console.WriteLine("Room #" + i); i++;
                System.Console.WriteLine(room.toStringWithoutPeers());
            }

        }

        public void drawRoom()
        {
            if (this.currentRoom != null)
            {
                System.Console.WriteLine("\nRoom Screen");
                System.Console.WriteLine("-----------");
				System.Console.WriteLine(this.currentRoom);
				System.Console.WriteLine("ID on room : " + this.currentRoom.getIDOnRoom(this.peerInfo.getID()));
                foreach (PeerInfo peer in this.currentRoom.getConnectedPeers())
                {
					if (cubenode[this.currentRoom.getIDOnRoom(this.peerInfo.getID())].Contains(this.currentRoom.getIDOnRoom(peer.getID())))
                        if (peer.getID() < this.peerInfo.getID())
                            System.Console.WriteLine("active connection with " + peer.ToString());
                        else if (peer.getID() > this.peerInfo.getID())
                            System.Console.WriteLine("passive connection with " + peer.ToString());
                }
            }
        }

        public void removePeerHandler(int peerID)
        {
            foreach (PeerHandler peer in peerList)
            {
                if (peer.info.getID() == peerID)
                {
                    this.peerList.Remove(peer);
                    return;
                }
            }
        }

        public void removeDeadPeer(int peerID)
        {
            removePeerHandler(peerID);
            this.currentRoom.removePlayer(peerID);
            byte[] m = new Message(MessageType.Player, this.currentRoom.getConnectedPeers()).toByte();
            foreach (PeerHandler peer in peerList)
            {
                peer.socket.BeginSend(m, 0, m.Length, SocketFlags.None, new AsyncCallback(onSendPeerServer), peer.socket);
            }
            drawRoom();
        }

        private void sendKeepAlive(object source, ElapsedEventArgs e)
        {
            byte[] msg = new Message(MessageType.KeepAlive, this.peerInfo.getID()).toByte();
            peerToTrackerSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(onSendTracker), null);
        }

        private void sendKeepAliveCreator(object source, ElapsedEventArgs e)
        {
            byte[] msg = new Message(MessageType.KeepAlive, this.peerInfo.getID()).toByte();
            peerToPeerSocketClient.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(onSendPeerClient), null);
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

        private void trackerDown(object source, ElapsedEventArgs e)
        {
            this.keepAliveResponse.Stop();
            this.peerInfo.setID(-1);
            this.state = GameState.Connect;
            System.Console.WriteLine("You have been disconnected because tracker is down");
            System.Console.WriteLine("Try to reconnect in a few minutes");
        }

        private void creatorDown(object source, ElapsedEventArgs e)
        {
            this.keepAliveResponseCreator.Stop();
            if (this.peerInfo.getID() == this.currentRoom.getConnectedPeers()[1].getID())
            {
                this.currentRoom.setCreator(this.peerInfo);
                this.currentRoom.getConnectedPeers().Clear();
                this.currentRoom.getConnectedPeers().Add(this.peerInfo);
                this.isCreator = true;
                this.peerList = new List<PeerHandler>();

                byte[] msgTracker = new Message(MessageType.SelfPromoteCreator, currentRoom.getCreator(), this.peerInfo).toByte();
                peerToTrackerSocket.BeginSend(msgTracker, 0, msgTracker.Length, SocketFlags.None, new AsyncCallback(onSendTracker), null);
            }
            else
            {
                this.currentRoom.setCreator(new PeerInfo(this.currentRoom.getConnectedPeers()[1]));
                connectToCreator(this.currentRoom.getConnectedPeers()[1].getIP(), this.currentRoom.getConnectedPeers()[1].getPort());
            }
        }

        public override string ToString()
        {
            return this.peerInfo.ToString();
        }

        public static long getCurrentTime()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        static void Main(string[] args)
        {
            string cmd = String.Empty;
            string[] cmds;
            Peer peer = new Peer();

            System.Console.WriteLine("GunbondGame Peer v1.0");

            while (cmd != "exit")
            {
                System.Console.Write(peer + ">");
                cmd = System.Console.ReadLine();
                cmds = Regex.Split(cmd, " ");
                if (cmds[0] == "connect" && peer.state == GameState.Connect)
                {
                    if (cmds[1] == "localhost")
                        cmds[1] = "127.0.0.1";

                    peer.connectToTracker(cmds[1], cmds[2]);
                }
                else if (cmds[0] == "create" && peer.state == GameState.Lobby)
                {
                    peer.createRoom(cmds[1], Convert.ToInt32(cmds[2]));
                }
                else if (cmds[0] == "quit" && peer.state == GameState.Room)
                {
                    peer.quitRoom();
                }
                else if (cmds[0] == "list" && peer.state == GameState.Lobby)
                {
                    peer.gotoLobby();
                }
                else if (cmds[0] == "join" && peer.state == GameState.Lobby)
                {
                    peer.joinRoom(cmds[1]);
                }
                else if (cmds[0] == "start" && peer.state == GameState.Room)
                {
                    peer.startRoom();
                }
                else if (cmds[0] == "update" && peer.state == GameState.Room)
                {
                    peer.updateRoom();
                }
            }
        }
    }
}