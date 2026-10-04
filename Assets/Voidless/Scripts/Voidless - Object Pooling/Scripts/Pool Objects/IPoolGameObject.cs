using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
    public interface IPoolGameObject : IPoolObject
    {
        /// <summary>Transform reference for the frustum evaluation.</summary>
		Transform referenceTransform { get; }
    }
}