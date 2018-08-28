using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Cubed.Data.Editor.Attributes {

	/// <summary>
	/// Name for attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public class InspectorDescriptionAttribute : Attribute {

		/// <summary>
		/// Resource manager
		/// </summary>
		static ResourceManager manager;

		/// <summary>
		/// Associated strings
		/// </summary>
		static Dictionary<string, string> lines;

		/// <summary>
		/// Current item name
		/// </summary>
		public string Description {
			get {
				return desc;
			}
		}

		/// <summary>
		/// Current entry name
		/// </summary>
		string desc;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="key">Key data</param>
		public InspectorDescriptionAttribute(string key) {
			if (lines == null) {
				lines = new Dictionary<string, string>();
			}
			if (!lines.ContainsKey(key)) {
				if (manager == null) {
					manager = new ResourceManager("Cubed.Forms.Resources.InspectorStrings", Assembly.GetExecutingAssembly());
				}
				lines.Add(key, manager.GetString(key));
			}
			desc = lines[key];
		}  
	}
}
