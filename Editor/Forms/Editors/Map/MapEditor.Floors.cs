using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Components.Rendering;
using Cubed.UI;
using Cubed.UI.Controls;
using Cubed.World;
using OpenTK;
using OpenTK.Input;

namespace Cubed.Forms.Editors.Map {
	partial class MapEditor {

		/// <summary>
		/// Floor proxies
		/// </summary>
		List<FloorEditorProxy> floorProxies;

		/// <summary>
		/// User interface for walls
		/// </summary>
		UserInterface floorInterface;

		/// <summary>
		/// Starting floor position
		/// </summary>
		Vector3 floorStartPos;

		/// <summary>
		/// Current floor selection pos
		/// </summary>
		Vector3 floorCurrentPos;

		/// <summary>
		/// Currently drawing walls
		/// </summary>
		bool floorDrawing;

		/// <summary>
		/// Erasing walls mode
		/// </summary>
		bool floorErasing;

		/// <summary>
		/// Opening floor editor
		/// </summary>
		void FloorToolOpen() {
			if (floorProxies == null) {
				floorProxies = new List<FloorEditorProxy>();
			}
			if (floorInterface == null) {
				floorInterface = new UserInterface();
			}
			engine.Interface = floorInterface;
			floorProxies.Clear();
			allowMouseLook = true;
			screen.Cursor = System.Windows.Forms.Cursors.Cross;
		}

		/// <summary>
		/// Closing floor editor
		/// </summary>
		void FloorToolClose() {
			foreach (FloorEditorProxy prx in floorProxies) {
				prx.Kill();
			}
			engine.Interface = null;
			allowMouseLook = true;
			screen.Cursor = System.Windows.Forms.Cursors.Default;
		}

		/// <summary>
		/// Floor tool updating
		/// </summary>
		void FloorToolUpdate() {
			// Hiding all proxies
			bool needRebuild = false;
			bool pickVisible = Input.Controls.Mouse != Vector2.One * -1;
			if (Input.Controls.MouseHit(MouseButton.Middle) && allowMouseLook || !pickVisible) {
				foreach (FloorEditorProxy prx in floorProxies) {
					prx.Block.Visible = false;
				}
			} else if (Input.Controls.MouseReleased(MouseButton.Middle) && allowMouseLook && pickVisible) {
				foreach (FloorEditorProxy prx in floorProxies) {
					prx.Block.Visible = true;
					needRebuild = true;
				}
			}

			// Picking current grid position
			Vector3 camPos = cam.ScreenToPoint(Input.Controls.Mouse.X, Input.Controls.Mouse.Y, 0);
			Vector3 camDir = cam.ScreenToPoint(Input.Controls.Mouse.X, Input.Controls.Mouse.Y, 1) - camPos;
			Vector3 pickPos = Vector3.Zero;
			float gridh = (((cam.Position.Y < gridHeight + 1) && (cam.Angles.X < 0)) ? gridHeight + 1 : gridHeight);
			if (Maths.Intersections.RayPlane(Vector3.UnitY * gridh, Vector3.UnitY, camPos, camDir.Normalized(), out pickPos)) {
				pickPos = new Vector3((float)System.Math.Floor(pickPos.X), gridh, (float)System.Math.Floor(pickPos.Z));
				Vector2 pcam = pickPos.Xz - cam.Position.Xz;
				float mag = pcam.Length;
				if (mag > 50) {
					pickPos.X = (float)Math.Floor(cam.Position.X + pcam.X / mag * 50f);
					pickPos.Z = (float)Math.Floor(cam.Position.Z + pcam.Y / mag * 50f);
				}
				if (floorCurrentPos != pickPos || needRebuild) {
					floorCurrentPos = pickPos;
					needRebuild = true;
				}
			}

			// Modifiers
			if (Input.Controls.KeyHit(Key.ControlLeft) || Input.Controls.KeyReleased(Key.ControlLeft)) {
				floorErasing = Input.Controls.KeyDown(Key.ControlLeft);
			}
			if (Input.Controls.MouseHit(MouseButton.Left)) {
				floorStartPos = floorCurrentPos;
				floorDrawing = true;
				needRebuild = true;
			} else if (Input.Controls.MouseReleased(MouseButton.Left)) {
				FloorToolProcess(floorStartPos, floorCurrentPos, floorErasing);
				floorDrawing = false;
				needRebuild = true;
			}
			allowMouseLook = !floorDrawing;

			// Rebuilding floors
			if (needRebuild && !display.MouseLock) {

				// Cleaning
				foreach (FloorEditorProxy prx in floorProxies) {
					prx.Kill();
				}
				floorProxies.Clear();

				// Filling
				FloorEditorProxy fp = null;
				if (floorDrawing) {
					int sx = (int)Math.Min(floorCurrentPos.X, floorStartPos.X);
					int sy = (int)Math.Min(floorCurrentPos.Z, floorStartPos.Z);
					int wx = (int)Math.Abs(floorCurrentPos.X - floorStartPos.X);
					int wy = (int)Math.Abs(floorCurrentPos.Z - floorStartPos.Z);
					for (int y = sy; y <= wy + sy; y++) {
						for (int x = sx; x <= wx + sx; x++) {
							fp = new FloorEditorProxy(scene);
							fp.WobbleOffset = 0.1f * (x - sx + y - sy);
							fp.Block.Position = new Vector3(x, gridh, y);
							floorProxies.Add(fp);
						}
					}
				} else {
					if (pickVisible) {
						fp = new FloorEditorProxy(scene);
						fp.WobbleOffset = 0;
						fp.Block.Position = floorCurrentPos;
						floorProxies.Add(fp);
					}
				}
			}

			// Wobbling colors for proxies
			foreach (FloorEditorProxy prx in floorProxies) {
				if (floorErasing) {
					prx.Wire.WireColor = System.Drawing.Color.FromArgb((int)((System.Math.Sin((wobble + prx.WobbleOffset) * (System.Math.PI * 2)) * 0.5 + 0.5) * 128) + 127, System.Drawing.Color.Red);
				} else {
					prx.Wire.WireColor = System.Drawing.Color.FromArgb((int)((System.Math.Sin((wobble + prx.WobbleOffset) * (System.Math.PI * 2)) * 0.5 + 0.5) * 128) + 127, NSTheme.UI_ACCENT);
				}
			}
		}

		/// <summary>
		/// Struct for floor proxy
		/// </summary>
		class FloorEditorProxy {
			public Entity Block;
			public float WobbleOffset;
			public WireCubeComponent Wire;
			Scene scene;
			public FloorEditorProxy(Scene s) {
				Block = new Entity();
				Block.Visible = true;
				Block.AddComponent(Wire = new WireCubeComponent() {
					Enabled = true,
					Position = new Vector3(0.5f, 0, 0.5f),
					Size = new Vector3(1f, 0.02f, 1f),
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
		/// <param name="erase">Erasing mode</param>
		void FloorToolProcess(Vector3 start, Vector3 end, bool erase) {

			// Collecting points
			List<Vector3> points = new List<Vector3>();
			int sx = (int)Math.Min(start.X, end.X);
			int sy = (int)Math.Min(start.Z, end.Z);
			int wx = (int)Math.Abs(start.X - end.X);
			int wy = (int)Math.Abs(start.Z - end.Z);
			for (int y = sy; y <= wy + sy; y++) {
				for (int x = sx; x <= wx + sx; x++) {
					points.Add(new Vector3(x, start.Y, y));
				}
			}
			
			// Processing walls
			if (points.Count > 0) {
				TriggerChanges();
			}
			foreach (Vector3 point in points) {

				// Blocks
				World.Map.Block top = map.GetBlockAtCoords(point.X, point.Y, point.Z);
				World.Map.Block bottom = map.GetBlockAtCoords(point.X, point.Y - 1, point.Z);

				if (!erase) {

					// Creating floor
					World.Map.FloorBlock ftop = top as World.Map.FloorBlock;
					if (ftop == null && !(top is World.Map.WallBlock)) {
						ftop = new World.Map.FloorBlock();
					}
					if (ftop != null) {
						if (!ftop.HasFloor) {
							ftop.HasFloor = true;
							ftop.Floor = GetCurrentTexture(emptyFloorTex);
							for (int i = 0; i < 4; i++) {
								ftop.FloorTrim[(World.Map.Side)i] = emptyFloorTex;
							}
						}
						map.SetBlockAtCoords((int)point.X, (int)point.Y, (int)point.Z, ftop);
					}

					// Creating ceiling
					World.Map.FloorBlock fbottom = bottom as World.Map.FloorBlock;
					if (fbottom == null && !(bottom is World.Map.WallBlock)) {
						fbottom = new World.Map.FloorBlock();
					}
					if (fbottom != null) {
						if (!fbottom.HasCeiling) {
							fbottom.HasCeiling = true;
							fbottom.Ceiling = GetCurrentTexture(emptyCeilTex);
							for (int i = 0; i < 4; i++) {
								fbottom.CeilingTrim[(World.Map.Side)i] = emptyCeilTex;
							}
						}
						map.SetBlockAtCoords((int)point.X, (int)point.Y - 1, (int)point.Z, fbottom);
					}

				} else {

					// Creating floor
					World.Map.FloorBlock ftop = top as World.Map.FloorBlock;
					if (ftop != null && !(bottom is World.Map.WallBlock)) {
						if (ftop.HasFloor) {
							ftop.HasFloor = false;
							ftop.FloorHeight = new float[] { 0, 0, 0, 0 };
							for (int i = 0; i < 4; i++) {
								ftop.FloorTrim[(World.Map.Side)i] = emptyFloorTex;
							}
						}
						map.SetBlockAtCoords((int)point.X, (int)point.Y, (int)point.Z, ftop);
					}

					// Erasing ceiling
					World.Map.FloorBlock fbottom = bottom as World.Map.FloorBlock;
					if (fbottom != null && !(top is World.Map.WallBlock)) {
						if (fbottom.HasCeiling) {
							fbottom.HasCeiling = false;
							fbottom.CeilingHeight = new float[] { 0, 0, 0, 0 };
							for (int i = 0; i < 4; i++) {
								ftop.CeilingTrim[(World.Map.Side)i] = emptyCeilTex;
							}
						}
						map.SetBlockAtCoords((int)point.X, (int)point.Y - 1, (int)point.Z, fbottom);
					}

				}
			}
		}
	}
}
