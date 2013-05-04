using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GunBond
{
	public class CharacterTexture
	{
		public Texture2D body, cannon, turret, wheel, projectile, shootMeter;
		public Texture2D[] healthBar;

		public CharacterTexture (Texture2D body, Texture2D cannon, Texture2D turret, Texture2D wheel, Texture2D projectile, Texture2D shootMeter, Texture2D[] healthBar)
		{
			this.body = body;
			this.cannon = cannon;
			this.turret = turret;
			this.wheel = wheel;
			this.projectile = projectile;
			this.shootMeter = shootMeter;
			this.healthBar = healthBar;
		}
	}
}

