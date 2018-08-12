using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Cubed.Data.Editor.Previews {

	/// <summary>
	/// Generator for preview
	/// </summary>
	public abstract class PreviewGenerator {

		/// <summary>
		/// Path for file
		/// </summary>
		public string Path {
			get;
			private set;
		}

		/// <summary>
		/// Resulting image
		/// </summary>
		public Image Result {
			get;
			protected set;
		}

		/// <summary>
		/// Flag to display subicon
		/// </summary>
		public bool ShowSubIcon {
			get;
			protected set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="path">Path to file</param>
		public PreviewGenerator(string path) {
			Path = path;
		}

		/// <summary>
		/// Empty constructor
		/// </summary>
		public PreviewGenerator() { }

		/// <summary>
		/// Background work
		/// </summary>
		public abstract void Generate();

	}
}
