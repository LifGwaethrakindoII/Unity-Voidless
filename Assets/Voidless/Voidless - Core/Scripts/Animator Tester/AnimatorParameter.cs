using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
	public enum AnimatorParameterType
	{
		Bool,
		Int,
		Float,
		BlendTreeFloat
	}

	[Serializable]
	public struct AnimatorParameter
	{
		public AnimatorCredential key;
		public AnimatorParameterType type;
		public bool boolValue;
		public int intValue;
		public float floatValue;
	}
}