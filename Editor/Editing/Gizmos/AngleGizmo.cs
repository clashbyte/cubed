using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Cubed.Components.Rendering;
using OpenTK;

namespace Cubed.Editing.Gizmos {
	
	/// <summary>
	/// Position gizmo
	/// </summary>
	public class AngleGizmo : Gizmo {

		/// <summary>
		/// Internal gizmo angle
		/// </summary>
		public float Angle {
			get {
				return angle;
			}
			set {
				angle = value;
				Angles = Vector3.UnitY * angle;
			}
		}

		/// <summary>
		/// Delegate for value filtering
		/// </summary>
		/// <param name="gizmo">Current gizmo</param>
		/// <param name="target">Target value</param>
		/// <returns>Value to set</returns>
		public delegate float PositionFilter(AngleGizmo gizmo, float target);

		/// <summary>
		/// Filter for position
		/// </summary>
		public PositionFilter Filter {
			get;
			set;
		}

		/// <summary>
		/// Dragging start position
		/// </summary>
		float startPosition;

		/// <summary>
		/// Pick plane position
		/// </summary>
		Vector3 planePos;

		/// <summary>
		/// Box size
		/// </summary>
		float boxSize;

		/// <summary>
		/// Distance of box
		/// </summary>
		float boxDist;

		/// <summary>
		/// Box color
		/// </summary>
		Color boxColor;

		/// <summary>
		/// Current size lerp
		/// </summary>
		float currentSize;

		/// <summary>
		/// Angle
		/// </summary>
		float angle;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="color">Gizmo color</param>
		/// <param name="size">Gizmo size</param>
		public AngleGizmo(Color color, float size = 0.3f, float range = 0.3f) {
			AddComponent(new LineComponent() {
				WireColor = color,
				Mode = LineComponent.LineType.Pairs
			});
			BuildArrow(range, size);
			boxSize = size;
			boxColor = color;
			boxDist = range;
		}

		/// <summary>
		/// Assigning to scene
		/// </summary>
		/// <param name="scene">Scene</param>
		/// <param name="parent">Parent</param>
		public override void Assign(World.Scene scene, World.Entity parent) {
			Parent = parent;

			scene.Entities.Add(this);
		}

		/// <summary>
		/// Unassigning from scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Unassign(World.Scene scene) {
			Parent = null;

			scene.Entities.Remove(this);
		}

		/// <summary>
		/// Checking for pick in frame
		/// </summary>
		/// <param name="rayPos">Ray position</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="hitDist">Output hit distance</param>
		/// <returns>True if picked</returns>
		public override bool Hit(Vector3 rayPos, Vector3 rayDir, out float hitDist) {
			Vector3 min = Position + CalculateOffset(angle) - Vector3.One * (boxSize / 2f);
			Vector3 max = min + Vector3.One * boxSize;
			return Maths.Intersections.RayIntersectsBox(rayPos, rayDir, min, max, out hitDist);
		}

		/// <summary>
		/// Starting drag
		/// </summary>
		/// <param name="rayPos">Ray position</param>
		/// <param name="rayDir">Ray direction</param>
		public override void StartIteraction(Vector3 rayPos, Vector3 rayDir, bool ctrl) {
			float hitDist = 0f;
			Vector3 min = Position + CalculateOffset(angle) - Vector3.One * (boxSize / 2f);
			Vector3 max = min + Vector3.One * boxSize;
			Maths.Intersections.RayIntersectsBox(rayPos, rayDir.Normalized(), min, max, out hitDist);

			// Checking pick plane
			Vector3 hitPos = rayPos + rayDir.Normalized() * hitDist;
			

			planePos = hitPos;
			startPosition = angle;

		}

		/// <summary>
		/// Updating iteraction
		/// </summary>
		/// <param name="rayPos">Ray position</param>
		/// <param name="rayDir">Ray direction</param>
		public override void UpdateIteraction(Vector3 rayPos, Vector3 rayDir) {
			Vector3 hitPos = Vector3.Zero;
			if (Maths.Intersections.RayPlane(planePos, Vector3.UnitY, rayPos, rayDir.Normalized(), out hitPos)) {
				Vector3 raw = hitPos - Position;
				float angle = (float)Math.Round(MathHelper.RadiansToDegrees((float)Math.Atan2(raw.X, raw.Z)) / 5f) * 5f;

				Angle = angle;
				
				/*
				pos = startPosition;
				Vector3 dif = hitPos - planePos;
				if (vertMode) {
					pos.Y += dif.Y;
				} else {
					pos.X += dif.X;
					pos.Z += dif.Z;
				}
				if (Filter != null) {
					//pos = Filter(this, pos);
				}
				Position = pos;
				 */
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rayPos"></param>
		/// <param name="rayDir"></param>
		public override void EndIteraction(Vector3 rayPos, Vector3 rayDir) {

		}

		/// <summary>
		/// Updating
		/// </summary>
		public override void Update(Vector3 rayPos, Vector3 rayDir, float tween, bool current, out System.Windows.Forms.Cursor cursor) {
			base.Update(rayPos, rayDir, tween, current, out cursor);
			currentSize = Math.Max(Math.Min(currentSize + (current ? 0.2f : -0.2f), 1f), 0f);
			BuildArrow(boxDist, boxSize);
			
			if (current) {
				cursor = System.Windows.Forms.Cursors.Cross;
			}
		}

		/// <summary>
		/// Calculating box angle
		/// </summary>
		/// <param name="ang">Angle</param>
		/// <returns>Offset vector</returns>
		Vector3 CalculateOffset(float ang) {
			ang = (float)MathHelper.DegreesToRadians(ang);
			float x = (float)Math.Sin(ang);
			float z = (float)Math.Cos(ang);
			return new Vector3(x, 0, z) * (boxDist + boxSize / 2f);
		}

		/// <summary>
		/// Building arrow
		/// </summary>
		/// <param name="offset">Offset</param>
		/// <param name="size">Size</param>
		void BuildArrow(float offset, float size) {
			size = (size - 0.05f * (1f - currentSize));
			float near = offset, far = offset + size;
			float half = size / 2f;


			GetComponent<LineComponent>().Vertices = new Vector3[] {
				new Vector3(-half, half, near),
				new Vector3(half, half, near),
				new Vector3(half, half, near),
				new Vector3(half, -half, near),
				new Vector3(half, -half, near),
				new Vector3(-half, -half, near),
				new Vector3(-half, -half, near),
				new Vector3(-half, half, near),
				new Vector3(-half, half, near),
				new Vector3(0, 0, far),
				new Vector3(half, half, near),
				new Vector3(0, 0, far),
				new Vector3(-half, -half, near),
				new Vector3(0, 0, far),
				new Vector3(half, -half, near),
				new Vector3(0, 0, far),
			};
		}

	}
}
