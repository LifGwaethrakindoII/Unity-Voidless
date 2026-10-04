using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
    public interface ISPNode<T, B>
    {
        ISpacePartitioningTree<T, B> parent { get; set; }

        T item { get; set; }

        B boundary { get; set; }
    }
}