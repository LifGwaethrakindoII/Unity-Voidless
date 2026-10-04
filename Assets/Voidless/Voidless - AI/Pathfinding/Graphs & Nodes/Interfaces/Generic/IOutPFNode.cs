using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI.PathFinding
{
    public interface IOutPFNode<out T>
    {
        public T data { get; }
    }
}