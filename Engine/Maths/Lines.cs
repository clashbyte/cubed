using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace Cubed.Maths {

	/// <summary>
	/// Line utilities
	/// </summary>
	public static class Lines {

		/// <summary>
		/// Calculating line points
		/// </summary>
		/// <param name="start">Start pos</param>
		/// <param name="end">End pos</param>
		/// <returns>Array of lines</returns>
		public static Vector2[] Bresenham(Vector2 start, Vector2 end) {
			List<Vector2> points = new List<Vector2>();
			int dx = (int)Math.Abs(end.X - start.X), sx = start.X < end.X ? 1 : -1;
			int dy = (int)Math.Abs(end.Y - start.Y), sy = start.Y < end.Y ? 1 : -1;
			int err = (dx > dy ? dx : -dy) / 2, e2;
			int px = (int)start.X;
			int py = (int)start.Y;
			while (true) {
				points.Add(new Vector2(px, py));
				if (px == end.X && py == end.Y) break;
				e2 = err;
				if (e2 > -dx) { err -= dy; px += sx; }
				if (e2 < dy) { err += dx; py += sy; }
			}
			return points.ToArray();
		}

	}
}
