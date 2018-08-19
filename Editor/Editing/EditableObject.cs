using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		public GamePrefab Prefab {
			get;
			protected set;
		}

		/// <summary>
		/// Object name
		/// </summary>
		public string Name {
			get;
			protected set;
		}

		/// <summary>
		/// Icon
		/// </summary>
		public UIIcon Icon {
			get;
			protected set;
		}

		/// <summary>
		/// Main gizmo
		/// </summary>
		public Entity Gizmo {
			get;
			protected set;
		}

		/// <summary>
		/// Selected gizmo
		/// </summary>
		public Entity SelectedGizmo {
			get;
			protected set;
		}

		/// <summary>
		/// Bounding box position
		/// </summary>
		public Vector3 BoundPosition {
			get;
			protected set;
		}

		/// <summary>
		/// Bounding box size
		/// </summary>
		public Vector3 BoundSize {
			get;
			protected set;
		}

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
	}
}
