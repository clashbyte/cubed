using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Components.Rendering;
using Cubed.UI;
using Cubed.UI.Basic;
using Cubed.UI.Controls;
using Cubed.World;
using OpenTK;
using OpenTK.Input;

namespace Cubed.Forms.Editors.Map {
	partial class MapEditor {

		/// <summary>
		/// Wall proxies
		/// </summary>
		List<WallEditorProxy> wallProxies;

		/// <summary>
		/// User interface for walls
		/// </summary>
		UserInterface wallsInterface;

		/// <summary>
		/// Starting wall position
		/// </summary>
		Vector3 wallStartPos;

		/// <summary>
		/// Current wall selection pos
		/// </summary>
		Vector3 wallCurrentPos;

		/// <summary>
		/// Currently drawing walls
		/// </summary>
		bool wallsDrawing;

		/// <summary>
		/// Drawing walls in rect mode
		/// </summary>
		bool wallsRectMode;

		/// <summary>
		/// Erasing walls mode
		/// </summary>
		bool wallsErasing;

		/// <summary>
		/// Opening wall editor
		/// </summary>
		void WallToolOpen() {
			if (wallProxies == null) {
				wallProxies = new List<WallEditorProxy>();
			}
			if (wallsInterface == null) {
				wallsInterface = new UserInterface();
				/*
				wallsInterface.Items.Add(new Label() {
					Position = Vector2.Zero,
					Text = "Testing"
				});
				*/
			}
			engine.Interface = wallsInterface;
			wallProxies.Clear();
			allowMouseLook = true;
			screen.Cursor = System.Windows.Forms.Cursors.Cross;
		}

		/// <summary>
		/// Closing wall editor
		/// </summary>
		void WallToolClose() {
			foreach (WallEditorProxy prx in wallProxies) {
				prx.Kill();
			}
			engine.Interface = null;
			allowMouseLook = true;
			screen.Cursor = System.Windows.Forms.Cursors.Default;
		}

		/// <summary>
		/// Updating walls
		/// </summary>
		void WallToolUpdate() {

			// Hiding all proxies
			bool needRebuild = false;
			if (Input.Controls.MouseHit(MouseButton.Middle) && allowMouseLook) {
				foreach (WallEditorProxy prx in wallProxies) {
					prx.Block.Visible = false;
				}
			} else if (Input.Controls.MouseReleased(MouseButton.Middle) && allowMouseLook) {
				foreach (WallEditorProxy prx in wallProxies) {
					prx.Block.Visible = true;
					needRebuild = true;
				}
			}
			
			// Picking current grid position
			Vector3 camPos = cam.ScreenToPoint(Input.Controls.Mouse.X, Input.Controls.Mouse.Y, 0);
			Vector3 camDir = cam.ScreenToPoint(Input.Controls.Mouse.X, Input.Controls.Mouse.Y, 1) - camPos;
			Vector3 pickPos = Vector3.Zero;
			if (Cubed.Maths.Intersections.RayPlane(Vector3.UnitY * (((cam.Position.Y < gridHeight + 1) && (cam.Angles.X < 0)) ? gridHeight + 1 : gridHeight), Vector3.UnitY, camPos, camDir.Normalized(), out pickPos)) {
				pickPos = new Vector3((float)System.Math.Floor(pickPos.X), gridHeight, (float)System.Math.Floor(pickPos.Z));
				Vector2 pcam = pickPos.Xz - cam.Position.Xz;
				float mag = pcam.Length;
				if (mag > 100) {
					pickPos.X = (float)Math.Floor(cam.Position.X + pcam.X / mag * 100f);
					pickPos.Z = (float)Math.Floor(cam.Position.Z + pcam.Y / mag * 100f);
				}
				if (wallCurrentPos != pickPos || needRebuild) {
					wallCurrentPos = pickPos;
					needRebuild = true;
				}
			}

			// Modifiers
			if (Input.Controls.KeyHit(Key.ShiftLeft) || Input.Controls.KeyReleased(Key.ShiftLeft)) {
				wallsRectMode = Input.Controls.KeyDown(Key.ShiftLeft);
				needRebuild = true;
			}
			if (Input.Controls.KeyHit(Key.ControlLeft) || Input.Controls.KeyReleased(Key.ControlLeft)) {
				wallsErasing = Input.Controls.KeyDown(Key.ControlLeft);
			}
			if (Input.Controls.MouseHit(MouseButton.Left)) {
				wallStartPos = wallCurrentPos;
				wallsDrawing = true;
				needRebuild = true;
			} else if (Input.Controls.MouseReleased(MouseButton.Left)) {
				WallToolProcess(wallStartPos, wallCurrentPos, wallsRectMode, wallsErasing);
				wallsDrawing = false;
				needRebuild = true;
			}
			allowMouseLook = !wallsDrawing;

			// Rebuilding walls
			if (needRebuild && !display.MouseLock) {

				// Cleaning
				foreach (WallEditorProxy prx in wallProxies) {
					prx.Kill();
				}
				wallProxies.Clear();

				// Filling
				WallEditorProxy wp = null;
				if (wallsDrawing) {
					if (wallsRectMode) {
						int sx = (int)Math.Min(wallCurrentPos.X, wallStartPos.X);
						int sy = (int)Math.Min(wallCurrentPos.Z, wallStartPos.Z);
						int wx = (int)Math.Abs(wallCurrentPos.X - wallStartPos.X);
						int wy = (int)Math.Abs(wallCurrentPos.Z - wallStartPos.Z);
						for (int x = sx; x <= wx + sx; x++) {

							// Top side
							wp = new WallEditorProxy(scene);
							wp.WobbleOffset = 0.1f * (x - sx);
							wp.Block.Position = new Vector3(x, gridHeight, sy);
							wallProxies.Add(wp);

							// Bottom side, if size is greated than two
							if (wy > 0) {
								wp = new WallEditorProxy(scene);
								wp.WobbleOffset = 0.1f * (x - sx + wy);
								wp.Block.Position = new Vector3(x, gridHeight, sy + wy);
								wallProxies.Add(wp);
							}

						}
						if (wy > 1) {
							for (int y = sy + 1; y < wy + sy; y++) {

								// Top side
								wp = new WallEditorProxy(scene);
								wp.WobbleOffset = 0.1f * (y - sy + 1);
								wp.Block.Position = new Vector3(sx, gridHeight, y);
								wallProxies.Add(wp);

								// Bottom side, if size is greated than two
								if (wx > 0) {
									wp = new WallEditorProxy(scene);
									wp.WobbleOffset = 0.1f * (y - sy + wx);
									wp.Block.Position = new Vector3(sx + wx, gridHeight, y);
									wallProxies.Add(wp);
								}

							}
						}
					} else {
						
						// Drawing line
						Vector2[] points = Maths.Lines.Bresenham(wallStartPos.Xz, wallCurrentPos.Xz);
						for (int i = 0; i < points.Length; i++) {
							wp = new WallEditorProxy(scene);
							wp.WobbleOffset = 0.1f * i;
							wp.Block.Position = new Vector3(points[i].X, gridHeight, points[i].Y);
							wallProxies.Add(wp);
						}

					}
				} else {
					wp = new WallEditorProxy(scene);
					wp.WobbleOffset = 0;
					wp.Block.Position = wallCurrentPos;
					wallProxies.Add(wp);
				}
			}
			
			// Wobbling colors for proxies
			foreach (WallEditorProxy prx in wallProxies) {
				if (wallsErasing) {
					prx.Wire.WireColor = System.Drawing.Color.FromArgb((int)((System.Math.Sin((wobble + prx.WobbleOffset) * (System.Math.PI * 2)) * 0.5 + 0.5) * 128) + 127, System.Drawing.Color.Red);
				} else {
					prx.Wire.WireColor = System.Drawing.Color.FromArgb((int)((System.Math.Sin((wobble + prx.WobbleOffset) * (System.Math.PI * 2)) * 0.5 + 0.5) * 128) + 127, NSTheme.UI_ACCENT);
				}
			}
		}

		/// <summary>
		/// Struct for wall proxy
		/// </summary>
		class WallEditorProxy {
			public Entity Block;
			public float WobbleOffset;
			public WireCubeComponent Wire;
			Scene scene;
			public WallEditorProxy(Scene s) {
				Block = new Entity();
				Block.Visible = true;
				Block.AddComponent(Wire = new WireCubeComponent() {
					Enabled = true,
					Position = Vector3.One * 0.5f,
					Size = Vector3.One,
					WireColor = System.Drawing.Color.Red,
					WireWidth = 2f
				});
				s.Entities.Add(Block);
				scene = s;
			}
			public void Kill() {
				Block.Destroy();
				scene.Entities.Remove(Block);
			}
		}

		/// <summary>
		/// Processing walls
		/// </summary>
		/// <param name="start">Starting point</param>
		/// <param name="end">Ending point</param>
		/// <param name="rectMode">Use rectangle method</param>
		/// <param name="erase">Erasing mode</param>
		void WallToolProcess(Vector3 start, Vector3 end, bool rectMode, bool erase) {

			// Collecting points
			List<Vector3> points = new List<Vector3>();
			if (rectMode) {
				int sx = (int)Math.Min(start.X, end.X);
				int sy = (int)Math.Min(start.Z, end.Z);
				int wx = (int)Math.Abs(start.X - end.X);
				int wy = (int)Math.Abs(start.Z - end.Z);
				for (int x = sx; x <= wx + sx; x++) {
					points.Add(new Vector3(x, start.Y, sy));
					if (wy > 0) {
						points.Add(new Vector3(x, start.Y, sy + wy));
					}
				}
				if (wy > 1) {
					for (int y = sy + 1; y < wy + sy; y++) {
						points.Add(new Vector3(sx, start.Y, y));
						if (wy > 0) {
							points.Add(new Vector3(sx + wx, start.Y, y));
						}
					}
				}
			} else {
				Vector2[] pl = Maths.Lines.Bresenham(start.Xz, end.Xz);
				foreach (Vector2 p in pl) {
					points.Add(new Vector3(p.X, start.Y, p.Y));
				}
			}

			// Processing walls
			foreach (Vector3 point in points) {

				// Blocks
				World.Map.Block top = map.GetBlockAtCoords(point.X, point.Y + 1, point.Z);
				World.Map.Block mid = map.GetBlockAtCoords(point.X, point.Y, point.Z);
				World.Map.Block bottom = map.GetBlockAtCoords(point.X, point.Y - 1, point.Z);

				if (!erase) {

					// Creating block
					if (mid == null || !(mid is World.Map.WallBlock)) {
						World.Map.WallBlock wall = new World.Map.WallBlock();
						for (int i = 0; i < 6; i++) {
							wall[(Cubed.World.Map.Side)i] = GetCurrentTexture(emptyWallTex);
						}
						map.SetBlockAtCoords((int)point.X, (int)point.Y, (int)point.Z, wall);

						// Top and bottom blocks
						World.Map.FloorBlock ftop = top as World.Map.FloorBlock, fbottom = bottom as World.Map.FloorBlock;

						// Making ceiling
						if (bottom == null || (!(bottom is World.Map.FloorBlock) && !(bottom is World.Map.WallBlock))) {
							fbottom = new World.Map.FloorBlock();
							map.SetBlockAtCoords((int)point.X, (int)point.Y - 1, (int)point.Z, fbottom);
						}
						if (fbottom != null && !fbottom.HasCeiling) {
							fbottom.HasCeiling = true;
							for (int i = 0; i < 4; i++) {
								fbottom.CeilingHeight[i] = 0;
								fbottom.CeilingTrim[(Cubed.World.Map.Side)i] = emptyCeilTex;
							}
							fbottom.Ceiling = emptyCeilTex;
						}

						// Making floor
						if (top == null || (!(top is World.Map.FloorBlock) && !(top is World.Map.WallBlock))) {
							ftop = new World.Map.FloorBlock();
							map.SetBlockAtCoords((int)point.X, (int)point.Y + 1, (int)point.Z, ftop);
						}
						if (ftop != null && !ftop.HasFloor) {
							ftop.HasFloor = true;
							for (int i = 0; i < 4; i++) {
								ftop.FloorHeight[i] = 0;
								ftop.FloorTrim[(Cubed.World.Map.Side)i] = emptyFloorTex;
							}
							ftop.Floor = emptyFloorTex;
						}

					}

				} else {

					// Erasing blocks
					if (mid is World.Map.WallBlock) {
						World.Map.Block block = null;
						if (top is World.Map.FloorBlock) {
							(top as World.Map.FloorBlock).HasFloor = false;
						}
						if (bottom is World.Map.FloorBlock) {
							(bottom as World.Map.FloorBlock).HasCeiling = false;
						}
						if ((bottom is World.Map.WallBlock) || (top is World.Map.WallBlock)) {
							World.Map.FloorBlock fb = new World.Map.FloorBlock();
							if (bottom is World.Map.WallBlock) {
								fb.HasFloor = true;
								fb.Floor = emptyFloorTex;
								fb.FloorHeight = new float[] { 0, 0, 0, 0 };
							}
							if (top is World.Map.WallBlock) {
								fb.HasCeiling = true;
								fb.Ceiling = emptyCeilTex;
								fb.CeilingHeight = new float[] { 0, 0, 0, 0 };
							}
							block = fb;
						}
						map.SetBlockAtCoords((int)point.X, (int)point.Y, (int)point.Z, block);
					}

				}
			}
		}
	}
}
