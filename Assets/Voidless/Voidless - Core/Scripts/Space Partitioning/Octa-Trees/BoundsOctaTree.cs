using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
    [Serializable]
    public class BoundsOctaTree : IEnumerable<BoundsOctaTree>, IEnumerable<Bounds>
    {
        public const int MAX_OBJECTS_PER_NODE = 8;

        [SerializeField] private Bounds _boundary;
        private List<Bounds> _objects;  // List of Boundss stored in this node
        private BoundsOctaTree[] _children; // Child nodes
        private bool _subdivided;

        /// <summary>Gets and Sets boundary property.</summary>
        public Bounds boundary
        {
            get { return _boundary; }
            set { _boundary = value; }
        }

        /// <summary>Gets and Sets objects property.</summary>
        public List<Bounds> objects
        {
            get { return _objects; }
            private set { _objects = value; }
        }

        /// <summary>Gets and Sets children property.</summary>
        public BoundsOctaTree[] children
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

        /// <summary>BoundsOctaTree constructor.</summary>
        /// <param name="_boundary">Boundary for the OctaTree node.</param>
        public BoundsOctaTree(Bounds _boundary)
        {
            boundary = _boundary;
            objects = new List<Bounds>();
            children = new BoundsOctaTree[8];
        }

        public bool Insert(Vector3 point)
        {
            Bounds b = new Bounds(point, Vector3.zero);
            return Insert(b);
        }

        /// <summary>Inserts a Bounds into the OctaTree based on its position.</summary>
        /// <param name="gameObject">Bounds to insert.</param>
        /// <returns>True if successfully inserted, false otherwise.</returns>
        public bool Insert(Bounds gameObject)
        {
            Vector3 position = gameObject.center;

            // If the position is not within this node's boundary, return false
            if (!boundary.Contains(position)) return false;

            // If there's room in this node and it hasn't reached capacity
            if (objects.Count < MAX_OBJECTS_PER_NODE)
            {
                objects.Add(gameObject);
                return true;
            }

            // Otherwise, subdivide if not already subdivided
            if (!subdivided) Subdivide();

            // Try to insert into the children
            foreach (BoundsOctaTree child in children)
            {
                if (child.Insert(gameObject)) return true;
            }

            return false;
        }

        /// <summary>Subdivides the OctaTree into 8 smaller regions.</summary>
        private void Subdivide()
        {
            Vector3 s = boundary.size * 0.5f;
            Vector3 d = s * 0.5f;
            Vector3 c = boundary.center;

            // Create 8 sub-regions (octants)
            children[0] = new BoundsOctaTree(new Bounds(c + new Vector3(d.x, d.y, d.z), s));  // NEU
            children[1] = new BoundsOctaTree(new Bounds(c + new Vector3(-d.x, d.y, d.z), s)); // NWU
            children[2] = new BoundsOctaTree(new Bounds(c + new Vector3(d.x, -d.y, d.z), s)); // SEU
            children[3] = new BoundsOctaTree(new Bounds(c + new Vector3(-d.x, -d.y, d.z), s)); // SWU
            children[4] = new BoundsOctaTree(new Bounds(c + new Vector3(d.x, d.y, -d.z), s));  // NED
            children[5] = new BoundsOctaTree(new Bounds(c + new Vector3(-d.x, d.y, -d.z), s)); // NWD
            children[6] = new BoundsOctaTree(new Bounds(c + new Vector3(d.x, -d.y, -d.z), s)); // SED
            children[7] = new BoundsOctaTree(new Bounds(c + new Vector3(-d.x, -d.y, -d.z), s)); // SWD

            subdivided = true;
        }

        /// <summary>Retrieves all Boundss within a specific query range.</summary>
        /// <param name="range">Range to query.</param>
        /// <param name="foundObjects">List to hold found Boundss.</param>
        /// <returns>List of found Boundss.</returns>
        public List<Bounds> Query(Bounds range, ref List<Bounds> foundObjects)
        {
            if (foundObjects == null) foundObjects = new List<Bounds>();

            // If the range does not intersect this node, return empty
            if (!boundary.Intersects(range)) return foundObjects;

            // Check objects in this node
            foreach (Bounds obj in objects)
            {
                if (range.Contains(obj.center)) foundObjects.Add(obj);
            }

            // Query the children if the node is subdivided
            if (subdivided)
            {
                foreach (BoundsOctaTree child in children)
                {
                    child.Query(range, ref foundObjects);
                }
            }

            return foundObjects;
        }

        /// <summary>Implementation of IEnumerable for BoundsOctaTree, enabling iteration through child nodes.</summary>
        /// <returns>Enumerator for BoundsOctaTree.</returns>
        public IEnumerator<BoundsOctaTree> GetEnumerator()
        {
            yield return this;

            if (subdivided)
            {
                foreach (BoundsOctaTree child in children)
                {
                    foreach (BoundsOctaTree subChild in child)
                    {
                        yield return subChild;
                    }
                }
            }
        }

        /// <summary>Implementation of IEnumerable for Boundss, enabling iteration through objects.</summary>
        /// <returns>Enumerator for Boundss.</returns>
        IEnumerator<Bounds> IEnumerable<Bounds>.GetEnumerator()
        {
            foreach (Bounds obj in objects)
            {
                yield return obj;
            }

            if (subdivided)
            {
                foreach (BoundsOctaTree child in children)
                {
                    foreach (Bounds obj in child.objects)
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

        public static BoundsOctaTree GenerateFromPoints(params Vector3[] points)
        {
            if (points == null || points.Length == 0) return null;

            Bounds b = VBounds.GetBoundsToFitSet(points);
            BoundsOctaTree tree = new BoundsOctaTree(b);

            foreach (Vector3 point in points)
            {
                tree.Insert(point);
            }

            return tree;
        }

        /// <summary>Draws Gizmos for visualization in the editor.</summary>
        public void DrawGizmos()
        {
            Gizmos.DrawWireCube(boundary.center, boundary.size);

            // Draw objects
            foreach (Bounds obj in objects)
            {
                Gizmos.DrawSphere(obj.center, 0.2f);
            }

            // Draw children
            if (subdivided)
            {
                foreach (BoundsOctaTree child in children)
                {
                    child.DrawGizmos();
                }
            }
        }
    }
}
