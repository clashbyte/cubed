using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cubed.Data.Editor.Attributes {
	
	/// <summary>
	/// Target prefab
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class TargetPrefabAttribute : Attribute {

		/// <summary>
		/// Cache
		/// </summary>
		static Dictionary<Type, Type> cache;

		/// <summary>
		/// Getting type by ID
		/// </summary>
		/// <param name="id">ID</param>
		/// <returns>Type</returns>
		public static Type GetEditableObject(Type tg) {
			if (cache == null) {
				cache = new Dictionary<Type, Type>();
				Assembly asm = typeof(Editing.EditableObject).Assembly;
				foreach (Type type in asm.GetTypes()) {
					TargetPrefabAttribute pref = Attribute.GetCustomAttribute(type, typeof(TargetPrefabAttribute), false) as TargetPrefabAttribute;
					if (pref != null) {
						if (cache.ContainsKey(pref.Prefab)) {
							throw new Exception("Prefab EditableObject copy");
						}
						cache.Add(pref.Prefab, type);
					}
				}
			}
			if (cache.ContainsKey(tg)) {
				return cache[tg];
			}
			return null;
		}

		/// <summary>
		/// Prefab type
		/// </summary>
		public Type Prefab {
			get;
			private set;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="prefab">Prefab type</param>
		public TargetPrefabAttribute(Type prefab) {
			Prefab = prefab;
		}

	}
}
