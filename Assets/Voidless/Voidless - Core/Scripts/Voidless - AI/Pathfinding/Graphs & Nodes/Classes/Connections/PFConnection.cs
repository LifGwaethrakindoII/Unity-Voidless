using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI.PathFinding
{
    [Serializable]
    public class PFConnection : IPFConnection<Vector3>
    {
        [SerializeField] private IPFNode<Vector3> _from;
        [SerializeField] private IPFNode<Vector3> _to;
        [SerializeField] private bool _directed;
        private float? _cost;

        /// <summary>Gets and Sets from property.</summary>
        public IPFNode<Vector3> from
        {
            get { return _from; }
            set { _from = value; }
        }

        /// <summary>Gets and Sets to property.</summary>
        public IPFNode<Vector3> to
        {
            get { return _to; }
            set { _to = value; }
        }

        /// <summary>Gets and Sets cost property.</summary>
        public float cost
        {
            get
            {
                if(!_cost.HasValue && from != null && to != null) _cost = VVector3.SqrDistance(from.data, to.data);
                return _cost.Value;
            }
            set { _cost = value; }
        }

        /// <summary>Gets and Sets directed property.</summary>
        public bool directed
        {
            get { return _directed; }
            set { _directed = value; }
        }
    }
}
