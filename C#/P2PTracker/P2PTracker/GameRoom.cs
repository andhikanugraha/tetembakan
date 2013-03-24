using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P2PTracker
{
    public class GameRoom
    {
        List<int> listOfPeerID { get; set; }
        string room_id;
        int max_player_num { get; set; }

        public GameRoom()
        {
            room_id = "";
            listOfPeerID = new List<int>();
            max_player_num = 8;
        }

        public GameRoom(string room_id, int new_max_player)
        {
            max_player_num = new_max_player;
        }

        public void addPlayer(int peer_id)
        {
            if (listOfPeerID.Count < max_player_num)
            {
                listOfPeerID.Add(peer_id);
            }
            else
            {
                // penuh
                return;
            }
        }

        public static List<byte[]> roomToBytes(GameRoom room)
        {
            List<byte[]> buffer = new List<byte[]>();
            buffer.Add(BitConverter.GetBytes(room.max_player_num));
            buffer.Add(BitConverter.GetBytes(room.listOfPeerID.Count));
            foreach (int id in room.listOfPeerID)
            {  
                buffer.Add(BitConverter.GetBytes(id));
            }

            byte[] temp = new byte[room.room_id.Length * sizeof(char)];
            Buffer.BlockCopy(room.room_id.ToCharArray(), 0, temp, 0, temp.Length);
            buffer.Add(temp);
            return buffer;
        }

        public static GameRoom bytesToRoom(List<byte[]> data)
        {
            GameRoom gameRoom = new GameRoom();
            gameRoom.max_player_num = BitConverter.ToInt16(data[0], 0);
            int id_num = BitConverter.ToInt16(data[1], 0);
            for (int i = 0; i < id_num; ++i)
            {
                gameRoom.listOfPeerID.Add(BitConverter.ToInt16(data[i + 2], 0));
            }

            char[] room_id = new char[data.Last().Length / sizeof(char)];
            System.Buffer.BlockCopy(data.Last(), 0, room_id, 0, data.Last().Length);
            gameRoom.room_id = new string(room_id);
            return gameRoom;
        }
    }
}
