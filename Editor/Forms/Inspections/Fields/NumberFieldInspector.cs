using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Data.Editor.Attributes;

namespace Cubed.Forms.Inspections.Fields {
	class NumberFieldInspector : FieldInspector {
		private Nested.NumberEditor numberEditor;

		/// <summary>
		/// Flag for type association
		/// </summary>
		bool typeSet = false;

		/// <summary>
		/// Type of number field
		/// </summary>
		Type fieldType = null;

		/// <summary>
		/// Custom data
		/// </summary>
		public NumberFieldInspector()
			: base() {
				InitializeComponent();
		}

		/// <summary>
		/// Updaing value
		/// </summary>
		public override void UpdateValue() {
			if (!typeSet) {
				fieldType = Info.PropertyType;
				numberEditor.IsInteger = fieldType != typeof(float);
				float minVal = float.MinValue;
				float maxVal = float.MaxValue;
				if (fieldType == typeof(int)) {
					minVal = int.MinValue;
					maxVal = int.MaxValue;
				} else if (fieldType == typeof(uint)) {
					minVal = uint.MinValue;
					maxVal = uint.MaxValue;
				} else if (fieldType == typeof(long)) {
					minVal = long.MinValue;
					maxVal = long.MaxValue;
				} else if (fieldType == typeof(ulong)) {
					minVal = ulong.MinValue;
					maxVal = ulong.MaxValue;
				} else if (fieldType == typeof(short)) {
					minVal = short.MinValue;
					maxVal = short.MaxValue;
				} else if (fieldType == typeof(ushort)) {
					minVal = ushort.MinValue;
					maxVal = ushort.MaxValue;
				} else if (fieldType == typeof(byte)) {
					minVal = byte.MinValue;
					maxVal = byte.MaxValue;
				} else if (fieldType == typeof(sbyte)) {
					minVal = sbyte.MinValue;
					maxVal = sbyte.MaxValue;
				}
				
				InspectorRangeAttribute ranges = Attribute.GetCustomAttribute(Info, typeof(InspectorRangeAttribute)) as InspectorRangeAttribute;
				if (ranges != null) {
					float min = Convert.ToSingle(ranges.Min);
					float max = Convert.ToSingle(ranges.Max);
					numberEditor.Min = Math.Max(min, minVal);
					numberEditor.Max = Math.Min(max, maxVal);
				} else {
					numberEditor.Min = minVal;
					numberEditor.Max = maxVal;
				}

				typeSet = true;
			}
			object obj = ReadValue();
			if (obj != null) {
				numberEditor.Value = Convert.ToSingle(obj);
			}
		}

		/// <summary>
		/// Component initializing
		/// </summary>
		private void InitializeComponent() {
			this.numberEditor = new Cubed.Forms.Inspections.Fields.Nested.NumberEditor();
			this.SuspendLayout();
			// 
			// numberEditor
			// 
			this.numberEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.numberEditor.IsInteger = false;
			this.numberEditor.Label = null;
			this.numberEditor.Location = new System.Drawing.Point(0, 0);
			this.numberEditor.Max = 3.402823E+38F;
			this.numberEditor.Min = -3.402823E+38F;
			this.numberEditor.Name = "numberEditor";
			this.numberEditor.Size = new System.Drawing.Size(360, 30);
			this.numberEditor.TabIndex = 0;
			this.numberEditor.Value = 0F;
			this.numberEditor.ValueChanged += numberEditor_ValueChanged;
			// 
			// NumberFieldInspector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Controls.Add(this.numberEditor);
			this.Name = "NumberFieldInspector";
			this.ResumeLayout(false);

		}

		/// <summary>
		/// Handling input
		/// </summary>
		void numberEditor_ValueChanged(object sender, EventArgs e) {
			float val = this.numberEditor.Value;
			SetValue(Convert.ChangeType(val, fieldType));
		}

	}
}
