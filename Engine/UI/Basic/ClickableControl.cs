using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Graphics;
using OpenTK;
using OpenTK.Input;

namespace Cubed.UI.Basic {


	public abstract class ClickableControl : Control {

		/// <summary>
		/// Is mouse inside
		/// </summary>
		protected bool MouseInside {
			get;
			private set;
		}

		/// <summary>
		/// Array of mouse buttons
		/// </summary>
		bool[] mouseButtons;

		/// <summary>
		/// Constructor for control
		/// </summary>
		public ClickableControl()
			: base() {
			mouseButtons = new bool[3];
		}

		/// <summary>
		/// Location
		/// </summary>
		/// <param name="state"></param>
		/// <param name="delta"></param>
		internal override void Update(Input.InputState state, float delta) {
			Vector2 pos = RealPosition;
			Vector2 size = RealSize;
			Vector2 mouse = state.Mouse - pos;
			MouseInside = true;
			if (mouse.X < 0 || mouse.Y < 0 || mouse.X > size.X || mouse.Y > size.Y) {
				MouseInside = false;
			}
			if (MouseInside) {
				for (int i = 0; i < 3; i++) {
					if (state.MouseHit((MouseButton)i)) {
						mouseButtons[i] = true;
						OnPress((MouseButton)i);
					}
				}
			}
			for (int i = 0; i < 3; i++) {
				if (state.MouseReleased((MouseButton)i) && mouseButtons[i]) {
					mouseButtons[i] = false;
					OnRelease((MouseButton)i);
				}
			}
		}

		/// <summary>
		/// Mouse pressed event
		/// </summary>
		/// <param name="button">Button</param>
		protected virtual void OnPress(MouseButton button) {}

		/// <summary>
		/// Mouse released event
		/// </summary>
		/// <param name="button">Button</param>
		protected virtual void OnRelease(MouseButton button) {}
	}
}
