using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cubed.Components.Controls;
using Cubed.Components.Rendering;
using Cubed.Core;
using Cubed.Data.Editor.Attributes;
using Cubed.Data.Projects;
using Cubed.Drivers.Files;
using Cubed.Drivers.Rendering;
using Cubed.Editing;
using Cubed.Forms.Common;
using Cubed.Forms.Resources;
using Cubed.Graphics;
using Cubed.World;
using OpenTK;
using OpenTK.Input;

namespace Cubed.Forms.Editors.Map {

	/// <summary>
	/// Maps editor
	/// </summary>
	public partial class MapEditor : EditorForm {

		/// <summary>
		/// Current engine
		/// </summary>
		Engine engine;

		/// <summary>
		/// Display module
		/// </summary>
		ProxyDisplay display;

		/// <summary>
		/// Current scene
		/// </summary>
		Scene scene;

		/// <summary>
		/// Default camera
		/// </summary>
		Camera cam;

		/// <summary>
		/// Current map
		/// </summary>
		Cubed.World.Map map;

		/// <summary>
		/// Grid
		/// </summary>
		Entity grid;

		/// <summary>
		/// Grid height
		/// </summary>
		float gridHeight;

		/// <summary>
		/// Allow mouse looking
		/// </summary>
		bool allowMouseLook;

		/// <summary>
		/// Wobbling for color effects
		/// </summary>
		float wobble;

		/// <summary>
		/// Current selected tool
		/// </summary>
		ToolType currentTool;

		/// <summary>
		/// Empty wall texture
		/// </summary>
		Texture emptyWallTex, emptyFloorTex, emptyCeilTex;
		
		/// <summary>
		/// Flag for init
		/// </summary>
		bool editorInitialized;

		/// <summary>
		/// Environment params
		/// </summary>
		EnvironmentInfo environment;

		/// <summary>
		/// Constructor
		/// </summary>
		public MapEditor() {
			InitializeComponent();
			cachedTextures = new Dictionary<Project.Entry, Texture>();
			textureAnimators = new Dictionary<Texture, TextureAnimator>();
			listContainer.Panel2Collapsed = true;

			// Environment and scripts
			environment = new EnvironmentInfo(this);

			// Making engine
			engine = new Engine();
			engine.Filesystem = new FolderFileSystem() {
				RootFolder = Project.Root.FullPath
			};

			scene = new Scene();
			cam = new Camera();
			map = new World.Map();
			scene.Camera = cam;
			scene.Map = map;
			engine.World = scene;

			// Making display
			display = new ProxyDisplay();
			display.Engine = engine;
			screen.Display = display;

			// Making debug scene
			cam.Position = new OpenTK.Vector3(0, 5, -3);
			cam.Angles = new OpenTK.Vector3(30, 0, 0);
			engine.UpdateLogic += engine_UpdateLogic;

			// Making grid
			grid = new Entity();
			grid.AddComponent(new WireGridComponent() {
				CellCount = 100,
				CellSize = 0.25f,
				GroupedCells = 4
			});
			scene.Entities.Add(grid);
			gridHeight = 0;
			allowMouseLook = true;

			// Handling updates
			Project.EntriesChangedEvent += Project_EntriesChangedEvent;

			// Initializing select
			sceneObjects = new List<EditableObject>();
			sceneSelectedObjects = new List<EditableObject>();
			SelectToolOpen();
		}

		/// <summary>
		/// Engine updating
		/// </summary>
		void engine_UpdateLogic(object sender, Engine.UpdateEventArgs e) {

			// Making engine current
			MainForm.CurrentEngine = engine;

			// Initialization
			if (!editorInitialized) {

				// Creating player camera
				playerCamera = new Camera();

				// Creating player
				playerCamHolder = new Entity();
				playerCamera.Parent = playerCamHolder;
				playerCamera.LocalPosition = Vector3.Zero;
				playerCamera.LocalAngles = Vector3.Zero;
				scene.Entities.Add(playerCamHolder);

				playerController = new WalkController();

				player = new Entity();
				player.BoxCollider = new Collider() {
					Size = new Vector3(0.35f, 0.45f, 0.35f)
				};
				player.AddComponent(playerController);
				scene.Entities.Add(player);

				
				// Filling textures
				emptyWallTex = new Texture(EditorTextures.Wall);
				emptyFloorTex = new Texture(EditorTextures.Floor);
				emptyCeilTex = new Texture(EditorTextures.Ceiling);

				// Updating flags
				skyboxEnabledFlag_CheckedChanged(null);
				lightsEnabledFlag_CheckedChanged(null);
				soundsEnabledFlag_CheckedChanged(null);

				// Init complete
				editorInitialized = true;
			}

			// Updating controls
			UpdateControls();

			// Updating textures
			UpdateTextureAnimators();

			// Grid movement
			float gheight = grid.Position.Y;
			float gtarget = gridHeight;
			if ((cam.Position.Y < gridHeight + 1) && (cam.Angles.X < 0)) {
				gtarget += 1;
			}
			if (Math.Abs(gheight - gtarget) < 0.001f) {
				gheight = gtarget;
			} else {
				gheight += (gtarget - gheight) * 0.4f;
			}
			grid.Position = new Vector3(
				(float)System.Math.Floor(cam.Position.X),
				gheight,
				(float)System.Math.Floor(cam.Position.Z)
			);
			grid.Visible = !walkModeEnable.Checked;

			// Updating wobble
			wobble = (wobble + 0.01f) % 1f;

			// Updating tools
			if (!walkModeEnable.Checked) {
				switch (currentTool) {
					case ToolType.Select:
						SelectToolUpdate();
						break;
					case ToolType.Walls:
						WallToolUpdate();
						break;
					case ToolType.Floors:
						FloorToolUpdate();
						break;
					case ToolType.FloorHeights:
						HeightToolUpdate();
						break;
					case ToolType.Painting:
						PaintToolUpdate();
						break;
					case ToolType.Logics:
						LogicToolUpdate();
						break;
				}
			}
		}

		/// <summary>
		/// Changing tool
		/// </summary>
		/// <param name="sender">Which button is pressed</param>
		private void tool_CheckedChanged(object sender) {
			ToolType newType = ToolType.Select;
			if (toolWalls.Checked) {
				newType = ToolType.Walls;
			} else if (toolFloors.Checked) {
				newType = ToolType.Floors;
			} else if (toolHeightmap.Checked) {
				newType = ToolType.FloorHeights;
			} else if (toolPaint.Checked) {
				newType = ToolType.Painting;
			} else if (toolLogics.Checked) {
				newType = ToolType.Logics;
			}
			if (newType != currentTool) {
				switch (currentTool) {
					case ToolType.Select:
						SelectToolClose();
						break;
					case ToolType.Walls:
						WallToolClose();
						break;
					case ToolType.Floors:
						FloorToolClose();
						break;
					case ToolType.FloorHeights:
						HeightToolClose();
						break;
					case ToolType.Painting:
						PaintToolClose();
						break;
					case ToolType.Logics:
						LogicToolClose();
						break;
				}
				switch (newType) {
					case ToolType.Select:
						SelectToolOpen();
						break;
					case ToolType.Walls:
						WallToolOpen();
						break;
					case ToolType.Floors:
						FloorToolOpen();
						break;
					case ToolType.FloorHeights:
						HeightToolOpen();
						break;
					case ToolType.Painting:
						PaintToolOpen();
						break;
					case ToolType.Logics:
						LogicToolOpen();
						break;
				}
				currentTool = newType;
			}
		}

		/// <summary>
		/// Going down
		/// </summary>
		void floorDown_Click(object sender, EventArgs e) {
			gridHeight -= 1;
			floorIndex.Text = gridHeight + "";
		}

		/// <summary>
		/// Going up
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void floorUp_Click(object sender, EventArgs e) {
			gridHeight += 1;
			floorIndex.Text = gridHeight + "";
		}

		/// <summary>
		/// Changing mode
		/// </summary>
		private void walkModeEnable_CheckedChanged(object sender) {
			if (walkModeEnable.Checked) {
				foreach (EditableObject eo in sceneObjects) {
					eo.StartPlayMode(scene);
				}
				scene.Camera = playerCamera;
				player.Position = cam.Position;
				playerCamHolder.Angles = cam.Angles;
				playerController.Reset();
				display.MouseLock = true;
				switch (currentTool) {
					case ToolType.Select:
						SelectToolClose();
						break;
					case ToolType.Walls:
						WallToolClose();
						break;
					case ToolType.Floors:
						FloorToolClose();
						break;
					case ToolType.FloorHeights:
						HeightToolClose();
						break;
					case ToolType.Painting:
						PaintToolClose();
						break;
					case ToolType.Logics:
						LogicToolClose();
						break;
				}
				screen.Focus();
			} else {
				foreach (EditableObject eo in sceneObjects) {
					eo.StopPlayMode(scene);
				}
				scene.Camera = cam;
				display.MouseLock = false;
				switch (currentTool) {
					case ToolType.Select:
						SelectToolOpen();
						break;
					case ToolType.Walls:
						WallToolOpen();
						break;
					case ToolType.Floors:
						FloorToolOpen();
						break;
					case ToolType.FloorHeights:
						HeightToolOpen();
						break;
					case ToolType.Painting:
						PaintToolOpen();
						break;
					case ToolType.Logics:
						LogicToolOpen();
						break;
				}
			}
		}

		/// <summary>
		/// Showing environment parameters
		/// </summary>
		private void envOptionsButton_Click(object sender, EventArgs e) {
			MainForm.SelectedTarget = environment;
		}

		/// <summary>
		/// Showing scripting parameters
		/// </summary>
		private void scriptOptionsButton_Click(object sender, EventArgs e) {
			MainForm.SelectedTarget = environment;
		}

		/// <summary>
		/// Skybox flag
		/// </summary>
		private void skyboxEnabledFlag_CheckedChanged(object sender) {
			if (skyboxEnabledFlag.Checked) {
				scene.Sky = environment.Sky;
				if (environment.FogEnabled) {
					scene.Fog = environment.FogData;
				} else {
					scene.Fog = null;
				}
			} else {
				scene.Sky = null;
				scene.Fog = null;
			}
		}

		/// <summary>
		/// Lighting flag
		/// </summary>
		private void lightsEnabledFlag_CheckedChanged(object sender) {
			if (lightsEnabledFlag.Checked) {
				map.Ambient = environment.Ambient;
			} else {
				map.Ambient = Color.White;
			}
		}

		/// <summary>
		/// Sounds flag
		/// </summary>
		private void soundsEnabledFlag_CheckedChanged(object sender) {

		}

		/// <summary>
		/// Type of tools
		/// </summary>
		enum ToolType {
			Select,
			Walls,
			Floors,
			FloorHeights,
			Painting,
			Logics
		}

		/// <summary>
		/// Environment parameters
		/// </summary>
		[InspectorIcon("Environment")]
		[InspectorName("EnvironmentInfo")]
		[InspectorDescription("EnvironmentInfoDesc")]
		[InspectorSection(0, "EnvLightGroup", "LightGroup")]
		[InspectorSection(1, "FogGroup", "FogGroup")]
		[InspectorSection(2, "SkyGroup", "SkyGroup")]
		[InspectorSection(3, "SoundGroup", "SoundGroup")]
		public class EnvironmentInfo {

			/// <summary>
			/// Ambient color
			/// </summary>
			[InspectorSection(0)]
			[InspectorName("AmbientLight")]
			public Color Ambient {
				get {
					return ambientColor;
				}
				set {
					parent.Saved = false;
					ambientColor = value;
					if (parent.lightsEnabledFlag.Checked) {
						parent.map.Ambient = ambientColor;
					}
				}
			}

			/// <summary>
			/// Sky top Texture
			/// </summary>
			[InspectorSection(2)]
			[InspectorName("SkyTop")]
			public Texture SkyTop {
				get {
					return Sky[Skybox.Side.Top];
				}
				set {
					parent.Saved = false;
					Sky[Skybox.Side.Top] = value;
				}
			}


			/// <summary>
			/// Sky top Texture
			/// </summary>
			[InspectorSection(2)]
			[InspectorName("SkyBottom")]
			public Texture SkyBottom {
				get {
					return Sky[Skybox.Side.Bottom];
				}
				set {
					parent.Saved = false;
					Sky[Skybox.Side.Bottom] = value;
				}
			}


			/// <summary>
			/// Sky left Texture
			/// </summary>
			[InspectorSection(2)]
			[InspectorName("SkyLeft")]
			public Texture SkyLeft {
				get {
					return Sky[Skybox.Side.Left];
				}
				set {
					parent.Saved = false;
					Sky[Skybox.Side.Left] = value;
				}
			}


			/// <summary>
			/// Sky right Texture
			/// </summary>
			[InspectorSection(2)]
			[InspectorName("SkyRight")]
			public Texture SkyRight {
				get {
					return Sky[Skybox.Side.Right];
				}
				set {
					parent.Saved = false;
					Sky[Skybox.Side.Right] = value;
				}
			}


			/// <summary>
			/// Sky front Texture
			/// </summary>
			[InspectorSection(2)]
			[InspectorName("SkyFront")]
			public Texture SkyFront {
				get {
					return Sky[Skybox.Side.Front];
				}
				set {
					parent.Saved = false;
					Sky[Skybox.Side.Front] = value;
				}
			}


			/// <summary>
			/// Sky back Texture
			/// </summary>
			[InspectorSection(2)]
			[InspectorName("SkyBack")]
			public Texture SkyBack {
				get {
					return Sky[Skybox.Side.Back];
				}
				set {
					parent.Saved = false;
					Sky[Skybox.Side.Back] = value;
				}
			}
			
			/// <summary>
			/// Fog mode
			/// </summary>
			[InspectorSection(1)]
			[InspectorName("Enabled")]
			public bool FogEnabled {
				get {
					return fogEnabled;
				}
				set {
					parent.Saved = false;
					fogEnabled = value;
					if (parent.skyboxEnabledFlag.Checked) {
						if (fogEnabled) {
							parent.scene.Fog = FogData;
						} else {
							parent.scene.Fog = null;
						}
					}
				}
			}

			/// <summary>
			/// Fog color
			/// </summary>
			[InspectorSection(1)]
			[InspectorName("FogColor")]
			public Color FogColor {
				get {
					return FogData.Color;
				}
				set {
					parent.Saved = false;
					FogData.Color = value;
				}
			}

			/// <summary>
			/// Fog color
			/// </summary>
			[InspectorSection(1)]
			[InspectorName("FogNear")]
			public float FogNear {
				get {
					return FogData.Near;
				}
				set {
					parent.Saved = false;
					FogData.Near = value;
				}
			}

			/// <summary>
			/// Fog color
			/// </summary>
			[InspectorSection(1)]
			[InspectorName("FogFar")]
			public float FogFar {
				get {
					return FogData.Far;
				}
				set {
					parent.Saved = false;
					FogData.Far = value;
				}
			}

			/// <summary>
			/// Internal sky
			/// </summary>
			[InspectorHidden]
			public Skybox Sky {
				get;
				private set;
			}

			/// <summary>
			/// Internal fog data
			/// </summary>
			[InspectorHidden]
			public Fog FogData {
				get;
				private set;
			}

			/// <summary>
			/// Ambient color data
			/// </summary>
			Color ambientColor;

			/// <summary>
			/// Flag for fog
			/// </summary>
			bool fogEnabled;

			/// <summary>
			/// Parent map editor
			/// </summary>
			MapEditor parent;

			/// <summary>
			/// Environment constructor
			/// </summary>
			public EnvironmentInfo(MapEditor parent) {
				this.parent = parent;
				ambientColor = Color.FromArgb(100, 100, 100);
				Sky = new Skybox();
				FogData = new Fog();
			}
		}

		public class ScriptInfo {

		}
	}
}
