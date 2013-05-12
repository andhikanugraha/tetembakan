using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Dynamics.Contacts;

namespace GunBond
{
	public enum Activity {Jumping, Running, Shooting, Idle}

	public class Player : Character
	{
		private World world;

		public Fixture wheel;
		public Fixture cannon;
		public FixedAngleJoint fixedAngleJoint;
		public RevoluteJoint motor;
		public RevoluteJoint turret;
		private float centerOffset;
		
		public Activity activity;
		protected Activity oldActivity;
		
		private Vector2 jumpForce;
		private float jumpDelayTime;
		
		private const float nextJumpDelayTime = 1f;
		private const float runSpeed = 8;
		private const float rotateSpeed = 2;
		private const float jumpImpulse = -200;
		
		private Vector2 bodyOrigin;
		private Vector2 cannonOrigin;
		private Vector2 turretOrigin;
		private Vector2 wheelOrigin;

		private CharacterTexture chTexture;
		private Texture2D healthTexture;

		protected override void Dispose (bool disposing)
		{
			if (disposing)
			{
				// world.RemoveJoint(motor);
				motor = null;
				world.RemoveBody(wheel.Body);
				// wheel.Dispose();
				wheel = null;
				// world.RemoveJoint(turret);
				turret = null;
				world.RemoveBody(cannon.Body);
				// cannon.Dispose();
				cannon = null;
				world.ProcessChanges();
			}
			base.Dispose (disposing);
		}

		private static bool getKeyState(int param, bool keyup, Keys k) {
			if (!keyup) {
				switch(k) {
				case Keys.A :
					return (param&1) > 0;
				case Keys.S :
					return (param&2) > 0;
				case Keys.D :
					return (param&4) > 0;
				case Keys.W :
					return (param&8) > 0;
				case Keys.Space :
					return (param&16) > 0;
				case Keys.Enter :
					return (param&32) > 0;
				default:
					return false;
				}
			} else {
				switch(k) {
				case Keys.A :
					return (param&64) > 0;
				case Keys.S :
					return (param&128) > 0;
				case Keys.D :
					return (param&256) > 0;
				case Keys.W :
					return (param&512) > 0;
				case Keys.Space :
					return (param&1024) > 0;
				case Keys.Enter :
					return (param&2048) > 0;
				default:
					return false;
				}
			}
		}

		public Player(World world, Vector2 position, float width, float height, float mass, int turn, Texture2D texture, CharacterTexture chTexture)
			: base(world, position, width, height, mass, turn, texture)
		{
			this.world = world;

			this.chTexture = chTexture;

			this.bodyOrigin = new Vector2(chTexture.body.Width / 2, chTexture.body.Height / 2);
			this.cannonOrigin = new Vector2(chTexture.cannon.Width / 2, chTexture.cannon.Height / 2);
			this.turretOrigin = new Vector2(chTexture.turret.Width / 2, chTexture.turret.Height / 2);
			this.wheelOrigin = new Vector2(chTexture.wheel.Width / 2, chTexture.wheel.Height / 2);

			if (width > height)
			{
				throw new Exception("Error width > height: can't make character because wheel would stick out of body");
			}
			
			activity = Activity.Idle;
			
			wheel.OnCollision += new OnCollisionEventHandler(OnCollision);
			fixture.OnCollision += new OnCollisionEventHandler(BodyOnCollision);
			cannon.OnCollision += new OnCollisionEventHandler(BodyOnCollision);
		}
		
		protected override void SetUpPhysics(World world, Vector2 position, float width, float height, float mass)
		{
			//Create a fixture with a body almost the size of the entire object
			//but with the bottom part cut off.
			float upperBodyHeight = height - (width / 2);
			
			fixture = FixtureFactory.CreateRectangle(world, (float)ConvertUnits.ToSimUnits(width), (float)ConvertUnits.ToSimUnits(upperBodyHeight), mass / 2);
			body = fixture.Body;
			fixture.Body.BodyType = BodyType.Dynamic;
			fixture.Restitution = 0.3f;
			fixture.Friction = 0.5f;
			//also shift it up a tiny bit to keey the new object's center correct
			body.Position = ConvertUnits.ToSimUnits(position - (Vector2.UnitY * (width / 4)));
			centerOffset = position.Y - (float)ConvertUnits.ToDisplayUnits(body.Position.Y); //remember the offset from the center for drawing
			
			//Now let's make sure our upperbody is always facing up.
			fixedAngleJoint = JointFactory.CreateFixedAngleJoint(world, body);
			
			//Create a wheel as wide as the whole object
			wheel = FixtureFactory.CreateCircle(world, (float)ConvertUnits.ToSimUnits(width / 2), mass / 2);
			//And position its center at the bottom of the upper body
			wheel.Body.Position = body.Position + ConvertUnits.ToSimUnits(Vector2.UnitY * (upperBodyHeight / 2));
			wheel.Body.BodyType = BodyType.Dynamic;
			wheel.Restitution = 0.3f;
			wheel.Friction = 0.5f;
			
			//These two bodies together are width wide and height high :)
			//So lets connect them together
			motor = JointFactory.CreateRevoluteJoint(world, body, wheel.Body, Vector2.Zero);
			motor.MotorEnabled = true;
			motor.MaxMotorTorque = 1000f; //set this higher for some more juice
			motor.MotorSpeed = 0;

			cannon = FixtureFactory.CreateRectangle(world, (float)ConvertUnits.ToSimUnits(width / 2), (float)ConvertUnits.ToSimUnits(height / 2), mass / 2);
			//cannon.Body.Position = new Vector2(body.Position.X - (float)ConvertUnits.ToSimUnits(width) / 12, body.Position.Y - (float)ConvertUnits.ToSimUnits(height) / 3);
			cannon.Body.Position = body.Position - ConvertUnits.ToSimUnits(Vector2.UnitY * width);
			cannon.Body.BodyType = BodyType.Dynamic;
			cannon.Restitution = 0.3f;
			cannon.Friction = 0.5f;

			turret = JointFactory.CreateRevoluteJoint(world, body, cannon.Body, Vector2.Zero);
			turret.MotorEnabled = true;
			turret.MaxMotorTorque = 1000f;
			turret.MotorSpeed = 0;
			
			//Make sure the two fixtures don't collide with each other
			wheel.CollisionFilter.IgnoreCollisionWith(fixture);
			fixture.CollisionFilter.IgnoreCollisionWith(wheel);
			cannon.CollisionFilter.IgnoreCollisionWith(fixture);
			cannon.CollisionFilter.IgnoreCollisionWith(wheel);
			fixture.CollisionFilter.IgnoreCollisionWith(cannon);
			wheel.CollisionFilter.IgnoreCollisionWith(cannon);
			
			//Set the friction of the wheel to float.MaxValue for fast stopping/starting
			//or set it higher to make the character slip.
			wheel.Friction = float.MaxValue;
		}
		
		//Fired when we collide with another object. Use this to stop jumping
		//and resume normal movement
		public bool OnCollision(Fixture fix1, Fixture fix2, Contact contact)
		{
			//Check if we are both jumping this frame and last frame
			//so that we ignore the initial collision from jumping away from 
			//the ground
			if (activity == Activity.Jumping && oldActivity == Activity.Jumping)
			{
				activity = Activity.Idle;
			}
			return true;
		}

		public bool BodyOnCollision(Fixture fix1, Fixture fix2, Contact contact)
		{
			if (fix2.Body.IsBullet == true)
			{
				health -= 10;
			}
			return true;
		}

		protected override void HandleInput(GameTime gameTime)
		{
			oldActivity = activity;
			keyState = Keyboard.GetState();
			int param1 = 0;
			//bool goAhead = true;
			//Console.WriteLine ("turn = " + turn + " playernum = " + (GameplayScreen.playernum -1) );
			if (turn == GameplayScreen.playernum -1) {
				if (keyState.IsKeyDown (Keys.A))
					param1 += 1;
				if (keyState.IsKeyDown (Keys.S))
					param1 += 2;
				if (keyState.IsKeyDown (Keys.D))
					param1 += 4;
				if (keyState.IsKeyDown (Keys.W))
					param1 += 8;
				if (keyState.IsKeyDown (Keys.Space))
					param1 += 16;
				if (keyState.IsKeyDown (Keys.Enter))
					param1 += 32;
				if (keyState.IsKeyUp (Keys.A))
					param1 += 64;
				if (keyState.IsKeyUp (Keys.S))
					param1 += 128;
				if (keyState.IsKeyUp (Keys.D))
					param1 += 256;
				if (keyState.IsKeyUp (Keys.W))
					param1 += 512;
				if (keyState.IsKeyUp (Keys.Space))
					param1 += 1024;
				if (keyState.IsKeyUp (Keys.Enter))
					param1 += 2048;
				if (param1 > 0 && param1 != 4032) {
					Peer.Instance.updateRoom (param1, gameTime.ElapsedGameTime.TotalSeconds);
					//Console.WriteLine (param1);
					Console.WriteLine("A");
				}
				if (GameplayScreen.playernum == 1) {
					HandleJumping (param1, oldState, gameTime);
					HandleAiming (param1, oldState, gameTime);
					
					if (activity != Activity.Jumping)
					{
						HandleRunning(param1, oldState, gameTime);
					}
					
					if (activity != Activity.Jumping && activity != Activity.Running)
					{
						HandleShooting(param1, oldState, gameTime);
					}
					
					oldState = param1;
				}
			}
			if ((turn == GameplayScreen.playernum - 1 && (GameplayScreen.playernum != 1)) || turn != GameplayScreen.playernum - 1) {
				if (Peer.Instance.messageQueue.Count != 0) {
					Message msgToProcess = Peer.Instance.messageQueue.Dequeue();
					param1 = msgToProcess.gameUpdate.param1;
					long thisTimestamp = msgToProcess.timestamp;
					Console.WriteLine("A");

					//thisTimestamp = 1000000 * thisTimestamp;
					//thisTimestamp += (long) (1000000 * Peer.Instance.msgReceived.gameUpdate.param2);

					/*
					if (thisTimestamp <= GameplayScreen.lastUpdate)
						goAhead = false;
					else {
						GameplayScreen.lastUpdate = thisTimestamp;
						goAhead = true;
						Console.WriteLine("A");
					}
					*/

					//Console.WriteLine("lu = " + GameplayScreen.lastUpdate.ToString() + " | tt = " + thisTimestamp.ToString() + " | goahead = " + goAhead.ToString()); 
				}
				else 
					param1 = 4032;
				HandleJumping (param1, oldState, gameTime);
				HandleAiming (param1, oldState, gameTime);
				
				if (activity != Activity.Jumping)
				{
					HandleRunning(param1, oldState, gameTime);
				}
				
				if (activity != Activity.Jumping && activity != Activity.Running)
				{
					HandleShooting(param1, oldState, gameTime);
				}
				
				oldState = param1;
			}


			/*
			if (!goAhead) {
				param1 = 4032;
			}
			*/

		}
		
		private void HandleJumping(int state, int oldState, GameTime gameTime)
		{
			if (jumpDelayTime < 0)
			{
				jumpDelayTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			
			if (getKeyState(state,false,Keys.Space) && activity != Activity.Jumping && activity != Activity.Shooting)
			{
				if (jumpDelayTime >= 0)
				{
					motor.MotorSpeed = 0;
					jumpForce.Y = jumpImpulse;
					body.ApplyLinearImpulse(jumpForce, body.Position);
					jumpDelayTime = -nextJumpDelayTime;
					activity = Activity.Jumping;
				}
			}
			
			if (activity == Activity.Jumping)
			{
				if (getKeyState(state,false,Keys.D))
				{
					if (body.LinearVelocity.X < 0)
					{
						body.LinearVelocity = new Vector2(-body.LinearVelocity.X * 2, body.LinearVelocity.Y);
					}
				}
				else if (getKeyState(state,false,Keys.A))
				{
					if (body.LinearVelocity.X > 0)
					{
						body.LinearVelocity = new Vector2(-body.LinearVelocity.X * 2, body.LinearVelocity.Y);
					}
				}
			}
		}
		
		private void HandleRunning(int state, int oldState, GameTime gameTime)
		{
			if (getKeyState(state,false,Keys.D) && p == null && activity != Activity.Shooting)
			{
				if (ConvertUnits.ToDisplayUnits(body.Position.X) > 800 - (width / 2))
				{
					motor.MotorSpeed = 0;
				}
				else
				{
					motor.MotorSpeed = runSpeed;
				}
				activity = Activity.Running;
			}
			else if (getKeyState(state,false,Keys.A) && p == null && activity != Activity.Shooting)
			{
				if (ConvertUnits.ToDisplayUnits(body.Position.X) < width / 2)
				{
					motor.MotorSpeed = 0;
				}
				else
				{
					motor.MotorSpeed = -runSpeed;
				}
				activity = Activity.Running;
			}

			if (getKeyState(state,true,Keys.D) && getKeyState(state,true,Keys.A) && activity != Activity.Shooting)
			{
				motor.MotorSpeed = 0;
				activity = Activity.Idle;
			}
		}

		private void HandleAiming(int state, int oldState, GameTime gameTime)
		{
			if (getKeyState(state,false,Keys.W) && p == null && activity != Activity.Shooting)
			{
				if (cannon.Body.Rotation < Math.PI / 2)
				{
					turret.MotorSpeed = rotateSpeed;
				}
				else
				{
					turret.MotorSpeed = 0;
				}
			}
			else if (getKeyState(state,false,Keys.S) && p == null && activity != Activity.Shooting)
			{
				if (cannon.Body.Rotation > -Math.PI / 2)
				{
					turret.MotorSpeed = -rotateSpeed;
				}
				else
				{
					turret.MotorSpeed = 0;
				}
			}

			if (getKeyState(state,true,Keys.W) && getKeyState(state,true,Keys.S))
			{
				turret.MotorSpeed = 0;
			}

		}

		private void HandleShooting(int state, int oldState, GameTime gameTime)
		{
			if (getKeyState(state,false,Keys.Enter) && getKeyState(oldState,true,Keys.Enter) && p == null)
			{
				shootPower = 0f;
				increasePower = true;
				activity = Activity.Shooting;
			}
			if (getKeyState(state,false,Keys.Enter) && getKeyState(oldState,false,Keys.Enter) && p == null)
			{
				if (increasePower == true)
				{
					if (shootPower < 40f)
					{
						shootPower += 0.2f;
					}
					if (shootPower >= 40f)
					{
						increasePower = false;
					}
				}
				else if (increasePower == false)
				{
					if (shootPower > 0f)
					{
						shootPower -= 0.2f;
					}
					if (shootPower <= 0f)
					{
						increasePower = true;
					}
				}
			}
			if (getKeyState(state,true,Keys.Enter) && getKeyState(oldState,false,Keys.Enter) && p == null)
			{
				p = new Projectile(world, new Vector2(ConvertUnits.ToDisplayUnits(cannon.Body.Position.X) + (float)Math.Cos(cannon.Body.Rotation - (float)Math.PI / 2) * 50, ConvertUnits.ToDisplayUnits(cannon.Body.Position.Y) + (float)Math.Sin(cannon.Body.Rotation - (float)Math.PI / 2) * 50), 
				                   16, 16, 1, cannon.Body.Rotation - (float)Math.PI / 2, shootPower, wind, chTexture.projectile);
				activity = Activity.Idle;
				shootPower = 0f;
			}
		}
		
		public override void Draw(SpriteBatch spriteBatch)
		{
			// Draw, body, wheel, cannon, and turret parts independently
			spriteBatch.Draw(chTexture.body, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y), (int)width, (int)(height - (width / 2))), 
			                 null, Color.White, body.Rotation, bodyOrigin, SpriteEffects.None, 0f);
			spriteBatch.Draw(chTexture.wheel, new Rectangle((int)ConvertUnits.ToDisplayUnits(wheel.Body.Position.X), (int)ConvertUnits.ToDisplayUnits(wheel.Body.Position.Y), (int)width, (int)width), 
			                 null, Color.White, wheel.Body.Rotation, wheelOrigin, SpriteEffects.None, 0f);
			spriteBatch.Draw(chTexture.cannon, new Rectangle((int)ConvertUnits.ToDisplayUnits(cannon.Body.Position.X), (int)ConvertUnits.ToDisplayUnits(cannon.Body.Position.Y), (int)width / 2, (int)height / 2), 
			                 null, Color.White, cannon.Body.Rotation, new Vector2(cannonOrigin.X, cannonOrigin.Y + (height / 4)), SpriteEffects.None, 0f);
			spriteBatch.Draw(chTexture.turret, new Rectangle((int)ConvertUnits.ToDisplayUnits(cannon.Body.Position.X), (int)ConvertUnits.ToDisplayUnits(cannon.Body.Position.Y), (int)width, (int)height / 2), 
			                 null, Color.White, 0f, turretOrigin, SpriteEffects.None, 0f);

			//This last draw call shows how to draw these two bodies with one texture (drawn semi-transparent here so you can see the inner workings)            
			// spriteBatch.Draw(texture, new Rectangle((int)Position.X, (int)(Position.Y), (int)width, (int)height), null, new Color(1, 1, 1, 0.5f), body.Rotation, origin, SpriteEffects.None, 0f);

			// Draw projectile
			if (p != null)
			{
				p.Draw(spriteBatch);
			}
			// Draw health
			if (health == 100)
			{
				healthTexture = chTexture.healthBar[0];
			}
			else if (health >= 80 && health < 100)
			{
				healthTexture = chTexture.healthBar[1];
			}
			else if (health >= 60 && health < 80)
			{
				healthTexture = chTexture.healthBar[2];
			}
			else if (health >= 40 && health < 60)
			{
				healthTexture = chTexture.healthBar[3];
			}
			else if (health >= 20 && health < 40)
			{
				healthTexture = chTexture.healthBar[4];
			}
			else if (health >= 0 && health < 20)
			{
				healthTexture = chTexture.healthBar[5];
			}
			spriteBatch.Draw(healthTexture, new Rectangle((int) Position.X - 30, (int)Position.Y - 100, (int)(width * health /50), 16), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			// Draw shoot power
			if (keyState.IsKeyDown(Keys.Enter))
			{
				spriteBatch.Draw(chTexture.shootMeter, new Rectangle(0, 0, 24, (int)(5 * shootPower)), new Rectangle(0, 0, 24, (int)(5 * shootPower)), 
				                 Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			}
		}
		
		public override Vector2 Position
		{
			get
			{
				return (ConvertUnits.ToDisplayUnits(body.Position) + Vector2.UnitY * centerOffset);
			}
		}
	}
}

