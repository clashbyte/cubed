using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Cubed.Core;
using Cubed.Data.Files;
using Cubed.Data.Game.Attributes;
using Cubed.Prefabs;
using OpenTK;

namespace Cubed.Formats {

	/// <summary>
	/// Map file reader/writer
	/// </summary>
	public static class MapReader {

		/// <summary>
		/// Writing map to chunk
		/// </summary>
		/// <param name="data">Map info</param>
		/// <returns>Data</returns>
		public static Chunk Write(MapData data) {

			// Root
			ContainerChunk mainChunk = new ContainerChunk() {
				ID = "SMAP",
				Version = 1
			};

			// Writing map data
			MemoryStream maps = new MemoryStream();
			BinaryWriter mapf = new BinaryWriter(maps);

			// Caching textures
			World.Map.Chunk[] chunks = data.Map.GetAllChunks();
			List<World.Map.Chunk> chunksToWrite = new List<World.Map.Chunk>();
			List<Graphics.Texture> texList = new List<Graphics.Texture>();
			texList.Add(null);
			foreach (World.Map.Chunk ch in chunks) {
				bool empty = false;
				for (int y = 0; y < World.Map.Chunk.BLOCKS; y++) {
					for (int x = 0; x < World.Map.Chunk.BLOCKS; x++) {
						World.Map.Block b = ch[x, y];
						if (b is World.Map.WallBlock) {
							World.Map.WallBlock wb = b as World.Map.WallBlock;
							for (int i = 0; i < 6; i++) {
								CheckTexture(wb[(World.Map.Side)i], texList);
							}
							empty = false;
						} else if (b is World.Map.FloorBlock) {
							World.Map.FloorBlock fb = b as World.Map.FloorBlock;
							if (fb.HasFloor) {
								CheckTexture(fb.Floor, texList);
								for (int i = 0; i < 4; i++) {
									CheckTexture(fb.FloorTrim[(World.Map.Side)i], texList);
								}
								empty = false;
							}
							if (fb.HasCeiling) {
								CheckTexture(fb.Ceiling, texList);
								for (int i = 0; i < 4; i++) {
									CheckTexture(fb.CeilingTrim[(World.Map.Side)i], texList);
								}
								empty = false;
							}
							
						}
					}
				}
				if (!empty) {
					chunksToWrite.Add(ch);
				}
			}

			// Writing textures
			mapf.Write((ushort)(texList.Count - 1));
			foreach (Graphics.Texture tex in texList) {
				if (tex != null) {
					mapf.Write(tex.Link);
				}
			}

			// Writing chunks
			mapf.Write((ushort)chunksToWrite.Count);
			foreach (World.Map.Chunk ch in chunksToWrite) {
				mapf.Write((Int32)ch.Location.X);
				mapf.Write((Int32)ch.Location.Y);
				mapf.Write((Int32)ch.Location.Z);
				for (int y = 0; y < World.Map.Chunk.BLOCKS; y++) {
					for (int x = 0; x < World.Map.Chunk.BLOCKS; x++) {
						World.Map.Block b = ch[x, y];
						if (b is World.Map.WallBlock) {
							World.Map.WallBlock wb = b as World.Map.WallBlock;
							mapf.Write((byte)1);
							for (int i = 0; i < 6; i++) {
								mapf.Write((ushort)Math.Max(texList.IndexOf(wb[(World.Map.Side)i]), 0));
							}
						} else if (b is World.Map.FloorBlock) {
							World.Map.FloorBlock fb = b as World.Map.FloorBlock;
							mapf.Write((byte)2);

							mapf.Write(fb.HasFloor);
							if (fb.HasFloor) {
								mapf.Write((ushort)Math.Max(texList.IndexOf(fb.Floor), 0));
								for (int i = 0; i < 4; i++) {
									mapf.Write((byte)Math.Ceiling(fb.FloorHeight[i] * 255f));	
								}
								for (int i = 0; i < 4; i++) {
									mapf.Write((ushort)Math.Max(texList.IndexOf(fb.FloorTrim[(World.Map.Side)i]), 0));
								}
							}
							mapf.Write(fb.HasCeiling);
							if (fb.HasCeiling) {
								mapf.Write((ushort)Math.Max(texList.IndexOf(fb.Ceiling), 0));
								for (int i = 0; i < 4; i++) {
									mapf.Write((byte)Math.Ceiling(fb.CeilingHeight[i] * 255f));
								}
								for (int i = 0; i < 4; i++) {
									mapf.Write((ushort)Math.Max(texList.IndexOf(fb.CeilingTrim[(World.Map.Side)i]), 0));
								}
							}

						} else {
							mapf.Write((byte)0);
						}
					}
				}
			}

			// Writing map chunk
			mainChunk.Children.Add(new BinaryChunk() {
				ID = "BLCK",
				Version = 1,
				Content = maps.ToArray()
			});

			// Writing objects
			List<BinaryChunk> entChunks = new List<BinaryChunk>();
			if (data.Entities != null) {
				foreach (GamePrefab gp in data.Entities) {

					MemoryStream entStr = new MemoryStream();
					BinaryWriter bw = new BinaryWriter(entStr);
					PrefabAttribute pa = Attribute.GetCustomAttribute(gp.GetType(), typeof(PrefabAttribute)) as PrefabAttribute;
					if (pa == null) {
						continue;
					}

					// Writing object
					bw.Write(pa.ID);
					gp.Save(bw);

					// Storing object
					entChunks.Add(new BinaryChunk() {
						ID = "PRFB",
						Version = 1,
						Content = entStr.ToArray()
					});
				}
			}

			// Writing map chunk
			ContainerChunk entChunk = new ContainerChunk() {
				ID = "OBJS",
				Version = 1
			};
			entChunk.Children.AddRange(entChunks);
			mainChunk.Children.Add(entChunk);

			// Writing ambient
			MemoryStream envs = new MemoryStream();
			BinaryWriter envf = new BinaryWriter(envs);
			envf.Write(new byte[] {
				data.Ambient.R,
				data.Ambient.G,
				data.Ambient.B
			});
			envf.Write(data.FogEnabled);
			if (data.Fog != null) {
				envf.Write(new byte[] {
					data.Fog.Color.R,
					data.Fog.Color.G,
					data.Fog.Color.B
				});
				envf.Write((float)data.Fog.Near);
				envf.Write((float)data.Fog.Far);
			} else {
				envf.Write(new byte[] {
					128, 128, 128
				});
				envf.Write((float)0f);
				envf.Write((float)5f);
			}
			if (data.Sky != null) {
				for (int i = 0; i < 6; i++) {
					World.Skybox.Side side = (World.Skybox.Side)i;
					if (data.Sky[side] != null && data.Sky[side].Link != "") {
						envf.Write(true);
						envf.Write(data.Sky[side].Link);
					} else {
						envf.Write(false);
					}
				}
			} else {
				for (int i = 0; i < 6; i++) {
					envf.Write(false);
				}
			}
			
			// Writing data
			mainChunk.Children.Add(new BinaryChunk() {
				ID = "ENVR",
				Version = 1,
				Content = envs.ToArray()
			});


			// Writing editor chunk
			MemoryStream devs = new MemoryStream();
			BinaryWriter devr = new BinaryWriter(devs);

			// Writing pos and angles
			devr.Write(data.CameraPos.X);
			devr.Write(data.CameraPos.Y);
			devr.Write(data.CameraPos.Z);
			devr.Write(data.CameraAngle.X);
			devr.Write(data.CameraAngle.Y);
			devr.Write(data.CameraAngle.Z);
			devr.Write((Int32)data.GridHeight);
			devr.Write((Int32)data.EditorFlags);

			// Writing map chunk
			mainChunk.Children.Add(new BinaryChunk() {
				ID = "EDTR",
				Version = 1,
				Content = devs.ToArray()
			});

			// Converting data
			return mainChunk;
		}

		/// <summary>
		/// Reading data
		/// </summary>
		/// <param name="chunk"></param>
		/// <returns></returns>
		public static MapData Read(Chunk chunk) {
			if (chunk.ID == "SMAP") {
				MapData data = new MapData();
				foreach (Chunk c in (chunk as ContainerChunk).Children) {
					switch (c.ID) {

						// Block map
						case "BLCK":
							BinaryReader mr = new BinaryReader(new MemoryStream((c as BinaryChunk).Content));
							
							// Reading textures
							int texCount = mr.ReadUInt16();
							Graphics.Texture[] texCache = new Graphics.Texture[texCount + 1];
							texCache[0] = null;
							for (int i = 0; i < texCount; i++) {
								texCache[i + 1] = new Graphics.Texture(mr.ReadString(), Graphics.Texture.LoadingMode.Queued);
							}

							// Reading chunks block
							World.Map map = new World.Map();
							int chunkCount = mr.ReadUInt16();
							for (int cn = 0; cn < chunkCount; cn++) {

								// Reading chunk coords
								int px = mr.ReadInt32();
								int py = mr.ReadInt32();
								int pz = mr.ReadInt32();
								World.Map.Chunk ch = new World.Map.Chunk();

								// Reading blocks
								for (int y = 0; y < World.Map.Chunk.BLOCKS; y++) {
									for (int x = 0; x < World.Map.Chunk.BLOCKS; x++) {
										byte type = mr.ReadByte();
										switch (type) {
											
											// Wall block
											case 1:
												World.Map.WallBlock wb = new World.Map.WallBlock();
												for (int s = 0; s < 6; s++) {
													wb[(World.Map.Side)s] = texCache[mr.ReadUInt16()];
												}
												ch[x, y] = wb;
												break;

											// Floor block
											case 2:
												World.Map.FloorBlock fb = new World.Map.FloorBlock();
												fb.HasFloor = mr.ReadBoolean();
												if (fb.HasFloor) {
													fb.Floor = texCache[mr.ReadUInt16()];
													float[] fheight = new float[4];
													for (int i = 0; i < 4; i++) {
														fheight[i] = (float)mr.ReadByte() / 255f;
													}
													for (int i = 0; i < 4; i++) {
														fb.FloorTrim[(World.Map.Side)i] = texCache[mr.ReadUInt16()];
													}
													fb.FloorHeight = fheight;
												}
												fb.HasCeiling = mr.ReadBoolean();
												if (fb.HasCeiling) {
													fb.Ceiling = texCache[mr.ReadUInt16()];
													float[] cheight = new float[4];
													for (int i = 0; i < 4; i++) {
														cheight[i] = (float)mr.ReadByte() / 255f;
													}
													for (int i = 0; i < 4; i++) {
														fb.CeilingTrim[(World.Map.Side)i] = texCache[mr.ReadUInt16()];
													}
													fb.CeilingHeight = cheight;
												}
												ch[x, y] = fb;
												break;
	
										}
									}
								}

								// Saving
								map[px, py, pz] = ch;
							}
							data.Map = map;
							break;

						// Objects
						case "OBJS":
							ContainerChunk cchunk = c as ContainerChunk;
							List<GamePrefab> objs = new List<GamePrefab>();
							foreach (Chunk bch in cchunk.Children) {
								BinaryChunk bc = bch as BinaryChunk;
								BinaryReader cr = new BinaryReader(new MemoryStream(bc.Content));
								
								// Reading type
								Type t = PrefabAttribute.GetPrefab(cr.ReadInt32());
								if (t != null) {
									GamePrefab pref = Activator.CreateInstance(t) as GamePrefab;
									pref.Load(cr);
									objs.Add(pref);
								}
							}
							data.Entities = objs.ToArray();
							break;

						// Environment chunk
						case "ENVR":
							BinaryReader er = new BinaryReader(new MemoryStream((c as BinaryChunk).Content));
							er.BaseStream.Position = 0;

							byte[] ambientRaw = er.ReadBytes(3);
							data.Ambient = Color.FromArgb(ambientRaw[0], ambientRaw[1], ambientRaw[2]);
							
							data.FogEnabled = er.ReadBoolean();
							data.Fog = new World.Fog();
							byte[] fogRaw = er.ReadBytes(3);
							data.Fog.Color = Color.FromArgb(fogRaw[0], fogRaw[1], fogRaw[2]);
							data.Fog.Near = er.ReadSingle();
							data.Fog.Far = er.ReadSingle();

							data.Sky = new World.Skybox();
							for (int i = 0; i < 6; i++) {
								bool hasTex = er.ReadBoolean();
								if (hasTex) {
									string path = er.ReadString();
									if (Engine.Current.Filesystem.Exists(path)) {
										data.Sky[(World.Skybox.Side)i] = new Graphics.Texture(path, Graphics.Texture.LoadingMode.Queued);
									}
								}
							}
							break;

						// Editor chunk
						case "EDTR":
							BinaryReader dr = new BinaryReader(new MemoryStream((c as BinaryChunk).Content));

							Vector3 cpos = new Vector3();
							Vector3 cang = new Vector3();
							cpos.X = dr.ReadSingle();
							cpos.Y = dr.ReadSingle();
							cpos.Z = dr.ReadSingle();
							cang.X = dr.ReadSingle();
							cang.Y = dr.ReadSingle();
							cang.Z = dr.ReadSingle();
							data.CameraPos = cpos;
							data.CameraAngle = cang;
							data.GridHeight = dr.ReadInt32();
							data.EditorFlags = dr.ReadInt32();

							break;


						default:
							break;
					}
				}
				return data;
			}
			return null;
		}

		/// <summary>
		/// Texture side
		/// </summary>
		static void CheckTexture(Graphics.Texture tex, List<Graphics.Texture> texCache) {
			if (tex != null) {
				if (tex.Link != "") {
					if (!texCache.Contains(tex)) {
						texCache.Add(tex);
					}
				}
			}
		}

		/// <summary>
		/// Map data
		/// </summary>
		public class MapData {

			/// <summary>
			/// Main map content
			/// </summary>
			public World.Map Map {
				get;
				set;
			}

			/// <summary>
			/// List of entities
			/// </summary>
			public GamePrefab[] Entities {
				get;
				set;
			}

			/// <summary>
			/// Fog info
			/// </summary>
			public World.Fog Fog {
				get;
				set;
			}

			/// <summary>
			/// Fog enabled flag
			/// </summary>
			public bool FogEnabled {
				get;
				set;
			}

			/// <summary>
			/// Skybox
			/// </summary>
			public World.Skybox Sky {
				get;
				set;
			}

			/// <summary>
			/// Ambient light
			/// </summary>
			public Color Ambient {
				get;
				set;
			}

			/// <summary>
			/// Camera position
			/// </summary>
			public Vector3 CameraPos {
				get;
				set;
			}

			/// <summary>
			/// Camera angle
			/// </summary>
			public Vector3 CameraAngle {
				get;
				set;
			}

			/// <summary>
			/// Grid height
			/// </summary>
			public int GridHeight {
				get;
				set;
			}

			/// <summary>
			/// Editor flags
			/// </summary>
			public int EditorFlags {
				get;
				set;
			}

		}
	}
}
