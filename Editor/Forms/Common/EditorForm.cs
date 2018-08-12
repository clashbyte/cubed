using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cubed.Forms.Common {

	/// <summary>
	/// Main prototype for editors
	/// </summary>
	public partial class EditorForm : Form {

		

		/// <summary>
		/// Is current file saved
		/// </summary>
		public bool Saved {
			get;
			private set;
		}


		/// <summary>
		/// Constructor
		/// </summary>
		public EditorForm() {
			InitializeComponent();
		}

		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			
		}
	}
}
