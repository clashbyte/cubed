using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Cubed.Core;
using Cubed.Data.Rendering;
using Cubed.Data.Shaders;
using Cubed.Graphics;
using Cubed.Maths;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Cubed.World {
	
	/// <summary>
	/// Single level
	/// </summary>
	public class Map {

		/// <summary>
		/// Milliseconds for all lights
		/// </summary>
		const int LIGHTS_QUOTA_MS = 4;

		/// <summary>
		/// Milliseconds for all chunks
		/// </summary>
		const int CHUNKS_QUOTA_MS = 4;

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
						c.needRelightStatic = true;
					}
				}
			}
		}

		/// <summary>
		/// Light for sun
		/// </summary>
		public DirectionalLight Sunlight {
			get;
			set;
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
		/// Lights update quota
		/// </summary>
		int lightUpdateQuota = 32;

		/// <summary>
		/// Chunks to relight quota
		/// </summary>
		int chunkRelightQuota = 32;

		/// <summary>
		/// Queue for light update
		/// </summary>
		Queue<Light> lightUpdateQueue;

		/// <summary>
		/// Queue for chunks light update
		/// </summary>
		Queue<Chunk> chunkUpdateQueue;

		/// <summary>
		/// Previous lights
		/// </summary>
		Light[] lastAffectedLights;

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
			lightUpdateQueue = new Queue<Light>();
			chunkUpdateQueue = new Queue<Chunk>();
			lastAffectedLights = new Light[0];
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
					chunks[hash].needRebuild = true;
				}
			}
			if (tx == Chunk.BLOCKS - 1) {
				hash = Hash(cx + 1, cy, cz);
				if (!chunks.ContainsKey(hash)) {
					Chunk ch = new Chunk();
					chunks.Add(hash, ch);
					ch.SetParent(this, cx + 1, cy, cz);
				} else {
					chunks[hash].needRebuild = true;
				}
			}
			if (ty == 0) {
				hash = Hash(cx, cy, cz - 1);
				if (!chunks.ContainsKey(hash)) {
					Chunk ch = new Chunk();
					chunks.Add(hash, ch);
					ch.SetParent(this, cx, cy, cz - 1);
				} else {
					chunks[hash].needRebuild = true;
				}
			}
			if (ty == Chunk.BLOCKS - 1) {
				hash = Hash(cx, cy, cz + 1);
				if (!chunks.ContainsKey(hash)) {
					Chunk ch = new Chunk();
					chunks.Add(hash, ch);
					ch.SetParent(this, cx, cy, cz + 1);
				} else {
					chunks[hash].needRebuild = true;
				}
			}

			// Setting block
			chunk[tx, ty] = block;
			chunk.needRebuild = true;
		}

		/// <summary>
		/// Get light level
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public Color GetLightLevel(float x, float y, float z) {
			float r = ambient.R;
			float g = ambient.G;
			float b = ambient.B;

			// Updating colors
			Vector3 loc = new Vector3(x, y, z);
			foreach (Light light in lastAffectedLights) {
				Vector3 dir = light.LastUpdatePoint - loc;
				float rng = light.Range;
				float dist = dir.Length;
				if (dist <= rng) {
					bool allow = true;
					if (light.Shadows) {
						MapIntersections.Hit hit = MapIntersections.Intersect(loc, dir.Normalized(), this);
						if (hit != null) {
							float len = (loc - hit.Point).LengthFast;
							if (len < dist) {
								allow = false;
							}
						}
					}
					if (allow) {
						float mul = 1f - dist / rng;
						r += (float)light.Color.R * mul;
						g += (float)light.Color.G * mul;
						b += (float)light.Color.B * mul;
					}
				}
			}

			// Getting color
			return Color.FromArgb((byte)Math.Min(r, 255), (byte)Math.Min(g, 255), (byte)Math.Min(b, 255));
		}

		/// <summary>
		/// Render map
		/// </summary>
		internal void Render(Scene parent) {
			Parent = parent;
			Caps.CheckErrors();
			foreach (KeyValuePair<string, Chunk> item in chunks) {
				item.Value.Render();
			}
			Caps.CheckErrors();
		}

		/// <summary>
		/// Rebuilding and relighting
		/// </summary>
		internal void Update(List<Light> lights, Scene parent) {
			Parent = parent;
			lastAffectedLights = lights.ToArray();
			CheckLighting(lights);
		}

		/// <summary>
		/// Get all the chunks
		/// </summary>
		/// <returns></returns>
		public Chunk[] GetAllChunks() {
			return chunks.Values.ToArray();
		}

		/// <summary>
		/// Checking all the chunks for rebuild
		/// </summary>
		void CheckLighting(IEnumerable<Light> lights) {

			// Updating queue
			Stopwatch sw = new Stopwatch();

			// Checking all chunks for rebuilding
			foreach (KeyValuePair<string, Chunk> pair in chunks) {
				Chunk chunk = pair.Value;
				chunk.CheckLights(lights);
				if (chunk.needRebuild) {
					chunk.RebuildGeometry();
					chunk.needRebuild = false;
					chunk.needRelightStatic = true;
					chunk.needRelightDynamic = true;
				}
			}
			Caps.CheckErrors();

			// Rebuilding directional sunlight
			if (Sunlight != null && Parent != null) {
				Sunlight.Update(Parent.Camera);
				Sunlight.Render(this);
			}
			Caps.CheckErrors();

			// Checking light queue
			foreach (Light light in lights) {
				if (light.IsChanged && !lightUpdateQueue.Contains(light)) {
					lightUpdateQueue.Enqueue(light);
				}
			}
			Caps.CheckErrors();

			// Updating lights in queue
			int updatedLights = 0;
			int lightLimit = lightUpdateQuota;
			sw.Restart();
			while (lightLimit > 0 && lightUpdateQueue.Count > 0) {
				Light light = lightUpdateQueue.Dequeue();
				light.RebuildTexture(this);
				light.Cleanup();

				lightLimit--;
				updatedLights++;
			}
			sw.Stop();
			Caps.CheckErrors();

			// Recalculating light quota
			if (sw.ElapsedMilliseconds > LIGHTS_QUOTA_MS && lightUpdateQuota > 1) {
				long allowedTicks = TimeSpan.TicksPerMillisecond * LIGHTS_QUOTA_MS;
				long ticks = sw.ElapsedTicks;
				long ticksPerLight = ticks / updatedLights;
				lightUpdateQuota = (int)Math.Max(allowedTicks / ticksPerLight, 1);
			}
			
			// Relight all needed chunks
			foreach (KeyValuePair<string, Chunk> pair in chunks) {
				Chunk chunk = pair.Value;
				if ((chunk.needRelightStatic || chunk.needRelightDynamic) && !chunkUpdateQueue.Contains(chunk)) {
					chunkUpdateQueue.Enqueue(chunk);
				}
			}

			// Updating lights in queue
			int updatedChunks = 0;
			int chunkLimit = chunkRelightQuota;
			sw.Restart();
			while (chunkLimit > 0 && chunkUpdateQueue.Count > 0) {
				Chunk chunk = chunkUpdateQueue.Dequeue();
				chunk.RebuildLighting();
				chunk.needRelightStatic = false;
				chunk.needRelightDynamic = false;
				
				chunkLimit--;
				updatedChunks++;
			}
			sw.Stop();
			Caps.CheckErrors();

			// Recalculating chunk quota
			if (sw.ElapsedMilliseconds > CHUNKS_QUOTA_MS && chunkRelightQuota > 1) {
				long allowedTicks = TimeSpan.TicksPerMillisecond * CHUNKS_QUOTA_MS;
				long ticks = sw.ElapsedTicks;
				long ticksPerChunk = ticks / updatedChunks;
				chunkRelightQuota = (int)Math.Max(allowedTicks / ticksPerChunk, 1);
			}
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
			internal bool needRebuild = false;

			/// <summary>
			/// Chunk needs to be relight in static
			/// </summary>
			internal bool needRelightStatic = false;

			/// <summary>
			/// Chunk needs to be relight in dynamic
			/// </summary>
			internal bool needRelightDynamic = false;

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
			/// Composite render group of all other groups
			/// </summary>
			RenderGroup shadowGroup;

			/// <summary>
			/// Texture of lighting
			/// </summary>
			internal int staticLightmap, dynamicLightmap;

			/// <summary>
			/// Lightmap dimensions
			/// </summary>
			int lightmapWidth, lightmapHeight;

			/// <summary>
			/// Framebuffer
			/// </summary>
			static int frameBuffer;

			/// <summary>
			/// Lights that touches this chunk
			/// </summary>
			List<Light> affectedLights;

			/// <summary>
			/// Chunk constructor
			/// </summary>
			public Chunk() {
				neighbors = new Chunk[6];
				blocks = new Block[Chunk.BLOCKS, Chunk.BLOCKS];
				groups = new Dictionary<Texture, RenderGroup>();
				affectedLights = new List<Light>();
				nullGroup = null;
				needRebuild = true;
				needRelightStatic = true;
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
						neighbors[(int)Side.Left].needRebuild = true;
					}
					if (x == BLOCKS - 1 && neighbors[(int)Side.Right] != null) {
						neighbors[(int)Side.Right].needRebuild = true;
					}
					if (y == 0 && neighbors[(int)Side.Back] != null) {
						neighbors[(int)Side.Back].needRebuild = true;
					}
					if (y == BLOCKS - 1 && neighbors[(int)Side.Forward] != null) {
						neighbors[(int)Side.Forward].needRebuild = true;
					}
					if (neighbors[(int)Side.Top] != null) {
						neighbors[(int)Side.Top].needRebuild = true;
					}
					if (neighbors[(int)Side.Bottom] != null) {
						neighbors[(int)Side.Bottom].needRebuild = true;
					}

					needRebuild = true;
					needRelightStatic = true;
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
				needRebuild = true;
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
			/// Check for light and chunk intersection
			/// </summary>
			/// <param name="l">Light</param>
			/// <returns>True if intersection exist</returns>
			internal bool TouchesLight(Light l) {
				if (!(l.Position.Y + l.Range <= Location.Y || l.Position.Y - l.Range > Location.Y + 1f)) {
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
					c.needRebuild = true;
				}
				if (!forget) {
					neighbors[side] = c;
				} else {
					neighbors[side] = null;
				}
				needRebuild = true;
			}

			/// <summary>
			/// Checking light data
			/// </summary>
			/// <param name="lights">Current light list</param>
			internal void CheckLights(IEnumerable<Light> lights) {
				foreach (Light l1 in lights) {
					if (TouchesLight(l1) && !affectedLights.Contains(l1)) {
						affectedLights.Add(l1);
						if (l1.Static) {
							needRelightStatic = true;
						} else {
							needRelightDynamic = true;
						}
					}
				}
				List<Light> tempLights = new List<Light>(affectedLights);
				foreach (Light l in tempLights) {
					if (!TouchesLight(l) || !lights.Contains(l)) {
						affectedLights.Remove(l);
						if (l.Static) {
							needRelightStatic = true;
						} else {
							needRelightDynamic = true;
						}
					}
				}
				foreach (Light l2 in affectedLights) {
					if (l2.IsChanged) {
						if (l2.Static) {
							needRelightStatic = true;
						} else {
							needRelightDynamic = true;
						}
					}
				}
			}

			/// <summary>
			/// Rendering
			/// </summary>
			internal void Render() {
				if (IsVisible()) {
					ShaderSystem.EntityMatrix = matrix;

					// Binding texture
					Caps.CheckErrors();
					GL.ActiveTexture(TextureUnit.Texture1);
					GL.Enable(EnableCap.Texture2D);
					GL.BindTexture(TextureTarget.Texture2D, staticLightmap);
					GL.ActiveTexture(TextureUnit.Texture2);
					GL.Enable(EnableCap.Texture2D);
					GL.BindTexture(TextureTarget.Texture2D, dynamicLightmap);
					GL.ActiveTexture(TextureUnit.Texture3);
					GL.Enable(EnableCap.Texture2D);
					Caps.CheckErrors();
					if (Parent.Sunlight != null) {
						GL.BindTexture(TextureTarget.Texture2D, Parent.Sunlight.Texture);
						MapShader.Shader.SunColor = Parent.Sunlight.Color;
						MapShader.Shader.SunMatrix = Parent.Sunlight.FullMatrix;
						Caps.CheckErrors();
					} else {
						MapShader.Shader.SunColor = Color.Black;
						MapShader.Shader.SunMatrix = Matrix4.Identity;
						Texture.BindEmpty();
						Caps.CheckErrors();
					}
					GL.ActiveTexture(TextureUnit.Texture0);
					Caps.CheckErrors();

					// Drawing surfaces
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
					Caps.CheckErrors();
					MapShader.Shader.Bind();
					foreach (KeyValuePair<Texture, RenderGroup> group in groups) {
						RenderSingle(group.Key, group.Value);
						Caps.CheckErrors();
					}
					RenderSingle(null, nullGroup);
					Caps.CheckErrors();

					// Unbinding
					MapShader.Shader.Unbind();
					Caps.CheckErrors();
					GL.ActiveTexture(TextureUnit.Texture1);
					GL.BindTexture(TextureTarget.Texture2D, 0);
					GL.Disable(EnableCap.Texture2D);
					GL.ActiveTexture(TextureUnit.Texture2);
					GL.BindTexture(TextureTarget.Texture2D, 0);
					GL.Disable(EnableCap.Texture2D);
					GL.ActiveTexture(TextureUnit.Texture3);
					Texture.BindEmpty();
					GL.Disable(EnableCap.Texture2D);
					GL.ActiveTexture(TextureUnit.Texture0);
					Caps.CheckErrors();
				}
			}

			/// <summary>
			/// Checkig for camera
			/// </summary>
			/// <returns>True if chunk is in view</returns>
			internal bool IsVisible() {
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

						Caps.CheckErrors();
						MapShader shader = MapShader.Shader;
						GL.BindBuffer(BufferTarget.ArrayBuffer, rg.VertexBuffer);
						GL.VertexAttribPointer(shader.VertexBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
						if (shader.NormalsBufferLocation > -1) {
							GL.BindBuffer(BufferTarget.ArrayBuffer, rg.NormalBuffer);
							GL.VertexAttribPointer(shader.NormalsBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
						}
						GL.BindBuffer(BufferTarget.ArrayBuffer, rg.TexCoordBuffer);
						GL.VertexAttribPointer(shader.TexCoordBufferLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
						GL.BindBuffer(BufferTarget.ArrayBuffer, rg.LightmapCoordBuffer);
						GL.VertexAttribPointer(shader.LightTexCoordBufferLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
						GL.BindBuffer(BufferTarget.ElementArrayBuffer, rg.IndexBuffer);
						Caps.CheckErrors();
						GL.DrawElements(PrimitiveType.Triangles, rg.IndexCount, DrawElementsType.UnsignedShort, 0);
						Caps.CheckErrors();
						GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
						GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
						Caps.CheckErrors();

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
			/// Render shadow group
			/// </summary>
			internal void RenderShadow() {
				MapShadowShader shader = MapShadowShader.Shader;
				GL.BindBuffer(BufferTarget.ArrayBuffer, shadowGroup.VertexBuffer);
				GL.VertexAttribPointer(shader.VertexBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, shadowGroup.IndexBuffer);
				GL.DrawElements(PrimitiveType.Triangles, shadowGroup.IndexCount, DrawElementsType.UnsignedShort, 0);
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			}

			/// <summary>
			/// Get chunk matrix
			/// </summary>
			/// <returns>Matrix</returns>
			internal Matrix4 GetMatrix() {
				return matrix;
			}

			/// <summary>
			/// Rebuilding geometry
			/// </summary>
			internal void RebuildGeometry() {

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

				// Prepare group list
				List<MapTriangulator.FaceGroup> faceGroups = new List<MapTriangulator.FaceGroup>();

				// Rebuilding surfaces
				foreach (Texture tex in textures) {

					// Face group
					MapTriangulator.FaceGroup faceGroup = new MapTriangulator.FaceGroup();

					// Composing blocks
					for (int y = 0; y < BLOCKS; y++) {
						for (int x = 0; x < BLOCKS; x++) {
							Block b = blocks[y, x];
							float div = 1f / (float)BLOCKS;
							float cx = (float)x * div, cy = 1f - (float)y * div;
							MapTriangulator.Face face = null;

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
										face = new MapTriangulator.Face() {
											Coords = new float[]{
												x, floorHeight[0], -y,
												x + 1, floorHeight[1], -y,
												x, floorHeight[2], -y - 1,
												x + 1, floorHeight[3], -y - 1
											},
											LightCoords = new float[] {
												0, 0,
												1, 0,
												0, 1,
												1, 1
											},
											TexCoords = new float[]{
												1f, 0f,
												0f, 0f,
												1f, 1f,
												0f, 1f
											},
											Indices = new ushort[] {
												0, 1, 2,
												1, 3, 2
											}
										};
										faceGroup.Faces.Add(face);
									}
								}
								if (fb.HasCeiling) {
									ceilingHeight = fb.CeilingHeight;
									if (fb.Ceiling == tex) {
										face = new MapTriangulator.Face() {
											Coords = new float[]{
												x + 1, 1 - ceilingHeight[0], -y,
												x, 1 - ceilingHeight[1], -y,
												x + 1, 1 - ceilingHeight[2], -y - 1,
												x, 1 - ceilingHeight[3], -y - 1
											},
											LightCoords = new float[] {
												0, 0,
												1, 0,
												0, 1,
												1, 1
											},
											TexCoords = new float[]{
												1f, 0f,
												0f, 0f,
												1f, 1f,
												0f, 1f
											},
											Indices = new ushort[] {
												0, 1, 2,
												1, 3, 2
											}
										};
										faceGroup.Faces.Add(face);
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
													0, height[0],
													1, height[1],
													0, height[2],
													1, height[3]
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
													0, height[0],
													1, height[1],
													0, height[2],
													1, height[3]
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
													0, height[0],
													1, height[1],
													0, height[2],
													1, height[3]
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
													0, height[0],
													1, height[1],
													0, height[2],
													1, height[3]
												};
												break;
											
										}

										// Adding surfaces
										if (lightUV != null && vertXYZ != null) {
											face = new MapTriangulator.Face() {
												Coords = vertXYZ,
												LightCoords = lightUV,
												TexCoords = new float[]{
													0f, 1f - height[0],
													1f, 1f - height[1],
													0f, 1f - height[2],
													1f, 1f - height[3]
												},
												Indices = new ushort[] {
													0, 2, 1,
													1, 2, 3
												}
											};
											faceGroup.Faces.Add(face);
										}
									}	

									// Floor trimming wall
									if (needFloorWall || needCeilWall) {
										float[] ofh = fnb.FloorHeight;
										float[] och = fnb.CeilingHeight;
										float[] baseCoords = null;

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
												break;
												
										}
										
										// Checking height
										if (needFloorWall && (hFL > baseFL || hFR > baseFR)) {
											if (hFL > baseFL && hFR > baseFR) {
												face = new MapTriangulator.Face() {
													Coords = new float[]{
														baseCoords[0], hFL, baseCoords[1],
														baseCoords[2], hFR, baseCoords[3],
														baseCoords[0], baseFL, baseCoords[1],
														baseCoords[2], baseFR, baseCoords[3],
													},
													TexCoords = new float[]{
														0f, 1f - hFL,
														1f, 1f - hFR,
														0f, 1f - baseFL,
														1f, 1f - baseFR
													},
													LightCoords = new float[]{
														0f, 1f - hFL,
														1f, 1f - hFR,
														0f, 1f - baseFL,
														1f, 1f - baseFR
													},
													Indices = new ushort[] {
														0, 2, 1,
														1, 2, 3
													}
												};
												faceGroup.Faces.Add(face);
											} else {
												float delta = GetFloorIntersection(baseFL, baseFR, hFL, hFR);
												if (hFL > baseFL) {
													face = new MapTriangulator.Face() {
														Coords = new float[]{
															baseCoords[0], baseFL, baseCoords[1],
															baseCoords[0], hFL, baseCoords[1],
															baseCoords[0] + (baseCoords[2] - baseCoords[0]) * delta,
															baseFL + (baseFR - baseFL) * delta,
															baseCoords[1] + (baseCoords[3] - baseCoords[1]) * delta
														},
														TexCoords = new float[]{
															0f, 1f - baseFL,
															0f, 1f - hFL,
															delta, 1f - (baseFL + (baseFR - baseFL) * delta)
														},
														LightCoords = new float[]{
															0f, 1f - baseFL,
															0f, 1f - hFL,
															delta, 1f - (baseFL + (baseFR - baseFL) * delta)
														},
														Indices = new ushort[] {
															0, 2, 1
														}
													};
													faceGroup.Faces.Add(face);
												} else {
													face = new MapTriangulator.Face() {
														Coords = new float[]{
															baseCoords[2], hFR, baseCoords[3],
															baseCoords[2], baseFR, baseCoords[3],
															baseCoords[0] + (baseCoords[2] - baseCoords[0]) * delta,
															baseFL + (baseFR - baseFL) * delta,
															baseCoords[1] + (baseCoords[3] - baseCoords[1]) * delta
														},
														TexCoords = new float[]{
															1f, 1f - hFR,
															1f, 1f - baseFR,
															delta, 1f - (baseFL + (baseFR - baseFL) * delta)
														},
														LightCoords = new float[]{
															1f, 1f - hFR,
															1f, 1f - baseFR,
															delta, 1f - (baseFL + (baseFR - baseFL) * delta)
														},
														Indices = new ushort[] {
															0, 2, 1
														}
													};
													faceGroup.Faces.Add(face);
												}
											}
										}
										
										if (needCeilWall && (hCL > baseCL || hCR > baseCR)) {
											if (hCL > baseCL && hCR > baseCR) {
												face = new MapTriangulator.Face() {
													Coords = new float[]{
														baseCoords[0], 1f - baseCL, baseCoords[1],
														baseCoords[2], 1f - baseCR, baseCoords[3],
														baseCoords[0], 1f - hCL, baseCoords[1],
														baseCoords[2], 1f - hCR, baseCoords[3],
													},
													TexCoords = new float[]{
														0f, baseCL,
														1f, baseCR,
														0f, hCL,
														1f, hCR
													},
													LightCoords = new float[]{
														0f, baseCL,
														1f, baseCR,
														0f, hCL,
														1f, hCR
													},
													Indices = new ushort[] {
														0, 2, 1,
														1, 2, 3
													}
												};
												faceGroup.Faces.Add(face);
											} else {
												float delta = GetFloorIntersection(baseCL, baseCR, hCL, hCR);
												if (hCL > baseCL) {
													face = new MapTriangulator.Face() {
														Coords = new float[]{
															baseCoords[0], 1f - hCL, baseCoords[1],
															baseCoords[0], 1f- baseCL, baseCoords[1],
															baseCoords[0] + (baseCoords[2] - baseCoords[0]) * delta,
															1f - (baseCL + (baseCR - baseCL) * delta),
															baseCoords[1] + (baseCoords[3] - baseCoords[1]) * delta
														},
														TexCoords = new float[]{
															0f, hCL,
															0f, baseCL,
															delta, (baseCL + (baseCR - baseCL) * delta)
														},
														LightCoords = new float[]{
															0f, hCL,
															0f, baseCL,
															delta, (baseCL + (baseCR - baseCL) * delta)
														},
														Indices = new ushort[] {
															0, 2, 1
														}
													};
													faceGroup.Faces.Add(face);
												} else {
													face = new MapTriangulator.Face() {
														Coords = new float[]{
															baseCoords[2], 1f - baseCR, baseCoords[3],
															baseCoords[2], 1f - hCR, baseCoords[3],
															baseCoords[0] + (baseCoords[2] - baseCoords[0]) * delta,
															1f - (baseCL + (baseCR - baseCL) * delta),
															baseCoords[1] + (baseCoords[3] - baseCoords[1]) * delta
														},
														TexCoords = new float[]{
															1f, baseCR,
															1f, hCR,
															delta, (baseCL + (baseCR - baseCL) * delta)
														},
														LightCoords = new float[]{
															1f, baseCR,
															1f, hCR,
															delta, (baseCL + (baseCR - baseCL) * delta)
														},
														Indices = new ushort[] {
															0, 2, 1
														}
													};
													faceGroup.Faces.Add(face);
												}
											}
										}
									}
								}
							}
						}
					}

					// Saving face group
					faceGroup.Texture = tex;
					faceGroups.Add(faceGroup);
				}

				// Making lightmap texture
				int lightSX = 0, lightSY = 0;
				MapTriangulator.CalculateLightCoords(faceGroups, out lightSX, out lightSY);
				lightmapWidth = lightSX;
				lightmapHeight = lightSY;

				// Compositing big mesh occluder
				if (shadowGroup == null) {
					shadowGroup = new RenderGroup();
				}
				FillRenderGroup(shadowGroup, faceGroups);

				// Converting to render groups
				foreach (MapTriangulator.FaceGroup fg in faceGroups) {

					// Creating group
					Texture tex = fg.Texture;
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
					FillRenderGroup(rg, new MapTriangulator.FaceGroup[] { fg });
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
						if (rg.NormalBuffer != 0) {
							GL.DeleteBuffer(rg.NormalBuffer);
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

				// Flagging lights to rebuild
				foreach (Light light in affectedLights) {
					light.MakeDirty();
					light.Static = light.Static;
				}
			}

			/// <summary>
			/// Rebuilding lights
			/// </summary>
			internal void RebuildLighting() {
				if (lightmapHeight == 0 || lightmapWidth == 0) {
					return;
				}
				if (needRelightStatic) {
					RebuildLightLayer(affectedLights, true, ref staticLightmap, lightmapWidth, lightmapHeight);
					Caps.CheckErrors();
				}
				if (needRelightDynamic) {
					RebuildLightLayer(affectedLights, false, ref dynamicLightmap, lightmapWidth, lightmapHeight);
					Caps.CheckErrors();
				}
			}

			/// <summary>
			/// Rebuild single light layer
			/// </summary>
			/// <param name="lights">Array of lights</param>
			/// <param name="isStatic">Flag for static lights</param>
			/// <param name="texWidth">Lightmap width</param>
			/// <param name="texHeight">Lightmap height</param>
			void RebuildLightLayer(IEnumerable<Light> lights, bool isStatic, ref int glTex, int texWidth, int texHeight) {

				// Checking internal buffers
				if (!GL.IsBuffer(shadowGroup.IndexBuffer) || !GL.IsBuffer(shadowGroup.VertexBuffer) || !GL.IsBuffer(shadowGroup.NormalBuffer) || !GL.IsBuffer(shadowGroup.LightmapCoordBuffer)) {
					return;
				}

				// Saving matrices
				Matrix4 camMat = ShaderSystem.CameraMatrix;
				Matrix4 entMat = ShaderSystem.EntityMatrix;
				Matrix4 prjMat = ShaderSystem.ProjectionMatrix;
				Matrix4 texMat = ShaderSystem.TextureMatrix;

				// Checking framebuffer
				if (frameBuffer == 0 || !GL.IsFramebuffer(frameBuffer)) {
					frameBuffer = GL.GenFramebuffer();
				}
				GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
				Caps.CheckErrors();

				// Checking texture
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.Enable(EnableCap.Texture2D);
				if (glTex == 0 || !GL.IsTexture(glTex)) {
					glTex = GL.GenTexture();
				}
				GL.BindTexture(TextureTarget.Texture2D, glTex);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Three, texWidth, texHeight, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
				GL.BindTexture(TextureTarget.Texture2D, 0);
				GL.Disable(EnableCap.Texture2D);

				// Adding attachments
				GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, glTex, 0);

				Caps.CheckErrors();

				GL.ActiveTexture(TextureUnit.Texture0);
				GL.Disable(EnableCap.DepthTest);

				// Rendering scene
				GL.Viewport(0, 0, texWidth, texHeight);
				GL.Enable(EnableCap.Blend);
				GL.ClearColor(0, 0, 0, 0);
				GL.Clear(ClearBufferMask.ColorBufferBit);
				GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
				Caps.CheckErrors();
				
				// Binding shader
				ShaderSystem.CameraMatrix = Matrix4.Identity;
				ShaderSystem.EntityMatrix = matrix;
				ShaderSystem.ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(0, 1, 0, 1, -1, 2);
				MapLightPassShader shader = MapLightPassShader.Shader;
				Caps.CheckErrors();

				shader.Bind();
				GL.BindBuffer(BufferTarget.ArrayBuffer, shadowGroup.VertexBuffer);
				GL.VertexAttribPointer(shader.VertexBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
				GL.BindBuffer(BufferTarget.ArrayBuffer, shadowGroup.NormalBuffer);
				GL.VertexAttribPointer(shader.NormalBufferLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
				GL.BindBuffer(BufferTarget.ArrayBuffer, shadowGroup.LightmapCoordBuffer);
				GL.VertexAttribPointer(shader.TexBufferLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, shadowGroup.IndexBuffer);
				shader.Unbind();

				// Enabling textures
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.Enable(EnableCap.TextureCubeMap);
				GL.ActiveTexture(TextureUnit.Texture1);
				GL.Enable(EnableCap.Texture2D);
				Caps.CheckErrors();

				// Rendering lights
				foreach (Light light in lights) {
					if (light.Static == isStatic) {
						Vector3 pos = light.LastUpdatePoint;
						pos.Z = -pos.Z;

						GL.ActiveTexture(TextureUnit.Texture0);
						GL.BindTexture(TextureTarget.TextureCubeMap, light.DepthTextureBuffer);
						GL.ActiveTexture(TextureUnit.Texture1);
						if (light.Texture != null && light.Texture.State == Texture.LoadingState.Complete) {
							light.Texture.Bind();
						} else {
							Texture.BindEmpty();
						}
						Caps.CheckErrors();

						shader.LightColor = light.Color;
						shader.LightRange = light.Range;
						shader.LightPos = pos;
						shader.LightRotation = light.TextureAngle;

						shader.Bind();
						Caps.CheckErrors();
						GL.DrawElements(PrimitiveType.Triangles, shadowGroup.IndexCount, DrawElementsType.UnsignedShort, 0);
						Caps.CheckErrors();
						shader.Unbind();
					}
				}

				// Unbinding data
				GL.Disable(EnableCap.Blend);
				GL.Enable(EnableCap.DepthTest);
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
				GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
				Caps.CheckErrors();

				// Unbinding textures
				GL.ActiveTexture(TextureUnit.Texture1);
				GL.BindTexture(TextureTarget.Texture2D, 0);
				GL.Disable(EnableCap.Texture2D);
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.TextureCubeMap, 0);
				GL.Disable(EnableCap.TextureCubeMap);

				// Restoring matrices
				ShaderSystem.CameraMatrix = camMat;
				ShaderSystem.EntityMatrix = entMat;
				ShaderSystem.ProjectionMatrix = prjMat;
				ShaderSystem.TextureMatrix = texMat;
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
			/// Converting face group to rendergroup
			/// </summary>
			/// <param name="rg">Target group</param>
			/// <param name="group">Face group</param>
			void FillRenderGroup(RenderGroup rg, IEnumerable<MapTriangulator.FaceGroup> groups) {

				// Triangulating
				List<MapTriangulator.FaceGroup> faceGroups = new List<MapTriangulator.FaceGroup>(groups);
				float[] coords = null, texCoords = null, lightCoords = null, normals = null;
				ushort[] indices = null;
				int total = 0;
				if (faceGroups.Count > 1) {
					total = MapTriangulator.TriangulateMerged(faceGroups, out coords, out texCoords, out lightCoords, out normals, out indices);
				} else {
					total = faceGroups[0].Triangulate(out coords, out texCoords, out lightCoords, out normals, out indices);
				}

				// Sending to GL
				if (rg.VertexBuffer == 0 || !GL.IsBuffer(rg.VertexBuffer)) {
					rg.VertexBuffer = GL.GenBuffer();
				}
				if (rg.NormalBuffer == 0 || !GL.IsBuffer(rg.NormalBuffer)) {
					rg.NormalBuffer = GL.GenBuffer();
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
				rg.IndexCount = total;
				GL.BindBuffer(BufferTarget.ArrayBuffer, rg.VertexBuffer);
				GL.BufferData(BufferTarget.ArrayBuffer, coords.Length * 4, coords, BufferUsageHint.StaticDraw);
				GL.BindBuffer(BufferTarget.ArrayBuffer, rg.NormalBuffer);
				GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * 4, normals, BufferUsageHint.StaticDraw);
				GL.BindBuffer(BufferTarget.ArrayBuffer, rg.TexCoordBuffer);
				GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Length * 4, texCoords, BufferUsageHint.StaticDraw);
				GL.BindBuffer(BufferTarget.ArrayBuffer, rg.LightmapCoordBuffer);
				GL.BufferData(BufferTarget.ArrayBuffer, lightCoords.Length * 4, lightCoords, BufferUsageHint.StaticDraw);
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, rg.IndexBuffer);
				GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * 2, indices, BufferUsageHint.StaticDraw);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
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
				/// Normals buffer ID
				/// </summary>
				public int NormalBuffer {
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
						Parent.needRebuild = true;
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
							Parent.needRebuild = true;
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
							Parent.needRebuild = true;
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
							Parent.needRebuild = true;
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
							Parent.needRebuild = true;
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
								Parent.needRebuild = true;
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
								Parent.needRebuild = true;
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
								parent.Parent.needRebuild = true;
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
		/// Directional light class
		/// </summary>
		public class DirectionalLight {

			/// <summary>
			/// Color of this light
			/// </summary>
			public Color Color {
				get;
				set;
			}

			/// <summary>
			/// Light angles
			/// </summary>
			public Vector2 Angles {
				get;
				set;
			}

			/// <summary>
			/// Projection matrix
			/// </summary>
			internal Matrix4 Projection {
				get;
				private set;
			}

			/// <summary>
			/// Model view matrix
			/// </summary>
			internal Matrix4 ModelView {
				get;
				private set;
			}

			/// <summary>
			/// Fully composed matrix
			/// </summary>
			internal Matrix4 FullMatrix {
				get;
				private set;
			}

			/// <summary>
			/// Texture buffer
			/// </summary>
			internal int Texture;

			/// <summary>
			/// Updating all matrices
			/// </summary>
			internal void Update(Camera camera) {
				Vector3 pos = camera.Position;
				pos.X = (float)Math.Round(pos.X);
				pos.Y = (float)Math.Round(pos.Y);
				pos.Z = (float)Math.Round(pos.Z);

				Projection = Matrix4.CreateOrthographic(64, 64, 0, 128);
				ModelView =
					Matrix4.CreateTranslation(-Vector3.UnitZ * 64) *
					Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-Angles.X)) *
					Matrix4.CreateRotationY(MathHelper.DegreesToRadians(-Angles.Y)) *
					Matrix4.CreateTranslation(pos);
				ModelView.Invert();

				FullMatrix = Projection * ModelView;
			}

			/// <summary>
			/// Rendering 
			/// </summary>
			internal void Render(Map map) {

				// Retrieving chunks
				Chunk[] chunks = map.GetAllChunks();

				// Setting up matrix
				List<Chunk> candidates = new List<Chunk>();
				Frustum.Setup(Projection, ModelView);
				foreach (Chunk chunk in chunks) {
					if (chunk.IsVisible()) {
						candidates.Add(chunk);
					}
				}

				// Rendering buffer
				Light.RebuildDirectionalTexture(candidates, ref Texture, 256);
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
