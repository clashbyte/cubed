using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Components;
using Cubed.Components.Rendering;
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
		/// Get light level
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		internal Color GetLightLevel(float x, float y, float z) {
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
			}
			return ambient;
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
					l.UpdateForMap(this);
					if (i == 1) {
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
		internal Chunk[] GetAllChunks() {
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
				dirty = true;
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
				if (lightingTex != 0) {

					y = (float)Chunk.BLOCKS - y;
					int cx = (int)Math.Floor(x / (float)Chunk.BLOCKS * (float)Chunk.LIGHTMAP_SIZE);
					int cy = (int)Math.Floor(y / (float)Chunk.BLOCKS * (float)Chunk.LIGHTMAP_SIZE);
					byte[] data = new byte[4];
					Color color = Color.Black;

					//GL.Enable(EnableCap.Texture2D);
					GL.BindTexture(TextureTarget.Texture2D, lightingTex);
					GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
					GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, lightingTex, 0);
					GL.BindTexture(TextureTarget.Texture2D, 0);
					GL.ReadPixels(cx, cy, 1, 1, PixelFormat.Rgb, PixelType.UnsignedByte, data);
					GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
					//GL.Disable(EnableCap.Texture2D);
					return Color.FromArgb(data[0], data[1], data[2]);
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
			internal void Update(List<Light> lights, bool forceRebuild = false) {
				foreach (Light l1 in lights) {
					if (!affectedLights.Contains(l1)) {
						affectedLights.Add(l1);
						relight = true;
					}
				}
				List<Light> tempLights = new List<Light>(affectedLights);
				foreach (Light l in tempLights) {
					if (!lights.Contains(l)) {
						affectedLights.Remove(l);
						relight = true;
					}
				}
				if (dirty || forceRebuild) {
					RebuildGeometry();
					RebuildObstructors();
					relight = true;
					dirty = false;
				}
				if (relight) {
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
									bool need = false;
									if (nb != null && nb is WallBlock) {
										if ((nb as WallBlock)[opposite] == tex) {
											need = true;
										}
									}
									if (need) {
										float[] lightUV = null;
										float[] vertXYZ = null;
										float bias = 0.001f;//1f / (float)(LIGHTMAP_SIZE * 2f);

										switch (side) {


											// Forward side
											case Side.Forward:
												vertXYZ = new float[] {
													x,		1, -y - 1,
													x + 1,	1, -y - 1, 
													x,		0, -y - 1,
													x + 1,	0, -y - 1
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
												vertXYZ = new float[] {
													x +	1,	1, -y,
													x,		1, -y, 
													x + 1,	0, -y,
													x,		0, -y
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
												vertXYZ = new float[] {
													x + 1,	1, -y - 1,
													x + 1,	1, -y, 
													x + 1,	0, -y - 1,
													x + 1,	0, -y
												};
												lightUV = new float[] {
													cx + div - bias,	cy - div,
													cx + div - bias,	cy,
													cx + div - bias,	cy - div,
													cx + div - bias,	cy,
												};
												break;

											// 
											case Side.Left:
												vertXYZ = new float[] {
													x,	1, -y,
													x,	1, -y - 1, 
													x,	0, -y,
													x,	0, -y - 1
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
												0f, 0f,
												1f, 0f,
												0f, 1f,
												1f, 1f
											});
											indices.AddRange(new short[]{
												(short)(idx + 0), (short)(idx + 2), (short)(idx + 1),
												(short)(idx + 1), (short)(idx + 2), (short)(idx + 3)
											});
											idx += 4;
										}
									}	
								}
							}

							// Building floors and ceilings
							if (b is FloorBlock) {
								FloorBlock fb = b as FloorBlock;
								if (fb.HasFloor && fb.Floor == tex) {
									vertCoords.AddRange(new float[]{
										x, 0, -y,
										x + 1, 0, -y,
										x, 0, -y - 1,
										x + 1, 0, -y - 1
									});
									lightCoords.AddRange(new float[]{
										cx, cy,
										cx + div, cy,
										cx, cy - div,
										cx + div, cy - div
									});
									texCoords.AddRange(new float[]{
										0f, 0f,
										1f, 0f,
										0f, 1f,
										1f, 1f
									});
									indices.AddRange(new short[]{
										(short)(idx + 0), (short)(idx + 1), (short)(idx + 2),
										(short)(idx + 1), (short)(idx + 3), (short)(idx + 2)
									});
									idx += 4;
								}
								if (fb.HasCeiling && fb.Ceiling == tex) {
									vertCoords.AddRange(new float[]{
										x + 1, 1, -y,
										x, 1, -y,
										x + 1, 1, -y - 1,
										x, 1, -y - 1
									});
									lightCoords.AddRange(new float[]{
										cx + div, cy,
										cx, cy,
										cx + div, cy - div,
										cx, cy - div
									});
									texCoords.AddRange(new float[]{
										0f, 0f,
										1f, 0f,
										0f, 1f,
										1f, 1f
									});
									indices.AddRange(new short[]{
										(short)(idx + 0), (short)(idx + 1), (short)(idx + 2),
										(short)(idx + 1), (short)(idx + 3), (short)(idx + 2)
									});
									idx += 4;
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
					GL.TexCoord2(0, l.textureFactor);
					GL.Vertex2(pos.X - l.Range, (pos.Y - l.Range));
					GL.TexCoord2(l.textureFactor, l.textureFactor);
					GL.Vertex2(pos.X + l.Range, (pos.Y - l.Range));
					GL.TexCoord2(l.textureFactor, 0);
					GL.Vertex2(pos.X + l.Range, (pos.Y + l.Range));
					GL.TexCoord2(0, 0);
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
			/// Floor and ceiling textures
			/// </summary>
			Texture floor, ceiling;
			
			/// <summary>
			/// Flags for floor and ceiling
			/// </summary>
			bool hasFloor, hasCeiling;

			/// <summary>
			/// Height values
			/// </summary>
			float[] heights;
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
