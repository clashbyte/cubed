using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.UI.Graphics;

namespace Cubed.Data.Editor.Attributes {

	/// <summary>
	/// Hiding field in inspector
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
	public class InspectorSectionAttribute : Attribute {

		/// <summary>
		/// Index of section
		/// </summary>
		public int ID {
			get;
			private set;
		}

		/// <summary>
		/// Section name
		/// </summary>
		public string Name {
			get;
			private set;
		}

		/// <summary>
		/// Section icon
		/// </summary>
		public UIIcon Icon {
			get;
			private set;
		}

		/// <summary>
		/// Section attribute
		/// </summary>
		/// <param name="id">Index</param>
		/// <param name="name">Sectiong name in InspectorStrings</param>
		/// <param name="icon">Section icon in InspectorIcons</param>
		public InspectorSectionAttribute(int id, string name = null, string icon = null) {
			ID = id;
			if (name != null) {
				Name = InspectorNameAttribute.GetName(name);
			}
			if (icon != null) {
				Icon = InspectorIconAttribute.GetIcon(icon);
			}
		}

	}
}
