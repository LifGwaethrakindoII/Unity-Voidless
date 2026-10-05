using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
    [Serializable]
    public class OctaTree<T> : SpacePartitioningTree<T, Bounds> where T : class
    {
        public const int MAX_POINTSPERNODE = 8;
        public const int NORTHEASTUP = 0;
        public const int NORTHWESTUP = 1;
        public const int SOUTHEASTUP = 2;
        public const int SOUTHWESTUP = 3;
        public const int NORTHEASTDOWN = 4;
        public const int NORTHWESTDOWN = 5;
        public const int SOUTHEASTDOWN = 6;
        public const int SOUTHWESTDOWN = 7;

        /// <summary>Gets max children's capacity.</summary>
        public override int maxChildCapacity { get { return MAX_POINTSPERNODE; } }

        /// <summary>Gets max object's capacity.</summary>
        public override int maxObjectCapacity { get { return MAX_POINTSPERNODE; } }

        /// <summary>ObjectBoundsOctaTree's constructor.</summary>
        /// <param name="_boundary">BoundsOctaTree's boundaries.</param>
        public OctaTree(Bounds _boundary, Func<T, Bounds> getBounds = null, ISpacePartitioningTree<T, Bounds> _parent = null, int _index = ROOT) : base(_boundary, getBounds, _parent, _index) { /*...*/ }

        /// <summary>Gets Position.</summary>
        /// <param name="p">Position [as Vector3].</param>
        public override Vector3 GetPosition() { return boundary.center; }

        /// <summary>Gets Position.</summary>
        /// <param name="d">Dimensions [as Vector3].</param>
        public override Vector3 GetDimensions() { return boundary.size; }

        /// <summary>Gets Position.</summary>
        /// <param name="p">Position [as Vector2].</param>
        public override Vector2 Get2DPosition() { return boundary.center; }

        /// <summary>Gets Position.</summary>
        /// <param name="d">Dimensions [as Vector2].</param>
        public override Vector2 Get2DDimensions() { return boundary.size; }

        /// <summary>Sets Position.</summary>
        /// <param name="p">Position [as Vector3].</param>
        public override void SetPosition(Vector3 p) { _boundary.center = p; }

        /// <summary>Sets Position.</summary>
        /// <param name="d">Dimensions [as Vector3].</param>
        public override void SetDimensions(Vector3 d) { _boundary.size = d; }

        /// <summary>Sets Position.</summary>
        /// <param name="p">Position [as Vector2].</param>
        public override void SetPosition(Vector2 p) { _boundary.center = p; }

        /// <summary>Sets Position.</summary>
        /// <param name="d">Dimensions [as Vector2].</param>
        public override void SetDimensions(Vector2 d) { _boundary.size = d; }

        /// <summary>Gets Position from Object.</summary>
        /// <param name="_object">Object's reference.</param>
        /// <param name="p">Object's position [as Vector3].</param>
        public override Vector3 GetObjectPosition(T _object) { return GetObjectBoundary != null ? GetObjectBoundary(_object).center : Vector3.zero; }

        /// <summary>Gets Position from Object.</summary>
        /// <param name="_object">Object's reference.</param>
        /// <param name="d">Object's dimensions [as Vector3].</param>
        public override Vector3 GetObjectDimensions(T _object) { return GetObjectBoundary != null ? GetObjectBoundary(_object).size : Vector3.zero; }

        /// <summary>Gets Position from Object.</summary>
        /// <param name="_object">Object's reference.</param>
        /// <param name="p">Object's position [as Vector2].</param>
        public override Vector2 GetObject2DPosition(T _object) { return GetObjectBoundary != null ? GetObjectBoundary(_object).center : Vector3.zero; }

        /// <summary>Gets Position from Object.</summary>
        /// <param name="_object">Object's reference.</param>
        /// <param name="d">Object's dimensions [as Vector2].</param>
        public override Vector2 GetObject2DDimensions(T _object) { return GetObjectBoundary != null ? GetObjectBoundary(_object).size : Vector3.zero; }

        /// <summary>Calculates distance between 2 objects.</summary>
        /// <param name="a">Object A.</param>
        /// <param name="b">Object B.</param>
        /// <returns>Distance between 2 objects.</returns>
        public override float Distance(T a, T b) { return VVector3.SqrDistance(GetObjectPosition(a), GetObjectPosition(b)); }

        /// <summary>Subdivides OctaTree into 4 more sub-OctaTrees.</summary>
        public override void Subdivide()
        {
            Vector3 s = boundary.size * 0.5f;
            Vector3 d = s * 0.5f;
            Vector3 c = boundary.center;

            // Create 8 sub-regions (octants)
            children[NORTHEASTUP] = new OctaTree<T>(new Bounds(c + new Vector3(d.x, d.y, d.z), s), GetObjectBoundary, this, NORTHEASTUP);  // NEU
            children[NORTHWESTUP] = new OctaTree<T>(new Bounds(c + new Vector3(-d.x, d.y, d.z), s), GetObjectBoundary, this, NORTHWESTUP); // NWU
            children[SOUTHEASTUP] = new OctaTree<T>(new Bounds(c + new Vector3(d.x, -d.y, d.z), s), GetObjectBoundary, this, SOUTHEASTUP); // SEU
            children[SOUTHWESTUP] = new OctaTree<T>(new Bounds(c + new Vector3(-d.x, -d.y, d.z), s), GetObjectBoundary, this, SOUTHWESTUP); // SWU
            children[NORTHEASTDOWN] = new OctaTree<T>(new Bounds(c + new Vector3(d.x, d.y, -d.z), s), GetObjectBoundary, this, NORTHEASTDOWN);  // NED
            children[NORTHWESTDOWN] = new OctaTree<T>(new Bounds(c + new Vector3(-d.x, d.y, -d.z), s), GetObjectBoundary, this, NORTHWESTDOWN); // NWD
            children[SOUTHEASTDOWN] = new OctaTree<T>(new Bounds(c + new Vector3(d.x, -d.y, -d.z), s), GetObjectBoundary, this, SOUTHEASTDOWN); // SED
            children[SOUTHWESTDOWN] = new OctaTree<T>(new Bounds(c + new Vector3(-d.x, -d.y, -d.z), s), GetObjectBoundary, this, SOUTHWESTDOWN); // SWD

            subdivided = true;
            OnAfterSubdivided();
        }

        /// <summary>Evaluates if point is contained within boundaries.</summary>
        /// <param name="p">Point in 2D space.</param>
        /// <returns>True if point is contained within boundaries.</returns>
        public override bool Contains(Vector2 p)
        {
            return boundary.Contains(p);
        }

        /// <summary>Evaluates if point is contained within boundaries.</summary>
        /// <param name="p">Point in 3D space.</param>
        /// <returns>True if point is contained within boundaries.</returns>
        public override bool Contains(Vector3 p)
        {
            return boundary.Contains(p);
        }

        /// <summary>Checks if tree, or children, contain provided object.</summary>
        /// <param name="_object">Object to evaluate.</param>
        /// <returns>True if object is contained within tree or children.</returns>
        public override bool Contains(T _object)
        {
            return GetObjectBoundary != null ? boundary.Contains(GetObjectBoundary(_object)) : false;
        }

        /// <summary>Checks if tree, or children, contain provided object.</summary>
        /// <param name="_boundary">Boundar to evaluate.</param>
        /// <param name="_object">Object to evaluate.</param>
        /// <returns>True if object is contained within tree or children.</returns>
        public override bool Contains(Bounds _boundary, T _object)
        {
            return GetObjectBoundary != null ? _boundary.Contains(GetObjectBoundary(_object)) : false;
        }

        /// <summary>Evaluates if object intersects with this tree's boundary.</summary>
        /// <param name="_object">Object to evaluate.</param>
        /// <returns>True if object intersects with tree's boundary</returns>
        public override bool Intersects(T _object)
        {
            return GetObjectBoundary != null ? VBounds.Intersects(boundary, GetObjectBoundary(_object)) : false;
        }

        /// <summary>Evaluates if 2 boundaries intersect.</summary>
        /// <param name="a">Boundary A.</param>
        /// <param name="b">Boundary B.</param>
        /// <returns>True if object intersects with tree's boundary</returns>
        public override bool Intersects(Bounds a, Bounds b)
        {
            return VBounds.Intersects(a, b);
        }

        /// <summary>Draws Gizmos [use on either OnDrawGizmos or OnDrawGizmosSelected].</summary>
        public override void DrawGizmos()
        {
            VGizmos.DrawBounds(boundary);

            if(objects != null)
            {
                foreach (T obj in objects)
                {
                    Bounds boundaries = GetObjectBoundary(obj);

                    if(GetGizmosParemeters != null)
                    {
                        GizmosDrawParameters parameters = GetGizmosParemeters(obj);
                        Gizmos.color = parameters.color;
                        switch (parameters.drawMode)
                        {
                            case GizmosDrawMode.Wired:
                                VGizmos.DrawBounds(boundaries);
                            break;

                            case GizmosDrawMode.Solid:
                                Gizmos.DrawCube(boundaries.center, boundaries.extents);
                            break;
                        }
                    }

                    if(boundaries.size.sqrMagnitude > 0.1f) VGizmos.DrawBounds(boundaries);
                    else Gizmos.DrawSphere(boundaries.center, 0.05f);
                }
            }

            if(children != null) foreach (OctaTree<T> child in children)
            {
                if(child != null) child.DrawGizmos();
            }
        }
    }
}