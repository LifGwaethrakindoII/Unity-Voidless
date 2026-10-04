using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI.PathFinding
{
    [Serializable]
    public class SPPFNode<T, B> : ISPPFNode<T, B>
    {
        [SerializeField] private IPFNode<T> _parent;
        [SerializeField] private ISPPFNode<T, B> _Parent;
        [SerializeField] private List<IPFNode<T>> _neighbors;
        [SerializeField] private T _data;
        [SerializeField] private float _gCost;
        [SerializeField] private float _hCost;
        [SerializeField] private bool _traversable;
        [SerializeField] private int _flags;
        [SerializeField] private B _boundary;

        /// <summary>Gets and Sets data property.</summary>
        public T data { get { return _data; } }

        /// <summary>Sets data property.</summary>
        public void SetData(T _value) { _data = _value; }

        /// <summary>Gets and Sets parent property.</summary>
        public IPFNode<T> parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <summary>Gets and Sets Parent property.</summary>
        public ISPPFNode<T, B> Parent
        
        {
            get { return _Parent; }
            set { _Parent = value; }
        }

        /// <summary>Gets and Sets neighbors property.</summary>
        public List<IPFNode<T>> neighbors
        {
            get { return _neighbors; }
            set { _neighbors = value; }
        }

        /// <summary>Gets and Sets gCost property.</summary>
        public float gCost
        {
            get { return _gCost; }
            set { _gCost = value; }
        }

        /// <summary>Gets and Sets hCost property.</summary>
        public float hCost
        {
            get { return _hCost; }
            set { _hCost = value; }
        }

        /// <summary>Gets and Sets traversable property.</summary>
        public bool traversable
        {
            get { return _traversable; }
            set { _traversable = value; }
        }

        /// <summary>Gets and Sets flags property.</summary>
        public int flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        /// <summary>Gets and Sets boundary property.</summary>
        public B boundary
        {
            get { return _boundary; }
            set { _boundary = value; }
        }

        /// <summary>SPPFNode's constructor.</summary>
        /// <param name="_data">Data.</param>
        /// <param name="_boundary">Boundary.</param>
        /// <param name="_traversable">Is it traversable? True by default.</param>
        /// <param name="_flags">Additional flags, none by default.</param>
        public SPPFNode(T _data, B _boundary, bool _traversable = true, int _flags = 0)
        {
            SetData(_data);
            boundary = _boundary;
            traversable = _traversable;
            flags = _flags;
        }

        public void AddNeighbors<G>(params G[] _neighbors) where G : IPFNode<T>
        {
            List<IPFNode<T>> newNeighbors = new List<IPFNode<T>>();

            foreach (G neighbor in _neighbors)
            {
                if(neighbor.GetHashCode() != this.GetHashCode())
                newNeighbors.Add(neighbor);
            }

            neighbors = newNeighbors;
        }

        /// <summary>Gets Node's Boundary.</summary>
        /// <param name="_node">Node's reference.</param>
        /// <returns>Node's Boundary.</returns>
        public static B GetNodeBoundary(SPPFNode<T, B> _node) { return _node.boundary; }
    }
}