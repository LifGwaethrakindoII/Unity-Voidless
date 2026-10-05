using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public static class VCollider2D
{
	/// <summary>Evaluates if point is inside Collider2D.</summary>
	/// <param name="_collider">Collider2D's reference.</param>
	/// <param name="p">Point to evaluate.</param>
	/// <returns>True if point is inside provided Collider2D.</returns>
	public static bool PointInside(this Collider2D _collider, Vector2 p)
	{
		Vector2 c = _collider.ClosestPoint(p);

		return c == p;
	}

		/// <summary>
		/// Returns the closest point on the Collider2D to the given position.
		/// Currently supports CircleCollider2D.
		/// </summary>
		public static Vector2 ClosestPoint(this Collider2D _collider, Vector2 _point)
		{
			CircleCollider2D circleCollider = _collider as CircleCollider2D;

			if (circleCollider != null)
			{
				// Calculate the world position of the circle's center
				Vector2 center = (Vector2)_collider.transform.position + circleCollider.offset;
				Vector2 direction = _point - center;

				// Theory: We use sqrMagnitude instead of magnitude to avoid the expensive square root calculation.
				if (direction.sqrMagnitude <= circleCollider.radius * circleCollider.radius)
				{
					// The point is inside the circle
					return _point;
				}

				// The point is outside, project it onto the edge
				return center + direction.normalized * circleCollider.radius;
			}

			// We will add BoxCollider2D and others in the next step if needed
			return _point;
		}

		/// <summary>Evaluates if point overlaps collider.</summary>
		/// <param name="_collider">Collider2D's reference.</param>
		/// <param name="p">Point to evaluate.</param>
		/// <param name="r">Radius inside the Collider to be considered overlapping.</param>
		/// <returns>True if point is overlapping provided Collider2D.</returns>
		public static bool PointOverlapping(this Collider2D _collider, Vector2 p, float r = VMath.EPSILON)
	{
		Vector2 c = (Vector2)_collider.transform.position + _collider.offset;
		Vector2 cP = _collider.ClosestPoint(p);
		Vector2 a = c - cP;
		Vector2 b = c - p;
		float dA = a.sqrMagnitude;
		float dB = b.sqrMagnitude;
		float d = (dA - dB);

		Debug.DrawRay(cP, Vector3.up * 10f, Color.red, 10f);
		Debug.DrawRay(p, Vector3.right * 10f, Color.blue, 10f);

		return d >= (r * r);
	}
}
}