using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Voidless.AI.PathFinding
{
    public class AStarPathFindingAlgorithm : IPathFindingAlgorithm<Vector3>
    {
        public List<IPFNode<Vector3>> CalculatePath(IPFNode<Vector3> start, IPFNode<Vector3> end)
        {
            if(start == null || end == null) return null;

            List<IPFNode<Vector3>> open = new List<IPFNode<Vector3>>();  // Priority Queue
            HashSet<IPFNode<Vector3>> closed = new HashSet<IPFNode<Vector3>>();

            open.Add(start);

            while (open.Count > 0)
            {
                // Get node with lowest FCost()
                IPFNode<Vector3> currentNode = open.OrderBy(n => n.FCost()).ThenBy(n => n.hCost).First();

                if (currentNode == end)
                {
                    // Path found, reconstruct the path
                    return ReconstructPath(currentNode);
                }

                open.Remove(currentNode);
                closed.Add(currentNode);

                foreach (var neighbor in currentNode.neighbors)
                {
                    if (!neighbor.traversable || closed.Contains(neighbor))
                    {
                        continue;
                    }

                    float tentativegCost = currentNode.gCost + Vector3.Distance(currentNode.data, neighbor.data);

                    if (tentativegCost < neighbor.gCost || !open.Contains(neighbor))
                    {
                        neighbor.gCost = tentativegCost;
                        neighbor.hCost = Vector3.Distance(neighbor.data, end.data);
                        neighbor.parent = currentNode;

                        if (!open.Contains(neighbor))
                        {
                            open.Add(neighbor);
                        }
                    }
                }
            }

            return null; // No path found
        }

        private List<IPFNode<Vector3>> ReconstructPath(IPFNode<Vector3> node)
        {
            List<IPFNode<Vector3>> path = new List<IPFNode<Vector3>>();
            IPFNode<Vector3> currentNode = node;

            while (currentNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            return path;
        }
    }
}