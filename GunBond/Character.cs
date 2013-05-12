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
		public float forcePower = 50;
		protected KeyboardState keyState;
		protected int oldState;
		protected Projectile p;
		protected int turn;
		public float health = 100;
		protected float shootPower;
		protected bool increasePower;
		public float wind;
		private Random r = new Random();
		public bool blowsToRight;

		protected override void Dispose (bool disposing)
		{
			if (disposing)
			{
				r = null;
			}
			base.Dispose (disposing);
		}

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
					return (turn + 1);
				}
			}
			return turn;
		}

		private void nextWind()
		{
			if (blowsToRight)
			{
				++this.wind;
			}
			else
			{
				--this.wind;
			}
			
			if ((this.wind == 4) || (this.wind == -4))
			{
				blowsToRight = !blowsToRight; // change direction
			}

			//this.wind = (float)(r.Next (-3, 4));
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

			//oldState = keyState;
		}
	}
}

