using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI.PathFinding
{
    public interface ISPPFNode<T, B> : IPFNode<T>
    {
        ISPPFNode<T, B> Parent { get; set; }
        B boundary { get; set; }
    }
}