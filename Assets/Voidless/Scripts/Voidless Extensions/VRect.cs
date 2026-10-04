using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
	public static class VRect
	{
		public static Rect Zero(Vector2 p)
		{
			return new Rect(p, Vector2.zero);
		}

		public static Rect ToRect(this Bounds _bounds)
		{
			return FromCenter(_bounds.center, _bounds.size);
		}

		public static Vector2 Extents(this Rect _rect)
		{
			return _rect.size * 0.5f;
		}

		public static Rect FromCenter(Vector2 c, Vector2 s)
		{
			return new Rect(c - (s * 0.5f), s);
		}

		public static Rect MoveWithCenter(this Rect _rect, Vector2 c)
		{
			return FromCenter(c, _rect.Extents());
		}

		/// <summary>Returns rect with modified X.</summary>
		/// <param name="_rect">Rect to extend.</param>
		/// <param name="_x">X to modify from extended Rect.</param>
		/// <returns>Modified Rect.</returns>
		public static Rect WithX(this Rect _rect, float _x)
		{
			Rect result = _rect;
			result.x = _x;

			return result;
		}

		/// <summary>Returns rect with modified Y.</summary>
		/// <param name="_rect">Rect to extend.</param>
		/// <param name="_y">Y to modify from extended Rect.</param>
		/// <returns>Modified Rect.</returns>
		public static Rect WithY(this Rect _rect, float _y)
		{
			Rect result = _rect;
			result.y = _y;
			
			return result;
		}

		/// <summary>Returns rect with modified Width.</summary>
		/// <param name="_rect">Rect to extend.</param>
		/// <param name="_width">Width to modify from extended Rect.</param>
		/// <returns>Modified Rect.</returns>
		public static Rect WithWidth(this Rect _rect, float _width)
		{
			Rect result = _rect;
			result.width = _width;
			
			return result;
		}

		/// <summary>Returns rect with modified Height.</summary>
		/// <param name="_rect">Rect to extend.</param>
		/// <param name="_height">Height to modify from extended Rect.</param>
		/// <returns>Modified Rect.</returns>
		public static Rect WithHeight(this Rect _rect, float _height)
		{
			Rect result = _rect;
			result.height = _height;
			
			return result;
		}

		/// <summary>Returns rect with added Y.</summary>
		/// <param name="_rect">Rect to extend.</param>
		/// <param name="_x">X to add from extended Rect.</param>
		/// <returns>Modified Rect.</returns>
		public static Rect WithAddedX(this Rect _rect, float _x)
		{
			Rect result = _rect;
			result.x += _x;

			return result;
		}

		/// <summary>Returns rect with added Y.</summary>
		/// <param name="_rect">Rect to extend.</param>
		/// <param name="_y">Y to add from extended Rect.</param>
		/// <returns>Modified Rect.</returns>
		public static Rect WithAddedY(this Rect _rect, float _y)
		{
			Rect result = _rect;
			result.y += _y;
			
			return result;
		}

		/// <summary>Returns rect with modified Width.</summary>
		/// <param name="_rect">Rect to extend.</param>
		/// <param name="_width">Width to modify from extended Rect.</param>
		/// <returns>Modified Rect.</returns>
		public static Rect WithAddedWidth(this Rect _rect, float _width)
		{
			Rect result = _rect;
			result.width += _width;
			
			return result;
		}

		/// <summary>Returns rect with modified Height.</summary>
		/// <param name="_rect">Rect to extend.</param>
		/// <param name="_height">Height to modify from extended Rect.</param>
		/// <returns>Modified Rect.</returns>
		public static Rect WithAddedHeight(this Rect _rect, float _height)
		{
			Rect result = _rect;
			result.height += _height;
			
			return result;
		}

        // Check if a point is within a 2D bounding box (Rect)
        public static bool Contains(Rect rect, Vector2 point)
        {
            return (point.x >= rect.xMin && point.x <= rect.xMax &&
                    point.y >= rect.yMin && point.y <= rect.yMax);
        }

        // Check if a point is within a 2D bounding box (Rect)
        public static bool Contains(Rect rect, Vector3 point)
        {
            return (point.x >= rect.xMin && point.x <= rect.xMax &&
                    point.y >= rect.yMin && point.y <= rect.yMax);
        }

	    /// <summary>
	    /// Checks if Rect 'a' fully contains Rect 'b'.
	    /// </summary>
	    /// <param name="a">The outer Rect.</param>
	    /// <param name="b">The inner Rect to check containment.</param>
	    /// <returns>True if 'a' fully contains 'b', otherwise False.</returns>
	    public static bool Contains(this Rect a, Rect b)
	    {
	        // Check if 'a' contains 'b.min' and 'b.max' (the extreme corners of the bounding box 'b').
	        return a.Contains(b.min) && a.Contains(b.max);
	    }

	    // Check if two 2D bounding boxes (Rect) intersect
	    public static bool Intersects(Rect a, Rect b)
	    {
	        return a.xMin <= b.xMax && a.xMax >= b.xMin &&
	               a.yMin <= b.yMax && a.yMax >= b.yMin;
	    }

	    /// <summary>Calculates Rect that better fit a given set of Rect.</summary>
		/// <param name="_bounds">Set of Rect.</param>
		/// <returns>Rect that fit set of Rect [it will return default Rect if the set is empty].</returns>
		public static Rect GetRectToFitSet(params Rect[] _bounds)
		{
			int length = _bounds.Length;

			if(_bounds == null || length == 0)
			{
#if UNITY_EDITOR
				Debug.Log("[VRect] No Rect provided as argument, returning a default Rect' structure...");
#endif
				return new Rect();

			} else if(length == 1) return _bounds[0];

			Rect a = _bounds[0];
			Rect b = default(Rect);

			for(int i = 1; i < length; i++)
			{
				b = _bounds[i];
				a = VMath.GetRectToFitPair(a, b);
			}

			return a;
		}

		/// <summary>Calculates Rect that better fit a given set of Rect.</summary>
		/// <param name="_points">Set of Rect.</param>
		/// <returns>Rect that fit set of Rect [it will return default Rect if the set is empty].</returns>
		public static Rect GetRectToFitSet(params Vector2[] _points)
		{
			int length = _points.Length;

			if(_points == null || length == 0)
			{
#if UNITY_EDITOR
				Debug.Log("[VRect] No Rect provided as argument, returning a default Rect' structure...");
#endif
				return new Rect();

			} else if(length == 1) return new Rect(_points[0], Vector2.zero);

			Rect a = new Rect(_points[0], Vector2.zero);
			Rect b = default(Rect);

			for(int i = 1; i < length; i++)
			{
				b = new Rect(_points[i], Vector2.zero);
				a = VMath.GetRectToFitPair(a, b);
			}

			return a;
		}

		/// <summary>Calculates Rect that better fit a given set of Rect.</summary>
		/// <param name="_bounds">Set of Rect.</param>
		/// <returns>Rect that fit set of Rect [it will return default Rect if the set is empty].</returns>
		public static Rect GetRectToFitSet<T>(Func<T, Rect> getRect, params T[] _objects)
		{
			int length = _objects.Length;

			if(_objects == null || length == 0)
			{
#if UNITY_EDITOR
				Debug.Log("[VRect] No Rect provided as argument, returning a default Rect' structure...");
#endif
				return new Rect();

			} else if(length == 1) return getRect(_objects[0]);

			Rect a = getRect(_objects[0]);
			Rect b = default(Rect);

			for(int i = 1; i < length; i++)
			{
				b = getRect(_objects[i]);
				a = VMath.GetRectToFitPair(a, b);
			}

			return a;
		}
	}
}