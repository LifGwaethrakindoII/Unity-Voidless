using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Voidless
{
public class CosmosPoolManager : Singleton<CosmosPoolManager>
{
	[SerializeField] private CosmosRock rockReference; 								/// <summary>Temporal CosmosRock's Reference.</summary>
	[SerializeField] private CosmosProjectile projectileReference; 					/// <summary>Temporal CosmosProjectile's Reference.</summary>
	[SerializeField] private ParticleEffect[] _particleEffectsReferences; 			/// <summary>Particle-Effects' References.</summary>
	[SerializeField] private CosmosProjectile[] _projectilesReferences; 			/// <summary>Projectiles' References.</summary>
	[SerializeField] private ParticleEffect particleEffectReference; 				/// <summary>Temporal Particle-Effect's Reference.</summary>
	[SerializeField] private PoolUIElement _poolTextReference; 						/// <summary>Pool Text's Reference.</summary>
	private GameObjectPool<CosmosProjectile> _projectilesPool; 						/// <summary>Pool of Projectiles.</summary>
	private GameObjectPool<CosmosRock> _rocksPool; 									/// <summary>Pool of Rocks.</summary>
	private GameObjectPool<ParticleEffect> _particleEffectsPool; 					/// <summary>Pool of Particle-Effects.</summary>
	private GameObjectPool<PoolUIElement> _poolTextsPool; 							/// <summary>Pool of PoolTexts.</summary>
	private Dictionary<int, GameObjectPool<ParticleEffect>> _particleEffectsPools; 	/// <summary>Pools of Particle-Effects.</summary>
	private Dictionary<int, GameObjectPool<CosmosProjectile>> _projectilesPools; 	/// <summary>Pools of Projectiles.</summary>

#region Getters/Setters:
	/// <summary>Gets particleEffectsReferences property.</summary>
	public ParticleEffect[] particleEffectsReferences { get { return _particleEffectsReferences; } }

	/// <summary>Gets projectilesReferences property.</summary>
	public CosmosProjectile[] projectilesReferences { get { return _projectilesReferences; } }

	/// <summary>Gets poolTextReference property.</summary>
	public PoolUIElement poolTextReference { get { return _poolTextReference; } }

	/// <summary>Gets and Sets projectilesPool property.</summary>
	public GameObjectPool<CosmosProjectile> projectilesPool
	{
		get { return _projectilesPool; }
		private set { _projectilesPool = value; }
	}

	/// <summary>Gets and Sets rocksPool property.</summary>
	public GameObjectPool<CosmosRock> rocksPool
	{
		get { return _rocksPool; }
		private set { _rocksPool = value; }
	}
	
	/// <summary>Gets and Sets particleEffectsPool property.</summary>
	public GameObjectPool<ParticleEffect> particleEffectsPool
	{
		get { return _particleEffectsPool; }
		private set { _particleEffectsPool = value; }
	}

	/// <summary>Gets and Sets poolTextsPool property.</summary>
	public GameObjectPool<PoolUIElement> poolTextsPool
	{
		get { return _poolTextsPool; }
		private set { _poolTextsPool = value; }
	}

	/// <summary>Gets and Sets particleEffectsPools property.</summary>
	public Dictionary<int, GameObjectPool<ParticleEffect>> particleEffectsPools
	{
		get { return _particleEffectsPools; }
		private set { _particleEffectsPools = value; }
	}

	/// <summary>Gets and Sets projectilesPools property.</summary>
	public Dictionary<int, GameObjectPool<CosmosProjectile>> projectilesPools
	{
		get { return _projectilesPools; }
		private set { _projectilesPools = value; }
	}
#endregion

	/// <summary>CosmosPoolManager's instance initialization.</summary>
	private void Awake()
	{
		projectilesPool = new GameObjectPool<CosmosProjectile>(projectileReference);
		rocksPool = new GameObjectPool<CosmosRock>(rockReference);
		particleEffectsPool = new GameObjectPool<ParticleEffect>(particleEffectReference);
		poolTextsPool = new GameObjectPool<PoolUIElement>(poolTextReference);
		projectilesPools = GameObjectPool<CosmosProjectile>.PopulatedPoolsDictionary(1, projectilesReferences);
		particleEffectsPools = GameObjectPool<ParticleEffect>.PopulatedPoolsDictionary(1, particleEffectsReferences);
	}

	/// <summary>Requests CosmosProjectile's instance.</summary>
	/// <param name="position">Projectile's Position.</param>
	/// <param name="direction">Projectile's Direction.</param>
	/// <param name="shooter">Shooter's GameObject reference [null by default].</param>
	/// <param name="_tag">Tag to assign to projectile [Empty by default].</param>
	public static CosmosProjectile RequestCosmosProjectile(Vector2 position, Vector3 direction, GameObject shooter = null, string _tag = null)
	{
		CosmosProjectile projectile = Instance.projectilesPool.Recycle(position, VQuaternion.RightLookRotation(direction));
		
		if(projectile != null) projectile.shooter = shooter;
		if(!string.IsNullOrEmpty(_tag)) projectile.gameObject.tag = _tag;

		return projectile;
	}

	/// <summary>Requests CosmosProjectile's instance.</summary>
	/// <param name="_reference">Projectile's reference.</param>
	/// <param name="position">Projectile's Position.</param>
	/// <param name="direction">Projectile's Direction.</param>
	/// <param name="shooter">Shooter's GameObject reference [null by default].</param>
	/// <param name="_tag">Tag to assign to projectile [Empty by default].</param>
	public static CosmosProjectile RequestCosmosProjectile(CosmosProjectile _reference, Vector2 position, Vector3 direction, GameObject shooter = null, string _tag = null)
	{
		if(_reference == null) return null;

		int ID = _reference.GetInstanceID();
		CosmosProjectile projectile = null;
		GameObjectPool<CosmosProjectile> pool = null;
		
		if(!Instance.projectilesPools.TryGetValue(ID, out pool)) return null;

		projectile = pool.Recycle(position, VQuaternion.RightLookRotation(direction));

		if(projectile != null) projectile.shooter = shooter;
		if(!string.IsNullOrEmpty(_tag)) projectile.gameObject.tag = _tag;

		return projectile;
	}

	/// <summary>Requests CosmosRock's intance.</summary>
	/// <param name="position">Rock's position.</param>
	/// <param name="rotation">Rock's rotation.</param>
	/// <param name="radiusRange">Rock's Radius-Range to assign.</param>
	public static CosmosRock RequestCosmosRock(Vector2 position, Quaternion rotation, FloatRange radiusRange)
	{
		CosmosRock rock = Instance.rocksPool.Recycle(position, rotation);

		rock.radiusRange = radiusRange;
		rock.GenerateRandomRock();

		return rock;
	}

	/// <summary>Requests Particle-Effect.</summary>
	/// <param name="_reference">Particle-Effect's reference.</param>
	/// <param name="position">Spawn Position.</param>
	/// <param name="direction">Look direction [Vector2.one by default].</param>
	public static ParticleEffect RequestParticleEffect(ParticleEffect _reference, Vector2 position, Vector2 direction = default(Vector2))
	{
		if(_reference == null) return null;

		int ID = _reference.GetInstanceID();
		ParticleEffect partticleEffect = null;
		GameObjectPool<ParticleEffect> pool = null;

		if(!Instance.particleEffectsPools.TryGetValue(ID, out pool)) return null;

		return pool.Recycle(position, direction.sqrMagnitude > 0.0f ? VQuaternion.RightLookRotation(direction) : Quaternion.identity);
	}

	/// <summary>Requests Pool Text.</summary>
	/// <param name="_screenPosition">Screen Position [must be already passed converted by the camera].</param>
	/// <param name="_text">Text's content.</param>
	public static PoolUIElement RequestPoolText(Vector2 _screenPosition, string _text)
	{
		PoolUIElement UIElement = Instance.poolTextsPool.Recycle(_screenPosition, Quaternion.identity);

		if(UIElement == null) return null;

		Text text = UIElement.GetComponent<Text>();

		if(text != null)
		{
			text.text = _text;
			text.color = Color.white;
		}

		return UIElement;
	}
}
}