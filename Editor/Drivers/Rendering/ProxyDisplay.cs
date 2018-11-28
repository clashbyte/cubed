using System;
using System.Drawing;
using Cubed.Input;
using OpenTK;

namespace Cubed.Drivers.Rendering {

	/// <summary>
	/// Display device for control
	/// </summary>
	public class ProxyDisplay : Display {

		/// <summary>
		/// Display resolution
		/// </summary>
		public override Vector2 Resolution {
			get {
				return res;
			}
		}

		/// <summary>
		/// Hidden resolution
		/// </summary>
		Vector2 res = Vector2.One;

		/// <summary>
		/// Window closing - unsupported
		/// </summary>
		public override void Close() {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Updating logics
		/// </summary>
		/// <param name="tween"></param>
		/// <param name="state"></param>
		public void UpdateLogic(float tween, InputState state, Size resolution) {
			res = new Vector2(resolution.Width, resolution.Height);
			UpdateEngine(tween, state);
		}

		/// <summary>
		/// Rendering single frame
		/// </summary>
		public void Render(Size resolution) {
			res = new Vector2(resolution.Width, resolution.Height);
			RenderFrame(1f);
		}
	}
}
