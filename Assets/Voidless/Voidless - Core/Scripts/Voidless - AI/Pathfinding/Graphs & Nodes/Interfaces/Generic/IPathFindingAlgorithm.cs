using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI.PathFinding
{
    public interface IPathFindingAlgorithm<T>
    {
        public List<IPFNode<T>> CalculatePath(IPFNode<T> start, IPFNode<T> end);
    }
}