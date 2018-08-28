using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cubed.Data.Editor.Attributes;
using OpenTK;

namespace Cubed.Forms.Inspections.Fields {
	class Vector2FieldInspector : FieldInspector {

		/// <summary>
		/// Flag for type association
		/// </summary>
		private Nested.NumberEditor fieldX;
		private Nested.NumberEditor fieldY;

		/// <summary>
		/// Flag for range detection
		/// </summary>
		bool rangesDetected = false;

		/// <summary>
		/// Hidden value
		/// </summary>
		Vector2 value;

		/// <summary>
		/// Custom data
		/// </summary>
		public Vector2FieldInspector()
			: base() {
				InitializeComponent();
				OnSizeChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Resizing
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if (fieldX != null && fieldY != null) {
				float ix = (float)Width / 2f;
				fieldX.Size = new System.Drawing.Size((int)ix, Height);
				fieldY.Size = new System.Drawing.Size((int)ix, Height);
				fieldX.Location = new System.Drawing.Point(0, 0);
				fieldY.Location = new System.Drawing.Point((int)ix, 0);
			}
		}

		/// <summary>
		/// Updaing value
		/// </summary>
		public override void UpdateValue() {
			if (!rangesDetected) {
				InspectorRangeAttribute ranges = Attribute.GetCustomAttribute(Info, typeof(InspectorRangeAttribute)) as InspectorRangeAttribute;
				if (ranges != null) {
					Vector2 min = (Vector2)ranges.Min;
					Vector2 max = (Vector2)ranges.Max;
					fieldX.Min = min.X;
					fieldY.Min = min.Y;
					fieldX.Max = max.X;
					fieldY.Max = max.Y;
				}
				rangesDetected = true;
			}
			object obj = ReadValue();
			if (obj != null) {
				Vector2 vec = (Vector2)obj;
				value = vec;
				fieldX.Value = vec.X;
				fieldY.Value = vec.Y;
			}
		}

		/// <summary>
		/// Component initializing
		/// </summary>
		private void InitializeComponent() {
			this.fieldX = new Cubed.Forms.Inspections.Fields.Nested.NumberEditor();
			this.fieldY = new Cubed.Forms.Inspections.Fields.Nested.NumberEditor();
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
			// Vector3FieldInspector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
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
			}
			SetValue(value);
		}

	}
}
