using System;
using Microsoft.Xna.Framework;
using DGui;

namespace GunBond
{
	public class CreateRoomScreen : Screen
	{
		DText roomNameLabel;
		DTextBox roomNameTextBox;
		DText maxPlayerLabel;
		DTextBox maxPlayerTextBox;
		DButton cancelButton;
		DButton createRoomButton;

		public CreateRoomScreen (Game1 game): base(game)
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
			roomNameLabel.Text = "Room Name:";
			roomNameLabel.Position = new Vector2(400, 100) - (roomNameLabel.Size / 2);
			roomNameLabel.HorizontalAlignment = DText.DHorizontalAlignment.Center;
			form.AddPanel(roomNameLabel);


			// init room name textbox
			roomNameTextBox = new DTextBox(guiManager);
			layout.Add(roomNameTextBox);
			roomNameTextBox.Initialize();
			roomNameTextBox.Text = "asdasd";
			roomNameTextBox.Position = new Vector2(400, 150) - (roomNameTextBox.Size / 2);
			form.AddPanel(roomNameTextBox);

			// init max players label
			maxPlayerLabel = new DText(guiManager);
			maxPlayerLabel.FontName = "Miramonte";
			layout.Add(maxPlayerLabel);
			maxPlayerLabel.Initialize();
			maxPlayerLabel.Text = "Max Players:";
			maxPlayerLabel.Position = new Vector2(400, 200) - (maxPlayerLabel.Size / 2);
			maxPlayerLabel.HorizontalAlignment = DText.DHorizontalAlignment.Center;
			form.AddPanel(maxPlayerLabel);

			// init max player textbox
			maxPlayerTextBox = new DTextBox(guiManager);
			layout.Add(maxPlayerTextBox);
			maxPlayerTextBox.Initialize();
			maxPlayerTextBox.Text = "8";
			maxPlayerTextBox.Position = new Vector2(400, 250) - (maxPlayerTextBox.Size / 2);
			form.AddPanel(maxPlayerTextBox);

			// init cancel button
			cancelButton = new DButton(guiManager);
			layout.Add(cancelButton);
			cancelButton.Text = "Cancel";
			cancelButton.Position = new Vector2(cancelButton.Size.X + 200, 400) - cancelButton.Size;
			cancelButton.Initialize();
			form.AddPanel(cancelButton);
			cancelButton.OnClick += new DButtonEventHandler(cancelButton_OnClick);


			// init create button
			createRoomButton = new DButton(guiManager);
			layout.Add(createRoomButton);
			createRoomButton.Text = "Create";
			createRoomButton.Position = new Vector2(600, 400) - createRoomButton.Size;
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
		}
		
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}

		void cancelButton_OnClick(GameTime gameTime)
		{
			game.currentScreen = ScreenState.Lobby;
			game.needToUpdateRoomList = true;
		}

		void createRoomButton_OnClick(GameTime gameTime)
		{
			if (game.peer.state == GameState.Lobby)
			{
				game.peer.createRoom(roomNameTextBox.Text, Convert.ToInt32(maxPlayerTextBox.Text));
				game.needToUpdateRoom = true;
			}
		}
	}
}

