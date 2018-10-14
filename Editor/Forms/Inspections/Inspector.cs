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
using Cubed.Data.Editor.Attributes;
using OpenTK;
using Cubed.Graphics;
using Cubed.UI.Graphics;

namespace Cubed.Forms.Inspections {
	public partial class Inspector : Panel {

		/// <summary>
		/// Assigned editors for props
		/// </summary>
		static readonly Dictionary<Type, Type> fieldTypes = new Dictionary<Type, Type>() {
			{ typeof(string),	typeof(StringFieldInspector) },
			{ typeof(bool),		typeof(BoolFieldInspector) },
			{ typeof(int),		typeof(NumberFieldInspector) },
			{ typeof(uint),		typeof(NumberFieldInspector) },
			{ typeof(short),	typeof(NumberFieldInspector) },
			{ typeof(ushort),	typeof(NumberFieldInspector) },
			{ typeof(byte),		typeof(NumberFieldInspector) },
			{ typeof(sbyte),	typeof(NumberFieldInspector) },
			{ typeof(long),		typeof(NumberFieldInspector) },
			{ typeof(ulong),	typeof(NumberFieldInspector) },
			{ typeof(float),	typeof(NumberFieldInspector) },
			{ typeof(Vector2),	typeof(Vector2FieldInspector) },
			{ typeof(Vector3),	typeof(Vector3FieldInspector) },
			{ typeof(Color),	typeof(ColorFieldInspector) },
			{ typeof(Texture),	typeof(FileFieldInspector) },
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
		/// Field groups
		/// </summary>
		FieldGroup[] fieldGroups;

		/// <summary>
		/// Logic update timer
		/// </summary>
		Timer updateTimer;

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
			SuspendLayout();
			if (target != null) {

				// Picking info
				Type type = target.GetType();
				NSDirectoryInspector.Entry mainInfo = new NSDirectoryInspector.Entry();
				InspectorNameAttribute nameAttrib = (InspectorNameAttribute)Attribute.GetCustomAttribute(type, typeof(InspectorNameAttribute));
				if (nameAttrib != null) {
					mainInfo.Name = nameAttrib.Name;
				} else {
					mainInfo.Name = type.Name;
				}
				InspectorDescriptionAttribute descAttrib = (InspectorDescriptionAttribute)Attribute.GetCustomAttribute(type, typeof(InspectorDescriptionAttribute));
				if (descAttrib != null) {
					mainInfo.SubName = descAttrib.Description;
				}
				InspectorIconAttribute iconAttrib = (InspectorIconAttribute)Attribute.GetCustomAttribute(type, typeof(InspectorIconAttribute));
				if (iconAttrib != null) {
					mainInfo.MainIcon = iconAttrib.Icon;
				}
				fileInfo.File = mainInfo;

				// Reading sections
				Dictionary<int, FieldGroup> groups = new Dictionary<int, FieldGroup>();
				InspectorSectionAttribute[] sectionAttribs = (InspectorSectionAttribute[])Attribute.GetCustomAttributes(type, typeof(InspectorSectionAttribute));
				if (sectionAttribs == null) {
					sectionAttribs = new InspectorSectionAttribute[0];
				}
				foreach (InspectorSectionAttribute sattr in sectionAttribs) {
					if (!groups.ContainsKey(sattr.ID)) {
						groups.Add(sattr.ID, new FieldGroup());
					}
					FieldGroup fg = groups[sattr.ID];
					fg.Name = sattr.Name;
					fg.Icon = sattr.Icon;
				}
				if (!groups.ContainsKey(-1)) {
					groups.Add(-1, new FieldGroup() {
						Name = InspectorNameAttribute.GetName("DefaultGroup"),
						Icon = InspectorIconAttribute.GetIcon("DefaultGroup"),
					});
				}

				// Reading properties
				PropertyInfo[] pinfos = type.GetProperties();
				foreach (PropertyInfo p in pinfos) {
					if (p.CanRead && p.GetGetMethod().IsPublic && Attribute.GetCustomAttribute(p, typeof(InspectorHiddenAttribute)) == null) {
						int section = -1;
						InspectorSectionAttribute sattr = (InspectorSectionAttribute)Attribute.GetCustomAttribute(p, typeof(InspectorSectionAttribute));
						if (sattr != null) {
							if (groups.ContainsKey(sattr.ID)) {
								section = sattr.ID;
							}
						}
						groups[section].Properties.Add(p);
					}
				}

				// Halting
				int py = 0;
				List<Control> controlsToDispose = new List<Control>();
				foreach (Control c in subPanel.Controls) {
					controlsToDispose.Add(c);
				}
				hostPanel.SuspendLayout();
				subPanel.SuspendLayout();
				subPanel.Controls.Clear();
				foreach (Control c in controlsToDispose) {
					c.Dispose();
				}

				// Iterating fields
				int[] indices = groups.Keys.ToArray();
				FieldGroup[] fgroups = groups.Values.ToArray();
				Array.Sort(indices, fgroups);
				foreach (FieldGroup fg in fgroups) {
					if (fg.Properties.Count > 0) {

						// Adding separator
						InspectorSplitter splitter = new InspectorSplitter(fg.Name, fg.Icon);
						splitter.Location = new Point(0, py + 3);
						splitter.Size = new Size(hostPanel.Width, splitter.Height);
						splitter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
						subPanel.Controls.Add(splitter);
						py += splitter.Height + 6;

						// Adding fields
						foreach (PropertyInfo p in fg.Properties) {
							Type tp = null;
							if (p.PropertyType.IsEnum) {
								tp = typeof(EnumFieldInspector);
							} else if (fieldTypes.ContainsKey(p.PropertyType)) {
								tp = fieldTypes[p.PropertyType];
							}
							if (tp != null) {
								FieldInspector fi = Activator.CreateInstance(tp) as FieldInspector;
								InspectorEntry ie = new InspectorEntry(p, fi);
								fi.SetParent(this, p, p.CanWrite && p.GetGetMethod().IsPublic);
								fi.UpdateValue(); 
								
								ie.Location = new Point(0, py);
								ie.Size = new Size(hostPanel.Width, ie.Height);
								ie.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
								subPanel.Controls.Add(ie);
								py += ie.Height;
								fg.Entries.Add(p, ie);
							}
						}
						py += 10;
						
					}
				}
				totalHeight = py;
				subPanel.ResumeLayout();
				hostPanel.ResumeLayout();
				fieldGroups = fgroups;
			} else {
				totalHeight = 1;
				fieldGroups = null;
			}
			hostPanel.Visible = infoPanel.Visible = target != null;
			emptyLabel.Visible = target == null;
			ResumeLayout();
			AdjustScrollbar();

			if (fieldGroups != null) {
				if (updateTimer == null) {
					updateTimer = new Timer();
					updateTimer.Tick += LogicTimerTick;
					updateTimer.Interval = 100;
					updateTimer.Start();
				}
			} else {
				if (updateTimer != null) {
					updateTimer.Stop();
					updateTimer = null;
				}
			}
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
				scrollBar.Value = 0;
			}
			subPanel.Size = new System.Drawing.Size(hostPanel.Width, totalHeight);
		}

		/// <summary>
		/// Timer for logic update
		/// </summary>
		void LogicTimerTick(object sender, EventArgs e) {
			foreach (FieldGroup fg in fieldGroups) {
				foreach (var item in fg.Entries) {
					item.Value.FieldInsp.UpdateValue();
				}
			}
		}

		/// <summary>
		/// Field group
		/// </summary>
		class FieldGroup {

			/// <summary>
			/// Name
			/// </summary>
			public string Name;

			/// <summary>
			/// Icon
			/// </summary>
			public UIIcon Icon;

			/// <summary>
			/// Entries
			/// </summary>
			public List<PropertyInfo> Properties;

			/// <summary>
			/// Entries
			/// </summary>
			public Dictionary<PropertyInfo, InspectorEntry> Entries;

			/// <summary>
			/// Constructor
			/// </summary>
			public FieldGroup() {
				Properties = new List<PropertyInfo>();
				Entries = new Dictionary<PropertyInfo, InspectorEntry>();
			}

		}
	}
}
