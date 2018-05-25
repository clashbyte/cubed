using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cubed.Forms.Common;
using Cubed.Forms.Dialogs;
using Cubed.Map;
using Cubed.Windows;

namespace Cubed {
	static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			MessageDialog.Open("Test", "More test", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question);


			//Application.Run(new StartupForm());
			

			//MapEditor.Start();
		}
	}
}
