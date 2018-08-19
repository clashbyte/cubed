using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Prefabs;
using Cubed.UI.Graphics;

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
		/// Creating item
		/// </summary>
		public void Create() {

		}

		/// <summary>
		/// Destroying it
		/// </summary>
		public void Destroy() {

		}
	}
}
