using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using Cubed.UI.Graphics;

namespace Cubed.Data.Editor.Attributes {

	/// <summary>
	/// Name for attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class InspectorIconAttribute : Attribute {

		/// <summary>
		/// Resource manager
		/// </summary>
		static ResourceManager manager;

		/// <summary>
		/// Associated strings
		/// </summary>
		static Dictionary<string, UIIcon> imgs;

		/// <summary>
		/// Current item name
		/// </summary>
		public Image Image {
			get {
				if (icon != null) {
					return icon.Scan;
				}
				return null;
			}
		}

		/// <summary>
		/// Current item name
		/// </summary>
		public UIIcon Icon {
			get {
				return icon;
			}
		}

		/// <summary>
		/// Current entry icon
		/// </summary>
		UIIcon icon;

		/// <summary>
		/// Retrieving icon from resources
		/// </summary>
		/// <returns></returns>
		internal static UIIcon GetIcon(string key) {
			if (imgs == null) {
				imgs = new Dictionary<string, UIIcon>();
			}
			if (!imgs.ContainsKey(key)) {
				if (manager == null) {
					manager = new ResourceManager("Cubed.Forms.Resources.InspectorIcons", Assembly.GetExecutingAssembly());
				}
				imgs.Add(key, new UIIcon((Image)manager.GetObject(key)));
			}
			return imgs[key];
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="key">Key data</param>
		public InspectorIconAttribute(string key) {
			icon = GetIcon(key);
		}  

	}
}
