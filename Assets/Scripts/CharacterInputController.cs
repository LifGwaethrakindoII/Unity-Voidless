using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Voidless
{
//[RequireComponent(typeof(PlayerInput))]
public class CharacterInputController : BaseCharacterInputController<Character>
{
	[Space(5f)]
	[SerializeField] private string runID; 	/// <summary>Run's ID.</summary>
	private InputAction runAction; 			/// <summary>Run's Action.</summary>

	/// <summary>Sets Input's Actions.</summary>
	protected override void SetInputActions()
	{
		base.SetInputActions();

		if(!string.IsNullOrEmpty(runID)) runAction = actionMap.FindAction(runID, true);

		if(runAction == null) return;

		runAction.performed += OnRunActionPerformed;
		runAction.canceled += OnRunActionCanceled;
	}

	/// <summary>Callback internally invoked when the Axes are updated, but before the previous axes' values get updated.</summary>
	protected override void OnAxesUpdated()
	{
		if(character == null) return;

		if(previousLeftAxes.sqrMagnitude >= (leftDeadZoneRadius * leftDeadZoneRadius))
		{
			character.Move(leftAxes);
			
			/*if(Mathf.Abs(leftAxes.x) >= (leftDeadZoneRadius * leftDeadZoneRadius))
			character.RotateSelf(leftAxes.x);*/
		}
		else character.GoIdle();
	}

	/// <summary>Callback invoked when the Run's InputAction is Performed.</summary>
	/// <param name="_context">Callback's Context.</param>
	private void OnRunActionPerformed(InputAction.CallbackContext _context)
	{
		character.Run(true);		
	}

	/// <summary>Callback invoked when the Run's InputAction is Canceled.</summary>
	/// <param name="_context">Callback's Context.</param>
	private void OnRunActionCanceled(InputAction.CallbackContext _context)
	{
		character.Run(false);		
	}
}
}