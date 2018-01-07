using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using Cubed.Data.Rendering;

namespace Cubed.Components.Volumes {
	
	/// <summary>
	/// Объем из треугольников
	/// </summary>
	public class TrimeshVolumeComponent : VolumeComponent {

		/// <summary>
		/// Vertices
		/// </summary>
		public Vector3[] Vertices {
			get;
			set;
		}

		/// <summary>
		/// Volume vertex indices
		/// </summary>
		public ushort[] Indices {
			get;
			set;
		}

		/// <summary>
		/// Check for ray intersection
		/// </summary>
		internal override bool RayHit(Vector3 rayPos, Vector3 rayDir, float rayLength, out Vector3 pos, out Vector3 normal) {

			// Если есть с чем пересекаться
			if (Indices != null && Vertices != null) {
				if (Indices.Length > 0) {

					float range = float.MaxValue;
					bool hit = false;
					Vector3 hpos = Vector3.Zero, hnorm = Vector3.Zero;
					for (int i = 0; i < Indices.Length; i+=3) {
						Vector3 v0 = Vertices[Indices[i+0]];
						Vector3 v1 = Vertices[Indices[i+1]];
						Vector3 v2 = Vertices[Indices[i+2]];
						Vector3 hp, hn;
						if (Intersections.RayTriangle(rayPos, rayDir, v0, v1, v2, false, out hp, out hn)) {
							float d = (hp - rayPos).LengthSquared;
							if (d < range) {
								range = d;
								hpos = hp;
								hnorm = hn;
								hit = true;
							}
						}
					}

					range = (float)Math.Sqrt(range);
					if (hit && range <= rayLength) {
						pos = hpos;
						normal = hnorm;
						return true;
					}
				}
			}

			// No intersection
			pos = Vector3.Zero;
			normal = Vector3.UnitZ;
			return false;
		}

		/// <summary>
		/// Debug rendering
		/// </summary>
		protected override void RenderDebug() {
			if (!Enabled) {
				return;
			}
			if (Vertices!=null && Indices != null) {
				GL.Color3(System.Drawing.Color.Yellow);
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
				GL.LineWidth(2f);
				GL.Disable(EnableCap.CullFace);

				GL.Begin(PrimitiveType.Triangles);
				foreach (ushort idx in Indices) {
					GL.Vertex3(Vertices[idx].X, Vertices[idx].Y, -Vertices[idx].Z);
				}
				GL.End();

				GL.Enable(EnableCap.CullFace);
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			}
		}
	}
}
