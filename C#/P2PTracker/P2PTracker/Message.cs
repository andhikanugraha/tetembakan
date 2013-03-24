using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.PeerToPeer;

namespace P2PTracker
{
    public class Message
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

        List<byte[]> message;

        public List<byte[]> getMessage()
        {
            return message;
        }
        public void setMessage(List<byte[]> new_message)
        {
            message = new_message;
        }

        public Message()
        {
            message = new List<byte[]>();
        }

        public Message(List<byte[]> new_message)
        {
            message = new_message;
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

        public static Message KeepAlive(int peer_id)
        {
            List<byte[]> buffer = new List<byte[]>();
            buffer.Add(dataToByte(PSTR, PSTR_SIZE));
            buffer.Add(RESERVED);
            buffer.Add(new byte[]{ KEEP_ALIVE_CODE });
            buffer.Add(BitConverter.GetBytes(peer_id));
            return new Message(buffer); 
        }

        public static Message Create(int new_peer_id, int max_player_num, string new_room_id)
        {
            List<byte[]> buffer = new List<byte[]>();
            buffer.Add(dataToByte(PSTR, PSTR_SIZE));
            buffer.Add(RESERVED);
            buffer.Add(new byte[] { CREATE_CODE });
            buffer.Add(BitConverter.GetBytes(new_peer_id));
            buffer.Add(BitConverter.GetBytes(max_player_num));
            buffer.Add(dataToByte(new_room_id, ROOM_ID_SIZE));
            return new Message(buffer);
        }

        public static Message List(int peer_id)
        {
            List<byte[]> buffer = new List<byte[]>();
            buffer.Add(dataToByte(PSTR, PSTR_SIZE));
            buffer.Add(RESERVED);
            buffer.Add(new byte[] { LIST_CODE });
            buffer.Add(BitConverter.GetBytes(peer_id));
            return new Message(buffer);
        }

        public static Message Room(int room_count, List<GameRoom> rooms)
        {
            List<byte[]> buffer = new List<byte[]>();
            buffer.Add(dataToByte(PSTR, PSTR_SIZE));
            buffer.Add(RESERVED);
            buffer.Add(new byte[] { ROOM_CODE });
            buffer.Add(BitConverter.GetBytes(room_count));
            foreach(GameRoom room in rooms)
            {
                buffer.AddRange(GameRoom.roomToBytes(room));
            }
            return new Message(buffer); 
        }

        public static Message Success()
        {
            List<byte[]> buffer = new List<byte[]>();
            buffer.Add(dataToByte(PSTR, PSTR_SIZE));
            buffer.Add(RESERVED);
            buffer.Add(new byte[] { SUCCESS_CODE });
            return new Message(buffer); 
        }

        public static Message Failed()
        {
            List<byte[]> buffer = new List<byte[]>();
            buffer.Add(dataToByte(PSTR, PSTR_SIZE));
            buffer.Add(RESERVED);
            buffer.Add(new byte[] { FAILED_CODE });
            return new Message(buffer);
        }

        public static Message Join(int peer_id, string room_id)
        {
            List<byte[]> buffer = new List<byte[]>();
            buffer.Add(dataToByte(PSTR, PSTR_SIZE));
            buffer.Add(RESERVED);
            buffer.Add(new byte[] { JOIN_CODE });
            buffer.Add(BitConverter.GetBytes(peer_id));
            buffer.Add(dataToByte(room_id, ROOM_ID_SIZE));
            return new Message(buffer);
        }

        public static Message Start(int peer_id, string room_id)
        {
            List<byte[]> buffer = new List<byte[]>();
            buffer.Add(dataToByte(PSTR, PSTR_SIZE));
            buffer.Add(RESERVED);
            buffer.Add(new byte[] { START_CODE });
            buffer.Add(BitConverter.GetBytes(peer_id));
            buffer.Add(dataToByte(room_id, ROOM_ID_SIZE));
            return new Message(buffer);
        }

        public static Message Quit(int peer_id)
        {
            List<byte[]> buffer = new List<byte[]>();
            buffer.Add(dataToByte(PSTR, PSTR_SIZE));
            buffer.Add(RESERVED);
            buffer.Add(new byte[] { QUIT_CODE });
            buffer.Add(BitConverter.GetBytes(peer_id));
            return new Message(buffer);
        }

    }
}
