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

		World world;

		Texture2D groundTexture;
		Texture2D squareTexture;
		StaticPhysicsObject ground;
		CompositeCharacter box;

		public Game1 ()
		{
			Window.Title = "GunBond";
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = false;		
			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
			world =  new World(new Vector2(0f, 9.82f));
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

			// terrain
			groundTexture = Content.Load<Texture2D>("background");
			squareTexture = Content.Load<Texture2D>("square");
			ground = new StaticPhysicsObject(world, new Vector2(GraphicsDevice.Viewport.Width / 2, 500), GraphicsDevice.Viewport.Width, 64, squareTexture);
			box = new CompositeCharacter(world, new Vector2(100, 0), 32, 64, 5, squareTexture);
			box.forcePower = 50;
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
			box.Update(gameTime);
			world.Step((float)(gameTime.ElapsedGameTime.TotalMilliseconds * 0.001));

			base.Update (gameTime);
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
			ground.Draw(spriteBatch);
			spriteBatch.End();

			base.Draw (gameTime);
		}
	}
}

