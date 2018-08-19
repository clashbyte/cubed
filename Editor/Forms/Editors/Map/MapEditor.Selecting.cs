using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Components.Rendering;
using Cubed.Editing;
using Cubed.Maths;
using Cubed.UI;
using Cubed.World;
using OpenTK;

namespace Cubed.Forms.Editors.Map {

	partial class MapEditor {

		Entity ent;

		UserInterface ui = null;

		UI.Basic.Label tlabel;

		/// <summary>
		/// Currently dragging object
		/// </summary>
		EditableObject draggingNewObject;

		/// <summary>
		/// Screen dragging location
		/// </summary>
		Vector2 draggingScreenLocation;

		void SelectToolOpen() {
			if (ent == null) {
				ent = new Entity();
				ent.AddComponent(new WireCubeComponent() {
					Size = Vector3.One * 0.2f,
					WireColor = System.Drawing.Color.Green,
					WireWidth = 2f
				});
			}
			if (ui == null) {
				ui = new UserInterface();
				ui.Items.Add(tlabel = new UI.Basic.Label() {
					Text = "",
					Position = Vector2.Zero,
					Anchor = Cubed.UI.Control.AnchorMode.TopLeft,
					HorizontalAlign = UserInterface.Align.Start,
					VerticalAlign = UserInterface.Align.Start
				});
			}
			engine.Interface = ui;
			scene.Entities.Add(ent);
		}

		void SelectToolClose() {
			engine.Interface = null;
			scene.Entities.Remove(ent);
		}

		void SelectToolUpdate() {

			// Picking current grid position
			Vector3 camPos = cam.ScreenToPoint(Input.Controls.Mouse.X, Input.Controls.Mouse.Y, 0);
			Vector3 camDir = cam.ScreenToPoint(Input.Controls.Mouse.X, Input.Controls.Mouse.Y, 1) - camPos;
			Vector3 pickPos = Vector3.Zero;
			MapIntersections.Hit hit = MapIntersections.Intersect(camPos, camDir.Normalized(), map);
			string labelText = "";
			if (hit != null) {
				pickPos = hit.Point;
				labelText = "Cell: " + hit.Cell + " Type: " + hit.Type + " Side: " + hit.Side + " Pos: " + pickPos;
			}
			tlabel.Text = labelText;
			ent.Position = pickPos;
		}

		/// <summary>
		/// Entering drag entry
		/// </summary>
		private void screen_DragEnter(object sender, DragEventArgs e) {
			e.Effect = e.AllowedEffect;
			object data = e.Data.GetData(typeof(object));

		}

		/// <summary>
		/// Removing drag
		/// </summary>
		private void screen_DragLeave(object sender, EventArgs e) {
			if (draggingNewObject != null) {
				draggingNewObject.Destroy();
				draggingNewObject = null;
			}
		}

		/// <summary>
		/// Dragging over
		/// </summary>
		private void screen_DragOver(object sender, DragEventArgs e) {
			if (draggingNewObject != null) {

			} else {
				e.Effect = DragDropEffects.None;
			}
		}

		/// <summary>
		/// Dropped object
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void screen_DragDrop(object sender, DragEventArgs e) {
			if (draggingNewObject != null) {

			} else {
				e.Effect = DragDropEffects.None;
			}
		}

	}
}
