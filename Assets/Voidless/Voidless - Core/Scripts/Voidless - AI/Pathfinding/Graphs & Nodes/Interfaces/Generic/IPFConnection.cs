using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI.PathFinding
{
    public interface IPFConnection<T>
    {
        IPFNode<T> from { get; set; }
        IPFNode<T> to { get; set; }
        float cost { get; set; }
        bool directed { get; set; }
    }
}