using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace Cubed.Input {

	/// <summary>
	/// Input controls
	/// </summary>
	public static class Controls {

		/// <summary>
		/// Mouse multiplier
		/// </summary>
		public static float MouseMult {
			get;
			set;
		}

		/// <summary>
		/// Mouse location
		/// </summary>
		public static Vector2 Mouse {
			get {
				if (state != null) {
					return state.Mouse;
				}
				return Vector2.Zero;
			}
		}

		/// <summary>
		/// Mouse movement speed
		/// </summary>
		public static Vector2 MouseDelta {
			get {
				if(state != null) {
					return state.MouseDelta;
				}
				return Vector2.Zero;
			}
		}

		/// <summary>
		/// Movement vector
		/// </summary>
		public static Vector2 Movement {
			get {
				if (state != null) {
					Vector2 m = Vector2.Zero;
					if (state.KeyDown(Key.Up) || state.KeyDown(Key.W)) {
						m.Y += 1f;
					}
					if (state.KeyDown(Key.Down) || state.KeyDown(Key.S)) {
						m.Y -= 1f;
					}
					if (state.KeyDown(Key.Right) || state.KeyDown(Key.D)) {
						m.X += 1f;
					}
					if (state.KeyDown(Key.Left) || state.KeyDown(Key.A)) {
						m.X -= 1f;
					}
					return m;
				}
				return Vector2.Zero;
			}
		}

		/// <summary>
		/// Check for key just pressed
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>True if key is hit</returns>
		public static bool KeyHit(Key key) {
			if (state != null) {
				return state.KeyHit(key);
			}
			return false;
		}

		/// <summary>
		/// Check for key being held
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>True if key is down</returns>
		public static bool KeyDown(Key key) {
			if (state != null) {
				return state.KeyDown(key);
			}
			return false;
		}

		/// <summary>
		/// Current state
		/// </summary>
		static InputState state;

		/// <summary>
		/// Input state updating
		/// </summary>
		/// <param name="s">Current state</param>
		internal static void Update(InputState s) {
			state = s;
		}
	

	}
}
