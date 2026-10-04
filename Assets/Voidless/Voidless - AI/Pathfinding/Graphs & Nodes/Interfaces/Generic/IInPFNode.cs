using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless.AI.PathFinding
{
    public interface IInPFNode<in T>
    {
        void SetData(T _value);
    }
}