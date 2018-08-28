using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Components.Rendering;
using Cubed.Core;
using Cubed.Data.Projects;
using Cubed.Editing;
using Cubed.Editing.Gizmos;
using Cubed.Forms.Common;
using Cubed.Graphics;
using Cubed.Maths;
using Cubed.UI;
using Cubed.UI.Controls;
using Cubed.World;
using OpenTK;
using OpenTK.Input;

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
		/// Current objects
		/// </summary>
		List<EditableObject> sceneSelectedObjects;

		/// <summary>
		/// Dragging current object
		/// </summary>
		bool draggingObjects;

		/// <summary>
		/// Vertical mode
		/// </summary>
		bool dragVerticalMode;

		/// <summary>
		/// Dragging start location
		/// </summary>
		Vector3 draggingOrigin;

		/// <summary>
		/// Array of dragging entities
		/// </summary>
		Entity[] draggingPathEntities;

		/// <summary>
		/// Origins of all dragging entities
		/// </summary>
		Vector3[] draggingEntityOrigins;

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
			engine.Interface = selectInterface;
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

			// Picking entity
			EditableObject pickedObject = null;
			Vector3 objectPickPos = Vector3.Zero;
			if (draggingNewObject == null) {
				float dist = float.MaxValue;
				foreach (EditableObject eo in sceneObjects) {
					Vector3 pos = eo.Prefab.Position + eo.BoundPosition;
					Vector3 size = eo.BoundSize / 2f;
					Vector3 min = pos - size;
					Vector3 max = pos + size;
					float hdist = 0;
					if (Intersections.RayIntersectsBox(camPos, camDir, min, max, out hdist)) {
						if (hdist < dist) {
							pickedObject = eo;
							objectPickPos = camPos + camDir * hdist;
							dist = hdist;
						}
					}
				}
			}

			// Picking gizmo
			Gizmo pickedGizmo = null;




			// Selection
			if (Input.Controls.MouseHit(MouseButton.Left) && !display.MouseLock) {
				if (pickedObject != null) {
					if (pickedGizmo != null) {

					} else {
						SelectEntity(pickedObject);
						draggingObjects = true;
						draggingOrigin = objectPickPos;
						dragVerticalMode = Input.Controls.KeyDown(Key.ControlLeft);
						draggingPathEntities = new Entity[sceneSelectedObjects.Count];
						draggingEntityOrigins = new Vector3[sceneSelectedObjects.Count];
						int idx = 0;
						foreach (EditableObject eo in sceneSelectedObjects) {
							Entity ent = new Entity();
							ent.Parent = eo.Prefab;
							ent.LocalPosition = Vector3.Zero;
							ent.AddComponent(new LineComponent() {
								WireColor = Color.Gray,
								WireWidth = 1f
							});
							draggingPathEntities[idx] = ent;
							draggingEntityOrigins[idx] = eo.Prefab.Position;
							scene.Entities.Add(ent);
							idx++;
						}
					}
				} else {
					SelectEntity(null);
				}
				
				allowMouseLook = false;
			} else if (Input.Controls.MouseReleased(MouseButton.Left)) {
				if (draggingPathEntities != null) {
					foreach (Entity ent in draggingPathEntities) {
						scene.Entities.Remove(ent);
					}
				}
				draggingPathEntities = null;
				draggingObjects = false;
				allowMouseLook = true;
			}

			// Dragging
			if (draggingObjects) {
				bool picked = false;
				Vector3 dragPickPos = Vector3.Zero;
				if (dragVerticalMode) {
					Vector3 forw = cam.VectorToWorld(-Vector3.UnitZ);
					forw.Y = 0;
					forw.Normalize();
					if (Intersections.RayPlane(draggingOrigin, forw, camPos, camDir.Normalized(), out dragPickPos)) {
						dragPickPos.X = draggingOrigin.X;
						dragPickPos.Z = draggingOrigin.Z;
						picked = true;
					}
				} else {
					if (Intersections.RayPlane(draggingOrigin, Vector3.UnitY, camPos, camDir.Normalized(), out dragPickPos)) {
						dragPickPos.Y = draggingOrigin.Y;
						picked = true;
					}
				}
				if (picked) {
					int idx = 0;
					foreach (EditableObject eo in sceneSelectedObjects) {
						eo.Prefab.Position = draggingEntityOrigins[idx] + (dragPickPos - draggingOrigin);
						draggingPathEntities[idx].GetComponent<LineComponent>().Vertices = new Vector3[]{
							eo.BoundPosition,
							eo.BoundPosition - (dragPickPos - draggingOrigin)
						};
						idx++;
					}
				}
			}

			// Removing
			if (Input.Controls.KeyHit(Key.Delete) && !Input.Controls.MouseDown(MouseButton.Left)) {
				foreach (EditableObject reo in sceneSelectedObjects) {
					reo.Deselect(scene);
					reo.Destroy(scene);
					sceneObjects.Remove(reo);
				}
				sceneSelectedObjects.Clear();
				MainForm.SelectedTarget = null;
			}

			// Dragging new object on scene
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
		/// Selecting object
		/// </summary>
		/// <param name="eo">Object to select</param>
		void SelectEntity(EditableObject eo) {
			if (eo == null || !sceneSelectedObjects.Contains(eo)) {
				EditableObject inspectorObject = null;
				if (!Input.Controls.KeyDown(Key.ShiftLeft)) {
					foreach (EditableObject e in sceneSelectedObjects) {
						e.Deselect(scene);

						// Removing gizmos here
					}
					sceneSelectedObjects.Clear();
				}
				if (eo != null) {
					eo.Select(scene);
					sceneSelectedObjects.Add(eo);

					// Adding gizmos here

					if (sceneSelectedObjects.Count == 1) {
						inspectorObject = eo;
					}
				}
				MainForm.SelectedTarget = inspectorObject;
			}
		}

		/// <summary>
		/// Entering drag entry
		/// </summary>
		private void screen_DragEnter(object sender, DragEventArgs e) {
			if (currentTool != ToolType.Select && currentTool != ToolType.Logics) {
				return;
			}
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
				} else if (data.Content is Project.Entry) {
					Project.Entry en = data.Content as Project.Entry;
					engine.MakeCurrent();
					EditableObject eo = null;

					// Making simple sprite
					string ext = System.IO.Path.GetExtension(en.Name).ToLower();
					if (".jpg;.jpeg;.png;.gif;.anim;.bmp;".Split(';').Contains(ext)) {

						Texture tex = new Texture(en.Path, Texture.LoadingMode.Queued);
						eo = new MapSprite();
						eo.Create(scene);
						(eo as MapSprite).Texture = tex;

					}

					// Adding object
					if (eo != null) {
						e.Effect = e.AllowedEffect;
						Point mouse = screen.PointToClient(new Point(e.X, e.Y));
						draggingScreenLocation = new Vector2(mouse.X, mouse.Y);
						draggingNewObject = eo;
					}
				}
			}
			if (draggingNewObject != null) {
				e.Effect = e.AllowedEffect;
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

				// Dropping item
				sceneObjects.Add(draggingNewObject);
				SelectEntity(draggingNewObject);
				draggingNewObject = null;
			} else {
				e.Effect = DragDropEffects.None;
			}
		}


	}
}
