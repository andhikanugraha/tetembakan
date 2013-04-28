using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;

namespace GunBond
{
	public class Character : PhysicsObject
	{
		public float forcePower;
		protected KeyboardState keyState;
		protected KeyboardState oldState;
		protected Projectile p;
		protected int turn;
		protected float health = 100;
		protected float shootPower;
		protected bool increasePower;
		public float wind;
		private int direction;
		private Random r = new Random();

		public Character (World world, Vector2 position, float width, float height, float mass, int turn, Texture2D texture) : base(world, position, width, height, mass, texture)
		{
			this.turn = turn;
			nextWind();
		}

		public virtual int Update(GameTime gameTime)
		{
			HandleInput(gameTime);
			if (p != null)
			{
				p.Update(gameTime);
				if (p.destroySig == 1 || ConvertUnits.ToDisplayUnits(p.body.Position.X) < 0 || ConvertUnits.ToDisplayUnits(p.body.Position.X) > 800)
				{
					p.Dispose();
					p = null;
					nextWind();
					return (turn + 1) % 2;
				}
			}
			return turn;
		}

		private void nextWind()
		{
			do
			{
				direction = r.Next(-1, 2);
			}
			while (direction == 0);
			this.wind = (float)(direction * (r.Next (-2, 3)));
		}

		protected virtual void HandleInput(GameTime gameTime)
		{
			keyState = Keyboard.GetState();

			Vector2 force = Vector2.Zero;
			if (keyState.IsKeyDown(Keys.A))
			{
				force.X -= forcePower * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			if (keyState.IsKeyDown(Keys.D))
			{
				force.X += forcePower * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			if (keyState.IsKeyDown(Keys.W))
			{
				force.Y -= forcePower * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			if (keyState.IsKeyDown(Keys.S))
			{
				force.Y += forcePower * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}

			body.ApplyLinearImpulse(force, body.Position);

			oldState = keyState;
		}
	}
}

