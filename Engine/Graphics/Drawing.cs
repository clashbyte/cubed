using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Cubed.Graphics {

	/// <summary>
	/// Drawing methods
	/// </summary>
	internal static class Drawing {

		/// <summary>
		/// Filled rectagle
		/// </summary>
		/// <param name="x">X</param>
		/// <param name="y">Y</param>
		/// <param name="w">Width</param>
		/// <param name="h">Height</param>
		/// <param name="tint">Color</param>
		/// <param name="tex">Texture</param>
		/// <param name="scale">Texture scale</param>
		/// <param name="shiftX">Texture X shift</param>
		/// <param name="shiftY">Texture Y shift</param>
		public static void FilledBox(float x, float y, float w, float h, Color tint, Texture tex = null, float scale = 1f, float shiftX = 0, float shiftY = 0) {
			
			// Binding texture
			float u0 = 0f, u1 = 0f, v0 = 0f, v1 = 0f;
			if(tex != null) {
				tex.Bind();
				scale = 1f / scale;
				float dx = 1f / (float)tex.Width * scale;
				float dy = 1f / (float)tex.Height * scale;
				u0 = dx * -shiftX;
				u1 = u0 + dx * w;
				v0 = dy * -shiftY;
				v1 = v0 + dy * h;
			}else{
				Texture.BindEmpty();
			}

			// Drawing quad
			GL.Color4(tint);
			GL.Begin(PrimitiveType.Quads);
			GL.TexCoord2(u0, v0);
			GL.Vertex2(x, y);
			GL.TexCoord2(u1, v0);
			GL.Vertex2(x + w, y);
			GL.TexCoord2(u1, v1);
			GL.Vertex2(x + w, y + h);
			GL.TexCoord2(u0, v1);
			GL.Vertex2(x, y + h);
			GL.End();
		}

		/// <summary>
		/// Drawing line box
		/// </summary>
		/// <param name="x">X</param>
		/// <param name="y">Y</param>
		/// <param name="w">Width</param>
		/// <param name="h">Height</param>
		/// <param name="tint">Color</param>
		/// <param name="tickness">Line width</param>
		public static void LineBox(float x, float y, float w, float h, Color tint, float tickness = 0.5f) {

			// Drawing quad
			Texture.BindEmpty();
			GL.Color4(tint);
			GL.LineWidth(tickness);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
			GL.Begin(PrimitiveType.Quads);
			GL.Vertex2(x + 1, y + 1);
			GL.Vertex2(x + w, y + 1);
			GL.Vertex2(x + w, y + h);
			GL.Vertex2(x + 1, y + h);
			GL.End();
			GL.LineWidth(0.5f);
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
		}

		/// <summary>
		/// Drawing a line
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <param name="tint"></param>
		/// <param name="tickness"></param>
		public static void Line(float x1, float y1, float x2, float y2, Color tint, float tickness = 0.5f) {

			// Drawing line
			Texture.BindEmpty();
			GL.Color4(tint);
			GL.LineWidth(tickness);
			GL.Begin(PrimitiveType.Lines);
			GL.Vertex2(x1, y1);
			GL.Vertex2(x2, y2);
			GL.End();
			GL.LineWidth(0.5f);

		}

		public static void TextLine(string text, float x, float y, float size, UI.Font font, Color tint) {
			font.Render(text, x, y, size, tint);
		}

	}
}
