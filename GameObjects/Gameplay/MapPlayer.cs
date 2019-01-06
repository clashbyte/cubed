using Cubed.Data.Files;
using Cubed.Drivers.Rendering;
using Cubed.Formats;
using Cubed.Graphics;
using Cubed.Prefabs;
using Cubed.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Gameplay {

	/// <summary>
	/// Class for map playing
	/// </summary>
	public class MapPlayer {

		/// <summary>
		/// Parental game
		/// </summary>
		public Game Parent {
			get;
			private set;
		}

		/// <summary>
		/// Flag for skip key waiting at loading
		/// </summary>
		public bool ImmediatePlay {
			get;
			set;
		}

		/// <summary>
		/// Main game player
		/// </summary>
		public Hero Player {
			get;
			private set;
		}

		/// <summary>
		/// Game scene
		/// </summary>
		public Scene Scene {
			get;
			private set;
		}

		/// <summary>
		/// Internal map
		/// </summary>
		Map map;

		/// <summary>
		/// All awaiting textures
		/// </summary>
		Texture[] textures;

		/// <summary>
		/// All map prefabs
		/// </summary>
		GamePrefab[] prefabs;

		/// <summary>
		/// Map player
		/// </summary>
		/// <param name="parent">Parental game</param>
		/// <param name="data">Raw map data</param>
		/// <param name="saveGame">Saved state data</param>
		public MapPlayer(Game parent, byte[] data, byte[] saveGame) {
			Parent = parent;

			// Creating scene
			Scene = new Scene();

			// Starting loading
			MapReader.MapData mapData = MapReader.Read(ChunkedFile.ReadFromBytes(data));
			map = mapData.Map;

			// Scanning textures
			List<GamePrefab> prefabList = new List<GamePrefab>();
			List<Texture> texturesToLoad = new List<Texture>();
			foreach (World.Map.Chunk ch in map.GetAllChunks()) {
				for (int y = 0; y < World.Map.Chunk.BLOCKS; y++) {
					for (int x = 0; x < World.Map.Chunk.BLOCKS; x++) {
						World.Map.Block b = ch[x, y];
						if (b is World.Map.WallBlock) {
							World.Map.WallBlock wb = b as World.Map.WallBlock;
							for (int i = 0; i < 6; i++) {
								World.Map.Side side = (World.Map.Side)i;
								WatchTexture(wb[side], texturesToLoad);
							}
						} else if (b is World.Map.FloorBlock) {
							World.Map.FloorBlock fb = b as World.Map.FloorBlock;
							WatchTexture(fb.Floor, texturesToLoad);
							WatchTexture(fb.Ceiling, texturesToLoad);
							for (int i = 0; i < 4; i++) {
								World.Map.Side side = (World.Map.Side)i;
								WatchTexture(fb.FloorTrim[side], texturesToLoad);
								WatchTexture(fb.CeilingTrim[side], texturesToLoad);
							}
						}
					}
				}
			}
			
			// Setting up map ambience
			map.Ambient = mapData.Ambient;
			Scene.Sky = mapData.Sky;
			Scene.Map = map;
			if (mapData.FogEnabled) {
				Scene.Fog = mapData.Fog;
			}
			if (Scene.Sky != null) {
				for (int i = 0; i < 6; i++) {
					Texture st = Scene.Sky[(Skybox.Side)i];
					if (st != null) {
						WatchTexture(st, texturesToLoad);
						st.Filtering = Texture.FilterMode.Enabled;
						st.WrapHorizontal = Texture.WrapMode.Clamp;
						st.WrapVertical = Texture.WrapMode.Clamp;
					}
				}
			}

			// Making player and lights
			bool playerPosFound = false;
			Player = new Hero(this);
			foreach (GamePrefab prefab in mapData.Entities) {
				if (prefab is PlayerStart) {
					PlayerStart ps = prefab as PlayerStart;
					Player.Spawn(ps.Position, ps.Angle);
					playerPosFound = true;
				}
				prefabList.Add(prefab);
				if (prefab is MapLight) {
					prefab.Assign(Scene);
				}
			}
			if (!playerPosFound) {
				Player.Spectate(mapData.CameraPos, mapData.CameraAngle);
			}
			Scene.Camera = Player.Camera;

			// Storing lists
			textures = texturesToLoad.ToArray();

			prefabs = prefabList.ToArray();

			// Assigning map to scene
			map.LazyRebuilding = true;
			Parent.Engine.World = Scene;

			// Switching state
			parent.State = Game.PlayerState.LoadingMap;
		}

		/// <summary>
		/// Enabling scene
		/// </summary>
		public void Show() {
			map.LazyRebuilding = false;
			Parent.Engine.World = Scene;
			Parent.Engine.Interface = null;
		}

		/// <summary>
		/// Updating loading process
		/// </summary>
		public void UpdateLoading() {
			bool ready = true;
			
			// Textures
			if (textures != null) {
				foreach (Texture tex in textures) {
					if (tex.State != Texture.LoadingState.Complete) {
						ready = false;
						break;
					}
				}
			}

			// Updating lights
			if (ready) {
				foreach (GamePrefab prefab in prefabs) {
					if (!prefab.Ready) {
						ready = false;
						break;
					}
				}
			}

			// Updating map
			if (ready) {
				ready = map.IsReady();
			}

			// Flagging ready state
			Parent.Loading.IsReady = ready;
			if (ready && (Input.Controls.AnyKeyHit() || ImmediatePlay)) {

				// Creating entities
				foreach (GamePrefab prefab in prefabs) {
					if (!(prefab is MapLight)) {
						prefab.Assign(Scene);
					}
				}
				
				// Jumping to game
				Parent.State = Game.PlayerState.Gameplay;
			}
		}

		/// <summary>
		/// Updating gameplay
		/// </summary>
		public void UpdateGame() {
			Display.Current.MouseLock = true;
			Player.Update();
		}

		/// <summary>
		/// Removing all the stuff
		/// </summary>
		public void Cleanup() {
			if (map != null) {
				
			}
			foreach (GamePrefab prefab in prefabs) {
				prefab.Unassign(Scene);
				prefab.Destroy();
			}
		}

		/// <summary>
		/// Adding texture to watch list
		/// </summary>
		/// <param name="suspect">Suspected texture</param>
		/// <param name="texs">Texture list</param>
		void WatchTexture(Texture suspect, List<Texture> texs) {
			if (suspect != null) {
				if (!texs.Contains(suspect)) {
					texs.Add(suspect);
				}
			}
		}
	}
}
