using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.World {

	/// <summary>
	/// Entity that blocks light
	/// </summary>
	public abstract class ObstructEntity : Entity {

		/// <summary>
		/// Last affected lights
		/// </summary>
		public Light[] AffectedLights {
			get;
			internal set;
		}

		/// <summary>
		/// Flags that it needs rebuild
		/// </summary>
		public bool NeedRebuild {
			get;
			internal set;
		}

		/// <summary>
		/// Static lightmap valid flag
		/// </summary>
		public bool StaticInvalid {
			get;
			internal set;
		}

		/// <summary>
		/// Static lightmap valid flag
		/// </summary>
		public bool DynamicInvalid {
			get;
			internal set;
		}

		/// <summary>
		/// Invalidating mesh part
		/// </summary>
		public void InvalidateMesh() {
			NeedRebuild = true;
		}

		/// <summary>
		/// Marking static lightmap invalid
		/// </summary>
		public void InvalidateStatic() {
			StaticInvalid = true;
		}

		/// <summary>
		/// Marking dynamic lightmap invalid
		/// </summary>
		public void InvalidateDynamic() {
			DynamicInvalid = true;
		}

		/// <summary>
		/// Marking all lights to update
		/// </summary>
		public void UpdateLights() {
			if (AffectedLights != null) {
				foreach (Light light in AffectedLights) {
					light.MakeDirty();
				}
			}
		}

		/// <summary>
		/// Checking all affected lights
		/// </summary>
		internal void CheckLights(IEnumerable<Light> lights) {
			if (AffectedLights == null) {
				AffectedLights = new Light[0];
			}
			List<Light> affectedLights = new List<Light>(AffectedLights);
			foreach (Light l1 in lights) {
				if (TouchesLight(l1) && !affectedLights.Contains(l1)) {
					affectedLights.Add(l1);
					if (l1.Static) {
						StaticInvalid = true;
					} else {
						DynamicInvalid = true;
					}
				}
			}
			List<Light> tempLights = new List<Light>(affectedLights);
			foreach (Light l in tempLights) {
				if (!TouchesLight(l) || !lights.Contains(l)) {
					affectedLights.Remove(l);
					if (l.Static) {
						StaticInvalid = true;
					} else {
						DynamicInvalid = true;
					}
				}
			}
			foreach (Light l2 in affectedLights) {
				if (l2.IsChanged) {
					if (l2.Static) {
						StaticInvalid = true;
					} else {
						DynamicInvalid = true;
					}
				}
			}
			AffectedLights = affectedLights.ToArray();
		}

		/// <summary>
		/// Rebuilding lightmap
		/// </summary>
		internal void RebuildLightmaps() {
			if (DynamicInvalid) {
				RebuildDynamicLightmap();
				DynamicInvalid = false;
			}
			if (StaticInvalid) {
				RebuildStaticLightmap();
				StaticInvalid = false;
			}
		}

		/// <summary>
		/// Check for intersection with light
		/// </summary>
		/// <param name="light">Light</param>
		/// <returns>True if entity touches light</returns>
		public abstract bool TouchesLight(Light light);

		/// <summary>
		/// Rebuilding dynamic lightmap
		/// </summary>
		public abstract void RebuildMesh();

		/// <summary>
		/// Rendering this entity as shadow caster
		/// </summary>
		public abstract void RenderShadowPass();

		/// <summary>
		/// Rebuilding static lightmap
		/// </summary>
		public abstract void RebuildStaticLightmap();

		/// <summary>
		/// Rebuilding dynamic lightmap
		/// </summary>
		public abstract void RebuildDynamicLightmap();
	}
}
