using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Input;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace Cubed.Drivers.Rendering {
	
	/// <summary>
	/// Windowed rendering device
	/// </summary>
	public class WindowDisplay : Display {

		/// <summary>
		/// Screen resolution
		/// </summary>
		public override Vector2 Resolution {
			get {
				return resolution;
			}
			set {
				resolution = value;
				if (window != null) {
					CheckResolution();
					window.ClientSize = new Size((int)resolution.X, (int)resolution.Y);
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
				if (window != null) {
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
				if (window != null) {
					window.Title = title;
				}
			}
		}

		/// <summary>
		/// Window icon
		/// </summary>
		public Icon Icon {
			get {
				return icon;
			}
			set {
				icon = value;
				if (window != null) {
					window.Icon = icon;
				}
			}
		}

		/// <summary>
		/// Enable mouse locking
		/// </summary>
		public override bool MouseLock {
			get {
				return lockMouse;
			}
			set {
				lockMouse = value;
				if (window != null) {
					window.CursorVisible = !lockMouse;
				}
			}
		}

		/// <summary>
		/// Internal resolution
		/// </summary>
		Vector2 resolution = new Vector2(800, 600);

		/// <summary>
		/// Internal fullscreen flag
		/// </summary>
		bool fullscreen = false;

		/// <summary>
		/// Window title
		/// </summary>
		string title = "Cubed";

		/// <summary>
		/// Window icon
		/// </summary>
		Icon icon;

		/// <summary>
		/// Привязка мыши
		/// </summary>
		bool lockMouse = false;

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
		/// Previous controls snapshot
		/// </summary>
		InputState.Snapshot prevSnapshot; 

		/// <summary>
		/// Current window
		/// </summary>
		GameWindow window;

		/// <summary>
		/// Running engine
		/// </summary>
		public void Run() {

			// Checking for single instance
			if (window != null) {
				throw new Exception("Engine is already running!");
			}

			// Creating window
			CheckResolution();
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
		/// Window closings
		/// </summary>
		public override void Close() {
			window.Close();
		}

		/// <summary>
		/// Checking resolution
		/// </summary>
		void CheckResolution() {
			if (resolution.X == 0 || resolution.Y == 0) {
				DisplayDevice dd = OpenTK.DisplayDevice.Default;
				resolution.X = dd.Width;
				resolution.Y = dd.Height;
			}
		}

		private void window_MouseUp(object sender, OpenTK.Input.MouseButtonEventArgs e) {
			mouseCounter = 2;
		}

		private void window_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e) {
			mouseLocation = e.Position;
			mouseInside = true;
		}

		private void window_MouseLeave(object sender, EventArgs e) {
			if (mouseCounter > 0) {
				mouseCounter--;
			} else {
				mouseInside = false;
			}
		}

		private void window_MouseEnter(object sender, EventArgs e) {
			mouseInside = true;
		}

		private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			//throw new NotImplementedException();
		}

		private void window_Resize(object sender, EventArgs e) {
			resolution = new Vector2(window.ClientSize.Width, window.ClientSize.Height);
		}

		private void window_RenderFrame(object sender, FrameEventArgs e) {
			
			// Drawing frame
			window.MakeCurrent();
			RenderFrame(1f);
			window.SwapBuffers();
		}

		private void window_UpdateFrame(object sender, FrameEventArgs e) {

			// Updating input
			if (prevSnapshot == null) {
				prevSnapshot = new InputState.Snapshot();
			}
			KeyboardState keybd = window.Keyboard.GetState();
			MouseState mouse = window.Mouse.GetState();
			InputState.Snapshot currentSnapshot = new InputState.Snapshot(mouse, keybd, mouseInside ? mouseLocation : new Point(-1, -1));
			
			// Creating input state
			InputState state = new InputState(prevSnapshot, currentSnapshot);
			prevSnapshot = currentSnapshot;

			// Updating
			float tween = (float)(e.Time / 0.016);
			UpdateEngine(tween, state);

		}
		
	}
}
