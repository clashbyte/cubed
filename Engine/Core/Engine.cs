using Cubed.UI;
using Cubed.World;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Drawing;
using Cubed.Drivers.Files;
using Cubed.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Cubed.Input;

namespace Cubed.Core {
	
	/// <summary>
	/// Heart of application
	/// </summary>
	public class Engine {

		/// <summary>
		/// Current engine
		/// </summary>
		public static Engine Current {
			get;
			private set;
		}

		/// <summary>
		/// Screen resolution
		/// </summary>
		public Vector2 ScreenSize {
			get {
				return resolution;
			}
			set {
				resolution = value;
				if(window != null) {
					window.ClientSize = new Size((int)value.X, (int)value.Y);
				}
			}
		}

		/// <summary>
		/// Fullscreen mode
		/// </summary>
		public bool Fullscreen {
			get {
				return fullscreen;
			}
			set {
				fullscreen = value;
				if(window != null) {
					if (fullscreen) {
						//window.WindowBorder = WindowBorder.Hidden;
						window.WindowState = WindowState.Fullscreen;
					} else {
						window.WindowBorder = WindowBorder.Resizable;
						window.WindowState = WindowState.Normal;
					}
				}
			}
		}

		/// <summary>
		/// Window title
		/// </summary>
		public string Title {
			get {
				return title;
			}
			set {
				title = value;
				if(window != null) {
					window.Title = title;
				}
			}
		}

		/// <summary>
		/// Mouse locking
		/// </summary>
		public bool MouseLock {
			get {
				return lockMouse;
			}
			set {
				lockMouse = value;
				if (window != null) {
					if (lockMouse) {
						
					} else {
						
					}
					window.CursorVisible = !value;
				}
			}
		}

		/// <summary>
		/// File driver for engine
		/// </summary>
		public FileSystem Filesystem {
			get;
			set;
		}

		/// <summary>
		/// Current scene
		/// </summary>
		public Scene World {
			get;
			set;
		}

		/// <summary>
		/// Current interface
		/// </summary>
		public UserInterface Interface {
			get;
			set;
		}

		/// <summary>
		/// Logic update callback
		/// </summary>
		public event EventHandler<UpdateEventArgs> UpdateLogic;

		/// <summary>
		/// Texture caching system
		/// </summary>
		internal TextureCache TextureCache {
			get;
			private set;
		}

		/// <summary>
		/// Console subsystem
		/// </summary>
		internal Cubed.UI.Console Console {
			get;
			private set;
		}

		/// <summary>
		/// Rendering window
		/// </summary>
		GameWindow window;

		/// <summary>
		/// Internal resolution
		/// </summary>
		Vector2 resolution = new Vector2(800, 600);

		/// <summary>
		/// Internal fullscreen flag
		/// </summary>
		bool fullscreen = false;

		/// <summary>
		/// Привязка мыши
		/// </summary>
		bool lockMouse = false;

		/// <summary>
		/// Window title
		/// </summary>
		string title = "Cubed";

		/// <summary>
		/// Previous input state
		/// </summary>
		InputState.Snapshot inputSnapshot;

		/// <summary>
		/// Mouse position
		/// </summary>
		Point mouseLocation;

		/// <summary>
		/// Mouse location before lock
		/// </summary>
		Point mousePreLock;

		/// <summary>
		/// Mouse inside window
		/// </summary>
		bool mouseInside;

		/// <summary>
		/// Mouse click workaround
		/// </summary>
		int mouseCounter;

		/// <summary>
		/// Engine constructor
		/// </summary>
		public Engine() {
			mouseLocation = Point.Empty;
		}

		/// <summary>
		/// Make current engine topmost
		/// </summary>
		public void MakeCurrent() {
			Current = this;
		}

		/// <summary>
		/// Start the engine
		/// </summary>
		public void Run() {
			
			// Initializing texture cacher
			this.TextureCache = new TextureCache();
			this.Console = new UI.Console();


			this.inputSnapshot = new InputState.Snapshot();

			// Make window and start listening for events
			window = new GameWindow((int)resolution.X, (int)resolution.Y, new GraphicsMode(new ColorFormat(32), 24, 0, 0, new ColorFormat(32), 2, false), title, fullscreen ? GameWindowFlags.Fullscreen : GameWindowFlags.Default, DisplayDevice.Default, 2, 4, GraphicsContextFlags.Default);
			window.VSync = VSyncMode.Off;
			window.UpdateFrame += window_UpdateFrame;
			window.RenderFrame += window_RenderFrame;
			window.Resize += window_Resize;
			window.Closing += window_Closing;
			window.MouseEnter += window_MouseEnter;
			window.MouseLeave += window_MouseLeave;
			window.MouseMove += window_MouseMove;
			window.MouseUp += window_MouseUp;
			if (lockMouse) {
				window.CursorVisible = false;
			}
			window.Run(60, 60);
		}
		
		/// <summary>
		/// Mouse up event - workaround for OpenTK glitch
		/// </summary>
		void window_MouseUp(object sender, MouseButtonEventArgs e) {
			mouseCounter = 2;
		}

		/// <summary>
		/// Handling mouse location
		/// </summary>
		void window_MouseMove(object sender, MouseMoveEventArgs e) {
			mouseLocation = e.Position;
			mouseInside = true;
		}

		/// <summary>
		/// Mouse leaving
		/// </summary>
		void window_MouseLeave(object sender, EventArgs e) {
			if (mouseCounter > 0) {
				mouseCounter--;
			} else {
				mouseInside = false;
			}
		}

		/// <summary>
		/// Mouse entering
		/// </summary>
		void window_MouseEnter(object sender, EventArgs e) {
			mouseInside = true;
		}

		/// <summary>
		/// Close current engine
		/// </summary>
		public void Close() {
			if (window != null) {
				window.Close();
			}
		}

		

		/// <summary>
		/// Frame update
		/// </summary>
		void window_UpdateFrame(object sender, FrameEventArgs e) {
			
			// Calculating tween
			float tween = (float)e.Time / 0.016f;

			// Get mouse cursor

			// Updating input
			KeyboardState keybd = window.Keyboard.GetState();
			MouseState mouse = window.Mouse.GetState();
			InputState.Snapshot currentSnapshot = new InputState.Snapshot(mouse, keybd, mouseInside ? mouseLocation : new Point(-1, -1));
			if (!window.Focused) {
				currentSnapshot.ClearTo(inputSnapshot);
			}
			InputState state = new InputState(inputSnapshot, currentSnapshot);

			// Updating console
			Console.Update(tween, state);
			if (Console.Enabled) {
				//currentSnapshot.ClearTo(inputSnapshot);
				//state = new InputState(inputSnapshot, currentSnapshot);
			}
			inputSnapshot = currentSnapshot;
			Controls.Update(state);

			// Updating world and scene
			EventHandler<UpdateEventArgs> updateHandle = UpdateLogic;
			if (updateHandle != null) {
				UpdateEventArgs uea = new UpdateEventArgs() {
					CurrentEngine = this,
					Tween = tween
				};
				updateHandle(this, uea);
			}
			if (World != null) {
				World.Update();
			}

			// Updating interface
			if (Interface != null) {
				Interface.Update(tween, state);
			}
		}

		/// <summary>
		/// Frame rendering
		/// </summary>
		void window_RenderFrame(object sender, FrameEventArgs e) {
			
			// Handling texture uploading
			if (e.Time > 0) {
				TextureCache.Update();
			}

			// Viewport and cleaning
			window.MakeCurrent();
			GL.Viewport(window.ClientSize);
			GL.ClearColor(Color.Black);
			GL.ClearAccum(0, 0, 0, 0);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit | ClearBufferMask.AccumBufferBit);

			// Rendering
			if(World != null) {
				if (World.Camera != null) {
					if (World.Camera.Size != resolution) {
						World.Camera.Size = resolution;
					}
				}
				World.Render();
			}
			if(Interface != null) {
				GL.Enable(EnableCap.Blend);
				GL.Disable(EnableCap.DepthTest);
				GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

				Interface.Setup(Vector2.Zero, resolution);
				Interface.Render();

				GL.Disable(EnableCap.Blend);
			}
			Console.Render();

			// Swapping buffers
			window.SwapBuffers();
		}

		/// <summary>
		/// Window resize event
		/// </summary>
		void window_Resize(object sender, EventArgs e) {
			resolution = new Vector2(window.ClientSize.Width, window.ClientSize.Height);
			if(World != null) {
				if (World.Camera != null) {
					World.Camera.Size = resolution;
				}
			}
			if(Interface != null) {
				Interface.Resize(resolution);
			}
			//window_UpdateFrame(sender, new FrameEventArgs(0.000000001f));
			//window_RenderFrame(sender, new FrameEventArgs(0.000000001f));
		}

		/// <summary>
		/// Application close
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			
		}

		/// <summary>
		/// Frame update event arguments
		/// </summary>
		public class UpdateEventArgs : EventArgs {
			public float Tween { get; set; }
			public Engine CurrentEngine { get; set; }
		}
	}
}
