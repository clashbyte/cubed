using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cubed.Forms.Dialogs {

	/// <summary>
	/// Form for text input
	/// </summary>
	public partial class TextInputDialog : Form {

		/// <summary>
		/// Validation delegate
		/// </summary>
		/// <param name="input">Value</param>
		/// <returns>True if value is correct</returns>
		public delegate bool ValidateValue(string input);

		/// <summary>
		/// Validator func
		/// </summary>
		public ValidateValue Validator {
			get;
			set;
		}

		/// <summary>
		/// Description text
		/// </summary>
		public string Description {
			get;
			set;
		}

		/// <summary>
		/// Main value
		/// </summary>
		public string Value {
			get;
			set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public TextInputDialog() {
			InitializeComponent();
			Description = "Input text:";
		}

		/// <summary>
		/// Applying text
		/// </summary>
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			descLabel.Text = Description;
			inputBox.Text = Value;
			Check();
		}

		/// <summary>
		/// Submitting text
		/// </summary>
		void submitButton_Click(object sender, EventArgs e) {
			Value = inputBox.Text;
			DialogResult = DialogResult.OK;
		}

		/// <summary>
		/// Closing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void cancelButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}

		/// <summary>
		/// Validating input
		/// </summary>
		void inputBox_TextChanged(object sender, EventArgs e) {
			Check();
		}

		/// <summary>
		/// Checking value
		/// </summary>
		void Check() {
			bool allow = true;
			if (Validator != null) {
				allow = Validator(inputBox.Text);
			}
			submitButton.Enabled = allow;
		}
	}
}
