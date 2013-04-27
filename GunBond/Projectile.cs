using System;

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
	public class Projectile : PhysicsObject
	{
		public int destroySig = 0;

		public Projectile (World world, Vector2 position, float width, float height, float mass, float angle, float shootPower, Texture2D texture) : base(world, position, width, height, mass, texture)
		{
			body.LinearVelocity = new Vector2((float)Math.Cos(angle) * shootPower, (float)Math.Sin(angle) * shootPower);

			fixture.OnCollision += new OnCollisionEventHandler(OnCollision);
		}

		public bool OnCollision(Fixture fix1, Fixture fix2, Contact contact)
		{
			destroySig = 1;
			return true;
		}
	}
}

