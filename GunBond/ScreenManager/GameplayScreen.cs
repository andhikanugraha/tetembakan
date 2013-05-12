using System;
using DGui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using System.Runtime.InteropServices;

namespace GunBond
{
	public class GameplayScreen : Screen
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);

		Texture2D backgroundTexture;
		Texture2D blockTexture;
		Texture2D squareTexture;
		Texture2D pointerTexture;
		StaticPhysicsObject ground;
		StaticPhysicsObject[] wall;
		Team team1, team2;
		Player[] player1, player2;
		Texture2D[] body, cannon, turret, wheel, bodya, cannona, turreta, wheela;

		Texture2D projectileTexture;
		Texture2D shootTexture;
		Texture2D[] healthTexture;

		World world;
		
		public int turn;
		public static int players;
		public static int playernum;
		public static long lastUpdate;


		public GameplayScreen (Game1 game): base(game)
		{

		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public void initObjects()
		{
			playernum = game.peer.currentRoom.getIDOnRoom(game.peer.peerInfo.getID());
			players = game.peer.currentRoom.getConnectedPeers().Count;
			turn = (game.peer.isCreator) ? game.peer.turn : game.peer.msgReceived.turn;

			Console.WriteLine(playernum + " : " + players + " : " + turn);
			
			player1 = new Player[players / 2];
			player2 = new Player[players / 2];

			// Team 1
			for (int i = 0, x = 16; i < player1.Length; i++, x += 64)
			{
				CharacterTexture chTexture = new CharacterTexture(body[i], cannon[i], turret[i], wheel[i], projectileTexture, shootTexture, healthTexture);
				player1[i] = new Player(world, new Vector2(x, 0), 32, 64, 20, 2 * i, squareTexture, chTexture);
			}
			team1 = new Team(player1);
			// Team 2
			// int mid = players / 2;
			for (int i = 0, x = GraphicsDevice.Viewport.Width - 16; i < player2.Length; i++, x -= 64)
			{
				CharacterTexture chTexture = new CharacterTexture(bodya[i], cannona[i], turreta[i], wheela[i], projectileTexture, shootTexture, healthTexture);
				player2[i] = new Player(world, new Vector2(x, 0), 32, 64, 20, (2 * i) + 1, squareTexture, chTexture);
			}
			team2 = new Team(player2);

		}

		protected override void LoadContent()
		{
			base.LoadContent();
			
			world =  new World(new Vector2(0, 9.82f));
			wall = new StaticPhysicsObject[9];
			body = new Texture2D[4];
			cannon = new Texture2D[4];
			turret = new Texture2D[4];
			wheel = new Texture2D[4];
			bodya = new Texture2D[4];
			cannona = new Texture2D[4];
			turreta = new Texture2D[4];
			wheela = new Texture2D[4];
			
			ConvertUnits.SetDisplayUnitToSimUnitRatio(30);

			// textures
			backgroundTexture = game.Content.Load<Texture2D>("background");
			blockTexture = game.Content.Load<Texture2D>("Block");
			squareTexture = game.Content.Load<Texture2D>("square");
			pointerTexture = game.Content.Load<Texture2D>("pointer");
			projectileTexture = game.Content.Load<Texture2D>("projectile");
			shootTexture = game.Content.Load<Texture2D>("shootMeter");
			healthTexture = new Texture2D[6];

			for (int i = 0; i < healthTexture.Length; i++)
			{
				healthTexture[i] = game.Content.Load<Texture2D>("health" + i);
			}

			for (int j = 0; j < 4; j++)
			{
				body[j] = game.Content.Load<Texture2D>("body" + (j + 1));
				bodya[j] = game.Content.Load<Texture2D>("bodya" + (j + 1));
				cannon[j] = game.Content.Load<Texture2D>("Cannon" + (j + 1));
				cannona[j] = game.Content.Load<Texture2D>("Cannona" + (j + 1));
				turret[j] = game.Content.Load<Texture2D>("turret" + (j + 1));
				turreta[j] = game.Content.Load<Texture2D>("turreta" + (j + 1));
				wheel[j] = game.Content.Load<Texture2D>("wheel" + (j + 1));
				wheela[j] = game.Content.Load<Texture2D>("wheela" + (j + 1));
			}

			ground = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width / 2, 500), GraphicsDevice.Viewport.Width, 96, blockTexture);
			wall[0] = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width / 2, 450), 32, 128, blockTexture);
			wall[1] = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width / 2 - 32, 450), 32, 64, blockTexture);
			wall[2] = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width / 2 + 32, 450), 32, 64, blockTexture);
			wall[3] = new StaticPhysicsObject(world, new Vector2(16, 450), 32, 192, blockTexture);
			wall[4] = new StaticPhysicsObject(world, new Vector2(48, 450), 32, 128, blockTexture);
			wall[5] = new StaticPhysicsObject(world, new Vector2(80, 450), 32, 64, blockTexture);
			wall[6] = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width - 16, 450), 32, 192, blockTexture);
			wall[7] = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width - 48, 450), 32, 128, blockTexture);
			wall[8] = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width - 80, 450), 32, 64, blockTexture);

			// font
			game.spriteFont = game.Content.Load<SpriteFont>("font");
		}
		
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (game.needToInitGameplay)
			{
				initObjects();
				game.needToInitGameplay = false;
			}

			for (int i = 0; i < players / 2; i++)
			{
				if (player1[i] != null)
				{
					if (player1[i].health <= 0)
					{
						player1[i].Dispose();
						player1[i] = null;
					}
				}
				if (player2[i] != null)
				{
					if (player2[i].health <= 0)
					{
						player2[i].Dispose();
						player2[i] = null;
					}
				}
			}
			if (turn % 2 == 0)
			{
				if (player1[turn / 2] != null)
				{
					turn = player1[turn / 2].Update(gameTime) % players;
				}
				else
				{
					turn = (turn + 1) % players;
				}
			}
			else
			{
				if (player2[(turn - 1) / 2] != null)
				{
					turn = player2[(turn - 1) / 2].Update(gameTime) % players;
				}
				else
				{
					turn = (turn + 1) % players;
				}
			}
			if (team1.IsLose())
			{
				if (playernum % 2 == 0)
				{
					MessageBox(new IntPtr(0), "You Lose!", "Game Over", 0);
				}
				else
				{
					MessageBox(new IntPtr(0), "You Win!", "Game Over", 0);
				}

				game.Exit();
			}
			if (team2.IsLose())
			{
				if (playernum % 2 != 0)
				{
					MessageBox(new IntPtr(0), "You Lose!", "Game Over", 0);
				}
				else
				{
					MessageBox(new IntPtr(0), "You Win!", "Game Over", 0);
				}

				game.currentScreen = ScreenState.Room;
			}

			world.Step((float)(gameTime.ElapsedGameTime.TotalMilliseconds * 0.001));
		}
		
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (!game.needToInitGameplay)
			{
				game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
				game.spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
				for (int i = 0; i < wall.Length; i++)
				{
					wall[i].Draw(game.spriteBatch);
				}
				ground.Draw(game.spriteBatch);
				for (int i = 0; i < players / 2; i++)
				{
					if (player1[i] != null)
					{
						player1[i].Draw(game.spriteBatch);
						if (turn == i * 2)
						{
							game.spriteBatch.Draw(pointerTexture, new Vector2(player1[i].Position.X - 8, player1[i].Position.Y - 125), Color.White);
							if (player1[i].wind != 0)
							{
								game.spriteBatch.DrawString(game.spriteFont, (player1[i].wind >= 0 ? ">>" : "<<") + Math.Abs(player1[i].wind).ToString(), new Vector2(400, 0), Color.White);
							}
							else
							{
								game.spriteBatch.DrawString(game.spriteFont, 0.ToString(), new Vector2(400, 0), Color.White);
							}
						}
					}
					if (player2[i] != null)
					{
						player2[i].Draw(game.spriteBatch);
						if (turn - 1 == i * 2)
						{
							game.spriteBatch.Draw(pointerTexture, new Vector2(player2[i].Position.X - 8, player2[i].Position.Y - 125), Color.White);
							if (player2[i].wind != 0)
							{
								game.spriteBatch.DrawString(game.spriteFont, (player2[i].wind >= 0 ? ">>" : "<<") + Math.Abs(player2[i].wind).ToString(), new Vector2(400, 0), Color.White);
							}
							else
							{
								game.spriteBatch.DrawString(game.spriteFont, 0.ToString(), new Vector2(400, 0), Color.White);
							}
						}
					}
				}

				game.spriteBatch.End();
			}
		}
	}
}

