using System;
using DGui;
using Microsoft.Xna.Framework;

namespace GunBond
{
	public class Screen : DrawableGameComponent
	{
		protected Game1 game;
		
		protected DGuiManager guiManager;
		protected DLayoutFlow layout;
		protected DForm form;
		
		public Screen (Game1 game): base(game)
		{
			this.game = game;
		}

		public override void Initialize()
		{
			base.Initialize();
		}
		
		protected override void LoadContent()
		{
			base.LoadContent();

			
			guiManager = new DGuiManager(game, game.spriteBatch);
			
			layout = new DLayoutFlow(1, 12, DLayoutFlow.DLayoutFlowStyle.Vertically);
			layout.Position = new Vector2(10, 10);
			
			form = new DForm(guiManager, "Gunbound", null);
			form.Size = new Vector2(800, 600);
			form.Position = new Vector2(0, 0);
			form.Initialize();
			guiManager.AddControl(form);
		}
		
		public override void Update(GameTime gameTime)
		{
			guiManager.Update(gameTime);
		}
		
		public override void Draw(GameTime gameTime)
		{
			guiManager.Draw(gameTime);
		}
	}
}

