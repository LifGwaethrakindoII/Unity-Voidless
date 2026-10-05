using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Voidless.AddressableAssets
{
	[Serializable]
	public class FiniteStateAudioClipAssetReference : VAssetReferenceT<FiniteStateAudioClip>
	{
		/// <summary>FiniteStateAudioClipAssetReference default constructor.</summary>
		public FiniteStateAudioClipAssetReference(string guid) : base(guid) { /*...*/ }
	}
}