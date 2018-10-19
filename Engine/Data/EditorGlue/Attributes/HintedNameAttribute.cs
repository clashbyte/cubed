using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Data.EditorGlue.Attributes {

	/// <summary>
	/// Class for attribute localization
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
	public class HintedNameAttribute : Attribute {

		/// <summary>
		/// Name
		/// </summary>
		public string Name {
			get;
			private set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public HintedNameAttribute(string name) {
			Name = name;
		}

	}
}
