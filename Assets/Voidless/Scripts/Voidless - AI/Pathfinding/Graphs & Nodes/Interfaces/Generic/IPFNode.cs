using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI.PathFinding
{
    public interface IPFNode<T> : IInPFNode<T>, IOutPFNode<T>
    {
        IPFNode<T> parent { get; set; }
        List<IPFNode<T>> neighbors { get; set; }
        float gCost { get; set; }
        float hCost { get; set; }
        bool traversable { get; set; }
        int flags { get; set; }
    }
}