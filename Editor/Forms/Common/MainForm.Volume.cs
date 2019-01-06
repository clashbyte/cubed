using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubed.Forms.Common {

	partial class MainForm {

		/// <summary>
		/// Volume options
		/// </summary>
		VolumeControlForm volumeForm;

		/// <summary>
		/// Creating form for next use
		/// </summary>
		void InitVolumeForm() {
			volumeForm = new VolumeControlForm();
			volumeForm.TopLevel = false;
			Controls.Add(volumeForm);
			volumeForm.BringToFront();
			volumeForm.VisibleChanged += VolumeForm_VisibleChanged;
		}

		/// <summary>
		/// Visibility changed
		/// </summary>
		void VolumeForm_VisibleChanged(object sender, EventArgs e) {
			bool state = volumeForm.Visible;
			if (state != volumeButton.Checked) {
				volumeButton.Checked = state;
			}
		}

		/// <summary>
		/// Volume button checked
		/// </summary>
		void volumeButton_CheckedChanged(object sender) {
			if (volumeButton.Checked) {
				System.Drawing.Point point = volumeButton.PointToScreen(new System.Drawing.Point(0, -volumeForm.Height));
				point = PointToClient(point);
				volumeForm.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
				volumeForm.Location = point;
				volumeForm.BringToFront();
				volumeForm.Show();
			} else {
				volumeForm.Hide();
			}
		}

	}
}
