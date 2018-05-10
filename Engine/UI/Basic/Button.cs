using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Graphics;
using Cubed.UI.Themes;

namespace Cubed.UI.Basic {
	
	public class Button : ClickableControl {

		/// <summary>
		/// String
		/// </summary>
		public string Text {
			get;
			set;
		}

		/// <summary>
		/// Text info
		/// </summary>
		public Icons Icon {
			get;
			set;
		}

		/// <summary>
		/// Vertical display mode
		/// </summary>
		public bool Vertical {
			get;
			set;
		}

		/// <summary>
		/// Flag to render checkboxes and 
		/// </summary>
		protected virtual bool DrawInactiveAsActive {
			get {
				return false;
			}
		}

		/// <summary>
		/// Is button pressed
		/// </summary>
		bool pressed;

		/// <summary>
		/// Handling button render
		/// </summary>
		internal override void Render() {
			InterfaceTheme.ButtonData info = new InterfaceTheme.ButtonData() {
				Text = this.Text,
				Icon = this.Icon,
				Vertical = this.Vertical,
				State = InterfaceTheme.ButtonRenderState.Off
			};
			if (pressed) {
				info.State = InterfaceTheme.ButtonRenderState.Down;
			} else {
				if (DrawInactiveAsActive) {
					info.State = InterfaceTheme.ButtonRenderState.Selected;
				} else {
					if (MouseInside) {
						info.State = InterfaceTheme.ButtonRenderState.Hover;
					} else {
						info.State = InterfaceTheme.ButtonRenderState.Off;
					}
						
				}
			}
			InterfaceTheme.Current.DrawButton(RealPosition.X, RealPosition.Y, RealSize.X, RealSize.Y, info);
		}

		/// <summary>
		/// Mouse down event
		/// </summary>
		/// <param name="button"></param>
		protected override void OnPress(OpenTK.Input.MouseButton button) {
			base.OnPress(button);
			if (button == OpenTK.Input.MouseButton.Left) {
				pressed = true;
			}
		}

		/// <summary>
		/// Mouse up event
		/// </summary>
		/// <param name="button"></param>
		protected override void OnRelease(OpenTK.Input.MouseButton button) {
			base.OnRelease(button);
			if (button == OpenTK.Input.MouseButton.Left) {
				pressed = false;
				if (MouseInside) {
					OnAction();
				}
			}
		}

		/// <summary>
		/// Handling button action
		/// </summary>
		protected virtual void OnAction() {
			
		}

	}
}
