using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cubed.Data.Editor.Attributes;
using System.Reflection;

namespace Cubed.Forms.Inspections {

	/// <summary>
	/// Inspector for big objects
	/// </summary>
	public partial class BigInspector : UserControl {

		/// <summary>
		/// Field changed event
		/// </summary>
		public event EventHandler FieldChanged;

		/// <summary>
		/// Target object
		/// </summary>
		public object Target {
			get {
				return target;
			}
			set {
				target = value;
				Rebuild();
			}
		}

		/// <summary>
		/// Hidden target object
		/// </summary>
		object target;

		/// <summary>
		/// Constructor for inspector
		/// </summary>
		public BigInspector() {
			InitializeComponent();
		}

		/// <summary>
		/// Building controller
		/// </summary>
		void Rebuild() {
			SuspendLayout();
			itemList.TabPages.Clear();
			if (target != null) {

				// Picking info
				Type type = target.GetType();
				PropertyInfo[] fields = type.GetProperties();

				// Creating tabs
				foreach (PropertyInfo fi in fields) {
					if (fi.CanRead && fi.PropertyType.IsClass) {

						// Filling name
						string name = fi.Name;
						InspectorNameAttribute nameAttrib = (InspectorNameAttribute)Attribute.GetCustomAttribute(fi, typeof(InspectorNameAttribute));
						if (nameAttrib != null) {
							name = nameAttrib.Name;
						}

						// Creating tab
						TabPage tp = new TabPage();
						tp.Text = name;

						Inspector insp = new Inspector();
						insp.InfoPanelVisible = false;
						insp.Location = Point.Empty;
						insp.Dock = DockStyle.Fill;
						insp.FieldChanged += insp_FieldChanged;
						tp.Controls.Add(insp);
						insp.Target = fi.GetValue(target);

						itemList.TabPages.Add(tp);
					}
				}
				
			}
			ResumeLayout();
		}

		/// <summary>
		/// Changed data
		/// </summary>
		void insp_FieldChanged(object sender, EventArgs e) {
			if (FieldChanged != null) {
				FieldChanged(sender, EventArgs.Empty);
			}
		}
	}
}
