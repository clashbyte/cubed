using System;
using System.Drawing;
using Cubed.Audio;
using Cubed.Components.Rendering;
using Cubed.Data.Editor.Attributes;
using Cubed.Editing.Gizmos;
using Cubed.Forms.Resources;
using Cubed.Graphics;
using Cubed.World;
using OpenTK;

namespace Cubed.Editing {

	[InspectorIcon("Door")]
	[InspectorName("Door")]
	[InspectorDescription("DoorDesc")]
	[TargetPrefab(typeof(Prefabs.MapDoor))]
	public class MapDoor : EditableObject {

		/// <summary>
		/// Is door opened
		/// </summary>
		public bool Opened {
			get {
				return (Prefab as Prefabs.MapDoor).Open;
			}
			set {
				(Prefab as Prefabs.MapDoor).Open = value;
			}
		}

		/// <summary>
		/// Front texture
		/// </summary>
		public Texture FrontTexture {
			get {
				return frontTex;
			}
			set {
				frontTex = value;
				(Prefab as Prefabs.MapDoor).FrontTexture = frontTex != null ? frontTex : emptyTex; ;
			}
		}

		/// <summary>
		/// Rear texture
		/// </summary>
		public Texture RearTexture {
			get {
				return rearTex;
			}
			set {
				rearTex = value;
				(Prefab as Prefabs.MapDoor).RearTexture = rearTex != null ? rearTex : emptyTex;
			}
		}

		/// <summary>
		/// Side texture texture
		/// </summary>
		public Texture SideTexture {
			get {
				return sideTex;
			}
			set {
				sideTex = value;
				(Prefab as Prefabs.MapDoor).SideTexture = sideTex != null ? sideTex : emptySideTex;
			}
		}

		/// <summary>
		/// Open sound
		/// </summary>
		public AudioTrack OpenSound {
			get {
				return (Prefab as Prefabs.MapDoor).OpenSound;
			}
			set {
				(Prefab as Prefabs.MapDoor).OpenSound = value;
			}
		}

		/// <summary>
		/// Close sound
		/// </summary>
		public AudioTrack CloseSound {
			get {
				return (Prefab as Prefabs.MapDoor).CloseSound;
			}
			set {
				(Prefab as Prefabs.MapDoor).CloseSound = value;
			}
		}

		/// <summary>
		/// Locked sound
		/// </summary>
		public AudioTrack LockedSound {
			get {
				return (Prefab as Prefabs.MapDoor).LockedSound;
			}
			set {
				(Prefab as Prefabs.MapDoor).LockedSound = value;
			}
		}

		/// <summary>
		/// Total width
		/// </summary>
		[InspectorRange(1, 64)]
		public int Width {
			get {
				return (Prefab as Prefabs.MapDoor).Width;
			}
			set {
				(Prefab as Prefabs.MapDoor).Width = value;
				UpdateCollider();
			}
		}

		/// <summary>
		/// Total height
		/// </summary>
		[InspectorRange(1, 64)]
		public int Height {
			get {
				return (Prefab as Prefabs.MapDoor).Height;
			}
			set {
				(Prefab as Prefabs.MapDoor).Height = value;
				UpdateCollider();
			}
		}

		/// <summary>
		/// Thickness
		/// </summary>
		[InspectorRange(0.1f, 64)]
		public float Thickness {
			get {
				return (Prefab as Prefabs.MapDoor).Thickness;
			}
			set {
				(Prefab as Prefabs.MapDoor).Thickness = value;
				UpdateCollider();
				rotateGizmo.Distance = Thickness / 2f + 0.4f;
			}
		}

		/// <summary>
		/// Rotation
		/// </summary>
		public float Angle {
			get {
				return (Prefab as Prefabs.MapDoor).Angle;
			}
			set {
				(Prefab as Prefabs.MapDoor).Angle = (float)Math.Round(value / 90f) * 90f;
				rotateGizmo.Angle = (Prefab as Prefabs.MapDoor).Angle;
				UpdateCollider();
			}
		}

		/// <summary>
		/// Door opening type
		/// </summary>
		public Door.DoorType Type {
			get {
				return (Prefab as Prefabs.MapDoor).Type;
			}
			set {
				(Prefab as Prefabs.MapDoor).Type = value;
			}
		}

		/// <summary>
		/// Spawn offset is zero
		/// </summary>
		public override Vector3 SpawnOffset {
			get {
				return Vector3.Zero;
			}
		}

		/// <summary>
		/// Interactive gizmos
		/// </summary>
		public override Gizmo[] ControlGizmos {
			get {
				return new Gizmo[] {
					rotateGizmo
				};
			}
		}

		/// <summary>
		/// Hidden textures
		/// </summary>
		Texture frontTex, rearTex, sideTex;

		/// <summary>
		/// Textures for sides
		/// </summary>
		static Texture emptyTex, emptySideTex;

		/// <summary>
		/// Is entity selected
		/// </summary>
		bool selected;

		/// <summary>
		/// Gizmo for rotation
		/// </summary>
		AngleGizmo rotateGizmo;

		/// <summary>
		/// Creating light
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Create(Scene scene) {
			if (emptyTex == null || emptyTex.IsReleased) {
				emptyTex = new Texture(EditorTextures.Door);
			}
			if (emptySideTex == null || emptySideTex.IsReleased) {
				emptySideTex = new Texture(EditorTextures.DoorSide);
			}
			if (Gizmo == null) {
				Gizmo = new Entity();
				Gizmo.Parent = Prefab;
				Gizmo.LocalPosition = Vector3.Zero;
				Gizmo.AddComponent(new WireCubeComponent() {
					Size = Vector3.One * 0.3f,
					WireColor = Color.LightBlue,
					WireWidth = 1f
				});
				scene.Entities.Add(Gizmo);
			}
			if (SelectedGizmo == null) {
				SelectedGizmo = new Entity();
				SelectedGizmo.Visible = false;
				SelectedGizmo.Parent = Prefab;
				SelectedGizmo.AddComponent(new WireCubeComponent() {
					Size = Vector3.One * 0.3f,
					WireColor = Color.LightBlue,
					WireWidth = 1.5f
				});
				SelectedGizmo.LocalPosition = Vector3.Zero;
				scene.Entities.Add(SelectedGizmo);
			}
			BoundPosition = Vector3.Zero;
			UpdateCollider();

			// Gizmo
			rotateGizmo = new AngleGizmo(Color.LightBlue, 0.2f, Thickness / 2f + 0.4f) {
				Parent = Prefab,
				Position = Prefab.Position,
				Filter = AngleFilter
			};

			// Adding scene gizmos
			Prefab.Assign(scene);

			// Accessing textures
			frontTex = (Prefab as Prefabs.MapDoor).FrontTexture;
			rearTex = (Prefab as Prefabs.MapDoor).RearTexture;
			sideTex = (Prefab as Prefabs.MapDoor).SideTexture;
			if (frontTex == null) {
				FrontTexture = null;
			}
			if (rearTex == null) {
				RearTexture = null;
			}
			if (sideTex == null) {
				SideTexture = null;
			}
		}

		/// <summary>
		/// Destroying
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Destroy(Scene scene) {
			if (Prefab != null) {
				Prefab.Unassign(scene);
				Prefab.Destroy();
			}
			if (Gizmo != null) {
				scene.Entities.Remove(Gizmo);
				Gizmo.Destroy();
			}
			if (SelectedGizmo != null) {
				scene.Entities.Remove(SelectedGizmo);
				SelectedGizmo.Destroy();
			}
		}
		
		/// <summary>
		/// Selecting object
		/// </summary>
		/// <param name="scene"></param>
		public override void Select(Scene scene) {
			selected = true;
			SelectedGizmo.Visible = true;
		}

		/// <summary>
		/// Deselecting object
		/// </summary>
		/// <param name="scene">Scene</param>
		public override void Deselect(Scene scene) {
			selected = false;
			SelectedGizmo.Visible = false;
		}

		/// <summary>
		/// Entering play mode
		/// </summary>
		public override void StartPlayMode(Scene scene) {
			SelectedGizmo.Visible = false;
		}

		/// <summary>
		/// Exiting play mode
		/// </summary>
		public override void StopPlayMode(Scene scene) {
			SelectedGizmo.Visible = selected;
		}

		/// <summary>
		/// Updating spawn block
		/// </summary>
		void UpdateCollider() {
			float rad = MathHelper.DegreesToRadians(Angle);
			float sin = (float)Math.Sin(rad);
			float cos = (float)Math.Cos(rad);
			Vector3 size = new Vector3(
				(float)Width * cos - Thickness * sin,
				Height,
				(float)Width * sin + Thickness * cos
			);
			Gizmo.GetComponent<WireCubeComponent>().Size = size;
			SelectedGizmo.GetComponent<WireCubeComponent>().Size = size;
			BoundSize = size;
		}

		/// <summary>
		/// Angle filter
		/// </summary>
		float AngleFilter(AngleGizmo gz, float src) {
			src = (float)Math.Round(src / 90f) * 90f;
			(Prefab as Prefabs.MapDoor).Angle = src;
			return src;
		}
	}
}
