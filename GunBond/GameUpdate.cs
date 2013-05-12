using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GunBond
{
	public class GameUpdate
	{
		public int param1;
		public double param2;
		
		public GameUpdate(int param1, double param2)
		{
			this.param1 = param1;
			this.param2 = param2;
		}
		
		// Concstruct Room Object from ByteArray Data
		public GameUpdate(byte[] roomByte)
		{
			int offset = 0;
			this.param1 = BitConverter.ToInt32(roomByte, offset); offset += 4;
			this.param2 = BitConverter.ToDouble(roomByte, offset); offset += 32;
		}
		
		// Convert this ROOM to ByteArray
		public byte[] toByte()
		{
			List<byte> gameUpdateByte = new List<byte>();
			gameUpdateByte.AddRange(BitConverter.GetBytes(param1));
			gameUpdateByte.AddRange(BitConverter.GetBytes(param2));
			
			return gameUpdateByte.ToArray();
		}
		
		public override string ToString()
		{
			return param1+" "+param2;
		}
	}
}
