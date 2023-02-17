using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public class CosmosProjectile : CosmosObject
{	
	[Space(5f)]
	[Header("Projectile's Attributes:")]
	[SerializeField] private float _lifespan; 				/// <summary>Projectile's Lifespan.</summary>
	private GameObject _shooter; 							/// <summary>Shooter GameObject's Reference.</summary>
	private float _time; 									/// <summary>Current Life's Time.</summary>

	/// <summary>Gets and Sets shooter property.</summary>
	public GameObject shooter
	{
		get { return _shooter; }
		set { _shooter = value; }
	}

	/// <summary>Gets and Sets lifespan property.</summary>
	public float lifespan
	{
		get { return _lifespan; }
		set { _lifespan = value; }
	}

	/// <summary>Gets and Sets time property.</summary>
	public float time
	{
		get { return _time; }
		set { _time = value; }
	}

	/// <summary>Callback invoked when CosmosProjectile's instance is disabled.</summary>
	private void OnDisable()
	{
		//Debug.Log("[CosmosProjectile] WTF...");
	}

	/// <summary>CosmosProjectile's instance initialization when loaded [Before scene loads].</summary>
	protected override void Awake()
	{
		base.Awake();
	}

	/// <summary>Updates CosmosProjectile's instance at each frame.</summary>
	protected override void Update()
	{
		base.Update();
		Move(transform.right);
	}

	/// <summary>Updates CosmosProjectile's instance at each Physics Thread's frame.</summary>
	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if(time >= lifespan) OnObjectDeactivation();
		else time += Time.fixedDeltaTime;
	}

	/// <summary>Callback internally invoked when impacting.</summary>
	/// <param name="_cosmosObject">CosmosObject that impacted with this CosmosObject.</param>
	protected override void OnImpact(CosmosObject _cosmosObject)
	{
		OnObjectDeactivation();
	}

	/// <summary>Independent Actions made when this Pool Object is being created.</summary>
	public override void OnObjectCreation()
	{
		base.OnObjectCreation();
		Awake();
	}

	/// <summary>Actions made when this Pool Object is being reseted.</summary>
	public override void OnObjectReset()
	{
		base.OnObjectReset();
		time = 0.0f;
		shooter = null;
	}

	/// <returns>String representing this CosmosRock.</returns>
	public override string ToString()
	{
		StringBuilder builder = new StringBuilder();

		builder.AppendLine(base.ToString());
		builder.Append("Lifespan: ");
		builder.AppendLine(lifespan.ToString());
		builder.Append("Current Time: ");
		builder.Append(time.ToString());

		return builder.ToString();
	}
}
}