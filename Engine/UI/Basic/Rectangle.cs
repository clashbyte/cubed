using System;
using System.Drawing;
using Cubed.Graphics;
using Cubed.Input;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubed.UI.Basic {

	/// <summary>
	/// Rectangle with texture or tint
	/// </summary>
	public class Quad : Control {

		/// <summary>
		/// Texture
		/// </summary>
		public Texture Texture {
			get;
			set;
		}

		/// <summary>
		/// Tinted color
		/// </summary>
		public Color Tint {
			get;
			set;
		}

		/// <summary>
		/// Texture coords
		/// </summary>
		public Vector2[] TexCoords {
			get;
			set;
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		public Quad() {
			Texture = null;
			Tint = Color.White;
		}

		/// <summary>
		/// Rendering
		/// </summary>
		internal override void Render() {

			// Computing size
			Vector2 start = RealPosition;
			Vector2 end = start + RealSize;

			// Binding texture
			GL.Color4(Tint);
			if (Texture != null && Texture.State == Texture.LoadingState.Complete) {
				Texture.Bind();
			} else {
				Texture.BindEmpty();
			}

			// Coordinates
			Vector2[] coords = null;
			if (TexCoords != null && TexCoords.Length == 4) {
				coords = new Vector2[] {
					TexCoords[0],
					TexCoords[1],
					TexCoords[3],
					TexCoords[2]
				};
			} else {
				coords = new Vector2[] {
					new Vector2(0, 0),
					new Vector2(1, 0),
					new Vector2(1, 1),
					new Vector2(0, 1)
				};
			}

			// Drawing quad
			GL.Begin(PrimitiveType.Quads);
			GL.TexCoord2(coords[0].X, coords[0].Y); GL.Vertex2(start.X, start.Y);
			GL.TexCoord2(coords[1].X, coords[1].Y); GL.Vertex2(end.X, start.Y);
			GL.TexCoord2(coords[2].X, coords[2].Y); GL.Vertex2(end.X, end.Y);
			GL.TexCoord2(coords[3].X, coords[3].Y); GL.Vertex2(start.X, end.Y);
			GL.End();
		}

		/// <summary>
		/// Updating quad
		/// </summary>
		/// <param name="state"></param>
		/// <param name="delta"></param>
		internal override void Update(InputState state, float delta) {}
	}
}
