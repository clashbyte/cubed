using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Cubed.Components.Controls {

	/// <summary>
	/// Controller for movement
	/// </summary>
	public class WalkController : EntityComponent, IUpdatable {

		/// <summary>
		/// Global gravity for movement
		/// </summary>
		public static float Gravity = -0.003f;

		/// <summary>
		/// Movement velocity
		/// </summary>
		Vector3 velocity = Vector3.Zero;

		/// <summary>
		/// Управление игроком
		/// </summary>
		public void Control(Vector2 move, float angle) {
			velocity = new Vector3(move.X, velocity.Y, move.Y);
		}

		public void Jump() {
			velocity.Y += 0.08f;
		}

		/// <summary>
		/// Update logic
		/// </summary>
		void IUpdatable.Update() {
			if (Parent == null) {
				return;
			}
			Vector3 pos = Parent.Position;
			Vector3 target = velocity;

			// Current max floor height
			float h = Parent.BoxCollider.GetFloorHeight(Vector3.Zero);
			float dist = Parent.BoxCollider.Size.Y / 2f + (Parent.BoxCollider.Size.X + Parent.BoxCollider.Size.Z) / 2f;
			velocity.Y += Gravity;
			if (pos.Y - h <= dist) {
				pos.Y = h + dist;
				if (velocity.Y < 0) {
					velocity.Y = 0;
				}
			}

			Parent.BoxCollider.Velocity = velocity + (pos - Parent.Position);

		}

	}
}
