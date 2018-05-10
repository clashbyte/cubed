using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Core;
using Cubed.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Cubed.UI {
	
	/// <summary>
	/// Graphical font
	/// </summary>
	public class Font {

		/// <summary>
		/// Texture atlas for font
		/// </summary>
		public Texture Atlas {
			get;
			private set;
		}

		/// <summary>
		/// Char definitions
		/// </summary>
		Dictionary<int, CharData> chars;

		/// <summary>
		/// Font size
		/// </summary>
		float size;

		/// <summary>
		/// Font constructor
		/// </summary>
		/// <param name="file">Data file</param>
		/// <param name="atlas">Character atlas</param>
		public Font(string file, Texture atlas) : this(Engine.Current.Filesystem.Get(file), atlas) { }

		/// <summary>
		/// Real constructor
		/// </summary>
		/// <param name="data">Raw data</param>
		/// <param name="atlas">Texture atlas</param>
		internal Font(byte[] data, Texture atlas) {
			
			// Reading data
			Atlas = atlas;
			chars = new Dictionary<int, CharData>();
			BinaryReader f = new BinaryReader(new MemoryStream(data));
			f.BaseStream.Position += 4; // Skip all intro shit
			while (f.BaseStream.Length - 1 > f.BaseStream.Position) {
				int blockType = f.ReadByte();
				int blockSize = f.ReadInt32();
				if (blockType == 4) {
					// Parse chars info
					int charCount = blockSize / 20;
					for (int i = 0; i < charCount; i++) {
						int id = f.ReadInt32();
						CharData c = new CharData();
						c.x = (float)f.ReadUInt16();
						c.y = (float)f.ReadUInt16();
						c.width = (float)f.ReadUInt16();
						c.height = (float)f.ReadUInt16();
						c.offsetX = (float)f.ReadInt16();
						c.offsetY = (float)f.ReadInt16();
						c.advance = (float)f.ReadUInt16();
						f.BaseStream.Position += 2;
						chars.Add(id, c);
					}
				} else if (blockType == 2) {
					f.BaseStream.Position += 2;
					size = (float)f.ReadUInt16();
					f.BaseStream.Position += blockSize - 4;
				} else {
					// Skip other blocks
					f.BaseStream.Position += blockSize;
				}

			}
			f.Close();
		}

		/// <summary>
		/// Render text quads
		/// </summary>
		/// <param name="text">String</param>
		/// <param name="x">X</param>
		/// <param name="y">Y</param>
		/// <param name="scale">Font scale</param>
		/// <param name="color">Font color</param>
		internal void Render(string text, float x, float y, float scale, Color color) {

			Atlas.Bind();
			
			char[] bytes = Encoding.Unicode.GetChars(Encoding.Unicode.GetBytes(text));
			GL.Color4(color);
			GL.Begin(PrimitiveType.Quads);
			float xpixel = 1f / (float)Atlas.Width;
			float ypixel = 1f / (float)Atlas.Height;

			for (int i = 0; i < bytes.Length; i++) {
				int idx = bytes[i];
				if(chars.ContainsKey(idx)) {
					CharData c = chars[idx];

					float ustart = c.x * xpixel;
					float vstart = c.y * ypixel;
					float uend = (c.x + c.width) * xpixel;
					float vend = (c.y + c.height) * ypixel;
					float chsx = x + (c.offsetX / size) * scale;
					float chsy = y + (c.offsetY / size) * scale;
					float chex = chsx + (c.width / size) * scale;
					float chey = chsy + (c.height / size) * scale;

					GL.TexCoord2(ustart, vstart);
					GL.Vertex2(chsx, chsy);
					GL.TexCoord2(uend, vstart);
					GL.Vertex2(chex, chsy);
					GL.TexCoord2(uend, vend);
					GL.Vertex2(chex, chey);
					GL.TexCoord2(ustart, vend);
					GL.Vertex2(chsx, chey);

					x += ((c.advance / size) * scale);
				}
			}
			GL.End();
		}

		/// <summary>
		/// Calculate width 
		/// </summary>
		/// <param name="text">String</param>
		/// <param name="scale">Font size</param>
		internal float Width(string text, float scale) {
			char[] bytes = Encoding.Unicode.GetChars(Encoding.Unicode.GetBytes(text));
			float width = 0f;
			for (int i = 0; i < bytes.Length; i++) {
				int idx = bytes[i];
				if (chars.ContainsKey(idx)) {
					CharData c = chars[idx];
					width += ((c.advance / size) * scale);
				}
			}
			return width;
		}

		/// <summary>
		/// Calculate height 
		/// </summary>
		/// <param name="text">String</param>
		/// <param name="scale">Font size</param>
		internal float Height(string text, float scale) {
			return scale * 1.1f;
			/*
			byte[] bytes = System.Text.Encoding.Unicode.GetBytes(text);
			float start = 5000f, end = 0f;
			for (int i = 0; i < bytes.Length; i++) {
				int idx = bytes[i];
				if (chars.ContainsKey(idx)) {
					CharData c = chars[idx];
					float chsy = (c.offsetY / size) * scale;
					float chey = (c.height / size) * scale;
					start = Math.Min(chsy, start);
					end = Math.Max(chey, end);
				}
			}
			return end - start;
			 */
		}

		internal void DebugDraw() {
			Atlas.Bind();
			GL.Color4(Color.White);
			GL.Begin(PrimitiveType.Quads);
			GL.TexCoord2(0, 0);
			GL.Vertex2(0, 0);
			GL.TexCoord2(1, 0);
			GL.Vertex2(Atlas.Width, 0);
			GL.TexCoord2(1, 1);
			GL.Vertex2(Atlas.Width, Atlas.Height);
			GL.TexCoord2(0, 1);
			GL.Vertex2(0, Atlas.Height);
			GL.End();

			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
			GL.Begin(PrimitiveType.Quads);
			
			GL.End();
			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
		}

		/// <summary>
		/// Internal char representation
		/// </summary>
		private struct CharData {
			public float x, y, width, height;
			public float offsetX, offsetY, advance;
		}
	}
}
