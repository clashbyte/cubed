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
		static Dictionary<string, Image> imgs;

		/// <summary>
		/// Current item name
		/// </summary>
		public Image Image {
			get {
				return img;
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
		/// Current entry name
		/// </summary>
		Image img;

		/// <summary>
		/// Current entry icon
		/// </summary>
		UIIcon icon;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="key">Key data</param>
		public InspectorIconAttribute(string key) {
			if (imgs == null) {
				imgs = new Dictionary<string, Image>();
			}
			if (!imgs.ContainsKey(key)) {
				if (manager == null) {
					manager = new ResourceManager("Cubed.Forms.Resources.InspectorIcons", Assembly.GetExecutingAssembly());
				}
				imgs.Add(key, (Image)manager.GetObject(key));
			}
			img = imgs[key];
			icon = new UIIcon(img);
		}  

	}
}
