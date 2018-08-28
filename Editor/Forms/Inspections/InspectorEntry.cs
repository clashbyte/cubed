using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Cubed.Data.Editor.Attributes;
using Cubed.Forms.Inspections.Fields;
using Cubed.UI.Controls;

namespace Cubed.Forms.Inspections {

	/// <summary>
	/// Entry
	/// </summary>
	class InspectorEntry : Panel {

		/// <summary>
		/// Entry label
		/// </summary>
		public override string Text {
			get {
				return label.Text;
			}
			set {
				label.Text = value;
				label.Invalidate();
			}
		}

		/// <summary>
		/// Content height
		/// </summary>
		public int Height {
			get {
				if (field != null) {
					return field.Height;
				}
				return 20;
			}
		}

		/// <summary>
		/// Current field
		/// </summary>
		FieldInspector field;

		/// <summary>
		/// Current label
		/// </summary>
		NSLabel label;

		/// <summary>
		/// Entry constructor
		/// </summary>
		public InspectorEntry(PropertyInfo info, FieldInspector fi) {


			// Text
			string name = info.Name;
			Size = new System.Drawing.Size(Width, fi.Height);
			InspectorNameAttribute nameAttrib = (InspectorNameAttribute)Attribute.GetCustomAttribute(info, typeof(InspectorNameAttribute));
			if (nameAttrib != null) {
				name = nameAttrib.Name;
			}

			// Label
			label = new NSLabel() {
				ForeColor = info.CanWrite ? Color.White : Color.DarkGray,
				Width = 125,
				Height = 30,
				Location = new Point(5, 0),
				TextAlign = ContentAlignment.MiddleLeft,
				Text = name
			};

			// Handling
			SuspendLayout();
			Controls.Add(label);

			// Adding field inspector
			field = fi;
			Controls.Add(field);
			field.Location = new Point(130, 0);
			field.Size = new System.Drawing.Size(Width - 130, field.Height);
			ResumeLayout();
		}

		/// <summary>
		/// Changing size
		/// </summary>
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if (field != null) {
				field.Location = new Point(130, 0);
				field.Size = new System.Drawing.Size(Width - 130, field.Height);
			}
		}

	}
}
