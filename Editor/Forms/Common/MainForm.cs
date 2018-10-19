using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Data.Editor;
using Cubed.Data.Editor.Previews;
using Cubed.Data.Projects;
using Cubed.Forms.Editors.Map;
using Cubed.Forms.Editors.Misc;

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
		/// Closing by code flag
		/// </summary>
		bool codeClosing;

		/// <summary>
		/// Constructor
		/// </summary>
		public MainForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Updating title
		/// </summary>
		public static void UpdateTitle() {
			if (Project.Info != null && Current != null) {
				Current.Text = Project.Info.Basic.Name + " - Cubed Editor";
			}
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
			UpdateTitle();

			// Populating project hierarchy
			Preview.PreviewReady += Preview_PreviewReady;
			Project.EntriesChangedEvent += Project_EntriesChangedEvent;
			PopulateProjectView();

			// Creating project types
			FileTypeManager.FileType[] types = FileTypeManager.EditableTypes();
			foreach (FileTypeManager.FileType ft in types) {
				ToolStripMenuItem mt = new ToolStripMenuItem(ft.Name, ft.Icon.Combined(16), contextMenuCreateItem_Click) {
					Tag = ft
				};
				projectCreateMenu.Items.Add(mt);
			}
			(projectPopupMenu.Items[0] as ToolStripMenuItem).DropDown = projectCreateMenu;

			// Opening all previously opened files
			editorsControl.TabPages.Clear();
			
			// Creating start page
			HomePage hpage = new HomePage();
			TabPage tp = new TabPage();
			hpage.TopLevel = false;
			hpage.Location = System.Drawing.Point.Empty;
			hpage.Visible = true;
			hpage.BringToFront();
			tp.Controls.Add(hpage);
			tp.Tag = hpage;
			hpage.Dock = DockStyle.Fill;
			editorsControl.AddTab(tp, true);



		}

		/// <summary>
		/// Closing form
		/// </summary>
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason == CloseReason.UserClosing && !codeClosing) {

				// Trying to close all editors
				if (!CloseAllEditors()) {
					e.Cancel = true;
					return;
				}

				// User closes the form - close parent
				ClosingAction = CloseAction.FullClose;
			}
			codeClosing = false;
		}

		/// <summary>
		/// Form closed totally
		/// </summary>
		private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {

			// Removing hooks
			Preview.PreviewReady -= Preview_PreviewReady;
			Project.EntriesChangedEvent -= Project_EntriesChangedEvent;

			// Stopping timers
			logicTimer.Stop();

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
