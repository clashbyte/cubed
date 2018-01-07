using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Core;
using Cubed.Graphics;
using Cubed.Input;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Cubed.UI {
	
	/// <summary>
	/// Console for debug
	/// </summary>
	internal sealed class Console {

		/// <summary>
		/// Is console dropped down
		/// </summary>
		public bool Enabled {
			get;
			set;
		}

		/// <summary>
		/// Vertical offset
		/// </summary>
		float offset;

		/// <summary>
		/// Main console font
		/// </summary>
		Font consoleFont;

		/// <summary>
		/// Background tile
		/// </summary>
		Texture backTile;

		/// <summary>
		/// Background offset
		/// </summary>
		float backOffset;

		/// <summary>
		/// Lines
		/// </summary>
		public Stack<string> Lines {
			get;
			private set;
		}


		public Console() {
			Enabled = false;
			offset = 640;
		}

		/// <summary>
		/// Updating console
		/// </summary>
		/// <param name="tween">Tween</param>
		/// <param name="state">Engine state</param>
		public void Update(float tween, InputState state) {

			// Console input
			if (state.KeyHit(Key.Tilde)) {
				Enabled = !Enabled;
			}
			if (Enabled) {
				offset = Math.Max(offset - tween * 80f, 320f);

				

			} else {
				offset = Math.Min(offset + tween * 80f, 640f);
			}

			// Toggling fullscreen
			if (state.KeyHit(Key.F11)) {
				Engine.Current.Fullscreen = !Engine.Current.Fullscreen;
			}
			if (state.KeyHit(Key.F12)) {
				Engine.Current.Close();
			}

			// Calculating back offset
			backOffset = (backOffset + tween * 0.3f) % 640f;

		}

		/// <summary>
		/// Rendering console
		/// </summary>
		public void Render() {
			
			// Skip rendering if console is invisible
			if(offset >= 640) {
				return;
			}

			// Caching texture
			if(backTile == null) {
				backTile = new Texture(Resources.ConsoleTile);
				backTile.Filtering = Texture.FilterMode.Enabled;
			}

			// Caching font
			if (consoleFont == null) {
				Texture t = new Texture(Resources.ConsoleFontAtlas);
				t.Filtering = Texture.FilterMode.Enabled;
				consoleFont = new Font(Resources.ConsoleFontData, t);
			}

			// Matrices
			Vector2 res = Engine.Current.ScreenSize;
			float width	 = 640f * (res.X / res.Y);
			Matrix4 proj = Matrix4.CreateOrthographicOffCenter(0, width, 640, 0, -1, 10);
			Matrix4 modl = Matrix4.Identity;

			// Setting up GL
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref proj);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modl);
			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			// Background
			Drawing.FilledBox(0, -offset, width, 640, Color.White, backTile, 1.5f, backOffset, backOffset);
			Drawing.Line(0, -offset + 641, width, -offset + 641, Color.FromArgb(20, 20, 20), 2);

			// Drawing lines
			int fontSize = 16;
			float fontPos = 640f - offset - fontSize * 1.4f;
			while (fontPos >= -fontSize) {
				consoleFont.Render("ABCDEFGHabcdefgh", 0, fontPos, fontSize, Color.LightBlue);
				fontPos -= fontSize;
			}
			// Disabling extensions
			GL.Disable(EnableCap.Texture2D);
			GL.Disable(EnableCap.Blend);
		}

	}
}
