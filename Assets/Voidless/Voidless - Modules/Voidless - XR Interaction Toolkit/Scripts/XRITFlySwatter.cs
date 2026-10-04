using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Voidless.XRIT
{
    public class XRITFlySwatter : MonoBehaviour
    {
        [SerializeField] private XRGrabInteractable _grabInteractable;
        [SerializeField] private Collider _hitCollider;
        [SerializeField] private TrailRenderer _swingRenderer;
        [SerializeField][Range(0.0f, 1.0f)] private float _forceScalarThreshold;
        [SerializeField] private float _minVelocityMagnitude;
        [SerializeField] private float _maxVelocityMagnitude;
        [SerializeField] private float _maxSwingForce;
        private Vector3 _previousInteractorPosition;
        private Vector3 _interactorVelocity;
        private Vector3 _backFacePoint;
        private Vector3 _frontFacePoint;
        private Rigidbody _rigidbody;

        /// <summary>Gets grabInteractable property.</summary>
        public XRGrabInteractable grabInteractable { get { return _grabInteractable; } }

        public Collider hitCollider { get { return _hitCollider; } }

        public TrailRenderer swingRenderer { get { return _swingRenderer; } }

        /// <summary>Gets forceScalarThreshold property.</summary>
        public float forceScalarThreshold { get { return _forceScalarThreshold; } }

        public float minVelocityMagnitude { get { return _minVelocityMagnitude; } }

        public float maxVelocityMagnitude { get { return _maxVelocityMagnitude; } }

        public float maxSwingForce { get { return _maxSwingForce; } }

        /// <summary>Gets and Sets previousInteractorPosition property.</summary>
        public Vector3 previousInteractorPosition
        {
            get { return _previousInteractorPosition; }
            set { _previousInteractorPosition = value; }
        }

        /// <summary>Gets and Sets interactorVelocity property.</summary>
        public Vector3 interactorVelocity
        {
            get { return _interactorVelocity; }
            set { _interactorVelocity = value; }
        }

        /// <summary>Gets and Sets backFacePoint property.</summary>
        public Vector3 backFacePoint
        {
            get { return _backFacePoint; }
            set { _backFacePoint = value; }
        }

        /// <summary>Gets and Sets frontFacePoint property.</summary>
        public Vector3 frontFacePoint
        {
            get { return _frontFacePoint; }
            set { _frontFacePoint = value; }
        }

        public Rigidbody rigidbody
        {
            get { return _rigidbody; }
            set { _rigidbody = value; }
        }

        private void Awake()
        {
            if(grabInteractable == null) return;

            swingRenderer.gameObject.SetActive(false);
            grabInteractable.selectEntered.AddListener(OnSelectEnter);
            grabInteractable.selectExited.AddListener(OnSelectExit);
            if(!grabInteractable.TryGetComponent<Rigidbody>(out _rigidbody)) TryGetComponent<Rigidbody>(out _rigidbody);
        }

        private void TryApplyForce(Collision _collision, float _forceScalar)
        {
            Debug.Log("[XRITFlySwatter] Trying to apply a force scalar of: " + _forceScalar);
            //if (_forceScalar < 0.6f) return;
            PoolGameObject poolObject = _collision.collider.GetComponentInParent<PoolGameObject>();
            if (poolObject == null) return;
            poolObject.OnObjectDeactivation();
        }

        /// <summary>Callback invoked when the Hit Collider enters collision.</summary>
        /// <param name="_collision">Collision Data</param>
        /// <param name="_eventType">Type of event</param>
        /// <param name="_ID">ID of the HitCollider</param>
        private void OnHitColliderEvent(Collision _collision, HitColliderEventTypes _eventType, int _ID)
        {
            Debug.Log("[XRITFlySwatter] Collision event of type " + _eventType.ToString());

            switch (_eventType)
            {
                case HitColliderEventTypes.Enter:
                    Vector3 normalA = hitCollider.transform.forward;
                    Vector3 normalB = -normalA;

                    foreach(ContactPoint contact in _collision.contacts)
                    {
                        Vector3 contactNormal = contact.normal;
                        float dotA = Vector3.Dot(contactNormal, normalA);   
                        float dotB = Vector3.Dot(contactNormal, normalB);   
                        float forceScalar = Mathf.Max(Mathf.Abs(dotA), Mathf.Abs(dotB));

                        if(forceScalar > forceScalarThreshold) forceScalar = 1.0f;

                        VisualDebug.DrawSphere(contact.point, 0.05f, VColor.orange);
                        VisualDebug.DrawRay(contact.point, contactNormal * 0.2f, VColor.orange);
                        VisualDebug.DrawRay(hitCollider.transform.position, normalA * 0.2f, Color.blue);
                        VisualDebug.DrawRay(hitCollider.transform.position, normalB * 0.2f, Color.cyan);
                        TryApplyForce(_collision, forceScalar);
                    }
                break;
            }
        }

        private void OnSelectEnter(SelectEnterEventArgs a)
        {
            swingRenderer.gameObject.SetActive(true);
        }

        private void OnSelectExit(SelectExitEventArgs a)
        {
            swingRenderer.gameObject.SetActive(false);
        }

        /// <summary>Updates XRFlySwatter's instance at each frame.</summary>
        private void Update()
        {
            //Debug.Log("Velocity: " + GetVelocity() + ", Angular Velocity: " + GetAngularVelocity());
        }

        /// <summary>Event triggered when this Collider/Rigidbody begun having contact with another Collider/Rigidbody.</summary>
        /// <param name="col">The Collision data associated with this collision Event.</param>
        private void OnCollisionEnter(Collision _collision)
        {
            GameObject obj = _collision.gameObject;

            //Debug.Log("[XRFlySwatter] Entered collision with " + obj.name + " with a velocity magnitude of: " + GetVelocitySqrMagnitude() + " and a force of: " + GetCurrentSwingForce());

            Vector3 normalA = hitCollider.transform.forward;
            Vector3 normalB = -normalA;

            foreach (ContactPoint contact in _collision.contacts)
            {
                if(contact.thisCollider != hitCollider) continue;

                Vector3 contactNormal = contact.normal;
                float dotA = Vector3.Dot(contactNormal, normalA);
                float dotB = Vector3.Dot(contactNormal, normalB);
                float forceScalar = Mathf.Max(Mathf.Abs(dotA), Mathf.Abs(dotB));

                if (forceScalar > forceScalarThreshold) forceScalar = 1.0f;

                VisualDebug.DrawSphere(contact.point, 0.05f, VColor.orange);
                VisualDebug.DrawRay(contact.point, contactNormal * 0.2f, VColor.orange);
                VisualDebug.DrawRay(hitCollider.transform.position, normalA * 0.2f, Color.blue);
                VisualDebug.DrawRay(hitCollider.transform.position, normalB * 0.2f, Color.cyan);
                TryApplyForce(_collision, forceScalar);
            }
        }

        public Vector3 GetVelocity()
        {
            return rigidbody != null ? rigidbody.velocity : Vector3.zero;
        }

        public Vector3 GetAngularVelocity()
        {
            return rigidbody != null ? rigidbody.angularVelocity: Vector3.zero;
        }

        public float GetVelocitySqrMagnitude()
        {
            return GetVelocity().sqrMagnitude;
        }

        public float GetCurrentSwingForce()
        {
            float minSqr = minVelocityMagnitude * minVelocityMagnitude;
            float maxSqr = maxVelocityMagnitude * maxVelocityMagnitude;
            float x = Mathf.Min(GetVelocitySqrMagnitude(), maxSqr);
            float t = RemapValueToNormalizedRange(x, minSqr, maxSqr);

            return t * maxSwingForce;
        }

        /// <summary>Remaps given input from map into normalized range.</summary>
        /// <param name="_input">Input value to remap.</param>
        /// <param name="_map">Original values mapping.</param>
        /// <returns>Input mapped into normalizedRange.</returns>
        public static float RemapValueToNormalizedRange(float _input, float _mapMin, float _mapMax)
        {
            return ((_input - _mapMin) / (_mapMax - _mapMin));
        }
    }
}