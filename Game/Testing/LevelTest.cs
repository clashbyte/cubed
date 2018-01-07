using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Components.Rendering;
using Cubed.Core;
using Cubed.Drivers.Files;
using Cubed.Graphics;
using Cubed.Input;
using Cubed.World;
using OpenTK;
using OpenTK.Input;

namespace Cubed.Main.Testing {

	/// <summary>
	/// Level testing
	/// </summary>
	public class LevelTest {

		/// <summary>
		/// Initialization flag
		/// </summary>
		bool initialized = false;

		/// <summary>
		/// Main camera
		/// </summary>
		Camera cam;

		/// <summary>
		/// Block texture
		/// </summary>
		Texture blockTex;

		/// <summary>
		/// Map light
		/// </summary>
		Light light;

		/// <summary>
		/// Map light
		/// </summary>
		Light light2;

		/// <summary>
		/// Light angle
		/// </summary>
		float lightTurn;

		/// <summary>
		/// Start the game
		/// </summary>
		public void Run(Engine engine) {

			// Handling logical update
			engine.UpdateLogic += engine_UpdateLogic;
			engine.MouseLock = true;
			engine.Filesystem = new FolderFileSystem() {
				RootFolder = @"D:\Sharp\Cubed\Project"
			};

		}

		/// <summary>
		/// Handle updates
		/// </summary>
		void engine_UpdateLogic(object sender, Engine.UpdateEventArgs e) {

			// Handling initialization
			if (!initialized) {

				Texture tex = new Texture("skytest.png");
				tex.Filtering = Texture.FilterMode.Disabled;
				blockTex = tex;

				Skybox sky = new Skybox();
				sky[Skybox.Side.Front] = new Texture("snowy_rt.png") {
					Filtering = Texture.FilterMode.Enabled,
					WrapHorizontal = Texture.WrapMode.Clamp,
					WrapVertical = Texture.WrapMode.Clamp
				};
				sky[Skybox.Side.Back] = new Texture("snowy_lf.png") {
					Filtering = Texture.FilterMode.Enabled,
					WrapHorizontal = Texture.WrapMode.Clamp,
					WrapVertical = Texture.WrapMode.Clamp
				};
				sky[Skybox.Side.Left] = new Texture("snowy_bk.png") {
					Filtering = Texture.FilterMode.Enabled,
					WrapHorizontal = Texture.WrapMode.Clamp,
					WrapVertical = Texture.WrapMode.Clamp
				};
				sky[Skybox.Side.Right] = new Texture("snowy_ft.png") {
					Filtering = Texture.FilterMode.Enabled,
					WrapHorizontal = Texture.WrapMode.Clamp,
					WrapVertical = Texture.WrapMode.Clamp
				};
				sky[Skybox.Side.Top] = new Texture("snowy_up.png") {
					Filtering = Texture.FilterMode.Enabled,
					WrapHorizontal = Texture.WrapMode.Clamp,
					WrapVertical = Texture.WrapMode.Clamp
				};
				sky[Skybox.Side.Bottom] = new Texture("snowy_dn.png") {
					Filtering = Texture.FilterMode.Enabled,
					WrapHorizontal = Texture.WrapMode.Clamp,
					WrapVertical = Texture.WrapMode.Clamp
				};

				// Camera
				cam = new Camera();
				cam.Zoom = 1f;
				cam.Position = new Vector3(0, 10, -15);
				cam.Angles = new Vector3(35, 0, 0);

				// Scene
				Scene scene = new Scene();
				scene.Camera = cam;
				scene.Sky = sky;

				// Test lamp
				light = new Light();
				light.Color = Color.LimeGreen;
				//light.Texture = new Texture("light.png");
				light.Range = 7;
				light.Position = new Vector3(0, 0.2f, 0);
				light.AddComponent(new WireCubeComponent() {
					Size = Vector3.One * 0.4f,
					WireColor = Color.Lime,
					WireWidth = 2f
				});
				scene.Entities.Add(light);

				// Test lamp
				light2 = new Light();
				light2.Color = Color.Orange;
				light2.Range = 7;
				//light2.Texture = new Texture("light2.png");
				light2.Position = new Vector3(0, 0.2f, 0);
				light2.AddComponent(new WireCubeComponent() {
					Size = Vector3.One * 0.4f,
					WireColor = Color.Lime,
					WireWidth = 2f
				});
				scene.Entities.Add(light2);

				// Test quad
				/*
				Entity ent = new Entity();
				ent.AddComponent(new WireGridComponent() {
					CellCount = 32,
					CellSize = 0.25f,
					GroupedCells = 4,
					WireMainColor = Color.DimGray,
					WireAccentColor = Color.Gray
				});
				scene.Entities.Add(ent);
				*/
				
				// Building map
				Map m = new Map();
				m.Ambient = Color.FromArgb(5, 5, 20);
				for (int y = -1; y < 1; y++) {
					for (int x = -1; x < 1; x++) {
						Map.Chunk c = new Map.Chunk();

						
						for (int i = 1; i < 7; i += 2) {
							SetChunkBlock(c, i, 3);
							SetChunkBlock(c, i, 4);
							SetChunkBlock(c, i + 1, 5);
							SetChunkBlock(c, i, 6);
						}
						

						m[x, 0, y] = c;
					}
				}
				scene.Map = m;

				// Setting scene
				e.CurrentEngine.World = scene;
				initialized = true;
			}

			// Moving camera
			Vector2 rot = Controls.MouseDelta * 0.2f;
			Vector2 mov = Controls.Movement * (Controls.KeyDown(Key.LShift) ? 0.5f : 0.1f);
			cam.FreeLookControls(new Vector3(mov.X, 0, mov.Y), new Vector3(rot.Y, rot.X, 0));

			// Light
			lightTurn += 0.002f * e.Tween;
			if (Controls.KeyHit(Key.Number1)) {
				light.Shadows = !light.Shadows;
			}
			if (Controls.KeyHit(Key.Number2)) {
				light.Position += Vector3.One * 0.0001f;
			}
			//light.TextureAngle = -lightTurn * 300f;
			//light2.TextureAngle = lightTurn * 300f;
			//light.Position = new Vector3((float)Math.Sin(lightTurn * 2f) * 8f, 0.2f, (float)Math.Cos(lightTurn) * 8f);
			//light2.Position = new Vector3((float)Math.Sin((lightTurn + 0.3f) * 2f) * 8f, 0.2f, (float)Math.Cos((lightTurn + 0.3f)) * 8f);

		}

		/// <summary>
		/// Filling chunk with texture
		/// </summary>
		/// <param name="c">Chunk</param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		void SetChunkBlock(Cubed.World.Map.Chunk c, int x, int y) {
			Map.WallBlock blck = new Map.WallBlock();
			
			blck[Map.Side.Left] = blockTex;
			blck[Map.Side.Right] = blockTex;
			blck[Map.Side.Forward] = blockTex;
			blck[Map.Side.Back] = blockTex;
			
			c[x, y] = blck;
		}
	}
}
