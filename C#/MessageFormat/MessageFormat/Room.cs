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
            neighbor = new ArrayList();
        }
        public string room_id { get; set; }
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
            buffer.AddRange(BitConverter.GetBytes(peer_id));
            buffer.AddRange(BitConverter.GetBytes(maxplayer));
            buffer.AddRange(BitConverter.GetBytes(neighbor.Count));
            for (int i = 0; i < neighbor.Count; i++)
            {
                Peer p = neighbor[i] as Peer;
                buffer.AddRange(BitConverter.GetBytes(p.peer_id));
            }

            byte[] temp = new byte[50];//room_id.Length * sizeof(char)];

            Debug.WriteLine(room_id.Length);

            for (int i = 0; i < 50; ++i)
            {
                if (i < room_id.Length)
                {
                    temp[i] = (byte)((room_id[(3 * i)] << 4 & 0xFF) | (room_id[(3 * i) + 1] & 15 & 0xFF));
                }
            }
            //Buffer.BlockCopy(room_id.ToCharArray(), 0, temp, 0, temp.Length);
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
}
