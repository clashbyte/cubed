using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Graphics;
using Cubed.Input;
using Cubed.UI.Themes;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubed.UI.Basic {

	/// <summary>
	/// Label control
	/// </summary>
	public class Label : Control {

		/// <summary>
		/// Label text
		/// </summary>
		public string Text {
			get;
			set;
		}

		/// <summary>
		/// Text size
		/// </summary>
		public float FontSize {
			get;
			set;
		}

		/// <summary>
		/// Font
		/// </summary>
		public Font Font {
			get;
			set;
		}

		/// <summary>
		/// Current color
		/// </summary>
		public Color Color {
			get;
			set;
		}

		/// <summary>
		/// Text horizontal align
		/// </summary>
		public UserInterface.Align HorizontalAlign {
			get;
			set;
		}

		/// <summary>
		/// Text vertical align
		/// </summary>
		public UserInterface.Align VerticalAlign {
			get;
			set;
		}

		/// <summary>
		/// Graphical font
		/// </summary>
		static Font font;

		/// <summary>
		/// Label constructor
		/// </summary>
		public Label() {
			Enabled = true;
			Position = Vector2.Zero;
			Size = new Vector2(200, 40);
			HorizontalAlign = UserInterface.Align.Middle;
			VerticalAlign = UserInterface.Align.Middle;
			Text = "Label";
			FontSize = 16f;
			Color = Color.White;
		}

		/// <summary>
		/// Rendering label
		/// </summary>
		internal override void Render() {
			InterfaceTheme.Current.DrawLabel(RealPosition.X, RealPosition.Y, RealSize.X, RealSize.Y, new InterfaceTheme.LabelData() {
				Font = this.Font,
				FontSize = this.FontSize,
				FontColor = this.Color,
				Text = this.Text,
				HorizontalAlign = this.HorizontalAlign,
				VerticalAlign = this.VerticalAlign
			});
		}

		/// <summary>
		/// Update label logic
		/// </summary>
		internal override void Update(InputState state, float delta) {
			
		}


	}
}
