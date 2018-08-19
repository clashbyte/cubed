using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Data.Editor;
using Cubed.Data.Projects;

namespace Cubed.Forms.Common {

	/// <summary>
	/// Main prototype for editors
	/// </summary>
	public partial class EditorForm : Form {

		/// <summary>
		/// Updating text
		/// </summary>
		public override string Text {
			get {
				if (File != null) {
					return File.NameWithoutExt + " - " + FileTypeManager.GetName(File);
				}
				return "Unknown file";
			}
			set {}
		}

		/// <summary>
		/// Is current file saved
		/// </summary>
		public bool Saved {
			get;
			private set;
		}

		/// <summary>
		/// Current project entry
		/// </summary>
		public Project.Entry File {
			get;
			protected set;
		}


		/// <summary>
		/// Constructor
		/// </summary>
		public EditorForm() {
			InitializeComponent();
			Saved = true;
		}

		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			
		}

	}
}
