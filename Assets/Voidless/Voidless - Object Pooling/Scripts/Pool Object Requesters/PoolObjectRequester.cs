using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

/*===========================================================================
**
** Class:  PoolObjectRequester<T>
**
** Purpose: This generic class requests a Pool-Object when it is within the
** view of the camera. It is a way to do occlusion culling with dynamic
** objects and to save memory allocation by recycling objects of the same 
** type.
**
**
** Author: Lîf Gwaethrakindo
**
===========================================================================*/
namespace Voidless
{
    [Flags]
    public enum RequesterStates
    {
        None = 0,
        RequesterSeen = 1,
        PoolObjectSeen = 2,
        PoolObjectWithinRadius = 4
    }

    public abstract class PoolObjectRequester<T> : PoolGameObject, IPoolObjectRequester<T> where T : MonoBehaviour, IPoolObject
    {
        [SerializeField] private Camera _camera;
        [Space(5f)]
        [SerializeField] private T _requestedPoolObject;
        [Space(5f)]
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Bounds _bounds;
        [Space(5f)]
        [SerializeField] private bool _evaluateOcclusionOnRequester;
        [SerializeField] private bool _evaluateOcclusionOnPoolObject;
        [SerializeField] private LayerMask _requesterOcclusionMask;
        [SerializeField] private LayerMask _poolObjectOcclusionMask;
        [Space(5f)]
        [Header("Deactivation Criteria's Attributes:")]
        [SerializeField] private float _invisibleTolerance;
        [SerializeField] private float _maxDistance;
        private T _poolObject;
        private Quaternion _rotation;
        private Vector3 _position;
        private Vector3 _scale;
        private float _invisibleTime;
        private RequesterStates _states;

        #region Getters/Setters:
        /// <summary>Gets Requested Pool-Object [this should be the prefab or the template object].</summary>
        public T requestedPoolObject
        {
            get { return _requestedPoolObject; }
            protected set { _requestedPoolObject = value; }
        }

        /// <summary>Gets Pool-Object [this should be the reference the currently requested Pool-Object].</summary>
        public T poolObject
        {
            get { return _poolObject; }
            protected set { _poolObject = value; }
        }

        /// <summary>Gets and Sets spawnPoint property.</summary>
        public Transform spawnPoint
        {
            get { return _spawnPoint; }
            set { _spawnPoint = value; }
        }

        /// <summary>Gets and Sets bounds property.</summary>
        public Bounds bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        /// <summary>Gets and Sets camera property.</summary>
        public new Camera camera
        {
            get
            {
                if (_camera == null) _camera = Camera.main;
                return _camera;
            }
            protected set { _camera = value; }
        }

        /// <summary>Gets and Sets states property.</summary>
        public RequesterStates states
        {
            get { return _states; }
            protected set { _states = value; }
        }

        /// <summary>Gets and Sets evaluateOcclusionOnRequester property.</summary>
        public bool evaluateOcclusionOnRequester
        {
            get { return _evaluateOcclusionOnRequester; }
            set { _evaluateOcclusionOnRequester = value; }
        }

        /// <summary>Gets and Sets evaluateOcclusionOnPoolObject property.</summary>
        public bool evaluateOcclusionOnPoolObject
        {
            get { return _evaluateOcclusionOnPoolObject; }
            set { _evaluateOcclusionOnPoolObject = value; }
        }

        /// <summary>Gets and Sets requesterOcclusionMask property.</summary>
        public LayerMask requesterOcclusionMask
        {
            get { return _requesterOcclusionMask; }
            set { _requesterOcclusionMask = value; }
        }

        /// <summary>Gets and Sets poolObjectOcclusionMask property.</summary>
        public LayerMask poolObjectOcclusionMask
        {
            get { return _poolObjectOcclusionMask; }
            set { _poolObjectOcclusionMask = value; }
        }

        /// <summary>Gets and Sets invisibleTolerance property.</summary>
        public float invisibleTolerance
        {
            get { return _invisibleTolerance; }
            set { _invisibleTolerance = value; }
        }

        /// <summary>Gets and Sets maxDistance property.</summary>
        public float maxDistance
        {
            get { return _maxDistance; }
            set { _maxDistance = value; }
        }

        /// <summary>Gets and Sets invisibleTime property.</summary>
        public float invisibleTime
        {
            get { return _invisibleTime; }
            protected set { _invisibleTime = value; }
        }
        #endregion

        /// <summary>Draws Gizmos on Editor mode when PoolObjectRequester's instance is selected.</summary>
        protected virtual void OnDrawGizmosSelected()
        {
            //DrawGizmos();
        }

        protected virtual void OnDrawGizmos()
        {
            DrawGizmos();
        }

        protected virtual void DrawGizmos()
        {
            Gizmos.color = VColor.transparentWhite;
            Gizmos.DrawCube(spawnPoint.TransformPoint(bounds.center), bounds.size);

            if (poolObject == null) return;

            Gizmos.DrawCube(poolObject.ReferenceTransform.TransformPoint(bounds.center), bounds.size);
        }

        /// <summary>PoolObjectRequester's instance initialization when loaded [Before scene loads].</summary>
        protected override void Awake()
        {
            base.Awake();
            states = RequesterStates.None;
        }

        /// <summary>Updates PoolObjectRequester's instance at each frame.</summary>
        protected override void Update()
        {
            if (poolObject == null)
            {
                //OnRequesterUnseen();
                OnPoolObjectUnseen();
                return;
            }

            bool requesterSeen = (states | RequesterStates.RequesterSeen) == states;
            bool poolObjectSeen = (states | RequesterStates.PoolObjectSeen) == states;

            if (poolObjectSeen)
            {
                invisibleTime = 0.0f;
            }
            else
            {
                bool outsideRadius = (states | RequesterStates.PoolObjectWithinRadius) != states;
                bool invisibleToleranceEnded = invisibleTolerance > 0.0f ? invisibleTime >= invisibleTolerance : true;

                /* Deactivate if:
					- Far-away enough.
					- Invisible to the camera beyond the tolerance wait.
					- The requester is also ut of sight.
					- The optional/additional deactivation condition is also met (true by default).
				*/
                if (outsideRadius && invisibleToleranceEnded && !requesterSeen && DeactivationCondition()) RequestDeactivation();
                invisibleTime += Time.deltaTime;
            }
        }

        /// \TODO Shit's temporal
        /// <returns>Pool-Object's Position.</returns>
        public virtual Vector3 GetPoolObjectPosition(Vector3 _default = default(Vector3))
        {
            return poolObject != null ? poolObject.ReferenceTransform.position : Vector3.zero;
        }

        /// <returns>Retrieved Pool-Object from pool.</returns>
        public abstract T RequestPoolObject();

        /// <summary>Evaluates if Requester's Pool-Object is far enough from Camera.</summary>
        /// <param name="_camera">Camera's reference.</param>
        public bool IsPoolObjectFarFromCamera(Camera _camera)
        {
            Vector3 direction = GetPoolObjectPosition() - camera.transform.position;
            float distance = maxDistance * maxDistance;
            return maxDistance > 0.0f ? direction.sqrMagnitude > distance : true;
        }

        /// <summary>Evaluates if this requester is seen by the provided Camera.</summary>
        /// <param name="_camera">Camera's reference.</param>
        /// <returns>True if the requester is seen by Camera.</returns>
        public bool IsRequesterSeenByCamera(Camera _camera)
        {
            return IsSeenByCamera(_camera, spawnPoint, evaluateOcclusionOnRequester, requesterOcclusionMask);
        }

        /// <summary>Evaluates if this requester's Pool-Object is seen by the provided Camera.</summary>
        /// <param name="_camera">Camera's reference.</param>
        /// <returns>True if the requester's Pool-Object is seen by Camera.</returns>
        public bool IsPoolObjectSeenByCamera(Camera _camera)
        {
            return poolObject != null ? IsSeenByCamera(_camera, poolObject.ReferenceTransform, evaluateOcclusionOnPoolObject, poolObjectOcclusionMask) : false;
        }

        /// <summary>Evaluates if provided Transform is seen by provided Camera.</summary>
        /// <param name="_camera">Camera's reference.</param>
        /// <param name="_transform">Transform to evaluate.</param>
        /// <param name="_evaluateOcclusion">Evaluate occlusion? true by default.</param>
        /// <param name="_occlusionMask">Occlusion mask [AllLayers by default].</param>
        /// <returns>True if the Transform is seen by Camera.</returns>
        public bool IsSeenByCamera(Camera _camera, Transform _transform, bool _evaluateOcclusion = true, int _occlusionMask = Physics.AllLayers)
        {
            return _camera.PointInsideFrustum(_transform.TransformPoint(bounds.center), bounds.size, _evaluateOcclusion, _occlusionMask);
        }

        /// <summary>Requests Deactivation.</summary>
        protected virtual void RequestDeactivation()
        {
            if (poolObject != null)
            {
                OnBeforePoolObjectDeactivation();
                poolObject.OnObjectDeactivation();
            }
            poolObject = null;
            states = RequesterStates.None;
        }

        /// <summary>Condition Evaluation, this must be true in order to be able to request.</summary>
        /// <returns>Condition's Evaluation, returns true by default if this is not overriden.</returns>
        public virtual bool RequestCondition() { return true; }

        /// <summary>After target object is invisible to camera, this additional evaluation occurs to determine whether to deactivate the Pool-Object.</summary>
        /// <returns>Deactivation Condition, returns true by default if this is not overriden.</returns>
        protected virtual bool DeactivationCondition() { return true; }

        #region Callbacks:
        /// <summary>Callback invoked when the Requester is seen by the camera.</summary>
        public virtual void OnRequesterSeen()
        {
            if ((states | RequesterStates.RequesterSeen) == states) return;

            states |= RequesterStates.RequesterSeen;

            if (poolObject != null || !RequestCondition()) return;

            poolObject = RequestPoolObject();
            if (poolObject != null)
            {
                OnPoolObjectRetrieved();
                OnPoolObjectUnseen();
            }
        }

        /// <summary>Callback invoked when the Requester is seen by the camera.</summary>
        public virtual void OnRequesterUnseen()
        {
            states &= ~RequesterStates.RequesterSeen;
        }

        /// <summary>Callback invoked when the Pool-Object is seen by the camera.</summary>
        public virtual void OnPoolObjectSeen()
        {
            states |= RequesterStates.PoolObjectSeen;
        }

        /// <summary>Callback invoked when the Pool-Object is seen by the camera.</summary>
        public virtual void OnPoolObjectUnseen()
        {
            states &= ~RequesterStates.PoolObjectSeen;
        }

        /// <summary>Callback invoked when the Pool-Object is outside the threshold radius.</summary>
        public virtual void OnPoolObjectWithinRadius()
        {
            states |= RequesterStates.PoolObjectWithinRadius;
        }

        /// <summary>Callback invoked when the Pool-Object is outside the threshold radius.</summary>
        public virtual void OnPoolObjectFar()
        {
            states &= ~RequesterStates.PoolObjectWithinRadius;
        }

        /// <summary>Callback internally invoked after Pool-Object was successfully retrieved.</summary>
        /// \TODO In the future, add a result argument [if the retrieval was or not successful, additinal info, etc.]
        protected virtual void OnPoolObjectRetrieved() { /*...*/ }

        /// <summary>Callback internally invoked before deactivating Pool-Object.</summary>
        protected virtual void OnBeforePoolObjectDeactivation() { /*...*/ }
        #endregion
    }
}