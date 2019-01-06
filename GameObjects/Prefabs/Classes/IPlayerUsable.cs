using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubed.Prefabs.Classes {

	/// <summary>
	/// Interface for player interaction
	/// </summary>
	public interface IPlayerUsable {

		/// <summary>
		/// Flag that player can use it
		/// </summary>
		bool CanUse {
			get;
		}

		/// <summary>
		/// Check object for using
		/// </summary>
		/// <param name="origin">Origin for block</param>
		/// <param name="direction"></param>
		/// <param name="distance"></param>
		/// <returns></returns>
		bool PickForUsing(Vector3 origin, Vector3 direction, out float distance);

		/// <summary>
		/// Try to use object
		/// </summary>
		/// <param name="hit">Use intersection</param>
		void Use(Vector3 hit);

	}
}
