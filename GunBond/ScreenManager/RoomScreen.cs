using System;
using Microsoft.Xna.Framework;
using DGui;

namespace GunBond
{
	public class RoomScreen : Screen
	{
		DText roomNameLabel;
		DText creatorLabel;
		DText playersLabel;
		DListBox playerList;
		DButton quitRoomButton;
		DButton startRoomButton;
		//DButton handshakeButton;

		public RoomScreen (Game1 game): base(game)
		{

		}

		public override void Initialize()
		{
			base.Initialize();
		}
		
		protected override void LoadContent()
		{
			base.LoadContent();

			// init room name label
			
			roomNameLabel = new DText(guiManager);
			roomNameLabel.FontName = "Miramonte";
			layout.Add(roomNameLabel);
			roomNameLabel.Initialize();
			roomNameLabel.Text = "Room ";
			roomNameLabel.Position = new Vector2(100, 100);
			roomNameLabel.HorizontalAlignment = DText.DHorizontalAlignment.Left;
			form.AddPanel(roomNameLabel);

			// init room creator label
			creatorLabel = new DText(guiManager);
			creatorLabel.FontName = "Miramonte";
			layout.Add(creatorLabel);
			creatorLabel.Initialize();
			creatorLabel.Text = "Creator: ";
			creatorLabel.Position = new Vector2(100, 125);
			creatorLabel.HorizontalAlignment = DText.DHorizontalAlignment.Left;
			form.AddPanel(creatorLabel);

			// init player label
			playersLabel = new DText(guiManager);
			playersLabel.FontName = "Miramonte";
			layout.Add(playersLabel);
			playersLabel.Initialize();
			playersLabel.Text = "Players: ";
			playersLabel.Position = new Vector2(100, 150);
			playersLabel.HorizontalAlignment = DText.DHorizontalAlignment.Left;
			form.AddPanel(playersLabel);

			// init player list box
			playerList = new DListBox(guiManager);
			layout.Add(playerList);
			playerList.Initialize();
			playerList.Position = new Vector2(100, 175);
			form.AddPanel(playerList);

			// init quit room button
			quitRoomButton = new DButton(guiManager);
			layout.Add(quitRoomButton);
			quitRoomButton.Text = "Quit";
			quitRoomButton.Position = new Vector2(quitRoomButton.Size.X + 100, 500) - quitRoomButton.Size;
			quitRoomButton.Initialize();
			form.AddPanel(quitRoomButton);
			quitRoomButton.OnClick += new DButtonEventHandler(quitRoomButton_OnClick);
			
			// init start room button
			startRoomButton = new DButton(guiManager);
			layout.Add(startRoomButton);
			startRoomButton.Text = "Start";
			startRoomButton.Position = new Vector2(800 - 100, 500) - startRoomButton.Size;
			startRoomButton.Enabled = false;
			startRoomButton.Initialize();
			form.AddPanel(startRoomButton);
			startRoomButton.OnClick += new DButtonEventHandler(startRoomButton_OnClick);
		}
		
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (game.peer.state == GameState.Lobby)
			{
				game.currentScreen = ScreenState.Lobby;
			}

			if (game.peer.msgReceived.msgType == MessageType.Start)
			{
				game.currentScreen = ScreenState.Gameplay;
				game.needToInitGameplay = true;

			}

			if (game.needToUpdateRoom)
			{
				game.peer.updateRoom();
				if (playerList.Items.Count != 0)
				{
					playerList.ClearItems();
				}

				roomNameLabel.Text = "Room " + game.peer.currentRoom.getRoomID();
				creatorLabel.Text = "Creator: " + game.peer.currentRoom.getCreator();

				if (game.peer.currentRoom.getConnectedPeers().Count != 0)
				{
					foreach (PeerInfo peer in game.peer.currentRoom.getConnectedPeers())
					{
						DListBoxItem newPlayer = new DListBoxItem(guiManager, peer.ToString2());
						playerList.AddListItem(newPlayer);
					}

					if (game.peer.currentRoom.getConnectedPeers().Count == game.peer.currentRoom.getMaxPlayer())
					{
						startRoomButton.Enabled = true;
					}
				}

				game.needToUpdateRoom = false;
			}

			if (game.peer.msgReceived.msgType == MessageType.GameUpdate)
			{
				game.needToUpdateRoom = true;
			}
		}
		
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}

		void quitRoomButton_OnClick(GameTime gameTime)
		{
			if (game.currentScreen == ScreenState.Room)
			{
				game.peer.quitRoom();
				game.needToUpdateRoomList = true;
			}
		}
		
		void startRoomButton_OnClick(GameTime gameTime)
		{
			if (startRoomButton.Enabled && game.peer.isCreator)
			{
				game.currentScreen = ScreenState.Gameplay;
				game.needToInitGameplay = true;
				game.peer.startRoom();
			}
		}
	}
}

