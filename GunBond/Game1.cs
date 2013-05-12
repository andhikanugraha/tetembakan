#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

	public enum ScreenState { Handshake, Lobby, CreateRoom, Room, Gameplay };
	public class Game1 : Game
	{
		public Peer peer = Peer.Instance;
		public ScreenState currentScreen;
		HandshakeScreen handshakeScreen;
		LobbyScreen lobbyScreen;
		CreateRoomScreen createRoomScreen;
		RoomScreen roomScreen;
		GameplayScreen gameplayScreen;

		public bool needToUpdateRoomList = false;
		public bool needToUpdateRoom = false;
		public bool needToInitGameplay = false;

		public GraphicsDeviceManager graphics {get; set;}
		public SpriteBatch spriteBatch {get; set;}
		public SpriteFont spriteFont;
		
		public Game1 ()
		{
			Window.Title = "GunBond";
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = false;		
			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;
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
			currentScreen = ScreenState.Handshake;
			//Console.WriteLine("Masuk init");	
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

			//load screens
			handshakeScreen = new HandshakeScreen(this);
			handshakeScreen.Initialize();

			lobbyScreen = new LobbyScreen(this);
			lobbyScreen.Initialize();

			createRoomScreen = new CreateRoomScreen(this);
			createRoomScreen.Initialize();

			roomScreen = new RoomScreen(this);
			roomScreen.Initialize();

			gameplayScreen = new GameplayScreen(this);
			gameplayScreen.Initialize();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
				Exit ();
			}

			// TODO: Add your update logic here	
			if (currentScreen == ScreenState.Handshake)
			{
				handshakeScreen.Update(gameTime);
			}
			else if (currentScreen == ScreenState.Lobby)
			{
				lobbyScreen.Update(gameTime);
			}
			else if (currentScreen == ScreenState.CreateRoom)
			{
				createRoomScreen.Update(gameTime);
			}
			else if (currentScreen == ScreenState.Room)
			{
				roomScreen.Update(gameTime);
			}
			if (currentScreen == ScreenState.Gameplay)
			{
				gameplayScreen.Update(gameTime);
			}

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
			if (currentScreen == ScreenState.Handshake)
			{
				handshakeScreen.Draw(gameTime);
			}
			else if (currentScreen == ScreenState.Lobby)
			{
				lobbyScreen.Draw(gameTime);
			}
			else if (currentScreen == ScreenState.CreateRoom)
			{
				createRoomScreen.Draw(gameTime);
			}
			else if (currentScreen == ScreenState.Room)
			{
				roomScreen.Draw(gameTime);
			}
			else if (currentScreen == ScreenState.Gameplay)
			{
				gameplayScreen.Draw(gameTime);
			}

			base.Draw (gameTime);
		}
	}
}

