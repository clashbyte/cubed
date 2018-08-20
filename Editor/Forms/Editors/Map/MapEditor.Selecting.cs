using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Components.Rendering;
using Cubed.Core;
using Cubed.Editing;
using Cubed.Maths;
using Cubed.UI;
using Cubed.UI.Controls;
using Cubed.World;
using OpenTK;

namespace Cubed.Forms.Editors.Map {

	partial class MapEditor {
		
		/// <summary>
		/// Main selection interface
		/// </summary>
		UserInterface selectInterface = null;

		/// <summary>
		/// Selected label
		/// </summary>
		UI.Basic.Label selectLabel;

		/// <summary>
		/// Currently dragging object
		/// </summary>
		EditableObject draggingNewObject;

		/// <summary>
		/// Screen dragging location
		/// </summary>
		Vector2 draggingScreenLocation;

		/// <summary>
		/// Current scene objects
		/// </summary>
		List<EditableObject> sceneObjects;

		/// <summary>
		/// Opening selection
		/// </summary>
		void SelectToolOpen() {
			if (selectInterface == null) {
				selectInterface = new UserInterface();
				selectInterface.Items.Add(selectLabel = new UI.Basic.Label() {
					Text = "",
					Position = Vector2.Zero,
					FontSize = 10f,
					Anchor = Cubed.UI.Control.AnchorMode.TopLeft,
					HorizontalAlign = UserInterface.Align.Start,
					VerticalAlign = UserInterface.Align.Start
				});
			}
			if (sceneObjects == null) {
				sceneObjects = new List<EditableObject>();
			}
			engine.Interface = selectInterface;
			foreach (EditableObject eo in sceneObjects) {
				eo.StopPlayMode(scene);
			}
		}

		/// <summary>
		/// Closing selection
		/// </summary>
		void SelectToolClose() {
			engine.Interface = null;
			if (draggingNewObject != null) {
				draggingNewObject.Destroy(scene);
				draggingNewObject = null;
			}
			foreach (EditableObject eo in sceneObjects) {
				eo.StartPlayMode(scene);
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
			bool pickVisible = mousePos != Vector2.One * -1;
			if (pickVisible && !display.MouseLock) {
				selectLabel.Position = mousePos + new Vector2(15, 15);
			} else {
				selectLabel.Position = new Vector2(0, -1000);
			}

			// Calculating cam
			Vector3 camPos = cam.ScreenToPoint(mousePos.X, mousePos.Y, 0);
			Vector3 camDir = cam.ScreenToPoint(mousePos.X, mousePos.Y, 1) - camPos;
			Vector3 pickPos = Vector3.Zero;
			bool pickedWorld = false;
			World.Map.Side pickSide = World.Map.Side.Top;

			// Picking map
			MapIntersections.Hit mapHit = MapIntersections.Intersect(camPos, camDir.Normalized(), map);
			if (mapHit != null) {
				pickPos = mapHit.Point;
				pickSide = mapHit.Side;
				pickedWorld = true;
			}

			// Picking grid
			Vector3 gridPickPos = Vector3.Zero;
			float gridh = (((cam.Position.Y < gridHeight + 1) && (cam.Angles.X < 0)) ? gridHeight + 1 : gridHeight);
			if (Intersections.RayPlane(Vector3.UnitY * gridh, Vector3.UnitY, camPos, camDir.Normalized(), out gridPickPos)) {
				if (pickedWorld) {
					if ((gridPickPos - camPos).LengthFast < (pickPos - camPos).LengthFast) {
						pickPos = gridPickPos;
						pickedWorld = true;
						pickSide = gridh < cam.Position.Y ? World.Map.Side.Top : World.Map.Side.Bottom;
					}
				} else {
					pickPos = gridPickPos;
					pickedWorld = true;
					pickSide = gridh < cam.Position.Y ? World.Map.Side.Top : World.Map.Side.Bottom;
				}
			}



			if (draggingNewObject != null && pickedWorld) {
				Vector3 newPos = pickPos - draggingNewObject.BoundPosition;
				Vector3 bound = draggingNewObject.BoundSize / 2f + Vector3.One * 0.05f;
				switch (pickSide) {
					case World.Map.Side.Forward:
						newPos.Z += bound.Z; 
						break;
					case World.Map.Side.Right:
						newPos.X += bound.X;
						break;
					case World.Map.Side.Back:
						newPos.Z -= bound.Z;
						break;
					case World.Map.Side.Left:
						newPos.X -= bound.X;
						break;
					case World.Map.Side.Top:
						newPos.Y += bound.Y;
						break;
					case World.Map.Side.Bottom:
						newPos.Y -= bound.Y;
						break;
				}
				draggingNewObject.Prefab.Position = newPos;
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
							engine.MakeCurrent();
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
