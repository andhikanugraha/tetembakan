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

		public Character (World world, Vector2 position, float width, float height, float mass, Texture2D texture) : base(world, position, width, height, mass, texture)
		{
		}

		public virtual void Update(GameTime gameTime)
		{
			HandleInput(gameTime);
			if (p != null)
			{
				if (p.destroySig == 1 || ConvertUnits.ToDisplayUnits(p.body.Position.X) < 0 || ConvertUnits.ToDisplayUnits(p.body.Position.Y) < 0)
				{
					p.Dispose();
					p = null;
				}
			}
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

