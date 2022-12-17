using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
[RequireComponent(typeof(CosmosShootAbility))]
[RequireComponent(typeof(SteeringVehicle2D))]
[RequireComponent(typeof(Health))]
public class CosmosEnemy : CosmosObject
{
	private CosmosShootAbility _shootAbility; 	/// <summary>CosmosShootAbility's Component.</summary>
	private SteeringVehicle2D _vehicle; 		/// <summary>SteeringVehicle2D's Component.</summary>
	private Health _health; 					/// <summary>Health's Component.</summary>

	/// <summary>Gets shootAbility Component.</summary>
	public CosmosShootAbility shootAbility
	{ 
		get
		{
			if(_shootAbility == null) _shootAbility = GetComponent<CosmosShootAbility>();
			return _shootAbility;
		}
	}

	/// <summary>Gets vehicle Component.</summary>
	public SteeringVehicle2D vehicle
	{ 
		get
		{
			if(_vehicle == null) _vehicle = GetComponent<SteeringVehicle2D>();
			return _vehicle;
		}
	}

	/// <summary>Gets health Component.</summary>
	public Health health
	{ 
		get
		{
			if(_health == null) _health = GetComponent<Health>();
			return _health;
		}
	}

	/// <summary>CosmosEnemy's instance initialization when loaded [Before scene loads].</summary>
	protected override void Awake()
	{
		base.Awake();
		health.onHealthEvent += OnHealthEvent;
	}

	/// <summary>Callback invoked when CosmosObject's instance is going to be destroyed and passed to the Garbage Collector.</summary>
	protected override void OnDestroy()
	{
		health.onHealthEvent -= OnHealthEvent;	
	}

	/// <summary>Event triggered when this Collider/Rigidbody begun having contact with another Collider/Rigidbody.</summary>
	/// <param name="col">The Collision data associated with this collision Event.</param>
	protected override void OnCollisionEnter2D(Collision2D col)
	{
		base.OnCollisionEnter2D(col);

		/// Don't affect the Rigidbody's properties:
		rigidbody.Sleep();

		foreach(ContactPoint2D contact in col.contacts)
		{
			CosmosPoolManager.RequestParticleEffect(hitParticleEffect, contact.point, contact.normal);
		}
	}

	/// <summary>Event invoked when a Health's event has occured.</summary>
	/// <param name="_event">Type of Health Event.</param>
	/// <param name="_amount">Amount of health that changed [0.0f by default].</param>
	/// <param name="_object">GameObject that caused the event, null be default.</param>
	private void OnHealthEvent(HealthEvent _event, float _amount = 0.0f, GameObject _object = null)
	{
		switch(_event)
		{
			case HealthEvent.Depleted:
				
			break;

			case HealthEvent.FullyDepleted:
				OnObjectDeactivation();
				InvokeEvent(ID_EVENT_DESTROYED);
			break;
		}
	}

	/// <summary>Shoots Projectile.</summary>
	public void ShootProjectile()
	{
		CosmosProjectile[] projectiles = shootAbility.Shoot();

		if(projectiles != null) foreach(CosmosProjectile projectile in projectiles)
		{
			InvokeEvent(ID_EVENT_SHOTPROJECTILE, projectile);
		}
	}
}
}