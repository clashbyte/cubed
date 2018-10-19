using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Data.EditorGlue.Attributes {

	/// <summary>
	/// Hint for section
	/// </summary>
	public class HintedSectionAttribute : Attribute {

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
		public string Icon {
			get;
			private set;
		}

		/// <summary>
		/// Section attribute
		/// </summary>
		/// <param name="id">Index</param>
		/// <param name="name">Sectiong name in InspectorStrings</param>
		/// <param name="icon">Section icon in InspectorIcons</param>
		public HintedSectionAttribute(int id, string name = null, string icon = null) {
			ID = id;
			Name = name;
			Icon = icon;
		}

	}
}
