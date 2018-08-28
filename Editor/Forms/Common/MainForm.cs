using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Data.Editor.Previews;
using Cubed.Data.Projects;
using Cubed.Forms.Editors.Map;

namespace Cubed.Forms.Common
{
	public partial class MainForm : Form
	{

		/// <summary>
		/// Handler for updates
		/// </summary>
		public static event EventHandler LogicUpdate;

		/// <summary>
		/// Current main form instance
		/// </summary>
		static MainForm Current;

		/// <summary>
		/// Constructor
		/// </summary>
		public MainForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Starting this form
		/// </summary>
		/// <returns>Dialog result</returns>
		public DialogResult Open() {


			HandleStartingAction();


			return ShowDialog();
		}

		/// <summary>
		/// Switching to form
		/// </summary>
		private void MainForm_Activated(object sender, EventArgs e) {


		}

		/// <summary>
		/// Showing form - process startup events
		/// </summary>
		private void MainForm_Shown(object sender, EventArgs e) {
			
			// Registering form
			Current = this;

			// Populating project hierarchy
			Preview.PreviewReady += Preview_PreviewReady;
			Project.EntriesChangedEvent += Project_EntriesChangedEvent;
			PopulateProjectView();

			// Opening all previously opened files
			editorsControl.TabPages.Clear();

		}

		/// <summary>
		/// Closing form
		/// </summary>
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing) {





				// User closes the form - close parent
				ClosingAction = CloseAction.FullClose;
			}
		}

		/// <summary>
		/// Form closed totally
		/// </summary>
		private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
			




		}

		/// <summary>
		/// Update all logic
		/// </summary>
		private void logicTimer_Tick(object sender, EventArgs e) {

			// Rescanning project
			Project.Update();

			// Handle previews
			Preview.UpdatePending();

			// Handle hooks
			if (LogicUpdate != null) {
				LogicUpdate(this, EventArgs.Empty);
			}
		}
	}
}
