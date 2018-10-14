using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Cubed.Maths {

	/// <summary>
	/// Intersections class
	/// </summary>
	public class Intersections {

		public static bool RayPlane(Vector3 plane, Vector3 normal, Vector3 ray, Vector3 direction, out Vector3 hit) {

			normal.Normalize();
			direction.Normalize();

			// Calculating delta
			float d = Vector3.Dot(normal, plane);
			if (System.Math.Abs(Vector3.Dot(normal, direction)) < 0.0001f) {
				hit = Vector3.Zero;
				return false;
			}
			float x = (d - Vector3.Dot(normal, ray)) / Vector3.Dot(normal, direction);
			hit = ray + direction.Normalized() * x;
			return true;
		}

		/// <summary>
		/// Determines whether there is an intersection between a ray and a triangle.
		/// </summary>
		/// <param name="ray">The ray to test.</param>
		/// <param name="vertex1">The first vertex of the triangle to test.</param>
		/// <param name="vertex2">The second vertex of the triangle to test.</param>
		/// <param name="vertex3">The third vertex of the triangle to test.</param>
		/// <param name="distance">When the method completes, contains the distance of the intersection,
		/// or 0 if there was no intersection.</param>
		/// <returns>Whether the two objects intersected.</returns>
		public static bool RayIntersectsTriangle(Vector3 pos, Vector3 dir, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, out float distance) {
			//Source: Fast Minimum Storage Ray / Triangle Intersection
			//Reference: http://www.cs.virginia.edu/~gfx/Courses/2003/ImageSynthesis/papers/Acceleration/Fast%20MinimumStorage%20RayTriangle%20Intersection.pdf

			//Compute vectors along two edges of the triangle.
			Vector3 edge1, edge2;

			//Edge 1
			edge1.X = vertex2.X - vertex1.X;
			edge1.Y = vertex2.Y - vertex1.Y;
			edge1.Z = vertex2.Z - vertex1.Z;

			//Edge2
			edge2.X = vertex3.X - vertex1.X;
			edge2.Y = vertex3.Y - vertex1.Y;
			edge2.Z = vertex3.Z - vertex1.Z;

			//Cross product of ray direction and edge2 - first part of determinant.
			Vector3 directioncrossedge2;
			directioncrossedge2.X = (dir.Y * edge2.Z) - (dir.Z * edge2.Y);
			directioncrossedge2.Y = (dir.Z * edge2.X) - (dir.X * edge2.Z);
			directioncrossedge2.Z = (dir.X * edge2.Y) - (dir.Y * edge2.X);

			//Compute the determinant.
			float determinant;
			//Dot product of edge1 and the first part of determinant.
			determinant = (edge1.X * directioncrossedge2.X) + (edge1.Y * directioncrossedge2.Y) + (edge1.Z * directioncrossedge2.Z);

			//If the ray is parallel to the triangle plane, there is no collision.
			//This also means that we are not culling, the ray may hit both the
			//back and the front of the triangle.
			if (Math.Abs(determinant) < 1e-6f) {
				distance = 0f;
				return false;
			}

			float inversedeterminant = 1.0f / determinant;

			//Calculate the U parameter of the intersection point.
			Vector3 distanceVector;
			distanceVector.X = pos.X - vertex1.X;
			distanceVector.Y = pos.Y - vertex1.Y;
			distanceVector.Z = pos.Z - vertex1.Z;

			float triangleU;
			triangleU = (distanceVector.X * directioncrossedge2.X) + (distanceVector.Y * directioncrossedge2.Y) + (distanceVector.Z * directioncrossedge2.Z);
			triangleU *= inversedeterminant;

			//Make sure it is inside the triangle.
			if (triangleU < 0f || triangleU > 1f) {
				distance = 0f;
				return false;
			}

			//Calculate the V parameter of the intersection point.
			Vector3 distancecrossedge1;
			distancecrossedge1.X = (distanceVector.Y * edge1.Z) - (distanceVector.Z * edge1.Y);
			distancecrossedge1.Y = (distanceVector.Z * edge1.X) - (distanceVector.X * edge1.Z);
			distancecrossedge1.Z = (distanceVector.X * edge1.Y) - (distanceVector.Y * edge1.X);

			float triangleV;
			triangleV = ((dir.X * distancecrossedge1.X) + (dir.Y * distancecrossedge1.Y)) + (dir.Z * distancecrossedge1.Z);
			triangleV *= inversedeterminant;

			//Make sure it is inside the triangle.
			if (triangleV < 0f || triangleU + triangleV > 1f) {
				distance = 0f;
				return false;
			}

			//Compute the distance along the ray to the triangle.
			float raydistance;
			raydistance = (edge2.X * distancecrossedge1.X) + (edge2.Y * distancecrossedge1.Y) + (edge2.Z * distancecrossedge1.Z);
			raydistance *= inversedeterminant;

			//Is the triangle behind the ray origin?
			if (raydistance < 0f) {
				distance = 0f;
				return false;
			}

			distance = raydistance;
			return true;
		}

		/// <summary>
		/// Determines whether there is an intersection between a <see cref="Ray"/> and a <see cref="BoundingBox"/>.
		/// </summary>
		/// <param name="ray">The ray to test.</param>
		/// <param name="box">The box to test.</param>
		/// <param name="distance">When the method completes, contains the distance of the intersection,
		/// or 0 if there was no intersection.</param>
		/// <returns>Whether the two objects intersected.</returns>
		public static bool RayIntersectsBox(Vector3 pos, Vector3 dir, Vector3 min, Vector3 max, out float distance) {
			//Source: Real-Time Collision Detection by Christer Ericson
			//Reference: Page 179

			distance = 0f;
			float tmax = float.MaxValue;

			if (Math.Abs(dir.X) < 1e-6f) {
				if (pos.X < min.X || pos.X > max.X) {
					distance = 0f;
					return false;
				}
			} else {
				float inverse = 1.0f / dir.X;
				float t1 = (min.X - pos.X) * inverse;
				float t2 = (max.X - pos.X) * inverse;

				if (t1 > t2) {
					float temp = t1;
					t1 = t2;
					t2 = temp;
				}

				distance = Math.Max(t1, distance);
				tmax = Math.Min(t2, tmax);

				if (distance > tmax) {
					distance = 0f;
					return false;
				}
			}

			if (Math.Abs(dir.Y) < 1e-6f) {
				if (pos.Y < min.Y || pos.Y > max.Y) {
					distance = 0f;
					return false;
				}
			} else {
				float inverse = 1.0f / dir.Y;
				float t1 = (min.Y - pos.Y) * inverse;
				float t2 = (max.Y - pos.Y) * inverse;

				if (t1 > t2) {
					float temp = t1;
					t1 = t2;
					t2 = temp;
				}

				distance = Math.Max(t1, distance);
				tmax = Math.Min(t2, tmax);

				if (distance > tmax) {
					distance = 0f;
					return false;
				}
			}

			if (Math.Abs(dir.Z) < 1e-6f) {
				if (pos.Z < min.Z || pos.Z > max.Z) {
					distance = 0f;
					return false;
				}
			} else {
				float inverse = 1.0f / dir.Z;
				float t1 = (min.Z - pos.Z) * inverse;
				float t2 = (max.Z - pos.Z) * inverse;

				if (t1 > t2) {
					float temp = t1;
					t1 = t2;
					t2 = temp;
				}

				distance = Math.Max(t1, distance);
				tmax = Math.Min(t2, tmax);

				if (distance > tmax) {
					distance = 0f;
					return false;
				}
			}

			return true;
		}
	}
}
