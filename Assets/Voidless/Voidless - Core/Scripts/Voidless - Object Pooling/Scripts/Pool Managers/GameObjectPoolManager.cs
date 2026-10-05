using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

/*===========================================================================
**
** Class:  GameObjectPoolManager
**
** Purpose: Singleton class that can contain many GameObject Pools.
** PoolObjectRequesters get their Pool-Objects from this class, so make sure
** both the manager and requesters reference the same type of Pool-Object.
**
**
** Author: Lîf Gwaethrakindo
**
===========================================================================*/
namespace Voidless
{
    public class GameObjectPoolManager : MonoBehaviour
    {
        private static GameObjectPoolManager _Instance;

        [SerializeField] private Camera _camera;
        [SerializeField] private GameObjectPool<PoolGameObject>[] _pools;
        [SerializeField] private float _evaluationRadius;
        [SerializeField] private bool _distancePriority;
        private Dictionary<int, GameObjectPool<PoolGameObject>> _poolsMap;
        private Dictionary<int, HashSet<PoolObjectRequester<PoolGameObject>>> _poolRequesters;

        /// <summary>Gets and Sets camera property.</summary>
        public new Camera camera
        {
            get
            {
                if(_camera == null) _camera = Camera.main;
                return _camera;
            }
            protected set { _camera = value; }
        }

        /// <summary>Gets and Sets Instance property.</summary>
        public static GameObjectPoolManager Instance
        {
            get { return _Instance; }
            private set { _Instance = value; }
        }

        /// <summary>Gets and Sets pools property.</summary>
        public GameObjectPool<PoolGameObject>[] Pools
        {
            get { return _pools; }
            set { _pools = value; }
        }

        /// <summary>Gets and Sets poolsMap property.</summary>
        public Dictionary<int, GameObjectPool<PoolGameObject>> PoolsMap
        {
            get { return _poolsMap; }
            set { _poolsMap = value; }
        }

        /// <summary>Gets and Sets poolRequesters property.</summary>
        public Dictionary<int, HashSet<PoolObjectRequester<PoolGameObject>>> PoolRequesters
        {
            get { return _poolRequesters; }
            set { _poolRequesters = value; }
        }

        /// <summary>Gets and Sets evaluationRadius property.</summary>
        public float EvaluationRadius
        {
            get { return _evaluationRadius; }
            set { _evaluationRadius = value; }
        }

        /// <summary>Gets and Sets distancePriority property.</summary>
        public bool DistancePriority
        {
            get { return _distancePriority; }
            set { _distancePriority = value; }
        }

        /// <summary>Draws Gizmos when this GameObject is selected.</summary>
        private void OnDrawGizmosSelected()
        {
            if(!Application.isPlaying) return;
            Gizmos.color = VColor.transparentWhite;
            Gizmos.DrawSphere(camera.transform.position, EvaluationRadius);
        }

        /// <summary>GameObjectPoolManager's instance initialization when loaded [Before scene loads].</summary>
        private void Awake()
        {
            /// \TODO Import Singleton<T> class and make this MonoBehaviour inherit from it.
            if(Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            PoolsMap = new Dictionary<int, GameObjectPool<PoolGameObject>>();
            PoolRequesters = new Dictionary<int, HashSet<PoolObjectRequester<PoolGameObject>>>();

            foreach(GameObjectPool<PoolGameObject> pool in Pools)
            {
                /// Don't create a pool if there isn't a reference object.
                if(pool.referenceObject == null) continue;

                pool.Initialize();
                PoolsMap.Add(pool.referenceObject.GetInstanceID(), pool);
            }
        }

        /// <summary>Updates GameObjectPoolManager's instance at the end of each frame.</summary>
        private void LateUpdate()
        {
            EvaluatePoolObjectRequesters();
        }

        /// <summary>Evaluates Pool-Object Requesters registered on this Manager's Dictionary.</summary>
        private void EvaluatePoolObjectRequesters()
        {
            if(PoolRequesters == null || PoolRequesters.Count == 0) return;

            foreach(KeyValuePair<int, HashSet<PoolObjectRequester<PoolGameObject>>> requesterPair in PoolRequesters)
            {
                IEnumerable<PoolObjectRequester<PoolGameObject>> requesters = requesterPair.Value;
                int limit = PoolsMap[requesterPair.Key].limit;
                int count = 0;

                if(DistancePriority) requesters = requesters.OrderBy(RequesterDistanceEvaluation);

                foreach(PoolObjectRequester<PoolGameObject> requester in requesters)
                {
                    if (!requester.isActiveAndEnabled) continue;

                    EvaluatePoolObjectRequester(requester, count < limit);
                    count++;
                }
            }
        }

        /// <summary>Evaluates Pool-Objec Requester.</summary>
        /// <param name="requester">Requester to evaluate.</param>
        private void EvaluatePoolObjectRequester(PoolObjectRequester<PoolGameObject> requester, bool slotsAvailable)
        {
            switch(slotsAvailable)
            {
                case true:
                    /// Evaluate if Requester is seen:
                    if(WithinRadius(requester.spawnPoint) || requester.IsRequesterSeenByCamera(camera))
                    {
                        requester.OnRequesterSeen();
#if UNITY_EDITOR
                        Debug.DrawLine(camera.transform.position, requester.spawnPoint.transform.position);
#endif
                    }
                    else requester.OnRequesterUnseen();

                    /// Evaluate if Pool-Object is seen:
                    if(requester.poolObject != null && (WithinRadius(requester.poolObject.ReferenceTransform) || requester.IsPoolObjectSeenByCamera(camera)))
                    {
                        requester.OnPoolObjectSeen();
#if UNITY_EDITOR
                    
                        Debug.DrawLine(camera.transform.position, requester.GetPoolObjectPosition());
#endif
                    }
                    else requester.OnPoolObjectUnseen();

                    /// Evaluate if Pool-Object is within distance radius:
                    if(!requester.IsPoolObjectFarFromCamera(camera)) requester.OnPoolObjectWithinRadius();
                    else requester.OnPoolObjectFar();
                break;

                case false:
                    /// Mark as unseen because due to limit reached it cannot be evaluated.
                    requester.OnRequesterUnseen();
                    requester.OnPoolObjectUnseen();
                    requester.OnPoolObjectFar();
                break;
            }
        }

        /// <summary>Distance-Evaluation predicate [used for LINQ's OrderBy].</summary>
        /// <param name="_requester">Pool-Object Requester to evaluate.</param>
        private float RequesterDistanceEvaluation(PoolObjectRequester<PoolGameObject> _requester)
        {
            Vector3 direction = _requester.spawnPoint.transform.position - camera.transform.position;
            return direction.sqrMagnitude;
        }

        /// <summary>Evaluates if Requester is within radius.</summary>
        /// <param name="_requester">Requester to evaluate</param>
        /// <returns>True if Requester is within evaluation radius</returns>
        private bool WithinRadius(Transform _requester)
        {
            return Vector3.SqrMagnitude(_requester.position - camera.transform.position) < EvaluationRadius * EvaluationRadius;
        }

        /// <summary>Tries to retrieve pool from manager.</summary>
        /// <param name="_reference">Reference Pool-Object [this object's Instance ID ought to be the key in the Dictionary].</param>
        public static GameObjectPool<PoolGameObject> GetPool(PoolGameObject _reference)
        {
            if(_reference == null) return null;

            int ID = _reference.GetInstanceID();
            GameObjectPool<PoolGameObject> pool = null;

            Instance.PoolsMap.TryGetValue(ID, out pool);

            //if(pool != null) Debug.Log("[GameObjectPoolManager] Current Pool's State: " + pool.ToString());
            return pool;
        }

        /// <summary>Adds PoolObjectRequester to this Manager.</summary>
        /// <param name="_requester">Requester to Add.</param>
        public static void AddRequester(PoolObjectRequester<PoolGameObject> _requester)
        {
            Dictionary<int, GameObjectPool<PoolGameObject>> poolDic = Instance.PoolsMap;
            Dictionary<int, HashSet<PoolObjectRequester<PoolGameObject>>> requesterDic = Instance.PoolRequesters;
            int poolObjectID = _requester.requestedPoolObject.GetInstanceID();

            if(!poolDic.ContainsKey(poolObjectID)) poolDic.Add(poolObjectID, new GameObjectPool<PoolGameObject>(_requester.requestedPoolObject));
            if(!requesterDic.ContainsKey(poolObjectID)) requesterDic.Add(poolObjectID, new HashSet<PoolObjectRequester<PoolGameObject>>());

            requesterDic[poolObjectID].Add(_requester);
        }

        /// <summary>Removes PoolObjectRequester to this Manager.</summary>
        /// <param name="_requester">Requester to Remove.</param>
        public static void RemoveRequester(PoolObjectRequester<PoolGameObject> _requester)
        {
            try
            {
                Dictionary<int, HashSet<PoolObjectRequester<PoolGameObject>>> requesterDic = Instance.PoolRequesters;
                int poolObjectID = _requester.requestedPoolObject.GetInstanceID();

                if(!requesterDic.ContainsKey(poolObjectID)) return;

                requesterDic[poolObjectID].Remove(_requester);
            }
            catch(Exception e) { Debug.LogWarning("Naughty little Lif. " + e.Message); }
        }

        /// <summary>Adds Pool-Object to a Pool containing the same reproduction reference.</summary>
        /// <param name="_reference">Reproduction reference.</param>
        /// <param name="_instance">Reproduction's instance to add to pool.</param>
        /// <param name="_enqueue">Enqueue (deactivate it and register it to the vacant slots)? true by default, otherwise it will be activated and put into the occupied slots.</param>
        public static void AddPoolObject(PoolGameObject _reference, PoolGameObject _instance, bool _enqueue = true)
        {
            if(_reference == null) return;

            int ID = _reference.GetInstanceID();
            GameObjectPool<PoolGameObject> pool = null;

            if(Instance.PoolsMap.TryGetValue(ID, out pool)) return;

            pool.Add(_instance, _enqueue);
        }

        public static void EnableAll(PoolGameObject _reference, bool _enable = true)
        {
            if(_reference == null) return;

            try
            {
                Dictionary<int, HashSet<PoolObjectRequester<PoolGameObject>>> requesterDic = Instance.PoolRequesters;
                HashSet<PoolObjectRequester<PoolGameObject>> requesterSet = null;
                int ID = _reference.GetInstanceID();

                if(!requesterDic.TryGetValue(ID, out requesterSet)) return;

                foreach(PoolObjectRequester<PoolGameObject> requester in requesterSet)
                {
                    if (requester == null) continue;

                    requester.gameObject.SetActive(_enable);
                }
            }
            catch (Exception e) { Debug.LogWarning("Naughty little Lif. " + e.Message); }
        }
    }
}