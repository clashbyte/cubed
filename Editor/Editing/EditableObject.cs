using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Data.Editor.Attributes;
using Cubed.Editing.Gizmos;
using Cubed.Prefabs;
using Cubed.UI.Graphics;
using Cubed.World;
using OpenTK;

namespace Cubed.Editing {

	/// <summary>
	/// Editable object class
	/// </summary>
	public abstract class EditableObject {

		/// <summary>
		/// Some game prefab
		/// </summary>
		[InspectorHidden]
		public GamePrefab Prefab {
			get {
				if (prefab == null) {
					prefab = CreatePrefab();
				}
				return prefab;
			}
		}

		/// <summary>
		/// Main gizmo
		/// </summary>
		[InspectorHidden]
		public Entity Gizmo {
			get;
			protected set;
		}

		/// <summary>
		/// Selected gizmo
		/// </summary>
		[InspectorHidden]
		public Entity SelectedGizmo {
			get;
			protected set;
		}

		/// <summary>
		/// Bounding box position
		/// </summary>
		[InspectorHidden]
		public Vector3 BoundPosition {
			get;
			protected set;
		}

		/// <summary>
		/// Bounding box size
		/// </summary>
		[InspectorHidden]
		public Vector3 BoundSize {
			get;
			protected set;
		}

		/// <summary>
		/// Handling gizmos
		/// </summary>
		[InspectorHidden]
		public virtual Gizmo[] ControlGizmos {
			get {
				return new Gizmo[0];
			}
		}

		/// <summary>
		/// Current prefab
		/// </summary>
		GamePrefab prefab;

		/// <summary>
		/// Creating item
		/// </summary>
		public abstract void Create(Scene scene);

		/// <summary>
		/// Destroying it
		/// </summary>
		public abstract void Destroy(Scene scene);

		/// <summary>
		/// Enable selection
		/// </summary>
		public abstract void Select(Scene scene);

		/// <summary>
		/// Disable selection
		/// </summary>
		public abstract void Deselect(Scene scene);

		/// <summary>
		/// Entering play mode
		/// </summary>
		public virtual void StartPlayMode(Scene scene) { }

		/// <summary>
		/// Stopping play mode
		/// </summary>
		public virtual void StopPlayMode(Scene scene) { }

		/// <summary>
		/// Updating logic
		/// </summary>
		public virtual void Update(Scene scene) { }

		/// <summary>
		/// Updating in editor
		/// </summary>
		public void EditorUpdate(Scene scene) {
			if (Prefab != null) {
				if (Gizmo != null) {
					Gizmo.Position = Prefab.Position;
					Gizmo.Angles = Prefab.Angles;
				}
				if (SelectedGizmo != null) {
					SelectedGizmo.Position = Prefab.Position;
					SelectedGizmo.Angles = Prefab.Angles;
				}
			}
			Update(scene);
		}

		/// <summary>
		/// Setting prefab
		/// </summary>
		/// <param name="p">Prefab</param>
		public void SetPrefab(GamePrefab p) {
			if (prefab != null) {
				prefab.Destroy();
			}
			prefab = p;
		}

		/// <summary>
		/// Spawning prefab for this object
		/// </summary>
		/// <returns></returns>
		GamePrefab CreatePrefab() {
			Type t = GetType();
			TargetPrefabAttribute target = Attribute.GetCustomAttribute(t, typeof(TargetPrefabAttribute)) as TargetPrefabAttribute;
			if (target == null) {
				throw new Exception("Unable to create editable prefab");
			}
			return Activator.CreateInstance(target.Prefab) as GamePrefab;
		}
	}
}
