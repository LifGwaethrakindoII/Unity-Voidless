using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Voidless;

/*===========================================================================
**
** Class:  VisualDebug
**
** Purpose: This class works as a singleton that allows for visual debugging.
** 
** The class uses Object-Pooling for the instancing of the primitives and
** LineRenderers.
**
** Author: Lîf Gwaethrakindo
**
===========================================================================*/
namespace Voidless
{
	public class VisualDebug : Singleton<VisualDebug>
	{
		public const float DEFAULT_WAIT = 3.0f;

		[Space(5f)]
		[SerializeField] private bool _debug;
		[Space(5f)]
		[Header("Pools:")]
		[SerializeField] private GameObjectPool<PoolLineRenderer> _lineRendererPool;
		[SerializeField] private GameObjectPool<PoolPrimitive> _boxPool;
		[SerializeField] private GameObjectPool<PoolPrimitive> _spherePool;
		[SerializeField] private GameObjectPool<PoolPrimitive> _capsulePool;
		[SerializeField] private GameObjectPool<PoolPrimitive> _cylinderPool;
		[SerializeField] private GameObjectPool<PoolText> _textPool;
        private Dictionary<int, Coroutine> coroutines;

		// Default color for debug line rendering.
		private static readonly Color COLOR_DEFAULT = Color.white;

		/// <summary>Gets and Sets debug property.</summary>
		public static bool debug
		{
			get { return Instance._debug; }
			set { Instance._debug = value; }
		}

		/// <summary>Gets lineRendererPool property.</summary>
		public GameObjectPool<PoolLineRenderer> LineRendererPool { get { return _lineRendererPool; } }

		/// <summary>Gets boxPool property.</summary>
		public GameObjectPool<PoolPrimitive> BoxPool { get { return _boxPool; } }

		/// <summary>Gets spherePool property.</summary>
		public GameObjectPool<PoolPrimitive> SpherePool { get { return _spherePool; } }

		/// <summary>Gets capsulePool property.</summary>
		public GameObjectPool<PoolPrimitive> CapsulePool { get { return _capsulePool; } }

		/// <summary>Gets cylinderPool property.</summary>
		public GameObjectPool<PoolPrimitive> CylinderPool { get { return _cylinderPool; } }

		/// <summary>Gets TextPool property.</summary>
		public GameObjectPool<PoolText> TextPool { get { return _textPool; } }

		/// <summary>Callback called on Awake if this Object is the Singleton's Instance.</summary>
        protected override void OnAwake()
		{
#if UNITY_EDITOR
			LineRendererPool.Initialize();
			BoxPool.Initialize();
			SpherePool.Initialize();
			CapsulePool.Initialize();
			CylinderPool.Initialize();
			TextPool.Initialize();
			coroutines = new Dictionary<int, Coroutine>();
#endif
        }

		/// <summary>Draws Ray.</summary>
		/// <param name="o">Ray's Origin.</param>
		/// <param name="d">Ray's Direction.</param>
		/// <param name="c">Ray's Color [white by default].</param>
		/// <param name="t">Time the Ray will last (DEFAULT_WAIT by default).</param>
		public static void DrawRay(Vector3 o, Vector3 d, Color c = default, float t = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolLineRenderer lineRenderer = RequestLineRenderer(c);

            if (lineRenderer == null) return;

            lineRenderer.SetAsRay(o, d);
			Instance.InvokeWaitCoroutine(lineRenderer , t);
#endif
		}

		/// <summary>Draws Line.</summary>
		/// <param name="a">Line's starting point.</param>
		/// <param name="b">Line's end point.</param>
		/// <param name="c">Line's Color [white by default].</param>
		/// <param name="t">Time the Line will last (DEFAULT_WAIT by default).</param>
		public static void DrawLine(Vector3 a, Vector3 b, Color c = default, float t = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolLineRenderer lineRenderer = RequestLineRenderer(c);

            if (lineRenderer == null) return;

            lineRenderer.SetAsLine(a, b);
			Instance.InvokeWaitCoroutine(lineRenderer , t);
#endif
		}

		/// <summary>Draws Line.</summary>
		/// <param name="a">Line's starting point.</param>
		/// <param name="b">Line's end point.</param>
		/// <param name="c">Line's Color [white by default].</param>
		/// <param name="t">Time the Line will last (DEFAULT_WAIT by default).</param>
		/// <param name="f">Line Function.</param>
        /// <param name="s">Sample count.</param>
		public static void DrawLine(Vector3 a, Vector3 b, Func<Vector3, Vector3, float, Vector3> f = null, int s = PoolLineRenderer.SAMPLES, Color c = default, float t = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolLineRenderer lineRenderer = RequestLineRenderer(c);

			if(lineRenderer == null) return;

			lineRenderer.SetAsLine(a, b, f, s);
			Instance.InvokeWaitCoroutine(lineRenderer , t);
#endif
		}

        /// <summary>Draws Projectile Projection.</summary>
        /// <param name="p0">Initial position.</param>
        /// <param name="pf">Final Position.</param>
        /// <param name="t">Time.</param>
        /// <param name="g">Gravity's force.</param>
        /// <param name="s">Sample count.</param>
        /// <param name="c">LineRenderer's color.</param>
        /// /// <param name="w">Time the Line will last (DEFAULT_WAIT by default).</param>
        public static void DrawProjectileProjection(Vector3 p0, Vector3 pf, float t, Vector3 g, int s = PoolLineRenderer.SAMPLES, Color c = default, float w = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolLineRenderer lineRenderer = RequestLineRenderer(c);

            if (lineRenderer == null) return;

            lineRenderer.SetAsProjectileProjection(p0, pf, t, g);
			Instance.InvokeWaitCoroutine(lineRenderer , w);
#endif
		}

		/// <summary>Draws Debug Sphere.</summary>
		/// <param name="p">Sphere's Position.</param>
		/// <param name="r">Sphere's Radius.</param>
		/// <param name="c">Sphere's Color [white by default].</param>
		/// <param name="t">Time the Sphere will last (DEFAULT_WAIT by default).</param>
		public static void DrawSphere(Vector3 p, float r, Color c = default, float t = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolPrimitive sphere = RequestSpherePrimitive(p, r, c);
			if(sphere != null) Instance.InvokeWaitCoroutine(sphere , t);
#endif
		}

		/// <summary>Draws Debug Box.</summary>
		/// <param name="p">Box's Position.</param>
		/// <param name="r">Box's Radius.</param>
		/// <param name="c">Box's Color [white by default].</param>
		/// <param name="t">Time the Box will last (DEFAULT_WAIT by default).</param>
		public static void DrawBox(Vector3 p, Quaternion r, Vector3 s, Color c = default, float t = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolPrimitive box = RequestBoxPrimitive(p, r, s, c);
			Instance.InvokeWaitCoroutine(box , t);
#endif
		}

		/// <summary>Draws Debug Capsule.</summary>
		/// <param name="p">Capsule's Position.</param>
		/// <param name="r">Capsule's Radius.</param>
		/// <param name="c">Capsule's Color [white by default].</param>
		/// <param name="t">Time the Capsule will last (DEFAULT_WAIT by default).</param>
		public static void DrawCapsule(Vector3 p, Quaternion rotation, float h, float r, Color c = default, float t = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolPrimitive capsule = RequestCapsulePrimitive(p, rotation, h, r, c);
			Instance.InvokeWaitCoroutine(capsule , t);
#endif
		}

		/// <summary>Draws Debug Cylinder.</summary>
		/// <param name="p">Cylinder's Position.</param>
		/// <param name="r">Cylinder's Radius.</param>
		/// <param name="c">Cylinder's Color [white by default].</param>
		/// <param name="t">Time the Cylinder will last (DEFAULT_WAIT by default).</param>
		public static void DrawCylinder(Vector3 p, Quaternion rotation, float h, float r, Color c = default, float t = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolPrimitive cylinder = RequestCylinderPrimitive(p, rotation, h, r, c);
			Instance.InvokeWaitCoroutine(cylinder , t);
#endif
		}

		/// <summary>Draws Debug Text.</summary>
		/// <param name="p">Text's Position.</param>
		/// <param name="r">Text's Rotation.</param>
		/// <param name="_contentPointer">Text's contentPointer [null by default].</param>
		/// <param name="t">Time the Text will last (DEFAULT_WAIT by default).</param>
		public static void DrawText(Vector3 p, Quaternion r, Func<string> _contentPointer = null, Color c = default, float t = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolText text = RequestText(p, r, _contentPointer);
			Instance.InvokeWaitCoroutine(text, t);
#endif
		}

		/// <summary>Draws Debug Text.</summary>
		/// <param name="p">Text's Position.</param>
		/// <param name="r">Text's Rotation.</param>
		/// <param name="_content">Text's Content [empty by default].</param>
		/// <param name="t">Time the Text will last (DEFAULT_WAIT by default).</param>
		public static void DrawText(Vector3 p, Quaternion r, string _content = "", Color c = default, float t = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolText text = RequestText(p, r, _content);
			Instance.InvokeWaitCoroutine(text, t);
#endif
		}

        /// <summary>Draws Debug Box copying the attributes of a BoxCollider.</summary>
        /// <param name="_boxCollider">BoxCollider's reference.</param>
        /// <param name="c">Box's Color [white by default].</param>
        /// <param name="t">Time the Box will last (DEFAULT_WAIT by default).</param>
        public static void DrawBoxCollider(BoxCollider _boxCollider, Color c = default, float t = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolPrimitive box = Instance.BoxPool.Recycle();
			box.SetColor(c);
			CopyBoxFromCollider(box, _boxCollider);
			Instance.InvokeBoxColliderWaitCoroutine(box, _boxCollider, t);
#endif		
		}

		/// <summary>Draws Debug Sphere copying the attributes of a SphereCollider.</summary>
		/// <param name="_sphereCollider">SphereCollider's reference.</param>
		/// <param name="c">Sphere's Color [white by default].</param>
		/// <param name="t">Time the Sphere will last (DEFAULT_WAIT by default).</param>
		public static void DrawSphereCollider(SphereCollider _sphereCollider, Color c = default, float t = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolPrimitive sphere = Instance.SpherePool.Recycle();
			sphere.SetColor(c);
			CopySphereFromCollider(sphere, _sphereCollider);
			Instance.InvokeSphereColliderWaitCoroutine(sphere, _sphereCollider, t);
#endif		
		}

		/// <summary>Draws Debug Capsule copying the attributes of a CapsuleCollider.</summary>
		/// <param name="_capsuleCollider">CapsuleCollider's reference.</param>
		/// <param name="c">Capsule's Color [white by default].</param>
		/// <param name="t">Time the Capsule will last (DEFAULT_WAIT by default).</param>
		public static void DrawCapsuleCollider(CapsuleCollider _capsuleCollider, Color c = default, float t = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolPrimitive capsule = Instance.CapsulePool.Recycle();
			capsule.SetColor(c);
			CopyCapsuleFromCollider(capsule, _capsuleCollider);
			Instance.InvokeCapsuleColliderWaitCoroutine(capsule, _capsuleCollider, t);
#endif		
		}

		/// <summary>Draws Debug Cylinder copying the attributes of a CylinderCollider.</summary>
		/// <param name="_capsuleCollider">CylinderCollider's reference.</param>
		/// <param name="c">Cylinder's Color [white by default].</param>
		/// <param name="t">Time the Cylinder will last (DEFAULT_WAIT by default).</param>
		public static void DrawCylinderCollider(CapsuleCollider _capsuleCollider, Color c = default, float t = DEFAULT_WAIT)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return;

			PoolPrimitive cylinder = Instance.CylinderPool.Recycle();
			cylinder.SetColor(c);
			CopyCylinderFromCollider(cylinder, _capsuleCollider);
			Instance.InvokeCapsuleColliderWaitCoroutine(cylinder, _capsuleCollider, t);
#endif		
		}

		/// <summary>Copies attributes from BoxCollider and applies them into a Box Primitive.</summary>
		/// <param name="box">Box Primitive to set.</param>
		/// <param name="_boxCollider">BoxCollider's reference.</param>
		private static void CopyBoxFromCollider(PoolGameObject box, BoxCollider _boxCollider)
		{
#if UNITY_EDITOR
			Vector3 p = _boxCollider.transform.TransformPoint(_boxCollider.center);
			Quaternion r = _boxCollider.transform.rotation;

			box.gameObject.SetUnityCubeAsBoxCollider(_boxCollider);
			box.transform.position = p;
			box.transform.rotation = r;
#endif
		}

		/// <summary>Copies attributes from SphereCollider and applies them into a Sphere Primitive.</summary>
		/// <param name="sphere">Sphere Primitive to set.</param>
		/// <param name="_sphereCollider">SphereCollider's reference.</param>
		private static void CopySphereFromCollider(PoolGameObject sphere, SphereCollider _sphereCollider)
		{
#if UNITY_EDITOR
			Vector3 p = _sphereCollider.transform.TransformPoint(_sphereCollider.center);
			Quaternion r = _sphereCollider.transform.rotation;

			sphere.gameObject.SetUnitySphereAsSphereCollider(_sphereCollider);
			sphere.transform.position = p;
			sphere.transform.rotation = r;
#endif
		}

		/// <summary>Copies attributes from CapsuleCollider and applies them into a Capsule Primitive.</summary>
		/// <param name="capsule">Capsule Primitive to set.</param>
		/// <param name="_capsuleCollider">CapsuleCollider's reference.</param>
		private static void CopyCapsuleFromCollider(PoolGameObject capsule, CapsuleCollider _capsuleCollider)
		{
#if UNITY_EDITOR
			Vector3 p = _capsuleCollider.transform.TransformPoint(_capsuleCollider.center);
			Quaternion r = _capsuleCollider.transform.rotation;

			capsule.gameObject.SetUnityCapsuleAsCapsuleCollider(_capsuleCollider);
			capsule.transform.position = p;
			capsule.transform.rotation = r;
#endif
		}

		/// <summary>Copies attributes from CapsuleCollider and applies them into a Cylinder Primitive.</summary>
		/// <param name="cylinder">Cylinder Primitive to set.</param>
		/// <param name="_capsuleCollider">CapsuleCollider's reference.</param>
		private static void CopyCylinderFromCollider(PoolGameObject cylinder, CapsuleCollider _capsuleCollider)
		{
#if UNITY_EDITOR
			Vector3 p = _capsuleCollider.transform.TransformPoint(_capsuleCollider.center);
			Quaternion r = _capsuleCollider.transform.rotation;

			cylinder.gameObject.SetUnityCylinderAsCapsuleCollider(_capsuleCollider);
			cylinder.transform.position = p;
			cylinder.transform.rotation = r;
#endif
		}

		/// <summary>Invokes wait coroutine for given Pool-Object.</summary>
		/// <param name="obj">Pool-Object to keep for a fixed period of time.</param>
		/// <param name="t">Time the Pool-Object will last active.</param>
		private void InvokeWaitCoroutine(PoolGameObject obj, float t)
		{
#if UNITY_EDITOR
			if(t <= 0.0f) return;
			
			int ID = obj.GetInstanceID();
			this.StartCoroutine(ID, ref coroutines, Keep(obj, t));
#endif
		}

		/// <summary>Invokes wait coroutine for given Box Pool-Object.</summary>
		/// <param name="obj">Pool-Object to keep for a fixed period of time.</param>
		/// <param name="t">Time the Pool-Object will last active.</param>
		private void InvokeBoxColliderWaitCoroutine(PoolGameObject obj, BoxCollider _boxCollider, float t)
		{
#if UNITY_EDITOR
			if(t <= 0.0f) return;
			
			int ID = obj.GetInstanceID();
			this.StartCoroutine(ID, ref coroutines, KeepAsBoxCollider(obj, _boxCollider, t));
#endif
		}

		/// <summary>Invokes wait coroutine for given Sphere Pool-Object.</summary>
		/// <param name="obj">Pool-Object to keep for a fixed period of time.</param>
		/// <param name="_sphereCollider">SphereCollider's reference.</param>
		/// <param name="t">Time the Pool-Object will last active.</param>
		private void InvokeSphereColliderWaitCoroutine(PoolGameObject obj, SphereCollider _sphereCollider, float t)
		{
#if UNITY_EDITOR
			if(t <= 0.0f) return;
			
			int ID = obj.GetInstanceID();
			this.StartCoroutine(ID, ref coroutines, KeepAsSphereCollider(obj, _sphereCollider, t));
#endif
		}

		/// <summary>Invokes wait coroutine for given Capsule Pool-Object.</summary>
		/// <param name="obj">Pool-Object to keep for a fixed period of time.</param>
		/// <param name="_capsuleCollider">CapsuleCollider's reference.</param>
		/// <param name="t">Time the Pool-Object will last active.</param>
		private void InvokeCapsuleColliderWaitCoroutine(PoolGameObject obj, CapsuleCollider _capsuleCollider, float t)
		{
#if UNITY_EDITOR
			if(t <= 0.0f) return;
			
			int ID = obj.GetInstanceID();
			this.StartCoroutine(ID, ref coroutines, KeepAsCapsuleCollider(obj, _capsuleCollider, t));
#endif
		}

		/// <summary>Requests Pool-LineRenderer from Pool.</summary>
		/// <param name="c">LineRenderer's Color (white by default).</param>
		/// <returns>Pool-LineRenderer.</returns>
		public static PoolLineRenderer RequestLineRenderer(Color c = default)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return null;

			PoolLineRenderer lineRenderer = Instance.LineRendererPool.Recycle(Vector3.zero, Quaternion.identity);

			if(lineRenderer == null) return null;

			lineRenderer.SetColor(c == default ? COLOR_DEFAULT : c);
			return lineRenderer;
#else
			return null;
#endif
		}

		/// <summary>Request Pool-Object Primitive.</summary>
		/// <param name="pool">Pool that contains the desired primitive type.</param>
		/// <param name="p">Primitive's position.</param>
		/// <param name="r">Primitive's rotation.</param>
		/// <param name="c">Primitive's color.</param>
		private static PoolPrimitive RequestPrimitive(GameObjectPool<PoolPrimitive> pool, Vector3 p, Quaternion r, Color c = default)
		{
#if UNITY_EDITOR
			if(Instance == null || pool == null) return null;

			PoolPrimitive primitive = pool.Recycle(p, r);
			
			if(primitive == null) return null;

			primitive.SetColor(c == default ? COLOR_DEFAULT : c);
			return primitive;
#else
			return null;
#endif
		}

		/// <summary>Request Box Pool-Object Primitive.</summary>
		/// <param name="p">Box's position.</param>
		/// <param name="r">Box's radius.</param>
		/// <param name="c">Box's color.</param>
		public static PoolPrimitive RequestBoxPrimitive(Vector3 p, Quaternion r, Vector3 s, Color c = default)
		{
#if UNITY_EDITOR
			PoolPrimitive box = RequestPrimitive(Instance.BoxPool, p, r, c);

			if(box == null) return null;

			box.gameObject.SetUnityCubeDimensions(s);
			return box;
#else
			return null;
#endif
		}

		/// <summary>Request Sphere Pool-Object Primitive.</summary>
		/// <param name="p">Sphere's position.</param>
		/// <param name="r">Sphere's radius.</param>
		/// <param name="c">Sphere's color.</param>
		public static PoolPrimitive RequestSpherePrimitive(Vector3 p, float r, Color c = default)
		{
#if UNITY_EDITOR
			PoolPrimitive sphere = RequestPrimitive(Instance.SpherePool, p, Quaternion.identity, c);

			if(sphere == null) return null;

			sphere.gameObject.SetUnitySphereRadius(r);
			return sphere;
#else
			return null;
#endif
		}

		/// <summary>Request Capsule Pool-Object Primitive.</summary>
		/// <param name="p">Capsule's position.</param>
		/// <param name="r">Capsule's radius.</param>
		/// <param name="c">Capsule's color.</param>
		public static PoolPrimitive RequestCapsulePrimitive(Vector3 p, Quaternion rotation, float h, float r, Color c = default)
		{
#if UNITY_EDITOR
			PoolPrimitive capsule = RequestPrimitive(Instance.CapsulePool, p, rotation, c);

			if(capsule == null) return null;

			capsule.gameObject.SetUnityCapsuleDimensions(h, r);
			return capsule;
#else
			return null;
#endif
		}

		/// <summary>Request Cylinder Pool-Object Primitive.</summary>
		/// <param name="p">Cylinder's position.</param>
		/// <param name="r">Cylinder's radius.</param>
		/// <param name="c">Cylinder's color.</param>
		public static PoolPrimitive RequestCylinderPrimitive(Vector3 p, Quaternion rotation, float h, float r, Color c = default)
		{
#if UNITY_EDITOR
			PoolPrimitive cylinder = RequestPrimitive(Instance.CylinderPool, p, rotation, c);

			if(cylinder == null) return null;

			cylinder.gameObject.SetUnityCylinderDimensions(h, r);
			return cylinder;
#else
			return null;
#endif
		}

		/// <summary>Request Text Pool-Object.</summary>
		/// <param name="p">Text's position.</param>
		/// <param name="r">Text's radius.</param>
		/// <param name="_contentPointer">Text's contentPointer [null by default].</param>
		public static PoolText RequestText(Vector3 p, Quaternion r, Func<string> _contentPointer = null, Color c = default)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return null;

			PoolText text = Instance.TextPool.Recycle(p, r);

			if(text == null) return null;

			text.SetContentPointer(_contentPointer);
			text.SetColor(c == default ? COLOR_DEFAULT: c);
			return text;
#else
			return null;
#endif	
		}

		/// <summary>Request Text Pool-Object.</summary>
		/// <param name="p">Text's position.</param>
		/// <param name="r">Text's radius.</param>
		/// <param name="_content">Text's content [empty by default].</param>
		public static PoolText RequestText(Vector3 p, Quaternion r, string _content = "", Color c = default)
		{
#if UNITY_EDITOR
			if(Instance == null || !debug) return null;

			PoolText text = Instance.TextPool.Recycle(p, r);

			if(text == null) return null;

			text.SetColor(c);
			text.SetText(_content);
			return text;
#else
			return null;
#endif	
		}

		/// <summary>Callback internally invoked each time a wait coroutine comes to an end.</summary>
		/// <param name="obj">Pool-Object to free.</param>
		private static void OnWaitFinished(PoolGameObject obj)
		{
			obj.OnObjectDeactivation();
			Instance.DispatchCoroutine(obj.GetInstanceID(), ref Instance.coroutines);
		}

		/// <summary>Keeps Pool-Object active for a given amount of time.</summary>
		/// <param name="time">Time to keep the Pool-Object active before deactivating.</param>
		private static IEnumerator Keep(PoolGameObject obj , float time)
		{
#if UNITY_EDITOR
			WaitForSeconds wait = new WaitForSeconds(time);

			yield return wait;

			OnWaitFinished(obj);
#else
			yield return null;
#endif
		}

		/// <summary>Keeps Pool-Object active for a given amount of time.</summary>
		/// <param name="time">Time to keep the Pool-Object active before deactivating.</param>
		private static IEnumerator KeepAsBoxCollider(PoolGameObject box, BoxCollider _boxCollider, float time)
		{
#if UNITY_EDITOR
			float t = 0.0f;

			while(t < time)
			{
				CopyBoxFromCollider(box, _boxCollider);
				t += Time.deltaTime;
				yield return null;
			}

			OnWaitFinished(box);
#else
			yield return null;
#endif
		}

		/// <summary>Keeps Pool-Object active for a given amount of time.</summary>
		/// <param name="time">Time to keep the Pool-Object active before deactivating.</param>
		private static IEnumerator KeepAsSphereCollider(PoolGameObject sphere, SphereCollider _sphereCollider, float time)
		{
#if UNITY_EDITOR
			float t = 0.0f;

			while(t < time)
			{
				CopySphereFromCollider(sphere, _sphereCollider);
				t += Time.deltaTime;
				yield return null;
			}

			OnWaitFinished(sphere);
#else
			yield return null;
#endif
		}

		/// <summary>Keeps Pool-Object active for a given amount of time.</summary>
		/// <param name="time">Time to keep the Pool-Object active before deactivating.</param>
		/// <param name="_capsuleCollider">CapsuleCollider's reference.</param>
		private static IEnumerator KeepAsCapsuleCollider(PoolGameObject capsule, CapsuleCollider _capsuleCollider, float time)
		{
#if UNITY_EDITOR
			float t = 0.0f;

			while(t < time)
			{
				CopyCapsuleFromCollider(capsule, _capsuleCollider);
				t += Time.deltaTime;
				yield return null;
			}

			OnWaitFinished(capsule);
#else
			yield return null;
#endif
		}
	}
}