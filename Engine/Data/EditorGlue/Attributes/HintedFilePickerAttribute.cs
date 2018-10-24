using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Data.EditorGlue.Attributes {

	/// <summary>
	/// File picker hint
	/// </summary>
	public class HintedFilePickerAttribute : Attribute {

		/// <summary>
		/// Allowed file extensions for picker
		/// </summary>
		public string Extensions {
			get;
			private set;
		}

		/// <summary>
		/// Constructor for hint
		/// </summary>
		/// <param name="exts">List of allowed extensions</param>
		public HintedFilePickerAttribute(string exts) {
			Extensions = exts;
		}

	}
}
