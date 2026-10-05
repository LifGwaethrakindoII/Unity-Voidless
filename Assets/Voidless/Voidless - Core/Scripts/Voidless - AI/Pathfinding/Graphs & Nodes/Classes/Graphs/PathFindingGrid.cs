using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_2017_2_OR_NEWER
    using Vector3Int = UnityEngine.Vector3Int;
#else
    using Vector3Int = Voidless.Vector3Int;
#endif

namespace Voidless.AI.PathFinding
{
    public class PFGrid : PFGraph
    {
        public static readonly Vector3Int[] DIRECTIONS_NEIGHBOR;

        private IPFNode<Vector3>[,,] _nodesGrid;
        private int _width, _height, _depth;

        /// <summary>Gets and Sets width property.</summary>
        public int width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>Gets and Sets height property.</summary>
        public int height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>Gets and Sets depth property.</summary>
        public int depth
        {
            get { return _depth; }
            set { _depth = value; }
        }

        public override int Count  { get { return nodesGrid != null ? nodesGrid.Length : 0; } }

        /// <summary>Gets and Sets nodesGrid property.</summary>
        public IPFNode<Vector3>[,,] nodesGrid
        {
            get { return _nodesGrid; }
            set { _nodesGrid = value; }
        }

        /// <summary>Gets and Sets IPFNode <Vector3>from array of IPFNodes<Vector3>.</summary>
        public IPFNode <Vector3>this[int x, int y, int z]
        {
            get { return GetNode(x, y, z); }
            set { if (IsValidPosition(x, y, z)) nodesGrid[x, y, z] = value; }
        }

        static PFGrid()
        {
            DIRECTIONS_NEIGHBOR = new Vector3Int[]
            {
                Vector3Int.left,
                Vector3Int.right,
                Vector3Int.down,
                Vector3Int.up,
                Vector3Int.back,
                Vector3Int.forward
            };
        }

        /// <summary>Gets and Sets IPFNode <Vector3>from array of IPFNodes<Vector3>.</summary>
        public IPFNode <Vector3>this[Vector3Int i]
        {
            get { return GetNode(i); }
            set { this[i.x, i.y, i.z] = value; }
        }

        /// <summary>PFGrid's constructor.</summary>
        /// <param name="_width">Dimension in the X-Axis.</param>
        /// <param name="_height">Dimension in the Y-Axis.</param>
        /// <param name="_depth">Dimension in the Z-Axis.</param>
        public PFGrid(int _width, int _height, int _depth)
        {
            width = _width;
            height = _height;
            depth = _depth;
            nodesGrid = new IPFNode<Vector3>[width, height, depth];
        }

        /// <summary>Evaluates whether a set of indices is valid.</summary>
        /// <param name="x">X's index.</param>
        /// <param name="y">Y's index.</param>
        /// <param name="z">Z's index.</param>
        /// <returns>True if the indices are within the dimension bounds.</returns>
        public bool IsValidPosition(int x, int y, int z)
        {
            return (x >= 0 && x < width)
            && (y >= 0 && y < height)
            && (z >= 0 && z < depth);
        }

        /// <summary>Evaluates whether a set of indices is valid.</summary>
        /// <param name="i">Set of indices as a Vector3Int.</param>
        /// <returns>True if the indices are within the dimension bounds.</returns>
        public bool IsValidPosition(Vector3Int i)
        {
            return IsValidPosition(i.x, i.y, i.z);
        }

        /// <summary>Returns Node given a set of indices.</summary>
        /// <param name="x">X's index.</param>
        /// <param name="y">Y's index.</param>
        /// <param name="z">Z's index.</param>
        public IPFNode <Vector3>GetNode(int x, int y, int z)
        {
            return IsValidPosition(x, y, z) ? nodesGrid[x, y, z] : null;
        }

        /// <summary>Returns Node given a set of indices.</summary>
        /// <param name="i">Set of indices as a Vector3Int.</param>
        public IPFNode <Vector3>GetNode(Vector3Int i)
        {
            return GetNode(i.x, i.y, i.z);
        }

        public List<IPFNode<Vector3>> GetNeighbors(IPFNode <Vector3>node)
        {
            return null;
        }

        public List<IPFNode<Vector3>> GetNeighbors(Vector3Int i)
        {
            List<IPFNode<Vector3>> neighbors = new List<IPFNode<Vector3>>();

            foreach (var direction in DIRECTIONS_NEIGHBOR)
            {
                int newX = i.x + direction.x;
                int newY = i.y + direction.y;
                int newZ = i.z + direction.z;
                IPFNode <Vector3>neighbor = GetNode(newX, newY, newZ);

                // Check if the new position is within grid bounds and is walkable
                if (IsValidPosition(newX, newY, newZ) && neighbor != null && neighbor.traversable)
                    neighbors.Add(neighbor);
            }

            return neighbors;
        }

        /// <returns>True if Neighbor's set of indices are of that of a diagonal neighbor.</returns>
        public bool DiagonalNeighbor(Vector3Int i)
        { /// This basically says: "if it takes more than 1 step to reach neighbor, it is diagonal".
            return i.Sum() > 1;
        }

        public void UpdateNeighbors()
        {
            if(nodesGrid == null || nodesGrid.Length == 0) return;

            for(int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        IPFNode <Vector3>node = GetNode(x, y, z);
                        UpdateNeighbors(node, x, y, z);
                    }
                }
            }
        }

        public void UpdateNeighbors(IPFNode <Vector3>node, int x, int y, int z)
        {
            if(node == null) return;
            if(node.neighbors == null) node.neighbors = new List<IPFNode<Vector3>>();
            else node.neighbors.Clear();

            Vector3Int currentDirection = new Vector3Int(x, y, z);
            int count = 0;

            foreach (Vector3Int direction in DIRECTIONS_NEIGHBOR)
            {
                Vector3Int displacedDirection = currentDirection + direction;
                if(!IsValidPosition(displacedDirection)) continue;

                node.neighbors.Add(GetNode(displacedDirection));
                count++;
            }

            Debug.Log("Got " + count + " neighbors.");
        }

        public override IEnumerator<ISPPFNode<Vector3, Bounds>> GetEnumerator()
        {
            return null;
            /*foreach(IPFNode<Vector3> node in nodesGrid)
            {
                yield return node;
            }*/
        }
    }
}