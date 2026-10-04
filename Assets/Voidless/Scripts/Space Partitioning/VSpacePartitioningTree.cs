using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Voidless.AI.PathFinding;

/*===========================================================================
**
** Class:  VSpacePartinioningTree
**
** Purpose: Static methods & functions for ISpacePartitionTree<T, B>.
**
**
** Author: Lîf Gwaethrakindo
**
===========================================================================*/
namespace Voidless
{
    public static class VSpacePartitioningTree
    {
        public static bool InsertAll<T, B>(this ISpacePartitioningTree<T, B> _tree, IEnumerable<T> _objects)
        {
            bool allInserted = true;

            foreach(T obj in _objects)
            {
                if(!_tree.Insert(obj)) allInserted = false;
            }

            return allInserted;
        }

        /// <summary>Removes all objects that follow a given criteria.</summary>
        /// <param name="_objects">Prospect objects to remove.</param>
        /// <param name="condition">Removal criteria.</param>
        /// <returns>True if all objects were successfully removed.</returns>
        public static bool RemoveAny<T, B>(this ISpacePartitioningTree<T, B> _tree, Predicate<T> condition)
        {
            if(_tree.objects == null || condition == null) return false;

            bool atLeastOne = false;

            foreach(T obj in _tree.objects)
            {
                if(condition(obj))
                {
                    _tree.Remove(obj);
                    atLeastOne = true;
                }
            }

            return atLeastOne;
        }

        /// <summary>Removes set of objects.</summary>
        /// <param name="_objects">Set of objects to remove.</param>
        /// <returns>True if all objects were successfully removed.</returns>
        public static bool RemoveAll<T, B>(this ISpacePartitioningTree<T, B> _tree, IEnumerable<T> _objects)
        {
            bool allRemoved = true;

            foreach(T obj in _objects)
            {
                if(!_tree.Remove(obj)) allRemoved = false;
            }
            
            return allRemoved;
        }

        /// <summary>Evaluates if set of objects are contained within tree's boundaries.</summary>
        /// <param name="_objects">Set of objects to evaluate.</param>
        /// <returns>True if all objects are contained.</returns>
        public static bool ContainsAll<T, B>(this ISpacePartitioningTree<T, B> _tree, IEnumerable<T> _objects)
        {
            foreach (T obj in _objects)
            {
                if(!_tree.Contains(obj)) return false;
            }

            return true;
        }

        public static bool Contains<T>(this ISpacePartitioningTree<T, Bounds> _tree, Vector3 x)
        {
            return _tree.Contains(x);
        }
        
        public static QuadTree<T> GenerateFromObjects<T>(Func<T, Rect> getRect, params T[] _objects) where T : class
        {
            Rect boundary = VRect.GetRectToFitSet(getRect, _objects);
            QuadTree<T> quadTree = new QuadTree<T>(boundary, getRect);
            quadTree.GetObjectBoundary = getRect;
            quadTree.InsertAll(_objects);

            return quadTree;
        }

        /*public static Rect GetNodeRect(this IPFNode<Vector2> _node)
        {
            return VRect.Zero(_node.data);
        }

        public static Bounds GetNodeBounds(this IPFNode<Vector3> _node)
        {
            return VBounds.Zero(_node.data);
        }*/
    }
}