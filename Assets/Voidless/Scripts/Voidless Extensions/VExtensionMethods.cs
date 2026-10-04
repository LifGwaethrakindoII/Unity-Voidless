using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

using Object = UnityEngine.Object;

namespace Voidless
{
	public static class VExtensionMethods
	{
		private static Vector3 _CENTER_VIEWPORT_VECTOR_3;
		private static Vector2 _CENTER_VIEWPORT_VECTOR_2;

		public static Vector3 CENTER_VIEWPORT_VECTOR_3
		{
			get
			{
				if(_CENTER_VIEWPORT_VECTOR_3 == default(Vector3)) _CENTER_VIEWPORT_VECTOR_3 = new Vector3(0.5f, 0.5f);
				return _CENTER_VIEWPORT_VECTOR_3;
			}
		}

		public static Vector2 CENTER_VIEWPORT_VECTOR_2
		{
			get
			{
				if(_CENTER_VIEWPORT_VECTOR_2 == default(Vector2)) _CENTER_VIEWPORT_VECTOR_2 = new Vector3(0.5f, 0.5f);
				return _CENTER_VIEWPORT_VECTOR_2;
			}
		}

		/// <returns>Current's Frame-Rate [as integer].</returns>
		public static int GetFrameRate()
		{
			return (int)(1.0f / Time.smoothDeltaTime);
		}

		/// <summary>Creates a ValueTuple from a Tuple [only if both generic types are value types].</summary>
		/// <param name="_tuple">Tuple to retreive values from.</param>
		/// <returns>ValueTuple from Tuple's values.</returns>
		public static ValueVTuple<A, B> ToValueTuple<A, B>(this VTuple<A, B> _tuple) where A : struct where B : struct
		{
			return new ValueVTuple<A, B>(_tuple.Item1, _tuple.Item2);
		} 

		/// <summary>Sets Active collection of GameObjects to the desired bool.</summary>
		/// <param name="_active">Determines whether all GameObjects will be or not active.</param>
		public static void SetAllActive(this List<GameObject> _gameObjects, bool _active)
		{
			for(int i = 0; i < _gameObjects.Count; i++)
			{
				_gameObjects[i].SetActive(_active);
			}
		}

		/// <summary>Sets Active collection of GameObjects to the desired bool.</summary>
		/// <param name="_mono">List of MonoBehaviours.</param>
		/// <param name="_active">Determines whether all GameObjects will be or not active.</param>
		public static void SetAllActive<C>(this List<C> _list, bool _active) where C : Component
		{
			for(int i = 0; i < _list.Count; i++)
			{
				_list[i].gameObject.SetActive(_active);
			}
		}

		///\ TODO: Adapt this function to be a library function (with parameters overload) \\\
		/// <summary>Gets the second minium value of a float list.</summary>
		/// <param name="_list">The list of floats from where the second least value will be given.</param>
		/// <returns>Second minimum value of the list.</returns>
		public static float GetSecondMinimum(this List<float> _list)
		{
			//So they enter by default on minimum range.
			float least = Mathf.Infinity;
			float secondLeast = Mathf.Infinity;

			foreach(float number in  _list)
			{
				if(number <= least) //If current number is lower than the least value, then the prior least value passes as the secondLeast, and the least updates.
				{
					secondLeast = least;
					least = number;
				}
				else if(number < secondLeast) //If at least the current number is lower than the current second, update the second.
				secondLeast = number;
			}

			return secondLeast;
		}

		/// <summary>Dispatches Class.</summary>
		/// <param name="_class">Class to dispatch.</param>
		public static void Dispatch<T>(ref T _class) where T : class
		{
			if(_class != null)
			_class = null;
		}

		/// <summary>Checks if the Transform is being visible on viewport.</summary>
		/// <param name="_transform">The Transform that will check if it's on viewport.</param>
		/// <returns>Transform being seen on viewport (bool).</returns>
		public static bool IsVisibleToCamera(this Transform _transform)
		{
			Vector3 transformView = Camera.main.WorldToViewportPoint(_transform.position);

			return (transformView.x > 0.0f && transformView.x < 1.0f && transformView.y > 0.0f && transformView.y < 1.0f);
		}

		/// <summary>Checks if the GameObject is being visible on viewport.</summary>
		/// <param name="_gameObject">The GameObject that will check if it's on viewport.</param>
		/// <returns>GameObject being seen on viewport (bool).</returns>
		public static bool IsVisibleToCamera(this GameObject _gameObject)
		{
			return _gameObject.transform.IsVisibleToCamera();
		}

		/// <summary>Gets If GameObject has Component attached.</summary>
		/// <param name="_object">The GameObject that will check if has T Componentn attached.</param>
		/// <returns>GameObject has Component T (bool).</returns>
		public static bool Has<T>(this GameObject _object) where T : UnityEngine.Object
		{
			return (_object.GetComponent<T>() != null);
		}

		public static void DestroyOnEditor(this GameObject _object)
		{
			UnityEngine.Object.DestroyImmediate(_object);
		}

		/// <summary>Updates Viewport Plane.</summary>
		/// <param name="_transform">Transform reference.</param>
		/// <param name="_viewportPlane">Viewport Plane to update.</param>
		/// <param name="_fov">Field of View.</param>
		/// <param name="_distance">Distance from the transform.</param>
		/// <param name="_aspect">Aspect Ratio.</param>
		public static void UpdateViewportPlane(this Transform _transform, ref CameraViewportPlane _viewportPlane, float _fov, float _distance, float _aspect)
		{
			float z = _distance;
			float x = (Mathf.Tan(_fov * 0.5f) * z);
			float y = (x / _aspect);

			_viewportPlane.centerPoint = (_transform.position + (_transform.forward * _distance));
			_viewportPlane.topLeftPoint = _viewportPlane.centerPoint + new Vector3(-x, y, 0f);
			_viewportPlane.topRightPoint = _viewportPlane.centerPoint + new Vector3(x, y, 0f);
			_viewportPlane.bottomLeftPoint = _viewportPlane.centerPoint + new Vector3(-x, -y, 0f);
			_viewportPlane.bottomRightPoint = _viewportPlane.centerPoint + new Vector3(x, -y, 0f);
		}

		/// <summary>Updates Camera's Viewport Plane.</summary>
		/// <param name="_camera">Camera's reference.</param>
		/// <param name="_viewportPlane">Viewport Plane to update.</param>
		/// <returns>Updated Viewport Plane.</returns>
		public static CameraViewportPlane UpdateViewportPlane(this Camera _camera, ref CameraViewportPlane _viewportPlane)
		{
			float z = _camera.nearClipPlane;
			float x = (Mathf.Tan(_camera.fieldOfView * 0.5f) * z);
			float y = (x / _camera.aspect);

			_viewportPlane.centerPoint = (_camera.transform.position + (_camera.transform.forward * z));
			_viewportPlane.topLeftPoint = _viewportPlane.centerPoint + new Vector3(-x, y, 0f);
			_viewportPlane.topRightPoint = _viewportPlane.centerPoint + new Vector3(x, y, 0f);
			_viewportPlane.bottomLeftPoint = _viewportPlane.centerPoint + new Vector3(-x, -y, 0f);
			_viewportPlane.bottomRightPoint = _viewportPlane.centerPoint + new Vector3(x, -y, 0f);
		
			return _viewportPlane;
		}

		/// <summary>Sets Component's GameObject Active depending on the bool provided.</summary>
		/// <param name="_active">Set GameObject as Active?.</param>
		public static void SetActive(this Component _component, bool _active)
		{
			_component.gameObject.SetActive(_active);
		}

		/// <summary>Checks if a GameObject is in a LayerMask</summary>
		/// <param name="_object">GameObject to test</param>
		/// <param name="_layerMask">LayerMask with all the layers to test against</param>
		/// <returns>True if in any of the layers in the LayerMask</returns>
		public static bool IsInLayerMask(this GameObject _object, LayerMask _layerMask)
		{
			// Convert the object's layer to a bitfield for comparison
			int objLayerMask = (1 << _object.layer);
			return ((_layerMask.value & objLayerMask) > 0);  // Extra round brackets required!
		}

		/// <summary>Checks if point is between viewport's threshold.</summary>
		/// <param name="_point">Point to evaluate.</param>
		/// <param name="_threshold">Threshold's limit.</param>
		/// <param name="_camera">Camera to calculate the viewport point [null as default, if null, it will get the main camera].</param>
		/// <param name="_calculateViewportPoint">Calculate viewport point? false by default.</param>
		public static bool BetweenViewportPoint(this Vector3 _point, Vector3 _threshold, Camera _camera = null, bool _calculateViewportPoint = false)
		{
			_threshold = _threshold.Clamped(0.0f, 0.5f) * 0.5f;
			if(_calculateViewportPoint)
			{
				if(_camera == null) _camera = Camera.main;
				_point = _camera.WorldToViewportPoint(_point);
			}

			return ((_point.x >= (CENTER_VIEWPORT_VECTOR_3.x - _threshold.x))
					&& (_point.x <= (CENTER_VIEWPORT_VECTOR_3.x + _threshold.x))
					&& (_point.y >=  (CENTER_VIEWPORT_VECTOR_3.y - _threshold.y))
					&& (_point.y <= (CENTER_VIEWPORT_VECTOR_3.y + _threshold.y)));
		}

		/// <summary>Checks if point is between viewport's threshold.</summary>
		/// <param name="_point">Point to evaluate.</param>
		/// <param name="_threshold">Threshold's limit.</param>
		/// <param name="_camera">Camera to calculate the viewport point [null as default, if null, it will get the main camera].</param>
		/// <param name="_calculateViewportPoint">Calculate viewport point? false by default.</param>
		public static bool BetweenViewportPoint(this Vector2 _point, Vector2 _threshold, Camera _camera = null, bool _calculateViewportPoint = false)
		{
			_threshold = _threshold.Clamped(0.0f, 0.5f) * 0.5f;
			if(_calculateViewportPoint)
			{
				if(_camera == null) _camera = Camera.main;
				_point = _camera.WorldToViewportPoint(_point);
			}

			return ((_point.x >= (CENTER_VIEWPORT_VECTOR_2.x - _threshold.x))
					&& (_point.x <= (CENTER_VIEWPORT_VECTOR_2.x + _threshold.x))
					&& (_point.y >=  (CENTER_VIEWPORT_VECTOR_2.y - _threshold.y))
					&& (_point.y <= (CENTER_VIEWPORT_VECTOR_2.y + _threshold.y)));
		}

		/// <summary>Checks if point is abovr viewport's threshold.</summary>
		/// <param name="_point">Point to evaluate.</param>
		/// <param name="_threshold">Threshold's limit.</param>
		/// <param name="_camera">Camera to calculate the viewport point [null as default, if null, it will get the main camera].</param>
		/// <param name="_calculateViewportPoint">Calculate viewport point? false by default.</param>
		public static bool AboveViewportPoint(this Vector3 _point, Vector3 _threshold, Camera _camera = null, bool _calculateViewportPoint = false)
		{
			_threshold = _threshold.Clamped(0.0f, 0.5f) * 0.5f;
			if(_calculateViewportPoint)
			{
				if(_camera == null) _camera = Camera.main;
				_point = _camera.WorldToViewportPoint(_point);
			}

			return ((_point.x <= (CENTER_VIEWPORT_VECTOR_3.x - _threshold.x))
					|| (_point.x >= (CENTER_VIEWPORT_VECTOR_3.x + _threshold.x))
					|| (_point.y <=  (CENTER_VIEWPORT_VECTOR_3.y - _threshold.y))
					|| (_point.y >= (CENTER_VIEWPORT_VECTOR_3.y + _threshold.y)));
		}

		/// <summary>Checks if point is abovr viewport's threshold.</summary>
		/// <param name="_point">Point to evaluate.</param>
		/// <param name="_threshold">Threshold's limit.</param>
		/// <param name="_camera">Camera to calculate the viewport point [null as default, if null, it will get the main camera].</param>
		/// <param name="_calculateViewportPoint">Calculate viewport point? false by default.</param>
		public static bool AboveViewportPoint(this Vector2 _point, Vector2 _threshold, Camera _camera = null, bool _calculateViewportPoint = false)
		{
			_threshold = _threshold.Clamped(0.0f, 0.5f) * 0.5f;
			if(_calculateViewportPoint)
			{
				if(_camera == null) _camera = Camera.main;
				_point = _camera.WorldToViewportPoint(_point);
			}

			return ((_point.x <= (CENTER_VIEWPORT_VECTOR_2.x - _threshold.x))
					|| (_point.x >= (CENTER_VIEWPORT_VECTOR_2.x + _threshold.x))
					|| (_point.y <=  (CENTER_VIEWPORT_VECTOR_2.y - _threshold.y))
					|| (_point.y >= (CENTER_VIEWPORT_VECTOR_2.y + _threshold.y))); 
		}

		/// <summary>Subscribes Sight Sensor Listener to Sight Sensor's Events.</summary>
		/// <param name="_listener">Sight Sensor's Listener to subscribe.</param>
		/// <param name="_sightSensor">Sight Sensor.</param>
		public static void SubscribeToSightSensorEvents(this ISightSensorListener _listener, SightSensor _sightSensor)
		{
			_sightSensor.onColliderSighted += _listener.OnColliderSighted;
			_sightSensor.onColliderOccluded += _listener.OnColliderOccluded; 
			_sightSensor.onColliderOutOfSight += _listener.OnColliderOutOfSight;
		}

		/// <summary>Unsubscribes Sight Sensor Listener to Sight Sensor's Events.</summary>
		/// <param name="_listener">Sight Sensor's Listener to unsubscribe.</param>
		/// <param name="_sightSensor">Sight Sensor.</param>
		public static void UnsubscribeToSightSensorEvents(this ISightSensorListener _listener, SightSensor _sightSensor)
		{
			_sightSensor.onColliderSighted -= _listener.OnColliderSighted;
			_sightSensor.onColliderOccluded -= _listener.OnColliderOccluded; 
			_sightSensor.onColliderOutOfSight -= _listener.OnColliderOutOfSight;
		}

		/// <returns>Gets Delta Time coefficient from Enum's value.</returns>
		/// <param name="_type">Type of Time's Delta.</param>
		public static float GetDeltaTime(DeltaTimeType _type)
		{
			switch(_type)
			{
				case DeltaTimeType.DeltaTime: return Time.deltaTime;
				case DeltaTimeType.FixedDeltaTime: return Time.fixedDeltaTime;
				case DeltaTimeType.SmoothDeltaTime: return Time.smoothDeltaTime;
				default: return Time.deltaTime;
			}
		}

		/// <returns>All GameObjects on Scene.</returns>
		public static GameObject[] GetAllGameObjectsInScene()
		{
			return UnityEngine.Object.FindObjectsOfType<GameObject>();
		}

		/// <summary>Retreives component from this object or its parents.</summary>
		/// <param name="obj">Main GameObject's reference.</param>
		/// <returns>Retreived component, if existing.</returns>
		public static T GetComponentHereOrInParent<T>(this GameObject obj) where T : class
		{
			T result = null;

			while(obj != null)
			{
				result = obj.GetComponent<T>();

				if(result != null) return result;

				obj = obj.transform.parent.gameObject;
			}

			return null;
		}

        public static readonly int HASH_ANIMATION_EMPTY;

        /// <summary>Static Constructor.</summary>
        static VExtensionMethods()
        {
            HASH_ANIMATION_EMPTY = Animator.StringToHash("Empty");
        }

        /// <summary>Iterates through Transform's parents.</summary>
        /// <param name="transform">Transform reference.</param>
        public static IEnumerable<Transform> IterateThroughParents(this Transform transform)
        {
            Transform parent = transform.parent;

            while (parent != null)
            {
                yield return parent;
                parent = parent.parent;
            }
        }

        #region TryGetComponentFunctions:
        /// <summary>Gets a reference to a component of type T on the same GameObject as the component specified, or any parent of the GameObject by using TryGetComponent.</summary>
        /// <param name="obj">GameObject to start looking the Component from.</param>
        /// <param name="component">Component reference.</param>
        /// <returns>Result of the component retrieval.</returns>
        public static bool TryGetComponentInParent<T>(this Transform transform, out T component)
        {
            Transform current = transform;
            component = default(T);

            while (current != null)
            {
                if (current.TryGetComponent<T>(out component)) return true;
                current = current.parent;
            }

            return false;
        }

        /// <summary>Gets a reference to a component of type T on the same GameObject as the component specified, or any parent of the GameObject by using TryGetComponent.</summary>
        /// <param name="obj">GameObject to start looking the Component from.</param>
        /// <param name="component">Component reference.</param>
        /// <returns>Result of the component retrieval.</returns>
        public static bool TryGetComponentInParent<T>(this GameObject obj, out T component)
        {
            return TryGetComponentInParent<T>(obj.transform, out component);
        }

        /// <summary>Gets a reference to a component of type T on the same GameObject as the component specified, or any parent of the GameObject by using TryGetComponent.</summary>
        /// <param name="obj">GameObject to start looking the Component from.</param>
        /// <param name="component">Component reference.</param>
        /// <returns>Result of the component retrieval.</returns>
        public static bool TryGetComponentInParent<T>(this Component obj, out T component)
        {
            return TryGetComponentInParent<T>(obj.transform, out component);
        }

        /// <summary>Gets a reference to a component of type T on the same GameObject as the component specified, or any child of the GameObject.</summary>
        /// <param name="obj">GameObject to start looking the Component from.</param>
        /// <param name="component">Component reference.</param>
        /// <param name="includeInactive">Also include inactive children? false by default.</param>
        /// <returns>Result of the component retrieval.</returns>
        public static bool TryGetComponentInChildren<T>(this Transform transform, out T component, bool includeInactive = false)
        {
            if (transform.TryGetComponent<T>(out component)) return true;

            /// Depth-First search
            foreach (Transform child in transform)
            {
                if (!includeInactive && !child.gameObject.activeSelf) continue;
                if (child.TryGetComponent<T>(out component)) return true;
            }

            /// If not, iterate through all children's children
            foreach (Transform child in transform)
            {
                if (!includeInactive && !child.gameObject.activeSelf) continue;
                if (child.TryGetComponentInChildren<T>(out component)) return true;
            }

            return false;
        }

        /// <summary>Gets a reference to a component of type T on the same GameObject as the component specified, or any child of the GameObject.</summary>
        /// <param name="obj">GameObject to start looking the Component from.</param>
        /// <param name="component">Component reference.</param>
        /// <param name="includeInactive">Also include inactive children? false by default.</param>
        /// <returns>Result of the component retrieval.</returns>
        public static bool TryGetComponentInChildren<T>(this GameObject obj, out T component, bool includeInactive = false)
        {
            return obj.transform.TryGetComponentInChildren<T>(out component, includeInactive);
        }

        /// <summary>Gets a reference to a component of type T on the same GameObject as the component specified, or any child of the GameObject.</summary>
        /// <param name="obj">GameObject to start looking the Component from.</param>
        /// <param name="component">Component reference.</param>
        /// <param name="includeInactive">Also include inactive children? false by default.</param>
        /// <returns>Result of the component retrieval.</returns>
        public static bool TryGetComponentInChildren<T>(this Component obj, out T component, bool includeInactive = false)
        {
            return obj.transform.TryGetComponentInChildren<T>(out component, includeInactive);
        }
        #endregion

        #region DestroyMethods:
        /// <summary>Marks GameObjects to not be destroyed when their scenes are unloaded.</summary>
        /// <param name="gameObjects">Target GameObjects.</param>
        public static void DontDestroyOnLoad(params GameObject[] gameObjects)
        {
            if (gameObjects == null) return;

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject != null) UnityEngine.Object.DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>Invokes the proper destruction method given the current application state.</summary>
        /// <param name="obj">Object to destroy.</param>
        public static void Destroy(this Object obj)
        {
            switch (Application.isPlaying)
            {
                case true:
                    Object.Destroy(obj);
                    break;

                case false:
                    Object.DestroyImmediate(obj);
                    break;
            }
        }

        /// <summary>Destroys all inactive children [just on the first level of the tree].</summary>
        /// <param name="obj">Parent GameObject.</param>
        public static void DestroyAllInactiveChildren(this GameObject obj)
        {
            foreach (Transform child in obj.transform)
            {
                GameObject childObj = child.gameObject;

                switch (childObj.activeSelf)
                {
                    case false:
                        Destroy(childObj);
                        break;
                }
            }
        }

        /// <summary>Destroys all inactive children recursively.</summary>
        /// <param name="obj">GameObject to begin the recursion with.</param>
        public static void DestroyAllInactiveChildrenRecursive(this GameObject obj)
        {
            foreach (Transform child in obj.transform)
            {
                GameObject childObj = child.gameObject;

                switch (childObj.activeSelf)
                {
                    case true:
                        childObj.DestroyAllInactiveChildrenRecursive();
                        break;

                    case false:
                        Destroy(childObj);
                        break;
                }
            }
        }
        #endregion

        /// <summary>Applies transform properties of Transform B into Transform A.</summary>
        /// <param name="a">Transform A.</param>
        /// <param name="b">Transform B.</param>
        /// <param name="p">Transform Properties to apply [TransformProperties.PositionAndRotation by default].</param>
        public static void CopyTransformProperties(this Transform a, Transform b, TransformProperties p = TransformProperties.PositionAndRotation)
        {
            if (a == null || b == null || p == TransformProperties.None) return;

            if ((p | TransformProperties.Position) == p) a.position = b.position;
            if ((p | TransformProperties.Rotation) == p) a.rotation = b.rotation;
            if ((p | TransformProperties.Scale) == p) a.localScale = b.localScale;
        }

        /// <summary>Plays Sound on AudioSource.</summary>
        /// <param name="_source">AudioSource's reference.</param>
        /// <param name="_clip">Clip to play.</param>
        /// <param name="_loop">Loop? true by default.</param>
        public static void Play(this AudioSource _source, AudioClip _clip, bool _loop = true)
        {
            if (_clip == null) return;

            _source.clip = _clip;
            _source.loop = _loop;
            _source.Play();
        }

        /// <summary>Sets Layers recursively.</summary>
        /// <param name="obj">GameObject's reference.</param>
        /// <param name="oldLayer">Old Layer.</param>
        /// <param name="newLayer">New Layer.</param>
        public static void SetLayerRecursive(this GameObject obj, int oldLayer, int newLayer)
        {
            foreach (Transform child in obj.transform)
            {
                if (child.gameObject.layer == oldLayer)
                    child.gameObject.layer = newLayer;

                child.gameObject.SetLayerRecursive(oldLayer, newLayer);
            }
        }

        /// <summary>Sets Layers recursively.</summary>
        /// <param name="obj">GameObject's reference.</param>
        /// <param name="layer">New Layer.</param>
        public static void SetLayerRecursive(this GameObject obj, int layer)
        {
            obj.SetLayerRecursive(obj.layer, layer);
        }

        /*/// <summary>Evaluates if any of the given Vector's components is NaN.</summary>
        /// <param name="v">Vector to evaluate.</param>
        /// <returns>True if any of the vector components is NaN.</returns>
        public static bool IsNaN(this Vector3 v)
        {
            if(float.IsNaN(v.x)) return true;
            if(float.IsNaN(v.y)) return true;
            if(float.IsNaN(v.z)) return true;
            return false;
        }

        /// <summary>Evaluates if a vector has any NaN component, if it does it returns a given default vector.</summary>
        /// <param name="v">Vector to evaluate.</param>
        /// <param name="defaultVector">Default vector to return if given vector has any NaN component.</param>
        /// <returns>Filtered Vector.</returns>
        public static Vector3 NaNFilter(this Vector3 v, Vector3 defaultVector = default(Vector3))
        {
            return v.IsNaN() ? defaultVector : v;
        }*/

        /// <summary>Gets a rotation where the forward vector would be the vector oriented towards the direction.</summary>
        /// <param name="d">Turn direction.</param>
        /// <param name="up">Upwards vector's reference.</param>
        /// <returns>Rotation for the forward vector to be oriented.</returns>
        public static Quaternion LookRotation(Vector3 d, Vector3 up)
        {
            return Quaternion.LookRotation(d, d.x >= 0.0f ? up : -up);
        }

        /// <param name="_rigidbody">Rigidbody's reference.</param>
        /// <returns>String representing referenced Rigidbody.</returns>
        public static string RigidbodyToString(this Rigidbody _rigidbody)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Rigidbody: \n{");
            builder.Append("\tMass = ");
            builder.AppendLine(_rigidbody.mass.ToString());
            builder.Append("\tDrag = ");
            builder.AppendLine(_rigidbody.drag.ToString());
            builder.Append("\tAngular Drag = ");
            builder.AppendLine(_rigidbody.angularDrag.ToString());
            builder.Append("\tUse Gravity = ");
            builder.AppendLine(_rigidbody.useGravity.ToString());
            builder.Append("\tVelocity = ");
            builder.AppendLine(_rigidbody.velocity.ToString());
            builder.Append("\tAngular Velocity = ");
            builder.AppendLine(_rigidbody.angularVelocity.ToString());
            builder.Append("}");

            return builder.ToString();
        }

        /// <summary>Toggles Canvas.</summary>
        /// <param name="_canvas">Canvas' GameObject.</param>
        /// <param name="_original">Canvas' Original Scale.</param>
        /// <param name="coroutine">Coroutine's reference.</param>
        public static void ToggleCanvas(this MonoBehaviour _monoBehaviour, GameObject _canvas, Vector3 _originalScale, ref Coroutine coroutine, Action onToggleEnds = null)
        {
            if (_canvas == null || coroutine != null) return;

            bool activate = !_canvas.activeSelf;
            Vector3 scale = activate ? _originalScale : Vector3.zero;
            Action OnToggleEnds = () =>
            {
                _canvas.SetActive(activate);
                if (onToggleEnds != null) onToggleEnds();
            };

            if (activate)
            {
                _canvas.SetActive(activate);
                _canvas.transform.localScale = Vector3.zero;
            }

            _monoBehaviour.StartCoroutine(_canvas.transform.Scale(scale, 0.45f, OnToggleEnds, VMath.EaseOutBounce), ref coroutine);
        }

        /// <summary>Toggles Canvas.</summary>
        /// <param name="_canvas">Canvas' GameObject.</param>
        /// <param name="_activate">Enable?.</param>
        /// <param name="_original">Canvas' Original Scale.</param>
        /// <param name="coroutine">Coroutine's reference.</param>
        public static void ToggleCanvas(this MonoBehaviour _monoBehaviour, GameObject _canvas, bool _activate, Vector3 _originalScale, ref Coroutine coroutine, Action onToggleEnds = null)
        {
            if (_canvas == null || coroutine != null) return;

            Vector3 scale = _activate ? _originalScale : Vector3.zero;
            Action OnToggleEnds = () =>
            {
                _canvas.SetActive(_activate);
                if (onToggleEnds != null) onToggleEnds();
            };

            if (_activate)
            {
                _canvas.SetActive(_activate);
                _canvas.transform.localScale = Vector3.zero;
            }

            _monoBehaviour.StartCoroutine(_canvas.transform.Scale(scale, 0.45f, OnToggleEnds, VMath.EaseOutBounce), ref coroutine);
        }

        /*/// <summary>Serializes given item into JSON format to a file located at provided path.</summary>
        /// <param name="_item">Item to serialize.</param>
        /// <param name="_path">Path to serialize the JSON's content.</param>
        /// <param name="_prettyPrint">If true, format the output for readability. If false, format the output for minimum size. Default is false.</param>
        public static void SerializeToJSON<T>(this T _item, string _path, bool _prettyPrint = false)
        {
            string json = JsonUtility.ToJson(_item, _prettyPrint);
            try { File.WriteAllText(_path, json); }
            catch (Exception exception) { Debug.LogWarning("[VExtensions] Catched Exception while trying to serialize to JSON: " + exception.Message); }
        }

        /// <summary>Deserializes JSON content from file located at provided path.</summary>
        /// <param name="_path">Path where the JSON should be located.</param>
        /// <returns>Deserialized item from JSON's content, if such exists.</returns>
        public static T DeserializeFromJSONFromPath<T>(string _path)
        {
            T item = default(T);
            string json = null;

            try
            {
                json = File.ReadAllText(_path);
                item = JsonUtility.FromJson<T>(json);
            }
            catch (Exception exception) { Debug.LogWarning("[VExtensions] Catched Exception while trying to deserialize object of type " + typeof(T) + " : " + exception.Message); }

            return item;
        }*/

        /// <summary>Evaluates if layer is inside LayerMask.</summary>
        /// <param name="layer">Layer to Evaluate.</param>
        /// <param name="mask">Target Layer Mask.</param>
        /// <returns>True if layer is inside mask, false otherwise.</returns>
        public static bool InsideLayerMask(int layer, LayerMask mask)
        {
            return (mask | 1 << layer) == mask;
        }

        /// <summary>Applies SetActive to a group of GameObjects.</summary>
        /// <param name="_enable">Set Active? true by default.</param>
        /// <param name="_obj">GameObjects to apply SetActive to.</param>
        public static void SetActive(bool _enable = true, params GameObject[] _objs)
        {
            if (_objs == null) return;

            foreach (GameObject obj in _objs)
            {
                if (obj != null) obj.SetActive(_enable);
            }
        }

        /// <summary>Evaluates if GameObject's layer is inside LayerMask.</summary>
        /// <param name="layer">GameObject to Evaluate.</param>
        /// <param name="mask">Target Layer Mask.</param>
        /// <returns>True if GameObject's layer is inside mask, false otherwise.</returns>
        public static bool InsideLayerMask(this GameObject obj, LayerMask mask)
        {
            return (mask | 1 << obj.layer) == mask;
        }

        /// <summary>Performs an action to for each element passed. This method is to avoid lazy array initializations.</summary>
        /// <param name="action">Action to perform to each element of array.</param>
        /// <param name="_array">Array of elements of type T.</param>
        public static void ForEach<T>(Action<T> action, params T[] _array)
        {
            if (action == null) return;

            foreach (T element in _array)
            {
                if (element != null) action(element);
            }
        }

        /// <summary>Evaluates if GameObject has any of the provided tags' array.</summary>
        /// <param name="obj">GameObject to evaluate.</param>
        /// <param name="tags">Tags to evaluate.</param>
        /// <returns>True whether GameObject has any of the tags, false otherwise.</returns>
        public static bool HasAnyOfTags(this GameObject obj, params string[] tags)
        {
            if (tags == null) return false;

            foreach (string tag in tags)
            {
                if (obj.CompareTag(tag)) return true;
            }

            return false;
        }

        /// <summary>Makes LineRenderer draw a projectile parabola.</summary>
        /// <param name="_lineRenderer">LineRenderer's reference.</param>
        /// <param name="p">Initial Position.</param>
        /// <param name="v">Initial Velocity.</param>
        /// <param name="g">Gravity that affects the projectile.</param>
        /// <param name="t">Projection's time.</param>
        /// <param name="s">LineRenderer's segments [50 by default].</param>
        public static void DrawParabola(this LineRenderer _lineRenderer, Vector3 p, Vector3 v, Vector3 g, float t, int s = 50)
        {
            if (s < 1) return;

            _lineRenderer.positionCount = s + 1;

            float r = t / (float)s;

            for (int i = 0; i < (s + 1); i++)
            {
                _lineRenderer.SetPosition(i, VPhysics.ProjectileProjection(i * r, v, p, g));
            }
        }

        /// <summary>Makes LineRenderer draw a projectile parabola [with Physics' Gravity].</summary>
        /// <param name="_lineRenderer">LineRenderer's reference.</param>
        /// <param name="p">Initial Position.</param>
        /// <param name="v">Initial Velocity.</param>
        /// <param name="t">Projection's time.</param>
        /// <param name="s">LineRenderer's segments [50 by default].</param>
        public static void DrawParabola(this LineRenderer _lineRenderer, Vector3 p, Vector3 v, float t, int s = 50)
        {
            _lineRenderer.DrawParabola(p, v, Physics.gravity, t, s);
        }

        public static T[] GetComponentsInChildrenExceptItself<T>(this Component _obj)
        {
            int childCount = _obj.transform.childCount;

            if (childCount == 0) return null;

            List<T> components = new List<T>();

            for (int i = 0; i < childCount; i++)
            {
                T[] array = _obj.transform.GetChild(i).GetComponentsInChildren<T>();
                if (array != null && array.Length > 0) components.AddRange(array);
            }

            return components.ToArray();
        }

        public static T[] GetComponentsInChildrenExceptItself<T>(this GameObject _obj)
        {
            int childCount = _obj.transform.childCount;

            if (childCount == 0) return null;

            List<T> components = new List<T>();

            for (int i = 0; i < childCount; i++)
            {
                T[] array = _obj.transform.GetChild(i).GetComponentsInChildren<T>();
                if (array != null && array.Length > 0) components.AddRange(array);
            }

            return components.ToArray();
        }


        #region UnityPrimitiveMethods:
        /// <summary>Sets the scale of this GameObject (asuming it is a Unity's Cube) so it has the desired dimensions.</summary>
        /// <param name="sphere">Cube's GameObject [Must be a Unity's sphere].</param>
        /// <param name="s">Cube's size in 3D-Space (width, height and depth).</param>
        public static void SetUnityCubeDimensions(this GameObject cube, Vector3 s)
        {
            cube.transform.localScale = s;
        }

        /// <summary>Sets the scale of this GameObject (asuming it is a Unity's Cube) so it has the desired dimensions.</summary>
        /// <param name="sphere">Cube's GameObject [Must be a Unity's sphere].</param>
        /// <param name="w">Desired's width.</param>
        /// <param name="h">Desired's height.</param>
        /// <param name="d">Desired's depth.</param>
        public static void SetUnityCubeDimensions(this GameObject cube, float w, float h, float d)
        {
            cube.transform.localScale = new Vector3(w, h, d);
        }

        /// <summary>Sets the scale of this GameObject (asuming it is a Unity's Cube) so it has the desired dimensions.</summary>
        /// <param name="sphere">Cube's GameObject [Must be a Unity's sphere].</param>
        /// <param name="x">Desired's dimension for width, height and depth.</param>
        public static void SetUnityCubeDimensions(this GameObject cube, float x)
        {
            cube.SetUnityCubeDimensions(x, x, x);
        }

        /// <summary>Sets the scale of this GameObject (asuming it is a Unity's Sphere) so it has the desired radius.</summary>
        /// <param name="sphere">Sphere's GameObject [Must be a Unity's sphere].</param>
        /// <param name="r">Desired's radius.</param>
        public static void SetUnitySphereRadius(this GameObject sphere, float r)
        {
            sphere.transform.localScale = (2.0f * r) * Vector3.one;
        }

        /// <summary>Sets the scale of this GameObject (asuming it is a Unity's Sphere) so it has the desired diameter.</summary>
        /// <param name="sphere">Sphere's GameObject [Must be a Unity's Sphere].</param>
        /// <param name="d">Desired's diameter.</param>
        public static void SetUnitySphereDiameter(this GameObject sphere, float d)
        {
            sphere.SetUnitySphereRadius(d * 0.5f);
        }

        /// <summary>Sets the scale of this GameObject (asuming it is a Unity's Capsule) so it has the desired height and radius.</summary>
        /// <param name="capsule">Capsule's GameObject [Must be a Unity's Capsule].</param>
        /// <param name="h">Desired's height.</param>
        /// <param name="r">Desired's radius.</param>
        public static void SetUnityCapsuleDimensions(this GameObject capsule, float h, float r)
        {
            // Calculate the scaling factors for the body and hemispheres.
            float bodyScaleFactor = (h - 2.0f * r) / h;
            float hemisphereScaleFactor = (2.0f * r) / h;

            // Set the local scale of the GameObject to adjust the l and r.
            Vector3 newScale = new Vector3(1.0f, bodyScaleFactor, 1.0f);
            capsule.transform.localScale = newScale;

            // Adjust the local position to align the capsule's center.
            /*Vector3 newPosition = new Vector3(0.0f, (h - r) * 0.5f, 0.0f);
            capsule.transform.localPosition = newPosition;*/
        }

        /// <summary>Sets the scale of this GameObject (asuming it is a Unity's Cylinder) so it has the desired height and radius.</summary>
        /// <param name="cylinder">Cylinder's GameObject [Must be a Unity's Cylinder].</param>
        /// <param name="h">Desired's height.</param>
        /// <param name="r">Desired's radius.</param>
        public static void SetUnityCylinderDimensions(this GameObject cylinder, float h, float r)
        {
            // Calculate the scale factors for h and r.
            float scaleHeight = h / (2 * r);
            Vector3 newScale = new Vector3(r * 2, scaleHeight, r * 2);

            // Set the local scale to adjust the dimensions.
            cylinder.transform.localScale = newScale;
        }

        /// <summary>Sets the attributes of a BoxCollider into this GameObject (asuming it is a Unity's Cube).</summary>
        /// <param name="cube">Cubes GameObject.</param>
        /// <param name="boxCollider">BoxCollider's reference.</param>
        public static void SetUnityCubeAsBoxCollider(this GameObject cube, BoxCollider boxCollider)
        {
            cube.SetUnityCubeDimensions(boxCollider.size);
            //cube.transform.localScale = boxCollider.transform.localScale;
            cube.transform.rotation = boxCollider.transform.rotation;
            cube.transform.position = boxCollider.transform.TransformPoint(boxCollider.center);
        }

        /// <summary>Sets the attributes of a SphereCollider into this GameObject (asuming it is a Unity's Sphere).</summary>
        /// <param name="sphere">Sphere's GameObject.</param>
        /// <param name="sphereCollider">SphereCollider's reference.</param>
        public static void SetUnitySphereAsSphereCollider(this GameObject sphere, SphereCollider sphereCollider)
        {
            sphere.SetUnitySphereRadius(sphereCollider.radius);
            //sphere.transform.localScale = sphereCollider.transform.localScale;
            sphere.transform.rotation = sphereCollider.transform.rotation;
            sphere.transform.position = sphereCollider.transform.TransformPoint(sphereCollider.center);
        }

        /// <summary>Sets the attributes of a CapsuleCollider into this GameObject (asuming it is a Unity's Capsule).</summary>
        /// <param name="capsule">Capsule's GameObject.</param>
        /// <param name="capsuleCollider">CapsuleCollider's reference.</param>
        public static void SetUnityCapsuleAsCapsuleCollider(this GameObject capsule, CapsuleCollider capsuleCollider)
        {
            capsule.SetUnityCapsuleDimensions(capsuleCollider.height, capsuleCollider.radius);
            //capsule.transform.localScale = capsuleCollider.transform.localScale;
            capsule.transform.rotation = capsuleCollider.transform.rotation;
            capsule.transform.position = capsuleCollider.transform.TransformPoint(capsuleCollider.center);
        }

        /// <summary>Sets the attributes of a CapsuleCollider into this GameObject (asuming it is a Unity's Cylinder).</summary>
        /// <param name="cylinder">Cylinders GameObject.</param>
        /// <param name="capsuleCollider">CapsuleCollider's reference.</param>
        public static void SetUnityCylinderAsCapsuleCollider(this GameObject cylinder, CapsuleCollider capsuleCollider)
        {
            cylinder.SetUnityCapsuleDimensions(capsuleCollider.height, capsuleCollider.radius);
            //cylinder.transform.localScale = capsuleCollider.transform.localScale;
            cylinder.transform.rotation = capsuleCollider.transform.rotation;
            cylinder.transform.position = capsuleCollider.transform.TransformPoint(capsuleCollider.center);
        }
        #endregion
    }
}
