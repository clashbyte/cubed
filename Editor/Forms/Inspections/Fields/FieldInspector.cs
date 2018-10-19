using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Cubed.Forms.Inspections.Fields {

	/// <summary>
	/// Field inspector
	/// </summary>
	public partial class FieldInspector : UserControl {

		/// <summary>
		/// Current field info
		/// </summary>
		protected PropertyInfo Info {
			get {
				return field;
			}
		}

		/// <summary>
		/// Current inspector
		/// </summary>
		Inspector inspector;

		/// <summary>
		/// Field
		/// </summary>
		PropertyInfo field;

		/// <summary>
		/// Is field writable
		/// </summary>
		bool writable;

		/// <summary>
		/// Constructor
		/// </summary>
		public FieldInspector() {
			InitializeComponent();
		}

		/// <summary>
		/// Setting value
		/// </summary>
		/// <param name="value">Parent</param>
		protected virtual void SetValue(object value) {
			if (writable) {
				if (inspector != null) {
					inspector.CallFieldEvent();
				}
				field.SetValue(inspector.Target, value, null);
			}
		}
		
		/// <summary>
		/// Reading value from object
		/// </summary>
		/// <returns>Value or null</returns>
		protected object ReadValue() {
			if (inspector.Target != null) {
				return field.GetValue(inspector.Target, null);
			}
			return null;
		}

		/// <summary>
		/// Updating value
		/// </summary>
		public virtual void UpdateValue(){}

		/// <summary>
		/// Detecting stuff on parent change
		/// </summary>
		public virtual void DetectType() { }

		/// <summary>
		/// Setting value for element
		/// </summary>
		/// <param name="inspector">Owner inspector</param>
		/// <param name="type">Field type</param>
		/// <param name="writable">Field writable</param>
		public void SetParent(Inspector inspector, PropertyInfo type, bool writable) {
			this.inspector = inspector;
			this.field = type;
			this.writable = writable;
			DetectType();
			UpdateValue();
		}
	}

	
}
