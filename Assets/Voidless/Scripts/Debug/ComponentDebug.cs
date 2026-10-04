using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{

	/// <summary>
	/// Tracks a list of monobehaviours and reports their status on a frame by frame basis, for troubleshooting
	/// strange prefab/component related problems that do not exist in editor.
	/// </summary>
	public class ComponentDebug : MonoBehaviour
	{

		[SerializeField] MonoBehaviour[] componentsList;

		private void Start () {
			if ( componentsList == null || componentsList.Length == 0 ) {
				// No components set, destroy self.
				Destroy( this );
				return;
			}

			StartCoroutine( DelayedUpdate() );
		}

		void Report() {
			if ( componentsList == null || componentsList.Length == 0 ) {
				Debug.LogWarning( $" [[ COMPONENT DEBUG ]] Array of components suddenly null or empty after start!" );
				// No components set, destroy self.
				Destroy( this );
				return;
			}

			string debugStr = " [[ COMPONENT DEBUG ]]\n";
			for ( int i = 0; i < componentsList.Length; i++ ) {
				var x = componentsList[i];
				if ( x != null )
					debugStr += $" -- Component: {x.GetType().Name} on object {x.name}<-{x.transform.parent?.name}<-{x.transform.parent?.parent?.name}, enabled: {x.isActiveAndEnabled}\n";
				else
					debugStr += $" -- COMPONENT AT INDEX {i} IS SUDDENLY NULL!\n";

				if (i > 0 && i % 5 == 0 ) {
					Debug.Log( debugStr );
					debugStr = " [[ COMPONENT DEBUG ]]\n";
				}
			}

			Debug.Log( debugStr );
		}

		IEnumerator DelayedUpdate() {
			while ( true ) {
				yield return new WaitForSeconds( 3 );

				Report();
			}
		}
	}
}