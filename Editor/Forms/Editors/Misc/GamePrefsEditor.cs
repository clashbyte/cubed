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
using Cubed.Data.Files;
using Cubed.Data.Projects;
using Cubed.Forms.Common;
using Cubed.Forms.Resources;
using Cubed.UI.Graphics;

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
				return text;
			}
		}

		/// <summary>
		/// Icon
		/// </summary>
		public override UIIcon CustomIcon {
			get {
				return icon;
			}
		}

		/// <summary>
		/// Hidden icon
		/// </summary>
		UIIcon icon;

		/// <summary>
		/// Text label
		/// </summary>
		string text;

		/// <summary>
		/// Copied project info
		/// </summary>
		ProjectInfo projectInfo = null;
		
		/// <summary>
		/// Project info
		/// </summary>
		public GamePrefsEditor() {
			InitializeComponent();
			text = CustomEditors.GamePrefs;
			icon = new UIIcon(DirectoryInspectorIcons.Gear);

			Chunk prefs = Project.Info.Save();
			projectInfo = ProjectInfo.Read(prefs);
			inspector.Target = projectInfo;
		}

		/// <summary>
		/// Flag for saved
		/// </summary>
		private void inspector_FieldChanged(object sender, EventArgs e) {
			Saved = false;
		}

		/// <summary>
		/// Saving game prefs
		/// </summary>
		public override void Save() {
			Chunk prefs = projectInfo.Save();
			Project.Info = ProjectInfo.Read(prefs);
			string err = "";
			ChunkedFile.Write(System.IO.Path.Combine(Project.Root.FullPath, ".cubed"), prefs, out err);
			MainForm.UpdateTitle();
			Saved = true;
		}
	}



}
