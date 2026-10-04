using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
    public class SPNode<T, B> : ISPNode<T, B>
    {
        private ISpacePartitioningTree<T, B> _parent;
        private T _item;
        private B _boundary;

        /// <summary>Gets and Sets parent property.</summary>
        public ISpacePartitioningTree<T, B> parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        /// <summary>Gets and Sets item property.</summary>
        public T item
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>Gets and Sets boundary property.</summary>
        public B boundary
        {
            get { return _boundary; }
            set { _boundary = value; }
        }
    }
}