using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public enum MuzzleShootType
{
	SingleMuzzle,
	FromAllMuzzles,
	OneByOneMuzzle
}

public class CosmosShootAbility : MonoBehaviour
{
	[SerializeField] private CosmosProjectile _projectileReference; 	/// <summary>Projectile's reference.</summary>
	[SerializeField] private ParticleEffect _particleEffect; 			/// <summary>Particle-Effect reference to emit whe shooting.</summary>
	[SerializeField] private MuzzleShootType _shootType; 				/// <summary>Muzzle's Shoot Type.</summary>
	[SerializeField] private Transform[] _muzzles; 						/// <summary>Ship's Muzzle.</summary>
	[SerializeField] private GameObjectTag _projectileTag; 				/// <summary>Tag to assign to shot projectiles.</summary>
	[SerializeField] private Cooldown _cooldown; 						/// <summary>Shoot's Cooldown.</summary>
	private int _muzzleIndex; 											/// <summary>current Muzzle's Index.</summary>

	/// <summary>Gets projectileReference property.</summary>
	public CosmosProjectile projectileReference { get { return _projectileReference; } }

	/// <summary>Gets particleEffect property.</summary>
	public ParticleEffect particleEffect { get { return _particleEffect; } }

	/// <summary>Gets shootType property.</summary>
	public MuzzleShootType shootType { get { return _shootType; } }

	/// <summary>Gets muzzles property.</summary>
	public Transform[] muzzles { get { return _muzzles; } }

	/// <summary>Gets and Sets cooldown property.</summary>
	public Cooldown cooldown
	{
		get { return _cooldown; }
		private set { _cooldown = value; }
	}

	/// <summary>Gets projectileTag property.</summary>
	public GameObjectTag projectileTag { get { return _projectileTag; } }

	/// <summary>Gets and Sets muzzleIndex property.</summary>
	public int muzzleIndex
	{
		get { return _muzzleIndex; }
		private set { _muzzleIndex = value; }
	}

	/// <summary>CosmosShootAbility's instance initialization.</summary>
	private void Awake()
	{
		cooldown.Initialize(this, null);
	}

	/// <summary>Shoots Projectile.</summary>
	/// <returns>Projectile shot [if there was no cooldown].</returns>
	public CosmosProjectile[] Shoot()
	{
		if(cooldown.active) return null;

		int length = shootType == MuzzleShootType.FromAllMuzzles ? muzzles.Length : 1;
		CosmosProjectile[] projectiles = new CosmosProjectile[length];
		Transform muzzle = null;

		switch(shootType)
		{
			case MuzzleShootType.SingleMuzzle:
				muzzle = muzzles[0];
				projectiles[0] = CosmosPoolManager.RequestCosmosProjectile(projectileReference, muzzle.position, muzzle.forward, gameObject, projectileTag);
				CosmosPoolManager.RequestParticleEffect(particleEffect, muzzle.position);
			break;

			case MuzzleShootType.FromAllMuzzles:
				for(int i = 0; i < length; i++)
				{
					muzzle = muzzles[i];
					projectiles[i] = CosmosPoolManager.RequestCosmosProjectile(projectileReference, muzzle.position, muzzle.forward, gameObject, projectileTag);
					CosmosPoolManager.RequestParticleEffect(particleEffect, muzzle.position);
				}
			break;

			case MuzzleShootType.OneByOneMuzzle:
				muzzle = muzzles[muzzleIndex];
				projectiles[0] = CosmosPoolManager.RequestCosmosProjectile(projectileReference, muzzle.position, muzzle.forward, gameObject, projectileTag);
				CosmosPoolManager.RequestParticleEffect(particleEffect, muzzle.position);
				muzzleIndex = muzzleIndex + 1 == muzzles.Length ? 0 : muzzleIndex + 1;
			break;
		}

		return projectiles;
	}
}
}