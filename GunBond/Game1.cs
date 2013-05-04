#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Factories;

#endregion

namespace GunBond
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		SpriteFont spriteFont;

		World world;

		Texture2D squareTexture;
		Texture2D projectileTexture;
		Texture2D pointerTexture;
		StaticPhysicsObject ground;
		StaticPhysicsObject[] wall;
		Team team1, team2;
		Player[] player1, player2;
		Texture2D[] body, cannon, turret, wheel, bodya, cannona, turreta, wheela;

		int turn;
		int players;

		public Game1 ()
		{
			Window.Title = "GunBond";
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = false;		
			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
			world =  new World(new Vector2(0, 9.82f));
			wall = new StaticPhysicsObject[9];
			players = 8;
			Random r = new Random();
			turn = r.Next(players);
			player1 = new Player[players / 2];
			player2 = new Player[players / 2];
			body = new Texture2D[4];
			cannon = new Texture2D[4];
			turret = new Texture2D[4];
			wheel = new Texture2D[4];
			bodya = new Texture2D[4];
			cannona = new Texture2D[4];
			turreta = new Texture2D[4];
			wheela = new Texture2D[4];
			ConvertUnits.SetDisplayUnitToSimUnitRatio(30);
			Console.WriteLine("Game start");
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
			base.Initialize ();
				
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);

			//TODO: use this.Content to load your game content here 

			// textures
			squareTexture = Content.Load<Texture2D>("square");
			projectileTexture = Content.Load<Texture2D>("projectile");
			pointerTexture = Content.Load<Texture2D>("pointer");
			for (int i = 0; i < 4; i++)
			{
				body[i] = Content.Load<Texture2D>("body" + (i + 1));
				bodya[i] = Content.Load<Texture2D>("bodya" + (i + 1));
				cannon[i] = Content.Load<Texture2D>("Cannon" + (i + 1));
				cannona[i] = Content.Load<Texture2D>("Cannona" + (i + 1));
				turret[i] = Content.Load<Texture2D>("turret" + (i + 1));
				turreta[i] = Content.Load<Texture2D>("turreta" + (i + 1));
				wheel[i] = Content.Load<Texture2D>("wheel" + (i + 1));
				wheela[i] = Content.Load<Texture2D>("wheela" + (i + 1));
			}
			ground = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width / 2, 500), GraphicsDevice.Viewport.Width, 96, squareTexture);
			wall[0] = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width / 2, 450), 32, 128, squareTexture);
			wall[1] = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width / 2 - 32, 450), 32, 64, squareTexture);
			wall[2] = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width / 2 + 32, 450), 32, 64, squareTexture);
			wall[3] = new StaticPhysicsObject(world, new Vector2(16, 450), 32, 192, squareTexture);
			wall[4] = new StaticPhysicsObject(world, new Vector2(48, 450), 32, 128, squareTexture);
			wall[5] = new StaticPhysicsObject(world, new Vector2(80, 450), 32, 64, squareTexture);
			wall[6] = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width - 16, 450), 32, 192, squareTexture);
			wall[7] = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width - 48, 450), 32, 128, squareTexture);
			wall[8] = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width - 80, 450), 32, 64, squareTexture);
			// Team 1
			for (int i = 0, x = 16; i < player1.Length; i++, x += 64)
			{
				player1[i] = new Player(world, new Vector2(x, 0), 32, 64, 20, 2 * i, squareTexture, body[i], cannon[i], turret[i], wheel[i], projectileTexture);
			}
			team1 = new Team(player1);
			// Team 2
			// int mid = players / 2;
			for (int i = 0, x = GraphicsDevice.Viewport.Width - 16; i < player2.Length; i++, x -= 64)
			{
				player2[i] = new Player(world, new Vector2(x, 0), 32, 64, 20, (2 * i) + 1, squareTexture, bodya[i], cannona[i], turreta[i], wheela[i], projectileTexture);
			}
			team2 = new Team(player2);

			// font
			spriteFont = Content.Load<SpriteFont>("font");
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
				Exit ();
			}
			// TODO: Add your update logic here	
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
			world.Step((float)(gameTime.ElapsedGameTime.TotalMilliseconds * 0.001));

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.CornflowerBlue);
		
			//TODO: Add your drawing code here
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			ground.Draw(spriteBatch);
			for (int i = 0; i < wall.Length; i++)
			{
				wall[i].Draw(spriteBatch);
			}
			for (int i = 0; i < players / 2; i++)
			{
				if (player1[i] != null)
				{
					player1[i].Draw(spriteBatch);
					if (turn == i * 2)
					{
						spriteBatch.Draw(pointerTexture, new Vector2(player1[i].Position.X - 8, player1[i].Position.Y - 125), Color.White);
						if (player1[i].wind != 0)
						{
							spriteBatch.DrawString(spriteFont, (player1[i].wind >= 0 ? ">>" : "<<") + Math.Abs(player1[i].wind).ToString(), new Vector2(400, 0), Color.Black);
						}
						else
						{
							spriteBatch.DrawString(spriteFont, 0.ToString(), new Vector2(400, 0), Color.Black);
						}
					}
				}
				if (player2[i] != null)
				{
					player2[i].Draw(spriteBatch);
					if (turn - 1 == i * 2)
					{
						spriteBatch.Draw(pointerTexture, new Vector2(player2[i].Position.X - 8, player2[i].Position.Y - 125), Color.White);
						if (player2[i].wind != 0)
						{
							spriteBatch.DrawString(spriteFont, (player2[i].wind >= 0 ? ">>" : "<<") + Math.Abs(player2[i].wind).ToString(), new Vector2(400, 0), Color.Black);
						}
						else
						{
							spriteBatch.DrawString(spriteFont, 0.ToString(), new Vector2(400, 0), Color.Black);
						}
					}
				}
			}
			spriteBatch.End();

			base.Draw (gameTime);
		}
	}
}

