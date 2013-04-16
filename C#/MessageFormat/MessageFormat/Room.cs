using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace MessageFormat
{
    public class Room
    {
        public Room() 
        {
            room_id = "";
            peer_id = 0;
            maxplayer = 8;
            neighbor = new List<Peer>();
            creatorIPAddress = "";
        }

        public Room(string new_room_id, int new_peer_id)
        {
            room_id = new_room_id;
            peer_id = new_peer_id;
            maxplayer = 8;
            neighbor = new List<Peer>();
            Peer peer = new Peer();
            peer.peer_id = new_peer_id;
            neighbor.Add(peer);
            creatorIPAddress = "";
        }

        public string room_id { get; set; }
        public int peer_id { get; set; }
        public string creatorIPAddress { get; set; }
        public int maxplayer { get; set; }
        public List<Peer> neighbor { get; set; }
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
            buffer.AddRange(BitConverter.GetBytes(peer_id));
            buffer.AddRange(BitConverter.GetBytes(maxplayer));
            buffer.AddRange(BitConverter.GetBytes(neighbor.Count));
            for (int i = 0; i < neighbor.Count; i++)
            {
                Peer p = neighbor[i] as Peer;
                buffer.AddRange(BitConverter.GetBytes(p.peer_id));
            }

            byte[] temp = stringTobyte(room_id);// new byte[50];//room_id.Length * sizeof(char)];
            byte[] temp2 = stringTobyte2(creatorIPAddress);
            //Debug.WriteLine(room_id.Length);

            //Buffer.BlockCopy(room_id.ToCharArray(), 0, temp, 0, temp.Length);
            buffer.AddRange(temp);
            buffer.AddRange(temp2);
            return buffer.ToArray();
        }

        public byte[] stringTobyte(string data)
        {
            byte[] result = new byte[50];//room_id.Length * sizeof(char)];

            //Debug.WriteLine(room_id.Length);

            for (int i = 0; i < 50; ++i)
            {
                if (i < data.Length)
                {
                    result[i] = (byte)((data[(3 * i)] << 4 & 0xFF) | (data[(3 * i) + 1] & 15 & 0xFF));
                }
            }

            return result;
        }

        public byte[] stringTobyte2(string data)
        {
            byte[] result = new byte[50];//room_id.Length * sizeof(char)];

            //Debug.WriteLine(room_id.Length);

            for (int i = 0; i < 50; ++i)
            {
                if (i < data.Length)
                {
                    result[i] = (byte)(data[i] & 0xFF);
                }
            }

            return result;
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
        }
        */
    }
}
