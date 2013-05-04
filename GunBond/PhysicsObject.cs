using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;

namespace GunBond
{
	public class PhysicsObject : IDisposable
	{
		protected float width;
		protected float height;
		public Body body;
		public Fixture fixture;
		protected Texture2D texture;
		protected Vector2 origin;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				texture = null;
				fixture.Dispose();
				fixture = null;
				body = null;
			}
		}
		
		~PhysicsObject()
		{
			Dispose(false);
		}
		
		public PhysicsObject (World world, Vector2 position, float width, float height, float mass, Texture2D texture)
		{
			this.texture = texture;
			this.origin = new Vector2(texture.Width / 2, texture.Height / 2);
			this.width = width;
			this.height = height;

			SetUpPhysics(world, position, width, height, mass);
		}

		protected virtual void SetUpPhysics(World world, Vector2 position, float width, float height, float mass)
		{
			fixture = FixtureFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(width), ConvertUnits.ToSimUnits(height), mass, ConvertUnits.ToSimUnits(position));
			body = fixture.Body;
			fixture.Body.BodyType = BodyType.Dynamic;
			fixture.Restitution = 0.3f;
			fixture.Friction = 0.5f;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)height), null, Color.White, body.Rotation, origin, SpriteEffects.None, 0f);
		}

		public virtual Vector2 Position
		{
			get
			{
				return body.Position;
			}
		}

		private int round(float toRound)
		{
			float remainder = toRound - (int)toRound;
			if (remainder >= 0.5)
			{
				return ((int)toRound + 1);
			}
			else
			{
				return (int)toRound;
			}
		}
	}
}

