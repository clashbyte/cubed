﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	public class LevelTest2 {

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
		/// Block texture
		/// </summary>
		Texture blockTex;

		/// <summary>
		/// Light angle
		/// </summary>
		float lightTurn;

		/// <summary>
		/// Spinning light
		/// </summary>
		Light spinLight;

		/// <summary>
		/// Current entity
		/// </summary>
		Entity[] ents;

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
			engine.UpdateLogic += engine_UpdateLogic;
			//engine.MouseLock = true;
			engine.Filesystem = new FolderFileSystem() {
				RootFolder = @"D:\Sharp\Cubed\Project"
			};

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
				cam.Position = new Vector3(4, 3, -1);
				cam.Angles = new Vector3(35, 0, 0);

				// Scene
				scene = new Scene();
				scene.Camera = cam;
				

				// Building map
				Map map = new Map();
				map.Ambient = Color.FromArgb(5, 5, 20);
				
				// Textures
				Texture wallTex = new Texture("40.bmp") {
					Filtering = Texture.FilterMode.Disabled
				};
				Texture wallLampTex = new Texture("c_002.bmp") {
					Filtering = Texture.FilterMode.Disabled
				};
				Texture floorTex = new Texture("floor_004.bmp") {
					Filtering = Texture.FilterMode.Disabled
				};
				Texture ceilTex = new Texture("floor_005.bmp") {
					Filtering = Texture.FilterMode.Disabled
				};

				// Chunk
				Map.Chunk chunk = new Map.Chunk();

				Map.FloorBlock fb = new Map.FloorBlock() {
					HasFloor = true,
					HasCeiling = true,
					Floor = floorTex,
					Ceiling = ceilTex
				};
				Map.WallBlock wb;
				for (int y = 1; y < 7; y++) {
					for (int x = 1; x < 7; x++) {
						if (x == 3 && y == 4 || x >= 4 && y == 3) {
							wb = new Map.WallBlock();
							for (int i = 0; i < 4; i++) {
								wb[(Map.Side)i] = wallTex;
							}
							chunk[x, y] = wb;
						} else {
							chunk[x, y] = fb;
						}
					}
				}
				for (int x = 0; x < 8; x++) {
					
					// Horizontal
					wb = new Map.WallBlock();
					for (int i = 0; i < 4; i++) {
						wb[(Map.Side)i] = (x == 2 || x == 5) ? wallLampTex : wallTex;
					}
					if (x == 2 || x == 5) {
						Light l = new Light();
						l.Position = new Vector3(x + 0.5f, 0.5f, 1.1f);
						l.Range = 6f;
						l.Color = x == 5 ? Color.Red : Color.Blue;
						l.Shadows = true;
						scene.Entities.Add(l);
						lights.Add(l);
						l = new Light();
						l.Position = new Vector3(x + 0.5f, 0.5f, 6.9f);
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
						wb[(Map.Side)i] = wallTex;
					}
					chunk[0, y] = wb;
					chunk[7, y] = wb;
				}


				map[0, 0, 0] = chunk;
				scene.Map = map;

				
				spinLight = new Light();
				spinLight.Color = Color.White;
				spinLight.Range = 4f;
				spinLight.AddComponent(new SpriteComponent(){
					Texture = new Texture("sprite.png"),
					Facing = SpriteComponent.FacingMode.Y
				});
				spinLight.Angles = Vector3.UnitY * 90f;
				scene.Entities.Add(spinLight);
				
				ents = new Entity[30];
				for (int i = 0; i < ents.Length; i++) {
					Entity ent = new Entity();
					ent.AddComponent(new SpriteComponent() {
						Texture = new Texture("sprite.png"),
						Facing = SpriteComponent.FacingMode.Y,
						Scale = Vector2.One * (float)r.NextDouble()
					});
					if (i == 0) {
						spinLight.Parent = ent;
						spinLight.LocalPosition = Vector3.Zero;
					}
					scene.Entities.Add(ent);
					ents[i] = ent;
				}

				sw = new Stopwatch();
				fpsFrames = 0;
				sw.Start();

				// Setting scene
				e.CurrentEngine.World = scene;
				initialized = true;
			}

			// Moving camera
			Vector2 rot = Controls.MouseDelta * 0.2f;
			Vector2 mov = Controls.Movement * (Controls.KeyDown(Key.LShift) ? 0.5f : 0.1f);
			cam.FreeLookControls(new Vector3(mov.X, 0, mov.Y), new Vector3(rot.Y, rot.X, 0));
			lightTurn += 1;

			// Moving camera
			if (Controls.KeyHit(Key.Q)) {
				//Engine.Current.MouseLock = !Engine.Current.MouseLock;
			}

			
			float rstep = ((float)Math.PI * 2f) / (float)ents.Length;
			for (int i = 0; i < ents.Length; i++) {
				Entity ent = ents[i];
				float rad = i * rstep + lightTurn * 0.01f;
				ent.Position = new Vector3(4 + (float)Math.Sin(rad) * 2f, 0.5f, 4 + (float)Math.Cos(rad) * 2f);
				ent.GetComponent<SpriteComponent>().Tint = scene.GetLightAtPoint(ent.Position.X, ent.Position.Y, ent.Position.Z);
			}
			 
			 
			
			//spinLight.Angles = new Vector3(lightTurn, lightTurn * 2, lightTurn * 3);
			//spinLight.Position = new Vector3(4 - (float)Math.Cos(-lightTurn * 0.02f) * 2.5f, 0.5f, 4 + (float)Math.Sin(-lightTurn * 0.01f) * 2.5f);

			spinLight.Range = (float)r.NextDouble() * 0.5f + 2.5f;

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
