using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
    [Serializable]
    public class ObjectQuadTree<T> : /*ICollection<T>, */IEnumerable<T>, IEnumerable<ObjectQuadTree<T>>
    {
        public const int MAX_POINTSPERNODE = 4;

        [SerializeField] private Rect _boundary;
        private Func<T, Rect> getRect;
        private HashSet<T> _objects;
        private ObjectQuadTree<T>[] _children;
        private bool _subdivided;

        /// <summary>Gets and Sets boundary property.</summary>
        public Rect boundary
        {
            get { return _boundary; }
            set { _boundary = value; }
        }

        /// <summary>Gets and Sets GetRect property.</summary>
        public Func<T, Rect> GetRect
        {
            get { return getRect; }
            set { getRect = value; }
        }

        /// <summary>Gets and Sets objects property.</summary>
        public HashSet<T> objects
        {
            get { return _objects; }
            private set { _objects = value; }
        }

        /// <summary>Gets and Sets children property.</summary>
        public ObjectQuadTree<T>[] children
        {
            get { return _children; }
            private set { _children = value; }
        }

        /// <summary>Gets and Sets subdivided property.</summary>
        public bool subdivided
        {
            get { return _subdivided; }
            private set { _subdivided = value; }
        }

        /// <returns>Collection's Count.</returns>
        public int Count { get { return children != null ? children.Length : 0; } }

        /// <returns>False [it is a write/read structure].</returns>
        public bool IsReadOnly { get { return false; } }

        /// <summary>ObjectQuadTree's constructor.</summary>
        /// <param name="_boundary">QuadTree's boundaries.</param>
        /// <param name="getRect">Rect function [null by default].</param>
        public ObjectQuadTree(Rect _boundary, Func<T, Rect> getRect = null)
        {
            boundary = _boundary;
            GetRect = getRect;
            objects = new HashSet<T>();
            children = new ObjectQuadTree<T>[MAX_POINTSPERNODE];
        }

        /// <summary>Tries to insert object into QuadTree.</summary>
        /// <param name="_object">Object to add.</param>
        /// <returns>True if object was successfully added (whether here or within children).</returns>
        public bool Insert(T _object)
        {
            if(GetRect == null) return false;

            Rect rect = GetRect(_object);

            if(!boundary.Contains(rect)) return false;

            /// Add the point if there is still room.
            if(objects.Count < MAX_POINTSPERNODE)
            {
                objects.Add(_object);
                return true;
            }

            /// Subdivide if necessary.
            if(!subdivided) Subdivide();

            foreach(ObjectQuadTree<T> child in children)
            {
                if(child.Insert(_object)) return true;
            }

            return false;
        }

        /// <summary>Removes Object.</summary>
        /// <param name="_object">Object to remove [if it is contained within QuadTree].</param>
        /// <returns>True if it was successfully removed.</returns>
        public bool Remove(T _object)
        {
            if (GetRect == null) return false;

            Rect rect = GetRect(_object);

            // If the object doesn't belong in this boundary, return false
            if (!boundary.Contains(rect)) return false;

            // If the object is in this node, remove it
            if (objects.Remove(_object)) return true;

            // Try to remove the object from the children
            if (subdivided)
            {
                foreach (ObjectQuadTree<T> child in children)
                {
                    if (child.Remove(_object)) return true;
                }
            }

            return false;
        }

        /// \TODO Improve this to be more efficient.
        /// <summary>Updates Object [if it did move from position].</summary>
        /// <param name="_object">Object to update.</param>
        public void UpdateObject(T _object)
        {
            Remove(_object);
            Insert(_object);
        }

        /// <summary>Subdivides QuadTree into 4 more sub-QuadTrees.</summary>
        private void Subdivide()
        {
            float hWidth = boundary.width * 0.5f;
            float hHeight = boundary.height * 0.5f;
            Vector2 c = boundary.center;

            children[0] = new ObjectQuadTree<T>(new Rect(c.x, c.y, hWidth, hHeight), GetRect); // NE
            children[1] = new ObjectQuadTree<T>(new Rect(c.x - hWidth, c.y, hWidth, hHeight), GetRect); // NW
            children[2] = new ObjectQuadTree<T>(new Rect(c.x, c.y - hHeight, hWidth, hHeight), GetRect); // SE
            children[3] = new ObjectQuadTree<T>(new Rect(c.x - hWidth, c.y - hHeight, hWidth, hHeight), GetRect); // SW

            subdivided = true;

            // Redistribute objects to children
            foreach (T obj in objects)
            {
                bool added = false;
                foreach (ObjectQuadTree<T> child in children)
                {
                    if (child.Insert(obj))
                    {
                        added = true;
                        break;
                    }
                }
                // If the object can't fit in any child, leave it in the parent
                if (!added)
                {
                    break;
                }
            }

            // Clear the current objects if they are fully moved into children
            objects.Clear();
        }

        /// <summary>Gets Objets within given range.</summary>
        /// <param name="_range">Query's Range.</param>
        /// <param name="_found">List of found objects passed by reference.</param>
        /// <returns>List of found objects within range.</returns>
        public List<T> Query(Rect _range, ref List<T> _found, bool _resetList = true)
        {
            if(_found == null) _found = new List<T>();
            if(_resetList) _found.Clear();

            if(GetRect == null) return null;

            if(!VRect.Intersects(boundary, _range)) return _found;

            foreach(T obj in objects)
            {
                if(VRect.Contains(_range, GetRect(obj))) _found.Add(obj);
            }

            if(subdivided) foreach(ObjectQuadTree<T> child in children)
            {
                child.Query(_range, ref _found, false);
            }

            return _found;
        }

        /// <summary>Finds Neighbors at a given distance radius.</summary>
        /// <param name="_object">Reference object.</param>
        /// <param name="_distance">Distance radius.</param>
        /// <returns>Neighbors inside the distance radius of the referenced object.</returns>
        public List<T> FindNeighbors(T _object, float _distance, ref List<T> _neighbors)
        {
            float d = _distance * 2.0f;
            Rect searchArea = new Rect(GetRect(_object).center, new Vector2(d, d));
            return Query(searchArea, ref _neighbors);
        }

        /// <summary>Draws Gizmos, to this QuadTree and children QuadTrees.</summary>
        public void DrawGizmos()
        {
            VGizmos.DrawWireRect(boundary);
         
            if(GetRect == null) return;
            
            if(objects != null) foreach(T obj in objects)
            {
                Rect rect = GetRect(obj);

                if(rect.size.sqrMagnitude > 0.1f) VGizmos.DrawWireRect(rect);
                else Gizmos.DrawSphere(rect.center, 0.05f);
            }

            if(children != null) foreach(ObjectQuadTree<T> child in children)
            {
                if(child != null) child.DrawGizmos();
            }
        }

#region StaticHelperFunctions:
        /// <returns>Rect from Collider.</returns>
        public static Rect GetColliderRect(Collider _collider) { return _collider.bounds.ToRect(); }

        /// <returns>Rect from Collider2D.</returns>
        public static Rect GetCollider2DRect(Collider2D _collider) { return _collider.bounds.ToRect(); }

        /// <returns>Rect from Renderer [includes SpriteRenderer].</returns>
        public static Rect GetRendererRect(Renderer _renderer) { return _renderer.bounds.ToRect(); }

        /// <summary>Creates a Collider's QuadTree.</summary>
        /// <param name="_boundary">QuadTree's boundaries.</param>
        public static ObjectQuadTree<Collider> CreateColliderQuadTree(Rect _boundary)
        {
            return new ObjectQuadTree<Collider>(_boundary, GetColliderRect);
        }

        /// <summary>Creates a Collider2D's QuadTree.</summary>
        /// <param name="_boundary">QuadTree's boundaries.</param>
        public static ObjectQuadTree<Collider2D> CreateCollider2DQuadTree(Rect _boundary)
        {
            return new ObjectQuadTree<Collider2D>(_boundary, GetCollider2DRect);
        }

        /// <summary>Creates a Renderer's QuadTree.</summary>
        /// <param name="_boundary">QuadTree's boundaries.</param>
        public static ObjectQuadTree<Renderer> CreateRendererQuadTree(Rect _boundary)
        {
            return new ObjectQuadTree<Renderer>(_boundary, GetRendererRect);
        }


        /// <summary>Generates QuadTree from set of objects.</summary>
        /// <param name="getRect">Rect function.</param>
        /// <param name="_objects">Set of objects.</param>
        public static ObjectQuadTree<T> GenerateFromObjects(Func<T, Rect> getRect, params T[] _objects)
        {
            if(_objects == null || _objects.Length == 0 || getRect == null) return null;

            Rect b = VRect.GetRectToFitSet<T>(getRect, _objects);
            ObjectQuadTree<T> tree = new ObjectQuadTree<T>(b, getRect);

            foreach(T obj in _objects)
            {
                tree.Insert(obj);
            }

            return tree;
        }
#endregion

#region InterfaceImplementations:
        /// <summary>Adds Item into QuadTree [internally calls for Insert without returning result].</summary>
        /// <param name="item">Item to add.</param>
        public void Add(T item)
        {
            Insert(item);
        }

        /// <summary>Clears Collection.</summary>
        public void Clear()
        {
            objects.Clear();
        }

        /// <returns>True if item is contained within QuadTree, or children if that's the case.</returns>
        public bool Contains(T item)
        {
            if(objects.Contains(item)) return true;

            foreach(ObjectQuadTree<T> child in children)
            {
                if(child.Contains(item)) return true;
            }

            return false;
        }

        /// <summary>Copies element [NOT IMPLEMENTED].</summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <returns>Iteration through collection of objects.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        /// <returns>Iteration through collection of objects.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <returns>Iteration through children QuadTrees.</returns>
        IEnumerator<ObjectQuadTree<T>> IEnumerable<ObjectQuadTree<T>>.GetEnumerator()
        {
            foreach (ObjectQuadTree<T> child in children)
            {
                yield return child;
            }
        }
#endregion
    }
}