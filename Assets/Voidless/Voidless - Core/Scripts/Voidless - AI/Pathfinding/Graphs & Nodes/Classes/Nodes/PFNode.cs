using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.Oculus;

namespace Voidless.AI.PathFinding
{
    [Serializable]
    public class PFNode : SPPFNode<Vector3, Bounds>
    {
        /// <summary>PathfindingNode's constructor.</summary>
        /// <param name="_data">Data</param>
        /// <param name="_traversable">Is it traversable? True by default.</param>
        /// <param name="_flags">Additional flags, none by default.</param>
        public PFNode(Vector3 _data, Bounds _boundary, bool _traversable = true, int _flags = 0) : base(_data, _boundary, _traversable, _flags)
        { /*...*/ }

        public static Bounds GetBoundary(PFNode _node) { return _node.boundary; }
    }
}