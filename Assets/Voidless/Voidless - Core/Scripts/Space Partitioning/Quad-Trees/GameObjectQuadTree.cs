using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
    [System.Serializable]
    public class GameObjectQuadTree : IEnumerable<GameObject>
    {
        public const int MAX_OBJECTS_PER_NODE = 4;

        [SerializeField] private Rect _boundary;
        private HashSet<GameObject> _objects;
        private GameObjectQuadTree[] _children;
        private bool _subdivided;

        /// <summary>Gets or sets the boundary of the QuadTree node.</summary>
        public Rect boundary
        {
            get { return _boundary; }
            set { _boundary = value; }
        }

        /// <summary>Gets or sets the list of GameObjects stored in this node.</summary>
        public HashSet<GameObject> objects
        {
            get { return _objects; }
            private set { _objects = value; }
        }

        /// <summary>Gets or sets the child nodes of this QuadTree node.</summary>
        public GameObjectQuadTree[] children
        {
            get { return _children; }
            private set { _children = value; }
        }

        /// <summary>Gets whether this node has been subdivided.</summary>
        public bool subdivided
        {
            get { return _subdivided; }
            private set { _subdivided = value; }
        }

        /// <summary>Gets the number of objects stored in this node.</summary>
        public int Count { get { return _objects.Count; } }

        /// <summary>GameObjectQuadTree's constructor.</summary>
        /// <param name="_boundary">The boundary of the QuadTree node.</param>
        public GameObjectQuadTree(Rect _boundary)
        {
            boundary = _boundary;
            objects = new HashSet<GameObject>();
            children = null;
            subdivided = false;
        }

        /// <summary>Tries to insert a GameObject into the QuadTree.</summary>
        /// <param name="gameObject">The GameObject to insert (its position is used).</param>
        /// <returns>True if the GameObject was successfully inserted, false otherwise.</returns>
        public bool Insert(GameObject gameObject)
        {
            Vector2 position = gameObject.transform.position;

            // If the GameObject's position is not within the QuadTree boundary, return false.
            if (!boundary.Contains(position)) return false;

            // If the node has space, add the GameObject directly.
            if (_objects.Count < MAX_OBJECTS_PER_NODE)
            {
                _objects.Add(gameObject);
                return true;
            }

            // Subdivide and attempt to insert the GameObject into children nodes.
            if (!subdivided) Subdivide();

            foreach (GameObjectQuadTree child in _children)
            {
                if (child.Insert(gameObject))
                    return true;
            }

            return false;
        }

        /// <summary>Removes a GameObject from the QuadTree.</summary>
        /// <param name="gameObject">The GameObject to remove.</param>
        /// <returns>True if the GameObject was successfully removed, false otherwise.</returns>
        public bool Remove(GameObject gameObject)
        {
            Vector2 position = gameObject.transform.position;

            // If the GameObject's position is not within the QuadTree boundary, return false.
            if (!boundary.Contains(position)) return false;

            // If the GameObject is in this node, remove it.
            if (_objects.Remove(gameObject)) return true;

            // Attempt to remove the GameObject from child nodes.
            if (subdivided)
            {
                foreach (GameObjectQuadTree child in _children)
                {
                    if (child.Remove(gameObject))
                        return true;
                }
            }

            return false;
        }

        /// <summary>Updates the GameObject in the QuadTree when its position has changed.</summary>
        /// <param name="gameObject">The GameObject to update.</param>
        public void UpdateObject(GameObject gameObject)
        {
            Remove(gameObject);
            Insert(gameObject);
        }

        /// <summary>Subdivides the current node into four child nodes (quadrants).</summary>
        private void Subdivide()
        {
            float halfWidth = boundary.width * 0.5f;
            float halfHeight = boundary.height * 0.5f;
            Vector2 center = boundary.center;

            // Create the four quadrants
            _children = new GameObjectQuadTree[4];
            _children[0] = new GameObjectQuadTree(new Rect(center.x, center.y, halfWidth, halfHeight)); // NE
            _children[1] = new GameObjectQuadTree(new Rect(center.x - halfWidth, center.y, halfWidth, halfHeight)); // NW
            _children[2] = new GameObjectQuadTree(new Rect(center.x, center.y - halfHeight, halfWidth, halfHeight)); // SE
            _children[3] = new GameObjectQuadTree(new Rect(center.x - halfWidth, center.y - halfHeight, halfWidth, halfHeight)); // SW

            subdivided = true;

            // Redistribute existing objects into the child nodes.
            foreach (GameObject obj in _objects)
            {
                bool added = false;
                foreach (GameObjectQuadTree child in _children)
                {
                    if (child.Insert(obj))
                    {
                        added = true;
                        break;
                    }
                }

                // If the object couldn't be added to any child, leave it in the parent node.
                if (!added) break;
            }

            // Clear the current node's objects if they've all been redistributed.
            _objects.Clear();
        }

        /// <summary>Gets all GameObjects within a specified range.</summary>
        /// <param name="range">The range (Rect) to query.</param>
        /// <param name="foundObjects">List to store found GameObjects (passed by reference).</param>
        /// <returns>List of found GameObjects within the range.</returns>
        public List<GameObject> Query(Rect range, ref List<GameObject> foundObjects)
        {
            if (foundObjects == null) foundObjects = new List<GameObject>();

            // If this node's boundary doesn't intersect the query range, return the current list.
            if (!boundary.Overlaps(range)) return foundObjects;

            // Check objects in this node.
            foreach (GameObject obj in _objects)
            {
                if (range.Contains((Vector2)obj.transform.position))
                    foundObjects.Add(obj);
            }

            // Check objects in child nodes if the node is subdivided.
            if (subdivided)
            {
                foreach (GameObjectQuadTree child in _children)
                {
                    child.Query(range, ref foundObjects);
                }
            }

            return foundObjects;
        }

        /// <summary>Finds GameObjects that are neighbors within a given distance.</summary>
        /// <param name="gameObject">The reference GameObject to find neighbors for.</param>
        /// <param name="distance">The distance radius for neighbors.</param>
        /// <returns>List of neighbor GameObjects within the distance radius.</returns>
        public List<GameObject> FindNeighbors(GameObject gameObject, float distance)
        {
            List<GameObject> neighbors = new List<GameObject>();
            float doubleDistance = distance * 2.0f;
            Vector2 position = gameObject.transform.position;
            Rect searchArea = new Rect(position.x - distance, position.y - distance, doubleDistance, doubleDistance);

            return Query(searchArea, ref neighbors);
        }

        /// <summary>Draws Gizmos to visualize the QuadTree and its objects.</summary>
        public void DrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boundary.center, boundary.size);

            // Draw objects in this node.
            foreach (GameObject obj in _objects)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(obj.transform.position, 0.1f);
            }

            // Draw child nodes recursively if subdivided.
            if (subdivided)
            {
                foreach (GameObjectQuadTree child in _children)
                {
                    child.DrawGizmos();
                }
            }
        }

#region InterfaceImplementations:
        /// <summary>Gets an enumerator for iterating through all GameObjects in this QuadTree.</summary>
        public IEnumerator<GameObject> GetEnumerator()
        {
            foreach (GameObject obj in _objects)
            {
                yield return obj;
            }

            if (subdivided)
            {
                foreach (GameObjectQuadTree child in _children)
                {
                    foreach (GameObject obj in child)
                    {
                        yield return obj;
                    }
                }
            }
        }

        /// <summary>Gets an enumerator for iterating through all GameObjects in this QuadTree (non-generic).</summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#endregion
    }
}
