using System;
using OpenTK;

namespace Cubed.Data.Rendering {

	/// <summary>
	/// Библиотека функций для поиска пересечений
	/// </summary>
	internal static class Intersections {

		/// <summary>
		/// Пересекается ли луч со сферой
		/// </summary>
		/// <param name="rayPos">Позиция начала луча</param>
		/// <param name="rayDir">Направление луча</param>
		/// <param name="spherePos">Расположение сферы</param>
		/// <param name="sphereRad">Радиус сферы</param>
		/// <returns>True если есть пересечение</returns>
		public static bool RaySphere(Vector3 rayPos, Vector3 rayDir, Vector3 spherePos, float sphereRad) {
			Vector3 q = spherePos - rayPos;
			float c = q.Length;
			float v = Vector3.Dot(q, rayDir);
			if ((sphereRad * sphereRad - (c*c - v*v))<0f) {
				return false;
			}
			return true;
		}

		/// <summary>
		/// Пересекается ли луч с треугольником
		/// </summary>
		/// <param name="rayPos">Позиция начала луча</param>
		/// <param name="rayDir">Направление луча</param>
		/// <param name="v0">Первая вершина</param>
		/// <param name="v1">Вторая вершина</param>
		/// <param name="v2">Третья вершина</param>
		/// <param name="twoSided">Учитывать обе стороны</param>
		/// <param name="hitPoint">Место пересечения</param>
		/// <param name="hitNormal">Нормаль пересечения</param>
		/// <returns>True если есть пересечение</returns>
		public static bool RayTriangle(Vector3 rayPos, Vector3 rayDir, Vector3 v0, Vector3 v1, Vector3 v2, bool twoSided, out Vector3 hitPoint, out Vector3 hitNormal) {
			Vector3 tv1 = v1 - v0;
			Vector3 tv2 = v2 - v0;
			Vector3 pv = Vector3.Cross(rayDir, tv2);
			float det = Vector3.Dot(tv1, pv);

			float pp = det;
			if (twoSided) pp = Math.Abs(det);
			if (pp < float.Epsilon) {
				hitPoint = Vector3.Zero;
				hitNormal = Vector3.Zero;
				return false;
			}

			float invDet = 1f / det;

			Vector3 tv = rayPos - v0;
			float u = Vector3.Dot(tv, pv) * invDet;
			if (u < 0 || u > 1) {
				hitPoint = Vector3.Zero;
				hitNormal = Vector3.Zero;
				return false;
			}

			Vector3 qv = Vector3.Cross(tv, tv1);;
			float v = Vector3.Dot(rayDir, qv) * invDet;
			if (v < 0 || u + v > 1) {
				hitPoint = Vector3.Zero;
				hitNormal = Vector3.Zero;
				return false;
			}

			float w = 1f - (u + v);
			hitPoint = v0 * u + v1 * v + v2 * w;
			hitNormal = Vector3.Cross(v1 - v0, v2 - v0);
			if (pp < 0) hitNormal *= -1f;
			hitNormal.Normalize();
			return true;
		}

	}
}
