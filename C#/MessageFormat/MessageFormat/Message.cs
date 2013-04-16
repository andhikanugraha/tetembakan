using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace MessageFormat
{
    public static class Message
    {
        public static int PSTR_SIZE = 11;
        public static int RESERVED_SIZE = 8;
        public static int CODE_SIZE = 1;
        public static int PEER_ID_SIZE = 4;
        public static int ROOM_ID_SIZE = 50;

        public const string PSTR = "GunbondGame";
        public static byte[] RESERVED = new byte[8];
        public const byte HANDSHAKE_CODE = 135;
        public const byte KEEP_ALIVE_CODE = 182;
        public const byte CREATE_CODE = 255;
        public const byte LIST_CODE = 254;
        public const byte ROOM_CODE = 200;
        public const byte SUCCESS_CODE = 127;
        public const byte FAILED_CODE = 128;
        public const byte JOIN_CODE = 253;
        public const byte START_CODE = 252;
        public const byte QUIT_CODE = 235;
        public const byte INSIDEROOM_CODE = 201;
        public const byte DUMMY_CODE = 181;

        public const int MAX_PLAYER_NUM = 8;
        public const string ROOM_ID = "dummy room";

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
            byte[] result = new byte[50];
            Array.Copy(message, 24, result, 0, 50);
            return BitConverter.ToString(result, 0);
        }

        public static String getRoomId2(byte[] message)
        {
            byte[] result = new byte[50];
            Array.Copy(message, 28, result, 0, 50);
            return BitConverter.ToString(result, 0);
        }

        public static int getRoomCount(byte[] message)
        {
            byte[] result = new byte[4];
            Array.Copy(message, 20, result, 0, 4);
            return BitConverter.ToInt32(result, 0);
        }

        public static ArrayList getRooms(byte[] message)
        {
            byte[] result = new byte[1];

            ArrayList rooms = new ArrayList();

            int room_count = getRoomCount(message);
            int iterator = 0;
            for (int i = 0; i < room_count; ++i)
            {
                result = new byte[4];
                Array.Copy(message, 24 + iterator, result, 0, 4);
                int peer_id = BitConverter.ToInt32(result, 0);
                Debug.WriteLine("peer_id: " + peer_id);
                iterator += 4;

                result = new byte[4];
                Array.Copy(message, 24 + iterator, result, 0, 4);
                int maxplayer = BitConverter.ToInt32(result, 0);
                Debug.WriteLine("maxplayer: " + maxplayer);
                iterator += 4;

                result = new byte[4];
                Array.Copy(message, 24 + iterator, result, 0, 4);
                int neighbor_count = BitConverter.ToInt32(result, 0);
                iterator += 4;
                Debug.WriteLine("neighbor_count: " + neighbor_count);

                Room room = new Room();
                room.peer_id = peer_id;
                room.maxplayer = maxplayer;
                for (int j = 0; j < neighbor_count; j++)
                {
                    result = new byte[4];
                    Array.Copy(message, 24 + iterator, result, 0, 4);
                    int peerID = BitConverter.ToInt32(result, 0);

                    Debug.WriteLine("peerID[{0}]: " + peerID, j);

                    Peer p = new Peer();
                    p.peer_id = peerID;
                    iterator += 4;
                    room.neighbor.Add(p);
                }

                room.room_id = "";
                for(int x = 0; x < 50; ++x)
                {
                    //Console.WriteLine("message ke-{0}: " + (char)message[24 + iterator + x], x);
                    //room_id = (byte)((message[24 + iterator + (5 * x)] << 4 & 0xFF) | (message[24 + iterator + ((5 * x) + 2)]));
                    room.room_id += (char)message[24 + iterator + x];
                    room.creatorIPAddress += (char)message[24 + (iterator + 50) + x];
                }
                room.room_id.Normalize();
                room.creatorIPAddress.Normalize();
                iterator += 100;
                Debug.WriteLine("roomID[{0}]: " + room.room_id + "\ncreatorIPAddress[{1}]: " + room.creatorIPAddress, i, i);

                rooms.Add(room);
            }

            return rooms;
        }

        public static Room getRoom(byte[] message)
        {
            int iterator = 0;
            byte[] result = new byte[4];
            Array.Copy(message, 20 + iterator, result, 0, 4);
            int peer_id = BitConverter.ToInt32(result, 0);
            Debug.WriteLine("peer_id: " + peer_id);
            iterator += 4;

            result = new byte[4];
            Array.Copy(message, 20 + iterator, result, 0, 4);
            int maxplayer = BitConverter.ToInt32(result, 0);
            Debug.WriteLine("maxplayer: " + maxplayer);
            iterator += 4;

            result = new byte[4];
            Array.Copy(message, 20 + iterator, result, 0, 4);
            int neighbor_count = BitConverter.ToInt32(result, 0);
            iterator += 4;
            Debug.WriteLine("neighbor_count: " + neighbor_count);

            Room room = new Room();
            room.peer_id = peer_id;
            room.maxplayer = maxplayer;
            for (int j = 0; j < neighbor_count; j++)
            {
                result = new byte[4];
                Array.Copy(message, 20 + iterator, result, 0, 4);
                int peerID = BitConverter.ToInt32(result, 0);

                Debug.WriteLine("peerID[{0}]: " + peerID, j);

                Peer p = new Peer();
                p.peer_id = peerID;
                iterator += 4;
                room.neighbor.Add(p);
            }

            room.room_id = "";
            for (int x = 0; x < 50; ++x)
            {
                //Console.WriteLine("message ke-{0}: " + (char)message[24 + iterator + x], x);
                //room_id = (byte)((message[24 + iterator + (5 * x)] << 4 & 0xFF) | (message[24 + iterator + ((5 * x) + 2)]));
                room.room_id += (char)message[20 + iterator + x];
                room.creatorIPAddress += (char)message[20 + (iterator + 50) + x];
            }
            room.room_id = normalizeString(room.room_id);
            room.creatorIPAddress = normalizeString(room.creatorIPAddress);
            Debug.WriteLine("roomID: " + room.room_id + "\ncreatorIPAddress: " + room.creatorIPAddress);
            
            return room;
        }

        public static byte[] dataToByte(string data, int allocation)
        {
            byte[] result = new byte[allocation];
            for (int i = 0; i < allocation; ++i)
            {
                if (i < data.Length)
                {
                    result[i] = (byte)data[i];
                }
            }
            return result;
        }

        public static string normalizeString(string data)
        {
            string result = "";
            int i = 0;
            while (i < data.Length)
            {
                if (data[i] != 0)
                {
                    result += data[i];
                }
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

        public static byte[] HandShake()
        {
            List<byte> buffer = new List<byte>();
            buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
            buffer.AddRange(RESERVED);
            buffer.Add(HANDSHAKE_CODE);
            return buffer.ToArray();
        }

        public static byte[] HandShakeResponse(int peer_id)
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

        public static byte[] Room(ArrayList rooms)
        {
            List<byte> buffer = new List<byte>();
            buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
            buffer.AddRange(RESERVED);
            buffer.Add(ROOM_CODE);
            buffer.AddRange(BitConverter.GetBytes(rooms.Count));
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

        public static byte[] RefreshInsideRoom(int peer_id)
        {
            List<byte> buffer = new List<byte>();
            buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
            buffer.AddRange(RESERVED);
            buffer.Add(INSIDEROOM_CODE);
            buffer.AddRange(BitConverter.GetBytes(peer_id));
            return buffer.ToArray();
        }

        public static byte[] InsideRoom(Room room)
        {
            List<byte> buffer = new List<byte>();
            buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
            buffer.AddRange(RESERVED);
            buffer.Add(INSIDEROOM_CODE);
            buffer.AddRange(room.roomToBytes());
            return buffer.ToArray();
        }

        public static byte[] Dummy()
        {
            List<byte> buffer = new List<byte>();
            buffer.AddRange(dataToByte(PSTR, PSTR_SIZE));
            buffer.AddRange(RESERVED);
            buffer.Add(DUMMY_CODE);
            return buffer.ToArray();
        }
    }


}
