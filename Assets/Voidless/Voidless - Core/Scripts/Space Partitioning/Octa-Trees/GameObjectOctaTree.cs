using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
    [Serializable]
    public class GameObjectOctaTree : IEnumerable<GameObjectOctaTree>, IEnumerable<GameObject>
    {
        public const int MAX_OBJECTS_PER_NODE = 8;

        [SerializeField] private Bounds _boundary;
        private List<GameObject> _objects;  // List of GameObjects stored in this node
        private GameObjectOctaTree[] _children; // Child nodes
        private bool _subdivided;

        /// <summary>Gets and Sets boundary property.</summary>
        public Bounds boundary
        {
            get { return _boundary; }
            set { _boundary = value; }
        }

        /// <summary>Gets and Sets objects property.</summary>
        public List<GameObject> objects
        {
            get { return _objects; }
            private set { _objects = value; }
        }

        /// <summary>Gets and Sets children property.</summary>
        public GameObjectOctaTree[] children
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

        /// <summary>GameObjectOctaTree constructor.</summary>
        /// <param name="_boundary">Boundary for the OctaTree node.</param>
        public GameObjectOctaTree(Bounds _boundary)
        {
            boundary = _boundary;
            objects = new List<GameObject>();
            children = new GameObjectOctaTree[8];
        }

        /// <summary>Inserts a GameObject into the OctaTree based on its position.</summary>
        /// <param name="gameObject">GameObject to insert.</param>
        /// <returns>True if successfully inserted, false otherwise.</returns>
        public bool Insert(GameObject gameObject)
        {
            Vector3 position = gameObject.transform.position;

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
            foreach (GameObjectOctaTree child in children)
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
            children[0] = new GameObjectOctaTree(new Bounds(c + new Vector3(d.x, d.y, d.z), s));  // NEU
            children[1] = new GameObjectOctaTree(new Bounds(c + new Vector3(-d.x, d.y, d.z), s)); // NWU
            children[2] = new GameObjectOctaTree(new Bounds(c + new Vector3(d.x, -d.y, d.z), s)); // SEU
            children[3] = new GameObjectOctaTree(new Bounds(c + new Vector3(-d.x, -d.y, d.z), s)); // SWU
            children[4] = new GameObjectOctaTree(new Bounds(c + new Vector3(d.x, d.y, -d.z), s));  // NED
            children[5] = new GameObjectOctaTree(new Bounds(c + new Vector3(-d.x, d.y, -d.z), s)); // NWD
            children[6] = new GameObjectOctaTree(new Bounds(c + new Vector3(d.x, -d.y, -d.z), s)); // SED
            children[7] = new GameObjectOctaTree(new Bounds(c + new Vector3(-d.x, -d.y, -d.z), s)); // SWD

            subdivided = true;
        }

        /// <summary>Retrieves all GameObjects within a specific query range.</summary>
        /// <param name="range">Range to query.</param>
        /// <param name="foundObjects">List to hold found GameObjects.</param>
        /// <returns>List of found GameObjects.</returns>
        public List<GameObject> Query(Bounds range, ref List<GameObject> foundObjects)
        {
            if (foundObjects == null) foundObjects = new List<GameObject>();

            // If the range does not intersect this node, return empty
            if (!boundary.Intersects(range)) return foundObjects;

            // Check objects in this node
            foreach (GameObject obj in objects)
            {
                if (range.Contains(obj.transform.position)) foundObjects.Add(obj);
            }

            // Query the children if the node is subdivided
            if (subdivided)
            {
                foreach (GameObjectOctaTree child in children)
                {
                    child.Query(range, ref foundObjects);
                }
            }

            return foundObjects;
        }

        /// <summary>Implementation of IEnumerable for GameObjectOctaTree, enabling iteration through child nodes.</summary>
        /// <returns>Enumerator for GameObjectOctaTree.</returns>
        public IEnumerator<GameObjectOctaTree> GetEnumerator()
        {
            yield return this;

            if (subdivided)
            {
                foreach (GameObjectOctaTree child in children)
                {
                    foreach (GameObjectOctaTree subChild in child)
                    {
                        yield return subChild;
                    }
                }
            }
        }

        /// <summary>Implementation of IEnumerable for GameObjects, enabling iteration through objects.</summary>
        /// <returns>Enumerator for GameObjects.</returns>
        IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
        {
            foreach (GameObject obj in objects)
            {
                yield return obj;
            }

            if (subdivided)
            {
                foreach (GameObjectOctaTree child in children)
                {
                    foreach (GameObject obj in child.objects)
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
            foreach (GameObject obj in objects)
            {
                Gizmos.DrawSphere(obj.transform.position, 0.2f);
            }

            // Draw children
            if (subdivided)
            {
                foreach (GameObjectOctaTree child in children)
                {
                    child.DrawGizmos();
                }
            }
        }
    }
}
