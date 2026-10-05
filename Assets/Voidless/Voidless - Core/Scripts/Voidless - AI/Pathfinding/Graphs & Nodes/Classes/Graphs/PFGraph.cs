using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.Oculus;
using Unity.XR.CoreUtils;

/*===========================================================================
**
** Class:  PFGraph
**
** Purpose: Graph for PathFinding.
**
**
** Author: Lîf Gwaethrakindo
**
===========================================================================*/
/// \TODO Create a PFGraph2D that has a QuadTree instead of OctaTree.
namespace Voidless.AI.PathFinding
{
    [Serializable]
    public class PFGraph : IPFGraph<Vector3, Bounds>
    {
        [SerializeField] private OctaTree<PFNode> _octaTree;

        /// <summary>Gets and Sets octaTree property.</summary>
        public OctaTree<PFNode> octaTree
        {
            get { return _octaTree; }
            set { _octaTree = value; }
        }

        public virtual int Count => octaTree != null ? octaTree.Count : 0;

        public bool IsReadOnly => throw new NotImplementedException();

        public PFGraph()
        {
            octaTree = new OctaTree<PFNode>(new Bounds(), PFNode.GetBoundary);
        }

        public PFGraph(Bounds _boundary, params PFNode[] _nodes) : this()
        {
            octaTree = new OctaTree<PFNode>(_boundary, PFNode.GetBoundary);
        }

        public PFGraph(OctaTree<PFNode> _octaTree)
        {
            octaTree = _octaTree;
        }

        public PFGraph(params PFNode[] _nodes) : this()
        {
            Func<PFNode, Bounds> f = PFNode.GetBoundary;
            Bounds boundary = VBounds.GetBoundsToFitSet(f, _nodes);
            octaTree = new OctaTree<PFNode>(boundary, f);
        }

        /// <summary>Draws Gizmos.</summary>
        public virtual void DrawGizmos()
        {
            if(octaTree != null) octaTree.DrawGizmos();
        }

        public ISPPFNode<Vector3, Bounds> GetClosestNode(Vector3 _data)
        {
            if(octaTree == null) return null;

            return octaTree.GetClosestObject(_data);
        }

        public virtual IEnumerator<ISPPFNode<Vector3, Bounds>> GetEnumerator()
        {
            return octaTree.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ISPPFNode<Vector3, Bounds> item)
        {
            if(octaTree == null) return;

            PFNode node = item as PFNode;

            if(node != null) octaTree.Insert(node);
        }

        public void Clear()
        {
            if(octaTree != null) octaTree.Clear();
        }

        public bool Contains(ISPPFNode<Vector3, Bounds> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(ISPPFNode<Vector3, Bounds>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(ISPPFNode<Vector3, Bounds> item)
        {
            throw new NotImplementedException();
        }

        public void UpdateNeighbors()
        {
            if(octaTree == null || octaTree.Count == 0) return;
        
            foreach(PFNode node in octaTree.objects)
            {
                List<PFNode> neighbors = null;
                octaTree.FindNeighbors(node, node.boundary.extents.MaxComponent() * 1.1f, ref neighbors);
                
                //node.neighbors = ;
            }
        }

        public static PFGraph ToOctaTreeGraph(Bounds boundary, LayerMask _obstacleMask)
        {
            Func<PFNode, GizmosDrawParameters> g = (n) =>
            {
                Color c;
                GizmosDrawMode m;

                switch (n.traversable)
                {
                    case true:
                        c = Color.white;
                        m = GizmosDrawMode.Wired;
                        break;

                    case false:
                        c = VColor.transparentRed;
                        m = GizmosDrawMode.Solid;
                        break;
                }
                return new GizmosDrawParameters(c, m);
            };

            OctaTree<PFNode> tree = new OctaTree<PFNode>(boundary, PFNode.GetBoundary);
            PFGraph grid = new PFGraph(tree);
            Collider[] colliders = Physics.OverlapBox(boundary.center, boundary.extents);
            grid.octaTree.GetGizmosParemeters = g;

            foreach(Collider collider in colliders)
            {
                Bounds bounds = collider.bounds;
                bool traversable = collider.isTrigger || !collider.gameObject.InsideLayerMask(_obstacleMask);
                PFNode node = new PFNode(bounds.center, bounds, traversable);
                
                grid.octaTree.Insert(node);
            }

            foreach(PFNode node in grid.octaTree)
            {
                List<PFNode> neighbors = null;
                grid.octaTree.FindNeighbors(node, 2.0f, ref neighbors);
                node.AddNeighbors(neighbors.ToArray());
            }

            Debug.Log("Grid's Boundaries: " + grid.octaTree.boundary.ToString());

            return grid;
        }
    }
}