using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cubed.Components;
using Cubed.Components.Volumes;
using Cubed.Data.Rendering;
using Cubed.Data.Types;
using Cubed.Data;
using OpenTK;

namespace Cubed.World {
	
	/// <summary>
	/// Single world object
	/// </summary>
	public class Entity {

		/// <summary>
		/// Hidden matrix
		/// </summary>
		protected Matrix4 mat = Matrix4.Identity;

		/// <summary>
		/// Inverted matrix
		/// </summary>
		protected Matrix4 invmat = Matrix4.Identity;

		/// <summary>
		/// Parent
		/// </summary>
		protected Entity parent;

		/// <summary>
		/// Position
		/// </summary>
		protected Vector3 pos = Vector3.Zero;

		/// <summary>
		/// Rotation
		/// </summary>
		protected Vector3 rot = Vector3.Zero;

		/// <summary>
		/// Object visibility flag
		/// </summary>
		protected bool visible = true;

		/// <summary>
		/// Object destruction flag
		/// </summary>
		private bool destroyed;

		/// <summary>
		/// Object components
		/// </summary>
		protected List<EntityComponent> components;

		/// <summary>
		/// Dirty cull sphere flag
		/// </summary>
		internal bool needCullRebuild;

		/// <summary>
		/// Cull sphere
		/// </summary>
		protected CullSphere cullSphere = new CullSphere();

		/// <summary>
		/// Body collider
		/// </summary>
		protected Collider collider;

		/// <summary>
		/// Object constructor
		/// </summary>
		public Entity() {
			Children = new List<Entity>();
			components = new List<EntityComponent>();
		}

		/// <summary>
		/// Object radius
		/// </summary>
		public float Radius {
			get {
				if (needCullRebuild) {
					RebuildCullSphere();
				}
				return cullSphere.Radius;
			}
		}

		/// <summary>
		/// Object visibility
		/// </summary>
		public bool Visible {
			get {
				if (destroyed) {
					return false;
				}
				Entity d = this;
				while (d != null) {
					if (!d.visible) {
						return false;
					}
					d = d.parent;
				}
				return true;
			}
			set {
				visible = value;
			}
		}

		/// <summary>
		/// Object position
		/// </summary>
		public Vector3 Position {
			get {
				Vector3 vt = pos;
				if (parent != null) {
					vt = Vector3.TransformPosition(vt, parent.mat);
				}
				return new Vector3(
					vt.X, vt.Y, -vt.Z
				);
			}
			set {
				pos = new Vector3(
					value.X, value.Y, -value.Z
				);
				if (parent != null) {
					pos = Vector3.TransformPosition(pos, parent.invmat);
				}
				RebuildMatrix();
			}
		}

		/// <summary>
		/// Object local position
		/// </summary>
		public Vector3 LocalPosition {
			get {
				return new Vector3(
					pos.X, pos.Y, -pos.Z
				);
			}
			set {
				pos = new Vector3(
					value.X, value.Y, -value.Z
				);
				RebuildMatrix();
			}
		}

		/// <summary>
		/// Object rotation
		/// </summary>
		public Vector3 Angles {
			get {
				Vector3 vt = rot;
				if (parent != null) {
					vt = ModifyAngles(rot, parent.mat);
				}
				return new Vector3(
					-vt.X, -vt.Y, vt.Z
				);
			}
			set {
				rot = new Vector3(
					-value.X, -value.Y, value.Z
				);
				if (parent != null) {
					rot = ModifyAngles(rot, parent.invmat);
				}
				RebuildMatrix();
			}
		}

		/// <summary>
		/// Object local rotation
		/// </summary>
		public Vector3 LocalAngles {
			get {
				return new Vector3(
					-rot.X, -rot.Y, rot.Z
				);
			}
			set {
				rot = new Vector3(
					-value.X, -value.Y, value.Z
				);
				RebuildMatrix();
			}
		}

		/// <summary>
		/// Children objects
		/// </summary>
		public List<Entity> Children {
			get;
			private set;
		}

		/// <summary>
		/// Parent
		/// </summary>
		public Entity Parent {
			get {
				return parent;
			}
			set {
				if (parent != null) {
					pos = Vector3.TransformPosition(pos, parent.mat);
					rot = ModifyAngles(rot, parent.mat);
					parent.Children.Remove(this);
					parent = null;
				}
				if (value != null) {
					parent = value;
					parent.Children.Add(this);
					pos = Vector3.TransformPosition(pos, parent.invmat);
					rot = ModifyAngles(rot, parent.invmat);
				}
				RebuildMatrix();
			}
		}

		/// <summary>
		/// Коллайдер для тела
		/// </summary>
		public Collider BoxCollider {
			get {
				return collider;
			}
			set {
				if (collider != null) {
					collider.owner = null;
				}
				collider = value;
				if (collider != null) {
					collider.owner = this;
				}
			}
		}

		/// <summary>
		/// Matrix - used by renderer
		/// </summary>
		internal Matrix4 RenditionMatrix {
			get {
				return mat;
			}
		}

		/// <summary>
		/// Destroying object
		/// </summary>
		public void Destroy() {
			destroyed = true;
			foreach (EntityComponent c in components) {
				c.Destroy();
			}
		}

		/// <summary>
		/// Add component to object
		/// </summary>
		/// <param name="c">New component</param>
		public void AddComponent(EntityComponent c) {
			c.Parent = this;
			if (c is IRenderable) {
				needCullRebuild = true;
			}
			components.Add(c);
		}

		/// <summary>
		/// Remove component
		/// </summary>
		/// <param name="c">Component</param>
		public void RemoveComponent(EntityComponent c) {
			if (components.Contains(c)) {
				c.Parent = null;
				if (c is IRenderable) {
					needCullRebuild = true;
				}
				components.Remove(c);
			}
		}

		/// <summary>
		/// Access specified component
		/// </summary>
		/// <typeparam name="T">Component type</typeparam>
		/// <param name="index">Index</param>
		/// <returns>Component or null</returns>
		public T GetComponent<T>(int index = 0) where T : EntityComponent {
			foreach (EntityComponent c in components) {
				if (c is T) {
					if (index > 0) {
						index--;
					} else {
						return (T)c;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Access all components with specified type
		/// </summary>
		/// <typeparam name="T">Component type</typeparam>
		/// <returns>Component array</returns>
		public T[] GetComponents<T>() where T : EntityComponent {
			List<T> rc = new List<T>();
			foreach (EntityComponent c in components) {
				if (c is T) {
					rc.Add((T)c);
				}
			}
			return rc.ToArray();
		}

		/// <summary>
		/// List of all updatable components
		/// </summary>
		internal IEnumerable<EntityComponent> GetLogicalComponents() {
			List<EntityComponent> cl = new List<EntityComponent>();
			if (destroyed) {
				return cl;
			}
			foreach (EntityComponent c in components) {
				if (c is IUpdatable && c.Enabled) {
					cl.Add(c);
				}
			}
			return cl;
		}

		/// <summary>
		/// List of all late updateable components
		/// </summary>
		internal IEnumerable<EntityComponent> GetLateLogicalComponents() {
			List<EntityComponent> cl = new List<EntityComponent>();
			if (destroyed) {
				return cl;
			}
			foreach (EntityComponent c in components) {
				if (c is ILateUpdatable && c.Enabled) {
					cl.Add(c);
				}
			}
			return cl;
		}

		/// <summary>
		/// List of all renderable components
		/// </summary>
		internal IEnumerable<EntityComponent> GetVisualComponents() {

			// Component list
			List<EntityComponent> cl = new List<EntityComponent>();
			if (destroyed) {
				return cl;
			}

			// Rebuilding cull sphere
			if (needCullRebuild) {
				RebuildCullSphere();
			}

			// Check for culling
			if (Frustum.Contains(cullSphere.Position, cullSphere.Radius)) {

				// Listing all visible components
				foreach (EntityComponent c in components) {
					if (c is IRenderable && c.Enabled) {
						cl.Add(c);
					}
				}
			}

			return cl;
		}


		internal bool RayCast(Vector3 pos, Vector3 dir, float rayLength, out Vector3 hitPos, out Vector3 hitNormal, out VolumeComponent hitVolume) {

			// Перестроение основной сферы
			if (needCullRebuild) {
				RebuildCullSphere();
			}

			// Пересечение с основной сферой
			if (Intersections.RaySphere(pos, dir, cullSphere.Position, cullSphere.Radius)) {
				float range = float.MaxValue;
				Vector3 norm = Vector3.Zero;
				Vector3 hitp = Vector3.Zero;
				VolumeComponent hvol = null;

				// Пересечение с волюмами
				Vector3 tfpos = PointToLocal(pos);
				Vector3 tfnrm = VectorToLocal(dir);
				VolumeComponent[] volumes = GetComponents<VolumeComponent>();
				if (volumes != null && volumes.Length > 0) {
					foreach (VolumeComponent v in volumes) {
						if (v.Enabled) {
							Vector3 hp, hn;
							if (v.RayHit(tfpos, tfnrm, rayLength, out hp, out hn)) {
								float dst = (hp - tfpos).Length;
								if (dst < range) {
									range = dst;
									hitp = hp;
									norm = hn;
									hvol = v;
								}
							}
						}
					}
				}

				// Возврат
				if (hvol != null && range <= rayLength) {
					hitPos = PointToWorld(hitp);
					hitNormal = VectorToWorld(norm);
					hitVolume = hvol;
					return true;
				}
			}

			// Нет пересечения
			hitPos = Vector3.Zero;
			hitNormal = Vector3.Zero;
			hitVolume = null;
			return false;
		}

		/// <summary>
		/// Converting point from world to local
		/// </summary>
		/// <param name="point">World point</param>
		/// <returns>Point in local coords</returns>
		public Vector3 PointToLocal(Vector3 point) {
			Vector3 v = Vector3.TransformPosition(new Vector3(point.X, point.Y, -point.Z), invmat);
			return new Vector3(v.X, v.Y, -v.Z);
		}

		/// <summary>
		/// Convert point from local coords to world
		/// </summary>
		/// <param name="point">Local point</param>
		/// <returns>World point</returns>
		public Vector3 PointToWorld(Vector3 point) {
			Vector3 v = Vector3.TransformPosition(new Vector3(point.X, point.Y, -point.Z), mat);
			return new Vector3(v.X, v.Y, -v.Z);
		}

		/// <summary>
		/// Convert vector from world to local
		/// </summary>
		/// <param name="vec">World vector</param>
		/// <returns>Local vector</returns>
		public Vector3 VectorToLocal(Vector3 vec) {
			Vector3 v = Vector3.TransformVector(new Vector3(vec.X, vec.Y, -vec.Z), invmat);
			return new Vector3(v.X, v.Y, -v.Z);
		}

		/// <summary>
		/// Convert vector from local space to world
		/// </summary>
		/// <param name="vec">Local vector</param>
		/// <returns>World vector</returns>
		public Vector3 VectorToWorld(Vector3 vec) {
			Vector3 v = Vector3.TransformVector(new Vector3(vec.X, vec.Y, -vec.Z), mat);
			return new Vector3(v.X, v.Y, -v.Z);
		}

		/// <summary>
		/// Matrix rebuilding
		/// </summary>
		protected virtual void RebuildMatrix() {

			// Internal rebuilding
			mat = Matrix4.CreateFromQuaternion(rot.ToQuaternion()) * Matrix4.CreateTranslation(pos);
			if (parent != null) {
				mat *= parent.mat;
			}
			invmat = mat.Inverted();

			// Rebuilding children
			foreach (Entity e in Children) {
				e.RebuildMatrix();
			}

			// Rebuilding cull
			RebuildCullSphere();
		}

		/// <summary>
		/// Rebuilding culling sphere
		/// </summary>
		protected virtual void RebuildCullSphere() {
			List<CullBox> culls = new List<CullBox>();
			foreach (EntityComponent c in components) {
				if (c is IRenderable) {
					CullBox cb = c.GetCullingBox();
					if (cb != null) {
						culls.Add(cb);
					}
				}
			}
			cullSphere = CullBox.FromBoxes(culls.ToArray()).ToSphere();
			cullSphere.Position = PointToWorld(cullSphere.Position);
			needCullRebuild = false;
		}

		/// <summary>
		/// Multiplying euler angles by matrix
		/// </summary>
		Vector3 ModifyAngles(Vector3 r, Matrix4 m) {
			return (r.ToQuaternion() * m.ExtractRotation()).ToEuler();
		}
		
	}
}
