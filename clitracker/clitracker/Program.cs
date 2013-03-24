using System;
using System.Collections;
using System.Linq;
using System.Text;

class Peer
{
    public int peer_id { get; set; }
}

class Room
{
    public int room_id { get; set; }
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
}

namespace clitracker
{
    class Program
    {
        static bool done;
        static ArrayList peers;
        static ArrayList rooms;
        const int cx = 0;
        const string pstr = "GundbondGame0000000";
        static string SUCCESS = "SUCCESS";//pstr + Convert.ToChar((int)127);
        static string FAILED = "FAILED";//pstr + Convert.ToChar((int)128);
        static int lastPeerID = 110;
        static int max_peer = 100;
        static int max_room = 100;
        static bool log = true;

        static int findPeer(int peer_id)
        {
            for (int i = 0; i < peers.Count; i++)
            {
                Peer p = peers[i] as Peer;
                if (p.peer_id == peer_id)
                    return i;
            }
            return -1;
        }

        static int findRoom(int room_id)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                Room r = rooms[i] as Room;
                if (r.room_id == room_id)
                    return i;
            }
            return -1;
        }

        static int countRoom()
        {
            return rooms.Count;
        }

        static bool joinRoom(int room_id, int peer_id)
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

        static string getAllRoom()
        {
            string s = "";
            for (int i=0; i < rooms.Count; i++)
            {
                Room r = rooms[i] as Room;
                byte[] byte_peer_id = BitConverter.GetBytes(r.peer_id);
                byte[] byte_room_id = BitConverter.GetBytes(r.room_id);
                byte[] byte_maxplayer = BitConverter.GetBytes(r.maxplayer);
                s += System.Text.Encoding.UTF8.GetString(byte_room_id) + System.Text.Encoding.UTF8.GetString(byte_peer_id) + System.Text.Encoding.UTF8.GetString(byte_maxplayer);
            }
            return s;
        }

        static string procesRequest(string s)
        {
            Console.Write("Create Code : ");
            string r = Console.ReadLine();
            switch (r)
            {
                case "135":
                    if (peers.Count == max_peer)
                    {
                        if (log)
                            Console.WriteLine("Handshake failed : max_peer reached");
                        return FAILED;
                    }
                    Peer p = new Peer();
                    p.peer_id = lastPeerID;
                    peers.Add(p);
                    byte[] byte_peer_id = BitConverter.GetBytes(lastPeerID);
                    lastPeerID++;
                    //return pstr + Convert.ToChar((int)135) + System.Text.Encoding.UTF8.GetString(byte_peer_id);
                    if (log)
                        Console.WriteLine("Handshake success : peer " + p.peer_id);
                    return pstr + Convert.ToChar((int)135) + p.peer_id;
                case "255":
                    Console.Write("Peer ID : ");
                    int peerid = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Max Player Num : ");
                    int maxplayer = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Room ID : ");
                    int roomid = Convert.ToInt32(Console.ReadLine());
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
                        return SUCCESS;
                    }
                    else
                    {
                        if (log)
                            Console.WriteLine("Room creation failed");
                        return FAILED;
                    }
                case "254":
                    Console.Write("Peer ID : ");
                    int peerid2 = Convert.ToInt32(Console.ReadLine());
                    if (log)
                        Console.WriteLine("Room listed for : peer " + peerid2);
                    return pstr + Convert.ToChar((int)200) + countRoom() + getAllRoom();
                case "253":
                    Console.Write("Peer ID : ");
                    int peerid3 = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Room ID : ");
                    int roomid3 = Convert.ToInt32(Console.ReadLine());
                    if (joinRoom(roomid3, peerid3))
                    {
                        if (log)
                            Console.WriteLine("Room join success : peer " + peerid3 + " room " + roomid3);
                        return SUCCESS;
                    }
                    else
                    {
                        if (log)
                            Console.WriteLine("Room join failed : peer " + peerid3 + " room " + roomid3);
                        return FAILED;
                    }
                case "252":
                    Console.Write("Peer ID : ");
                    int peerid4 = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Room ID : ");
                    int roomid4 = Convert.ToInt32(Console.ReadLine());
                    int roomidx = findRoom(roomid4);
                    if (roomidx == -1)
                    {
                        if (log)
                            Console.WriteLine("Room start failed : room not found. peer " + peerid4 + " room " + roomid4);
                        return FAILED;
                    }
                    Room r4 = rooms[roomidx] as Room;
                    if (r4.peer_id != peerid4)
                    {
                        if (log)
                            Console.WriteLine("Room start failed : peer is not master. peer " + peerid4 + " room " + roomid4);
                        return FAILED;
                    }
                    rooms.RemoveAt(roomidx);
                    if (log)
                        Console.WriteLine("Room start success peer " + peerid4 + " room " + roomid4);
                    return SUCCESS;
                case "235":
                    Console.Write("Peer ID : ");
                    int peerid5 = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Room ID : ");
                    int roomid5 = Convert.ToInt32(Console.ReadLine());
                    int roomidx5 = findRoom(roomid5);
                    if (roomidx5 == -1)
                    {
                        if (log)
                            Console.WriteLine("Room quit failed : room not found. peer " + peerid5 + " room " + roomid5);
                        return FAILED;
                    }
                    Room r5 = rooms[roomidx5] as Room;
                    if (r5.findNeighbor(peerid5) == -1)
                    {
                        if (log)
                            Console.WriteLine("Room quit failed : peer hasn't joined yet. peer " + peerid5 + " room " + roomid5);
                        return FAILED;
                    }
                    r5.neighbor.RemoveAt(r5.findNeighbor(peerid5));
                    if (log)
                        Console.WriteLine("Room quit success. peer " + peerid5 + " room " + roomid5);
                    return SUCCESS;
                case "shutdown":
                    done = true;
                    return "Shutting down...";
                case "log on":
                    log = true;
                    return "Log turned on...";
                case "log off":
                    log = true;
                    return "Log turned off...";
                case "max_peer":
                    Console.Write("Input max_peer : ");
                    max_peer = Convert.ToInt32(Console.ReadLine());
                    return "max_peer changed into " + max_peer;
                case "max_room":
                    Console.Write("Input max_room : ");
                    max_room = Convert.ToInt32(Console.ReadLine());
                    return "max_room changed into " + max_room;
                default:
                    return "Other";
            }
        }
        static void Main(string[] args)
        {
            string s,reply;
            done = false;
            peers = new ArrayList();
            rooms = new ArrayList();
            while (!done)
            {
                //s = Console.ReadLine();
                reply = procesRequest(""+Convert.ToChar(135));
                Console.WriteLine(reply);
            }
            Console.WriteLine("already done");
        }
    }
}
