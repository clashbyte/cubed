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
using Cubed.UI.Graphics;

namespace Cubed.Forms.Inspections {

	/// <summary>
	/// Entry
	/// </summary>
	class InspectorSplitter : Panel {

		/// <summary>
		/// Entry label
		/// </summary>
		public override string Text {
			get {
				return name;
			}
			set {
				name = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Content height
		/// </summary>
		public UIIcon Icon {
			get {
				return icon;
			}
			set {
				icon = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Splitter name
		/// </summary>
		string name;

		/// <summary>
		/// Icon
		/// </summary>
		UIIcon icon;

		/// <summary>
		/// Entry constructor
		/// </summary>
		public InspectorSplitter(string name, UIIcon icon) {
			this.name = name;
			this.icon = icon;
			DoubleBuffered = true;
			Size = new Size(100, 30);
			Font = new System.Drawing.Font("Tahoma", 8.25f);
		}

		/// <summary>
		/// Changing size
		/// </summary>
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			Invalidate();
		}

		/// <summary>
		/// Painting
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			System.Drawing.Graphics g = e.Graphics;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			g.Clear(Color.FromArgb(45, 45, 45));
			g.DrawRectangle(new Pen(Color.FromArgb(35, 35, 35)), 0, 0, Width - 1, Height - 1);

			int w = 10;
			if (icon != null && icon.Scan != null) {
				icon.Draw(g, new Rectangle(7, 7, Height - 14, Height - 14));
				w += Height - 7;
			}
			if (name != null) {
				SizeF ssz = g.MeasureString(name, Font);
				Point p = new Point(w, Height / 2 - (int)(ssz.Height / 2f));

				g.DrawString(name, Font, Brushes.Black, p.X + 1, p.Y + 1);
				g.DrawString(name, Font, Brushes.White, p.X, p.Y);
			}
		}

	}
}
