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
		StaticPhysicsObject ground;
		StaticPhysicsObject wall;
		Player box;
		Player box2;

		int turn = 0;

		public Game1 ()
		{
			Window.Title = "GunBond";
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = false;		
			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
			world =  new World(new Vector2(0, 9.82f));
			ConvertUnits.SetDisplayUnitToSimUnitRatio(30);
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
			ground = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width / 2, 500), GraphicsDevice.Viewport.Width, 64, squareTexture);
			wall = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width / 2, 400), 32, 150, squareTexture);
			box = new Player(world, new Vector2(100, 0), 32, 64, 20, 0, squareTexture);
			box.forcePower = 50;
			box2 = new Player(world, new Vector2(700, 0), 32, 64, 20, 1, squareTexture);
			box2.forcePower = 50;

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
			if (turn == 0)
			{
				turn = box.Update(gameTime);
			}
			else
			{
				turn = box2.Update(gameTime);
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
			box.Draw(spriteBatch);
			box2.Draw(spriteBatch);
			ground.Draw(spriteBatch);
			wall.Draw(spriteBatch);
			if (turn == 0)
			{
				if (box.wind != 0) spriteBatch.DrawString(spriteFont, (box.wind >= 0 ? ">> " : "<< ") + Math.Abs(box.wind).ToString(), new Vector2(400, 0), Color.Black);
				else spriteBatch.DrawString(spriteFont, 0.ToString(), new Vector2(400, 0), Color.Black);
			}
			else if (turn == 1)
			{
				if (box2.wind != 0) spriteBatch.DrawString(spriteFont, (box2.wind >= 0 ? ">> " : "<< ") + Math.Abs(box2.wind).ToString(), new Vector2(400, 0), Color.Black);
				else spriteBatch.DrawString(spriteFont, 0.ToString(), new Vector2(400, 0), Color.Black);
			}
			spriteBatch.End();

			base.Draw (gameTime);
		}
	}
}

