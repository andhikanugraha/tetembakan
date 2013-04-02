using System;
using System.Text;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Collections.Generic;

public static class Message
{
    public static int PSTR_SIZE = 11;
    public static int RESERVED_SIZE = 8;
    public static int CODE_SIZE = 1;
    public static int PEER_ID_SIZE = 4;
    public static int ROOM_ID_SIZE = 50;

    public static string PSTR = "GunbondGame";
    public static byte[] RESERVED = new byte[8];
    public static byte HANDSHAKE_CODE = 135;
    public static byte KEEP_ALIVE_CODE = 182;
    public static byte CREATE_CODE = 255;
    public static byte LIST_CODE = 254;
    public static byte ROOM_CODE = 200;
    public static byte SUCCESS_CODE = 127;
    public static byte FAILED_CODE = 128;
    public static byte JOIN_CODE = 253;
    public static byte START_CODE = 252;
    public static byte QUIT_CODE = 235;

    public static int MAX_PLAYER_NUM = 8;
    public static string ROOM_ID = "dummy room";


    public static string getPSTR(byte[] message)
    {
        byte[] result = new byte[11];
        Array.Copy(message, 0, result, 0, 11);
        return Encoding.Default.GetString(result);
    }

    public static string getReserved(byte[] message)
    {
        byte[] result = new byte[8];
        Array.Copy(message, 11, result, 0, 8);
        return Encoding.Default.GetString(result);
    }

    public static int getCode(byte[] message)
    {
        byte result = message[19];
        return result;
    }

    public static int getPeerId(byte[] message)
    {
        byte[] result = new byte[4];
        Array.Copy(message, 20, result, 0, 4);
        return BitConverter.ToInt32(result, 0);
    }

    public static int getMaxPlayer(byte[] message)
    {
        byte[] result = new byte[4];
        Array.Copy(message, 24, result, 0, 4);
        return BitConverter.ToInt32(result, 0);
    }

    public static String getRoomId(byte[] message)
    {
        byte[] result = new byte[4];
        Array.Copy(message, 24, result, 0, 50);
        return BitConverter.ToString(result, 0);
    }

    public static String getRoomId2(byte[] message)
    {
        byte[] result = new byte[4];
        Array.Copy(message, 28, result, 0, 50);
        return BitConverter.ToString(result, 0);
    }

    public static byte[] dataToByte(string data, int allocation)
    {
        byte[] result = new byte[allocation];
        for (int i = 0; i < allocation; ++i)
        {
            result[i] = (byte)data[i];
        }
        return result;
    }

    public static byte[] KeepAlive(int peer_id)
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
        buffer.AddRange(RESERVED);
        buffer.Add(KEEP_ALIVE_CODE);
        buffer.AddRange(BitConverter.GetBytes(peer_id));
        return buffer.ToArray();
    }

    public static byte[] HandShake(int peer_id)
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
        buffer.AddRange(RESERVED);
        buffer.Add(HANDSHAKE_CODE);
        buffer.AddRange(BitConverter.GetBytes(peer_id));
        return buffer.ToArray();
    }

    public static byte[] Create(int new_peer_id, int max_player_num, string new_room_id)
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
        buffer.AddRange(RESERVED);
        buffer.Add(CREATE_CODE);
        buffer.AddRange(BitConverter.GetBytes(new_peer_id));
        buffer.AddRange(BitConverter.GetBytes(max_player_num));
        buffer.AddRange(dataToByte(new_room_id, ROOM_ID_SIZE));
        return buffer.ToArray();
    }

    public static byte[] List(int peer_id)
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
        buffer.AddRange(RESERVED);
        buffer.Add(LIST_CODE);
        buffer.AddRange(BitConverter.GetBytes(peer_id));
        return buffer.ToArray();
    }

    public static byte[] Room(int room_count, ArrayList rooms)
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
        buffer.AddRange(RESERVED);
        buffer.Add(ROOM_CODE);
        buffer.AddRange(BitConverter.GetBytes(room_count));
        for (int i = 0; i < rooms.Count; i++)
        {
            Room r = rooms[i] as Room;
            buffer.AddRange(r.roomToBytes());
        }
        return buffer.ToArray();
    }

    public static byte[] Success()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
        buffer.AddRange(RESERVED);
        buffer.Add(SUCCESS_CODE);
        return buffer.ToArray();
    }

    public static byte[] Failed()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
        buffer.AddRange(RESERVED);
        buffer.Add(FAILED_CODE);
        return buffer.ToArray();
    }

    public static byte[] Join(int peer_id, string room_id)
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
        buffer.AddRange(RESERVED);
        buffer.Add(JOIN_CODE);
        buffer.AddRange(BitConverter.GetBytes(peer_id));
        buffer.AddRange(dataToByte(room_id, ROOM_ID_SIZE));
        return buffer.ToArray();
    }

    public static byte[] Start(int peer_id, string room_id)
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
        buffer.AddRange(RESERVED);
        buffer.Add(START_CODE);
        buffer.AddRange(BitConverter.GetBytes(peer_id));
        buffer.AddRange(dataToByte(room_id, ROOM_ID_SIZE));
        return buffer.ToArray();
    }

    public static byte[] Quit(int peer_id)
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
        buffer.AddRange(RESERVED);
        buffer.Add(QUIT_CODE);
        buffer.AddRange(BitConverter.GetBytes(peer_id));
        return buffer.ToArray();
    }
}

class Peer
{
    public int peer_id { get; set; }
}

class Room
{
    public Room() { }
    public String room_id { get; set; }
    public int peer_id { get; set; }
    public int maxplayer { get; set; }
    public ArrayList neighbor { get; set; }
    public int findNeighbor(int peer_id)
    {
        for (int i = 0; i < neighbor.Count; i++)
        {
            Peer p = neighbor[i] as Peer;
            if (p.peer_id == peer_id)
                return i;
        }
        return -1;
    }
    public byte[] roomToBytes()
    {
        List<byte> buffer = new List<byte>();
        buffer.AddRange(BitConverter.GetBytes(maxplayer));
        buffer.AddRange(BitConverter.GetBytes(neighbor.Count));
        for (int i = 0; i < neighbor.Count; i++)
        {
            Peer p = neighbor[i] as Peer;
            buffer.AddRange(BitConverter.GetBytes(p.peer_id));
        }

        byte[] temp = new byte[room_id.Length * sizeof(char)];
        Buffer.BlockCopy(room_id.ToCharArray(), 0, temp, 0, temp.Length);
        buffer.AddRange(temp);
        return buffer.ToArray();
    }
    /*
    public Room(byte[] data)
    {
        maxplayer = BitConverter.ToInt16(data[0], 0);
        int id_num = BitConverter.ToInt16(data[1], 0);
        for (int i = 0; i < id_num; ++i)
        {
            Peer p;
            p.peer_id = BitConverter.ToInt16(data[i + 2], 0);
            neighbor.Add(p);
        }

        char[] room_id = new char[data.Last().Length / sizeof(char)];
        System.Buffer.BlockCopy(data.Last(), 0, room_id, 0, data.Last().Length);
        gameRoom.room_id = new string(room_id);
        return gameRoom;
    }*/
}
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
                    return i;
            }
            return -1;
        }

        private int countRoom()
        {
            return rooms.Count;
        }

        private bool joinRoom(String room_id, int peer_id)
        {
            int pindex = findPeer(peer_id);
            int rindex = findRoom(room_id);
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
                    case 135:
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
                            byte[] buff = Message.HandShake(p.peer_id);
                            clientStream.Write(buff, 0, buff.Length);
                            clientStream.Flush();
                        }
                        break;
                    case 255:
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
                    case 254:
                        int peerid2 = Message.getPeerId(message);
                        if (log)
                            Console.WriteLine("Room listed for : peer " + peerid2);
                        byte[] buff2 = Message.List(peerid2);
                        clientStream.Write(buff2, 0, buff2.Length);
                        clientStream.Flush();
                        break;
                    case 253:
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
                    case 252:
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
                    case 235:
                        int peerid5 = Message.getPeerId(message);
                        String roomid5 = Message.getRoomId(message);
                        int roomidx5 = findRoom(roomid5);
                        if (roomidx5 == -1)
                        {
                            if (log)
                                Console.WriteLine("Room quit failed : room not found. peer " + peerid5 + " room " + roomid5);
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
                                    Console.WriteLine("Room quit failed : peer hasn't joined yet. peer " + peerid5 + " room " + roomid5);
                                byte[] buff = Message.Failed();
                                clientStream.Write(buff, 0, buff.Length);
                                clientStream.Flush();
                            }
                            else
                            {
                                r5.neighbor.RemoveAt(r5.findNeighbor(peerid5));
                                if (log)
                                    Console.WriteLine("Room quit success. peer " + peerid5 + " room " + roomid5);
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

        public static void Main(string[] args)
        {
            Server server = new Server();
            bool shutdown = false;
            while (!shutdown)
            {
                Console.Write("> ");
                string s = Console.ReadLine();
                string[] command = s.Split(' ');
                Console.WriteLine(command[0]);
                if (command[0].Equals("shutdown"))
                {
                    shutdown = true;
                }
                if (command[0].Equals("log"))
                {
                    if (command.Length < 2)
                    {
                        Console.WriteLine("Unknown parameter");
                    } else
                    if (command[1].Equals("on"))
                    {
                        server.log = true;
                        Console.WriteLine("Log turned on");
                    }
                    else if (command[1].Equals("off"))
                    {
                        server.log = false;
                        Console.WriteLine("Log turned off");
                    }
                    else
                    {
                        Console.WriteLine("Unknown parameter");
                    }
                }
                if (command[0].Equals("max_peer"))
                {
                    server.max_peer = Int32.Parse(command[1]);
                    Console.WriteLine("Max peer changed into "+server.max_peer);
                }
                if (command[0].Equals("max_room"))
                {
                    server.max_room = Int32.Parse(command[1]);
                    Console.WriteLine("Max room changed into " + server.max_room);
                }
                Console.WriteLine(Encoding.Default.GetByteCount(s.Split(' ')[0]));
            }
            Console.WriteLine("Program exited");
            Environment.Exit(0);
        }

    }

}
