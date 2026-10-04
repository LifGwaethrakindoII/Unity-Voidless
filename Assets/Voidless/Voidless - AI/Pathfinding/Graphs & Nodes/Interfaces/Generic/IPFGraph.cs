using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI.PathFinding
{
    public interface IPFGraph<T, B> : IEnumerable<ISPPFNode<T, B>>, ICollection<ISPPFNode<T, B>>
    {
        ISPPFNode<T, B> GetClosestNode(T _data);
    }
}