using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    - Subdivide
    - FindNeighbors
    - Insert
    - Remove
    - UpdateObject
 */
namespace Voidless
{
    public interface ISpacePartitioningTree<T, B> : IEnumerable<T>
    {
        ISpacePartitioningTree<T, B> parent { get; set; }

        int index { get; set; }
        B boundary { get; set; }
        bool subdivided { get; set; }

        HashSet<T> objects { get; set; }
        Dictionary<T, List<T>> neighbors { get; set; }
        ISpacePartitioningTree<T, B>[] children { get; set; }
        Func<T, B> GetObjectBoundary { get; set; }
        int maxChildCapacity { get; }

        int maxObjectCapacity { get; }

        /// <summary>Gets Position.</summary>
        Vector3 GetPosition();

        /// <summary>Gets Position.</summary>
        Vector3 GetDimensions();

        /// <summary>Gets Position.</summary>
        Vector2 Get2DPosition();

        /// <summary>Gets Position.</summary>
        Vector2 Get2DDimensions();

        /// <summary>Sets Position.</summary>
        /// <param name="p">Position [as Vector3].</param>
        void SetPosition(Vector3 p);

        /// <summary>Sets Position.</summary>
        /// <param name="d">Dimensions [as Vector3].</param>
        void SetDimensions(Vector3 d);

        /// <summary>Sets Position.</summary>
        /// <param name="p">Position [as Vector2].</param>
        void SetPosition(Vector2 p);

        /// <summary>Sets Position.</summary>
        /// <param name="d">Dimensions [as Vector2].</param>
        void SetDimensions(Vector2 d);

        /// <summary>Gets Position from Object.</summary>
        /// <param name="_object">Object's reference.</param>
        /// <param name="p">Object's position [as Vector3].</param>
        Vector3 GetObjectPosition(T _object);

        /// <summary>Gets Position from Object.</summary>
        /// <param name="_object">Object's reference.</param>
        /// <param name="d">Object's dimensions [as Vector3].</param>
        Vector3 GetObjectDimensions(T _object);

        /// <summary>Gets Position from Object.</summary>
        /// <param name="_object">Object's reference.</param>
        /// <param name="p">Object's position [as Vector2].</param>
        Vector2 GetObject2DPosition(T _object);

        /// <summary>Gets Position from Object.</summary>
        /// <param name="_object">Object's reference.</param>
        /// <param name="d">Object's dimensions [as Vector2].</param>
        Vector2 GetObject2DDimensions(T _object);

        /// <summary>Subdivides Tree.</summary>
        void Subdivide();

        /// <summary>Insertsobject into tree.</summary>
        /// <param name="_object">Object to insert.</param>
        /// <returns>True if object was successfully inserted.</returns>
        bool Insert(T _object);

        /// <summary>Removes object from tree or within children.</summary>
        /// <param name="_object">Object to remove.</param>
        /// <returns>True if object was successfully removed.</returns>
        bool Remove(T _object);

        /// <summary>Evaluates if point is contained within boundaries.</summary>
        /// <param name="p">Point in 2D space.</param>
        /// <returns>True if point is contained within boundaries.</returns>
        bool Contains(Vector2 p);

        /// <summary>Evaluates if point is contained within boundaries.</summary>
        /// <param name="p">Point in 3D space.</param>
        /// <returns>True if point is contained within boundaries.</returns>
        bool Contains(Vector3 p);

        /// <summary>Checks if tree, or children, contain provided object.</summary>
        /// <param name="_object">Object to evaluate.</param>
        /// <returns>True if object is contained within tree or children.</returns>
        bool Contains(T _object);

        /// <summary>Checks if tree, or children, contain provided object.</summary>
        /// <param name="_boundary">Boundar to evaluate.</param>
        /// <param name="_object">Object to evaluate.</param>
        /// <returns>True if object is contained within tree or children.</returns>
        bool Contains(B _boundary, T _object);

        /// <summary>Evaluates if object intersects with this tree's boundary.</summary>
        /// <param name="_object">Object to evaluate.</param>
        /// <returns>True if object intersects with tree's boundary</returns>
        bool Intersects(T _object);

        /// <summary>Evaluates if 2 boundaries intersect.</summary>
        /// <param name="a">Boundary A.</param>
        /// <param name="b">Boundary B.</param>
        /// <returns>True if object intersects with tree's boundary</returns>
        bool Intersects(B a, B b);

        /// <summary>Updates object.</summary>
        /// <param name="_object">Object to update</param>
        void UpdateObject(T _object);

        /// <summary>Retrieves all objects within a specific query range.</summary>
        /// <param name="_range">Range to query.</param>
        /// <param name="_objects">Reference to List to hold found Bounds.</param>
        /// <param name="_resetList">Reset List? True by default.</param>
        /// <returns>List of found Bounds.</returns>
        List<T> Query(B _range, ref List<T> _objects, bool _resetList = true);

        /// <summary>Gets Neighbors from given object.</summary>
        /// <param name="_object">Refrence object.</param>
        /// <param name="_distance">Distance Radius.</param>
        /// <param name="_neighbors">Reference to List of found neighbors.</param>
        List<T> FindNeighbors(T _object, float _distance, ref List<T> _neighbors);

        /// <summary>
        /// Check neighboring quadrants and the parent's other children for neighbors.
        /// </summary>
        /// <param name="_object">The object for which to find neighbors.</param>
        /// <param name="_distance">The distance within which to find neighbors.</param>
        /// <param name="_neighbors">Reference list to store found neighbors.</param>
        void CheckNeighboringQuadrants(T _object, float _distance, ref List<T> _neighbors);

        /// <summary>Gets closest object from given point.</summary>
        /// <param name="p">Point in 2D Space.</param>
        /// <returns>Object closest to point.</returns>
        T GetClosestObject(Vector2 p);

        /// <summary>Gets closest object from given point.</summary>
        /// <param name="p">Point in 3D Space.</param>
        /// <returns>Object closest to point.</returns>
        T GetClosestObject(Vector3 p);
    }
}