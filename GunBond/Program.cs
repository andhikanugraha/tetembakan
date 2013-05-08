#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace GunBond
{
	static class Program
	{
		private static Game1 game;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			//Console.WriteLine("Masuk");
			//int playernum = 8, players = 8, turn = 0;
			game = new Game1 ();//(playernum, players, turn);
			game.Run ();
		}
	}
}
