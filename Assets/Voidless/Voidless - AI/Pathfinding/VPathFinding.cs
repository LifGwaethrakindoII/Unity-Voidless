using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI.PathFinding
{
    public static class VPathFinding
    {
        public static float FCost<T>(this IPFNode<T> _node) { return _node.gCost + _node.hCost; }

        /// <summary>Calculates the Manhattan distance (sum of the component-wise differences) of 2 vectors.</summary>
        /// <param name="nodeA">IOutNode A.</param>
        /// <param name="nodeB">IOutNode B.</param>
        /// <returns>Manhattan distance between IOutNode<Vector3>s A and B.</returns>
        public static float ManhattanDistance(IOutPFNode<Vector3> nodeA, IOutPFNode<Vector3> nodeB)
        {
            Vector3 a = nodeA.data;
            Vector3 b = nodeB.data;

            return VVector3.ManhattanDistance(a, b);
        }

        /// <summary>Calculates the Chebysher distance (maximum of the component-wise differences) of 2 vectors.</summary>
        /// <param name="nodeA">IOutNode A.</param>
        /// <param name="nodeB">IOutNode B.</param>
        /// <returns>Chebysher distance between IOutNode<Vector3>s A and B.</returns>
        public static float ChebysherDistance(IOutPFNode<Vector3> nodeA, IOutPFNode<Vector3> nodeB)
        {
            Vector3 a = nodeA.data;
            Vector3 b = nodeB.data;
            
            return VVector3.ChebysherDistance(a, b);
        }

        /// <summary>Calculates the Manhattan distance (sum of the component-wise differences) of 2 vectors.</summary>
        /// <param name="nodeA">IOutNode A.</param>
        /// <param name="nodeB">IOutNode B.</param>
        /// <returns>Manhattan distance between IOutNode<Vector2>s A and B.</returns>
        public static float ManhattanDistance(IOutPFNode<Vector2> nodeA, IOutPFNode<Vector2> nodeB)
        {
            Vector2 a = nodeA.data;
            Vector2 b = nodeB.data;

            return VVector2.ManhattanDistance(a, b);
        }

        /// <summary>Calculates the Chebysher distance (maximum of the component-wise differences) of 2 vectors.</summary>
        /// <param name="nodeA">IOutNode A.</param>
        /// <param name="nodeB">IOutNode B.</param>
        /// <returns>Chebysher distance between IOutNode<Vector2>s A and B.</returns>
        public static float ChebysherDistance(IOutPFNode<Vector2> nodeA, IOutPFNode<Vector2> nodeB)
        {
            Vector2 a = nodeA.data;
            Vector2 b = nodeB.data;
            
            return VVector3.ChebysherDistance(a, b);
        }
    }
}