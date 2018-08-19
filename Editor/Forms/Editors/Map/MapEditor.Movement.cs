using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Components.Controls;
using Cubed.World;
using OpenTK;
using OpenTK.Input;

namespace Cubed.Forms.Editors.Map {
	partial class MapEditor {

		Entity player;

		Entity playerCamHolder;

		WalkController playerController;

		Camera playerCamera;

		float playerBob, playerLandDisp;

		/// <summary>
		/// Handling controls in window
		/// </summary>
		void UpdateControls() {

			if (walkModeEnable.Checked) {

				// Moving camera
				Vector2 rot = Input.Controls.MouseDelta;
				Vector2 mov = Input.Controls.Movement * (Input.Controls.KeyDown(Key.LShift) ? 0.1f : 0.06f);
				rot *= 0.1f;

				// Moving camera
				Vector3 ang = playerCamHolder.LocalAngles + new Vector3(rot.Y, rot.X, 0);
				ang.X = Math.Sign(ang.X) * Math.Min(Math.Abs(ang.X), 90);
				ang.Y = ang.Y % 360f;
				ang.Z = 0;
				playerCamHolder.LocalAngles = ang;


				// Movement
				bool run = Input.Controls.KeyDown(Key.ShiftLeft);
				playerController.Control(mov, run, playerCamera.Angles.Y);
				if (Input.Controls.KeyHit(Key.Space)) {
					if (playerController.IsGrounded) {
						playerController.Jump();
						playerLandDisp = 0.04f;
					}
				}

				// View bobbing
				float bobDist = 0.04f;
				if (run) {
					bobDist = 0.08f;
				}
				if ((mov.X != 0 || mov.Y != 0) && playerController.IsGrounded) {
					playerBob += 0.1f;
					if (run) {
						playerBob += 0.05f;
					}
				} else {
					playerBob = 0;
				}
				float bobAng = (float)MathHelper.DegreesToRadians(playerBob);
				Vector3 bobVec = new Vector3((float)Math.Sin(playerBob) * bobDist, (float)Math.Cos(playerBob * 2f) * bobDist - bobDist, 0);
				playerCamera.LocalPosition += (bobVec - playerCamera.LocalPosition) * 0.2f;

				// Positioning cam
				Vector3 plrPos = player.Position;
				Vector3 camPos = playerCamHolder.Position;
				playerCamHolder.Position = new Vector3(plrPos.X, camPos.Y + (plrPos.Y + 0.2f - playerLandDisp - camPos.Y) * 0.3f, plrPos.Z);
				if (playerController.IsLanded) {
					playerLandDisp = Math.Min(playerController.LandVelocity * 3f, 0.5f);
				}
				playerLandDisp *= 0.85f;

				// Exiting walk mode
				if (Input.Controls.KeyHit(Key.Escape)) {
					walkModeEnable.Checked = false;
				}

			} else {

				// Changing height
				if (Input.Controls.KeyHit(Key.Q) && allowMouseLook) {
					gridHeight -= 1;
					floorIndex.Text = gridHeight + "";
				} else if (Input.Controls.KeyHit(Key.E) && allowMouseLook) {
					gridHeight += 1;
					floorIndex.Text = gridHeight + "";
				}

				// Mouse look
				Vector2 mov = Cubed.Input.Controls.Movement * (Input.Controls.KeyDown(Key.ShiftLeft) ? 0.2f : 0.08f);
				Vector3 rot = Vector3.Zero;
				if (Input.Controls.MouseHit(MouseButton.Middle) && allowMouseLook) {
					display.MouseLock = true;
				} else if (Input.Controls.MouseReleased(MouseButton.Middle) && allowMouseLook) {
					display.MouseLock = false;
				}
				if (display.MouseLock) {
					rot.X = Input.Controls.MouseDelta.Y * 0.1f;
					rot.Y = Input.Controls.MouseDelta.X * 0.1f;
				}
				cam.FreeLookControls(new Vector3(mov.X, 0, mov.Y), rot);
			}
		}


	}
}
