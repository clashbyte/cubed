using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Data.Editor.Attributes;
using OpenTK;

namespace Cubed.Forms.Inspections.Fields {
	class Vector3FieldInspector : FieldInspector {

		/// <summary>
		/// Flag for type association
		/// </summary>
		private Nested.NumberEditor fieldX;
		private Nested.NumberEditor fieldY;
		private Nested.NumberEditor fieldZ;

		/// <summary>
		/// Flag for range detection
		/// </summary>
		bool rangesDetected = false;

		/// <summary>
		/// Hidden value
		/// </summary>
		Vector3 value;

		/// <summary>
		/// Custom data
		/// </summary>
		public Vector3FieldInspector()
			: base() {
				InitializeComponent();
		}

		/// <summary>
		/// Resizing
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if (fieldX != null && fieldY != null && fieldZ != null) {
				float ix = (float)Width / 3f;
				fieldX.Size = new System.Drawing.Size((int)ix, Height);
				fieldY.Size = new System.Drawing.Size((int)ix, Height);
				fieldZ.Size = new System.Drawing.Size((int)ix, Height);
				fieldX.Location = new System.Drawing.Point(0, 0);
				fieldY.Location = new System.Drawing.Point((int)ix, 0);
				fieldZ.Location = new System.Drawing.Point((int)(ix * 2), 0);
			}
		}

		/// <summary>
		/// Updaing value
		/// </summary>
		public override void UpdateValue() {
			if (!rangesDetected) {
				InspectorRangeAttribute ranges = Attribute.GetCustomAttribute(Info, typeof(InspectorRangeAttribute)) as InspectorRangeAttribute;
				if (ranges != null) {
					Vector3 min = (Vector3)ranges.Min;
					Vector3 max = (Vector3)ranges.Max;
					fieldX.Min = min.X;
					fieldY.Min = min.Y;
					fieldZ.Min = min.Z;
					fieldX.Max = max.X;
					fieldY.Max = max.Y;
					fieldZ.Max = max.Z;
				}
				rangesDetected = true;
			}
			object obj = ReadValue();
			if (obj != null && !ContainsFocus) {
				Vector3 vec = (Vector3)obj;
				value = vec;
				fieldX.Value = vec.X;
				fieldY.Value = vec.Y;
				fieldZ.Value = vec.Z;
			}
		}

		/// <summary>
		/// Component initializing
		/// </summary>
		private void InitializeComponent() {
			this.fieldX = new Cubed.Forms.Inspections.Fields.Nested.NumberEditor();
			this.fieldY = new Cubed.Forms.Inspections.Fields.Nested.NumberEditor();
			this.fieldZ = new Cubed.Forms.Inspections.Fields.Nested.NumberEditor();
			this.SuspendLayout();
			// 
			// fieldX
			// 
			this.fieldX.IsInteger = false;
			this.fieldX.Label = null;
			this.fieldX.Location = new System.Drawing.Point(0, 0);
			this.fieldX.Max = 3.402823E+38F;
			this.fieldX.Min = -3.402823E+38F;
			this.fieldX.Name = "fieldX";
			this.fieldX.Size = new System.Drawing.Size(117, 30);
			this.fieldX.TabIndex = 0;
			this.fieldX.Value = 0F;
			this.fieldX.ValueChanged += numberEditor_ValueChanged;
			// 
			// fieldY
			// 
			this.fieldY.IsInteger = false;
			this.fieldY.Label = "Y";
			this.fieldY.Location = new System.Drawing.Point(122, 0);
			this.fieldY.Max = 3.402823E+38F;
			this.fieldY.Min = -3.402823E+38F;
			this.fieldY.Name = "fieldY";
			this.fieldY.Size = new System.Drawing.Size(117, 30);
			this.fieldY.TabIndex = 1;
			this.fieldY.Value = 0F;
			this.fieldY.ValueChanged += numberEditor_ValueChanged;
			// 
			// fieldZ
			// 
			this.fieldZ.IsInteger = false;
			this.fieldZ.Label = "Z";
			this.fieldZ.Location = new System.Drawing.Point(240, 0);
			this.fieldZ.Max = 3.402823E+38F;
			this.fieldZ.Min = -3.402823E+38F;
			this.fieldZ.Name = "fieldZ";
			this.fieldZ.Size = new System.Drawing.Size(117, 30);
			this.fieldZ.TabIndex = 2;
			this.fieldZ.Value = 0F;
			this.fieldZ.ValueChanged += numberEditor_ValueChanged;
			// 
			// Vector3FieldInspector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Controls.Add(this.fieldZ);
			this.Controls.Add(this.fieldY);
			this.Controls.Add(this.fieldX);
			this.Name = "Vector3FieldInspector";
			this.ResumeLayout(false);

		}

		/// <summary>
		/// Handling input
		/// </summary>
		void numberEditor_ValueChanged(object sender, EventArgs e) {
			Nested.NumberEditor ne = sender as Nested.NumberEditor;
			if (ne == fieldX) {
				value.X = ne.Value;
			} else if (ne == fieldY) {
				value.Y = ne.Value;
			} else {
				value.Z = ne.Value;
			}
			SetValue(value);
		}

	}
}
