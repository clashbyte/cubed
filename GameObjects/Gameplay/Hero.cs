using Cubed.Components.Controls;
using Cubed.Drivers.Rendering;
using Cubed.Maths;
using Cubed.Prefabs.Classes;
using Cubed.World;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Gameplay {

	/// <summary>
	/// Main game player
	/// </summary>
	public class Hero {

		/// <summary>
		/// Parent game loop
		/// </summary>
		public MapPlayer Parent {
			get;
			private set;
		}

		/// <summary>
		/// Player eyes
		/// </summary>
		public Camera Camera {
			get;
			private set;
		}

		/// <summary>
		/// Is player spectating
		/// </summary>
		bool spectating;

		/// <summary>
		/// Player body entity
		/// </summary>
		Entity body;

		/// <summary>
		/// Camera holder
		/// </summary>
		Entity camHolder;

		/// <summary>
		/// Movement controller
		/// </summary>
		WalkController controller;
		
		/// <summary>
		/// Floats for camera bobbing
		/// </summary>
		float bob, landDisp;

		/// <summary>
		/// Creating player
		/// </summary>
		/// <param name="parent">Parent game loop</param>
		public Hero(MapPlayer parent) {
			Parent = parent;

			Camera = new Camera();

			// Creating player
			camHolder = new Entity();
			Parent.Scene.Entities.Add(camHolder);
			controller = new WalkController();
			body = new Entity();
			body.BoxCollider = new Collider() {
				Size = new Vector3(0.35f, 0.45f, 0.35f)
			};
			body.AddComponent(controller);
			Parent.Scene.Entities.Add(body);
		}

		/// <summary>
		/// Spawning player
		/// </summary>
		/// <param name="position">Target position</param>
		/// <param name="angle">Spawn angle</param>
		public void Spawn(Vector3 position, float angle) {

			// Camera spawning
			Camera.Parent = camHolder;
			Camera.LocalPosition = Vector3.Zero;
			Camera.LocalAngles = Vector3.Zero;

			// Positioning player
			controller.Reset();
			body.Position = position;
			camHolder.Position = position;
			camHolder.LocalAngles = Vector3.UnitY * angle;
			
			spectating = false;
		}

		/// <summary>
		/// Spawn as spectator
		/// </summary>
		/// <param name="position">Position</param>
		/// <param name="angles">Rotation</param>
		public void Spectate(Vector3 position, Vector3 angles) {

			// Creating free camera
			Camera.Parent = null;
			Camera.Position = position;
			Camera.Angles = angles;
			spectating = true;

		}

		/// <summary>
		/// Updating controls
		/// </summary>
		public void Update() {
			UpdateMovement();
			UpdateInteraction();
		}

		/// <summary>
		/// Updating movement in world
		/// </summary>
		void UpdateMovement() {
			if (!spectating) {

				// Moving camera
				Vector2 rot = Input.Controls.MouseDelta;
				Vector2 mov = Input.Controls.Movement * (Input.Controls.KeyDown(Key.LShift) ? 0.1f : 0.06f);
				rot *= 0.1f;

				// Moving camera
				Vector3 ang = camHolder.LocalAngles + new Vector3(rot.Y, rot.X, 0);
				ang.X = Math.Sign(ang.X) * Math.Min(Math.Abs(ang.X), 90);
				ang.Y = ang.Y % 360f;
				ang.Z = 0;
				camHolder.LocalAngles = ang;

				// Movement
				bool run = Input.Controls.KeyDown(Key.ShiftLeft);
				controller.Control(mov, run, Camera.Angles.Y);
				if (Input.Controls.KeyHit(Key.Space)) {
					if (controller.IsGrounded) {
						controller.Jump();
						landDisp = 0.04f;
					}
				}

				// View bobbing
				float bobDist = 0.04f;
				if (run) {
					bobDist = 0.08f;
				}
				if ((mov.X != 0 || mov.Y != 0) && controller.IsGrounded) {
					bob += 0.1f;
					if (run) {
						bob += 0.05f;
					}
				} else {
					bob = 0;
				}
				float bobAng = (float)MathHelper.DegreesToRadians(bob);
				Vector3 bobVec = new Vector3((float)Math.Sin(bob) * bobDist, (float)Math.Cos(bob * 2f) * bobDist - bobDist, 0);
				Camera.LocalPosition += (bobVec - Camera.LocalPosition) * 0.2f;

				// Positioning cam
				Vector3 plrPos = body.Position;
				Vector3 camPos = camHolder.Position;
				camHolder.Position = new Vector3(plrPos.X, camPos.Y + (plrPos.Y + 0.2f - landDisp - camPos.Y) * 0.3f, plrPos.Z);
				if (controller.IsLanded) {
					landDisp = Math.Min(controller.LandVelocity * 3f, 0.5f);
				}
				landDisp *= 0.85f;
				if (Math.Abs(landDisp) < 0.00001f) {
					landDisp = 0;
				}

			} else {

				// Spectating look
				Vector2 mov = Cubed.Input.Controls.Movement * (Input.Controls.KeyDown(Key.ShiftLeft) ? 0.2f : 0.08f);
				Vector3 rot = Vector3.Zero;
				rot.X = Input.Controls.MouseDelta.Y * 0.1f;
				rot.Y = Input.Controls.MouseDelta.X * 0.1f;
				Camera.FreeLookControls(new Vector3(mov.X, 0, mov.Y), rot);

			}
		}

		/// <summary>
		/// Checking usage
		/// </summary>
		void UpdateInteraction() {
			if (!spectating) {

				// Picking object to interact with
				Vector2 center = Display.Current.Resolution * 0.5f;
				Vector3 pos = Camera.ScreenToPoint(center.X, center.Y, 0);
				Vector3 dir = (Camera.ScreenToPoint(center.X, center.Y, 1f) - pos).Normalized();
				IPlayerUsable usable = null;
				float dist = float.MaxValue;
				Vector3 pickPos = Vector3.Zero;

				// First interact with map
				MapIntersections.Hit mapHit = MapIntersections.Intersect(pos, dir, Parent.Scene.Map);
				if (mapHit != null) {
					pickPos = mapHit.Point;
					dist = (mapHit.Point - pos).Length;
				}

				// Scanning all the objects
				foreach (Entity entity in Parent.Scene.Entities) {
					if (entity is IPlayerUsable) {
						IPlayerUsable temp = entity as IPlayerUsable;
						float tempDist = 0f;
						if (temp.PickForUsing(pos, dir, out tempDist)) {
							if (tempDist < dist) {
								dist = tempDist;
								usable = temp;
								pickPos = pos + dir * tempDist;
							}
						}
					}
				}
				if (dist > 1f || (usable != null && !usable.CanUse)) {
					usable = null;
				}

				// Using object
				if (usable != null) {
					if (Input.Controls.KeyHit(Key.E)) {
						usable.Use(pickPos);
					}
				}
			}
		}
	}
}
