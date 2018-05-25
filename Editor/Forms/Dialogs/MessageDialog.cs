using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cubed.Forms.Dialogs {
	public partial class MessageDialog : Form {

		/// <summary>
		/// Title of textbox
		/// </summary>
		public string Title {
			get;
			set;
		}

		/// <summary>
		/// Content
		/// </summary>
		public string Content {
			get;
			set;
		}

		/// <summary>
		/// Message box button set
		/// </summary>
		public MessageBoxButtons Buttons {
			get;
			set;
		}

		/// <summary>
		/// Icon and sound
		/// </summary>
		public new MessageBoxIcon Icon {
			get;
			set;
		}

		/// <summary>
		/// Opening the box
		/// </summary>
		/// <param name="label">Title text</param>
		/// <param name="text">Contained info</param>
		/// <param name="buttons">Buttons to display</param>
		/// <param name="icon">Message icon</param>
		public static DialogResult Open(string label, string text, MessageBoxButtons buttons, MessageBoxIcon icon) {
			MessageDialog md = new MessageDialog() {
				Title = label,
				Content = text,
				Buttons = buttons,
				Icon = icon
			};
			return md.ShowDialog();
		}




		public MessageDialog() {
			InitializeComponent();
		}

		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			switch (Buttons) {
				case MessageBoxButtons.OKCancel:
					button1.Visible = false;
					button2.Visible = true;
					button3.Visible = true;

					
					break;
				case MessageBoxButtons.AbortRetryIgnore:
					break;
				case MessageBoxButtons.YesNoCancel:
					break;
				case MessageBoxButtons.YesNo:
					break;
				case MessageBoxButtons.RetryCancel:
					break;
				default:
					break;
			}
		}
	}
}
