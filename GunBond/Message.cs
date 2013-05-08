using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GunBond
{
    public enum MessageType
    {
        Handshake,          // Handshake message from peer to tracker            : <pstr><reserved>135<username>
        HandshakeResponse,  // Handshake response message from tracker to peer   : <pstr><reserved>136<peer_info>
        KeepAlive,          // KeepAlive message duplex peer tracker             : <pstr><reserved>182<peer_id>
        Create,             // Create room message from peer to tracker          : <pstr><reserved>255<peer_id><max_player><room_id>
        List,               // List room message from peer to tracker            : <pstr><reserved>254<peer_id>
        Room,               // List room response message from tracker to peer   : <pstr><reserved>200<room_count><room>..<room>
        Success,            // Success message from tracker to peer              : <pstr><reserved>127
        Failed,             // Failed message from tracker to peer               : <pstr><reserved>128
        Join,               // Join message from peer to tracker                 : <pstr><reserved>253<peer_id><room_id>
        Start,              // Start message from creator peer to tracker        : <pstr><reserved>252<peer_id><room_id>
        Quit,               // Quit message from peer to tracker                 : <pstr><reserved>235<peer_id>
        Check,              // Check message from tracker to peer                : <pstr><reserved>177<peer_id_requester>
        CheckResponse,      // Check response message from peer to tracker       : <pstr><reserved>178<peer_id_requester><isJoinAccepted:boolean><port>
        HandshakeCreator,   // HandshakeCreator message from peer to creator peer: <pstr><reserved>111<peer_info>
        Player,             // Player message from creator peer to peer          : <pstr><reserved>112<player_count><player>..<player>
        JoinSuccess,        // JoinSuccess message from tracker to peer          : <pstr><reserved>250<room>
        CreatorQuit,        // CreatorQuit message from creator to tracker       : <pstr><reserved>222<newCreator:PeerInfo>
        RoomInfo,           // RoomInfo message from creator to tracker/peer     : <pstr><reserved>123<room>
        SelfPromoteCreator, // SelfPC message from confused creator to tracker   : <pstr><reserved>117<oldCreator><newCreator>
        GameUpdate,         // Message to update game                            : <pstr><reserved>118<timestamp><gameUpdate>
        Null                // Undefined Message type
    }

    public class Message
    {
        public MessageType msgType; //Message type
        public string pstr; //protocol string identifier = "GunbondGame"
        public byte[] reservedByte = new byte[8];
        public int peerID;
        public string roomID;
        public int maxPlayer;
        public int roomCount;
        public int peerIDJoinRoom;
        public bool isJoinAccepted;
        public List<Room> roomList = new List<Room>();
        public List<PeerInfo> player = new List<PeerInfo>();
        public Room room;
		public int port;
		public int turn;
        public PeerInfo peer;
        public PeerInfo peer2;
        public string username;
        public long timestamp;
        public GameUpdate gameUpdate;

        //Default constructor
        public Message()
        {
            this.msgType = MessageType.Null;
        }

        // Constructor for Handshake message
        public Message(MessageType msgType, string username)
        {
            this.msgType = msgType;
            this.username = username;
        }

        // Constructor for Success, Failed message
        public Message(MessageType msgType)
        {
            this.msgType = msgType;
        }

        // Constructor for KeepAlive, List, Quit, Check message
        public Message(MessageType msgType, int number)
        {
            this.msgType = msgType;
            if (msgType == MessageType.Check)
                this.peerIDJoinRoom = number;
            else
                this.peerID = number;
        }

        // Constructor for Join message
        public Message(MessageType msgType, int peerID, string roomID)
        {
            this.msgType = msgType; this.peerID = peerID; this.roomID = roomID;
        }

		// Constructor for Create, Start message
        public Message(MessageType msgType, int peerID, int maxPlayer, string roomID)
        {
            this.msgType = msgType; this.peerID = peerID; this.turn = this.maxPlayer = maxPlayer; this.roomID = roomID;
        }

        // Constructor for Room message
        public Message(MessageType msgType, List<Room> roomList)
        {
            this.msgType = msgType; this.roomCount = roomList.Count;
            foreach (Room room in roomList)
            {
                this.roomList.Add(new Room(room));
            }
        }

        // Constructor for CheckResponse Message
        public Message(MessageType msgType, int peerIDJoinRoom, bool isJoinAccepted, int port)
        {
            this.msgType = msgType; this.peerIDJoinRoom = peerIDJoinRoom;
            this.isJoinAccepted = isJoinAccepted; this.port = port;
        }

        // Constructor for Player Message
        public Message(MessageType msgType, List<PeerInfo> peerList)
        {
            this.msgType = msgType;
            foreach (PeerInfo peer in peerList)
            {
                this.player.Add(new PeerInfo(peer.getIP(), peer.getID(), peer.getPort(), peer.getUsername()));
            }
        }

        // Constructor for JoinSuccess, RoomInfo message
        public Message(MessageType msgType, Room room)
        {
            this.msgType = msgType; this.room = room;
        }

        // Constructor for HandshakeCreator, CreatorQuit, Handshake Response message
        public Message(MessageType msgType, PeerInfo peer)
        {
            this.msgType = msgType; this.peer = peer;
        }

        // Constructor for SelfPromoteCreator message
        public Message(MessageType msgType, PeerInfo peer, PeerInfo peer2)
        {
            this.msgType = msgType; this.peer = peer; this.peer2 = peer2;
        }

        // Constructor for GameUpdate message
        public Message(MessageType msgType, long timestamp, GameUpdate gameUpdate)
        {
            this.msgType = msgType; this.timestamp = timestamp; this.gameUpdate = gameUpdate;
        }

        // Construct message object from ByteArray
        public Message(byte[] msg)
        {
            int offset = 0;
            string pstr = Encoding.ASCII.GetString(msg, offset, GameConstant.pstr.Length);
            offset += GameConstant.pstr.Length;

            if (pstr.Equals(GameConstant.pstr))
            {
                this.pstr = pstr;
                Buffer.BlockCopy(msg, offset, this.reservedByte, 0, 8);
                offset += reservedByte.Length;
                int messageCode = (int)msg[offset]; offset++;

                switch (messageCode)
                {
                    case GameConstant.handshakeCode:
                        this.msgType = MessageType.Handshake;
                        this.username = Encoding.ASCII.GetString(msg, offset, GameConstant.usernameSize).Trim();
                        break;

                    case GameConstant.handshakeResponseCode:
                        this.msgType = MessageType.HandshakeResponse;
                        byte[] peerHR = new byte[GameConstant.peerInfoSize];
                        Buffer.BlockCopy(msg, offset, peerHR, 0, GameConstant.peerInfoSize);
                        this.peer = new PeerInfo(peerHR);
                        //this.peerID = BitConverter.ToInt32(msg, offset); offset += 4;
                        //this.port = BitConverter.ToInt32(msg, offset);
                        break;

                    case GameConstant.keepAliveCode:
                        this.msgType = MessageType.KeepAlive;
                        this.peerID = BitConverter.ToInt32(msg, offset);
                        break;

                    case GameConstant.createCode:
                        this.msgType = MessageType.Create;
                        this.peerID = BitConverter.ToInt32(msg, offset); offset += GameConstant.peerIdSize;
                        this.maxPlayer = BitConverter.ToInt32(msg, offset); offset += GameConstant.maxPlayerSize;
                        this.roomID = Encoding.ASCII.GetString(msg, offset, GameConstant.roomIDSize);
                        this.roomID = this.roomID.Replace("\0", String.Empty).Trim();
                        break;

                    case GameConstant.listCode:
                        this.msgType = MessageType.List;
                        this.peerID = BitConverter.ToInt32(msg, offset);
                        break;

                    case GameConstant.roomCode:
                        this.msgType = MessageType.Room;
                        this.roomCount = BitConverter.ToInt32(msg, offset);
                        offset += GameConstant.roomCountSize;
                        for (int i = 0; i < this.roomCount; i++)
                        {
                            if ((int)msg[offset] == GameConstant.beginRoomCode)
                            {
                                offset++;
                                int beginRoom = offset;
                                int roomByteLength = 0;
                                while ((int)msg[offset] != GameConstant.endRoomCode)
                                {
                                    roomByteLength++;
                                    offset++;
                                }
                                offset++;

                                byte[] roomByte = new byte[roomByteLength];
                                Buffer.BlockCopy(msg, beginRoom, roomByte, 0, roomByteLength);
                                this.roomList.Add(new Room(roomByte));
                            }
                        }
                        break;

                    case GameConstant.successCode:
                        this.msgType = MessageType.Success;
                        break;

                    case GameConstant.failedCode:
                        this.msgType = MessageType.Failed;
                        break;

                    case GameConstant.joinCode:
                        this.msgType = MessageType.Join;
                        this.peerID = BitConverter.ToInt32(msg, offset);
                        offset += GameConstant.peerIdSize;
                        this.roomID = Encoding.ASCII.GetString(msg, offset, GameConstant.roomIDSize);
                        this.roomID = this.roomID.Replace("\0", String.Empty).Trim();
                        break;

                    case GameConstant.startCode:
                        this.msgType = MessageType.Start;
                        this.peerID = BitConverter.ToInt32(msg, offset);
						offset += GameConstant.peerIdSize;
						this.turn = BitConverter.ToInt32(msg, offset);
						offset += 4;
                        this.roomID = Encoding.ASCII.GetString(msg, offset, GameConstant.startSize);
                        this.roomID = this.roomID.Replace("\0", String.Empty).Trim();
                        break;

                    case GameConstant.quitCode:
                        this.msgType = MessageType.Quit;
                        this.peerID = BitConverter.ToInt32(msg, offset);
                        break;

                    case GameConstant.checkCode:
                        this.msgType = MessageType.Check;
                        this.peerIDJoinRoom = BitConverter.ToInt32(msg, offset);
                        break;

                    case GameConstant.checkResponseCode:
                        this.msgType = MessageType.CheckResponse;
                        this.peerIDJoinRoom = BitConverter.ToInt32(msg, offset);
                        offset += GameConstant.peerIdSize;
                        this.isJoinAccepted = BitConverter.ToBoolean(msg, offset); offset += 1;
                        this.port = BitConverter.ToInt32(msg, offset);
                        break;

                    case GameConstant.handshakeCreatorCode:
                        this.msgType = MessageType.HandshakeCreator;
                        byte[] peer = new byte[GameConstant.peerInfoSize];
                        Buffer.BlockCopy(msg, offset, peer, 0, GameConstant.peerInfoSize);
                        this.peer = new PeerInfo(peer);
                        break;

                    case GameConstant.playerCode:
                        this.msgType = MessageType.Player;
                        int players = BitConverter.ToInt32(msg, offset); offset += 4;
                        for (int i = 0; i < players; i++)
                        {
                            byte[] peerByte = new byte[GameConstant.peerInfoSize];
                            Buffer.BlockCopy(msg, offset, peerByte, 0, GameConstant.peerInfoSize);
                            offset += GameConstant.peerInfoSize;

                            this.player.Add(new PeerInfo(peerByte));
                        }
                        break;

                    case GameConstant.joinSuccessCode:
                        this.msgType = MessageType.JoinSuccess;
                        if ((int)msg[offset] == GameConstant.beginRoomCode)
                        {
                            offset++;
                            int beginRoom = offset;
                            int roomByteLength = 0;
                            while ((int)msg[offset] != GameConstant.endRoomCode)
                            {
                                roomByteLength++; offset++;
                            }
                            offset++;

                            byte[] roomByte = new byte[roomByteLength];
                            Buffer.BlockCopy(msg, beginRoom, roomByte, 0, roomByteLength);

                            this.room = new Room(roomByte);
                        }
                        break;

                    case GameConstant.creatorQuitCode:
                        this.msgType = MessageType.CreatorQuit;
                        byte[] creatorPeer = new byte[GameConstant.peerInfoSize];
                        Buffer.BlockCopy(msg, offset, creatorPeer, 0, GameConstant.peerInfoSize);
                        this.peer = new PeerInfo(creatorPeer);
                        break;

                    case GameConstant.RoomInfoCode:
                        this.msgType = MessageType.RoomInfo;
                        if ((int)msg[offset] == GameConstant.beginRoomCode)
                        {
                            offset++;
                            int beginRoom = offset;
                            int roomByteLength = 0;
                            while ((int)msg[offset] != GameConstant.endRoomCode)
                            {
                                roomByteLength++; offset++;
                            }
                            offset++;

                            byte[] roomByte = new byte[roomByteLength];
                            Buffer.BlockCopy(msg, beginRoom, roomByte, 0, roomByteLength);

                            this.room = new Room(roomByte);
                        }
                        break;

                    case GameConstant.SelfPromoteCreatorCode:
                        this.msgType = MessageType.SelfPromoteCreator;
                        byte[] promote = new byte[GameConstant.peerInfoSize];
                        Buffer.BlockCopy(msg, offset, promote, 0, GameConstant.peerInfoSize);
                        offset += GameConstant.peerInfoSize;
                        this.peer = new PeerInfo(promote);
                        Buffer.BlockCopy(msg, offset, promote, 0, GameConstant.peerInfoSize);
                        this.peer2 = new PeerInfo(promote);
                        break;

                    case GameConstant.GameUpdate:
                        this.msgType = MessageType.GameUpdate;
                        this.timestamp = BitConverter.ToInt64(msg, offset);
                        offset += GameConstant.timestampSize;
                        byte[] gameUpdate = new byte[GameConstant.gameUpdateSize];
                        Buffer.BlockCopy(msg, offset, gameUpdate, 0, GameConstant.gameUpdateSize);
                        this.gameUpdate = new GameUpdate(gameUpdate);
                        break;

                }
            }
        }

        // Convert message to byte array
        public byte[] toByte()
        {
            switch (this.msgType)
            {
                case MessageType.Handshake: return constructMessage(MessageType.Handshake, this.username);
                case MessageType.HandshakeResponse: return constructMessage(MessageType.HandshakeResponse, this.peer);
                case MessageType.KeepAlive: return constructMessage(MessageType.KeepAlive, this.peerID);
                case MessageType.Create: return constructMessage(MessageType.Create, this.peerID, this.maxPlayer, this.roomID);
                case MessageType.List: return constructMessage(MessageType.List, this.peerID);
                case MessageType.Room: return constructMessage(MessageType.Room, this.roomCount, this.roomList);
                case MessageType.Join: return constructMessage(MessageType.Join, this.peerID, this.roomID);
                case MessageType.Success: return constructMessage(MessageType.Success);
                case MessageType.Failed: return constructMessage(MessageType.Failed);
                case MessageType.Start: return constructMessage(MessageType.Start, this.peerID, this.turn, this.roomID);
                case MessageType.Quit: return constructMessage(MessageType.Quit, this.peerID);
                case MessageType.Check: return constructMessage(MessageType.Check, this.peerIDJoinRoom);
                case MessageType.CheckResponse: return constructMessage(MessageType.CheckResponse, this.peerIDJoinRoom, this.isJoinAccepted, this.port);
                case MessageType.HandshakeCreator: return constructMessage(MessageType.HandshakeCreator, this.peer);
                case MessageType.Player: return constructMessage(MessageType.Player, this.player);
                case MessageType.JoinSuccess: return constructMessage(MessageType.JoinSuccess, this.room);
                case MessageType.CreatorQuit: return constructMessage(MessageType.CreatorQuit, this.peer);
                case MessageType.RoomInfo: return constructMessage(MessageType.RoomInfo, this.room);
                case MessageType.SelfPromoteCreator: return constructMessage(MessageType.SelfPromoteCreator, this.peer, this.peer2);
                case MessageType.GameUpdate: return constructMessage(MessageType.GameUpdate, this.timestamp, this.gameUpdate);
                default: return null;
            }
        }

        // Construct Message for Handshake message
        private byte[] constructMessage(MessageType msgType, string username)
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(GameConstant.pstrByte);
            msg.AddRange(GameConstant.reservedByte);
            msg.Add((byte)GameConstant.handshakeCode);
            byte[] usernameByte = Encoding.ASCII.GetBytes(username); Array.Resize(ref usernameByte, 10);
            msg.AddRange(usernameByte);

            return msg.ToArray();
        }

        // Construct Message for Success, Failed Message
        private byte[] constructMessage(MessageType msgType)
        {
            byte code;
            switch (msgType)
            {
                case MessageType.Success:
                    code = (byte)GameConstant.successCode; break;

                case MessageType.Failed:
                    code = (byte)GameConstant.failedCode; break;
                default: return null;
            }

            List<byte> msg = new List<byte>();
            msg.AddRange(GameConstant.pstrByte);
            msg.AddRange(GameConstant.reservedByte);
            msg.Add(code);

            return msg.ToArray();

        }

        // Construct Message for HandshakeResponse Message
        private byte[] constructMessage(MessageType msgtype, int peerID, int port)
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(GameConstant.pstrByte);
            msg.AddRange(GameConstant.reservedByte);
            msg.Add((byte)GameConstant.handshakeResponseCode);
            msg.AddRange(BitConverter.GetBytes(peerID));
            msg.AddRange(BitConverter.GetBytes(port));

            return msg.ToArray();
        }

        // Construct Message for KeepAlive, List, Quit, Check Message
        private byte[] constructMessage(MessageType msgType, int number)
        {
            byte code;
            switch (msgType)
            {
                case MessageType.KeepAlive:
                    code = (byte)GameConstant.keepAliveCode; break;

                case MessageType.List:
                    code = (byte)GameConstant.listCode; break;

                case MessageType.Quit:
                    code = (byte)GameConstant.quitCode; break;

                case MessageType.Check:
                    code = (byte)GameConstant.checkCode; break;

                default:
                    return null;
            }

            List<byte> msg = new List<byte>();
            msg.AddRange(GameConstant.pstrByte);
            msg.AddRange(GameConstant.reservedByte);
            msg.Add(code);
            msg.AddRange(BitConverter.GetBytes(number));

            return msg.ToArray();
        }

        // Construct Message for Join and Start message
        private byte[] constructMessage(MessageType msgType, int peerID, string roomID)
        {
            byte code;
            switch (msgType)
            {
                case MessageType.Join:
                    code = (byte)GameConstant.joinCode; break;

                case MessageType.Start:
                    code = (byte)GameConstant.startCode; break;

                default:
                    return null;
            }

            byte[] roomIDByte = Encoding.ASCII.GetBytes(roomID);
            Array.Resize(ref roomIDByte, GameConstant.roomIDSize);

            List<byte> msg = new List<byte>();
            msg.AddRange(GameConstant.pstrByte);
            msg.AddRange(GameConstant.reservedByte);
            msg.Add(code);
            msg.AddRange(BitConverter.GetBytes(peerID));
            msg.AddRange(roomIDByte);

            return msg.ToArray();
        }

        // Construct Message for Create message
        private byte[] constructMessage(MessageType msgType, int peerID, int maxPlayer, string roomID)
        {
			if (msgType != MessageType.Create && msgType != MessageType.Start) return null;
            else
            {
                byte[] roomIDByte = Encoding.ASCII.GetBytes(roomID);
                Array.Resize(ref roomIDByte, GameConstant.roomIDSize);

                List<byte> msg = new List<byte>();
                msg.AddRange(GameConstant.pstrByte);
                msg.AddRange(GameConstant.reservedByte);
				if (msgType == MessageType.Create)
                	msg.Add((byte)GameConstant.createCode);
				else
					msg.Add((byte)GameConstant.startCode);
                msg.AddRange(BitConverter.GetBytes(peerID));
                msg.AddRange(BitConverter.GetBytes(maxPlayer));
                msg.AddRange(roomIDByte);

                return msg.ToArray();
            }
        }

        // Construct Message for Room message
        private byte[] constructMessage(MessageType msgType, int roomCount, List<Room> roomList)
        {
            if (msgType != MessageType.Room) return null;
            else if (roomCount != roomList.Count) return null;
            else
            {
                List<byte> msg = new List<byte>();
                msg.AddRange(GameConstant.pstrByte);
                msg.AddRange(GameConstant.reservedByte);
                msg.Add((byte)GameConstant.roomCode);
                msg.AddRange(BitConverter.GetBytes(roomCount));
                foreach (Room room in roomList)
                {
                    msg.Add((byte)GameConstant.beginRoomCode);
                    msg.AddRange(room.toByte());
                    msg.Add((byte)GameConstant.endRoomCode);
                }
                return msg.ToArray();
            }

        }

        // Construct Message for Player message
        private byte[] constructMessage(MessageType msgType, List<PeerInfo> peerList)
        {
            if (msgType != MessageType.Player) return null;
            else
            {
                List<byte> msg = new List<byte>();
                msg.AddRange(GameConstant.pstrByte);
                msg.AddRange(GameConstant.reservedByte);
                msg.Add((byte)GameConstant.playerCode);
                msg.AddRange(BitConverter.GetBytes(peerList.Count));
                foreach (PeerInfo peer in peerList)
                {
                    msg.AddRange(peer.toByte());
                }

                return msg.ToArray();
            }
        }

        // Construct Message for CheckResponse message
        private byte[] constructMessage(MessageType msgType, int peerIDJoinRoom, bool isJoinAccepted, int port)
        {
            if (msgType != MessageType.CheckResponse) return null;
            else
            {
                List<byte> msg = new List<byte>();
                msg.AddRange(GameConstant.pstrByte);
                msg.AddRange(GameConstant.reservedByte);
                msg.Add((byte)GameConstant.checkResponseCode);
                msg.AddRange(BitConverter.GetBytes(peerIDJoinRoom));
                msg.AddRange(BitConverter.GetBytes(isJoinAccepted));
                msg.AddRange(BitConverter.GetBytes(port));

                return msg.ToArray();
            }
        }

        // Construct message for JoinSuccess, RoomInfo message
        private byte[] constructMessage(MessageType msgType, Room room)
        {
            if (msgType == MessageType.JoinSuccess || msgType == MessageType.RoomInfo)
            {
                List<byte> msg = new List<byte>();
                msg.AddRange(GameConstant.pstrByte);
                msg.AddRange(GameConstant.reservedByte);
                if (msgType == MessageType.JoinSuccess)
                    msg.Add((byte)GameConstant.joinSuccessCode);
                else
                    msg.Add((byte)GameConstant.RoomInfoCode);
                msg.Add((byte)GameConstant.beginRoomCode);
                msg.AddRange(room.toByte());
                msg.Add((byte)GameConstant.endRoomCode);

                return msg.ToArray();
            }
            else return null;
        }

        // Construct message for HandshakeCreator, CreatorQuit message
        private byte[] constructMessage(MessageType msgType, PeerInfo peer)
        {
            byte code;
            switch (msgType)
            {
                case MessageType.HandshakeCreator:
                    code = (byte)GameConstant.handshakeCreatorCode; break;

                case MessageType.CreatorQuit:
                    code = (byte)GameConstant.creatorQuitCode; break;

                case MessageType.HandshakeResponse:
                    code = (byte)GameConstant.handshakeResponseCode; break;

                default:
                    return null;
            }
            List<byte> msg = new List<byte>();
            msg.AddRange(GameConstant.pstrByte);
            msg.AddRange(GameConstant.reservedByte);
            msg.Add(code);
            msg.AddRange(peer.toByte());

            return msg.ToArray();
        }

        // Construct message for SelfPromoteCreator message
        private byte[] constructMessage(MessageType msgType, PeerInfo peer, PeerInfo peer2)
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(GameConstant.pstrByte);
            msg.AddRange(GameConstant.reservedByte);
            msg.Add((byte)GameConstant.SelfPromoteCreatorCode);
            msg.AddRange(peer.toByte());
            msg.AddRange(peer2.toByte());

            return msg.ToArray();
        }

        // Construct message for GameUpdate message
        private byte[] constructMessage(MessageType msgType, long timestamp, GameUpdate gameUpdate)
        {
            List<byte> msg = new List<byte>();
            msg.AddRange(GameConstant.pstrByte);
            msg.AddRange(GameConstant.reservedByte);
            msg.Add((byte)GameConstant.GameUpdate);
            msg.AddRange(BitConverter.GetBytes(timestamp));
            msg.AddRange(gameUpdate.toByte());

            return msg.ToArray();
        }
    }
}
