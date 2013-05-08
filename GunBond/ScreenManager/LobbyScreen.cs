using System;
using Microsoft.Xna.Framework;
using DGui;

namespace GunBond
{
	public class LobbyScreen : Screen
	{
		DText lobbyLabel;
		DListBox roomList;
		DButton lobbyButton;
		DButton joinRoomButton;
		DButton createRoomButton;

		public LobbyScreen (Game1 game): base(game)
		{

		}

		public override void Initialize()
		{
			base.Initialize();
		}
		
		protected override void LoadContent()
		{
			base.LoadContent();

			// init lobby label
			
			lobbyLabel = new DText(guiManager);
			lobbyLabel.FontName = "Miramonte";
			layout.Add(lobbyLabel);
			lobbyLabel.Initialize();
			lobbyLabel.Text = "Room List";
			lobbyLabel.Position = new Vector2(100, 100);
			lobbyLabel.HorizontalAlignment = DText.DHorizontalAlignment.Center;
			form.AddPanel(lobbyLabel);

			// init list box
			roomList = new DListBox(guiManager);
			layout.Add(roomList);
			roomList.Initialize();
			roomList.Position = new Vector2(100, 150);
			form.AddPanel(roomList);

			// init join room button
			joinRoomButton = new DButton(guiManager);
			layout.Add(joinRoomButton);
			joinRoomButton.Text = "Join";
			joinRoomButton.Position = new Vector2(joinRoomButton.Size.X + 100, 500) - joinRoomButton.Size;
			joinRoomButton.Initialize();
			form.AddPanel(joinRoomButton);
			joinRoomButton.OnClick += new DButtonEventHandler(joinRoomButton_OnClick);

			// init create room button
			createRoomButton = new DButton(guiManager);
			layout.Add(createRoomButton);
			createRoomButton.Text = "Create";
			createRoomButton.Position = new Vector2(800 - 100, 500) - createRoomButton.Size;
			createRoomButton.Initialize();
			form.AddPanel(createRoomButton);
			createRoomButton.OnClick += new DButtonEventHandler(createRoomButton_OnClick);
		}
		
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (game.peer.state == GameState.Room)
			{
				game.currentScreen = ScreenState.Room;
			}

			if (game.needToUpdateRoomList)
			{
				game.peer.gotoLobby();
				game.needToUpdateRoomList = false;
			}

			if (game.peer.msgReceived.roomList.Count != 0) 
			{
				if (roomList.Items.Count != 0)
				{
					roomList.ClearItems();
				}

				foreach (Room room in game.peer.msgReceived.roomList)
				{
					DListBoxItem newRoom = new DListBoxItem(guiManager, room.toStringWithoutPeers2());
					roomList.AddListItem(newRoom);
				}
			}
		}
		
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}

		void joinRoomButton_OnClick(GameTime gameTime)
		{
			if (game.currentScreen == ScreenState.Lobby && roomList.SelectedIndex != -1)
			{
				string roomID = roomList.Items[roomList.SelectedIndex].Text.Split(' ')[0];
				game.peer.joinRoom(roomID);
				game.needToUpdateRoom = true;
			}
		}

		void createRoomButton_OnClick(GameTime gameTime)
		{
			game.currentScreen = ScreenState.CreateRoom;

			Console.WriteLine("Berhasil create room");
		}

	}
}

