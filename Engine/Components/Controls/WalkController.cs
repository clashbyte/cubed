using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Core;
using Cubed.World;
using OpenTK;

namespace Cubed.Components.Controls {

	/// <summary>
	/// Controller for movement
	/// </summary>
	public class WalkController : EntityComponent, IUpdatable, ILateUpdatable {

		/// <summary>
		/// Global gravity for movement
		/// </summary>
		public static float Gravity = -0.002f;


		/// <summary>
		/// Something attached to this walker
		/// </summary>
		public Entity Actor {
			get;
			set;
		}

		/// <summary>
		/// Position
		/// </summary>
		public float ActorPosition {
			get;
			set;
		}

		/// <summary>
		/// Walking speed
		/// </summary>
		public float WalkSpeed {
			get;
			set;
		}

		/// <summary>
		/// Running speed
		/// </summary>
		public float RunSpeed {
			get;
			set;
		}

		/// <summary>
		/// Is controller grounded
		/// </summary>
		public bool IsGrounded {
			get {
				return grounded;
			}
		}

		/// <summary>
		/// Is controller landed in this frame
		/// </summary>
		public bool IsLanded {
			get;
			set;
		}

		/// <summary>
		/// Landing velocity
		/// </summary>
		public float LandVelocity {
			get;
			private set;
		}

		/// <summary>
		/// Movement velocity
		/// </summary>
		Vector3 velocity = Vector3.Zero;

		/// <summary>
		/// Movement 
		/// </summary>
		Vector2 movement = Vector2.Zero;

		/// <summary>
		/// Running
		/// </summary>
		bool running = false;

		/// <summary>
		/// Is controller grounded
		/// </summary>
		bool grounded = false;

		/// <summary>
		/// Movement lerp
		/// </summary>
		float runLerp;

		/// <summary>
		/// Constructor
		/// </summary>
		public WalkController() {
			WalkSpeed = 0.04f;
			RunSpeed = 0.08f;
			runLerp = 0f;
		}

		/// <summary>
		/// Controls
		/// </summary>
		public void Control(Vector2 move, bool run, float angle) {
			float rad = (float)MathHelper.DegreesToRadians(-angle);
			float sin = (float)Math.Sin(rad);
			float cos = (float)Math.Cos(rad);

			// Вектор движения
			movement = new Vector2(
				cos * move.X - sin * move.Y,
				sin * move.X + cos * move.Y
			);
			if (movement.X != 0 || movement.Y != 0) {
				movement.NormalizeFast();
			}
			running = run;
			
		}

		/// <summary>
		/// Make jump
		/// </summary>
		public void Jump() {
			if (grounded) {
				velocity.Y = 0.04f;
				grounded = false;
			}
		}

		/// <summary>
		/// Resetting velocity
		/// </summary>
		public void Reset() {
			velocity = Vector3.Zero;
			grounded = false;
		}

		/// <summary>
		/// Update logic
		/// </summary>
		void IUpdatable.Update() {
			if (Parent == null) {
				return;
			}

			// Movement
			Vector3 tr = velocity;
			tr.Y = 0;
			runLerp = (float)MathHelper.Clamp(runLerp + (running ? 0.1f : -0.05f), 0, 1);
			float speed = WalkSpeed + (RunSpeed - WalkSpeed) * runLerp;
			float mult = 0.03f;
			float decMult = 0.8f;
			if (!grounded) {
				mult = 0.001f;
				decMult = 0.99f;
				speed = 0.5f;
			}

			tr += new Vector3(movement.X, 0, movement.Y) * mult;
			tr *= decMult;
			if (tr.LengthFast > speed) {
				tr = tr.Normalized() * speed;
			} else if(tr.LengthFast < 0.0001f) {
				tr = Vector3.Zero;
			}
			tr.Y = velocity.Y;
			velocity = tr;

			// Current max floor height
			Vector3 pos = Parent.Position;
			Vector3 target = velocity;
			float h = Parent.BoxCollider.GetFloorHeight(Vector3.Zero);
			float dist = Parent.BoxCollider.Size.Y / 2f + (Parent.BoxCollider.Size.X + Parent.BoxCollider.Size.Z) / 3f;
			float error = 0f;
			if (velocity.Y < 0) {
				error = 0.01f;
			}
			LandVelocity = 0;
			IsLanded = false;

			velocity.Y += Gravity;
			if (pos.Y - h < dist + error && velocity.Y <= 0) {
				if (!grounded) {
					grounded = true;
					LandVelocity = -velocity.Y;
					IsLanded = true;
				}
				pos.Y = h + dist;
				velocity.Y = Math.Max(velocity.Y, -0.1f);
			} else {
				if (grounded) {
					velocity.Y = 0;
					grounded = false;
				}
			}

			Parent.BoxCollider.Velocity = velocity + (pos - Parent.Position);

		}

		/// <summary>
		/// Late update logic
		/// </summary>
		void ILateUpdatable.LateUpdate() {
			if (Parent == null) {
				return;
			}

			/*
			if (velocity.X != 0) {
				if (Math.Sign(Parent.BoxCollider.Response.X) == Math.Sign(velocity.X)) {
					velocity.X = 0;
				}
			}
			if (velocity.Z != 0) {
				if (Math.Sign(Parent.BoxCollider.Response.Z) == Math.Sign(velocity.Z)) {
					velocity.Z = 0;
				}
			}
			 */

			if (velocity.Y > 0) {
				if (Parent.BoxCollider.Response.Y < 0) {
					velocity.Y = Gravity;
				}
			}

			Vector3 pos = Parent.Position;
			Vector3 target = velocity;
			float h = Parent.BoxCollider.GetFloorHeight(Vector3.Zero);
			float dist = Parent.BoxCollider.Size.Y / 2f + (Parent.BoxCollider.Size.X + Parent.BoxCollider.Size.Z) / 3f;
			float error = 0f;
			if (velocity.Y < 0) {
				error = 0.01f;
			}
			if (pos.Y - h < dist + error && velocity.Y <= 0) {
				Parent.BoxCollider.Velocity = -Vector3.UnitY * (pos.Y - h - dist - 0.001f);
				Parent.BoxCollider.Collide(Scene.Current.Map, Scene.Current.GetAllColliders()); 
				velocity.Y = Math.Max(velocity.Y, -0.1f);
			}

			// Setting up actor position
			if (Actor != null) {
				Actor.Position = Parent.Position + Vector3.UnitY * ActorPosition;
			}

		}

	}
}
