using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI.PathFinding
{
    [Serializable]
    public class PFNode2D : SPPFNode<Vector2, Rect>
    {
        /// <summary>PathfindingNode's constructor.</summary>
        /// <param name="_data">Data</param>
        /// <param name="_traversable">Is it traversable? True by default.</param>
        /// <param name="_flags">Additional flags, none by default.</param>
        public PFNode2D(Vector2 _data, Rect _boundary, bool _traversable = true, int _flags = 0) : base(_data, _boundary, _traversable, _flags)
        { /*...*/ }
    }
}