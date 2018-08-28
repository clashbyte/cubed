using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Core;
using OpenTK;

namespace Cubed.World {

	/// <summary>
	/// Dynamical collider
	/// </summary>
	public class Collider {

		/// <summary>
		/// Collider size
		/// </summary>
		public Vector3 Size {
			get {
				return size;
			}
			set {
				size = value;
			}
		}

		/// <summary>
		/// Moving velocity
		/// </summary>
		public Vector3 Velocity {
			get {
				return velocity;
			}
			set {
				velocity = value;
			}
		}

		/// <summary>
		/// Collision response
		/// </summary>
		public Vector3 Response {
			get {
				return response;
			}
		}

		/// <summary>
		/// AABB size
		/// </summary>
		protected Vector3 size;

		/// <summary>
		/// Body velocity
		/// </summary>
		protected Vector3 velocity;

		/// <summary>
		/// Velocity response
		/// </summary>
		protected Vector3 response;

		/// <summary>
		/// Enable collision with other AABB
		/// </summary>
		protected bool collideWithOther;

		/// <summary>
		/// Enable collision with world
		/// </summary>
		protected bool collideWithWorld;

		/// <summary>
		/// Владелец
		/// </summary>
		internal Entity owner;

		public Collider() {
			collideWithOther = true;
			collideWithWorld = true;
		}

		/// <summary>
		/// Resetting all data
		/// </summary>
		internal void Reset() {
			response = Vector3.Zero;
		}

		/// <summary>
		/// Collisions calculation
		/// </summary>
		/// <returns>True if box collided</returns>
		internal bool Collide(Map map, Collider[] others) {
			response = Vector3.Zero;
			if (velocity != Vector3.Zero && owner != null) {

				// Size
				float bw = size.X / 2f;
				float bh = size.Y / 2f;
				float bd = size.Z / 2f;
				bool collided = false;
				float epsilon = 0.000001f;
				Vector3 start = owner.Position;
				Vector3 result = start + velocity;
				Vector3 finish = result;
				Vector3 bsz = new Vector3(bw, bh, bd);

				// Colliding with map
				if (map != null)
				{
					// Colliding horizontal
					if (velocity.Z != 0) {
						int steps = (int)Math.Ceiling(Math.Abs(velocity.Z) / bd);
						for (int step = 0; step <= steps; step++) {
							Vector3 target = new Vector3(0, 0, bd * Math.Sign(velocity.Z) * step);
							if (Math.Abs(target.Z) > Math.Abs(velocity.Z)) {
								target.Z = velocity.Z;
							}
							target += start;

							int sx = (int)Math.Floor(target.X - bw);
							int ex = (int)Math.Floor(target.X + bw);
							int sy = (int)Math.Floor(target.Y - bh);
							int ey = (int)Math.Floor(target.Y + bh);
							int sz = (int)Math.Floor(target.Z - bd);
							int ez = (int)Math.Floor(target.Z + bd);
							bool forw = velocity.Z > 0;
							float barrier = forw ? ez + 1f : sz;
							bool hit = false;
							int z = forw ? ez : sz;

							// Calculating collision with map
							if (collideWithWorld && map != null) {
								for (int x = sx; x <= ex; x++) {
									for (int y = sy; y <= ey; y++) {
										Map.Block blk = map.GetBlockAtCoords(x, y, z);
										if (blk != null) {
											if (blk is Map.WallBlock) {
												barrier = z;
												if (!forw) {
													barrier += 1f;
												}
												hit = true;
											} else if (blk is Map.FloorBlock) {
												Map.FloorBlock mfb = blk as Map.FloorBlock;
												Vector3 ps = new Vector3(target.X - (float)x, target.Y - (float)y, target.Z - (float)z);
												if (y + 1f > target.Y && mfb.HasCeiling) {
													float dist = 0f;
													if (CollideCeilVertical(ps, bsz, mfb.CeilingHeight, forw, out dist)) {
														dist += z;
														if (forw) {
															barrier = Math.Min(dist, barrier);
														} else {
															barrier = Math.Max(dist, barrier);
														}
														hit = true;
													}
												}
												if (y < target.Y && mfb.HasFloor) {
													float dist = 0f;
													if (CollideFloorVertical(ps, bsz, mfb.FloorHeight, forw, out dist)) {
														dist += z;
														if (forw) {
															barrier = Math.Min(dist, barrier);
														} else {
															barrier = Math.Max(dist, barrier);
														}
														hit = true;
													}
												}
											}
										}
									}
								}
							}

							// Colliding with other AABBs
							if (collideWithOther) {
								foreach (Collider col in others) {
									if (col != this) {
										Vector3 opos = col.owner.Position;
										Vector3 obd = col.size / 2f;
										if (!(
											((opos.X + obd.X) < (start.X - bw)) || 
											((opos.X - obd.X) > (start.X + bw)) || 
											((opos.Y + obd.Y) < (start.Y - bh)) ||
											((opos.Y - obd.Y) > (start.Y + bh)))) 
										{
											if (forw) {
												if (start.Z < opos.Z && (target.Z + bd) > opos.Z - obd.Z) {
													hit = true;
													barrier = Math.Min(opos.Z - obd.Z, barrier);
												}
											} else {
												if (start.Z > opos.Z && (target.Z - bd) < opos.Z + obd.Z) {
													hit = true;
													barrier = Math.Max(opos.Z + obd.Z, barrier);
												}
											}
										}
									}
								}
							}

							// Pushing body out
							if (hit) {
								if (forw) {
									if (target.Z + bd > barrier) {
										result.Z = barrier - bd - epsilon;
										velocity.Z = 0;
										collided = true;
										break;
									}
								} else {
									if (target.Z - bd < barrier) {
										result.Z = barrier + bd + epsilon;
										velocity.Z = 0;
										collided = true;
										break;
									}
								}
							}
						}
					}

					// Colliding vertical
					if (velocity.X != 0) {
						int steps = (int)Math.Ceiling(Math.Abs(velocity.X) / bw);
						for (int step = 0; step <= steps; step++) {
							Vector3 target = new Vector3(bw * Math.Sign(velocity.X) * step, 0, 0);
							if (Math.Abs(target.X) > Math.Abs(velocity.X)) {
								target.X = velocity.X;
							}
							target += start;

							int sx = (int)Math.Floor(target.X - bw);
							int ex = (int)Math.Floor(target.X + bw);
							int sy = (int)Math.Floor(target.Y - bh);
							int ey = (int)Math.Floor(target.Y + bh);
							int sz = (int)Math.Floor(target.Z - bd);
							int ez = (int)Math.Floor(target.Z + bd);
							bool right = velocity.X > 0;
							float barrier = right ? ex + 1f : sx;
							bool hit = false;
							int x = right ? ex : sx;

							// Colliding with map
							if (collideWithWorld && map != null) {
								for (int z = sz; z <= ez; z++) {
									for (int y = sy; y <= ey; y++) {
										Map.Block blk = map.GetBlockAtCoords(x, y, z);
										if (blk != null) {
											if (blk is Map.WallBlock) {
												barrier = x;
												if (!right) {
													barrier += 1f;
												}
												hit = true;
											} else if (blk is Map.FloorBlock) {
												Map.FloorBlock mfb = blk as Map.FloorBlock;
												Vector3 ps = new Vector3(target.X - (float)x, target.Y - (float)y, target.Z - (float)z);
												if (y + 1f > target.Y && mfb.HasCeiling) {
													float dist = 0f;
													if (CollideCeilHorizontal(ps, bsz, mfb.CeilingHeight, right, out dist)) {
														dist += x;
														if (right) {
															barrier = Math.Min(dist, barrier);
														} else {
															barrier = Math.Max(dist, barrier);
														}
														hit = true;
													}
												}
												if (y < target.Y && mfb.HasFloor) {
													float dist = 0f;
													if (CollideFloorHorizontal(ps, bsz, mfb.FloorHeight, right, out dist)) {
														dist += x;
														if (right) {
															barrier = Math.Min(dist, barrier);
														} else {
															barrier = Math.Max(dist, barrier);
														}
														hit = true;
													}
												}
											}
										}
									}
								}
							}

							// Colliding with other AABBs
							if (collideWithOther) {
								foreach (Collider col in others) {
									if (col != this) {
										Vector3 opos = col.owner.Position;
										Vector3 obd = col.size / 2f;
										if (!(
											((opos.Z + obd.Z) < (start.Z - bd)) ||
											((opos.Z - obd.Z) > (start.Z + bd)) ||
											((opos.Y + obd.Y) < (start.Y - bh)) ||
											((opos.Y - obd.Y) > (start.Y + bh)))) {
											if (right) {
												if (start.X < opos.X && (target.X + bw) > opos.X - obd.X) {
													hit = true;
													barrier = Math.Min(opos.X - obd.X, barrier);
												}
											} else {
												if (start.X > opos.X && (target.X - bw) < opos.X + obd.X) {
													hit = true;
													barrier = Math.Max(opos.X + obd.X, barrier);
												}
											}
										}
									}
								}
							}

							// Pushing body out
							if (hit) {
								if (right) {
									if (target.X + bd > barrier) {
										result.X = barrier - bd - epsilon;
										velocity.X = 0;
										collided = true;
										break;
									}
								} else {
									if (target.X - bd < barrier) {
										result.X = barrier + bd + epsilon;
										velocity.X = 0;
										collided = true;
										break;
									}
								}
							}
						}
					}

					// Colliding in height
					if (velocity.Y != 0) {
						int steps = (int)Math.Ceiling(Math.Abs(velocity.Y) / bh);
						for (int step = 0; step <= steps; step++) {
							Vector3 target = new Vector3(0, bh * Math.Sign(velocity.Y) * step, 0);
							if (Math.Abs(target.Y) > Math.Abs(velocity.Y)) {
								target.Y = velocity.Y;
							}
							target += start;
							int sx = (int)Math.Floor(target.X - bw);
							int ex = (int)Math.Floor(target.X + bw);
							int sy = (int)Math.Floor(target.Y - bh);
							int ey = (int)Math.Floor(target.Y + bh);
							int sz = (int)Math.Floor(target.Z - bd);
							int ez = (int)Math.Floor(target.Z + bd);
							float ceilh = float.MaxValue;
							float floorh = float.MinValue;
							bool up = velocity.Y > 0;

							// Colliding with floor
							if (collideWithWorld && map != null) {
								for (int z = sz; z <= ez; z++) {
									for (int x = sx; x <= ex; x++) {
										for (int h = sy; h <= ey; h++) {
											Map.Block blk = map.GetBlockAtCoords(x, h, z);
											if (blk != null) {
												if (blk is Map.WallBlock) {
													if (velocity.Y > 0) {
														ceilh = Math.Min(ceilh, h + 1);
													} else {
														floorh = Math.Max(floorh, h);
													}
												} else if (blk is Map.FloorBlock) {
													Map.FloorBlock mfb = blk as Map.FloorBlock;
													if (mfb.HasFloor && h < target.Y) {
														floorh = Math.Max(
															floorh,
															GetMaxFloorHeight(new Vector2(target.X - x, target.Z - z), new Vector2(bw, bd), mfb.FloorHeight) + h
														);
													}
													if (mfb.HasCeiling && h + 1 > target.Y) {
														ceilh = Math.Min(
															ceilh,
															1f - GetMaxCeilingHeight(new Vector2(target.X - x, target.Z - z), new Vector2(bw, bd), mfb.CeilingHeight) + h
														);
													}
												}
											}
										}
									}
								}
							}

							// Collide with other AABBs
							if (collideWithOther) {
								foreach (Collider col in others) {
									if (col != this) {
										Vector3 opos = col.owner.Position;
										Vector3 obd = col.size / 2f;
										if (!(
											((opos.Z + obd.Z) < (start.Z - bd)) ||
											((opos.Z - obd.Z) > (start.Z + bd)) ||
											((opos.X + obd.X) < (start.X - bw)) ||
											((opos.X - obd.X) > (start.X + bw)))) {
											if (up) {
												if (start.Y < opos.Y && (target.Y + bh) > opos.Y - obd.Y) {
													ceilh = Math.Min(opos.Y - obd.Y, ceilh);
												}
											} else {
												if (start.Y > opos.Y && (target.Y - bh) < opos.Y + obd.Y) {
													floorh = Math.Max(opos.Y + obd.Y, floorh);
												}
											}
										}
									}
								}
							}

							// Response
							if (target.Y + bh > ceilh) {
								result.Y = ceilh - bh - epsilon;
								velocity.Z = 0;
								collided = true;
								break;
							}
							if (target.Y - bh < floorh) {
								result.Y = floorh + bh + epsilon;
								velocity.Z = 0;
								collided = true;
								break;
							}
						}
					}
				}

				owner.Position = result;
				velocity = finish - result;
				response = result - finish;
				return collided;
			}
			return false;
		}

		/// <summary>
		/// Получение самого близкого пола
		/// </summary>
		/// <param name="map">Карта</param>
		/// <returns>Высота</returns>
		public float GetFloorHeight(Vector3 offset) {
			if (owner != null) {
				float bw = size.X / 2f;
				float bh = size.Y / 2f;
				float bd = size.Z / 2f;
				Vector3 start = owner.Position + offset;
				Vector3 target = start - Vector3.UnitY * (size.Y * 1.5f);
				Vector3 bsz = new Vector3(bw, bh, bd);

				int sx = (int)Math.Floor(target.X - bw);
				int ex = (int)Math.Floor(target.X + bw);
				int sy = (int)Math.Floor(target.Y);
				int ey = (int)Math.Floor(start.Y);
				int sz = (int)Math.Floor(target.Z - bd);
				int ez = (int)Math.Floor(target.Z + bd);
				float floorh = float.MinValue;
				Map map = Engine.Current.World.Map;
				if (collideWithWorld && map != null) {
					for (int z = sz; z <= ez; z++) {
						for (int x = sx; x <= ex; x++) {
							for (int y = sy; y <= ey; y++) {
								Map.Block blk = map.GetBlockAtCoords(x, y, z);
								if (blk != null) {
									if (blk is Map.WallBlock) {
										floorh = Math.Max(floorh, y + 1);
									} else if (blk is Map.FloorBlock) {
										Map.FloorBlock mfb = blk as Map.FloorBlock;
										if (mfb.HasFloor && y < start.Y) {
											floorh = Math.Max(
												floorh,
												GetMaxFloorHeight(new Vector2(target.X - x, target.Z - z), new Vector2(bw, bd), mfb.FloorHeight) + y
											);
										}
									}
								}
							}
						}
					}
				}
				if (collideWithOther) {
					Collider[] others = Engine.Current.World.GetAllColliders();
					foreach (Collider col in others) {
						if (col != this) {
							Vector3 opos = col.owner.Position;
							Vector3 obd = col.size / 2f;
							if (!(
								((opos.Z + obd.Z) < (start.Z - bd)) ||
								((opos.Z - obd.Z) > (start.Z + bd)) ||
								((opos.X + obd.X) < (start.X - bw)) ||
								((opos.X - obd.X) > (start.X + bw)))) {
								if (start.Y > opos.Y && (target.Y - bh) < opos.Y + obd.Y) {
									floorh = Math.Max(opos.Y + obd.Y, floorh);
								}
							}
						}
					}
				}
				return floorh;
			}
			return float.MinValue;
		}


		/// <summary>
		/// Calculate maximal floor height
		/// </summary>
		/// <param name="pos">Box position</param>
		/// <param name="size">Box size</param>
		/// <returns>Maximal height</returns>
		internal float GetMaxFloorHeight(Vector2 pos, Vector2 size, float[] floorHeights) {
			float sx = Math.Max(pos.X - size.X, 0f);
			float ex = Math.Min(pos.X + size.X, 1f);
			float sy = Math.Max(pos.Y - size.Y, 0f);
			float ey = Math.Min(pos.Y + size.Y, 1f);
			return Math.Max(
				Math.Max(
					GetMatrixVal(floorHeights[0], floorHeights[1], floorHeights[2], floorHeights[3], sx, sy),
					GetMatrixVal(floorHeights[0], floorHeights[1], floorHeights[2], floorHeights[3], ex, sy)
				),
				Math.Max(
					GetMatrixVal(floorHeights[0], floorHeights[1], floorHeights[2], floorHeights[3], sx, ey),
					GetMatrixVal(floorHeights[0], floorHeights[1], floorHeights[2], floorHeights[3], ex, ey)
				)
			);
		}

		/// <summary>
		/// Calculate maximal ceiling height
		/// </summary>
		/// <param name="pos">Box position</param>
		/// <param name="size">Box size</param>
		/// <returns>Maximal height</returns>
		static float GetMaxCeilingHeight(Vector2 pos, Vector2 size, float[] ceilHeights) {
			float sx = Math.Max(pos.X - size.X, 0f);
			float ex = Math.Min(pos.X + size.X, 1f);
			float sy = Math.Max(pos.Y - size.Y, 0f);
			float ey = Math.Min(pos.Y + size.Y, 1f);
			return Math.Max(
				Math.Max(
					GetMatrixVal(ceilHeights[1], ceilHeights[0], ceilHeights[3], ceilHeights[2], sx, sy),
					GetMatrixVal(ceilHeights[1], ceilHeights[0], ceilHeights[3], ceilHeights[2], ex, sy)
				),
				Math.Max(
					GetMatrixVal(ceilHeights[1], ceilHeights[0], ceilHeights[3], ceilHeights[2], sx, ey),
					GetMatrixVal(ceilHeights[1], ceilHeights[0], ceilHeights[3], ceilHeights[2], ex, ey)
				)
			);
		}


		static bool CollideFloorVertical(Vector3 pos, Vector3 size, float[] floorHeights, bool forward, out float depth) {
			float sx = MathHelper.Clamp(pos.X - size.X, 0f, 1f);
			float ex = MathHelper.Clamp(pos.X + size.X, 0f, 1f);
			float sy = MathHelper.Clamp(pos.Z - size.Z, 0f, 1f);
			float ey = MathHelper.Clamp(pos.Z + size.Z, 0f, 1f);
			float sh = pos.Y - size.Y;
			depth = 0f;

			// Calculate matrix
			float p1 = GetMatrixVal(floorHeights[0], floorHeights[1], floorHeights[2], floorHeights[3], sx, sy);
			float p2 = GetMatrixVal(floorHeights[0], floorHeights[1], floorHeights[2], floorHeights[3], ex, sy);
			float p3 = GetMatrixVal(floorHeights[0], floorHeights[1], floorHeights[2], floorHeights[3], sx, ey);
			float p4 = GetMatrixVal(floorHeights[0], floorHeights[1], floorHeights[2], floorHeights[3], ex, ey);

			// First step - collide with barrier
			if (forward) {
				if (Math.Max(p1, p2) > sh && pos.Z < 0) {
					depth = 0f;
					return true;
				}
			} else {
				if (Math.Max(p3, p4) > sh && pos.Z > 1f) {
					depth = 1f;
					return true;
				}
			}

			// Second step - resolve max depth
			bool hl = false;
			bool hr = false;
			float dl = 0f;
			float dr = 0f;
			if (forward) {
				hl = CollideWithSlope(p1, p3, sh, out dl);
				hr = CollideWithSlope(p2, p4, sh, out dr);
				dl = sy + (ey - sy) * dl;
				dr = sy + (ey - sy) * dr;
			} else {
				hl = CollideWithSlope(p4, p2, sh, out dl);
				hr = CollideWithSlope(p3, p1, sh, out dr);
				dl = ey + (sy - ey) * dl;
				dr = ey + (sy - ey) * dr;
			}
			if (hl || hr) {
				if (hl && hr) {
					depth = Math.Min(dl, dr);
				} else {
					if (hl) {
						depth = dl;
					} else {
						depth = dr;
					}
				}

				// Mirror for rear collision
				if (!forward) {
					depth = 1f - depth;
				}
				return true;
			}

			// No collision
			return false;
		}


		static bool CollideFloorHorizontal(Vector3 pos, Vector3 size, float[] floorHeights, bool right, out float depth) {
			float sx = MathHelper.Clamp(pos.X - size.X, 0f, 1f);
			float ex = MathHelper.Clamp(pos.X + size.X, 0f, 1f);
			float sy = MathHelper.Clamp(pos.Z - size.Z, 0f, 1f);
			float ey = MathHelper.Clamp(pos.Z + size.Z, 0f, 1f);
			float sh = pos.Y - size.Y;
			depth = 0f;

			// Calculate matrix
			float p1 = GetMatrixVal(floorHeights[0], floorHeights[1], floorHeights[2], floorHeights[3], sx, sy);
			float p2 = GetMatrixVal(floorHeights[0], floorHeights[1], floorHeights[2], floorHeights[3], ex, sy);
			float p3 = GetMatrixVal(floorHeights[0], floorHeights[1], floorHeights[2], floorHeights[3], sx, ey);
			float p4 = GetMatrixVal(floorHeights[0], floorHeights[1], floorHeights[2], floorHeights[3], ex, ey);

			// First step - collide with barrier
			if (right) {
				if (Math.Max(p1, p3) > sh && pos.X < 0) {
					depth = 0f;
					return true;
				}
			} else {
				if (Math.Max(p2, p4) > sh && pos.X > 1f) {
					depth = 1f;
					return true;
				}
			}

			// Second step - resolve max depth
			bool hl = false;
			bool hr = false;
			float dl = 0f;
			float dr = 0f;
			if (right) {
				hl = CollideWithSlope(p3, p4, sh, out dl);
				hr = CollideWithSlope(p1, p2, sh, out dr);
				dl = sx + (ex - sx) * dl;
				dr = sx + (ex - sx) * dr;
			} else {
				hl = CollideWithSlope(p2, p1, sh, out dl);
				hr = CollideWithSlope(p4, p3, sh, out dr);
				dl = ex + (sx - ex) * dl;
				dr = ex + (sx - ex) * dr;
			}
			if (hl || hr) {
				if (hl && hr) {
					depth = Math.Min(dl, dr);
				} else {
					if (hl) {
						depth = dl;
					} else {
						depth = dr;
					}
				}
				return true;
			}

			// No collision
			return false;
		}


		static bool CollideCeilVertical(Vector3 pos, Vector3 size, float[] ceilHeights, bool forward, out float depth) {
			float sx = MathHelper.Clamp(pos.X - size.X, 0f, 1f);
			float ex = MathHelper.Clamp(pos.X + size.X, 0f, 1f);
			float sy = MathHelper.Clamp(pos.Z - size.Z, 0f, 1f);
			float ey = MathHelper.Clamp(pos.Z + size.Z, 0f, 1f);
			float sh = 1f - (pos.Y + size.Y);
			depth = 0f;

			// Calculate matrix
			float p1 = GetMatrixVal(ceilHeights[1], ceilHeights[0], ceilHeights[3], ceilHeights[2], sx, sy);
			float p2 = GetMatrixVal(ceilHeights[1], ceilHeights[0], ceilHeights[3], ceilHeights[2], ex, sy);
			float p3 = GetMatrixVal(ceilHeights[1], ceilHeights[0], ceilHeights[3], ceilHeights[2], sx, ey);
			float p4 = GetMatrixVal(ceilHeights[1], ceilHeights[0], ceilHeights[3], ceilHeights[2], ex, ey);

			// First step - collide with barrier
			if (forward) {
				if (Math.Max(p1, p2) > sh && pos.Z < 0) {
					depth = 0f;
					return true;
				}
			} else {
				if (Math.Max(p3, p4) > sh && pos.Z > 1f) {
					depth = 1f;
					return true;
				}
			}

			// Second step - resolve max depth
			bool hl = false;
			bool hr = false;
			float dl = 0f;
			float dr = 0f;
			if (forward) {
				hl = CollideWithSlope(p1, p3, sh, out dl);
				hr = CollideWithSlope(p2, p4, sh, out dr);
				dl = sy + (ey - sy) * dl;
				dr = sy + (ey - sy) * dr;
			} else {
				hl = CollideWithSlope(p4, p2, sh, out dl);
				hr = CollideWithSlope(p3, p1, sh, out dr);
				dl = ey + (sy - ey) * dl;
				dr = ey + (sy - ey) * dr;
			}
			if (hl || hr) {
				if (hl && hr) {
					depth = Math.Min(dl, dr);
				} else {
					if (hl) {
						depth = dl;
					} else {
						depth = dr;
					}
				}
				return true;
			}

			// No collision
			return false;
		}


		static bool CollideCeilHorizontal(Vector3 pos, Vector3 size, float[] ceilHeights, bool right, out float depth) {
			float sx = MathHelper.Clamp(pos.X - size.X, 0f, 1f);
			float ex = MathHelper.Clamp(pos.X + size.X, 0f, 1f);
			float sy = MathHelper.Clamp(pos.Z - size.Z, 0f, 1f);
			float ey = MathHelper.Clamp(pos.Z + size.Z, 0f, 1f);
			float sh = 1f - (pos.Y + size.Y);
			depth = 0f;

			// Calculate matrix
			float p1 = GetMatrixVal(ceilHeights[1], ceilHeights[0], ceilHeights[3], ceilHeights[2], sx, sy);
			float p2 = GetMatrixVal(ceilHeights[1], ceilHeights[0], ceilHeights[3], ceilHeights[2], ex, sy);
			float p3 = GetMatrixVal(ceilHeights[1], ceilHeights[0], ceilHeights[3], ceilHeights[2], sx, ey);
			float p4 = GetMatrixVal(ceilHeights[1], ceilHeights[0], ceilHeights[3], ceilHeights[2], ex, ey);

			// First step - collide with barrier
			if (right) {
				if (Math.Max(p3, p1) > sh && pos.X < 0) {
					depth = 0f;
					return true;
				}
			} else {
				if (Math.Max(p2, p4) > sh && pos.X > 1f) {
					depth = 1f;
					return true;
				}
			}

			// Second step - resolve max depth
			bool hl = false;
			bool hr = false;
			float dl = 0f;
			float dr = 0f;
			if (right) {
				hl = CollideWithSlope(p3, p4, sh, out dl);
				hr = CollideWithSlope(p1, p2, sh, out dr);
				dl = sx + (ex - sx) * dl;
				dr = sx + (ex - sx) * dr;
			} else {
				hl = CollideWithSlope(p2, p1, sh, out dl);
				hr = CollideWithSlope(p4, p3, sh, out dr);
				dl = ex + (sx - ex) * dl;
				dr = ex + (sx - ex) * dr;
			}
			if (hl || hr) {
				if (hl && hr) {
					depth = Math.Min(dl, dr);
				} else {
					if (hl) {
						depth = dl;
					} else {
						depth = dr;
					}
				}
				return true;
			}

			// No collision
			return false;
		}

		/// <summary>
		/// Point-slope collision
		/// </summary>
		/// <param name="p1">Near point</param>
		/// <param name="p2">Far point</param>
		/// <param name="pos">Point depth</param>
		/// <param name="depth">Nearest position</param>
		/// <returns>True if point collided</returns>
		static bool CollideWithSlope(float p1, float p2, float pos, out float depth) {
			depth = 1f;
			if (p1 > p2) {
				return false;
			}
			if (pos < p1) {
				depth = 0f;
				return true;
			}
			if (pos > p2) {
				return false;
			}

			depth = (pos - p1) / (p2 - p1);
			return true;
		}


		/// <summary>
		/// Get value from lerp matrix
		/// </summary>
		/// <param name="m1">Value 1</param>
		/// <param name="m2">Value 2</param>
		/// <param name="m3">Value 3</param>
		/// <param name="m4">Value 4</param>
		/// <param name="x">X lerp</param>
		/// <param name="y">Y lepr</param>
		/// <returns>Lerp value</returns>
		static float GetMatrixVal(float m1, float m2, float m3, float m4, float x, float y) {
			float near = m1 + (m2 - m1) * x;
			float far = m3 + (m4 - m3) * x;
			return near + (far - near) * y;
		}

	}
}
