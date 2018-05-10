using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cubed.Drivers.Rendering;
using Cubed.Drivers.Windows;
using Cubed.Input;
using OpenTK;
using OpenTK.Input;

namespace Cubed.Drivers {
	
	/// <summary>
	/// Custom control for interface
	/// </summary>
	public class EngineControl : Control {

		/// <summary>
		/// Display module
		/// </summary>
		public ProxyDisplay Display {
			get;
			set;
		}

		/// <summary>
		/// GL device control
		/// </summary>
		GLControl gl;

		/// <summary>
		/// Timer for logic and rendering
		/// </summary>
		Timer timer;

		/// <summary>
		/// Keyboard and mouse states
		/// </summary>
		bool[] keyStates, mouseStates;

		/// <summary>
		/// Previous snapshot
		/// </summary>
		InputState.Snapshot prevSnapshot;

		/// <summary>
		/// Cursor position
		/// </summary>
		Point mousePos, prevMousePos;

		/// <summary>
		/// Mouse wheel
		/// </summary>
		int wheel, prevWheel;

		/// <summary>
		/// Is mouse inside
		/// </summary>
		bool mouseInside;

		/// <summary>
		/// Sync mouse on move
		/// </summary>
		bool mouseResetDelta;

		/// <summary>
		/// Locking mouse
		/// </summary>
		bool mouseLocked;

		/// <summary>
		/// Control constructor
		/// </summary>
		public EngineControl() {
			prevSnapshot = new InputState.Snapshot();
			keyStates = new bool[256];
			mouseStates = new bool[5];
			mousePos = new Point(-1, -1);
			prevMousePos = new Point(-1, -1);
		}

		/// <summary>
		/// Generating control
		/// </summary>
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if (!DesignMode) {

				// Making control
				SuspendLayout();
				gl = new GLControl(new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(32), 24, 0, 0, new OpenTK.Graphics.ColorFormat(32), 2, false), 2, 4, OpenTK.Graphics.GraphicsContextFlags.Default);
				gl.Paint += gl_Paint;
				gl.KeyDown += gl_KeyDown;
				gl.KeyUp += gl_KeyUp;
				gl.MouseMove += gl_MouseMove;
				gl.MouseEnter += gl_MouseEnter;
				gl.MouseLeave += gl_MouseLeave;
				gl.MouseDown += gl_MouseDown;
				gl.MouseUp += gl_MouseUp;
				gl.Dock = DockStyle.Fill;
				Controls.Add(gl);
				ResumeLayout();

				// Making timer
				timer = new Timer();
				timer.Interval = 10;
				timer.Tick += timer_Tick;
				timer.Start();
			}
		}

		/// <summary>
		/// Mouse movement
		/// </summary>
		void gl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			mousePos = new Point(e.X, e.Y);
			if (mouseResetDelta) {
				prevMousePos = mousePos;
				mouseResetDelta = false;
			}
		}

		/// <summary>
		/// Mouse key release
		/// </summary>
		void gl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			int btn = 0;
			int evb = (int)e.Button >> 20;
			for (int i = 0; i < 5; i++) {
				if ((evb & (1 << i)) != 0) {
					btn = i;
					break;
				}
			}
			mouseStates[btn] = false;
		}

		/// <summary>
		/// Mouse press
		/// </summary>
		void gl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			int btn = 0;
			int evb = (int)e.Button >> 20;
			for (int i = 0; i < 5; i++) {
				if ((evb & (1 << i)) != 0) {
					btn = i;
					break;
				}
			}
			mouseStates[btn] = true;
		}

		/// <summary>
		/// Mouse leaving 
		/// </summary>
		void gl_MouseLeave(object sender, EventArgs e) {
			mouseInside = false;
			mousePos = new Point(-1, -1);
			prevMousePos = new Point(-1, -1);
		}

		/// <summary>
		/// Mouse entering control
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void gl_MouseEnter(object sender, EventArgs e) {
			mouseInside = true;
			mouseResetDelta = true;
		}

		/// <summary>
		/// Key release
		/// </summary>
		void gl_KeyUp(object sender, KeyEventArgs e) {
			keyStates[(int)WinKeyMap.TranslateKey((int)e.KeyCode)] = false;
		}

		/// <summary>
		/// Key hit
		/// </summary>
		void gl_KeyDown(object sender, KeyEventArgs e) {
			keyStates[(int)WinKeyMap.TranslateKey((int)e.KeyCode)] = true;
		}

		/// <summary>
		/// Updating logic
		/// </summary>
		void timer_Tick(object sender, EventArgs e) {

			// Locking
			if (Display.MouseLock != mouseLocked) {
				if (Display.MouseLock) {
					mouseResetDelta = true;
					Point center = gl.PointToScreen(new Point(gl.ClientSize.Width / 2, gl.ClientSize.Height / 2));
					Mouse.SetPosition(center.X, center.Y);
					mousePos = center;
					prevMousePos = center;
					System.Windows.Forms.Cursor.Hide();
				} else {
					System.Windows.Forms.Cursor.Show();
				}
				mouseLocked = Display.MouseLock;
			}

			// Updating logic
			if (Display != null) {
				InputState.Snapshot current = new InputState.Snapshot(mouseStates, keyStates, mousePos, wheel, prevMousePos, prevWheel);
				InputState state = new InputState(prevSnapshot, current);
				prevSnapshot = current;
				prevMousePos = mousePos;

				// Locking mouse
				if (mouseLocked) {
					mouseResetDelta = true;
					Point center = gl.PointToScreen(new Point(gl.ClientSize.Width / 2, gl.ClientSize.Height / 2));
					Mouse.SetPosition(center.X, center.Y);
					prevMousePos = center;
					mousePos = center;
				}

				Display.UpdateLogic(1f, state, gl.ClientSize);
			}

			// Rendering
			gl.Invalidate();
		}

		/// <summary>
		/// Rendering frame
		/// </summary>
		void gl_Paint(object sender, PaintEventArgs e) {

			// Rendering to control
			gl.MakeCurrent();
			if (Display != null) {
				Display.Render(gl.ClientSize);
			}
			gl.SwapBuffers();
		}

	}
}
