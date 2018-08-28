using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Cubed.Components.Rendering;
using Cubed.Data.Game.Attributes;
using Cubed.Graphics;
using OpenTK;

namespace Cubed.Prefabs {

	/// <summary>
	/// Sprite on map
	/// </summary>
	[Prefab(1)]
	public class MapSprite : GamePrefab {

		/// <summary>
		/// Core texture
		/// </summary>
		public Texture Texture {
			get {
				return sprite.Texture;
			}
			set {
				sprite.Texture = value;
				
			}
		}

		/// <summary>
		/// Affecting by light attribute
		/// </summary>
		public bool AffectedByLight {
			get {
				return sprite.AffectedByLight;
			}
			set {
				sprite.AffectedByLight = value;
			}
		}

		/// <summary>
		/// Tint color
		/// </summary>
		public Color Tint {
			get {
				return sprite.Tint;
			}
			set {
				sprite.Tint = value;
			}
		}

		/// <summary>
		/// Texture offset
		/// </summary>
		public Vector2 Offset {
			get {
				return sprite.Offset;
			}
			set {
				sprite.Offset = value;
			}
		}

		/// <summary>
		/// Texture scale
		/// </summary>
		public Vector2 Scale {
			get {
				return sprite.Scale;
			}
			set {
				sprite.Scale = value;
			}
		}

		/// <summary>
		/// Internal sprite
		/// </summary>
		SpriteComponent sprite;

		/// <summary>
		/// Map sprite
		/// </summary>
		public MapSprite() {
			sprite = new SpriteComponent() {
				AffectedByLight = true,
				Facing = SpriteComponent.FacingMode.Y
			};
			AddComponent(sprite);
		}

		/// <summary>
		/// Adding to scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Assign(World.Scene scene) {
			scene.Entities.Add(this);
		}

		/// <summary>
		/// Removing from scene
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Unassign(World.Scene scene) {
			scene.Entities.Remove(this);
		}
	}
}
