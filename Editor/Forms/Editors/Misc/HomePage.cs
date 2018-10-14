using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cubed.Forms.Common;
using Cubed.Forms.Resources;
using Cubed.UI.Graphics;

namespace Cubed.Forms.Editors.Misc {

	/// <summary>
	/// Form for chromium
	/// </summary>
	public partial class HomePage : EditorForm {

		/// <summary>
		/// Icon for tab
		/// </summary>
		public override UI.Graphics.UIIcon CustomIcon {
			get {
				return tabIcon;
			}
		}

		/// <summary>
		/// Main text
		/// </summary>
		public override string Text {
			get {
				return tabName;
			}
		}

		/// <summary>
		/// Cached icon
		/// </summary>
		UIIcon tabIcon;

		/// <summary>
		/// Cached name
		/// </summary>
		string tabName;

		/// <summary>
		/// Creating home page form
		/// </summary>
		public HomePage() {
			InitializeComponent();
			tabName = "Starting page";
			tabIcon = new UIIcon(DirectoryInspectorIcons.Home);

			SuspendLayout();
			CefSharp.WinForms.ChromiumWebBrowser cwb = new CefSharp.WinForms.ChromiumWebBrowser("https://clashbyte.ru/cubed/home");
			cwb.Dock = DockStyle.Fill;
			Controls.Add(cwb);
			ResumeLayout();
		}
	}
}
