using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Data.Editor.Attributes;
using Cubed.Data.Files;
using Cubed.Data.Projects;
using Cubed.Editing;
using Cubed.Formats;
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


			Chunk chunk = Read();
			if (chunk != null) {
				MapReader.MapData data = MapReader.Read(chunk);
				if (data != null) {

					// Cleaning texture cache
					cachedTextures.Clear();
					textureAnimators.Clear();

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
						eo.Deselect(scene);
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
					cam.Position = data.CameraPos;
					cam.Angles = data.CameraAngle;
					gridHeight = data.GridHeight;

				}
			}
		}

		/// <summary>
		/// Saving file
		/// </summary>
		public override void Save() {

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
			Chunk chunk = MapReader.Write(data);
			if (chunk != null) {
				Write(chunk);
			}
			Saved = true;
		}

		/// <summary>
		/// Handling changes in project tree
		/// </summary>
		void Project_EntriesChangedEvent(object sender, Project.MultipleEntryEventArgs e) {
			engine.MakeCurrent();
			foreach (Project.EntryEventArgs ee in e.Events) {
				
				// Handling map changes
				if (cachedTextures.ContainsKey(ee.Entry as Project.Entry)) {
					if (ee.Type == Project.EntryEvent.Modified) {
						cachedTextures[ee.Entry as Project.Entry].Reload();
					} else if(ee.Type == Project.EntryEvent.Deleted) {
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
							} else if(ee.Type == Project.EntryEvent.Deleted) {
								environment.Sky[side] = null;
							}
						}
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
		
	}
}
