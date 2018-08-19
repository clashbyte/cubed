using System;
using System.Drawing;
using Cubed.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubed.Graphics {

	/// <summary>
	/// Texture object
	/// </summary>
	public class Texture : IDisposable {

		/// <summary>
		/// Empty texture (used for shaders)
		/// </summary>
		static int emptyGLTexture;

		/// <summary>
		/// Not loaded checker texture
		/// </summary>
		static int checkerGLTexture;

		/// <summary>
		/// File name in internal filesystem
		/// </summary>
		public string Link {
			get;
			private set;
		}

		/// <summary>
		/// Current texture frame
		/// </summary>
		public int CurrentFrame {
			get;
			set;
		}

		/// <summary>
		/// Number of frames
		/// </summary>
		public int TotalFrames {
			get {
				if (tex != null) {
					return tex.TotalFrames;
				}
				return 0;
			}
		}

		/// <summary>
		/// Loading state of texture
		/// </summary>
		public LoadingState State {
			get {
				if (IsReleased || tex == null) {
					return LoadingState.Empty;
				} else {
					return (LoadingState)((byte)tex.State);
				}
			}
		}

		/// <summary>
		/// Texture transparency mode
		/// </summary>
		public TransparencyMode Transparency {
			get {
				if (tex != null) {
					return (TransparencyMode)((byte)tex[CurrentFrame].Transparency);
				}
				return TransparencyMode.Opaque;
			}
		}

		/// <summary>
		/// Is texture released
		/// </summary>
		public bool IsReleased {
			get;
			private set;
		}

		/// <summary>
		/// Proxy texture (placeholder)
		/// </summary>
		public Texture Proxy {
			get;
			set;
		}

		/// <summary>
		/// Texture horizontal wrapping
		/// </summary>
		public WrapMode WrapHorizontal {
			get;
			set;
		}

		/// <summary>
		/// Texture vertical wrapping
		/// </summary>
		public WrapMode WrapVertical {
			get;
			set;
		}

		/// <summary>
		/// Texture filtering
		/// </summary>
		public FilterMode Filtering {
			get;
			set;
		}

		/// <summary>
		/// Texture width
		/// </summary>
		public int Width {
			get {
				if (tex != null) {
					return tex[CurrentFrame].Width;
				}
				return 0;
			}
		}

		/// <summary>
		/// Texture height
		/// </summary>
		public int Height {
			get {
				if (tex != null) {
					return tex[CurrentFrame].Height;
				}
				return 0;
			}
		}

		/// <summary>
		/// Total animation length
		/// </summary>
		public int AnimationLength {
			get {
				if (tex != null) {
					return tex.AnimationLength;
				}
				return 0;
			}
		}

		/// <summary>
		/// Texture cache entry
		/// </summary>
		TextureCache.CacheEntry tex;

		/// <summary>
		/// Texture constructor
		/// </summary>
		/// <param name="file">File name</param>
		/// <param name="loadMode">Loading mode</param>
		public Texture(string file, LoadingMode loadMode = LoadingMode.Instant) {
			Link = file;
			WrapHorizontal = WrapMode.Repeat;
			WrapVertical = WrapMode.Repeat;
			tex = Engine.Current.TextureCache.Get(file, loadMode == LoadingMode.Instant);
			tex.IncrementReference();
		}

		/// <summary>
		/// Internal texture generation from image
		/// </summary>
		/// <param name="image">Raw image</param>
		public Texture(Image image) {
			Link = "";
			WrapHorizontal = WrapMode.Repeat;
			WrapVertical = WrapMode.Repeat;
			tex = Engine.Current.TextureCache.GetFromImage(image);
			tex.IncrementReference();
		}

		/// <summary>
		/// Texture removal
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Internal texture freeing
		/// </summary>
		void Dispose(bool disposing) {
			if (!IsReleased) {
				if (tex != null) {
					tex.DecrementReference();
					tex = null;
				}
				IsReleased = true;
			}
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~Texture() {
			Dispose(false);
		}

		/// <summary>
		/// Освобождение текстуры
		/// </summary>
		public void Release() {

		}

		/// <summary>
		/// Getting frame delay
		/// </summary>
		/// <param name="index">Index</param>
		/// <returns>Frame delay in ms</returns>
		public int GetFrameDelay(int index) {
			if (tex != null) {
				if (tex.TotalFrames > index) {
					return tex[index].Delay;
				}
			}
			return 0;
		}

		/// <summary>
		/// Texture binding
		/// </summary>
		internal void Bind() {
			bool bindable = false;
			if (State != LoadingState.Empty) {
				if (tex != null) {
					if (tex.TotalFrames > 0) {
						bindable = true;
					}
				}
			}

			// If texture is bindable
			if (bindable) {

				// Setting up
				tex.Bind(CurrentFrame);

				// Filtering
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
				switch (Filtering) {
					case FilterMode.Disabled:
						GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
						break;
					case FilterMode.Enabled:
						GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
						break;
				}

				// Horizontal wrap
				switch (WrapHorizontal) {
					case WrapMode.Clamp:
						GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
						break;
					case WrapMode.Repeat:
						GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
						break;
					case WrapMode.Mirror:
						GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.MirroredRepeat);
						break;
				}

				// Vertical wrap
				switch (WrapVertical) {
					case WrapMode.Clamp:
						GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
						break;
					case WrapMode.Repeat:
						GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
						break;
					case WrapMode.Mirror:
						GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.MirroredRepeat);
						break;
				}

			} else {
				if (Proxy != null) {
					Proxy.Bind();
				} else {
					BindNotFound();
					//BindEmpty();
				}
			}
		}

		/// <summary>
		/// Binding empty texture to pipeline
		/// </summary>
		internal static void BindEmpty() {
			if(Caps.ShaderPipeline) {
				if (emptyGLTexture == 0) {
					emptyGLTexture = GL.GenTexture();
					GL.BindTexture(TextureTarget.Texture2D, emptyGLTexture);
					GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Four, 1, 1, 0, PixelFormat.Rgba, PixelType.UnsignedByte, new byte[] { 255, 255, 255, 255 });
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
				} else {
					GL.BindTexture(TextureTarget.Texture2D, emptyGLTexture);
				}
				ShaderSystem.TextureMatrix = Matrix4.Identity;
			} else {
				GL.BindTexture(TextureTarget.Texture2D, 0);
			}
		}

		/// <summary>
		/// Binding checker texture to pipeline
		/// </summary>
		internal static void BindNotFound() {
			bool generate = false;
			if (checkerGLTexture == 0 || !GL.IsTexture(checkerGLTexture)) {
				checkerGLTexture = GL.GenTexture();
				generate = true;
			}
			GL.BindTexture(TextureTarget.Texture2D, checkerGLTexture);
			if (generate) {

				// Generating checker pattern
				byte[] data = new byte[64];
				for (int y = 0; y < 4; y++) {
					for (int x = 0; x < 4; x++) {
						int i = (x + y * 4) * 4;
						if (((x + y) % 2) == 0) {
							data[i + 0] = 200;
							data[i + 1] = 200;
							data[i + 2] = 200;
						} else {
							data[i + 0] = 255;
							data[i + 1] = 255;
							data[i + 2] = 255;
						}
						data[i + 3] = 255;
					}
				}
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Four, 4, 4, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			}
			if (Caps.ShaderPipeline) {
				ShaderSystem.TextureMatrix = Matrix4.Identity;
			}
		}

		/// <summary>
		/// Texture loading mode
		/// </summary>
		public enum LoadingMode : byte {
			/// <summary>
			/// Instant loading into GPU
			/// </summary>
			Instant = 0,
			/// <summary>
			/// Queued for background file reading
			/// </summary>
			Queued = 1
		}

		/// <summary>
		/// Texture loading state
		/// </summary>
		public enum LoadingState : byte {
			/// <summary>
			/// Waiting to be read from disk
			/// </summary>
			Waiting = 0,
			/// <summary>
			/// Reading from disk
			/// </summary>
			Reading = 1,
			/// <summary>
			/// Waiting to be sent to GPU
			/// </summary>
			NotSent = 2,
			/// <summary>
			/// Sending to GPU
			/// </summary>
			Sending = 3,
			/// <summary>
			/// Complete and ready
			/// </summary>
			Complete = 4,
			/// <summary>
			/// Released
			/// </summary>
			Empty = 5
		}

		/// <summary>
		/// Texture mapping and repeating mode
		/// </summary>
		public enum WrapMode : byte {
			/// <summary>
			/// Clamp to the border
			/// </summary>
			Clamp = 0,
			/// <summary>
			/// Repeat
			/// </summary>
			Repeat = 1,
			/// <summary>
			/// Repeat with mirroring
			/// </summary>
			Mirror = 2
		}

		/// <summary>
		/// Texture filtering
		/// </summary>
		public enum FilterMode : byte {
			/// <summary>
			/// Raw pixels (nearest)
			/// </summary>
			Disabled = 0,
			/// <summary>
			/// Smooth (bilinear)
			/// </summary>
			Enabled = 1
		}

		/// <summary>
		/// Transparency mode
		/// </summary>
		public enum TransparencyMode : byte {
			/// <summary>
			/// Fully opaque
			/// </summary>
			Opaque = 0,
			/// <summary>
			/// One-bit transparency
			/// </summary>
			AlphaCut = 1,
			/// <summary>
			/// Interpolated transparency
			/// </summary>
			AlphaFull = 2
		}
	}

}
