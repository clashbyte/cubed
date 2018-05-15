using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cubed.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubed.Graphics {

	/// <summary>
	/// Texture caching service
	/// </summary>
	internal class TextureCache {

		/// <summary>
		/// File reading threads
		/// </summary>
		const int LOADING_THREADS = 2;

		/// <summary>
		/// Texture uploads per frame
		/// </summary>
		const int SENDS_PER_FRAME = 15;

		/// <summary>
		/// Texture releases per frame
		/// </summary>
		const int RELEASES_PER_FRAME = 60;

		/// <summary>
		/// Lifetime of a texture with no references
		/// </summary>
		const int LOST_TEXTURE_LIFETIME = 3000;

		/// <summary>
		/// Cached textures
		/// </summary>
		ConcurrentDictionary<string, CacheEntry> textures = new ConcurrentDictionary<string, CacheEntry>();

		/// <summary>
		/// Textures for reading from disk
		/// </summary>
		ConcurrentQueue<CacheEntry> loadQueue = new ConcurrentQueue<CacheEntry>();

		/// <summary>
		/// Textures for sending to GPU
		/// </summary>
		ConcurrentQueue<CacheEntry> sendQueue = new ConcurrentQueue<CacheEntry>();

		/// <summary>
		/// Textures for releasing
		/// </summary>
		ConcurrentBag<int> releaseQueue = new ConcurrentBag<int>();

		/// <summary>
		/// Background loading threads
		/// </summary>
		Thread[] loadingThreads;

		/// <summary>
		/// Owner engine
		/// </summary>
		Engine engine;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="eng">Engine</param>
		public TextureCache(Engine eng) {
			engine = eng;
		}

		/// <summary>
		/// Texture cache update
		/// </summary>
		internal void Update() {

			// Sending textures
			int sendQuota = SENDS_PER_FRAME;
			while (!sendQueue.IsEmpty) {

				CacheEntry ce = null;
				if (sendQueue.TryDequeue(out ce)) {
					ce.SendToGL();
				}

				sendQuota--;
				if (sendQuota == 0) {
					break;
				}
			}

			// All checks 
			List<string> namesToRemove = new List<string>();
			foreach (KeyValuePair<string, CacheEntry> e in textures) {
				if (e.Value.UseCount == 0) {
					if ((DateTime.Now - e.Value.LastUseLostTime).Milliseconds > LOST_TEXTURE_LIFETIME) {
						e.Value.Release();
						namesToRemove.Add(e.Key);
					}
				}
			}

			// Texture releasing
			foreach (string nm in namesToRemove) {
				CacheEntry ce;
				textures.TryRemove(nm, out ce);
			}
		}

		/// <summary>
		/// Getting texture
		/// </summary>
		/// <param name="name">File name</param>
		internal CacheEntry Get(string name, bool instant) {
			name = name.ToLower();
			if (!textures.ContainsKey(name)) {
				CacheEntry ce = new CacheEntry(name);
				if (instant) {
					ce.LoadNow(engine);
				} else {
					ce.AddToLoadingQueue(engine);
				}
				textures.TryAdd(name, ce);
			}
			return textures[name];
		}

		/// <summary>
		/// Entry from image
		/// </summary>
		/// <param name="image">Image</param>
		/// <returns>Uncached entry</returns>
		internal CacheEntry GetFromImage(Image image) {
			return new CacheEntry(image);
		}

		/// <summary>
		/// Thread checking
		/// </summary>
		void CheckThreads() {
			if (loadingThreads == null) {
				Caps.CheckCaps();
				loadingThreads = new Thread[LOADING_THREADS];
				for (int i = 0; i < LOADING_THREADS; i++) {
					Thread t = new Thread(ThreadedLoading);
					t.Priority = ThreadPriority.BelowNormal;
					t.IsBackground = true;
					t.Start();
				}
			}
		}

		/// <summary>
		/// Threaded texture background loading job
		/// </summary>
		void ThreadedLoading() {
			while (true) {
				if (!loadQueue.IsEmpty) {
					CacheEntry t = null;
					if (loadQueue.TryDequeue(out t)) {
						t.ReadData(engine);
						if (t.State != EntryState.Empty) {
							sendQueue.Enqueue(t);
						}
					}
					Thread.Sleep(1);
				} else {
					Thread.Sleep(50);
				}
			}
		}

		/// <summary>
		/// Single entry in cache
		/// </summary>
		internal class CacheEntry {

			/// <summary>
			/// Texture file name
			/// </summary>
			public string FileName {
				get;
				private set;
			}

			/// <summary>
			/// Usages count
			/// </summary>
			public int UseCount {
				get;
				private set;
			}

			/// <summary>
			/// Last usage time
			/// </summary>
			public DateTime LastUseLostTime {
				get;
				private set;
			}

			/// <summary>
			/// GL texture
			/// </summary>
			public int GLTex {
				get;
				private set;
			}

			/// <summary>
			/// Data format
			/// </summary>
			public OpenTK.Graphics.OpenGL.PixelFormat DataFormat {
				get;
				private set;
			}

			/// <summary>
			/// Raw scan data
			/// </summary>
			public byte[] Data {
				get;
				private set;
			}

			/// <summary>
			/// Transparency mode
			/// </summary>
			public TransparencyMode Transparency {
				get;
				private set;
			}

			/// <summary>
			/// Internal entry state
			/// </summary>
			public EntryState State {
				get;
				private set;
			}

			/// <summary>
			/// Width
			/// </summary>
			public int Width {
				get;
				private set;
			}

			/// <summary>
			/// Height
			/// </summary>
			public int Height {
				get;
				private set;
			}

			/// <summary>
			/// Computed width (if NPOT is not supported)
			/// </summary>
			public int ComputedWidth {
				get;
				private set;
			}

			/// <summary>
			/// Computed height (if NPOT is not supported)
			/// </summary>
			public int ComputedHeight {
				get;
				private set;
			}

			/// <summary>
			/// Horizontal delta for NPOT to POT transformation
			/// </summary>
			public float HorizontalDelta {
				get;
				private set;
			}

			/// <summary>
			/// Vertical delta for NPOT to POT transformation
			/// </summary>
			public float VerticalDelta {
				get;
				private set;
			}

			/// <summary>
			/// NPOT to POT texture transformation matrix
			/// </summary>
			public Matrix4 TextureMatrix {
				get;
				private set;
			}

			/// <summary>
			/// Adding usage
			/// </summary>
			public void IncrementReference() {
				UseCount++;
			}

			/// <summary>
			/// Removing usage
			/// </summary>
			public void DecrementReference() {
				UseCount--;
				if (UseCount == 0) {
					LastUseLostTime = DateTime.Now;
				}
			}

			/// <summary>
			/// Entry constructor
			/// </summary>
			/// <param name="file">File name</param>
			public CacheEntry(string file) {
				State = EntryState.Empty;
				FileName = file;
				TextureMatrix = Matrix4.Identity;
			}

			/// <summary>
			/// Entry from image
			/// </summary>
			/// <param name="img">Image</param>
			public CacheEntry(Image img) {
				FileName = "";
				ProcessBitmap((Bitmap)img);
				SendToGL();
			}

			/// <summary>
			/// Instant texture loading
			/// </summary>
			public void LoadNow(Engine eng) {
				ReadData(eng);
				SendToGL();
			}

			/// <summary>
			/// Adding to queue
			/// </summary>
			public void AddToLoadingQueue(Engine eng) {
				if (State != EntryState.Reading && State != EntryState.Waiting && !Engine.Current.TextureCache.loadQueue.Contains(this)) {
					eng.TextureCache.loadQueue.Enqueue(this);
					eng.TextureCache.CheckThreads();
				}
			}

			/// <summary>
			/// Reading from disk
			/// </summary>
			public void ReadData(Engine eng) {
				State = EntryState.Reading;

				// Reading byte array
				if (eng.Filesystem.Exists(FileName)) {

					// Loadin image
					byte[] data = eng.Filesystem.Get(FileName);
					Bitmap bmp = Bitmap.FromStream(new MemoryStream(data)) as Bitmap;
					ProcessBitmap(bmp);
					State = EntryState.NotSent;

				} else {
					State = EntryState.Empty;
				}
			}

			/// <summary>
			/// Bitmap handling
			/// </summary>
			/// <param name="bmp">Bitmap</param>
			void ProcessBitmap(Bitmap bmp) {
				Width = bmp.Width;
				Height = bmp.Height;

				// Handling software NPOT conversion
				if (!Caps.NonPowerOfTwoTextures) {
					ComputedWidth = Pow2(Width);
					ComputedHeight = Pow2(Height);
					if (Width != ComputedWidth || Height != ComputedHeight) {
						Bitmap pr = new Bitmap(ComputedWidth, ComputedHeight);
						using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(pr)) {
							g.DrawImage(bmp, new Rectangle(0, 0, Width, Height), new Rectangle(0, 0, Width, Height), GraphicsUnit.Pixel);
						}
						bmp.Dispose();
						bmp = pr;
					}
					HorizontalDelta = (float)Width / (float)ComputedWidth;
					VerticalDelta = (float)Height / (float)ComputedHeight;
					TextureMatrix = Matrix4.CreateScale(HorizontalDelta, VerticalDelta, 1f);
				} else {
					ComputedWidth = Width;
					ComputedHeight = Height;
					HorizontalDelta = 1f;
					VerticalDelta = 1f;
					TextureMatrix = Matrix4.Identity;
				}

				// Converting into scan
				BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				byte[] scan = new byte[Math.Abs(bd.Stride * bd.Height)];
				Marshal.Copy(bd.Scan0, scan, 0, scan.Length);
				bmp.UnlockBits(bd);
				bmp.Dispose();

				// Saving params
				DataFormat = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
				Data = scan;

				// Calculating scan transparency
				Transparency = TransparencyMode.Opaque;
				for (int ty = 0; ty < Height; ty++) {
					for (int tx = 0; tx < Width; tx++) {
						byte a = scan[(ty * ComputedWidth + tx) * 4 + 3];
						if (a < 255) {
							Transparency = TransparencyMode.AlphaCut;
							if (a > 0) {
								Transparency = TransparencyMode.AlphaFull;
								break;
							}
						}
					}
				}
			}

			/// <summary>
			/// Uploading texture to GPU
			/// </summary>
			public void SendToGL() {
				State = EntryState.Sending;
				GL.Enable(EnableCap.Texture2D);

				GLTex = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, GLTex);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Four, ComputedWidth, ComputedHeight, 0, DataFormat, PixelType.UnsignedByte, Data);
				GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvModeCombine.Modulate);
				GL.BindTexture(TextureTarget.Texture2D, 0);

				Data = null;

				State = EntryState.Complete;
			}

			/// <summary>
			/// Releasing texture
			/// </summary>
			public void Release() {
				if (GLTex != 0) {
					if (GL.IsTexture(GLTex)) {
						GL.DeleteTexture(GLTex);
					}
					GLTex = 0;
				}
				if (Data != null) {
					Data = null;
				}
				State = EntryState.Empty;
			}

			/// <summary>
			/// Binding texture to pipeline
			/// </summary>
			public void Bind() {

				// Включение в конвейер
				GL.BindTexture(TextureTarget.Texture2D, GLTex);

				// Загрузка матрицы
				ShaderSystem.TextureMatrix = TextureMatrix;
				GL.MatrixMode(MatrixMode.Texture);
				Matrix4 mat = TextureMatrix;
				GL.LoadMatrix(ref mat);
				GL.MatrixMode(MatrixMode.Modelview);


			}

			/// <summary>
			/// Calculating highest power of two
			/// </summary>
			/// <param name="num">Number</param>
			/// <returns>Nearest power-of-two number</returns>
			int Pow2(int num) {
				int d = 1;
				while (d < num) {
					d *= 2;
				}
				return d;
			}
		}

		/// <summary>
		/// Texture state
		/// </summary>
		internal enum EntryState : byte {
			/// <summary>
			/// Waiting to be read
			/// </summary>
			Waiting = 0,
			/// <summary>
			/// Reading from disk
			/// </summary>
			Reading = 1,
			/// <summary>
			/// Waiting to be sent
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
			/// Released or not loaded
			/// </summary>
			Empty = 5
		}

		/// <summary>
		/// Transparency mode
		/// </summary>
		internal enum TransparencyMode : byte {
			/// <summary>
			/// Non-transparent
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
