using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cubed.Components.Rendering;
using Cubed.Core;
using Cubed.Drivers.Rendering;
using Cubed.World;
using OpenTK;
using OpenTK.Input;

namespace Cubed.Windows {

	/// <summary>
	/// Map editing form
	/// </summary>
	public partial class MapEditorForm : Form {

		/// <summary>
		/// Current engine
		/// </summary>
		Engine engine;

		/// <summary>
		/// Display module
		/// </summary>
		ProxyDisplay display;

		/// <summary>
		/// Current scene
		/// </summary>
		Scene scene;

		/// <summary>
		/// Default camera
		/// </summary>
		Camera cam;

		/// <summary>
		/// Grid
		/// </summary>
		Entity grid;

		/// <summary>
		/// Testing entity
		/// </summary>
		Entity test;

		/// <summary>
		/// Grid height
		/// </summary>
		float gridHeight;

		

		/// <summary>
		/// Form constructor
		/// </summary>
		public MapEditorForm() {
			InitializeComponent();

			// Making engine
			engine = new Engine();

			scene = new Scene();
			cam = new Camera();
			scene.Camera = cam;
			engine.World = scene;

			// Making display
			display = new ProxyDisplay();
			display.Engine = engine;
			screen.Display = display;

			// Making debug scene
			cam.Position = new OpenTK.Vector3(0, 5, -3);
			cam.Angles = new OpenTK.Vector3(30, 0, 0);
			engine.UpdateLogic += engine_UpdateLogic;

			// Making grid
			grid = new Entity();
			grid.AddComponent(new WireGridComponent() {
				CellCount = 100,
				CellSize = 0.25f,
				GroupedCells = 4
			});
			scene.Entities.Add(grid);
			gridHeight = 0;

			test = new Entity();
			test.AddComponent(new WireCubeComponent() {
				Size = Vector3.One * 1f,
				Position = Vector3.One * 0.5f,
				WireWidth = 2f,
				WireColor = Color.AliceBlue
			});
			scene.Entities.Add(test);

		}

		/// <summary>
		/// Engine updating
		/// </summary>
		void engine_UpdateLogic(object sender, Engine.UpdateEventArgs e) {
			
			// Mouse look
			Vector2 mov = Cubed.Input.Controls.Movement * (Input.Controls.KeyDown(Key.ShiftLeft) ? 0.2f : 0.08f);
			Vector3 rot = Vector3.Zero;
			if (Input.Controls.MouseHit(MouseButton.Middle)) {
				display.MouseLock = true;
			} else if (Input.Controls.MouseReleased(MouseButton.Middle)) {
				display.MouseLock = false;
			}
			if (display.MouseLock) {
				rot.X = Input.Controls.MouseDelta.Y * 0.1f;
				rot.Y = Input.Controls.MouseDelta.X * 0.1f;
			}
			cam.FreeLookControls(new Vector3(mov.X, 0, mov.Y), rot);
			
			// Grid movement
			grid.Position = new Vector3(
				(float)System.Math.Floor(cam.Position.X),
				gridHeight,
				(float)System.Math.Floor(cam.Position.Z)
			);
			
			
			Vector3 pos = cam.ScreenToPoint(Input.Controls.Mouse.X, Input.Controls.Mouse.Y, 0);
			Vector3 dir = cam.ScreenToPoint(Input.Controls.Mouse.X, Input.Controls.Mouse.Y, 1) - pos;
			Vector3 ppos = Vector3.Zero;
			if (Cubed.Math.Intersections.RayPlane(Vector3.Zero, Vector3.UnitY, pos, dir.Normalized(), out ppos)) {
				ppos = new Vector3((float)System.Math.Floor(ppos.X), 0, (float)System.Math.Floor(ppos.Z));
				test.Position = ppos;
			}
		}
	}
}
