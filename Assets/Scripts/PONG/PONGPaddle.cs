using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
[RequireComponent(typeof(CharacterController))]
public class PONGPaddle : MonoBehaviour
{
	[SerializeField] private float _speed; 				/// <summary>Paddle's Speed.</summary>
	private CharacterController _characterController; 	/// <summary>CharacterController's Component.</summary>

	/// <summary>Gets speed property.</summary>
	public float speed { get { return _speed; } }

	/// <summary>Gets characterController Component.</summary>
	public CharacterController characterController
	{ 
		get
		{
			if(_characterController == null) _characterController = GetComponent<CharacterController>();
			return _characterController;
		}
	}

	/// <summary>Moves on the Y-Axis.</summary>
	/// <param name="y">Displacement on the Y-Axis.</param>
	public void Move(float y)
	{
		characterController.Move(Vector3.up * y * speed * Time.deltaTime);
	}
}
}