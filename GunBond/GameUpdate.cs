using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GunBond
{
    class GameUpdate
    {
        int param1;
        int param2;
        int param3;

        public GameUpdate(int param1, int param2, int param3)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
        }

        // Concstruct Room Object from ByteArray Data
        public GameUpdate(byte[] roomByte)
        {
            int offset = 0;
            this.param1 = BitConverter.ToInt32(roomByte, offset); offset += 4;
            this.param2 = BitConverter.ToInt32(roomByte, offset); offset += 4;
            this.param3 = BitConverter.ToInt32(roomByte, offset); offset += 4;
        }

        // Convert this ROOM to ByteArray
        public byte[] toByte()
        {
            List<byte> gameUpdateByte = new List<byte>();
            gameUpdateByte.AddRange(BitConverter.GetBytes(param1));
            gameUpdateByte.AddRange(BitConverter.GetBytes(param2));
            gameUpdateByte.AddRange(BitConverter.GetBytes(param3));

            return gameUpdateByte.ToArray();
        }

        public override string ToString()
        {
            return ""+param1+param2+param3;
        }
    }
}
