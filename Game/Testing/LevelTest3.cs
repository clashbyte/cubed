using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Components.Controls;
using Cubed.Components.Rendering;
using Cubed.Core;
using Cubed.Drivers.Files;
using Cubed.Graphics;
using Cubed.Input;
using Cubed.UI;
using Cubed.UI.Basic;
using Cubed.World;
using OpenTK;
using OpenTK.Input;

namespace Cubed.Main.Testing {

	/// <summary>
	/// Level testing
	/// </summary>
	public class LevelTest3 {

		/// <summary>
		/// Initialization flag
		/// </summary>
		bool initialized = false;

		/// <summary>
		/// Current scene
		/// </summary>
		Scene scene;

		/// <summary>
		/// Main camera
		/// </summary>
		Camera cam;

		/// <summary>
		/// Игрок
		/// </summary>
		Entity player;

		/// <summary>
		/// Контроллер игрока
		/// </summary>
		WalkController playerController;

		/// <summary>
		/// Тестовый коллайдер
		/// </summary>
		Entity collider;

		/// <summary>
		/// Фонарик
		/// </summary>
		Light fl;

		/// <summary>
		/// Тестовый спрайт
		/// </summary>
		Entity sprite;

		/// <summary>
		/// Lights
		/// </summary>
		List<Light> lights = new List<Light>();

		/// <summary>
		/// Interface
		/// </summary>
		UserInterface ui;

		/// <summary>
		/// Лейбл для FPS
		/// </summary>
		Label fpsLabel;

		/// <summary>
		/// Лейбл для FPS
		/// </summary>
		Label dipLabel;


		/// <summary>
		/// Количество кадров
		/// </summary>
		int fpsFrames;

		/// <summary>
		/// Таймер
		/// </summary>
		Stopwatch sw;

		/// <summary>
		/// Start the game
		/// </summary>
		public void Run(Engine engine) {

			// Handling logical update
			engine.Filesystem = new FolderFileSystem() {
				RootFolder = @"D:\Sharp\Cubed\Project"
			};
			engine.UpdateLogic += engine_UpdateLogic;
			//engine.MouseLock = true;

			fpsLabel = new Label() {
				Position = Vector2.Zero,
				Text = "FPS: ",
				HorizontalAlign = UserInterface.Align.Start,
				VerticalAlign = UserInterface.Align.Start
			};
			dipLabel = new Label() {
				Position = Vector2.UnitY * 20,
				Text = "DrawCalls: ",
				HorizontalAlign = UserInterface.Align.Start,
				VerticalAlign = UserInterface.Align.Start
			};
			ui = new UserInterface();
			ui.Items.Add(fpsLabel);
			ui.Items.Add(dipLabel);
			engine.Interface = ui;

		}

		/// <summary>
		/// Handle updates
		/// </summary>
		void engine_UpdateLogic(object sender, Engine.UpdateEventArgs e) {

			// Handling initialization
			Random r = new Random();
			if (!initialized) {

				// Camera
				cam = new Camera();
				cam.Zoom = 1f;
				cam.Position = new Vector3(0, 4f, 0);
				cam.Angles = new Vector3(35, 45, 0);

				// Scene
				scene = new Scene();
				scene.Camera = cam;
				

				// Building map
				Map map = new Map();
				map.Ambient = Color.FromArgb(5, 5, 20);

				// Textures
				Texture wallLowerTex = new Texture("38.bmp", Texture.LoadingMode.Queued) {
					Filtering = Texture.FilterMode.Disabled
				};
				Texture wallTex = new Texture("39.bmp", Texture.LoadingMode.Queued) {
					Filtering = Texture.FilterMode.Disabled
				};
				Texture wallLampTex = new Texture("c_003.bmp", Texture.LoadingMode.Queued) {
					Filtering = Texture.FilterMode.Disabled
				};
				Texture floorTex = new Texture("floor_004.bmp", Texture.LoadingMode.Queued) {
					Filtering = Texture.FilterMode.Disabled
				};
				Texture ceilTex = new Texture("floor_005.bmp", Texture.LoadingMode.Queued) {
					Filtering = Texture.FilterMode.Disabled
				};

				// Chunk
				for (int hgt = 0; hgt < 2; hgt++) {
					Map.Chunk chunk = new Map.Chunk();
					Map.FloorBlock fb = new Map.FloorBlock() {
						HasFloor = hgt == 0,
						HasCeiling = hgt == 1,
						Floor = floorTex,
						Ceiling = ceilTex
					};
					Map.WallBlock wb;
					for (int y = 1; y < 7; y++) {
						for (int x = 1; x < 7; x++) {
							if (x >= 3 && y == 3) {
								if (hgt == 0) {
									wb = new Map.WallBlock();
									for (int i = 0; i < 4; i++) {
										wb[(Map.Side)i] = wallTex;
									}
									chunk[x, y] = wb;
								} else {
									chunk[x, y] = new Map.FloorBlock() {
										HasFloor = true,
										HasCeiling = true,
										Floor = floorTex,
										Ceiling = ceilTex
									};
								}
							} else {
								chunk[x, y] = fb;
							}
						}
					}
					for (int x = 0; x < 8; x++) {

						// Horizontal
						wb = new Map.WallBlock();
						for (int i = 0; i < 4; i++) {
							wb[(Map.Side)i] = hgt == 1 ? ((x == 2 || x == 5) ? wallLampTex : wallTex) : wallLowerTex;
						}
						if (x == 2 || x == 5) {
							Light l = new Light();
							l.Position = new Vector3(x + 0.5f, 0.5f + hgt, 1.1f);
							l.Range = 6f;
							l.Color = x == 5 ? Color.Red : Color.Blue;
							l.Shadows = true;
							scene.Entities.Add(l);
							lights.Add(l);
							l = new Light();
							l.Position = new Vector3(x + 0.5f, 0.5f + hgt, 6.9f);
							l.Range = 6f;
							l.Shadows = true;
							l.Color = x == 5 ? Color.Lime : Color.Yellow;
							scene.Entities.Add(l);
							lights.Add(l);
						}
						chunk[x, 0] = wb;
						chunk[x, 7] = wb;

					}
					for (int y = 1; y < 7; y++) {
						wb = new Map.WallBlock();
						for (int i = 0; i < 4; i++) {
							wb[(Map.Side)i] = hgt == 1 ? wallTex : wallLowerTex;
						}
						chunk[0, y] = wb;
						chunk[7, y] = wb;
					}

					if (hgt == 0) {
						// Height test
						Map.FloorBlock fh = new Map.FloorBlock() {
							HasFloor = true,
							HasCeiling = true,
							Floor = floorTex,
							Ceiling = ceilTex,
							FloorHeight = new float[4] { 
								0.0f, 0.0f, 0.5f, 0.5f
							},
							CeilingHeight = new float[4] { 
								0.0f, 0.0f, 0.2f, 0.2f 
							}
						};
						for (int i = 0; i < 4; i++) {
							fh.CeilingTrim[(Map.Side)i] = wallTex;
							fh.FloorTrim[(Map.Side)i] = wallTex;
						}
						Map.FloorBlock fh2 = new Map.FloorBlock() {
							HasFloor = true,
							HasCeiling = false,
							Floor = floorTex,
							Ceiling = ceilTex,
							FloorHeight = new float[4] { 
								0.7f, 0.0f, 0.7f, 0.0f
							}
						};
						for (int i = 0; i < 4; i++) {
							fh2.CeilingTrim[(Map.Side)i] = wallTex;
							fh2.FloorTrim[(Map.Side)i] = wallTex;
						}
						chunk[3, 3] = fh;
						chunk[2, 3] = fh2;
					}
					
					map[0, hgt, 0] = chunk;
				}
				scene.Map = map;


				collider = new Entity();
				collider.AddComponent(new WireCubeComponent() {
					WireColor = Color.Aqua,
					WireWidth = 1f,
					Size = new Vector3(0.3f, 0.7f, 0.3f)
				});
				collider.BoxCollider = new Collider() {
					Size = new Vector3(0.3f, 0.7f, 0.3f)
				};
				collider.Position = new Vector3(2, 0.5f, 2);
				scene.Entities.Add(collider);

				sprite = new Entity();
				sprite.AddComponent(new SpriteComponent() {
					Texture = new Texture("sprite.png"),
					Facing = SpriteComponent.FacingMode.Y
				});
				sprite.Parent = collider;
				sprite.LocalPosition = Vector3.UnitY * 0.15f;
				scene.Entities.Add(sprite);

				playerController = new WalkController();


				player = new Entity();
				player.BoxCollider = new Collider() {
					Size = new Vector3(0.3f, 0.6f, 0.3f)
				};
				player.AddComponent(new WireCubeComponent() {
					WireColor = Color.LimeGreen,
					WireWidth = 1f,
					Size = new Vector3(0.3f, 0.6f, 0.3f)
				});
				player.AddComponent(playerController);
				scene.Entities.Add(player);
				player.Position = new Vector3(4f, 1f, 2f);



				sw = new Stopwatch();
				fpsFrames = 0;
				sw.Start();

				// Setting scene
				e.CurrentEngine.World = scene;
				initialized = true;
			}

			// Moving camera
			Vector2 rot = Controls.MouseDelta * 0.2f;
			Vector2 mov = Controls.Movement * (Controls.KeyDown(Key.LShift) ? 0.1f : 0.06f);
			//cam.FreeLookControls(new Vector3(mov.X, 0, mov.Y), new Vector3(rot.Y, rot.X, 0));
			//lightTurn += 1;

			// Moving camera
			if (Controls.KeyDown(Key.E)) {
				collider.Angles += Vector3.UnitY;
			}

			collider.BoxCollider.Velocity = -Vector3.UnitY * 0.03f;


			playerController.Control(mov, 0f);
			if (Controls.KeyHit(Key.Space)) {
				playerController.Jump();
			}




			// Счётчики
			if (fpsFrames >= 60) {
				sw.Stop();

				int fps = (int)Math.Ceiling(1000f / ((float)sw.ElapsedMilliseconds / (float)fpsFrames));
				sw.Reset();
				sw.Start();
				fpsFrames = 0;
				fpsLabel.Text = "FPS: " + fps;

			} else {
				fpsFrames++;
			}
			dipLabel.Text = "DrawCalls: " + ((Engine)sender).DrawnPrimitives;


		}
	}
}
