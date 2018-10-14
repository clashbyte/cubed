using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace Cubed.Maths {

	/// <summary>
	/// Calculating intersection with map
	/// </summary>
	public static class MapIntersections {
		
		/// <summary>
		/// Calculating map intersection
		/// </summary>
		/// <param name="pos">Ray position</param>
		/// <param name="dir">Ray direction</param>
		/// <param name="map"></param>
		/// <returns>Hit point or null</returns>
		public static Hit Intersect(Vector3 pos, Vector3 dir, World.Map map) {
			Hit hit = null;
			float dist = float.MaxValue;
			World.Map.Chunk[] chunks = map.GetAllChunks();
			foreach (World.Map.Chunk chunk in chunks) {
				float hdist = 0;
				Vector3 rpos = new Vector3(chunk.Location.X * World.Map.Chunk.BLOCKS, chunk.Location.Y, chunk.Location.Z * World.Map.Chunk.BLOCKS);
				Vector3 rend = rpos + new Vector3(World.Map.Chunk.BLOCKS, 1, World.Map.Chunk.BLOCKS);
				if (Intersections.RayIntersectsBox(pos, dir, rpos, rend, out hdist)) {
					if (hdist < dist) {

						// Colliding with blocks
						for (int y = 0; y < World.Map.Chunk.BLOCKS; y++) {
							for (int x = 0; x < World.Map.Chunk.BLOCKS; x++) {

								World.Map.Block block = chunk[x, y];
								if (block is World.Map.WallBlock) {
								
									// Checking wall
									Vector3 bpos = new Vector3(chunk.Location.X * World.Map.Chunk.BLOCKS + x, chunk.Location.Y, chunk.Location.Z * World.Map.Chunk.BLOCKS + y);
									float wdist = 0;
									if (Intersections.RayIntersectsBox(pos, dir, bpos + Vector3.UnitY * 0.001f, bpos + new Vector3(1, 0.999f, 1), out wdist)) {
										if (wdist < dist) {
											hit = new Hit();
											hit.Point = pos + dir * wdist;
											hit.Cell = bpos;
											hit.Block = block;
											hit.Type = HitType.Wall;
											hit.Chunk = chunk;
											
											// Calculating side;
											Vector3 localHit = hit.Point - (bpos + Vector3.One * 0.5f);
											float absx = Math.Abs(localHit.X);
											float absy = Math.Abs(localHit.Y);
											float absz = Math.Abs(localHit.Z);
											float tmax = Math.Max(Math.Max(absx, absy), absz);
											if (tmax == absx) {
												hit.Side = localHit.X < 0 ? World.Map.Side.Left : World.Map.Side.Right;
											} else if (tmax == absy) {
												hit.Side = localHit.Y < 0 ? World.Map.Side.Bottom : World.Map.Side.Top;
											} else {
												hit.Side = localHit.Z < 0 ? World.Map.Side.Back : World.Map.Side.Forward;
											}

											// Storing current distance
											dist = wdist;
										}
									}

								} else if (block is World.Map.FloorBlock) {

									// Checking temp collision with current cell
									Vector3 bpos = new Vector3(chunk.Location.X * World.Map.Chunk.BLOCKS + x, chunk.Location.Y, chunk.Location.Z * World.Map.Chunk.BLOCKS + y);
									float bldist = 0;
									if (Intersections.RayIntersectsBox(pos, dir, bpos, bpos + Vector3.One, out bldist)) {
										if (bldist < dist) {
											World.Map.FloorBlock fb = block as World.Map.FloorBlock;

											// Checking floor
											if (fb.HasFloor) {
												for (int i = 0; i < 5; i++) {
													Vector3[] floorVerts = GetFloorVerts(fb, bpos, (World.Map.Side)i);
													for (int v = 0; v < floorVerts.Length; v+=3) {
														float fdist = 0;
														if (Intersections.RayIntersectsTriangle(pos, dir, floorVerts[v], floorVerts[v + 1], floorVerts[v + 2], out fdist)) {
															if (fdist < dist) {
																hit = new Hit();
																hit.Point = pos + dir * fdist;
																hit.Cell = bpos;
																hit.Block = block;
																hit.Type = i == 4 ? HitType.Floor : HitType.FloorTrim;
																hit.Chunk = chunk;
																hit.Side = (World.Map.Side)i;
																dist = fdist;
															}
														}
													}
												}
											}

											// Checking ceiling
											if (fb.HasCeiling) {
												for (int i = 0; i < 6; i++) {
													if (i == 4) {
														continue;
													}
													Vector3[] ceilVerts = GetCeilVerts(fb, bpos, (World.Map.Side)i);
													for (int v = 0; v < ceilVerts.Length; v += 3) {
														float cdist = 0;
														if (Intersections.RayIntersectsTriangle(pos, dir, ceilVerts[v], ceilVerts[v + 1], ceilVerts[v + 2], out cdist)) {
															if (cdist < dist) {
																hit = new Hit();
																hit.Point = pos + dir * cdist;
																hit.Cell = bpos;
																hit.Block = block;
																hit.Type = i == 5 ? HitType.Ceiling : HitType.CeilingTrim;
																hit.Chunk = chunk;
																hit.Side = (World.Map.Side)i;
																dist = cdist;
															}
														}
													}
												}
											}


											/*
											hit = new Hit();
											hit.Point = pos + dir * wdist;
											hit.Cell = bpos;
											hit.Block = block;
											hit.Type = HitType.Wall;
											hit.Chunk = chunk;

											// Calculating side;
											Vector3 localHit = hit.Point - (bpos + Vector3.One * 0.5f);
											float absx = Math.Abs(localHit.X);
											float absy = Math.Abs(localHit.Y);
											float absz = Math.Abs(localHit.Z);
											float tmax = Math.Max(Math.Max(absx, absy), absz);
											if (tmax == absx) {
												hit.Side = localHit.X < 0 ? World.Map.Side.Left : World.Map.Side.Right;
											} else if (tmax == absy) {
												hit.Side = localHit.Y < 0 ? World.Map.Side.Bottom : World.Map.Side.Top;
											} else {
												hit.Side = localHit.Z < 0 ? World.Map.Side.Back : World.Map.Side.Forward;
											}

											// Storing current distance
											dist = wdist;
											 */




										}
									}

								}

							}
						}
					}
				}
			}
			return hit;
		}

		/// <summary>
		/// Calculating vertices for floor
		/// </summary>
		/// <param name="block">Data block</param>
		/// <param name="blockPos">Block location</param>
		/// <param name="side">Side to calculate</param>
		/// <returns>Array of vertices</returns>
		static Vector3[] GetFloorVerts(World.Map.FloorBlock block, Vector3 blockPos, World.Map.Side side) {

			// Calculating vertices
			List<Vector3> verts = new List<Vector3>();

			if (side == World.Map.Side.Top) {
				// Top side
				verts.AddRange(new Vector3[] {
					new Vector3(0, block.FloorHeight[0], 0),
					new Vector3(1, block.FloorHeight[1], 0),
					new Vector3(0, block.FloorHeight[2], 1),
					new Vector3(1, block.FloorHeight[1], 0),
					new Vector3(1, block.FloorHeight[3], 1),
					new Vector3(0, block.FloorHeight[2], 1),
				});

			} else {
				// Other sides
				Vector3 vh1 = Vector3.Zero, vh2 = Vector3.Zero;
				switch (side) {
					case Cubed.World.Map.Side.Forward:
						vh1 = new Vector3(0, block.FloorHeight[2], 1);
						vh2 = new Vector3(1, block.FloorHeight[3], 1);
						break;
					case Cubed.World.Map.Side.Right:
						vh1 = new Vector3(1, block.FloorHeight[3], 1);
						vh2 = new Vector3(1, block.FloorHeight[1], 0);
						break;
					case Cubed.World.Map.Side.Back:
						vh1 = new Vector3(1, block.FloorHeight[1], 0);
						vh2 = new Vector3(0, block.FloorHeight[0], 0);
						break;
					case Cubed.World.Map.Side.Left:
						vh1 = new Vector3(0, block.FloorHeight[0], 0);
						vh2 = new Vector3(0, block.FloorHeight[2], 1);
						break;
				}

				// Making one or two triangles
				if (vh1.Y > 0 || vh2.Y > 0) {
					Vector3 mask = new Vector3(1, 0, 1);
					verts.Add(vh1);
					verts.Add(vh2);
					if (vh1.Y > 0) {
						verts.Add(vh1 * mask);
					}
					if (vh2.Y > 0) {
						verts.Add(vh2 * mask);
						if (vh1.Y > 0) {
							verts.Add(vh1 * mask);
							verts.Add(vh2);
						}
					}
				}
			}

			// Handling output
			Vector3[] output = verts.ToArray();
			for (int i = 0; i < output.Length; i++) {
				output[i] += blockPos;
			}
			return output;
		}

		/// <summary>
		/// Calculating vertices for ceiling
		/// </summary>
		/// <param name="block">Data block</param>
		/// <param name="blockPos">Block location</param>
		/// <param name="side">Side to calculate</param>
		/// <returns>Array of vertices</returns>
		static Vector3[] GetCeilVerts(World.Map.FloorBlock block, Vector3 blockPos, World.Map.Side side) {

			// Calculating vertices
			List<Vector3> verts = new List<Vector3>();

			if (side == World.Map.Side.Bottom) {
				// Top sides
				verts.AddRange(new Vector3[] {
					new Vector3(1, 1f - block.CeilingHeight[0] - 0.001f, 0),
					new Vector3(0, 1f - block.CeilingHeight[1] - 0.001f, 0),
					new Vector3(1, 1f - block.CeilingHeight[2] - 0.001f, 1),
					new Vector3(0, 1f - block.CeilingHeight[1] - 0.001f, 0),
					new Vector3(0, 1f - block.CeilingHeight[3] - 0.001f, 1),
					new Vector3(1, 1f - block.CeilingHeight[2] - 0.001f, 1),
				});

			} else {
				// Other sides
				Vector3 vh1 = Vector3.Zero, vh2 = Vector3.Zero;
				switch (side) {
					case Cubed.World.Map.Side.Forward:
						vh1 = new Vector3(0, 1f - block.CeilingHeight[3], 1);
						vh2 = new Vector3(1, 1f - block.CeilingHeight[2], 1);
						break;
					case Cubed.World.Map.Side.Right:
						vh1 = new Vector3(1, 1f - block.CeilingHeight[2], 1);
						vh2 = new Vector3(1, 1f - block.CeilingHeight[0], 0);
						break;
					case Cubed.World.Map.Side.Back:
						vh1 = new Vector3(1, 1f - block.CeilingHeight[0], 0);
						vh2 = new Vector3(0, 1f - block.CeilingHeight[1], 0);
						break;
					case Cubed.World.Map.Side.Left:
						vh1 = new Vector3(0, 1f - block.CeilingHeight[1], 0);
						vh2 = new Vector3(0, 1f - block.CeilingHeight[3], 1);
						break;
				}

				// Making one or two triangles
				if (vh1.Y < 1 || vh2.Y < 1) {
					Vector3 mask = new Vector3(1, 0, 1);
					verts.Add(vh1);
					verts.Add(vh2);
					if (vh1.Y < 1) {
						verts.Add(new Vector3(vh1.X, 1, vh1.Z));
					}
					if (vh2.Y < 1) {
						verts.Add(new Vector3(vh2.X, 1, vh2.Z));
						if (vh1.Y < 1) {
							verts.Add(new Vector3(vh1.X, 1, vh1.Z));
							verts.Add(vh2);
						}
					}
				}
			}

			// Handling output
			Vector3[] output = verts.ToArray();
			for (int i = 0; i < output.Length; i++) {
				output[i] += blockPos;
			}
			return output;
		}

		
		/// <summary>
		/// Hit result
		/// </summary>
		public class Hit {

			/// <summary>
			/// Intersection point
			/// </summary>
			public Vector3 Point;

			/// <summary>
			/// Cell global coords
			/// </summary>
			public Vector3 Cell;

			/// <summary>
			/// Block
			/// </summary>
			public World.Map.Block Block;

			/// <summary>
			/// Chunk
			/// </summary>
			public World.Map.Chunk Chunk;

			/// <summary>
			/// Side
			/// </summary>
			public World.Map.Side Side;

			/// <summary>
			/// Pick type
			/// </summary>
			public HitType Type;
		}

		/// <summary>
		/// Hit type
		/// </summary>
		public enum HitType {
			Wall,
			Floor,
			FloorTrim,
			Ceiling,
			CeilingTrim
		}

	}
}
