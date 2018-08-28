using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Components;
using Cubed.Components.Rendering;
using Cubed.Core;
using Cubed.Data.Rendering;
using Cubed.Data.Shaders;
using Cubed.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubed.World {
	
	/// <summary>
	/// Single level
	/// </summary>
	public class Map {

		/// <summary>
		/// Get or set map chunk
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <param name="z">Z coordinate</param>
		/// <returns>Chunk or null</returns>
		public Chunk this[int x, int y, int z] {
			get {
				string h = Hash(x, y, z);
				if (chunks.ContainsKey(h)) {
					return chunks[h];
				}
				return null;
			}
			set {
				string h = Hash(x, y, z);
				if (value != null && chunks.ContainsValue(value)) {
					string oldKey = chunks.FirstOrDefault(kv => kv.Value == value).Key;
					if (oldKey != h) {
						Chunk oc = chunks[oldKey];
						chunks.Remove(oldKey);
						oc.RemoveParent();
						chunks.Add(h, oc);
						oc.SetParent(this, x, y, z);
					}
				} else {
					Chunk oc = null;
					if (chunks.ContainsKey(h)) {
						oc = chunks[h];
						chunks.Remove(h);
						oc.RemoveParent();
					}
					if (value != null) {
						chunks.Add(h, value);
						value.SetParent(this, x, y, z);
					}
				}
			}
		}

		/// <summary>
		/// Minimal environment color
		/// </summary>
		public Color Ambient {
			get {
				return ambient;
			}
			set {
				if (ambient != value) {
					ambient = value;
					foreach (Chunk c in chunks.Values) {
						c.QueueRelight();
					}
				}
			}
		}

		/// <summary>
		/// Dictionary of chunks
		/// </summary>
		Dictionary<string, Chunk> chunks;

		/// <summary>
		/// Scene ambient color
		/// </summary>
		Color ambient;

		/// <summary>
		/// Parental scene
		/// </summary>
		internal Scene Parent {
			get;
			set;
		}

		/// <summary>
		/// Make hash from coords
		/// </summary>
		/// <param name="x">X</param>
		/// <param name="y">Y</param>
		/// <param name="z">Z</param>
		/// <returns>String hash</returns>
		string Hash(int x, int y, int z) {
			return x + "," + y + "," + z;
		}

		/// <summary>
		/// Map constructor
		/// </summary>
		public Map() {
			chunks = new Dictionary<string, Chunk>();
			Ambient = Color.DimGray;
		}

		/// <summary>
		/// Getting block from coords
		/// </summary>
		/// <param name="x">X</param>
		/// <param name="y">Y</param>
		/// <param name="z">Z</param>
		/// <returns>Block or null</returns>
		public Block GetBlockAtCoords(float x, float y, float z) {
			int cx = (int)Math.Floor(x / (float)Chunk.BLOCKS);
			int cy = (int)Math.Floor(y);
			int cz = (int)Math.Floor(z / (float)Chunk.BLOCKS);

			string hash = Hash(cx, cy, cz);
			if (chunks.ContainsKey(hash)) {
				float tx = x % Chunk.BLOCKS;
				float ty = z % Chunk.BLOCKS;
				if (tx < 0) {
					tx += Chunk.BLOCKS;
				}
				if (ty < 0) {
					ty += Chunk.BLOCKS;
				}

				// Calculating light from coords
				return chunks[hash][(int)tx, (int)ty];
			}
			return null;
		}

		/// <summary>
		/// Setting block at coords
		/// </summary>
		/// <param name="x">X</param>
		/// <param name="y">Y</param>
		/// <param name="z">Z</param>
		public void SetBlockAtCoords(int x, int y, int z, Block block) {
			int cx = (int)Math.Floor(x / (float)Chunk.BLOCKS);
			int cy = y;
			int cz = (int)Math.Floor(z / (float)Chunk.BLOCKS);

			string hash = Hash(cx, cy, cz);
			if (!chunks.ContainsKey(hash)) {
				Chunk ch = new Chunk();
				chunks.Add(hash, ch);
				ch.SetParent(this, cx, cy, cz);
			}
			Chunk chunk = chunks[hash];

			// Calculating block location
			int tx = x % Chunk.BLOCKS;
			int ty = z % Chunk.BLOCKS;
			if (tx < 0) {
				tx += Chunk.BLOCKS;
			}
			if (ty < 0) {
				ty += Chunk.BLOCKS;
			}

			// Checking near chunks
			if (tx == 0) {
				hash = Hash(cx - 1, cy, cz);
				if (!chunks.ContainsKey(hash)) {
					Chunk ch = new Chunk();
					chunks.Add(hash, ch);
					ch.SetParent(this, cx - 1, cy, cz);
				} else {
					chunks[hash].QueueRebuild();
				}
			}
			if (tx == Chunk.BLOCKS - 1) {
				hash = Hash(cx + 1, cy, cz);
				if (!chunks.ContainsKey(hash)) {
					Chunk ch = new Chunk();
					chunks.Add(hash, ch);
					ch.SetParent(this, cx + 1, cy, cz);
				} else {
					chunks[hash].QueueRebuild();
				}
			}
			if (ty == 0) {
				hash = Hash(cx, cy, cz - 1);
				if (!chunks.ContainsKey(hash)) {
					Chunk ch = new Chunk();
					chunks.Add(hash, ch);
					ch.SetParent(this, cx, cy, cz - 1);
				} else {
					chunks[hash].QueueRebuild();
				}
			}
			if (ty == Chunk.BLOCKS - 1) {
				hash = Hash(cx, cy, cz + 1);
				if (!chunks.ContainsKey(hash)) {
					Chunk ch = new Chunk();
					chunks.Add(hash, ch);
					ch.SetParent(this, cx, cy, cz + 1);
				} else {
					chunks[hash].QueueRebuild();
				}
			}

			// Setting block
			chunk[tx, ty] = block;
			chunk.QueueRebuild();
			if (Engine.Current != null) {
				if (Engine.Current.World != null) {
					if (Engine.Current.World.Map == this) {
						Vector3 pos = new Vector3(x, y, z);
						foreach (Entity ent in Engine.Current.World.Entities) {
							if (ent is Light) {
								Light l = ent as Light;
								if (l.Range >= (l.Position - pos).LengthFast) {
									l.MakeDirty();
									foreach (Chunk ch in chunks.Values) {
										if (ch.TouchesLight(l) && ch != chunk) {
											ch.QueueRelight();
										}
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Get light level
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public Color GetLightLevel(float x, float y, float z) {
			int cx = (int)Math.Floor(x / (float)Chunk.BLOCKS);
			int cy = (int)Math.Floor(y);
			int cz = (int)Math.Floor(z / (float)Chunk.BLOCKS);

			string hash = Hash(cx, cy, cz);
			if (chunks.ContainsKey(hash)) {
				float tx = x % Chunk.BLOCKS;
				float ty = z % Chunk.BLOCKS;
				if (tx < 0) {
					tx += Chunk.BLOCKS;
				}
				if (ty < 0) {
					ty += Chunk.BLOCKS;
				}

				// Calculating light from coords
				return chunks[hash].GetLightLevel(
					tx, ty
				);
			} else {

				// Calculating raw light distances
				Vector2 pos = new Vector2(x, z);
				float r = ambient.R, g = ambient.G, b = ambient.B;
				foreach (Entity ent in Engine.Current.World.Entities) {
					if (ent is Light) {
						Light l = ent as Light;
						Vector3 lpos = ent.Position;
						if (Math.Floor(lpos.Y) == Math.Floor(y)) {
							float magSquared = (pos - lpos.Xz).LengthSquared;
							if (magSquared < l.Range * l.Range) {
								float amount = 1f - (float)Math.Sqrt(magSquared) / l.Range;
								r += l.Color.R * amount;
								g += l.Color.G * amount;
								b += l.Color.B * amount;
							}
						}
					}
				}
				return Color.FromArgb(
					(byte)Math.Min(r, 255),
					(byte)Math.Min(g, 255),
					(byte)Math.Min(b, 255)
				);
			}
		}

		/// <summary>
		/// Render map
		/// </summary>
		internal void Render() {
			foreach (KeyValuePair<string, Chunk> item in chunks) {
				item.Value.Render();
			}
		}

		/// <summary>
		/// Rebuilding and relighting
		/// </summary>
		internal void Update(List<Light> lights, bool forceRebuild = false) {
			for (int i = 0; i < 2; i++) {
				foreach (Light l in lights) {
					if (i == 1) {
						l.UpdateForMap(this);
						l.Cleanup();
					}
				}
				foreach (KeyValuePair<string, Chunk> chunk in chunks) {
					chunk.Value.Update(lights, forceRebuild);
				}
			}
		}

		/// <summary>
		/// Get all the chunks
		/// </summary>
		/// <returns></returns>
		public Chunk[] GetAllChunks() {
			return chunks.Values.ToArray();
		}

		/// <summary>
		/// Single map chunk
		/// </summary>
		public class Chunk {

			/// <summary>
			/// Chunk size in blocks
			/// </summary>
			public const int BLOCKS = 8;

			/// <summary>
			/// Lightmap resolution
			/// </summary>
			public const int LIGHTMAP_SIZE = 128;

			/// <summary>
			/// Chunk location in map
			/// </summary>
			public Vector3 Location {
				get;
				private set;
			}

			/// <summary>
			/// Parental map
			/// </summary>
			public Map Parent {
				get;
				private set;
			}

			/// <summary>
			/// Neighbor chunks
			/// </summary>
			Chunk[] neighbors;

			/// <summary>
			/// Chunk needs to be rebuilt
			/// </summary>
			bool dirty = false;

			/// <summary>
			/// Chunk needs to be relight
			/// </summary>
			bool relight = false;

			/// <summary>
			/// Current chunk
			/// </summary>
			Matrix4 matrix;

			/// <summary>
			/// Array of blocks
			/// </summary>
			Block[,] blocks;

			/// <summary>
			/// Кеш освещения
			/// </summary>
			Color[,] lightCache;

			/// <summary>
			/// Renderable groups
			/// </summary>
			Dictionary<Texture, RenderGroup> groups;

			/// <summary>
			/// Empty render group
			/// </summary>
			RenderGroup nullGroup;

			/// <summary>
			/// Texture of lighting
			/// </summary>
			int lightingTex;

			/// <summary>
			/// Framebuffer
			/// </summary>
			static int frameBuffer;

			/// <summary>
			/// Lights that touches this chunk
			/// </summary>
			List<Light> affectedLights;

			/// <summary>
			/// Light barriers
			/// </summary>
			List<LightObstructor> obstructors;

			/// <summary>
			/// Chunk constructor
			/// </summary>
			public Chunk() {
				neighbors = new Chunk[6];
				blocks = new Block[Chunk.BLOCKS, Chunk.BLOCKS];
				groups = new Dictionary<Texture, RenderGroup>();
				affectedLights = new List<Light>();
				obstructors = new List<LightObstructor>();
				nullGroup = null;
				dirty = true;
				relight = true;
			}

			/// <summary>
			/// Accesing chunk blocks
			/// </summary>
			/// <param name="x">X coordinate</param>
			/// <param name="y">Y coordinate</param>
			/// <returns>Block or null</returns>
			public Block this[int x, int y] {
				get {
					return blocks[y, x];
				}
				set {
					if (blocks[y, x] != null) {
						blocks[y, x].Parent = null;
						blocks[y, x] = null;
					}
					if (value != null) {
						blocks[y, x] = value;
						blocks[y, x].Parent = this;
					}
					if (x == 0 && neighbors[(int)Side.Left] != null) {
						neighbors[(int)Side.Left].QueueRebuild();
					}
					if (x == BLOCKS - 1 && neighbors[(int)Side.Right] != null) {
						neighbors[(int)Side.Right].QueueRebuild();
					}
					if (y == 0 && neighbors[(int)Side.Back] != null) {
						neighbors[(int)Side.Back].QueueRebuild();
					}
					if (y == BLOCKS - 1 && neighbors[(int)Side.Forward] != null) {
						neighbors[(int)Side.Forward].QueueRebuild();
					}
					if (neighbors[(int)Side.Top] != null) {
						neighbors[(int)Side.Top].QueueRebuild();
					}
					if (neighbors[(int)Side.Bottom] != null) {
						neighbors[(int)Side.Bottom].QueueRebuild();
					}

					dirty = true;
					relight = true;
				}
			}

			/// <summary>
			/// Setting parent
			/// </summary>
			protected internal void SetParent(Map map, int x, int y, int z) {
				matrix = Matrix4.CreateTranslation(x * BLOCKS, y, -z * BLOCKS);
				Location = new Vector3(x, y, z);
				Parent = map;
				for (int i = 0; i < 6; i++) {
					SetNeighbor(i);
				}
				QueueRebuild();
			}

			/// <summary>
			/// Removing parent
			/// </summary>
			protected internal void RemoveParent() {
				for (int i = 0; i < 6; i++) {
					SetNeighbor(i, true);
				}
			}

			/// <summary>
			/// Get light level of mesh
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			internal Color GetLightLevel(float x, float y) {

				// Checking for lights
				List<Light> lights = new List<Light>();
				foreach (Entity ent in Engine.Current.World.Entities) {
					if (ent is Light) {
						lights.Add(ent as Light);
					}
				}
				
				// Updating all lights
				Parent.Update(lights);

				// Getting color
				if (lightingTex != 0) {

					// Check for cache
					if (lightCache == null) {
						lightCache = new Color[LIGHTMAP_SIZE, LIGHTMAP_SIZE];
						byte[] rawData = new byte[LIGHTMAP_SIZE * LIGHTMAP_SIZE * 3];

						GL.BindTexture(TextureTarget.Texture2D, lightingTex);
						GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
						GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, lightingTex, 0);
						GL.BindTexture(TextureTarget.Texture2D, 0);
						GL.ReadPixels(0, 0, LIGHTMAP_SIZE, LIGHTMAP_SIZE, PixelFormat.Rgb, PixelType.UnsignedByte, rawData);
						GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

						// Converting pixel data to array
						int dataPos = 0;
						for (int ry = 0; ry < LIGHTMAP_SIZE; ry++) {
							for (int rx = 0; rx < LIGHTMAP_SIZE; rx++) {
								lightCache[ry, rx] = Color.FromArgb(rawData[dataPos], rawData[dataPos + 1], rawData[dataPos + 2]);
								dataPos += 3;
							}
						}
					}

					// Resolving light level
					y = (float)Chunk.BLOCKS - y;
					int cx = Math.Min((int)Math.Floor(x / (float)Chunk.BLOCKS * (float)Chunk.LIGHTMAP_SIZE), LIGHTMAP_SIZE - 1);
					int cy = Math.Min((int)Math.Floor(y / (float)Chunk.BLOCKS * (float)Chunk.LIGHTMAP_SIZE), LIGHTMAP_SIZE - 1);
					Color cached = lightCache[cy, cx];
					Color ambient = Engine.Current.World.Map.ambient;
					return Color.FromArgb(
						Math.Min(ambient.R + cached.R, 255),
						Math.Min(ambient.G + cached.G, 255),
						Math.Min(ambient.B + cached.B, 255)
					);
				}
				return Color.Red;
			}

			/// <summary>
			/// Queue geometry rebuilding
			/// </summary>
			internal void QueueRebuild() {
				dirty = true;
			}

			/// <summary>
			/// Queue light rebuilding
			/// </summary>
			internal void QueueRelight() {
				relight = true;
			}

			/// <summary>
			/// Check for light and chunk intersection
			/// </summary>
			/// <param name="l">Light</param>
			/// <returns>True if intersection exist</returns>
			internal bool TouchesLight(Light l) {
				if (l.Position.Y >= Location.Y && l.Position.Y < Location.Y + 1f) {
					RectangleF lightRect = new RectangleF(l.Position.X - l.Range, l.Position.Z - l.Range, l.Range * 2f, l.Range * 2f);
					RectangleF chunkRect = new RectangleF(Location.X * BLOCKS, Location.Z * BLOCKS, BLOCKS, BLOCKS);
					return lightRect.IntersectsWith(chunkRect);
				}
				return false;
			}

			/// <summary>
			/// Setting neighbor
			/// </summary>
			/// <param name="side">Side</param>
			/// <param name="forget">Flag to forget neighbor</param>
			void SetNeighbor(int side, bool forget = false) { 
				int nside = 0;
				int x = (int)Location.X, y = (int)Location.Y, z = (int)Location.Z;
				switch ((Side)side) {
					case Side.Forward:
						z += 1;
						nside = (int)Side.Back;
						break;
					case Side.Right:
						x += 1;
						nside = (int)Side.Left;
						break;
					case Side.Back:
						z -= 1;
						nside = (int)Side.Forward;
						break;
					case Side.Left:
						x -= 1;
						nside = (int)Side.Right;
						break;
					case Side.Top:
						y += 1;
						nside = (int)Side.Bottom;
						break;
					case Side.Bottom:
						y -= 1;
						nside = (int)Side.Top;
						break;
					default:
						break;
				}
				Chunk c = Parent[x, y, z];
				if (c != null) {
					if (!forget) {
						c.neighbors[nside] = this; 
					} else {
						c.neighbors[nside] = null;
					}
					c.dirty = true;
				}
				if (!forget) {
					neighbors[side] = c;
				} else {
					neighbors[side] = null;
				}
				dirty = true;
			}

			/// <summary>
			/// Check for rebuild and relighting
			/// </summary>
			/// <param name="forceRebuild"></param>
			internal void Update(List<Light> lights, bool forceRebuild = false, bool allowRelight = true) {
				foreach (Light l1 in lights) {
					if (TouchesLight(l1) && !affectedLights.Contains(l1)) {
						affectedLights.Add(l1);
						relight = true;
					}
				}
				List<Light> tempLights = new List<Light>(affectedLights);
				foreach (Light l in tempLights) {
					if (!TouchesLight(l) || !lights.Contains(l)) {
						affectedLights.Remove(l);
						relight = true;
					}
				}
				foreach (Light l2 in affectedLights) {
					if (l2.IsChanged) {
						relight = true;
					}
				}

				if (dirty || forceRebuild) {
					RebuildGeometry();
					RebuildObstructors();
					relight = true;
					dirty = false;
				}
				if (relight && allowRelight) {
					RebuildObstructors();
					RecalculateLight();
					relight = false;
				}
			}

			/// <summary>
			/// Rendering
			/// </summary>
			internal void Render() {
				if (IsVisible()) {
					ShaderSystem.EntityMatrix = matrix;

					// Binding texture
					GL.ActiveTexture(TextureUnit.Texture1);
					GL.Enable(EnableCap.Texture2D);
					GL.BindTexture(TextureTarget.Texture2D, lightingTex);
					GL.ActiveTexture(TextureUnit.Texture0);

					// Drawing surfaces
					MapShader.Shader.Bind();
					MapShader.Shader.AmbientColor = Parent.Ambient;

					// Setting up fog
					Fog fog = Scene.Current.Fog;
					MapShader.Shader.FogEnabled = fog != null;
					if (fog != null) {
						MapShader.Shader.FogNear = fog.Near;
						MapShader.Shader.FogFar = fog.Far;
						MapShader.Shader.FogColor = fog.Color;
					}

					// Rendering surfaces
					foreach (KeyValuePair<Texture, RenderGroup> group in groups) {
						RenderSingle(group.Key, group.Value);
					}
					RenderSingle(null, nullGroup);

					// Unbinding
					MapShader.Shader.Unbind();
					GL.ActiveTexture(TextureUnit.Texture1);
					Texture.BindEmpty();
					GL.Disable(EnableCap.Texture2D);
					GL.ActiveTexture(TextureUnit.Texture0);
				}
			}

			/// <summary>
			/// Return transformed obstructors
			/// </summary>
			/// <param name="l">Light</param>
			/// <returns>Array of obstructors</returns>
			internal LightObstructor[] GetObstructors(Light l) {
				Vector3 lp = l.Position;
				List<LightObstructor> ls = new List<LightObstructor>();
				foreach (LightObstructor ob in obstructors) {
					LightObstructor o = new LightObstructor();
					foreach (Vector2 v in ob.Points) {
						o.Points.Add(new Vector2(
							v.X + Location.X * BLOCKS - lp.X,
							v.Y + Location.Z * BLOCKS - lp.Z
						));
					}
					ls.Add(o);
				}
				return ls.ToArray();
			}

			/// <summary>
			/// Checkig for camera
			/// </summary>
			/// <returns>True if chunk is in view</returns>
			bool IsVisible() {
				if (Parent != null) {
					return Frustum.Contains(new Vector3(
						Location.X * BLOCKS + BLOCKS / 2,
						Location.Y + 0.5f,
						Location.Z * BLOCKS + BLOCKS / 2
					), 5.6785f);
				}
				return false;
			}

			/// <summary>
			/// Render single texture group
			/// </summary>
			/// <param name="tex">Texture</param>
			/// <param name="rg">Group</param>
			void RenderSingle(Texture tex, RenderGroup rg) {
				if (rg != null && rg.IndexCount > 0) {
					if (tex != null) {
						tex.Bind();
					} else {
						Texture.BindEmpty();
					}
					if (Caps.ShaderPipeline) {

						MapShader shader = MapShader.Shader;
						GL.BindBuffer(BufferTarget.ArrayBuffer, rg.VertexBuffer);
						GL.VertexAttribPointer(shader.VertexBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
						GL.BindBuffer(BufferTarget.ArrayBuffer, rg.TexCoordBuffer);
						GL.VertexAttribPointer(shader.TexCoordBufferLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
						GL.BindBuffer(BufferTarget.ArrayBuffer, rg.LightmapCoordBuffer);
						GL.VertexAttribPointer(shader.LightTexCoordBufferLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
						GL.BindBuffer(BufferTarget.ElementArrayBuffer, rg.IndexBuffer);
						GL.DrawElements(PrimitiveType.Triangles, rg.IndexCount, DrawElementsType.UnsignedShort, 0);
						GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
						GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

					} else {
						/*
						GL.MatrixMode(MatrixMode.Modelview);
						GL.PushMatrix();
						GL.MultMatrix(ref matrix); int num = 0;

						GL.BindBuffer(BufferTarget.ArrayBuffer, rg.VertexBuffer);
						GL.EnableClientState(ArrayCap.VertexArray);
						GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

						GL.ClientActiveTexture(TextureUnit.Texture0);
						GL.DisableClientState(ArrayCap.TextureCoordArray);
						GL.ClientActiveTexture(TextureUnit.Texture1);
						GL.DisableClientState(ArrayCap.TextureCoordArray);
						GL.ClientActiveTexture(TextureUnit.Texture2);
						GL.DisableClientState(ArrayCap.TextureCoordArray);


						GL.BindBuffer(BufferTarget.ElementArrayBuffer, rg.IndexBuffer);
						GL.DrawElements(PrimitiveType.Points, rg.IndexCount, DrawElementsType.UnsignedShort, 0);


						GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
						GL.DisableClientState(ArrayCap.VertexArray);
						
						GL.PopMatrix();
						 */
					}
					Engine.Current.drawCalls++;
				}
			}

			/// <summary>
			/// Rebuilding geometry
			/// </summary>
			void RebuildGeometry() {

				// Listing all textures
				List<Texture> textures = new List<Texture>();
				for (int y = -1; y <= BLOCKS; y++) {
					for (int x = -1; x <= BLOCKS; x++) {
						Block b = GetBlock(x, y);
						if (b != null) {
							if (b is WallBlock) {
								WallBlock wb = b as WallBlock;
								for (int i = 0; i < 6; i++) {
									Texture tex = wb[(Side)i];
									if (!textures.Contains(tex)) {
										textures.Add(tex);
									}
								}
							} else if(b is FloorBlock) {
								FloorBlock fb = b as FloorBlock;
								if (fb.HasFloor && !textures.Contains(fb.Floor)) {
									textures.Add(fb.Floor);
								}
								if (fb.HasCeiling && !textures.Contains(fb.Ceiling)) {
									textures.Add(fb.Ceiling);
								}
								for (int i = 0; i < 4; i++) {
									if (fb.HasFloor && !textures.Contains(fb.FloorTrim[(Side)i])) {
										textures.Add(fb.FloorTrim[(Side)i]);
									}
									if (fb.HasCeiling && !textures.Contains(fb.CeilingTrim[(Side)i])) {
										textures.Add(fb.CeilingTrim[(Side)i]);
									}
								}
							}
						}
					}
				}
				if (!textures.Contains(null)) {
					textures.Add(null);
				}


				// Rebuilding surfaces
				foreach (Texture tex in textures) {

					// Creating arrays
					List<float> vertCoords = new List<float>();
					List<float> texCoords = new List<float>();
					List<float> lightCoords = new List<float>();
					List<short> indices = new List<short>();
					int idx = 0;

					// Composing blocks
					for (int y = 0; y < BLOCKS; y++) {
						for (int x = 0; x < BLOCKS; x++) {
							Block b = blocks[y, x];
							float div = 1f / (float)BLOCKS;
							float cx = (float)x * div, cy = 1f - (float)y * div;

							// Floor and ceiling height
							float[] floorHeight = new float[] {
								0f, 0f, 0f, 0f
							};
							float[] ceilingHeight = new float[] {
								0f, 0f, 0f, 0f
							};

							// Building floors and ceilings
							if (b is FloorBlock) {
								FloorBlock fb = b as FloorBlock;
								if (fb.HasFloor) {
									floorHeight = fb.FloorHeight;
									if (fb.Floor == tex) {
										vertCoords.AddRange(new float[]{
											x, floorHeight[0], -y,
											x + 1, floorHeight[1], -y,
											x, floorHeight[2], -y - 1,
											x + 1, floorHeight[3], -y - 1
										});
										lightCoords.AddRange(new float[]{
											cx, cy,
											cx + div, cy,
											cx, cy - div,
											cx + div, cy - div
										});
										texCoords.AddRange(new float[]{
											1f, 0f,
											0f, 0f,
											1f, 1f,
											0f, 1f
										});
										indices.AddRange(new short[]{
											(short)(idx + 0), (short)(idx + 1), (short)(idx + 2),
											(short)(idx + 1), (short)(idx + 3), (short)(idx + 2)
										});
										idx += 4;
									}
								}
								if (fb.HasCeiling) {
									ceilingHeight = fb.CeilingHeight;
									if (fb.Ceiling == tex) {
										vertCoords.AddRange(new float[]{
											x + 1, 1 - ceilingHeight[0], -y,
											x, 1 - ceilingHeight[1], -y,
											x + 1, 1 - ceilingHeight[2], -y - 1,
											x, 1 - ceilingHeight[3], -y - 1
										});
										lightCoords.AddRange(new float[]{
											cx + div, cy,
											cx, cy,
											cx + div, cy - div,
											cx, cy - div
										});
										texCoords.AddRange(new float[]{
											1f, 0f,
											0f, 0f,
											1f, 1f,
											0f, 1f
										});
											indices.AddRange(new short[]{
											(short)(idx + 0), (short)(idx + 1), (short)(idx + 2),
											(short)(idx + 1), (short)(idx + 3), (short)(idx + 2)
										});
										idx += 4;
									}
								}
							}

							if (!(b is WallBlock)) {
								
								// Calculating neighbors
								for (int isd = 0; isd < 6; isd++) {

									// Building wall surfaces
									int dx = 0, dy = 0;
									Side side = (Side)isd;
									Side opposite = Side.Back;
									switch (side) {
										case Side.Forward:
											dy = 1;
											opposite = Side.Back;
											break;
										case Side.Back:
											dy = -1;
											opposite = Side.Forward;
											break;
										case Side.Right:
											dx = 1;
											opposite = Side.Left;
											break;
										case Side.Left:
											dx = -1;
											opposite = Side.Right;
											break;
										default:
											continue;
									}

									// Getting neighbor block
									Block nb = GetBlock(x + dx, y + dy);
									FloorBlock fnb = null;
									bool needWall = false, needFloorWall = false, needCeilWall = false;
									if (nb != null) {
										if (nb is WallBlock) {
											if ((nb as WallBlock)[opposite] == tex) {
												needWall = true;
											}
										} else if(nb is FloorBlock) {
											fnb = nb as FloorBlock;
											if (fnb.HasFloor && fnb.FloorTrim[opposite] == tex) {
												needFloorWall = true;
											}
											if (fnb.HasCeiling && fnb.CeilingTrim[opposite] == tex) {
												needCeilWall = true;
											}
										}
									}
									if (needWall) {
										float[] lightUV = null;
										float[] vertXYZ = null;
										float bias = 0.001f;//1f / (float)(LIGHTMAP_SIZE * 2f);

										// Wall height
										float[] height = new float[] {
											1, 1, 0, 0
										};

										switch (side) {

											// Forward side
											case Side.Forward:
												height = new float[] {
													1f - ceilingHeight[3], 1f - ceilingHeight[2],
													floorHeight[2], floorHeight[3]
												};
												vertXYZ = new float[] {
													x,		height[0], -y - 1,
													x + 1,	height[1], -y - 1, 
													x,		height[2], -y - 1,
													x + 1,	height[3], -y - 1
												};
												lightUV = new float[] {
													cx,			cy - div + bias,
													cx + div,	cy - div + bias,
													cx,			cy - div + bias,
													cx + div,	cy - div + bias
												};
												break;

											// Back side
											case Side.Back:
												height = new float[] {
													1f - ceilingHeight[0], 1f - ceilingHeight[1],
													floorHeight[1], floorHeight[0]
												};
												vertXYZ = new float[] {
													x +	1,	height[0], -y,
													x,		height[1], -y, 
													x + 1,	height[2], -y,
													x,		height[3], -y
												};
												lightUV = new float[] {
													cx + div,	cy - bias,
													cx,			cy - bias,
													cx + div,	cy - bias,
													cx,			cy - bias
												};
												break;

											// Right side
											case Side.Right:
												height = new float[] {
													1f - ceilingHeight[2], 1f - ceilingHeight[0],
													floorHeight[3], floorHeight[1]
												};
												vertXYZ = new float[] {
													x + 1,	height[0], -y - 1,
													x + 1,	height[1], -y, 
													x + 1,	height[2], -y - 1,
													x + 1,	height[3], -y
												};
												lightUV = new float[] {
													cx + div - bias,	cy - div,
													cx + div - bias,	cy,
													cx + div - bias,	cy - div,
													cx + div - bias,	cy,
												};
												break;

											// Left side
											case Side.Left:
												height = new float[] {
													1f - ceilingHeight[1], 1f - ceilingHeight[3],
													floorHeight[0], floorHeight[2]
												};
												vertXYZ = new float[] {
													x,	height[0], -y,
													x,	height[1], -y - 1, 
													x,	height[2], -y,
													x,	height[3], -y - 1
												};
												lightUV = new float[] {
													cx + bias,	cy,
													cx + bias,	cy - div,
													cx + bias,	cy,
													cx + bias,	cy - div
												};
												break;
											
										}

										// Adding surfaces
										if (lightUV != null && vertXYZ != null) {
											vertCoords.AddRange(vertXYZ);
											lightCoords.AddRange(lightUV);
											texCoords.AddRange(new float[]{
												0f, 1f - height[0],
												1f, 1f - height[1],
												0f, 1f - height[2],
												1f, 1f - height[3]
											});
											indices.AddRange(new short[]{
												(short)(idx + 0), (short)(idx + 2), (short)(idx + 1),
												(short)(idx + 1), (short)(idx + 2), (short)(idx + 3)
											});
											idx += 4;
										}
									}	

									// Floor trimming wall
									if (needFloorWall || needCeilWall) {
										float bias = 0.001f;
										float[] ofh = fnb.FloorHeight;
										float[] och = fnb.CeilingHeight;
										float[] baseCoords = null;
										float[] baseLightCoords = null;

										// Trim height
										float baseFL = 0, baseFR = 0;
										float hFL = 0, hFR = 0;
										float baseCL = 0, baseCR = 0;
										float hCL = 0, hCR = 0;

										switch (side) {

											// Forward side
											case Side.Forward:
												baseFL = floorHeight[2];
												baseFR = floorHeight[3];
												baseCL = ceilingHeight[3];
												baseCR = ceilingHeight[2];
												hFL = ofh[0];
												hFR = ofh[1];
												hCL = och[1];
												hCR = och[0];
												baseCoords = new float[] {
													x,		-y - 1,
													x + 1,	-y - 1
												};
												baseLightCoords = new float[] {
													cx,			cy - div + bias,
													cx + div,	cy - div + bias
												};
												break;

											
											// Back side
											case Side.Back:
												baseFL = floorHeight[1];
												baseFR = floorHeight[0];
												baseCL = ceilingHeight[0];
												baseCR = ceilingHeight[1];
												hFL = ofh[3];
												hFR = ofh[2];
												hCL = och[2];
												hCR = och[3];
												baseCoords = new float[] {
													x +	1,	-y,
													x,		-y
												};
												baseLightCoords = new float[] {
													cx + div,	cy - bias,
													cx,			cy - bias
												};
												break;

											
											// Right side
											case Side.Right:
												baseFL = floorHeight[3];
												baseFR = floorHeight[1];
												baseCL = ceilingHeight[2];
												baseCR = ceilingHeight[0];
												hFL = ofh[2];
												hFR = ofh[0];
												hCL = och[3];
												hCR = och[1];
												baseCoords = new float[] {
													x + 1,	-y - 1,
													x + 1,	-y
												};
												baseLightCoords = new float[] {
													cx + div - bias,	cy - div,
													cx + div - bias,	cy
												};
												break;

											// Left side
											case Side.Left:
												baseFL = floorHeight[0];
												baseFR = floorHeight[2];
												baseCL = ceilingHeight[1];
												baseCR = ceilingHeight[3];
												hFL = ofh[1];
												hFR = ofh[3];
												hCL = och[0];
												hCR = och[2];
												baseCoords = new float[] {
													x, -y,
													x, -y - 1
												};
												baseLightCoords = new float[] {
													cx + bias,	cy,
													cx + bias,	cy - div,
												};
												break;
												
										}

										// Checking height
										if (needFloorWall && (hFL > baseFL || hFR > baseFR)) {
											if (hFL > baseFL && hFR > baseFR) {
												vertCoords.AddRange(new float[]{
													baseCoords[0], hFL, baseCoords[1],
													baseCoords[2], hFR, baseCoords[3],
													baseCoords[0], baseFL, baseCoords[1],
													baseCoords[2], baseFR, baseCoords[3],
												});
												lightCoords.AddRange(new float[]{
													baseLightCoords[0], baseLightCoords[1],
													baseLightCoords[2], baseLightCoords[3],
													baseLightCoords[0], baseLightCoords[1],
													baseLightCoords[2], baseLightCoords[3]
												});
												texCoords.AddRange(new float[]{
													0f, 1f - hFL,
													1f, 1f - hFR,
													0f, 1f - baseFL,
													1f, 1f - baseFR
												});
												indices.AddRange(new short[]{
													(short)(idx + 0), (short)(idx + 2), (short)(idx + 1),
													(short)(idx + 1), (short)(idx + 2), (short)(idx + 3)
												});
												idx += 4;
											} else {
												float delta = GetFloorIntersection(baseFL, baseFR, hFL, hFR);
												if (hFL > baseFL) {
													vertCoords.AddRange(new float[]{
														baseCoords[0], baseFL, baseCoords[1],
														baseCoords[0], hFL, baseCoords[1],
														baseCoords[0] + (baseCoords[2] - baseCoords[0]) * delta, 
														baseFL + (baseFR - baseFL) * delta, 
														baseCoords[1] + (baseCoords[3] - baseCoords[1]) * delta
													});
													lightCoords.AddRange(new float[]{
														baseLightCoords[0], baseLightCoords[1],
														baseLightCoords[0], baseLightCoords[1],
														baseLightCoords[0] + (baseLightCoords[2] - baseLightCoords[0]) * delta,
														baseLightCoords[1] + (baseLightCoords[3] - baseLightCoords[1]) * delta,
													});
													texCoords.AddRange(new float[]{
														0f, 1f - baseFL,
														0f, 1f - hFL,
														delta, 1f - (baseFL + (baseFR - baseFL) * delta) 
													});
												} else {
													vertCoords.AddRange(new float[]{
														baseCoords[2], hFR, baseCoords[3],
														baseCoords[2], baseFR, baseCoords[3],
														baseCoords[0] + (baseCoords[2] - baseCoords[0]) * delta, 
														baseFL + (baseFR - baseFL) * delta, 
														baseCoords[1] + (baseCoords[3] - baseCoords[1]) * delta
													});
													lightCoords.AddRange(new float[]{
														baseLightCoords[0], baseLightCoords[1],
														baseLightCoords[0], baseLightCoords[1],
														baseLightCoords[0] + (baseLightCoords[2] - baseLightCoords[0]) * delta,
														baseLightCoords[1] + (baseLightCoords[3] - baseLightCoords[1]) * delta,
													});
													texCoords.AddRange(new float[]{
														1f, 1f - hFR,
														1f, 1f - baseFR,
														delta, 1f - (baseFL + (baseFR - baseFL) * delta) 
													});
												}
												indices.AddRange(new short[]{
													(short)(idx + 0), (short)(idx + 2), (short)(idx + 1),
												});
												idx += 3;
											}
										}
										
										if (needCeilWall && (hCL > baseCL || hCR > baseCR)) {
											if (hCL > baseCL && hCR > baseCR) {
												vertCoords.AddRange(new float[]{
													baseCoords[0], 1f - baseCL, baseCoords[1],
													baseCoords[2], 1f - baseCR, baseCoords[3],
													baseCoords[0], 1f - hCL, baseCoords[1],
													baseCoords[2], 1f - hCR, baseCoords[3],
												});
												lightCoords.AddRange(new float[]{
													baseLightCoords[0], baseLightCoords[1],
													baseLightCoords[2], baseLightCoords[3],
													baseLightCoords[0], baseLightCoords[1],
													baseLightCoords[2], baseLightCoords[3]
												});
												texCoords.AddRange(new float[]{
													0f, baseCL,
													1f, baseCR,
													0f, hCL,
													1f, hCR
												});
												indices.AddRange(new short[]{
													(short)(idx + 0), (short)(idx + 2), (short)(idx + 1),
													(short)(idx + 1), (short)(idx + 2), (short)(idx + 3)
												});
												idx += 4;
											} else {
												float delta = GetFloorIntersection(baseCL, baseCR, hCL, hCR);
												if (hCL > baseCL) {
													vertCoords.AddRange(new float[]{
														baseCoords[0], 1f - hCL, baseCoords[1],
														baseCoords[0], 1f- baseCL, baseCoords[1],
														baseCoords[0] + (baseCoords[2] - baseCoords[0]) * delta, 
														1f - (baseCL + (baseCR - baseCL) * delta), 
														baseCoords[1] + (baseCoords[3] - baseCoords[1]) * delta
													});
													lightCoords.AddRange(new float[]{
														baseLightCoords[0], baseLightCoords[1],
														baseLightCoords[0], baseLightCoords[1],
														baseLightCoords[0] + (baseLightCoords[2] - baseLightCoords[0]) * delta,
														baseLightCoords[1] + (baseLightCoords[3] - baseLightCoords[1]) * delta,
													});
													texCoords.AddRange(new float[]{
														0f, hCL,
														0f, baseCL,
														delta, (baseCL + (baseCR - baseCL) * delta) 
													});
												} else {
													vertCoords.AddRange(new float[]{
														baseCoords[2], 1f - baseCR, baseCoords[3],
														baseCoords[2], 1f - hCR, baseCoords[3],
														baseCoords[0] + (baseCoords[2] - baseCoords[0]) * delta, 
														1f - (baseCL + (baseCR - baseCL) * delta), 
														baseCoords[1] + (baseCoords[3] - baseCoords[1]) * delta
													});
													lightCoords.AddRange(new float[]{
														baseLightCoords[0], baseLightCoords[1],
														baseLightCoords[0], baseLightCoords[1],
														baseLightCoords[0] + (baseLightCoords[2] - baseLightCoords[0]) * delta,
														baseLightCoords[1] + (baseLightCoords[3] - baseLightCoords[1]) * delta,
													});
													texCoords.AddRange(new float[]{
														1f, baseCR,
														1f, hCR,
														delta, (baseCL + (baseCR - baseCL) * delta) 
													});
												}
												indices.AddRange(new short[]{
													(short)(idx + 0), (short)(idx + 2), (short)(idx + 1),
												});
												idx += 3;
											}
										}
									}
								}
							}
						}
					}

					// Creating group
					RenderGroup rg = null;
					if (tex == null) {
						if (nullGroup == null) {
							nullGroup = new RenderGroup();
						}
						rg = nullGroup;
					} else {
						if (groups.ContainsKey(tex)) {
							rg = groups[tex];
						} else {
							rg = new RenderGroup();
							groups.Add(tex, rg);
						}
					}
					
					if (rg.VertexBuffer == 0 || !GL.IsBuffer(rg.VertexBuffer)) {
						rg.VertexBuffer = GL.GenBuffer();
					}
					if (rg.TexCoordBuffer == 0 || !GL.IsBuffer(rg.TexCoordBuffer)) {
						rg.TexCoordBuffer = GL.GenBuffer();
					}
					if (rg.LightmapCoordBuffer == 0 || !GL.IsBuffer(rg.LightmapCoordBuffer)) {
						rg.LightmapCoordBuffer = GL.GenBuffer();
					}
					if (rg.IndexBuffer == 0 || !GL.IsBuffer(rg.IndexBuffer)) {
						rg.IndexBuffer = GL.GenBuffer();
					}
					rg.IndexCount = indices.Count;
					GL.BindBuffer(BufferTarget.ArrayBuffer, rg.VertexBuffer);
					GL.BufferData(BufferTarget.ArrayBuffer, vertCoords.Count * 4, vertCoords.ToArray(), BufferUsageHint.StaticDraw);
					GL.BindBuffer(BufferTarget.ArrayBuffer, rg.TexCoordBuffer);
					GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Count * 4, texCoords.ToArray(), BufferUsageHint.StaticDraw);
					GL.BindBuffer(BufferTarget.ArrayBuffer, rg.LightmapCoordBuffer);
					GL.BufferData(BufferTarget.ArrayBuffer, lightCoords.Count * 4, lightCoords.ToArray(), BufferUsageHint.StaticDraw);
					GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, rg.IndexBuffer);
					GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * 2, indices.ToArray(), BufferUsageHint.StaticDraw);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
				}
				
				// Cleaning unused buffers
				List<Texture> groupsToRemove = new List<Texture>();
				foreach (KeyValuePair<Texture, RenderGroup> pair in groups) {
					if (!textures.Contains(pair.Key)) {
						RenderGroup rg = pair.Value;
						if (rg.VertexBuffer != 0) {
							GL.DeleteBuffer(rg.VertexBuffer);
						}
						if (rg.TexCoordBuffer != 0) {
							GL.DeleteBuffer(rg.TexCoordBuffer);
						}
						if (rg.LightmapCoordBuffer != 0) {
							GL.DeleteBuffer(rg.LightmapCoordBuffer);
						}
						if (rg.IndexBuffer != 0) {
							GL.DeleteBuffer(rg.IndexBuffer);
						}
						groupsToRemove.Add(pair.Key);
					}
				}
				foreach (Texture tex in groupsToRemove) {
					groups.Remove(tex);
				}

			}

			/// <summary>
			/// Calculating slope position
			/// </summary>
			/// <param name="baseL">Left Y</param>
			/// <param name="baseR">Right Y</param>
			/// <param name="otherL">Other Left Y</param>
			/// <param name="otherR">Other Right Y</param>
			/// <returns>Delta in (0-1) range</returns>
			float GetFloorIntersection(float baseL, float baseR, float otherL, float otherR) {
				double d = (otherR - otherL) - (baseR - baseL);
				double n_a = (baseL - otherL);
				if (d == 0)
					return 0.5f;

				double ua = n_a / d;
				return (float)ua;
			}

			/// <summary>
			/// Searching for block
			/// </summary>
			/// <param name="x">X coord</param>
			/// <param name="y">Y coord</param>
			/// <returns>Block or null</returns>
			Block GetBlock(int x, int y) {
				Chunk c = this;
				if (x < 0) {
					if (c != null) {
						x += BLOCKS;
						c = c.neighbors[(int)Side.Left];
					} else {
						return null;
					}
				}
				if (y < 0) {
					if (c != null) {
						y += BLOCKS;
						c = c.neighbors[(int)Side.Back];
					} else {
						return null;
					}
				}
				if (x >= BLOCKS) {
					if (c != null) {
						x -= BLOCKS;
						c = c.neighbors[(int)Side.Right];
					} else {
						return null;
					}
				}
				if (y >= BLOCKS) {
					if (c != null) {
						y -= BLOCKS;
						c = c.neighbors[(int)Side.Forward];
					} else {
						return null;
					}
				}
				if (c != null) {
					return c.blocks[y, x];
				}
				return null;
			}

			/// <summary>
			/// Rebuilding light texture
			/// </summary>
			void RecalculateLight() {
				// Pushing state
				GL.PushAttrib(AttribMask.AllAttribBits);

				// Storing matrices
				Matrix4 projMatrix = Matrix4.CreateOrthographicOffCenter(0, BLOCKS, BLOCKS, 0, -1, 1);
				GL.MatrixMode(MatrixMode.Projection);
				GL.PushMatrix();
				GL.LoadMatrix(ref projMatrix);
				GL.MatrixMode(MatrixMode.Modelview);
				GL.PushMatrix();
				GL.LoadIdentity();

				/*
				Matrix4 oldCam = ShaderSystem.CameraMatrix;
				Matrix4 oldProj = ShaderSystem.ProjectionMatrix;
				ShaderSystem.EntityMatrix = Matrix4.Identity;
				ShaderSystem.CameraMatrix = Matrix4.Identity;
				ShaderSystem.ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, BLOCKS, BLOCKS, 0, -1, 1);
				GL.MatrixMode(MatrixMode.Projection);
				GL.PushMatrix();
				GL.LoadMatrix(ref ShaderSystem.ProjectionMatrix);
				GL.MatrixMode(MatrixMode.Modelview);
				GL.PushMatrix();
				GL.LoadIdentity();
				GL.Disable(EnableCap.DepthTest);
				*/
				 
				// Preparing render texture
				GL.Enable(EnableCap.Texture2D);
				if (lightingTex == 0 || !GL.IsTexture(lightingTex)) {
					lightingTex = GL.GenTexture();
					GL.BindTexture(TextureTarget.Texture2D, lightingTex);
					GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, LIGHTMAP_SIZE, LIGHTMAP_SIZE, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Clamp);
					GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Clamp);
				} else {
					GL.BindTexture(TextureTarget.Texture2D, lightingTex);
				}

				// Preparing framebuffer
				if (frameBuffer == 0 || !GL.IsFramebuffer(frameBuffer)) {
					frameBuffer = GL.GenFramebuffer();
				}
				GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
				GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, lightingTex, 0);
				GL.BindTexture(TextureTarget.Texture2D, 0);
				
				// Processing lights
				GL.ClearColor(0, 0, 0, 1);
				GL.Viewport(new Size(LIGHTMAP_SIZE, LIGHTMAP_SIZE));
				GL.Clear(ClearBufferMask.ColorBufferBit);
				GL.Disable(EnableCap.CullFace);

				/*
				GL.Begin(PrimitiveType.Quads);
				GL.Color4(Color.Green);
				GL.Vertex2(0, 0);
				GL.Color4(Color.Red);
				GL.Vertex2(8, 0);
				GL.Color4(Color.Blue);
				GL.Vertex2(8, 8);
				GL.Color4(Color.Yellow);
				GL.Vertex2(0, 8);
				GL.End();
				 */
				// Rendering light one by one
				GL.Enable(EnableCap.Blend);
				GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);		
				GL.Enable(EnableCap.Texture2D);
				foreach (Light l in affectedLights) {
					Vector2 pos = new Vector2(l.Position.X - Location.X * BLOCKS, l.Position.Z - Location.Z * BLOCKS);

					GL.BindTexture(TextureTarget.Texture2D, l.textureBuffer);
					GL.Begin(PrimitiveType.Quads);
					GL.Color4(Color.White);
					float tf = l.textureFactor;
					GL.TexCoord2(0.5f - tf * 0.5f, 0.5f + tf * 0.5f);
					GL.Vertex2(pos.X - l.Range, (pos.Y - l.Range));
					GL.TexCoord2(0.5f + tf * 0.5f, 0.5f + tf * 0.5f);
					GL.Vertex2(pos.X + l.Range, (pos.Y - l.Range));
					GL.TexCoord2(0.5f + tf * 0.5f, 0.5f - tf * 0.5f);
					GL.Vertex2(pos.X + l.Range, (pos.Y + l.Range));
					GL.TexCoord2(0.5f - tf * 0.5f, 0.5f - tf * 0.5f);
					GL.Vertex2(pos.X - l.Range, (pos.Y + l.Range));
					GL.End();
				}
				GL.Disable(EnableCap.Blend);

				// Releasing buffer
				GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

				// Restoring matrices
				/*
				ShaderSystem.CameraMatrix = oldCam;
				ShaderSystem.ProjectionMatrix = oldProj;
				*/
				GL.MatrixMode(MatrixMode.Projection);
				GL.PopMatrix(); 
				GL.MatrixMode(MatrixMode.Modelview);
				GL.PopMatrix();

				// Returning attributes
				GL.PopAttrib();
				lightCache = null;
			}

			/// <summary>
			/// Recalculate all obstructors
			/// </summary>
			void RebuildObstructors() {

				// Clearing
				obstructors.Clear();

				// Creating blocks
				for (int y = 0; y < BLOCKS; y++) {
					for (int x = 0; x < BLOCKS; x++) {
						if (blocks[y, x] is WallBlock) {
							obstructors.Add(LightObstructor.ForRectangle(x, y, 1, 1));
						}
					}
				}

			}

			/// <summary>
			/// Renderable group
			/// </summary>
			protected class RenderGroup {

				/// <summary>
				/// Vertex buffer ID
				/// </summary>
				public int VertexBuffer {
					get;
					set;
				}

				/// <summary>
				/// Index buffer ID
				/// </summary>
				public int IndexBuffer {
					get;
					set;
				}

				/// <summary>
				/// Texture coordinates buffer ID
				/// </summary>
				public int TexCoordBuffer {
					get;
					set;
				}

				/// <summary>
				/// Lightmap coordinates buffer ID
				/// </summary>
				public int LightmapCoordBuffer {
					get;
					set;
				}

				/// <summary>
				/// Total index count
				/// </summary>
				public int IndexCount {
					get;
					set;
				}
			}
		}

		/// <summary>
		/// Single cell block
		/// </summary>
		public abstract class Block {

			/// <summary>
			/// Parent
			/// </summary>
			internal Chunk Parent {
				get;
				set;
			}

		}

		/// <summary>
		/// Wall cell
		/// </summary>
		public class WallBlock : Block {

			/// <summary>
			/// Accessing wall textures
			/// </summary>
			/// <param name="side"></param>
			/// <returns></returns>
			public Texture this[Side side] {
				get {
					return textures[(int)side];
				}
				set {
					textures[(int)side] = value;
					if (Parent != null) {
						Parent.QueueRebuild();
					}
				}
			}

			/// <summary>
			/// Hidden textures
			/// </summary>
			Texture[] textures;
				
			/// <summary>
			/// Wall block constructor
			/// </summary>
			public WallBlock() {
				textures = new Texture[6];
			}
		}

		/// <summary>
		/// Floor and ceiling block
		/// </summary>
		public class FloorBlock : Block {

			/// <summary>
			/// Contstructor
			/// </summary>
			public FloorBlock()
				: base() {
					floorHeights = new float[4];
					ceilHeights = new float[4];
					floorArray = new TextureArray(this);
					ceilingArray = new TextureArray(this);
			}

			/// <summary>
			/// Floor texture
			/// </summary>
			public Texture Floor {
				get {
					return floor;
				}
				set {
					if (value != floor) {
						floor = value;
						if (Parent != null) {
							Parent.QueueRebuild();
						}
					}
				}
			}

			/// <summary>
			/// Ceiling texture
			/// </summary>
			public Texture Ceiling {
				get {
					return ceiling;
				}
				set {
					if (value != ceiling) {
						ceiling = value;
						if (Parent != null) {
							Parent.QueueRebuild();
						}
					}
				}
			}

			/// <summary>
			/// Floor trim textures
			/// </summary>
			public TextureArray FloorTrim {
				get {
					return floorArray;
				}
			}

			/// <summary>
			/// Ceiling trim textures
			/// </summary>
			public TextureArray CeilingTrim {
				get {
					return ceilingArray;
				}
			}

			/// <summary>
			/// Floor flag
			/// </summary>
			public bool HasFloor {
				get {
					return hasFloor;
				}
				set {
					if (value != hasFloor) {
						hasFloor = value;
						if (Parent != null) {
							Parent.QueueRebuild();
						}
					}
				}
			}

			/// <summary>
			/// Ceiling flag
			/// </summary>
			public bool HasCeiling {
				get {
					return hasCeiling;
				}
				set {
					if (value != hasCeiling) {
						hasCeiling = value;
						if (Parent != null) {
							Parent.QueueRebuild();
						}
					}
				}
			}

			/// <summary>
			/// Floor height
			/// </summary>
			public float[] FloorHeight {
				get {
					float[] ret = new float[4];
					for (int i = 0; i < 4; i++) {
						ret[i] = floorHeights[i];
					}
					return ret;
				}
				set {
					if (value == null || value.Length != 4) {
						throw new Exception("Incorrect data for heightmap (must be array of 4 items)");
					}
					for (int i = 0; i < 4; i++) {
						if (floorHeights[i] != value[i]) {
							floorHeights[i] = value[i];
							if (Parent != null) {
								Parent.QueueRebuild();
							}
						}
					}
				}
			}

			/// <summary>
			/// Floor height
			/// </summary>
			public float[] CeilingHeight {
				get {
					float[] ret = new float[4];
					for (int i = 0; i < 4; i++) {
						ret[i] = ceilHeights[i];
					}
					return ret;
				}
				set {
					if (value == null || value.Length != 4) {
						throw new Exception("Incorrect data for heightmap (must be array of 4 items)");
					}
					for (int i = 0; i < 4; i++) {
						if (ceilHeights[i] != value[i]) {
							ceilHeights[i] = value[i];
							if (Parent != null) {
								Parent.QueueRebuild();
							}
						}
					}
				}
			}

			/// <summary>
			/// Floor and ceiling textures
			/// </summary>
			Texture floor, ceiling;
			
			/// <summary>
			/// Flags for floor and ceiling
			/// </summary>
			bool hasFloor, hasCeiling;

			/// <summary>
			/// Floor height values
			/// </summary>
			float[] floorHeights;

			/// <summary>
			/// Ceiling height values
			/// </summary>
			float[] ceilHeights;

			/// <summary>
			/// Trim textures
			/// </summary>
			TextureArray floorArray, ceilingArray;

			/// <summary>
			/// Textures arrays
			/// </summary>
			public class TextureArray {

				/// <summary>
				/// Texture for floor or ceiling sides
				/// </summary>
				/// <param name="s">Side</param>
				/// <returns>Associated texture</returns>
				public Texture this[Side s] {
					get {
						return textures[(int)s];
					}
					set{
						if (textures[(int)s] != value) {
							textures[(int)s] = value;
							if (parent.Parent != null) {
								parent.Parent.QueueRebuild();
							}
						}
					}
				}

				/// <summary>
				/// Texture array
				/// </summary>
				Texture[] textures;

				/// <summary>
				/// Parental chunk
				/// </summary>
				FloorBlock parent;

				/// <summary>
				/// Internal constructor
				/// </summary>
				/// <param name="c">Parent block</param>
				internal TextureArray(FloorBlock c) {
					textures = new Texture[4] {
						null, null, null, null	
					};
					parent = c;
				}
			}
		}
		
		/// <summary>
		/// Light barrier
		/// </summary>
		internal class LightObstructor {

			/// <summary>
			/// Array of points
			/// </summary>
			public List<Vector2> Points {
				get;
				private set;
			}

			/// <summary>
			/// Obstructor constructor
			/// </summary>
			public LightObstructor() {
				Points = new List<Vector2>();
			}

			/// <summary>
			/// Create obstructor for rectangle
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <param name="w"></param>
			/// <param name="h"></param>
			/// <returns></returns>
			public static LightObstructor ForRectangle(float x, float y, float w, float h) {
				LightObstructor o = new LightObstructor();
				o.Points.Add(new Vector2(x, y));
				o.Points.Add(new Vector2(x+w, y));
				o.Points.Add(new Vector2(x+w, y+h));
				o.Points.Add(new Vector2(x, y+h));
				return o;
			}
		}

		/// <summary>
		/// Chunk side
		/// </summary>
		public enum Side : byte {
			/// <summary>
			/// Z+
			/// </summary>
			Forward = 0,
			/// <summary>
			/// X+
			/// </summary>
			Right = 1,
			/// <summary>
			/// Z-
			/// </summary>
			Back = 2,
			/// <summary>
			/// X-
			/// </summary>
			Left = 3,
			/// <summary>
			/// Y+
			/// </summary>
			Top = 4,
			/// <summary>
			/// Y-
			/// </summary>
			Bottom = 5
		}
	}
}
