using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Core;
using OpenTK;
using OpenTK.Input;

namespace Cubed.Input {
	
	/// <summary>
	/// Current engine input state
	/// </summary>
	internal class InputState {

		/// <summary>
		/// Mouse location
		/// </summary>
		public Vector2 Mouse {
			get;
			private set;
		}

		/// <summary>
		/// Mouse movement delta
		/// </summary>
		public Vector2 MouseDelta {
			get;
			private set;
		}

		/// <summary>
		/// Mouse wheel delta
		/// </summary>
		public float MouseWheel {
			get;
			private set;
		}

		/// <summary>
		/// Keyboard key states
		/// </summary>
		KeyState[] keyboardState;

		/// <summary>
		/// Mouse key states
		/// </summary>
		KeyState[] mouseState;

		/// <summary>
		/// State constructor
		/// </summary>
		/// <param name="previous">Previous state</param>
		/// <param name="current">Current state</param>
		public InputState(Snapshot previous, Snapshot current) {
			
			// Calculating keyboard
			keyboardState = new KeyState[current.Keyboard.Length];
			for (int i = 0; i < keyboardState.Length; i++) {
				bool old = previous.Keyboard[i];
				bool cur = current.Keyboard[i];
				if (cur) {
					keyboardState[i] = old ? KeyState.Down : KeyState.Pressed;
				} else {
					keyboardState[i] = old ? KeyState.Released : KeyState.Up;
				}
			}

			// Calculating mouse
			mouseState = new KeyState[current.Mouse.Length];
			for (int i = 0; i < mouseState.Length; i++) {
				bool old = previous.Mouse[i];
				bool cur = current.Mouse[i];
				if (cur) {
					mouseState[i] = old ? KeyState.Down : KeyState.Pressed;
				} else {
					mouseState[i] = old ? KeyState.Released : KeyState.Up;
				}
			}

			// Handling mouse shift
			Mouse = current.MousePosition;
			MouseDelta = current.MouseDeltaPosition.Xy - previous.MouseDeltaPosition.Xy;
			MouseWheel = current.MouseDeltaPosition.Z - previous.MouseDeltaPosition.Z;
		}


		/// <summary>
		/// Keyboard key down check
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>True if key is down</returns>
		public bool KeyDown(Key key) {
			return keyboardState[(int)key] == KeyState.Pressed || keyboardState[(int)key] == KeyState.Down;
		}

		/// <summary>
		/// Keyboard key just-hit check
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>True if key just pressed</returns>
		public bool KeyHit(Key key) {
			return keyboardState[(int)key] == KeyState.Pressed;
		}

		/// <summary>
		/// Keyboard key just released
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>True if key just pressed</returns>
		public bool KeyReleased(Key key) {
			return keyboardState[(int)key] == KeyState.Released;
		}


		/// <summary>
		/// Mouse button down check
		/// </summary>
		/// <param name="button">Mouse button</param>
		/// <returns>True if button is down</returns>
		public bool MouseDown(MouseButton button) {
			return mouseState[(int)button] == KeyState.Pressed || keyboardState[(int)button] == KeyState.Down;
		}

		/// <summary>
		/// Mouse button press check
		/// </summary>
		/// <param name="button">Mouse button</param>
		/// <returns>True if button is pressed</returns>
		public bool MouseHit(MouseButton button) {
			return mouseState[(int)button] == KeyState.Pressed;
		}

		/// <summary>
		/// Mouse button release check
		/// </summary>
		/// <param name="button">Mouse button</param>
		/// <returns>True if button released</returns>
		public bool MouseReleased(MouseButton button) {
			return mouseState[(int)button] == KeyState.Released;
		}

		/// <summary>
		/// Keyboard and mouse input snapshot
		/// </summary>
		public class Snapshot {

			/// <summary>
			/// Keyboard state
			/// </summary>
			public bool[] Keyboard;

			/// <summary>
			/// Mouse state
			/// </summary>
			public bool[] Mouse;

			/// <summary>
			/// Mouse location
			/// </summary>
			public Vector2 MousePosition;

			/// <summary>
			/// Mouse delta for movement
			/// </summary>
			internal Vector3 MouseDeltaPosition;

			/// <summary>
			/// Snapshot constructor
			/// </summary>
			public Snapshot() {
				Keyboard = new bool[256];
				Mouse = new bool[10];
				MousePosition = Vector2.Zero;
				MouseDeltaPosition = Vector3.Zero;
			}

			/// <summary>
			/// Snapshot from input state constructor
			/// </summary>
			/// <param name="mouse">Mouse input</param>
			/// <param name="keyboard">Keyboard input</param>
			/// <param name="cursor">Mouse coords</param>
			public Snapshot(MouseState mouse, KeyboardState keyboard, Point cursor) : this() {
				for (int i = 0; i < 256; i++) {
					Keyboard[i] = keyboard.IsKeyDown((short)i);
				}
				for (int i = 0; i < 10; i++) {
					Mouse[i] = mouse.IsButtonDown((MouseButton)i);
				}
				MousePosition = new Vector2(cursor.X, cursor.Y);
				MouseDeltaPosition = new Vector3(mouse.X, mouse.Y, mouse.WheelPrecise);
			}

			/// <summary>
			/// Clearing snapshot to specified state
			/// </summary>
			/// <param name="master"></param>
			public void ClearTo(Snapshot master) {
				Keyboard = new bool[256];
				Mouse = new bool[10];
				MousePosition = Vector2.Zero;
				MouseDeltaPosition = Vector3.Zero;
				if (master != null) {
					MousePosition = master.MousePosition;
					MouseDeltaPosition = master.MouseDeltaPosition;
				}
			}
		}

		/// <summary>
		/// Keyboard and mouse key states
		/// </summary>
		public enum KeyState {

			/// <summary>
			/// Fully up
			/// </summary>
			Up,

			/// <summary>
			/// Just pressed
			/// </summary>
			Pressed,

			/// <summary>
			/// Held down
			/// </summary>
			Down,

			/// <summary>
			/// Just released
			/// </summary>
			Released
		}
	}
}
