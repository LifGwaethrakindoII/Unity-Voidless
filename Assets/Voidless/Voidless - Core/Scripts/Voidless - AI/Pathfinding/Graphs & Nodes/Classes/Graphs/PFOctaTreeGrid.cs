using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voidless.AI.PathFinding
{
    public class PFOctaTreeGrid : PFGraph
    {
        private OctaTree<PFNode> _nodeTree;

        /// <summary>Gets & Sets nodeTree property.</summary>
        public OctaTree<PFNode> nodeTree
        {
            get { return _nodeTree; }
            set { _nodeTree = value; }
        }

        public override int Count { get { return nodeTree != null && nodeTree.objects != null ? nodeTree.objects.Count : 0; } }

        public PFOctaTreeGrid() : base()
        {
            nodeTree = new OctaTree<PFNode>(default(Bounds), PFNode.GetBoundary);
        }

        /// <summary>Draws Gizmos.</summary>
        public override void DrawGizmos()
        {
            base.DrawGizmos();
            if (nodeTree != null)
            {
                nodeTree.DrawGizmos();
                Debug.Log("Debug..");
            }
        }
        public override IEnumerator<ISPPFNode<Vector3, Bounds>> GetEnumerator()
        {
            foreach (PFNode node in nodeTree)
            {
                yield return node;
            }
        }
    }
}