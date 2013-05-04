using System;

namespace GunBond
{
	public class Team
	{
		Player[] players;

		public Team (Player[] players)
		{
			this.players = players;
		}

		public bool IsLose()
		{
			for (int i = 0; i < players.Length; i++)
			{
				if (players[0] == null)
				{
					return true;
				}
			}
			return false;
		}
	}
}

