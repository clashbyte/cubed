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
	public class PositionGizmo : Gizmo {

		/// <summary>
		/// Axis locking
		/// </summary>
		public AxisLock Lock {
			get;
			set;
		}

		/// <summary>
		/// Delegate for value filtering
		/// </summary>
		/// <param name="gizmo">Current gizmo</param>
		/// <param name="target">Target value</param>
		/// <returns>Value to set</returns>
		public delegate Vector3 PositionFilter(PositionGizmo gizmo, Vector3 target);

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
		Vector3 startPosition;

		/// <summary>
		/// Pick plane position
		/// </summary>
		Vector3 planePos;

		/// <summary>
		/// Pick plane direction
		/// </summary>
		Vector3 planeDir;

		/// <summary>
		/// Vertical mode
		/// </summary>
		bool vertMode;

		/// <summary>
		/// Box size
		/// </summary>
		float boxSize;

		/// <summary>
		/// Box color
		/// </summary>
		Color boxColor;

		/// <summary>
		/// Current size lerp
		/// </summary>
		float currentSize;

		

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="color">Gizmo color</param>
		/// <param name="size">Gizmo size</param>
		public PositionGizmo(Color color, float size = 0.3f) {
			Lock = AxisLock.None;
			AddComponent(new WireCubeComponent() {
				Size = Vector3.One * size,
				WireColor = color
			});
			boxSize = size;
			boxColor = color;
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
			Vector3 min = Position - Vector3.One * (boxSize / 2f);
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
			Vector3 min = Position - Vector3.One * (boxSize / 2f);
			Vector3 max = min + Vector3.One * boxSize;
			Maths.Intersections.RayIntersectsBox(rayPos, rayDir.Normalized(), min, max, out hitDist);

			// Checking pick plane
			Vector3 hitPos = rayPos + rayDir.Normalized() * hitDist;
			vertMode = Lock == AxisLock.VerticalOnly || ctrl;
			if (vertMode && Lock == AxisLock.HorizontalOnly) {
				vertMode = false;
			}

			planePos = hitPos;
			if (vertMode) {
				planeDir = -rayDir;
				planeDir.Y = 0;
				planeDir.Normalize();
			} else {
				planeDir = Vector3.UnitY;
			}
			startPosition = Position;

		}

		/// <summary>
		/// Updating iteraction
		/// </summary>
		/// <param name="rayPos">Ray position</param>
		/// <param name="rayDir">Ray direction</param>
		public override void UpdateIteraction(Vector3 rayPos, Vector3 rayDir) {
			Vector3 hitPos = Vector3.Zero;
			if (Maths.Intersections.RayPlane(planePos, planeDir.Normalized(), rayPos, rayDir.Normalized(), out hitPos)) {
				Vector3 pos = startPosition;
				Vector3 dif = hitPos - planePos;
				if (vertMode) {
					pos.Y += dif.Y;
				} else {
					pos.X += dif.X;
					pos.Z += dif.Z;
				}
				if (Filter != null) {
					pos = Filter(this, pos);
				}
				Position = pos;
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
		/// Updating iteraction
		/// </summary>
		/// <param name="rayPos"></param>
		/// <param name="rayDir"></param>
		/// <param name="tween"></param>
		/// <param name="current"></param>
		/// <param name="cursor"></param>
		public override void Update(Vector3 rayPos, Vector3 rayDir, float tween, bool current, out System.Windows.Forms.Cursor cursor) {
			base.Update(rayPos, rayDir, tween, current, out cursor);
			currentSize = Math.Max(Math.Min(currentSize + (current ? 0.2f : -0.2f), 1f), 0f);
			GetComponent<WireCubeComponent>().Size = Vector3.One * (boxSize - (boxSize * 0.25f) * (1f - currentSize));


			if (current) {
				cursor = System.Windows.Forms.Cursors.Cross;
			}
		}

		/// <summary>
		/// Gizmo movement lock
		/// </summary>
		public enum AxisLock {
			None,
			HorizontalOnly,
			VerticalOnly
		}

	}
}
