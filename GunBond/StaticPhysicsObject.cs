using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;

namespace GunBond
{
	public class StaticPhysicsObject : PhysicsObject
	{
		public StaticPhysicsObject (World world, Vector2 position, float width, float height, Texture2D texture) : base(world, position, width, height, 1, texture)
		{
			body.BodyType = BodyType.Static;
		}
	}
}

