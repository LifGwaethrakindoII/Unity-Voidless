using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
    [Serializable]
    public class ObjectOctaTree<T> : IEnumerable<ObjectOctaTree<T>>, IEnumerable<T>
    {
        public const int MAX_OBJECTS_PER_NODE = 8;

        private Bounds _boundary;
        private List<T> _objects;
        private ObjectOctaTree<T>[] _children;
        private bool _subdivided;

        private Func<T, Bounds> getBounds;  // Delegate for getting Bounds from object T.

        /// <summary>Gets & Sets getBounds property.</summary>
        public Func<T, Bounds> GetBounds
        {
            get { return getBounds; }
            set { getBounds = value; }
        }

        /// <summary>Gets and Sets boundary property.</summary>
        public Bounds boundary
        {
            get { return _boundary; }
            set { _boundary = value; }
        }

        /// <summary>Gets and Sets objects property.</summary>
        public List<T> objects
        {
            get { return _objects; }
            private set { _objects = value; }
        }

        /// <summary>Gets and Sets children property.</summary>
        public ObjectOctaTree<T>[] children
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

        /// <summary>ObjectOctaTree constructor.</summary>
        /// <param name="_boundary">Boundary for the ObjectOctaTree node.</param>
        /// <param name="GetBounds">Delegate that returns the Bounds from an object of type T.</param>
        public ObjectOctaTree(Bounds _boundary, Func<T, Bounds> GetBounds)
        {
            boundary = _boundary;
            objects = new List<T>();
            children = new ObjectOctaTree<T>[8];
            this.GetBounds = GetBounds;
        }

        /// <summary>Inserts an object into the ObjectOctaTree.</summary>
        /// <param name="obj">Object of type T to insert.</param>
        /// <returns>True if successfully inserted, false otherwise.</returns>
        public bool Insert(T obj)
        {
            Bounds bounds = GetBounds(obj);

            // If the bounds are not within this node, return false
            if (!boundary.Intersects(bounds)) return false;

            // If there's room in this node and it hasn't reached capacity
            if (objects.Count < MAX_OBJECTS_PER_NODE)
            {
                objects.Add(obj);
                return true;
            }

            // Otherwise, subdivide if not already subdivided
            if (!subdivided) Subdivide();

            // Try to insert into children
            foreach (ObjectOctaTree<T> child in children)
            {
                if (child.Insert(obj)) return true;
            }

            return false;
        }

        /// <summary>Subdivides the ObjectOctaTree into 8 smaller regions.</summary>
        private void Subdivide()
        {
            Vector3 s = boundary.size * 0.5f;
            Vector3 d = s * 0.5f;
            Vector3 c = boundary.center;

            // Create 8 sub-regions (octants)
            children[0] = new ObjectOctaTree<T>(new Bounds(c + new Vector3(d.x, d.y, d.z), s), GetBounds);  // NEU
            children[1] = new ObjectOctaTree<T>(new Bounds(c + new Vector3(-d.x, d.y, d.z), s), GetBounds); // NWU
            children[2] = new ObjectOctaTree<T>(new Bounds(c + new Vector3(d.x, -d.y, d.z), s), GetBounds); // SEU
            children[3] = new ObjectOctaTree<T>(new Bounds(c + new Vector3(-d.x, -d.y, d.z), s), GetBounds); // SWU
            children[4] = new ObjectOctaTree<T>(new Bounds(c + new Vector3(d.x, d.y, -d.z), s), GetBounds);  // NED
            children[5] = new ObjectOctaTree<T>(new Bounds(c + new Vector3(-d.x, d.y, -d.z), s), GetBounds); // NWD
            children[6] = new ObjectOctaTree<T>(new Bounds(c + new Vector3(d.x, -d.y, -d.z), s), GetBounds); // SED
            children[7] = new ObjectOctaTree<T>(new Bounds(c + new Vector3(-d.x, -d.y, -d.z), s), GetBounds); // SWD

            subdivided = true;
        }

        /// <summary>Retrieves all objects within a specific query range.</summary>
        /// <param name="range">Range to query.</param>
        /// <param name="foundObjects">List to hold found objects of type T.</param>
        /// <returns>List of found objects.</returns>
        public List<T> Query(Bounds range, ref List<T> foundObjects)
        {
            if (foundObjects == null) foundObjects = new List<T>();

            // If the range does not intersect this node, return empty
            if (!boundary.Intersects(range)) return foundObjects;

            // Check objects in this node
            foreach (T obj in objects)
            {
                Bounds objBounds = GetBounds(obj);
                if (range.Intersects(objBounds)) foundObjects.Add(obj);
            }

            // Query the children if the node is subdivided
            if (subdivided)
            {
                foreach (ObjectOctaTree<T> child in children)
                {
                    child.Query(range, ref foundObjects);
                }
            }

            return foundObjects;
        }

        /// <summary>Implementation of IEnumerable for ObjectOctaTree, enabling iteration through child nodes.</summary>
        /// <returns>Enumerator for ObjectOctaTree.</returns>
        public IEnumerator<ObjectOctaTree<T>> GetEnumerator()
        {
            yield return this;

            if (subdivided)
            {
                foreach (ObjectOctaTree<T> child in children)
                {
                    foreach (ObjectOctaTree<T> subChild in child)
                    {
                        yield return subChild;
                    }
                }
            }
        }

        /// <summary>Implementation of IEnumerable for objects of type T, enabling iteration through the objects.</summary>
        /// <returns>Enumerator for objects of type T.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (T obj in objects)
            {
                yield return obj;
            }

            if (subdivided)
            {
                foreach (ObjectOctaTree<T> child in children)
                {
                    foreach (T obj in child.objects)
                    {
                        yield return obj;
                    }
                }
            }
        }

        /// <summary>Non-generic IEnumerator implementation.</summary>
        /// <returns>Non-generic IEnumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Draws Gizmos for visualization in the editor.</summary>
        public void DrawGizmos()
        {
            Gizmos.DrawWireCube(boundary.center, boundary.size);

            // Draw objects
            foreach (T obj in objects)
            {
                Bounds objBounds = GetBounds(obj);
                if (objBounds.size.sqrMagnitude > 0.01f) Gizmos.DrawWireCube(objBounds.center, objBounds.size);
                else Gizmos.DrawSphere(objBounds.center, 0.2f);
            }

            // Draw children
            if (subdivided)
            {
                foreach (ObjectOctaTree<T> child in children)
                {
                    child.DrawGizmos();
                }
            }
        }

#region StaticHelperFunctions:

        public static Bounds GetCollider2DBounds(Collider2D _collider) { return _collider.bounds; }
        public static Bounds GetColliderBounds(Collider _collider) { return _collider.bounds; }

        public static Bounds GetRendererBounds(Renderer _renderer) { return _renderer.bounds; }

        public static ObjectOctaTree<Collider> CreateColliderOctaTree(Bounds _boundary)
        {
            return new ObjectOctaTree<Collider>(_boundary, GetColliderBounds);
        }

        public static ObjectOctaTree<Collider2D> CreateCollider2DOctaTree(Bounds _boundary)
        {
            return new ObjectOctaTree<Collider2D>(_boundary, GetCollider2DBounds);
        }

        public static ObjectOctaTree<Renderer> CreateRendererOctaTree(Bounds _boundary)
        {
            return new ObjectOctaTree<Renderer>(_boundary, GetRendererBounds);
        }

        public static ObjectOctaTree<T> GenerateFromObjects(Func<T, Bounds> getRect, params T[] _objects)
        {
            if (_objects == null || _objects.Length == 0 || getRect == null) return null;

            Bounds bounds = VBounds.GetBoundsToFitSet<T>(getRect, _objects);
            ObjectOctaTree<T> tree = new ObjectOctaTree<T>(bounds, getRect);

            foreach (T obj in _objects)
            {
                tree.Insert(obj);
            }

            return tree;
        }

#endregion

    }
}
