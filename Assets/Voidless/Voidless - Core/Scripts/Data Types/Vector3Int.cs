using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
	[Serializable]
	public struct Vector3Int
	{
		public static readonly Vector3Int left = new Vector3Int(-1, 0, 0);
		public static readonly Vector3Int right = new Vector3Int(1, 0, 0);
		public static readonly Vector3Int down = new Vector3Int(0, -1, 0);
		public static readonly Vector3Int up = new Vector3Int(0, 1, 0);
		public static readonly Vector3Int back = new Vector3Int(0, 0, -1);
		public static readonly Vector3Int forward = new Vector3Int(0, 0, 1);

		[SerializeField] private int _x;
		[SerializeField] private int _y;
		[SerializeField] private int _z;

		public int x
		{
			get { return _x; }
			set { _x = value; }
		}

		public int y
		{
			get { return _y; }
			set { _y = value; }
		}

		public int z
		{
			get { return _z; }
			set { _z = value; }
		}

		public static Vector3Int operator + (Vector3Int a, Vector3Int b)
		{
			return new Vector3Int
			(
				a.x + b.x,
				a.y + b.y,
				a.z + b.z
			);
		}

		public static Vector3Int operator - (Vector3Int a, Vector3Int b)
		{
			return new Vector3Int
			(
				a.x - b.x,
				a.y - b.y,
				a.z - b.z
			);
		}

		public static Vector3Int operator * (Vector3Int a, int x)
		{
			return new Vector3Int
			(
				a.x * x,
				a.y * x,
				a.z * x
			);
		}

		public static Vector3Int operator / (Vector3Int a, int x)
		{
			return new Vector3Int
			(
				a.x / x,
				a.y / x,
				a.z / x
			);
		}

		public Vector3Int(int _x, int _y, int _z) : this()
		{
			x = _x;
			y = _y;
			z = _z;
		}
	}
}