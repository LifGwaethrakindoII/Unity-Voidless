using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Voidless
{
	/// <summary>Event invoked when the CoroutineBehavior ends.</summary>
	public delegate void OnCoroutineEnds();

	public abstract class CoroutineBehavior<T> : MonoBehaviour
	{
		public event OnCoroutineEnds onCoroutineEnds; 	/// <summary>OnCoroutineEnds event delegate.</summary>

		[Space(5f)]
		[Header("Gizmos' Attributes:")]
#if ODIN_INSPECTOR	
		[TabGroup("Gizmos Group", "Gizmos")]
#endif
		[SerializeField] public bool drawGizmos;
#if ODIN_INSPECTOR
		[TabGroup("Gizmos Group", "Gizmos")]
#endif
		[SerializeField] public Color gizmosColor;
#if ODIN_INSPECTOR
		[TabGroup("Gizmos Group", "Gizmos")]
#endif
		[SerializeField] public float gizmosRadius;

		/// <summary>Draws Gizmos [if drawGizmos' flag is turned on].</summary>
		protected virtual void DrawGizmos() { /*...*/ }

		/// <summary>Draws Gizmos on Editor mode when CoroutineBehavior's instance is selected.</summary>
		private void OnDrawGizmosSelected()
		{
			if(drawGizmos) DrawGizmos();
		}

		/// <summary>Coroutine's IEnumerator.</summary>
		/// <param name="obj">Object of type T's argument.</param>
		public virtual IEnumerator Routine(T obj) { yield return null; }

		/// <summary>Invokes OnCoroutineEnds' delegate.</summary>
		public void InvokeCoroutineEnd() { if(onCoroutineEnds != null) onCoroutineEnds(); }
	}
}