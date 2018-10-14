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
using Cubed.Drivers.Rendering;

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
		/// Количество отрисовок
		/// </summary>
		public uint DrawnPrimitives {
			get {
				return drawCalls;
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
		/// Engine pause flag
		/// </summary>
		public bool Paused {
			get;
			private set;
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
		/// Количество отрисовок
		/// </summary>
		internal uint drawCalls;


		/// <summary>
		/// Previous input state
		/// </summary>
		InputState.Snapshot inputSnapshot;

		/// <summary>
		/// Engine constructor
		/// </summary>
		public Engine() {
			this.TextureCache = new TextureCache(this);
			this.Console = new UI.Console();
			this.inputSnapshot = new InputState.Snapshot();
		}

		/// <summary>
		/// Making this engine current
		/// </summary>
		public void MakeCurrent() {
			Current = this;
		}

		/// <summary>
		/// Engine pause
		/// </summary>
		public void Pause() {
			Paused = true;
			// TODO: Pause logic
		}

		/// <summary>
		/// Engine resuming
		/// </summary>
		public void Resume() {
			Paused = false;
			if (World != null) {
				World.ResetUpdateCounter();
			}

			// TODO: Unpause logic
		}

		/// <summary>
		/// Update engine
		/// </summary>
		/// <param name="display">Display device</param>
		/// <param name="tween">Delta frames</param>
		/// <param name="state">Current input state</param>
		internal void Update(Display display, float tween, InputState state) {

			Current = this;

			// Updating controls
			Controls.Update(state);

			if (!Paused) {

				// Updating world
				if (World != null) {
					World.Update();
				}

				// Updating world and scene
				EventHandler<UpdateEventArgs> updateHandle = UpdateLogic;
				if (updateHandle != null) {
					UpdateEventArgs uea = new UpdateEventArgs() {
						CurrentEngine = this,
						Tween = tween
					};
					updateHandle(this, uea);
				}

				// Updating interface
				if (Interface != null) {
					Interface.Update(tween, state);
				}
			}
			
			Current = null;
		}

		/// <summary>
		/// Render frame
		/// </summary>
		/// <param name="display"></param>
		/// <param name="tween"></param>
		internal void Render(Display display, float tween) {

			// Handling texture cache
			Current = this;
			Caps.CheckCaps();
			TextureCache.Update();
			drawCalls = 0;

			if (!Paused) {

				// Viewport and cleaning
				Vector2 res = display.Resolution;
				GL.Viewport(new Size((int)res.X, (int)res.Y));
				GL.ClearColor(Color.Black);
				GL.ClearAccum(0, 0, 0, 0);
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit | ClearBufferMask.AccumBufferBit);

				// Rendering
				if (World != null) {
					if (World.Camera != null) {
						if (World.Camera.Size != res) {
							World.Camera.Size = res;
						}
					}
					World.Render();
				}
				if (Interface != null) {
					GL.Enable(EnableCap.Blend);
					GL.Disable(EnableCap.DepthTest);
					GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

					Interface.Setup(Vector2.Zero, res);
					Interface.Render();

					GL.Disable(EnableCap.Blend);
				}
				Console.Render();
			}
			
			Current = null;
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
