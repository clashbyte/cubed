using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Data.Editor;
using Cubed.Data.Editor.Attributes;
using Cubed.Data.Files;
using Cubed.Data.Projects;
using Cubed.Editing;
using Cubed.Editing.Gizmos;
using Cubed.Formats;
using Cubed.Forms.Common;
using Cubed.Prefabs;

namespace Cubed.Forms.Editors.Map {
	partial class MapEditor {

		/// <summary>
		/// Reading file
		/// </summary>
		public override void Load() {
			engine.MakeCurrent();
			engine_UpdateLogic(null, null);
			display.Render(screen.Size);
			engine.MakeCurrent();

			LoadFromChunk(Read(), true);
		}

		/// <summary>
		/// Saving file
		/// </summary>
		public override void Save() {
			Chunk chunk = SaveToChunk();
			if (chunk != null) {
				Write(chunk);
			}
			Saved = true;
		}

		/// <summary>
		/// Saving file to chunk
		/// </summary>
		/// <returns>Chunk</returns>
		protected override Chunk GetHistoryItem() {
			return SaveToChunk();
		}

		/// <summary>
		/// Reading from history
		/// </summary>
		/// <param name="chunk">Chunk to read from</param>
		protected override void RestoreHistoryItem(Chunk chunk) {
			engine.MakeCurrent();
			engine_UpdateLogic(null, null);
			display.Render(screen.Size);
			engine.MakeCurrent();

			LoadFromChunk(chunk, false);
			SelectEntity(null);
		}

		/// <summary>
		/// Loading data from chunk
		/// </summary>
		void LoadFromChunk(Chunk chunk, bool affectCamera) {
			if (chunk != null) {
				MapReader.MapData data = MapReader.Read(chunk);
				if (data != null) {

					// Cleaning texture cache
					cachedTextures.Clear();
					textureAnimators.Clear();
					InspectingObject = null;

					// Reading map
					if (data.Map != null) {
						foreach (World.Map.Chunk ch in data.Map.GetAllChunks()) {
							for (int y = 0; y < World.Map.Chunk.BLOCKS; y++) {
								for (int x = 0; x < World.Map.Chunk.BLOCKS; x++) {
									World.Map.Block b = ch[x, y];
									if (b is World.Map.WallBlock) {
										World.Map.WallBlock wb = b as World.Map.WallBlock;
										for (int i = 0; i < 6; i++) {
											World.Map.Side side = (World.Map.Side)i;
											wb[side] = LoadingCheckTexture(wb[side], emptyWallTex);
										}
									} else if (b is World.Map.FloorBlock) {
										World.Map.FloorBlock fb = b as World.Map.FloorBlock;
										fb.Floor = LoadingCheckTexture(fb.Floor, emptyFloorTex);
										fb.Ceiling = LoadingCheckTexture(fb.Ceiling, emptyCeilTex);
										for (int i = 0; i < 4; i++) {
											World.Map.Side side = (World.Map.Side)i;
											fb.FloorTrim[side] = LoadingCheckTexture(fb.FloorTrim[side], emptyFloorTex);
											fb.CeilingTrim[side] = LoadingCheckTexture(fb.CeilingTrim[side], emptyCeilTex);
										}
									}
								}
							}
						}
						map = data.Map;
						map.Ambient = data.Ambient;
						scene.Map = map;
					}

					// Reading entities
					foreach (EditableObject eo in sceneSelectedObjects) {
						Gizmo[] gizmos = eo.ControlGizmos;
						eo.Deselect(scene); 
						foreach (Gizmo gz in eo.ControlGizmos) {
							gz.Unassign(scene);
						}
					}
					foreach (EditableObject eo in sceneObjects) {
						eo.Destroy(scene);
					}
					List<EditableObject> entities = new List<EditableObject>();
					if (data.Entities != null) {
						foreach (GamePrefab gp in data.Entities) {
							Type t = TargetPrefabAttribute.GetEditableObject(gp.GetType());
							if (t != null) {
								EditableObject eo = Activator.CreateInstance(t) as EditableObject;
								eo.SetPrefab(gp);
								eo.Create(scene);
								entities.Add(eo);
							}
						}
					}
					sceneObjects = entities;
					sceneSelectedObjects.Clear();

					// Rewriting ambient data
					environment.Ambient = data.Ambient;
					for (int i = 0; i < 6; i++) {
						environment.Sky[(World.Skybox.Side)i] = data.Sky[(World.Skybox.Side)i];
					}
					environment.FogEnabled = data.FogEnabled;
					environment.FogData.Color = data.Fog.Color;
					environment.FogData.Near = data.Fog.Near;
					environment.FogData.Far = data.Fog.Far;

					// Reading editor parameters
					if (affectCamera) {
						cam.Position = data.CameraPos;
						cam.Angles = data.CameraAngle;
						gridHeight = data.GridHeight;
					}

				}
			}
		}

		/// <summary>
		/// Saving data to chunk
		/// </summary>
		/// <returns>Saved chunk</returns>
		Chunk SaveToChunk() {

			// Creating file
			MapReader.MapData data = new MapReader.MapData();
			data.Map = map;
			data.Sky = environment.Sky;
			data.FogEnabled = environment.FogEnabled;
			data.Fog = environment.FogData;
			data.Ambient = environment.Ambient;
			data.EditorFlags = 0;
			data.CameraPos = cam.Position;
			data.CameraAngle = cam.Angles;
			data.GridHeight = (int)gridHeight;

			// Saving entities
			List<GamePrefab> prefabs = new List<GamePrefab>();
			foreach (EditableObject eo in sceneObjects) {
				prefabs.Add(eo.Prefab);
			}
			data.Entities = prefabs.ToArray();

			// Writing data
			return MapReader.Write(data);
		}

		/// <summary>
		/// Handling changes in project tree
		/// </summary>
		void Project_EntriesChangedEvent(object sender, Project.MultipleEntryEventArgs e) {
			engine.MakeCurrent();
			foreach (Project.EntryEventArgs ee in e.Events) {
				
				// Handling map changes
				if (ee.Entry is Project.Entry) {
					bool changes = false;

					if (cachedTextures.ContainsKey(ee.Entry as Project.Entry)) {
						if (ee.Type == Project.EntryEvent.Modified) {
							cachedTextures[ee.Entry as Project.Entry].Reload();
						} else if (ee.Type == Project.EntryEvent.Deleted) {
							Graphics.Texture tx = cachedTextures[ee.Entry as Project.Entry];
							foreach (World.Map.Chunk ch in map.GetAllChunks()) {
								for (int y = 0; y < World.Map.Chunk.BLOCKS; y++) {
									for (int x = 0; x < World.Map.Chunk.BLOCKS; x++) {
										World.Map.Block b = ch[x, y];
										bool needRewrite = true;
										if (b is World.Map.WallBlock) {
											World.Map.WallBlock wb = b as World.Map.WallBlock;
											for (int i = 0; i < 6; i++) {
												World.Map.Side side = (World.Map.Side)i;
												if (wb[side] == tx) {
													wb[side] = emptyWallTex;
													needRewrite = true;
												}
											}
										} else if (b is World.Map.FloorBlock) {
											World.Map.FloorBlock fb = b as World.Map.FloorBlock;
											if (fb.Floor == tx) {
												fb.Floor = emptyFloorTex;
												needRewrite = true;
											}
											if (fb.Ceiling == tx) {
												fb.Ceiling = emptyCeilTex;
												needRewrite = true;
											}
											for (int i = 0; i < 4; i++) {
												World.Map.Side side = (World.Map.Side)i;
												if (fb.FloorTrim[side] == tx) {
													fb.FloorTrim[side] = emptyFloorTex;
													needRewrite = true;
												}
												if (fb.CeilingTrim[side] == tx) {
													fb.CeilingTrim[side] = emptyCeilTex;
													needRewrite = true;
												}
											}
										}
										if (needRewrite) {
											map.SetBlockAtCoords((int)ch.Location.X * World.Map.Chunk.BLOCKS + x, (int)ch.Location.Y, (int)ch.Location.Z * World.Map.Chunk.BLOCKS + y, b);
											changes = true;
										}
									}
								}
							}
						}
					}

					if (environment.Sky != null) {
						for (int i = 0; i < 6; i++) {
							World.Skybox.Side side = (World.Skybox.Side)i;
							if (environment.Sky[side] != null && environment.Sky[side].Link == ee.Entry.Path) {
								if (ee.Type == Project.EntryEvent.Modified) {
									environment.Sky[side].Reload();
									break;
								} else if (ee.Type == Project.EntryEvent.Deleted) {
									environment.Sky[side] = null;
								}
								changes = true;
							}
						}
					}

					if (changes) {
						Saved = false;
					}
				}
				
			}
		}

		/// <summary>
		/// Checking texture
		/// </summary>
		/// <returns>Texture or null</returns>
		Graphics.Texture LoadingCheckTexture(Graphics.Texture tex, Graphics.Texture replacement) {
			if (tex == null) {
				return replacement;
			}
			if (!cachedTextures.ContainsValue(tex)) {
				Project.Entry pe = Project.GetFile(tex.Link) as Project.Entry;
				if (pe != null) {
					if (cachedTextures.ContainsKey(pe)) {
						return cachedTextures[pe];
					}
					cachedTextures.Add(pe, tex);
				} else {
					return replacement;
				}
			}
			return tex;
		}

		/// <summary>
		/// Flag for undo permit
		/// </summary>
		public override bool CanUndo {
			get {
				if (!allowMouseLook || walkModeEnable.Checked) {
					return false;
				}
				return base.CanUndo;
			}
		}

		/// <summary>
		/// Flag for redo permit
		/// </summary>
		public override bool CanRedo {
			get {
				if (!allowMouseLook || walkModeEnable.Checked) {
					return false;
				}
				return base.CanRedo;
			}
		}

		/// <summary>
		/// Flag for copying and cutting
		/// </summary>
		public override bool CanCopyOrCut {
			get {
				if (!allowMouseLook || walkModeEnable.Checked) {
					return false;
				}
				if (currentTool != ToolType.Select && currentTool != ToolType.Logics) {
					return false;
				}
				if (sceneSelectedObjects.Count == 0) {
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Flag for pasting
		/// </summary>
		public override bool CanPaste {
			get {
				if (!allowMouseLook || walkModeEnable.Checked) {
					return false;
				}
				if (!ClipboardContent.HasChunk()) {
					return false;
				}
				Chunk ch = ClipboardContent.Read();
				if (ch != null){
					if (ch.ID == "GPRF") {
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Flag for selection
		/// </summary>
		public override bool CanSelectAll {
			get {
				if (!allowMouseLook || walkModeEnable.Checked) {
					return false;
				}
				if (currentTool != ToolType.Select && currentTool != ToolType.Logics) {
					return false;
				}
				return sceneObjects.Count > 0;
			}
		}

		/// <summary>
		/// Copying stuff
		/// </summary>
		public override void Copy(bool cut) {
			if (!CanCopyOrCut) {
				return;
			}

			// Saving objects to chunk
			List<GamePrefab> prefabs = new List<GamePrefab>();
			foreach (EditableObject eo in sceneSelectedObjects) {
				prefabs.Add(eo.Prefab);
			}
			Chunk[] chunks = GamePrefab.ToChunkArray(prefabs.ToArray());

			// Creating main chunk
			ContainerChunk cont = new ContainerChunk();
			cont.ID = "GPRF";
			cont.Children.AddRange(chunks);
			ClipboardContent.Write(cont);

			// Removing if cut
			if (cut) {
				TriggerChanges();
				foreach (EditableObject reo in sceneSelectedObjects) {
					reo.Deselect(scene);
					foreach (Gizmo g in reo.ControlGizmos) {
						g.Unassign(scene);
					}
					reo.Destroy(scene);
					sceneObjects.Remove(reo);
				}
				sceneSelectedObjects.Clear();
				InspectingObject = null;
				makePrefabButton.Enabled = false;
				makePrefabButton.Invalidate();
				MainForm.UpdateEditingMenu();
			}
		}

		/// <summary>
		/// Pasting stuff
		/// </summary>
		public override void Paste() {
			if (!CanPaste) {
				return;
			}
			if (currentTool != ToolType.Select && currentTool != ToolType.Logics) {
				toolSelect.Checked = true;
			}
			engine.MakeCurrent();
			engine_UpdateLogic(null, null);
			display.Render(screen.Size);
			engine.MakeCurrent();

			// Restoring objects
			ContainerChunk cont = (ContainerChunk)ClipboardContent.Read();
			GamePrefab[] prefabs = GamePrefab.FromChunkArray(cont.Children.ToArray());
			List<EditableObject> objs = new List<EditableObject>();
			foreach (GamePrefab pref in prefabs) {
				Type t = TargetPrefabAttribute.GetEditableObject(pref.GetType());
				if (t != null) {
					EditableObject eo = Activator.CreateInstance(t) as EditableObject;
					eo.SetPrefab(pref);
					eo.Create(scene);
					objs.Add(eo);
				}
			}

			// Adding to scene and selecting
			sceneObjects.AddRange(objs);
			SelectEntities(objs.ToArray());
		}

		/// <summary>
		/// Selecting all objects
		/// </summary>
		public override void SelectAll() {
			if (!CanSelectAll) {
				return;
			}
			SelectEntities(sceneObjects.ToArray());
		}
		
	}
}
