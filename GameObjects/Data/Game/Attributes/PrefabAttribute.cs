using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cubed.Data.Game.Attributes {

	/// <summary>
	/// Attribute for prefab type
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class PrefabAttribute : Attribute {

		/// <summary>
		/// Cache
		/// </summary>
		static Dictionary<int, Type> cache;

		/// <summary>
		/// Getting type by ID
		/// </summary>
		/// <param name="id">ID</param>
		/// <returns>Type</returns>
		public static Type GetPrefab(int id) {
			if (cache == null) {
				cache = new Dictionary<int, Type>();
				Assembly asm = typeof(Prefabs.GamePrefab).Assembly;
				foreach (Type type in asm.GetTypes()) {
					PrefabAttribute pref = Attribute.GetCustomAttribute(type, typeof(PrefabAttribute), false) as PrefabAttribute;
					if (pref != null) {
						if (cache.ContainsKey(pref.ID)) {
							throw new Exception("Prefab ID copy");
						}
						cache.Add(pref.ID, type);
					}
				}
			}
			if (cache.ContainsKey(id)) {
				return cache[id];
			}
			return null;
		}

		/// <summary>
		/// Unique ID
		/// </summary>
		public int ID {
			get;
			private set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">Unique ID</param>
		public PrefabAttribute(int id) {
			ID = id;
		}

	}
}
