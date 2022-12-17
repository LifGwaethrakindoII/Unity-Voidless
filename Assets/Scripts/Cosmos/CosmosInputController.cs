using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Voidless
{
public class CosmosInputController : BaseInputController
{
	[SerializeField] private CosmosVoyager _cosmosVoyager; 	/// <summary>Cosmos' Voyager GameObject.</summary>
	[Space(5f)]
	[SerializeField] private string _shootProjectileID; 	/// <summary>Shoot Projectile's ID.</summary>
	private InputAction _shootProjectileAction; 			/// <summary>Shoot Projectile's Action.</summary>

	/// <summary>Gets and Sets cosmosVoyager property.</summary>
	public CosmosVoyager cosmosVoyager
	{
		get { return _cosmosVoyager; }
		set { _cosmosVoyager = value; }
	}

	/// <summary>Gets shootProjectileID property.</summary>
	public string shootProjectileID { get { return _shootProjectileID; } }

	/// <summary>Gets and Sets shootProjectileAction property.</summary>
	public InputAction shootProjectileAction
	{
		get { return _shootProjectileAction; }
		set { _shootProjectileAction = value; }
	}

	/// <summary>Sets Input's Actions.</summary>
	protected override void SetInputActions()
	{
		base.SetInputActions();
		if(!string.IsNullOrEmpty(shootProjectileID)) shootProjectileAction = actionMap.FindAction(shootProjectileID, true);

		if(shootProjectileAction != null) shootProjectileAction.performed += OnShootProjectileActionPerformed;
	}

	/// <summary>Callback internally invoked when the Axes are updated, but before the previous axes' values get updated.</summary>
	protected override void OnAxesUpdated()
	{
		if(cosmosVoyager == null) return;

		cosmosVoyager.Move(leftAxes);
	}

	/// <summary>Callback invoked when the Shoot Projectile's InputAction is Performed.</summary>
	/// <param name="_context">Callback's Context.</param>
	private void OnShootProjectileActionPerformed(InputAction.CallbackContext _context)
	{
		if(cosmosVoyager == null) return;

		cosmosVoyager.ShootProjectile();
	}
}
}