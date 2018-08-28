using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Cubed.Data.Game.Attributes;
using Cubed.Graphics;
using Cubed.World;

namespace Cubed.Prefabs {

	/// <summary>
	/// Map light entity
	/// </summary>
	[Prefab(2)]
	public class MapLight : GamePrefab {

		/// <summary>
		/// Light color
		/// </summary>
		public Color Color {
			get {
				return light.Color;
			}
			set {
				light.Color = Color.FromArgb(value.R, value.G, value.B);
			}
		}
		
		/// <summary>
		/// Light range
		/// </summary>
		public float Range {
			get {
				return light.Range;
			}
			set {
				light.Range = Math.Max(value, 0.1f);
			}
		}

		/// <summary>
		/// Light texture
		/// </summary>
		public Texture Texture {
			get {
				return light.Texture;
			}
			set {
				light.Texture = value;
			}
		}

		/// <summary>
		/// Texture angle
		/// </summary>
		public float TextureAngle {
			get {
				return light.TextureAngle;
			}
			set {
				light.TextureAngle = value;
			}
		}

		/// <summary>
		/// Light entity
		/// </summary>
		Light light;

		/// <summary>
		/// Constructor
		/// </summary>
		public MapLight() {
			light = new Light() {
				Color = Color.White,
				Range = 5
			};
			light.Parent = this;
		}

		/// <summary>
		/// Assigning to scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Assign(Scene scene) {
			scene.Entities.Add(this);
			scene.Entities.Add(light);
		}

		/// <summary>
		/// Unassigning from scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Unassign(Scene scene) {
			scene.Entities.Remove(this);
			scene.Entities.Remove(light);
		}

		
	}
}
