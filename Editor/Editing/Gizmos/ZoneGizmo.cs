using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Components.Rendering;
using OpenTK;

namespace Cubed.Editing.Gizmos {

	/// <summary>
	/// Gizmo for zones
	/// </summary>
	public class ZoneGizmo : Gizmo {

		/// <summary>
		/// Box position
		/// </summary>
		public Vector3 BoxPosition {
			get {
				return pos;
			}
			set {
				pos = value;
				Rebuild();
			}
		}

		/// <summary>
		/// Box size
		/// </summary>
		public Vector3 BoxSize {
			get {
				return size;
			}
			set {
				size = value;
				Rebuild();
			}
		}

		/// <summary>
		/// Delegate for value filtering
		/// </summary>
		/// <param name="gizmo">Current gizmo</param>
		/// <param name="target">Target value</param>
		/// <returns>Value to set</returns>
		public delegate Tuple<Vector3, Vector3> PositionFilter(ZoneGizmo gizmo, Tuple<Vector3, Vector3> target);

		/// <summary>
		/// Filter for position
		/// </summary>
		public PositionFilter Filter {
			get;
			set;
		}

		/// <summary>
		/// Hidden object gizmos
		/// </summary>
		PositionGizmo[] gizmos;

		/// <summary>
		/// Box component
		/// </summary>
		WireCubeComponent box;

		/// <summary>
		/// Box position
		/// </summary>
		new Vector3 pos = Vector3.Zero;

		/// <summary>
		/// Box size
		/// </summary>
		Vector3 size = Vector3.One;

		/// <summary>
		/// Current iteraction gizmo
		/// </summary>
		PositionGizmo iteractingGizmo;

		/// <summary>
		/// Use opposite mode
		/// </summary>
		bool oppositeMode = false;

		/// <summary>
		/// Constructor for this gizmo
		/// </summary>
		/// <param name="boxColor">Picking box color</param>
		/// <param name="boxSize">Picking box size</param>
		public ZoneGizmo(Color boxColor, float boxSize = 0.3f) {
			gizmos = new PositionGizmo[6];
			for (int i = 0; i < 6; i++) {
				gizmos[i] = new PositionGizmo(boxColor, boxSize) {
					Parent = this,
					Lock = (i == 2 || i == 3) ? PositionGizmo.AxisLock.VerticalOnly : PositionGizmo.AxisLock.HorizontalOnly,
					Filter = GizmoFilter
				};
			}
			box = new WireCubeComponent() {
				Position = pos,
				Size = size,
				WireColor = boxColor,
				WireWidth = 0.5f
			};
			AddComponent(box);
			iteractingGizmo = null;
		}

		/// <summary>
		/// Assigning to scene
		/// </summary>
		/// <param name="scene">Scene</param>
		/// <param name="parent">Parent entity</param>
		public override void Assign(World.Scene scene, World.Entity parent) {
			Parent = parent;
			scene.Entities.Add(this);
			foreach (PositionGizmo pg in gizmos) {
				scene.Entities.Add(pg);
			}
			Position = parent.Position;
		}

		/// <summary>
		/// Removing from scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Unassign(World.Scene scene) {
			Parent = null;
			scene.Entities.Remove(this);
			foreach (PositionGizmo pg in gizmos) {
				scene.Entities.Remove(pg);
			}
		}

		/// <summary>
		/// Check for intersection with boxes
		/// </summary>
		/// <param name="rayPos">Ray position</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="hitDist">Hit distance</param>
		/// <returns>True if collision happened</returns>
		public override bool Hit(OpenTK.Vector3 rayPos, OpenTK.Vector3 rayDir, out float hitDist) {
			hitDist = float.MaxValue;
			bool hit = false;
			foreach (PositionGizmo pg in gizmos) {
				float hdist = float.MaxValue;
				if (pg.Hit(rayPos, rayDir, out hdist)) {
					hitDist = Math.Min(hitDist, hdist);
					hit = true;
				}
			}
			return hit;
		}

		/// <summary>
		/// Starting iteraction with gizmos
		/// </summary>
		/// <param name="rayPos">Ray position</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="ctrl">Control key pressed</param>
		public override void StartIteraction(OpenTK.Vector3 rayPos, OpenTK.Vector3 rayDir, bool ctrl) {
			iteractingGizmo = PickGizmo(rayPos, rayDir);
			if (iteractingGizmo != null) {
				iteractingGizmo.StartIteraction(rayPos, rayDir, ctrl);
			}
			oppositeMode = !ctrl;
		}

		/// <summary>
		/// Updating iteraction
		/// </summary>
		/// <param name="rayPos">Ray position</param>
		/// <param name="rayDir">Ray direction</param>
		public override void UpdateIteraction(OpenTK.Vector3 rayPos, OpenTK.Vector3 rayDir) {
			if (iteractingGizmo != null) {
				iteractingGizmo.UpdateIteraction(rayPos, rayDir);
				if (oppositeMode) {
					int idx = Array.IndexOf(gizmos, iteractingGizmo);
					idx += (idx % 2) == 0 ? 1 : -1;
					PositionGizmo opposite = gizmos[idx];

					Vector3 center = pos + Position;
					opposite.Position = center - (iteractingGizmo.Position - center);
				}

				AdjustValues();
				if (Filter != null) {
					Tuple<Vector3, Vector3> filtered = Filter(this, new Tuple<Vector3,Vector3>(pos, size));
					pos = filtered.Item1;
					size = filtered.Item2;
				}
				Rebuild();
			}
		}

		/// <summary>
		/// Stopping iteraction
		/// </summary>
		/// <param name="rayPos">Ray position</param>
		/// <param name="rayDir">Ray direction</param>
		public override void EndIteraction(OpenTK.Vector3 rayPos, OpenTK.Vector3 rayDir) {
			if (iteractingGizmo != null) {
				iteractingGizmo.EndIteraction(rayPos, rayDir);
			}
			iteractingGizmo = null;
		}

		/// <summary>
		/// Updating object
		/// </summary>
		/// <param name="rayPos">Ray position</param>
		/// <param name="rayDir">Ray direction</param>
		/// <param name="tween">Update tween</param>
		/// <param name="current">Is this gadget current</param>
		/// <param name="cursor">Cursor to override</param>
		public override void Update(OpenTK.Vector3 rayPos, OpenTK.Vector3 rayDir, float tween, bool current, out System.Windows.Forms.Cursor cursor) {
			PositionGizmo curGizmo = null;
			if (current) {
				curGizmo = PickGizmo(rayPos, rayDir);
			}
			if (iteractingGizmo != null) {
				curGizmo = iteractingGizmo;
			}
			Angles = Vector3.Zero;
			
			cursor = System.Windows.Forms.Cursors.Arrow;
			foreach (PositionGizmo pg in gizmos) {
				System.Windows.Forms.Cursor cur;
				pg.Update(rayPos, rayDir, tween, pg == curGizmo, out cur);
				if (pg == curGizmo) {
					cursor = cur;
				}
			}
		}

		/// <summary>
		/// Picking gizmo
		/// </summary>
		/// <param name="rayPos">Position</param>
		/// <param name="rayDir">Direction</param>
		/// <returns>Nearest gizmo</returns>
		PositionGizmo PickGizmo(Vector3 rayPos, Vector3 rayDir) {
			PositionGizmo current = null;
			float dist = float.MaxValue;
			foreach (PositionGizmo pg in gizmos) {
				float hdist = float.MaxValue;
				if (pg.Hit(rayPos, rayDir, out hdist)) {
					if (hdist < dist) {
						dist = hdist;
						current = pg;
					}
				}
			}
			return current;
		}

		/// <summary>
		/// Rebuilding gizmos
		/// </summary>
		void Rebuild() {
			Vector3 p = Position + BoxPosition;
			Vector3 s = BoxSize / 2f;

			// Boxes position
			gizmos[0].Position = p + Vector3.UnitX * s;
			gizmos[1].Position = p - Vector3.UnitX * s;
			gizmos[2].Position = p + Vector3.UnitY * s;
			gizmos[3].Position = p - Vector3.UnitY * s;
			gizmos[4].Position = p + Vector3.UnitZ * s;
			gizmos[5].Position = p - Vector3.UnitZ * s;

			// Box
			box.Position = BoxPosition;
			box.Size = BoxSize;
		}

		/// <summary>
		/// Filtering gizmo input
		/// </summary>
		/// <param name="gizmo">Gizmo object</param>
		/// <param name="input">Input value</param>
		/// <returns>Output value</returns>
		Vector3 GizmoFilter(PositionGizmo gizmo, Vector3 input) {

			return input;
		}

		/// <summary>
		/// Compositing values from gizmos
		/// </summary>
		void AdjustValues() {
			Vector3 min = new Vector3(
				gizmos[1].Position.X,
				gizmos[3].Position.Y,
				gizmos[5].Position.Z
			) - Position;
			Vector3 max = new Vector3(
				gizmos[0].Position.X,
				gizmos[2].Position.Y,
				gizmos[4].Position.Z
			) - Position;
			size = max - min;
			pos = min + size / 2f;
		}
	}
}
