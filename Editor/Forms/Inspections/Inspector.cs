using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.UI.Controls;
using System.Reflection;
using Cubed.Forms.Inspections.Fields;

namespace Cubed.Forms.Inspections {
	public partial class Inspector : Panel {

		/// <summary>
		/// Assigned editors for props
		/// </summary>
		static readonly Dictionary<Type, Type> fieldTypes = new Dictionary<Type, Type>() {
			{ typeof(string), typeof(StringFieldInspector) },
			{ typeof(bool), typeof(BoolFieldInspector) },
		};


		/// <summary>
		/// Target object
		/// </summary>
		public object Target {
			get {
				return target;
			}
			set {
				if (target != value) {
					target = value;
					Rebuild();
				}
			}
		}

		/// <summary>
		/// Target object
		/// </summary>
		object target;

		/// <summary>
		/// Current fields
		/// </summary>
		Dictionary<PropertyInfo, InspectorEntry> fields;

		/// <summary>
		/// Total elements height
		/// </summary>
		int totalHeight;

		/// <summary>
		/// Current scroll
		/// </summary>
		int scrollPos;

		/// <summary>
		/// Empty label
		/// </summary>
		NSLabel emptyLabel;

		/// <summary>
		/// Main container panel
		/// </summary>
		Panel hostPanel;

		/// <summary>
		/// Internal panel
		/// </summary>
		Panel subPanel;

		/// <summary>
		/// Panel for information
		/// </summary>
		Panel infoPanel;

		/// <summary>
		/// Information about file
		/// </summary>
		NSFileInfo fileInfo;

		/// <summary>
		/// Scroll bar
		/// </summary>
		NSVScrollBar scrollBar;

		/// <summary>
		/// Inspector constructor
		/// </summary>
		public Inspector() {

			// Empty label
			emptyLabel = new NSLabel() {
				Text = "Select object to edit properties",
				Dock = DockStyle.Fill,
				TextAlign = ContentAlignment.MiddleCenter,
				ForeColor = Color.DarkGray
			};

			// Creating hosting panel
			subPanel = new Panel() {
				AutoScroll = false
			};
			subPanel.Location = new Point(0, 0);
			
			// Creating hosting panel
			hostPanel = new Panel() {
				AutoScroll = false,
				Dock = DockStyle.Fill
			};
			hostPanel.Controls.Add(subPanel);

			// Scroll bar
			scrollBar = new NSVScrollBar() {
				Dock = DockStyle.Right
			};
			scrollBar.Scroll += scrollBar_Scroll;

			// File info
			fileInfo = new NSFileInfo() {
				Dock = DockStyle.Fill
			};
			infoPanel = new Panel() {
				AutoScroll = false,
				Dock = DockStyle.Top,
				BackColor = Color.Lime,
				Size = new Size(1, 57)
			};
			infoPanel.Controls.Add(fileInfo);


			DoubleBuffered = true;
			BackColor = Color.FromArgb(50, 50, 50);
			SuspendLayout();
			Controls.Add(emptyLabel);
			Controls.Add(hostPanel);
			Controls.Add(scrollBar);
			Controls.Add(infoPanel);
			ResumeLayout();
			Rebuild();
		}


		/// <summary>
		/// Rebuilding all components
		/// </summary>
		void Rebuild() {
			if (target != null) {

				// Decoding properties
				hostPanel.SuspendLayout();
				subPanel.SuspendLayout();
				subPanel.Controls.Clear();
				Type type = target.GetType();
				PropertyInfo[] pinfos = type.GetProperties();
				fields = new Dictionary<PropertyInfo, InspectorEntry>();
				int py = 0;
				foreach (PropertyInfo p in pinfos) {
					if (p.CanRead) {
						Type tp = null;
						if (p.PropertyType.IsEnum) {
							tp = typeof(EnumFieldInspector);
						} else if (fieldTypes.ContainsKey(p.PropertyType)) {
							tp = fieldTypes[p.PropertyType];
						}
						if (tp != null) {
							FieldInspector fi = Activator.CreateInstance(tp) as FieldInspector;
							InspectorEntry ie = new InspectorEntry(p, fi);
							fi.SetParent(this, p, p.CanWrite);
							fi.UpdateValue();

							subPanel.Controls.Add(ie);
							ie.Location = new Point(0, py);
							ie.Size = new Size(subPanel.Width, ie.Height);
							ie.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
							py += ie.Height;
							fields.Add(p, ie);
						}
					}
				}
				totalHeight = py;
				subPanel.Size = new System.Drawing.Size(hostPanel.Width, py);
				subPanel.ResumeLayout();
				hostPanel.ResumeLayout();

			} else {
				subPanel.Size = new System.Drawing.Size(hostPanel.Width, 1);
			}
			hostPanel.Visible = infoPanel.Visible = target != null;
			emptyLabel.Visible = target == null;
			AdjustScrollbar();
		}

		/// <summary>
		/// Control resized
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			hostPanel.SuspendLayout();
			subPanel.Size = new Size(hostPanel.Width, totalHeight);
			hostPanel.ResumeLayout();
			AdjustScrollbar();
		}

		/// <summary>
		/// Scrolling by wheel
		/// </summary>
		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			if (scrollBar.Visible) {
				int pos = scrollBar.Value;
				pos += -e.Delta / 6;
				scrollBar.Value = Math.Max(Math.Min(pos, scrollBar.Maximum - 1), 0);
			}
		}

		/// <summary>
		/// Scrolling data
		/// </summary>
		void scrollBar_Scroll(object sender) {
			SuspendLayout();
			subPanel.Location = new Point(0, -scrollBar.Value);
			ResumeLayout();
		}

		/// <summary>
		/// Adjusting scrollbar
		/// </summary>
		void AdjustScrollbar() {
			if (totalHeight > hostPanel.Height) {
				scrollBar.Visible = true;
				scrollBar.Enabled = true;
				scrollBar.Minimum = 0;
				scrollBar.Maximum = totalHeight - hostPanel.Height;
				scrollBar.Value = 0;
			} else {
				scrollBar.Visible = false;
			}
		}
	}
}
