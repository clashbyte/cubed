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
using Cubed.Drivers.Rendering;
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
		/// Штатив
		/// </summary>
		Entity camHolder;

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

		Entity gun;

		Vector2 gunMove;
		
		float bob;


		float landDisp;


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
				//RootFolder = @"D:\Sharp\Cubed\Project"
				RootFolder = AppDomain.CurrentDomain.BaseDirectory + @"\Data"
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

				Display.Current.MouseLock = true;

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
								0.7f, 0.7f, 0.0f, 0.0f
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
				/*
				collider.AddComponent(new WireCubeComponent() {
					WireColor = Color.Aqua,
					WireWidth = 1f,
					Size = new Vector3(0.3f, 0.7f, 0.3f)
				});*/
				collider.BoxCollider = new Collider() {
					Size = new Vector3(0.3f, 0.7f, 0.3f)
				};
				collider.Position = new Vector3(2, 0.5f, 2);
				scene.Entities.Add(collider);

				sprite = new Entity();
				sprite.AddComponent(new SpriteComponent() {
					Texture = new Texture("sprite.png"),
					Facing = SpriteComponent.FacingMode.Y,
					AffectedByLight = true
				});
				sprite.Parent = collider;
				sprite.LocalPosition = Vector3.UnitY * 0.15f;
				scene.Entities.Add(sprite);

				camHolder = new Entity();
				cam.Parent = camHolder;
				cam.LocalPosition = Vector3.Zero;
				cam.LocalAngles = Vector3.Zero;
				scene.Entities.Add(camHolder);

				playerController = new WalkController();
				//playerController.Actor = camHolder;
				//playerController.ActorPosition = 0.2f;

				player = new Entity();
				player.BoxCollider = new Collider() {
					Size = new Vector3(0.35f, 0.5f, 0.35f)
				};
				player.AddComponent(playerController);
				scene.Entities.Add(player);
				player.Position = new Vector3(4f, 1f, 2f);


				// Gun
				gun = new Entity();
				gun.AddComponent(new SpriteComponent() {
					Facing = SpriteComponent.FacingMode.Disabled,
					AffectedByLight = true,
					Scale = Vector2.One * 0.05f,
					Offset = new Vector2(0, -0.04f),
					Texture = new Texture("weapon_01.png", Texture.LoadingMode.Instant)
				});
				gun.Parent = cam;
				gun.LocalPosition = Vector3.UnitZ * 0.1f;
				scene.Entities.Add(gun);

				sw = new Stopwatch();
				fpsFrames = 0;
				sw.Start();

				// Setting scene
				e.CurrentEngine.World = scene;
				initialized = true;
			}



			// Moving camera
			Vector2 rot = Controls.MouseDelta;
			Vector2 mov = Controls.Movement * (Controls.KeyDown(Key.LShift) ? 0.1f : 0.06f);
			gunMove += rot * 0.0001f;
			gunMove *= 0.8f;
			gunMove.X = MathHelper.Clamp(gunMove.X, -0.03f, 0.03f);
			gunMove.Y = MathHelper.Clamp(gunMove.Y, -0.02f, 0.002f);
			rot *= 0.12f;

			// Moving camera
			if (Controls.KeyDown(Key.E)) {
				collider.Angles += Vector3.UnitY;
			}

			collider.BoxCollider.Velocity = -Vector3.UnitY * 0.03f;

			
			Vector3 ang = camHolder.LocalAngles + new Vector3(rot.Y, rot.X, 0);
			ang.X = Math.Sign(ang.X) * Math.Min(Math.Abs(ang.X), 90);
			ang.Y = ang.Y % 360f;
			ang.Z = 0;
			camHolder.LocalAngles = ang;


			// Movement
			bool run = Controls.KeyDown(Key.ShiftLeft);
			playerController.Control(mov, run, cam.Angles.Y);
			if (Controls.KeyHit(Key.Space)) {
				if (playerController.IsGrounded) {
					playerController.Jump();
					landDisp = 0.04f;
				}
			}

			// View bobbing
			float bobDist = 0.04f;
			if (run) {
				bobDist = 0.08f;
			}
			if ((mov.X != 0 || mov.Y != 0) && playerController.IsGrounded) {
				bob += 0.1f;
				if (run) {
					bob += 0.05f;
				}
			} else {
				bob = 0;
			}
			float bobAng = (float)MathHelper.DegreesToRadians(bob);
			Vector3 bobVec = new Vector3((float)Math.Sin(bob) * bobDist, (float)Math.Cos(bob * 2f) * bobDist - bobDist, 0);
			cam.LocalPosition += (bobVec - cam.LocalPosition) * 0.2f;
			
			// Positioning cam
			Vector3 plrPos = player.Position;
			Vector3 camPos = camHolder.Position;
			camHolder.Position = new Vector3(plrPos.X, camPos.Y + (plrPos.Y + 0.2f - landDisp - camPos.Y) * 0.3f, plrPos.Z);
			gun.LocalPosition = Vector3.UnitZ * 0.1f + cam.LocalPosition * 0.1f - Vector3.UnitY * landDisp * 0.1f + new Vector3(-gunMove.X, gunMove.Y, 0);
			if (playerController.IsLanded) {
				landDisp = Math.Min(playerController.LandVelocity * 3f, 0.5f);
			}
			landDisp *= 0.85f;

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

			if (Controls.KeyHit(Key.Escape)) {
				Display.Current.Close();
			}

		}
	}
}
