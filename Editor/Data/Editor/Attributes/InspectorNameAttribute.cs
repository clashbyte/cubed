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
	public class InspectorNameAttribute : Attribute {

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
		public string Name {
			get {
				return name;
			}
		}

		/// <summary>
		/// Retrieving name from resources
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>String</returns>
		internal static string GetName(string key) {
			if (lines == null) {
				lines = new Dictionary<string, string>();
			}
			if (!lines.ContainsKey(key)) {
				if (manager == null) {
					manager = new ResourceManager("Cubed.Forms.Resources.InspectorStrings", Assembly.GetExecutingAssembly());
				}
				string val = manager.GetString(key);
				if (string.IsNullOrEmpty(val)) {
					val = "#" + key;
				}
				lines.Add(key, val);
			}
			return lines[key];
		}

		/// <summary>
		/// Current entry name
		/// </summary>
		string name;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="key">Key data</param>
		public InspectorNameAttribute(string key) {
			name = GetName(key);
		}  

	}
}
