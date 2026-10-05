using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
	public static class VVector3Int
	{
		/// <returns>Sumation of all Vector's components combined.</returns>
        public static int Sum(this Vector3Int v)
        {
            return v.x + v.y + v.z;
        }
	}
}