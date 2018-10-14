using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cubed.Data.Defines;
using Cubed.Data.Projects;
using Cubed.Forms.Common;

namespace Cubed.Forms.Editors.Misc {

	/// <summary>
	/// Form for game options
	/// </summary>
	public partial class GamePrefsEditor : EditorForm {

		/// <summary>
		/// Text for tab
		/// </summary>
		public override string Text {
			get {
				return "Game preferences";
			}
		}

		/// <summary>
		/// Copied project info
		/// </summary>
		ProjectInfo projectInfo = null;
		
		/// <summary>
		/// Project info
		/// </summary>
		public GamePrefsEditor() {
			InitializeComponent();
			projectInfo = Project.Info;
			inspector.Target = projectInfo;
		}
	}



}
