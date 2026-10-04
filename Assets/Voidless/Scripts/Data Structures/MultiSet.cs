using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
    [Serializable]
    public class MultiSet<T> : IEnumerable<T>
    {
        private Dictionary<T, int> _items;

        /// <summary>Gets and Sets items property.</summary>
        public Dictionary<T, int> items
        {
            get { return _items; }
            private set { _items = value; }
        }

        public MultiSet()
        {
            items = new Dictionary<T, int>();
        }

        /// <summary>Add an entry to the MultiSet.</summary>
        /// <param name="item">Item to add.</param>
        /// <returns>True if successfully added.</returns>
        public void Add(T item)
        {
            if(items.ContainsKey(item)) items[item]++;
            else items.Add(item, 1);
        }

        /// <summary>Remove an entry from the MultiSet.</summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>True if successfully removed.</returns>
        public bool Remove(T item)
        {
            if(items.ContainsKey(item))
            {
                items[item]--;
                if (items[item] == 0)
                {
                    items.Remove(item);
                }
                return true;
            }
            return false;
        }

        /// <summary>Remove an entry from the MultiSet x times.</summary>
        /// <param name="item">Item to remove.</param>
        /// <param name="times">How many times to remove the entry.</param>
        /// <returns>True if successfully removed.</returns>
        public bool Remove(T item, int times)
        {
            if(items.ContainsKey(item))
            {
                int value = items[item];

                value = Mathf.Max(0, value - times);
                items[item] = value;

                if(value == 0)
                {
                    items.Remove(item);
                }
                return true;
            }
            return false;
        }

        /// <summary>Check if the MultiSet contains an entry.</summary>
        public bool Contains(T item)
        {
            return items.ContainsKey(item);
        }

        /// <summary>Get the number of occurrences of an entry.</summary>
        public int Count(T item)
        {
            int count = 0;
            return items.TryGetValue(item, out count) ? count : 0;
        }

        /// <summary>Total count of all items in the MultiSet.</summary>
        public int TotalCount()
        {
            int count = 0;
            foreach (var kvp in items)
            {
                count += kvp.Value;
            }
            return count;
        }

        /// <summary>Get all unique items in the MultiSet.</summary>
        public IEnumerable<T> UniqueItems()
        {
            return items.Keys;
        }

        /// <summary>Clear all items from the MultiSet.</summary>
        public void Clear()
        {
            items.Clear();
        }

        /// <returns>Iteration through collection of items.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return items.Keys.GetEnumerator();
        }

        /// <returns>Iteration through collection of items.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.Keys.GetEnumerator();
        }
    }
}