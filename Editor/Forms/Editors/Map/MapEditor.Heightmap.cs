using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Components.Rendering;
using Cubed.Maths;
using Cubed.UI.Controls;
using Cubed.World;
using OpenTK;
using OpenTK.Input;

namespace Cubed.Forms.Editors.Map {
	partial class MapEditor {

		/// <summary>
		/// Heightmap levels
		/// </summary>
		const int HEIGHT_LEVELS = 16;

		/// <summary>
		/// Modification mask
		/// </summary>
		static readonly float[][] heightGizmoWeights = new float[][]{
			new float[] {
				0, 0,
				1, 0
			},
			new float[] {
				0, 0,
				1, 1
			},
			new float[] {
				0, 0,
				0, 1
			},
			new float[] {
				1, 0,
				1, 0
			},
			new float[] {
				1, 1,
				1, 1
			},
			new float[] {
				0, 1,
				0, 1
			},
			new float[] {
				1, 0,
				0, 0
			},
			new float[] {
				1, 1,
				0, 0
			},
			new float[] {
				0, 1,
				0, 0
			},
		};

		/// <summary>
		/// Gizmos for changing height
		/// </summary>
		Entity[] heightGizmos;

		/// <summary>
		/// Bounds
		/// </summary>
		Vector2 heightBoundsMin, heightBoundsMax;

		/// <summary>
		/// Alpha for gizmos
		/// </summary>
		float[] heightGizmoAlpha;

		/// <summary>
		/// Heightmap cells
		/// </summary>
		List<HeightCell> heightCells;

		/// <summary>
		/// Current cell
		/// </summary>
		HeightCell currentHeightCell;

		/// <summary>
		/// Current gizmo
		/// </summary>
		int currentHeightGizmo;

		/// <summary>
		/// Points cache
		/// </summary>
		float[] heightPointsCache;

		/// <summary>
		/// Current selected items
		/// </summary>
		List<PaintEditorProxy> heightProxies;

		/// <summary>
		/// Currently height dragging
		/// </summary>
		bool heightChanging;

		/// <summary>
		/// Currently selecting height
		/// </summary>
		bool heightCellSelecting;

		/// <summary>
		/// Current height plane position
		/// </summary>
		Vector3 heightPlanePos;

		/// <summary>
		/// Current height plane direction
		/// </summary>
		Vector3 heightPlaneDir;

		/// <summary>
		/// Y of initial pick
		/// </summary>
		float heightPickLevel;

		/// <summary>
		/// Previous position of level
		/// </summary>
		float heightPrevLevel;

		/// <summary>
		/// Y of gizmo
		/// </summary>
		float heightGizmoOriginal;

		/// <summary>
		/// Opening heightmap tool
		/// </summary>
		void HeightToolOpen() {

			// Switching cursor
			screen.Cursor = System.Windows.Forms.Cursors.Cross;

			// Creating gizmos
			if (heightGizmos == null) {
				heightGizmos = new Entity[9];
				for (int i = 0; i < 9; i++) {
					Entity gizmo = new Entity();
					gizmo.AddComponent(new WireCubeComponent() {
						Size = Vector3.One * 0.15f,
						WireWidth = 2f
					});
					gizmo.Visible = false;
					heightGizmos[i] = gizmo;
				}
			}
			foreach (Entity gizmo in heightGizmos) {
				scene.Entities.Add(gizmo);
			}

			if (heightCells == null) {
				heightCells = new List<HeightCell>();
			}
			if (heightProxies == null) {
				heightProxies = new List<PaintEditorProxy>();
			}
			heightGizmoAlpha = new float[9];
			heightCells.Clear();
			currentHeightGizmo = -1;

		}

		/// <summary>
		/// Closing heightmap tool
		/// </summary>
		void HeightToolClose() {

			// Switching cursor
			screen.Cursor = System.Windows.Forms.Cursors.Default;

			// Removing gizmos
			foreach (Entity gizmo in heightGizmos) {
				scene.Entities.Remove(gizmo);
			}
			foreach (PaintEditorProxy prx in heightProxies) {
				prx.Kill();
			}
			

		}

		/// <summary>
		/// Editing height
		/// </summary>
		void HeightToolUpdate() {

			// Rebuilding
			bool needRebuild = false;

			// Picking current grid position
			Vector3 camPos = cam.ScreenToPoint(Input.Controls.Mouse.X, Input.Controls.Mouse.Y, 0);
			Vector3 camDir = cam.ScreenToPoint(Input.Controls.Mouse.X, Input.Controls.Mouse.Y, 1) - camPos;
			camDir.Normalize();
			Vector3 pickPos = Vector3.Zero;
			MapIntersections.Hit hit = MapIntersections.Intersect(camPos, camDir, map);
			HeightCell pickedBlock = null;
			if (hit != null) {
				if (hit.Block is World.Map.FloorBlock) {
					if (hit.Side == World.Map.Side.Top || hit.Side == World.Map.Side.Bottom) {
						if (currentHeightCell != null) {
							if (currentHeightCell.Block != hit.Block || (currentHeightCell.Ceiling != (hit.Side == World.Map.Side.Bottom))) {
								pickedBlock = new HeightCell() {
									Block = hit.Block,
									Ceiling = hit.Side == World.Map.Side.Bottom,
									Pos = hit.Cell
								};
							} else {
								pickedBlock = currentHeightCell;
							}
						} else {
							pickedBlock = new HeightCell() {
								Block = hit.Block,
								Ceiling = hit.Side == World.Map.Side.Bottom,
								Pos = hit.Cell
							};
						}
					}
				}
			}

			// Picking
			float pickDist = float.MaxValue;
			int currentGizmo = -1;
			Vector3 gizmoPoint = Vector3.Zero;
			if (pickedBlock != null) {
				pickDist = (hit.Point - camPos).Length;
			}
			if (heightCells.Count > 0) {
				for (int i = 0; i < heightGizmos.Length; i++) {
					Vector3 gpos = heightGizmos[i].Position;
					float dist = 0;
					if (Intersections.RayIntersectsBox(camPos, camDir, gpos - Vector3.One * 0.1f, gpos + Vector3.One * 0.1f, out dist)) {
						if (dist < pickDist) {
							gizmoPoint = camPos + camDir * dist;
							currentGizmo = i;
							pickDist = dist;
						}
					}
				}
			}
			if (Input.Controls.MouseDown(MouseButton.Middle)) {
				pickedBlock = null;
				currentGizmo = -1;
			}

			if (Input.Controls.MouseHit(MouseButton.Left)) {
				allowMouseLook = false;
				if (currentGizmo != -1) {
					// Selecting dragging gizmo
					if (heightCells.Count > 0) {
						currentHeightGizmo = currentGizmo;
						heightChanging = true;
						HeightCacheCells(heightCells[0].Ceiling);
						needRebuild = true;

						heightPickLevel = gizmoPoint.Y;
						heightGizmoOriginal = heightGizmos[currentGizmo].Position.Y;
						heightPlanePos = gizmoPoint;
						heightPlaneDir = cam.VectorToWorld(-Vector3.UnitZ);
						heightPlaneDir.Y = 0;
						heightPlaneDir.Normalize();
					}
				} else {
					// Selecting cells
					if (!Input.Controls.KeyDown(Key.ShiftLeft)) {
						heightCells.Clear();
					}
					heightCellSelecting = true;
					needRebuild = true;
				}

			} else if (Input.Controls.MouseReleased(MouseButton.Left)) {
				if (heightChanging) {
					HeightmapCalculateGizmos();
				}
				allowMouseLook = true;
				heightCellSelecting = false;
				heightChanging = false;
			}
			if (heightChanging) {
				// Picking point on plane
				Vector3 planeHit = Vector3.Zero;
				if (Intersections.RayPlane(heightPlanePos, heightPlaneDir, camPos, camDir, out planeHit)) {
					float div = 1f / (float)HEIGHT_LEVELS;
					float level = planeHit.Y - heightPickLevel + heightGizmoOriginal - heightCells[0].Pos.Y;
					level = Math.Max(Math.Min((float)Math.Round(level / (float)div) * div, 1), 0);
					
					// Implement caching
					if (level != heightPrevLevel) {
						float delta = level - (heightGizmoOriginal - heightCells[0].Pos.Y);
						if (heightCells[0].Ceiling) {
							delta = -delta;
						}
						HeightModify(delta, currentHeightGizmo);
						heightPrevLevel = level;
						needRebuild = true;
					}
					
					Vector3 pos = heightGizmos[currentHeightGizmo].Position;
					pos.Y = level + heightCells[0].Pos.Y;
					heightGizmos[currentHeightGizmo].Position = pos;
				}

			} else if(heightCellSelecting) {
				// Picking cells
				if (pickedBlock != null) {
					bool found = false;
					foreach (HeightCell hc in heightCells) {
						if (hc.Block == pickedBlock.Block && hc.Ceiling == pickedBlock.Ceiling && hc.Pos == pickedBlock.Pos) {
							found = true;
							break;
						}
					}
					if (!found) {
						if (heightCells.Count > 0) {
							if (heightCells[0].Ceiling != pickedBlock.Ceiling || heightCells[0].Pos.Y != pickedBlock.Pos.Y) {
								heightCells.Clear();
							}
						}
						heightCells.Add(pickedBlock);
						HeightmapCalculateGizmos();
						needRebuild = true;
					}
				}
			} else {
				if (currentGizmo != currentHeightGizmo) {
					screen.Cursor = currentGizmo == -1 ? System.Windows.Forms.Cursors.Cross : System.Windows.Forms.Cursors.SizeNS;
					currentHeightGizmo = currentGizmo;
				}
			}

			// Setting picked block to current
			if (currentHeightGizmo != -1) {
				pickedBlock = null;
			}
			if (pickedBlock != currentHeightCell) {
				currentHeightCell = pickedBlock;
				needRebuild = true;
			}

			// Rebuilding proxies
			if (needRebuild) {
				
				// Cleaning
				foreach (PaintEditorProxy prx in heightProxies) {
					prx.Kill();
				}
				heightProxies.Clear();

				// All current selected cells
				foreach (HeightCell hc in heightCells) {
					PaintEditorProxy prx = new PaintEditorProxy(scene, hc.Block, hc.Ceiling ? World.Map.Side.Bottom : World.Map.Side.Top, hc.Ceiling);
					prx.Block.Position = hc.Pos;
					heightProxies.Add(prx);
				}

				// Current cell
				if (currentHeightCell != null) {
					PaintEditorProxy prx = new PaintEditorProxy(scene, currentHeightCell.Block, currentHeightCell.Ceiling ? World.Map.Side.Bottom : World.Map.Side.Top, currentHeightCell.Ceiling);
					prx.Block.Position = currentHeightCell.Pos;
					heightProxies.Add(prx);
				}
			}

			// Wobbling colors for proxies
			foreach (PaintEditorProxy prx in heightProxies) {
				prx.Mesh.Diffuse = System.Drawing.Color.FromArgb((int)((System.Math.Sin((wobble + prx.WobbleOffset) * (System.Math.PI * 2)) * 0.5 + 0.5) * 64) + 32, NSTheme.UI_ACCENT);
			}

			// Block gizmos
			for (int i = 0; i < heightGizmos.Length; i++) {
				heightGizmoAlpha[i] = Math.Max(Math.Min(heightGizmoAlpha[i] + ((currentHeightGizmo == i) ? 0.2f : -0.2f), 1f), 0f);
				Entity gizmo = heightGizmos[i];
				gizmo.Visible = heightCells.Count > 0;
				if (heightChanging) {
					gizmo.Visible = currentHeightGizmo == i;
				}

				WireCubeComponent cube = gizmo.GetComponent<WireCubeComponent>();
				cube.Size = Vector3.One * (0.1f + heightGizmoAlpha[i] * 0.05f);
				cube.WireColor = System.Drawing.Color.FromArgb((int)((System.Math.Sin(wobble * System.Math.PI * 2) * 0.5 + 0.5) * 64f + 63f * heightGizmoAlpha[i]) + 128, NSTheme.UI_ACCENT);
			}
		}

		/// <summary>
		/// Rebuilding bounds
		/// </summary>
		void HeightmapCalculateGizmos() {

			// Building positions
			Vector2 max = new Vector2(float.MinValue, float.MinValue);
			Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
			foreach (HeightCell hc in heightCells) {
				Vector2 ps = hc.Pos.Xz;
				if (ps.X < min.X) {
					min.X = ps.X;
				}
				if (ps.X + 1 > max.X) {
					max.X = ps.X + 1;
				}
				if (ps.Y < min.Y) {
					min.Y = ps.Y;
				}
				if (ps.Y + 1 > max.Y) {
					max.Y = ps.Y + 1;
				}
			}
			Vector2 half = (max - min) / 2f;
			heightBoundsMin = min;
			heightBoundsMax = max;
			
			// Building grid and positioning gizmos
			World.Map.FloorBlock[,] blockGrid = new World.Map.FloorBlock[(int)(max - min).Y, (int)(max - min).X];
			foreach (HeightCell hc in heightCells) {
				blockGrid[(int)(hc.Pos.Z - min.Y), (int)(hc.Pos.X - min.X)] = hc.Block as World.Map.FloorBlock;
			}
			float[] heights = new float[9];
			for (int i = 0; i < heights.Length; i++) {
				int dx = i % 3;
				int dy = 2 - (i - dx) / 3;
				float px = dx * half.X;
				float py = dy * half.Y;
				int cx = Math.Min((int)Math.Floor(px), (int)(max - min).X - 1);
				int cy = Math.Min((int)Math.Floor(py), (int)(max - min).Y - 1);
				float gx = px - cx;
				float gy = py - cy;
				World.Map.FloorBlock fb = blockGrid[cy, cx];
				if (fb != null) {
					float[] hmap;
					if (heightCells[0].Ceiling) {
						hmap = new float[]{
							fb.CeilingHeight[1], fb.CeilingHeight[0],
							fb.CeilingHeight[3], fb.CeilingHeight[2]
						};
					} else {
						hmap = new float[]{
							fb.FloorHeight[0], fb.FloorHeight[1],
							fb.FloorHeight[2], fb.FloorHeight[3]
						};
					}
					float h1 = Maths.Lerps.Lerp(hmap[0], hmap[1], gx);
					float h2 = Maths.Lerps.Lerp(hmap[2], hmap[3], gx);
					heights[i] = Maths.Lerps.Lerp(h1, h2, gy);
				}
				float div = 1f / HEIGHT_LEVELS;
				heights[i] = (float)Math.Ceiling(heights[i] / div) * div;

				if (heightCells[0].Ceiling) {
					heights[i] = heightCells[0].Pos.Y + 1f - heights[i];
				} else {
					heights[i] = heightCells[0].Pos.Y + heights[i];
				}
			}
			
			// Positioning
			for (int i = 0; i < 9; i++) {
				int dx = i % 3;
				int dy = 2 - (i - dx) / 3;
				heightGizmos[i].Position = new Vector3(dx * half.X + min.X, heights[i], dy * half.Y + min.Y);
			}
		}

		/// <summary>
		/// Caching cells
		/// </summary>
		/// <param name="ceil">Use ceiling</param>
		void HeightCacheCells(bool ceil) {
			heightPointsCache = new float[heightCells.Count * 4];
			for (int i = 0; i < heightCells.Count; i++) {
				World.Map.FloorBlock b = heightCells[i].Block as World.Map.FloorBlock;
				for (int j = 0; j < 4; j++) {
					heightPointsCache[j + i * 4] = ceil ? b.CeilingHeight[j] : b.FloorHeight[j];
				}
			}
		}

		/// <summary>
		/// Modifying cells
		/// </summary>
		/// <param name="delta">Amount of change</param>
		/// <param name="mask">Mask</param>
		void HeightModify(float delta, int mask) {
			bool ceil = heightCells[0].Ceiling;
			float div = 1f / (float)HEIGHT_LEVELS;
			float[] msk = heightGizmoWeights[mask];
			for (int hi = 0; hi < heightCells.Count; hi++) {
				HeightCell hc = heightCells[hi];
				World.Map.FloorBlock fb = hc.Block as World.Map.FloorBlock;
				float[] hmap = new float[4];
				for (int i = 0; i < 4; i++) {
					float h = heightPointsCache[i + hi * 4];
					int px = i % 2;
					int py = (i - px) / 2;
					if (ceil) {
						px = 1 - px;
					}
					Vector2 loc = new Vector2(px + hc.Pos.X - heightBoundsMin.X, py + hc.Pos.Z - heightBoundsMin.Y);
					loc.X /= heightBoundsMax.X - heightBoundsMin.X;
					loc.Y /= heightBoundsMax.Y - heightBoundsMin.Y;
					float w1 = Maths.Lerps.Lerp(msk[0], msk[1], loc.X);
					float w2 = Maths.Lerps.Lerp(msk[2], msk[3], loc.X);
					float dg = Maths.Lerps.Lerp(w1, w2, loc.Y);
					h += delta * dg;
					h = (float)Math.Round(h / div) * div;
					hmap[i] = Math.Max(Math.Min(h, 1), 0);
				}
				if (ceil) {
					fb.CeilingHeight = hmap;
				} else {
					fb.FloorHeight = hmap;
				}
				map.SetBlockAtCoords((int)hc.Pos.X, (int)hc.Pos.Y, (int)hc.Pos.Z, fb);
			}
		}

		/// <summary>
		/// Cells to paint
		/// </summary>
		class HeightCell {
			public Vector3 Pos;
			public World.Map.Block Block;
			public bool Ceiling;
		}

	}
}
