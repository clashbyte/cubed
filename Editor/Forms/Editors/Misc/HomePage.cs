using System.Drawing;
using System.Windows.Forms;
using Cubed.Forms.Common;
using Cubed.Forms.Resources;
using Cubed.UI.Controls;
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
			tabName = CustomEditors.StartingPage;
			tabIcon = new UIIcon(DirectoryInspectorIcons.Home);

			SuspendLayout();
			if (Program.AllowBrowser) {
				CefSharp.WinForms.ChromiumWebBrowser cwb = new CefSharp.WinForms.ChromiumWebBrowser("https://clashbyte.ru/cubed/home");
				cwb.Dock = DockStyle.Fill;
				Controls.Add(cwb);
			} else {
				NSLabel label = new NSLabel() {
					Text = "Unable to find CEFSharp bindings",
					Dock = DockStyle.Fill,
					ForeColor = Color.LightGray,
					TextAlign = ContentAlignment.MiddleCenter
				};
				Controls.Add(label);
			}
			ResumeLayout();
		}
	}
}
