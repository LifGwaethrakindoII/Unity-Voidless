using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public class Character : MonoBehaviour
{
	public const int STATE_FLAG_RUNNING = 1 << 0;

	[SerializeField] private VAnimatorController animatorController; 	/// <summary>AnimatorController's Component attached to the Animator.</summary>
	[SerializeField] private AngleDotProduct forwardDotLimit; 			/// <summary>Forward Vector's Dot Limit.</summary>
	[SerializeField] private AngleDotProduct backwardDotLimit; 			/// <summary>Backward Vector's Dot Limit.</summary>
	[SerializeField] private float movementSpeed; 						/// <summary>Movement's Speed.</summary>
	[SerializeField] private float runScalar; 							/// <summary>Running's Scalar.</summary>
	[SerializeField] private float backwardsMovementScalar; 			/// <summary>Backwards' Movement Scalar.</summary>
	[SerializeField] private float rotationSpeed; 						/// <summary>Rotation's Speed.</summary>
	private int state; 													/// <summary>State Flags.</summary>

#region UnityMethods:
	/// <summary>Character's instance initialization.</summary>
	private void Awake()
	{
		
	}

	/// <summary>Character's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		
	}
	
	/// <summary>Character's tick at each frame.</summary>
	private void Update ()
	{
		
	}
#endregion

	/// <summary>Goes Idle.</summary>
	public void GoIdle()
	{
		animatorController.animator.SetFloat("LeftAxisY", 0.0f);
	}

	/// <summary>Activates/Deactivates Running Flag.</summary>
	/// <param name="_run">Activate?.</param>
	public void Run(bool _run)
	{
		switch(_run)
		{
			case true:
			state |= STATE_FLAG_RUNNING;
			break;

			case false:
			state &= ~STATE_FLAG_RUNNING;
			break;
		}
	}

	/// <summary>Displaces Character.</summary>
	/// <param name="axes">Displacement's Axes.</param>
	public void Move(Vector2 axes)
	{
		Vector3 axes3D = new Vector3(axes.x, 0.0f, axes.y);
		
		if(axes3D.sqrMagnitude > 1.0f) axes3D.Normalize();

		float sign = Mathf.Sign(axes3D.z);
		float blend = 0.0f;

		RotateSelf(axes3D.x * sign);

		if(Mathf.Abs(axes3D.z) > Mathf.Abs(axes3D.x))
		{
			if(axes3D.z < 0.0f)
			{
				sign *= backwardsMovementScalar;
				blend = -2.0f;

			} else if((state | STATE_FLAG_RUNNING) == state)
			{
				sign *= runScalar;
				blend = 2.0f;
			}
			else blend = 1.0f;

			transform.Translate(Vector3.forward * axes3D.magnitude * sign * movementSpeed * Time.deltaTime);
		}
		else blend = -1.0f;

		animatorController.animator.SetFloat("LeftAxisY", blend);
	}

	/// <summary>Rotates itself on the left or right, depending of the provided sign.</summary>
	/// <param name="s">Normalized sign that determines which side to rotate relative to itself.</param>
	public void RotateSelf(float s)
	{
		transform.Rotate(Vector3.up * rotationSpeed * s * Time.deltaTime, Space.Self);
	}

	/// <summary>Rotates Character.</summary>
	/// <param name="direction">Look Direction.</param>
	public void Rotate(Vector2 direction)
	{
		direction.y = 0.0f;

		Quaternion lookRotation = Quaternion.LookRotation(direction);
		Quaternion rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

		transform.rotation = rotation;
	}
}
}