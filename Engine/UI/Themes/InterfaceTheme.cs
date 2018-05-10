using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Graphics;
using Cubed.UI.Basic;

namespace Cubed.UI.Themes {

	
	/// <summary>
	/// Base for interface
	/// </summary>
	public abstract class InterfaceTheme {

		/// <summary>
		/// Gets current interface theme
		/// </summary>
		internal static InterfaceTheme Current {
			get;
			set;
		}


		/// <summary>
		/// Rendering label
		/// </summary>
		/// <param name="x">X</param>
		/// <param name="y">Y</param>
		/// <param name="w">Width</param>
		/// <param name="h">Height</param>
		/// <param name="info">Info for label rendering</param>
		public virtual void DrawLabel(float x, float y, float w, float h, LabelData info) {}

		/// <summary>
		/// Rendering button
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <param name="info"></param>
		public virtual void DrawButton(float x, float y, float w, float h, ButtonData info) {}


		/// <summary>
		/// Info for label drawing
		/// </summary>
		public class LabelData {
			public string Text;
			public Font Font;
			public float FontSize;
			public Color FontColor;
			public UserInterface.Align HorizontalAlign;
			public UserInterface.Align VerticalAlign;
		}

		/// <summary>
		/// Info for button drawing
		/// </summary>
		public class ButtonData {
			public string Text;
			public Icons Icon;
			public Texture Image;
			public bool Vertical;
			public ButtonRenderState State;
		}

		/// <summary>
		/// Rendering button
		/// </summary>
		public enum ButtonRenderState {
			Off,
			Hover,
			Down,
			Selected
		}
	}
}
