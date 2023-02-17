using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Voidless
{
public class CosmosCameraController : MonoBehaviour
{
	[SerializeField] private Transform _target; 						/// <summary>Camera's Target.</summary>
	[SerializeField] private Vector3 _offset; 							/// <summary>Offset from Target.</summary>
	[SerializeField][Range(0.0f, 1.0f)] private float _smoothSpeed; 	/// <summary>Smooth's Speed.</summary>

	/// <summary>Gets and Sets target property.</summary>
	public Transform target
	{
		get { return _target; }
		set { _target = value; }
	}

	/// <summary>Gets and Sets offset property.</summary>
	public Vector3 offset
	{
		get { return _offset; }
		set { _offset = value; }
	}

	/// <summary>Gets and Sets smoothSpeed property.</summary>
	public float smoothSpeed
	{
		get { return _smoothSpeed; }
		set { _smoothSpeed = Mathf.Clamp(value, 0.0f, 1.0f); }
	}

	/// <summary>CosmosCameraController's instance initialization when loaded [Before scene loads].</summary>
	private void Awake()
	{
		//VTime.EqualizeFixedDeltaTime(30);
	}

	[OnInspectorGUI]
	/// <summary>Updates CosmosCameraController's instance at the end of each frame.</summary>
	private void LateUpdate()
	{
		if(target == null) return;

		Vector3 a = transform.position;
		Vector3 b = target.position + offset;

		transform.position = Vector3.Lerp(a, b, smoothSpeed);
	}
}
}