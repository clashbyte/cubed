using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Cubed.Forms.Resources;
using Cubed.UI.Graphics;

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
		/// Hidden button results
		/// </summary>
		DialogResult[] results;

		/// <summary>
		/// Icon to draw
		/// </summary>
		UIIcon icon = null;

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

		/// <summary>
		/// Constructor
		/// </summary>
		public MessageDialog() {
			InitializeComponent();
		}

		/// <summary>
		/// Showing event
		/// </summary>
		/// <param name="e"></param>
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);

			Text = Title;
			contentLabel.Text = Content;
			results = new DialogResult[] {
				DialogResult.Cancel,
				DialogResult.Cancel,
				DialogResult.Cancel
			};

			switch (Buttons) {
				case MessageBoxButtons.OKCancel:
					button1.Visible = false;
					button2.Visible = true;
					button3.Visible = true;

					button2.Text = MessageBoxData.okLabel;
					button3.Text = MessageBoxData.cancelLabel;

					button2.IconImage = MessageBoxData.buttonPositive;
					button3.IconImage = MessageBoxData.buttonNegative;

					results[1] = System.Windows.Forms.DialogResult.OK;
					results[2] = System.Windows.Forms.DialogResult.Cancel;
					break;

				case MessageBoxButtons.AbortRetryIgnore:
					button1.Visible = true;
					button2.Visible = true;
					button3.Visible = true;

					button1.Text = MessageBoxData.retryLabel;
					button2.Text = MessageBoxData.abortLabel;
					button3.Text = MessageBoxData.ignoreLabel;

					button1.IconImage = MessageBoxData.buttonRetry;
					button2.IconImage = MessageBoxData.buttonNegative;
					button3.IconImage = MessageBoxData.buttonIgnore;

					results[0] = System.Windows.Forms.DialogResult.Retry;
					results[1] = System.Windows.Forms.DialogResult.Abort;
					results[2] = System.Windows.Forms.DialogResult.Ignore;
					break;
				case MessageBoxButtons.YesNoCancel:
					button1.Visible = true;
					button2.Visible = true;
					button3.Visible = true;

					button1.Text = MessageBoxData.yesLabel;
					button2.Text = MessageBoxData.noLabel;
					button3.Text = MessageBoxData.cancelLabel;

					button1.IconImage = MessageBoxData.buttonPositive;
					button2.IconImage = MessageBoxData.buttonNegative;
					button3.IconImage = MessageBoxData.buttonIgnore;

					results[0] = System.Windows.Forms.DialogResult.Yes;
					results[1] = System.Windows.Forms.DialogResult.No;
					results[2] = System.Windows.Forms.DialogResult.Cancel;
					break;
				case MessageBoxButtons.YesNo:
					button1.Visible = false;
					button2.Visible = true;
					button3.Visible = true;

					button2.Text = MessageBoxData.yesLabel;
					button3.Text = MessageBoxData.noLabel;

					button2.IconImage = MessageBoxData.buttonPositive;
					button3.IconImage = MessageBoxData.buttonNegative;

					results[1] = System.Windows.Forms.DialogResult.Yes;
					results[2] = System.Windows.Forms.DialogResult.No;
					break;
				case MessageBoxButtons.RetryCancel:
					button1.Visible = false;
					button2.Visible = true;
					button3.Visible = true;

					button2.Text = MessageBoxData.retryLabel;
					button3.Text = MessageBoxData.cancelLabel;

					button2.IconImage = MessageBoxData.buttonRetry;
					button3.IconImage = MessageBoxData.buttonNegative;

					results[1] = System.Windows.Forms.DialogResult.Retry;
					results[2] = System.Windows.Forms.DialogResult.Cancel;
					break;
				default:
					button1.Visible = false;
					button2.Visible = false;
					button3.Visible = true;

					button3.Text = MessageBoxData.okLabel;

					button3.IconImage = MessageBoxData.buttonPositive;

					results[2] = System.Windows.Forms.DialogResult.OK;
					break;
			}

			switch (Icon) {
				case MessageBoxIcon.Asterisk:
					icon = new UIIcon(MessageBoxData.infoIcon);
					break;
				case MessageBoxIcon.Error:
					icon = new UIIcon(MessageBoxData.errorIcon);
					break;
				case MessageBoxIcon.Exclamation:
					icon = new UIIcon(MessageBoxData.warningIcon);
					break;
				case MessageBoxIcon.Question:
					icon = new UIIcon(MessageBoxData.questionIcon);
					break;
			}

			iconPanel.Invalidate();
		}

		/// <summary>
		/// Drawing icon
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void iconPanel_Paint(object sender, PaintEventArgs e) {
			System.Drawing.Graphics g = e.Graphics;
			g.Clear(Color.FromArgb(50, 50, 50));
			if (icon != null) {
				icon.Draw(g, new Rectangle(0, 0, 64, 64), 2);
			}
		}

		/// <summary>
		/// Button hit
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_Click(object sender, EventArgs e) {
			if (sender == button1) {
				DialogResult = results[0];
			} else if(sender == button2) {
				DialogResult = results[1];
			} else {
				DialogResult = results[2];
			}
		}
	}
}
