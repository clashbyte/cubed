using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Input;
using OpenTK;

namespace Cubed.UI {

	/// <summary>
	/// Default control
	/// </summary>
	public abstract class Control {

		/// <summary>
		/// Flag for control enable
		/// </summary>
		public bool Enabled {
			get;
			set;
		}

		/// <summary>
		/// Position
		/// </summary>
		public Vector2 Position {
			get {
				return position;
			}
			set {
				position = value;
			}
		}

		/// <summary>
		/// Size
		/// </summary>
		public Vector2 Size {
			get {
				return size;
			}
			set {
				size = value;
			}
		}

		/// <summary>
		/// Position anchor
		/// </summary>
		public AnchorMode Anchor {
			get;
			set;
		}

		/// <summary>
		/// Internal real position
		/// </summary>
		protected Vector2 RealPosition {
			get {
				return realPosition;
			}
		}

		/// <summary>
		/// Internal real size
		/// </summary>
		protected Vector2 RealSize {
			get {
				return realSize;
			}
		}

		/// <summary>
		/// Requested position
		/// </summary>
		Vector2 position;

		/// <summary>
		/// Requested size
		/// </summary>
		Vector2 size;

		/// <summary>
		/// Calculated position
		/// </summary>
		Vector2 realPosition;

		/// <summary>
		/// Calculated size
		/// </summary>
		Vector2 realSize;
		
		/// <summary>
		/// Parameters resolution
		/// </summary>
		Vector2 paramResolution;

		/// <summary>
		/// Constructor for control
		/// </summary>
		public Control() {
			Enabled = true;
			Anchor = AnchorMode.TopLeft;
		}

		/// <summary>
		/// Rebuilding control parameters
		/// </summary>
		/// <param name="screenPosition">Position of the screen</param>
		/// <param name="screenSize">Screen size</param>
		/// <param name="input">Input state</param>
		internal void RebuildPositions(Vector2 screenPosition, Vector2 screenSize) {
			Vector2 pos = position;
			if (Anchor == AnchorMode.TopRight || Anchor == AnchorMode.BottomRight) {
				pos.X = screenSize.X - size.X - pos.X;
			}
			if (Anchor == AnchorMode.BottomLeft || Anchor == AnchorMode.BottomRight) {
				pos.Y = screenSize.Y - size.Y - pos.Y;
			}
			realPosition = pos + screenPosition;
			realSize = size;
		}

		/// <summary>
		/// Updating control logic
		/// </summary>
		internal abstract void Update(InputState state, float delta);

		/// <summary>
		/// Rendering control
		/// </summary>
		internal abstract void Render();

		/// <summary>
		/// Control anchoring mode
		/// </summary>
		public enum AnchorMode {
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight,
			Fill
		}
	}
}
