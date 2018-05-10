using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Core;
using Cubed.Input;
using OpenTK;

namespace Cubed.Drivers.Rendering {
	
	/// <summary>
	/// Display device
	/// </summary>
	public abstract class Display {

		/// <summary>
		/// Current display device
		/// </summary>
		public static Display Current {
			get;
			internal set;
		}

		/// <summary>
		/// Mouse lock
		/// </summary>
		public bool MouseLock {
			get;
			set;
		}

		/// <summary>
		/// Current engine
		/// </summary>
		public Engine Engine {
			get {
				return engine;
			}
			set {
				engine = value;
			}
		}

		/// <summary>
		/// Screen size
		/// </summary>
		public virtual Vector2 Resolution {
			get {
				return Vector2.One;
			}
		}

		/// <summary>
		/// Internal engine
		/// </summary>
		Engine engine;

		/// <summary>
		/// Force engine logical update
		/// </summary>
		/// <param name="tween">Number of frames passed</param>
		/// <param name="inputState">Current input state</param>
		protected void UpdateEngine(float tween, InputState inputState) {
			if (engine != null) {
				engine.Update(this, tween, inputState);
			}
		}

		/// <summary>
		/// Render single frame
		/// </summary>
		/// <param name="tween">Number of frames passed</param>
		protected void RenderFrame(float tween) {
			if (engine != null) {
				engine.Render(this, tween);
			}
		}
	}

}
