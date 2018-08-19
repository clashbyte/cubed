using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Components.Rendering;
using Cubed.Editing;
using Cubed.Maths;
using Cubed.UI;
using Cubed.UI.Controls;
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

		/// <summary>
		/// Opening selection
		/// </summary>
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

		/// <summary>
		/// Closing selection
		/// </summary>
		void SelectToolClose() {
			engine.Interface = null;
			scene.Entities.Remove(ent);
			if (draggingNewObject != null) {
				draggingNewObject.Destroy(scene);
				draggingNewObject = null;
			}
		}

		/// <summary>
		/// Updating
		/// </summary>
		void SelectToolUpdate() {

			// Picking current grid position
			Vector2 mousePos = Input.Controls.Mouse;
			if (draggingNewObject != null) {
				mousePos = draggingScreenLocation;
			}

			Vector3 camPos = cam.ScreenToPoint(mousePos.X, mousePos.Y, 0);
			Vector3 camDir = cam.ScreenToPoint(mousePos.X, mousePos.Y, 1) - camPos;
			Vector3 pickPos = Vector3.Zero;
			MapIntersections.Hit mapHit = MapIntersections.Intersect(camPos, camDir.Normalized(), map);
			if (mapHit != null) {
				pickPos = mapHit.Point;
			}

			tlabel.Text = mousePos.ToString();;


			if (draggingNewObject != null) {
				draggingNewObject.Prefab.Position = pickPos + Vector3.UnitY * 0.1f;
				draggingNewObject.EditorUpdate(scene);
			}
		}

		/// <summary>
		/// Entering drag entry
		/// </summary>
		private void screen_DragEnter(object sender, DragEventArgs e) {
			e.Effect = e.AllowedEffect;
			NSDirectoryInspector.DropData data = (NSDirectoryInspector.DropData)e.Data.GetData(typeof(NSDirectoryInspector.DropData));
			if (data != null) {
				if (data.Content is Type) {
					Type t = data.Content as Type;
					if (typeof(EditableObject).IsAssignableFrom(t)) {
						EditableObject eo = Activator.CreateInstance(t) as EditableObject;
						if (eo != null) {
							e.Effect = e.AllowedEffect;
							draggingNewObject = eo;
							eo.Create(scene);

							Point mouse = screen.PointToClient(new Point(e.X, e.Y));
							draggingScreenLocation = new Vector2(mouse.X, mouse.Y);
						}
					}
				}
			}
			//if (data is Type) {
			//	Type t = data as Type;
			//	System.Diagnostics.Debug.WriteLine(t.Name);
			//} 
		}

		/// <summary>
		/// Removing drag
		/// </summary>
		private void screen_DragLeave(object sender, EventArgs e) {
			if (draggingNewObject != null) {
				draggingNewObject.Destroy(scene);
				draggingNewObject = null;
			}
		}

		/// <summary>
		/// Dragging over
		/// </summary>
		private void screen_DragOver(object sender, DragEventArgs e) {
			if (draggingNewObject != null) {
				Point mouse = screen.PointToClient(new Point(e.X, e.Y));
				draggingScreenLocation = new Vector2(mouse.X, mouse.Y);
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
				Point mouse = screen.PointToClient(new Point(e.X, e.Y));
				draggingScreenLocation = new Vector2(mouse.X, mouse.Y);
				draggingNewObject = null;
			} else {
				e.Effect = DragDropEffects.None;
			}
		}

	}
}
