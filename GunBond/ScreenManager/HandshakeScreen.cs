using System;
using Microsoft.Xna.Framework;
using DGui;

namespace GunBond
{
	public class HandshakeScreen : Screen
	{
		DText gundboundLabel;
		DTextBox ipAddressTextBox;
		DTextBox usernameTextBox;
		DButton handshakeButton;

		public HandshakeScreen (Game1 game): base(game)
		{

		}

		public override void Initialize()
		{
			base.Initialize();
		}
		
		protected override void LoadContent()
		{
			base.LoadContent();

			// init gundbound label
			
			gundboundLabel = new DText(guiManager);
			gundboundLabel.FontName = "Miramonte";
			layout.Add(gundboundLabel);
			gundboundLabel.Initialize();
			gundboundLabel.Text = "Gunbound";
			gundboundLabel.Position = new Vector2(400, 200) - (gundboundLabel.Size / 2);
			gundboundLabel.HorizontalAlignment = DText.DHorizontalAlignment.Center;
			form.AddPanel(gundboundLabel);

			// init ip address textbox
			ipAddressTextBox = new DTextBox(guiManager);
			layout.Add(ipAddressTextBox);
			ipAddressTextBox.Initialize();
			ipAddressTextBox.Text = "127.0.0.1";
			ipAddressTextBox.Position = new Vector2(400, 250) - (ipAddressTextBox.Size / 2);
			form.AddPanel(ipAddressTextBox);

			// init username textbox
			usernameTextBox = new DTextBox(guiManager);
			layout.Add(usernameTextBox);
			usernameTextBox.Initialize();
			usernameTextBox.Text = "username";
			usernameTextBox.Position = new Vector2(400, 300) - (usernameTextBox.Size / 2);
			form.AddPanel(usernameTextBox);

			// init handshake button
			handshakeButton = new DButton(guiManager);
			layout.Add(handshakeButton);
			handshakeButton.Text = "Connect";
			handshakeButton.Position = new Vector2(400, 350) - (handshakeButton.Size / 2);
			handshakeButton.Initialize();
			form.AddPanel(handshakeButton);
			handshakeButton.OnClick += new DButtonEventHandler(handshakeButton_OnClick);
		}
		
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (game.peer.state == GameState.Lobby)
			{
				game.currentScreen = ScreenState.Lobby;
			}
		}
		
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}

		void handshakeButton_OnClick(GameTime gameTime)
		{
			if (game.peer.state == GameState.Connect)
			{
				string trackerIPAddress = ipAddressTextBox.Text;
				if (ipAddressTextBox.Text.Equals("localhost"))
					trackerIPAddress = "127.0.0.1";
				
				game.peer.connectToTracker(trackerIPAddress, usernameTextBox.Text);
			}
		}
	}
}

