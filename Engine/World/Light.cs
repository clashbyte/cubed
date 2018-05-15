using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubed.World {
	
	/// <summary>
	/// Light entity
	/// </summary>
	public class Light : Entity {

		/// <summary>
		/// Texels per one unit
		/// </summary>
		internal const int TEXELS_PER_UNIT = 64;

		/// <summary>
		/// Maximal texture size
		/// </summary>
		internal const int TEXTURE_MAX_SIZE = 4096;

		/// <summary>
		/// Light color
		/// </summary>
		public Color Color {
			get {
				return color;
			}
			set {
				color = value;
				changed = true;
			}
		}

		/// <summary>
		/// Light range
		/// </summary>
		public float Range {
			get {
				return range;
			}
			set {
				if (range != value) {
					changed = true;
					range = Math.Abs(value);
				}
			}
		}

		/// <summary>
		/// Light texture
		/// </summary>
		public Texture Texture {
			get {
				return texture;
			}
			set {
				if (texture != value) {
					textureWasReady = false;
					changed = true;
					texture = value;
				}
			}
		}

		/// <summary>
		/// Current texture angle
		/// </summary>
		public float TextureAngle {
			get {
				return textureAngle;
			}
			set {
				if (textureAngle != value) {
					textureAngle = value;
					changed = true;
				}
			}
		}

		/// <summary>
		/// Shadows
		/// </summary>
		public bool Shadows {
			get {
				return hasShadows;
			}
			set {
				if (hasShadows != value) {
					hasShadows = value;
					changed = true;
				}
			}
		}

		/// <summary>
		/// Handle updates
		/// </summary>
		internal void UpdateForMap(Map map) {
			bool dirty = changed;
			if (texture != null && !textureWasReady) {
				if (texture.State == Graphics.Texture.LoadingState.Complete) {
					textureWasReady = true;
					dirty = true;
				}
			}
			if (dirty) {
				RebuildTexture(map);
			}
		}

		/// <summary>
		/// Making light dirty
		/// </summary>
		internal void MakeDirty() {
			changed = true;
		}

		/// <summary>
		/// Recalculate light
		/// </summary>
		void RebuildTexture(Map map) {
			// Pushing state
			GL.PushAttrib(AttribMask.AllAttribBits);
			int realSize = (int)Math.Ceiling((float)TEXELS_PER_UNIT * range * 2);
			int size = Math.Min(realSize, TEXTURE_MAX_SIZE);
			int texSize = MathHelper.NextPowerOfTwo(size);
			textureFactor = (float)size / (float)texSize;

			// Storing matrices
			Matrix4 projMatrix = Matrix4.CreateOrthographic(range * 2, -range * 2, -1, 1);
			GL.MatrixMode(MatrixMode.Projection);
			GL.PushMatrix();
			GL.LoadMatrix(ref projMatrix);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.PushMatrix();
			GL.LoadIdentity();

			/*
			Matrix4 oldCam = ShaderSystem.CameraMatrix;
			Matrix4 oldProj = ShaderSystem.ProjectionMatrix;
			ShaderSystem.EntityMatrix = Matrix4.Identity;
			ShaderSystem.CameraMatrix = Matrix4.Identity;
			ShaderSystem.ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, BLOCKS, BLOCKS, 0, -1, 1);
			GL.MatrixMode(MatrixMode.Projection);
			GL.PushMatrix();
			GL.LoadMatrix(ref ShaderSystem.ProjectionMatrix);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.PushMatrix();
			GL.LoadIdentity();
			GL.Disable(EnableCap.DepthTest);
			*/

			// Preparing render texture
			GL.Enable(EnableCap.Texture2D);
			if (textureBuffer == 0 || !GL.IsTexture(textureBuffer)) {
				textureBuffer = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, textureBuffer);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, texSize, texSize, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Clamp);
			} else {
				GL.BindTexture(TextureTarget.Texture2D, textureBuffer);
			}

			// Preparing framebuffer
			if (frameBuffer == 0 || !GL.IsFramebuffer(frameBuffer)) {
				frameBuffer = GL.GenFramebuffer();
			}
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, textureBuffer, 0);
			GL.BindTexture(TextureTarget.Texture2D, 0);

			// Clearing surface
			GL.ClearColor(0, 0, 0, 1);
			GL.Viewport(new Size(size, size));
			GL.Clear(ClearBufferMask.ColorBufferBit);
			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.Texture2D);

			// Rendering brightmap
			if (texture != null && texture.State == Graphics.Texture.LoadingState.Complete) {

				// Binding texture
				GL.Enable(EnableCap.Texture2D);
				texture.Bind();

				// Rendering texture plane
				GL.MatrixMode(MatrixMode.Modelview);
				GL.PushMatrix();
				GL.Rotate(-textureAngle, Vector3.UnitZ);
				GL.Color4(color);
				GL.Begin(PrimitiveType.Quads);
				GL.TexCoord2(0, 0);
				GL.Vertex2(-range, -range);
				GL.TexCoord2(1, 0);
				GL.Vertex2(range, -range);
				GL.TexCoord2(1, 1);
				GL.Vertex2(range, range);
				GL.TexCoord2(0, 1);
				GL.Vertex2(-range, range);
				GL.End();
				GL.PopMatrix();

				// Binding empty
				GL.BindTexture(TextureTarget.Texture2D, 0);
				GL.Disable(EnableCap.Texture2D);

			} else {

				// Rendering without texture
				GL.Begin(PrimitiveType.TriangleFan);
				GL.Color4(color);
				GL.Vertex2(0, 0);
				GL.Color4(Color.Black);
				
				for (float i = 0; i <= 360; i += 10) {
					float rad = i / 180f * (float)Math.PI;
					GL.Vertex2(Math.Sin(rad) * range, Math.Cos(rad) * range);
				}
				GL.End();
			}

			// Rendering shadows
			if (hasShadows) {
				Map.Chunk[] chunks = map.GetAllChunks();
				List<BlockingLine> lines = new List<BlockingLine>();

				foreach (Map.Chunk c in chunks) {
					if (c.TouchesLight(this)) {

						Map.LightObstructor[] obs = c.GetObstructors(this);
						foreach (Map.LightObstructor o in obs) {
							int cnt = o.Points.Count;
							for (int i = 0; i < cnt; i++) {
								BlockingLine ln = new BlockingLine(
									o.Points[i],
									o.Points[(i + 1) % cnt]
								);

								// Adding to list
								float delta = -ln.First.X * (ln.Last.Y - ln.First.Y) + ln.First.Y * (ln.Last.X - ln.First.X);
								if (delta <= 0) {
									c.QueueRelight();
									lines.Add(ln);
								}
							}
						}
					}	
				}

				// Composing shadow buffer
				float[] coords = new float[lines.Count * 8];
				int idx = 0;
				foreach (BlockingLine l in lines) {
					Vector2 firstNorm = l.First;
					Vector2 lastNorm = l.Last;
					firstNorm.NormalizeFast();
					lastNorm.NormalizeFast();
					Vector2 extFirst = l.First + firstNorm * 5000f;
					Vector2 extLast = l.Last + lastNorm * 5000f;

					coords[idx + 0] = l.First.X;
					coords[idx + 1] = l.First.Y;
					coords[idx + 2] = l.Last.X;
					coords[idx + 3] = l.Last.Y;
					coords[idx + 4] = extLast.X;
					coords[idx + 5] = extLast.Y;
					coords[idx + 6] = extFirst.X;
					coords[idx + 7] = extFirst.Y;
					idx += 8;
				}

				// Rendering shadow buffer
				GL.Color4(Color.Black);
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				GL.EnableClientState(ArrayCap.VertexArray);
				GL.VertexPointer(2, VertexPointerType.Float, 0, coords);
				GL.DrawArrays(PrimitiveType.Quads, 0, idx / 2);
				GL.DisableClientState(ArrayCap.VertexArray);
			}

			// Releasing buffer
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

			// Restoring matrices
			/*
			ShaderSystem.CameraMatrix = oldCam;
			ShaderSystem.ProjectionMatrix = oldProj;
			*/
			GL.MatrixMode(MatrixMode.Projection);
			GL.PopMatrix();
			GL.MatrixMode(MatrixMode.Modelview);
			GL.PopMatrix();

			// Returning attributes
			GL.PopAttrib();
		}

		/// <summary>
		/// Saving current state
		/// </summary>
		internal void Cleanup() {
			changed = false;
		}

		/// <summary>
		/// Light real position
		/// </summary>
		internal Vector3 RealPosition {
			get;
			set;
		}

		/// <summary>
		/// Does light have changes
		/// </summary>
		internal bool IsChanged {
			get {
				return changed;
			}
		}

		/// <summary>
		/// Light internal range
		/// </summary>
		float range = 3f;

		/// <summary>
		/// Light internal color
		/// </summary>
		Color color = Color.White;

		/// <summary>
		/// Internal texture
		/// </summary>
		Texture texture = null;

		/// <summary>
		/// Current texture angle
		/// </summary>
		float textureAngle = 0;

		/// <summary>
		/// Light parameters changed flag
		/// </summary>
		bool changed = false;

		/// <summary>
		/// Texture ready flag
		/// </summary>
		bool textureWasReady = false;

		/// <summary>
		/// Does this light cast shadows
		/// </summary>
		bool hasShadows = true;

		/// <summary>
		/// Current light texture
		/// </summary>
		internal int textureBuffer;

		/// <summary>
		/// Multiplier for texture
		/// </summary>
		internal float textureFactor;

		/// <summary>
		/// Basic render buffer
		/// </summary>
		static int frameBuffer;

		/// <summary>
		/// Matrix rebuilding
		/// </summary>
		protected override void RebuildMatrix() {
			changed = true;
			base.RebuildMatrix();
			RealPosition = Position;
		}

		/// <summary>
		/// Single light blocking line
		/// </summary>
		protected class BlockingLine {

			/// <summary>
			/// First vertex
			/// </summary>
			public Vector2 First {
				get;
				private set;
			}

			/// <summary>
			/// Last vertex
			/// </summary>
			public Vector2 Last {
				get;
				private set;
			}

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="p1"></param>
			/// <param name="p2"></param>
			public BlockingLine(Vector2 p1, Vector2 p2) {
				First = p1;
				Last = p2;
			}
		}

	}
}
