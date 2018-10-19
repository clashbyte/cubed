using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Components.Rendering;
using Cubed.Graphics;
using Cubed.Maths;
using Cubed.UI;
using Cubed.UI.Basic;
using Cubed.UI.Controls;
using Cubed.World;
using OpenTK;
using OpenTK.Input;

namespace Cubed.Forms.Editors.Map {
	partial class MapEditor {

		/// <summary>
		/// Painting proxies
		/// </summary>
		List<PaintEditorProxy> paintProxies;

		/// <summary>
		/// User interface for paint
		/// </summary>
		UserInterface paintInterface;

		/// <summary>
		/// Info label
		/// </summary>
		Label infoLabel;

		/// <summary>
		/// Current paint item
		/// </summary>
		PaintItem currentPaintItem;

		/// <summary>
		/// Filling paint
		/// </summary>
		bool paintFillMode;

		/// <summary>
		/// Drawing data
		/// </summary>
		bool paintDrawing;

		/// <summary>
		/// Opening paint tool
		/// </summary>
		void PaintToolOpen() {
			if (paintProxies == null) {
				paintProxies = new List<PaintEditorProxy>();
			}
			if (paintInterface == null) {
				paintInterface = new UserInterface();
				paintInterface.Items.Add(infoLabel = new Label() {
					Anchor = Control.AnchorMode.TopLeft,
					HorizontalAlign = UserInterface.Align.Start,
					VerticalAlign = UserInterface.Align.Start,
					Text = ""
				});
			}
			engine.Interface = paintInterface;
			paintProxies.Clear();
			allowMouseLook = true;
			screen.Cursor = System.Windows.Forms.Cursors.Cross;
		}

		/// <summary>
		/// Closing paint tool
		/// </summary>
		void PaintToolClose() {
			foreach (PaintEditorProxy prx in paintProxies) {
				prx.Kill();
			}
			engine.Interface = null;
			allowMouseLook = true;
			screen.Cursor = System.Windows.Forms.Cursors.Default;
		}

		// Updating paint tool logics
		void PaintToolUpdate() {
			// Hiding all proxies
			bool needRebuild = false;
			if (Input.Controls.MouseHit(MouseButton.Middle) && allowMouseLook) {
				foreach (PaintEditorProxy prx in paintProxies) {
					prx.Block.Visible = false;
				}
			} else if (Input.Controls.MouseReleased(MouseButton.Middle) && allowMouseLook) {
				foreach (PaintEditorProxy prx in paintProxies) {
					prx.Block.Visible = true;
					needRebuild = true;
				}
			}

			// Picking current grid position
			Vector3 camPos = cam.ScreenToPoint(Input.Controls.Mouse.X, Input.Controls.Mouse.Y, 0);
			Vector3 camDir = cam.ScreenToPoint(Input.Controls.Mouse.X, Input.Controls.Mouse.Y, 1) - camPos;
			camDir.Normalize();
			Vector3 pickPos = Vector3.Zero;
			MapIntersections.Hit hit = MapIntersections.Intersect(camPos, camDir, map);
			if (hit != null && GetCurrentTexture(null) != null) {
				PaintItem pi = new PaintItem() {
					Coords = hit.Cell,
					Side = hit.Side,
					UseCeiling = hit.Type == MapIntersections.HitType.Ceiling || hit.Type == MapIntersections.HitType.CeilingTrim,
					Block = hit.Block
				};
				if (currentPaintItem != null) {
					if (pi.Coords != currentPaintItem.Coords || pi.Side != currentPaintItem.Side || pi.UseCeiling != currentPaintItem.UseCeiling) {
						needRebuild = true;
						currentPaintItem = pi;
						if (paintDrawing) {
							PaintToolHandle(CalculatePaintItems(currentPaintItem, paintFillMode), GetCurrentTexture(null));
						}
					}
				} else {
					currentPaintItem = pi;
					needRebuild = true;
					if (paintDrawing) {
						PaintToolHandle(CalculatePaintItems(currentPaintItem, paintFillMode), GetCurrentTexture(null));
					}
				}
			} else {
				if (currentPaintItem != null) {
					needRebuild = true;
				}
				currentPaintItem = null;
			}

			// Modifiers
			paintFillMode = Input.Controls.KeyDown(Key.ShiftLeft);
			if (Input.Controls.KeyHit(Key.ShiftLeft) || Input.Controls.KeyReleased(Key.ShiftLeft)) {
				if (paintDrawing) {
					PaintToolHandle(CalculatePaintItems(currentPaintItem, paintFillMode), GetCurrentTexture(null));
				}
				needRebuild = true;
			}
			if (Input.Controls.MouseHit(MouseButton.Left)) {
				TriggerChanges();
				paintDrawing = true;
				needRebuild = true;
				if (paintDrawing) {
					PaintToolHandle(CalculatePaintItems(currentPaintItem, paintFillMode), GetCurrentTexture(null));
				}
			} else if (Input.Controls.MouseReleased(MouseButton.Left)) {
				Cubed.Forms.Common.MainForm.UpdateEditingMenu();
				paintDrawing = false;
				needRebuild = true;
				if (paintDrawing) {
					PaintToolHandle(CalculatePaintItems(currentPaintItem, paintFillMode), GetCurrentTexture(null));
				}
			}
			allowMouseLook = !paintDrawing;

			// Rebuilding floors
			if (needRebuild && !display.MouseLock) {

				// Cleaning
				foreach (PaintEditorProxy prx in paintProxies) {
					prx.Kill();
				}
				paintProxies.Clear();

				// Items to render
				Texture tex = GetCurrentTexture(null);
				PaintItem[] items = CalculatePaintItems(currentPaintItem, paintFillMode);
				foreach (PaintItem item in items) {
					PaintEditorProxy p = new PaintEditorProxy(scene, map.GetBlockAtCoords(item.Coords.X, item.Coords.Y, item.Coords.Z), item.Side, item.UseCeiling);
					p.Block.Position = item.Coords;
					p.Mesh.Texture = tex;
					paintProxies.Add(p);
				}
			}

			// Wobbling colors for proxies
			foreach (PaintEditorProxy prx in paintProxies) {
				if (!paintDrawing) {
					prx.Mesh.Diffuse = System.Drawing.Color.FromArgb((int)((System.Math.Sin((wobble + prx.WobbleOffset) * (System.Math.PI * 2)) * 0.5 + 0.5) * 128) + 127, NSTheme.UI_ACCENT);
				} else {
					prx.Mesh.Diffuse = System.Drawing.Color.Transparent;
				}
			}
		}

		/// <summary>
		/// Filling selected items
		/// </summary>
		/// <param name="items">Array of items to fill</param>
		void PaintToolHandle(PaintItem[] items, Texture texture) {
			foreach (PaintItem item in items) {
				if (item.Block is World.Map.WallBlock) {
					World.Map.WallBlock wblock = item.Block as World.Map.WallBlock;
					wblock[item.Side] = texture;
				} else if(item.Block is World.Map.FloorBlock) {
					World.Map.FloorBlock fblock = item.Block as World.Map.FloorBlock;
					if (item.UseCeiling) {
						if (item.Side == World.Map.Side.Bottom) {
							fblock.Ceiling = texture;
						} else {
							fblock.CeilingTrim[item.Side] = texture; 
						}
					} else {
						if (item.Side == World.Map.Side.Top) {
							fblock.Floor = texture;
						} else {
							fblock.FloorTrim[item.Side] = texture;
						}
					}
				}
				map.SetBlockAtCoords((int)item.Coords.X, (int)item.Coords.Y, (int)item.Coords.Z, item.Block);
			}
		}

		/// <summary>
		/// Calculating items for painting
		/// </summary>
		/// <param name="root">Root paint item</param>
		/// <param name="fill">Filling</param>
		/// <returns>Array of drawing elements</returns>
		PaintItem[] CalculatePaintItems(PaintItem root, bool fill) {
			List<PaintItem> items = new List<PaintItem>();
			if (root != null) {
				items.Add(root);
				if (fill) {
					Texture searchTex = null;
					if (root.Block is World.Map.WallBlock) {
						searchTex = (root.Block as World.Map.WallBlock)[root.Side];
					} else if(root.Block is World.Map.FloorBlock) {
						if (root.Side == World.Map.Side.Top) {
							searchTex = (root.Block as World.Map.FloorBlock).Floor;
						} else if (root.Side == World.Map.Side.Bottom) {
							searchTex = (root.Block as World.Map.FloorBlock).Ceiling;
						} else {
							if (root.UseCeiling) {
								searchTex = (root.Block as World.Map.FloorBlock).CeilingTrim[root.Side];
							} else {
								searchTex = (root.Block as World.Map.FloorBlock).FloorTrim[root.Side];
							}
						}
					}
					List<string> hashes = new List<string>();
					items.AddRange(CalculatePaintSiblings(root.Coords, root.Side, root.UseCeiling, searchTex, hashes, true));
				}
			}
			return items.ToArray();
		}

		/// <summary>
		/// Calculate siblings to paint
		/// </summary>
		/// <param name="pos">Position to start</param>
		/// <param name="side">Side to paint</param>
		/// <param name="ceilHint">Use ceiling part</param>
		/// <param name="protoTex">Texture to search</param>
		/// <returns>Array of items</returns>
		PaintItem[] CalculatePaintSiblings(Vector3 pos, World.Map.Side side, bool ceilHint, Texture protoTex, List<string> hashes, bool skipProcess = false) {
			List<PaintItem> result = new List<PaintItem>();
			string hash = GetPaintCellHash(pos, side, ceilHint);
			if (!hashes.Contains(hash)) {
				World.Map.Block block = map.GetBlockAtCoords(pos.X, pos.Y, pos.Z);
				
				// Checking current block
				if (!skipProcess) {
					if (block == null) {
						return result.ToArray();
					}
					if (block is World.Map.WallBlock) {
						// Wall
						if ((block as World.Map.WallBlock)[side] != protoTex) {
							return result.ToArray();
						}
					} else if(block is World.Map.FloorBlock) {
						// Floor block
						if (side == World.Map.Side.Top) {
							// Floor
							if ((block as World.Map.FloorBlock).Floor != protoTex) {
								return result.ToArray();
							}
						} else if(side == World.Map.Side.Bottom) {
							// Ceiling
							if ((block as World.Map.FloorBlock).Floor != protoTex) {
								return result.ToArray();
							}
						} else {
							// Trims
							if (ceilHint) {
								if ((block as World.Map.FloorBlock).CeilingTrim[side] != protoTex) {
									return result.ToArray();
								}
							} else {
								if ((block as World.Map.FloorBlock).FloorTrim[side] != protoTex) {
									return result.ToArray();
								}
							}
						}
					}

					// Adding this to output
					result.Add(new PaintItem() {
						Block = block,
						Coords = pos,
						Side = side,
						UseCeiling = ceilHint
					});
				}

				// Adding hash
				hashes.Add(hash);

				// Checking siblings
				if (block is World.Map.WallBlock) {
					if (side != World.Map.Side.Top && side != World.Map.Side.Bottom) {
						// Going up
						if (CheckPaintVerticalBlock(pos, side, true, protoTex, false, hashes)) {
							result.AddRange(CalculatePaintSiblings(pos + Vector3.UnitY, side, false, protoTex, hashes));
						}

						// Going down
						if (CheckPaintVerticalBlock(pos, side, false, protoTex, true, hashes)) {
							result.AddRange(CalculatePaintSiblings(pos - Vector3.UnitY, side, false, protoTex, hashes));
						}

						// Going sideways
						for (int i = 0; i < 2; i++) {
							Vector3 target = Vector3.Zero;
							Vector3 cross = Vector3.Zero;
							switch (side) {
								case Cubed.World.Map.Side.Forward:
									target.X += (i == 1) ? -1 : 1;
									break;
								case Cubed.World.Map.Side.Right:
									target.Z += (i == 1) ? 1 : -1;
									break;
								case Cubed.World.Map.Side.Back:
									target.X += (i == 1) ? 1 : -1;
									break;
								case Cubed.World.Map.Side.Left:
									target.Z += (i == 1) ? -1 : 1;
									break;
							}
							if (CheckPaintHorizontalBlock(pos, side, i == 1, protoTex, false, hashes)) {
								result.AddRange(CalculatePaintSiblings(pos + target, side, false, protoTex, hashes));
							}
							if (CheckPaintHorizontalBlock(pos, side, i == 1, protoTex, true, hashes)) {
								result.AddRange(CalculatePaintSiblings(pos + target, side, true, protoTex, hashes));
							}
						}
					}
				} else if (block is World.Map.FloorBlock) {
					World.Map.FloorBlock fb = block as World.Map.FloorBlock;
					if (side == World.Map.Side.Top || side == World.Map.Side.Bottom) {
						if (((side == World.Map.Side.Top) ? fb.HasFloor : fb.HasCeiling)) {
							Vector3[] offs = new Vector3[] {
								Vector3.UnitX, Vector3.UnitZ,
								-Vector3.UnitX, -Vector3.UnitZ
							};
							foreach (Vector3 off in offs) {
								if (CheckPaintFloorBlock(pos + off, protoTex, side == World.Map.Side.Bottom, hashes)) {
									result.AddRange(CalculatePaintSiblings(pos + off, side, side == World.Map.Side.Bottom, protoTex, hashes));
								}
							}
						}
					} else {

					}
				}

			}
			return result.ToArray();

		}

		/// <summary>
		/// Vertical block checking
		/// </summary>
		/// <returns></returns>
		bool CheckPaintVerticalBlock(Vector3 pos, World.Map.Side side, bool goUp, Texture tex, bool ceil, List<string> hashes) {
			string hash = GetPaintCellHash(pos + Vector3.UnitY * (goUp ? 1 : -1), side, ceil);
			World.Map.Block blk = map.GetBlockAtCoords(pos.X, pos.Y + (goUp ? 1 : -1), pos.Z);
			if (blk is World.Map.FloorBlock) {
				if (goUp != ceil) {
					return false;
				}
			}
			if (!hashes.Contains(hash)) {
				if (blk != null) {
					Vector3 dir = Vector3.Zero;

					// Checking side
					switch (side) {
						case Cubed.World.Map.Side.Forward:
							dir.Z = 1;
							break;
						case Cubed.World.Map.Side.Right:
							dir.X = 1;
							break;
						case Cubed.World.Map.Side.Back:
							dir.Z = -1;
							break;
						case Cubed.World.Map.Side.Left:
							dir.X = -1;
							break;
					}
					dir += pos;

					// Mid block
					World.Map.Block mblk = map.GetBlockAtCoords(dir.X, dir.Y, dir.Z);
					if (mblk != null) {
						if (mblk is World.Map.WallBlock) {
							return false;
						} else if (mblk is World.Map.FloorBlock) {
							World.Map.FloorBlock fmblk = mblk as World.Map.FloorBlock;
							if ((goUp ? fmblk.HasCeiling : fmblk.HasFloor)) {
								return false;
							}
						}
					}

					// Up/down block
					World.Map.Block oblk = map.GetBlockAtCoords(dir.X, dir.Y + (goUp ? 1 : -1), dir.Z);
					if (oblk != null) {
						if (oblk is World.Map.WallBlock) {
							return false;
						} else if (oblk is World.Map.FloorBlock) {
							World.Map.FloorBlock foblk = oblk as World.Map.FloorBlock;
							if ((goUp ? foblk.HasFloor : foblk.HasCeiling)) {
								return false;
							}
						}
					}
					
					// All ok - returning
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Checking right wall to paint
		/// </summary>
		/// <param name="pos">Position</param>
		/// <param name="side">Side</param>
		/// <param name="goRight">Go right or left</param>
		/// <param name="tex">Texture to search</param>
		/// <param name="ceil">Ceiling texture</param>
		/// <param name="hashes">Hashes array</param>
		/// <returns>True if can traverse there</returns>
		bool CheckPaintHorizontalBlock(Vector3 pos, World.Map.Side side, bool goRight, Texture tex, bool ceil, List<string> hashes) {
			
			// Computing direction
			Vector3 target = Vector3.Zero;
			Vector3 cross = Vector3.Zero;
			switch (side) {
				case Cubed.World.Map.Side.Forward:
					target.X += goRight ? -1 : 1;
					cross.Z += 1;
					break;
				case Cubed.World.Map.Side.Right:
					target.Z += goRight ? 1 : -1;
					cross.X += 1;
					break;
				case Cubed.World.Map.Side.Back:
					target.X += goRight ? 1 : -1;
					cross.Z -= 1;
					break;
				case Cubed.World.Map.Side.Left:
					target.Z += goRight ? -1 : 1;
					cross.X -= 1;
					break;
			}
			
			// Checking target
			string hash = GetPaintCellHash(pos + target, side, ceil);
			World.Map.Block blk = map.GetBlockAtCoords(pos.X + target.X, pos.Y + target.Y, pos.Z + target.Z);
			if (!hashes.Contains(hash)) {
				if (blk != null) {

					// Blocking section
					Vector3 dir = pos + target + cross;
					World.Map.Block tblk = map.GetBlockAtCoords(dir.X, dir.Y, dir.Z);
					if (tblk != null) {
						if (tblk is World.Map.WallBlock) {
							return false;
						}
					}

					// All ok - returning
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Checking right wall to paint
		/// </summary>
		/// <param name="pos">Position</param>
		/// <param name="side">Side</param>
		/// <param name="goRight">Go right or left</param>
		/// <param name="tex">Texture to search</param>
		/// <param name="ceil">Ceiling texture</param>
		/// <param name="hashes">Hashes array</param>
		/// <returns>True if can traverse there</returns>
		bool CheckPaintFloorBlock(Vector3 pos, Texture tex, bool ceil, List<string> hashes) {

			// Checking target
			string hash = GetPaintCellHash(pos, ceil ? World.Map.Side.Bottom : World.Map.Side.Top, ceil);
			World.Map.Block blk = map.GetBlockAtCoords(pos.X, pos.Y, pos.Z);
			if (!hashes.Contains(hash)) {
				if (blk != null) {

					// Wall
					if (blk is World.Map.WallBlock) {
						return false;
					}
					if (blk is World.Map.FloorBlock) {
						World.Map.FloorBlock fb = blk as World.Map.FloorBlock;
						if (!(ceil ? fb.HasCeiling : fb.HasFloor)) {
							return false;
						}
						if ((ceil ? fb.Ceiling : fb.Floor) != tex) {
							return false;	
						}
					}

					// All ok - returning
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Calculating hash for array
		/// </summary>
		/// <param name="pos">Location</param>
		/// <param name="side">Side</param>
		/// <param name="ceilHint">Ceiling hint</param>
		/// <returns>Hash for array</returns>
		string GetPaintCellHash(Vector3 pos, World.Map.Side side, bool ceilHint = false) {
			string hash = pos.ToString() + side.ToString();
			if (side != World.Map.Side.Top && side == World.Map.Side.Bottom) {
				hash += " " + (ceilHint ? "U" : "D");
			}
			return hash;
		}

		/// <summary>
		/// Drawing cell
		/// </summary>
		class PaintItem {
			public Vector3 Coords;
			public World.Map.Block Block;
			public World.Map.Side Side;
			public bool UseCeiling;
		}

		/// <summary>
		/// Struct for floor proxy
		/// </summary>
		class PaintEditorProxy {
			public Entity Block;
			public float WobbleOffset;
			public MeshComponent Mesh;
			Scene scene;
			public PaintEditorProxy(Scene s, World.Map.Block block, World.Map.Side side, bool useCeil = false) {
				Mesh = BuildMesh(block, side, useCeil);
				Block = new Entity();
				Block.Visible = true;
				Block.AddComponent(Mesh);
				s.Entities.Add(Block);
				scene = s;
			}
			public void Kill() {
				Block.Destroy();
				scene.Entities.Remove(Block);
			}

			/// <summary>
			/// Creating mesh for side
			/// </summary>
			/// <param name="block">Block</param>
			/// <param name="side">Block side</param>
			/// <param name="useCeil">Use ceiling</param>
			/// <returns>Mesh</returns>
			MeshComponent BuildMesh(World.Map.Block block, World.Map.Side side, bool useCeil = false) {
				float span = 0.002f;
				MeshComponent mc = new MeshComponent();
				mc.Unlit = true;

				if (block is World.Map.WallBlock) {
					// Wall block
					switch (side) {
						case Cubed.World.Map.Side.Forward:
							mc.Vertices = new Vector3[] {
								new Vector3(1, 1, 1f + span),
								new Vector3(0, 1, 1f + span),
								new Vector3(1, 0, 1f + span),
								new Vector3(0, 0, 1f + span)
							};
							break;
						case Cubed.World.Map.Side.Right:
							mc.Vertices = new Vector3[] {
								new Vector3(1f + span, 1, 0),
								new Vector3(1f + span, 1, 1),
								new Vector3(1f + span, 0, 0),
								new Vector3(1f + span, 0, 1)
							};
							break;
						case Cubed.World.Map.Side.Back:
							mc.Vertices = new Vector3[] {
								new Vector3(0, 1, 0f - span),
								new Vector3(1, 1, 0f - span),
								new Vector3(0, 0, 0f - span),
								new Vector3(1, 0, 0f - span)
							};
							break;
						case Cubed.World.Map.Side.Left:
							mc.Vertices = new Vector3[] {
								new Vector3(0f - span, 1, 1),
								new Vector3(0f - span, 1, 0),
								new Vector3(0f - span, 0, 1),
								new Vector3(0f - span, 0, 0)
							};
							break;
						case Cubed.World.Map.Side.Top:
							mc.Vertices = new Vector3[] {
								new Vector3(0, 1f + span, 1),
								new Vector3(1, 1f + span, 1),
								new Vector3(0, 1f + span, 0),
								new Vector3(1, 1f + span, 0)
							};
							break;
						case Cubed.World.Map.Side.Bottom:
							mc.Vertices = new Vector3[] {
								new Vector3(1, 0f - span, 1),
								new Vector3(0, 0f - span, 1),
								new Vector3(1, 0f - span, 0),
								new Vector3(0, 0f - span, 0)
							};
							break;
						default:
							break;
					}
					mc.TexCoords = new Vector2[] {
						new Vector2(0, 0),
						new Vector2(1, 0),
						new Vector2(0, 1),
						new Vector2(1, 1)
					};
					mc.Indices = new ushort[] {
						0, 1, 2,
						1, 3, 2
					};

				} else if (block is World.Map.FloorBlock) {
					World.Map.FloorBlock fblock = block as World.Map.FloorBlock;
					if (useCeil) {
						// Ceiling
						if (side == World.Map.Side.Bottom) {
							// Top sides
							mc.Vertices = new Vector3[] {
								new Vector3(1, 1f - fblock.CeilingHeight[0] - span, 0),
								new Vector3(0, 1f - fblock.CeilingHeight[1] - span, 0),
								new Vector3(1, 1f - fblock.CeilingHeight[2] - span, 1),
								new Vector3(0, 1f - fblock.CeilingHeight[3] - span, 1),
							};
							mc.TexCoords = new Vector2[] {
								new Vector2(0, 0),
								new Vector2(1, 0),
								new Vector2(0, 1),
								new Vector2(1, 1)
							};
							mc.Indices = new ushort[] {
								0, 2, 1,
								1, 2, 3
							};
						} else {
							// Other sides
							Vector3 vh1 = Vector3.Zero, vh2 = Vector3.Zero;
							switch (side) {
								case Cubed.World.Map.Side.Forward:
									vh1 = new Vector3(0, 1f - fblock.CeilingHeight[3], 1f + span);
									vh2 = new Vector3(1, 1f - fblock.CeilingHeight[2], 1f + span);
									break;
								case Cubed.World.Map.Side.Right:
									vh1 = new Vector3(1f + span, 1f - fblock.CeilingHeight[2], 1);
									vh2 = new Vector3(1f + span, 1f - fblock.CeilingHeight[0], 0);
									break;
								case Cubed.World.Map.Side.Back:
									vh1 = new Vector3(1, 1f - fblock.CeilingHeight[0], 0f - span);
									vh2 = new Vector3(0, 1f - fblock.CeilingHeight[1], 0f - span);
									break;
								case Cubed.World.Map.Side.Left:
									vh1 = new Vector3(0f - span, 1f - fblock.CeilingHeight[1], 0);
									vh2 = new Vector3(0f - span, 1f - fblock.CeilingHeight[3], 1);
									break;
							}

							// Making one or two triangles
							if (vh1.Y < 1 || vh2.Y < 1) {
								mc.Vertices = new Vector3[] {
									new Vector3(vh1.X, 1, vh1.Z),
									new Vector3(vh2.X, 1, vh2.Z),
									vh1, vh2
								};
								mc.TexCoords = new Vector2[] {
									new Vector2(1, 0),
									new Vector2(0, 0),
									new Vector2(1, 1f - vh1.Y),
									new Vector2(0, 1f - vh2.Y),
								};
								if (vh1.Y < 1 || vh2.Y < 1) {
									mc.Indices = new ushort[]{
										0, 2, 1,
										1, 2, 3
									};
								} else {
									mc.Indices = new ushort[]{
										0, 1,
										(vh2.Y < 1) ? (ushort)3 : (ushort)2
									};
								}
							}
						}
					} else {
						// Floor
						if (side == World.Map.Side.Top) {
							// Top sides
							mc.Vertices = new Vector3[] {
								new Vector3(0, fblock.FloorHeight[0] + span, 0),
								new Vector3(1, fblock.FloorHeight[1] + span, 0),
								new Vector3(0, fblock.FloorHeight[2] + span, 1),
								new Vector3(1, fblock.FloorHeight[3] + span, 1),
							};
							mc.TexCoords = new Vector2[] {
								new Vector2(0, 0),
								new Vector2(1, 0),
								new Vector2(0, 1),
								new Vector2(1, 1)
							};
							mc.Indices = new ushort[] {
								0, 2, 1,
								1, 2, 3
							};
						} else {
							// Other sides
							Vector3 vh1 = Vector3.Zero, vh2 = Vector3.Zero;
							switch (side) {
								case Cubed.World.Map.Side.Forward:
									vh1 = new Vector3(0, fblock.FloorHeight[2], 1f + span);
									vh2 = new Vector3(1, fblock.FloorHeight[3], 1f + span);
									break;
								case Cubed.World.Map.Side.Right:
									vh1 = new Vector3(1f + span, fblock.FloorHeight[3], 1);
									vh2 = new Vector3(1f + span, fblock.FloorHeight[1], 0);
									break;
								case Cubed.World.Map.Side.Back:
									vh1 = new Vector3(1, fblock.FloorHeight[1], 0f - span);
									vh2 = new Vector3(0, fblock.FloorHeight[0], 0f - span);
									break;
								case Cubed.World.Map.Side.Left:
									vh1 = new Vector3(0f - span, fblock.FloorHeight[0], 0);
									vh2 = new Vector3(0f - span, fblock.FloorHeight[2], 1);
									break;
							}

							// Making one or two triangles
							if (vh1.Y > 0 || vh2.Y > 0) {
								mc.Vertices = new Vector3[] {
									new Vector3(vh1.X, 0, vh1.Z),
									new Vector3(vh2.X, 0, vh2.Z),
									vh1, vh2
								};
								mc.TexCoords = new Vector2[] {
									new Vector2(1, 1),
									new Vector2(0, 1),
									new Vector2(1, 1f - vh1.Y),
									new Vector2(0, 1f - vh2.Y),
								};
								if (vh1.Y > 0 || vh2.Y > 0) {
									mc.Indices = new ushort[]{
										0, 1, 2,
										1, 3, 2
									};
								} else {
									mc.Indices = new ushort[]{
										1, 0,
										(vh2.Y > 0) ? (ushort)3 : (ushort)2
									};
								}
							}
						}
					}
				}
				return mc;
			}
		}

	}
}
