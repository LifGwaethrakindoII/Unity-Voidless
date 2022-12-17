using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using Random = UnityEngine.Random;

namespace Voidless
{
public enum AsteroidsGameState
{
	Menu,
	Gameplay,
	GameOver
}

[RequireComponent(typeof(Boundaries2DContainer))]
[RequireComponent(typeof(LineRenderer))]
public class CosmosAsteroidsGameplayController : MonoBehaviour
{
	[InfoBox("@ToString()")]
	[SerializeField] private Camera _camera; 															/// <summary>Main Camera's Reference.</summary>
	[SerializeField] private CosmosAsteroidsGUIController _UIController; 								/// <summary>CosmosAsteroidsGUIController's reference.</summary>
	[SerializeField] private Clock _clock; 																/// <summary>Time's Clock.</summary>
	[SerializeField] private CosmosVoyager _player; 													/// <summary>Player's Avatar.</summary>
	[SerializeField] private RandomDistributionSystem _boundariesDistributionSystem; 					/// <summary>Boundaries' Random Distribution System.</summary>
	[SerializeField] private FloatRange _outsideBoundariesSpawnSpace; 									/// <summary>Spawn Space outside the boundaries.</summary>
	[SerializeField] private float _updateRate; 														/// <summary>Game's Update Rate.</summary>
	[SerializeField] private float _timeForMaxAreaCoveragePercentage; 									/// <summary>Required time to evaluate the Maximum of the boundaries' area percentage.</summary>
	[SerializeField] private FloatRange _areaCoveragePercentage; 										/// <summary>Percentage of Maximum boundaries' area that can be covered for all CosmosObjects in scene (defined as a range).</summary>
	[SerializeField][Range(0.0f, 1.0f)] private float _maxAreaCoveragePercentage; 						/// <summary>Percentage of maximum boundaries' area that can be covered for all CosmosObjects in scene.</summary>
	[SerializeField] private float _maxDifficultyTime; 													/// <summary>Gameplay Time to reach maximum's difficulty.</summary>
	[Space(5f)]
	[Header("Asteroids' Attributes:")]
	[TabGroup("SpawnGroup", "Asteroids")][SerializeField] private FloatRange _minimumRockRadiusRange; 	/// <summary>Minimum Rocks' Radius-Range.</summary>
	[TabGroup("SpawnGroup", "Asteroids")][SerializeField] private FloatRange _maximumRockRadiusRange; 	/// <summary>Maximum Rocks' Radius-Range.</summary>
	[TabGroup("SpawnGroup", "Asteroids")][SerializeField] private FloatRange _rockSpeedRange; 			/// <summary>Rocks' Speed Range.</summary>
	[Space(5f)]
	[Header("Tags:")]
	[SerializeField] private GameObjectTag _playerTag; 													/// <summary>Player's Tag.</summary>
	[SerializeField] private GameObjectTag _enemyTag; 													/// <summary>Enemy's Tag.</summary>
	[SerializeField] private GameObjectTag _playerProjectileTag; 										/// <summary>Player Projectile's Tag.</summary>
	[SerializeField] private GameObjectTag _enemyProjectileTag; 										/// <summary>Enemy Projectile's Tag.</summary>
	[SerializeField] private GameObjectTag _rockTag; 													/// <summary>Rock's Tag.</summary>
	[Space(5f)]
	[Header("Scoring's Attributes:")]
	[SerializeField] private int _rockScore; 															/// <summary>Score amount given for hitting rocks.</summary>
	[SerializeField] private FloatIntDictionary _rockRadiusScoreScalarMap; 								/// <summary>Mapping of how Rock's scoring gets scaled depending on the radius of the destroyed Rock.</summary>
	[Space(5f)]
	[Header("Camera's Settings:")]
	[SerializeField] private ShakeAttributes _playerHitShakeAttributes; 								/// <summary>Camera Shake's Attributes for when the player is hit.</summary>
	private HashSet<CosmosObject> _cosmosObjects; 														/// <summary>CosmosObjects on scene.</summary>
	private HashSet<CosmosObject> _outerCosmosObjects; 													/// <summary>CosmosObjects outside the boundaries.</summary>
	private HashSet<CosmosEnemy> _cosmosEnemies; 														/// <summary>CosmosEnemies.</summary>
	private List<CosmosObject> _cosmosObjectsToRemove; 													/// <summary>Cosmos Objects to remove at the end of frame.</summary>
	private float _time; 																				/// <summary>Gameplay's Time.</summary>
	private float _spawnTime; 																			/// <summary>Current Time.</summary>
	private float _area; 																				/// <summary>Boundaries' Area.</summary>
	private int _score; 																				/// <summary>Game Session's Score.</summary>
	private AsteroidsGameState _gameState; 																/// <summary>current Game State.</summary>
	private Boundaries2DContainer _boundariesContainer; 												/// <summary>Boundaries2DContainer's Component.</summary>
	private LineRenderer _lineRenderer; 																/// <summary>LineRenderer's Component.</summary>
	private Coroutine loop; 																			/// <summary>Loop Coroutine's Reference.</summary>
	private Coroutine cameraShake; 																		/// <summary>Camera Shale's Coroutine.</summary>

#region Getters/Setters:
	/// <summary>Gets camera property.</summary>
	public Camera camera { get { return _camera; } }

	/// <summary>Gets UIController property.</summary>
	public CosmosAsteroidsGUIController UIController { get { return _UIController; } }

	/// <summary>Gets clock property.</summary>
	public Clock clock { get { return _clock; } }

	/// <summary>Gets player property.</summary>
	public CosmosVoyager player { get { return _player; } }

	/// <summary>Gets boundariesDistributionSystem property.</summary>
	public RandomDistributionSystem boundariesDistributionSystem { get { return _boundariesDistributionSystem; } }

	/// <summary>Gets outsideBoundariesSpawnSpace property.</summary>
	public FloatRange outsideBoundariesSpawnSpace { get { return _outsideBoundariesSpawnSpace; } }

	/// <summary>Gets areaCoveragePercentage property.</summary>
	public FloatRange areaCoveragePercentage { get { return _areaCoveragePercentage; } }

	/// <summary>Gets minimumRockRadiusRange property.</summary>
	public FloatRange minimumRockRadiusRange { get { return _minimumRockRadiusRange; } }

	/// <summary>Gets maximumRockRadiusRange property.</summary>
	public FloatRange maximumRockRadiusRange { get { return _maximumRockRadiusRange; } }

	/// <summary>Gets rockSpeedRange property.</summary>
	public FloatRange rockSpeedRange { get { return _rockSpeedRange; } }

	/// <summary>Gets updateRate property.</summary>
	public float updateRate { get { return _updateRate; } }

	/// <summary>Gets timeForMaxAreaCoveragePercentage property.</summary>
	public float timeForMaxAreaCoveragePercentage { get { return _timeForMaxAreaCoveragePercentage; } }

	/// <summary>Gets playerTag property.</summary>
	public GameObjectTag playerTag { get { return _playerTag; } }

	/// <summary>Gets enemyTag property.</summary>
	public GameObjectTag enemyTag { get { return _enemyTag; } }

	/// <summary>Gets playerProjectileTag property.</summary>
	public GameObjectTag playerProjectileTag { get { return _playerProjectileTag; } }

	/// <summary>Gets enemyProjectileTag property.</summary>
	public GameObjectTag enemyProjectileTag { get { return _enemyProjectileTag; } }

	/// <summary>Gets rockTag property.</summary>
	public GameObjectTag rockTag { get { return _rockTag; } }

	/// <summary>Gets playerHitShakeAttributes property.</summary>
	public ShakeAttributes playerHitShakeAttributes { get { return _playerHitShakeAttributes; } }

	/// <summary>Gets and Sets cosmosObjects property.</summary>
	public HashSet<CosmosObject> cosmosObjects
	{
		get { return _cosmosObjects; }
		private set { _cosmosObjects = value; }
	}

	/// <summary>Gets and Sets outerCosmosObjects property.</summary>
	public HashSet<CosmosObject> outerCosmosObjects
	{
		get { return _outerCosmosObjects; }
		private set { _outerCosmosObjects = value; }
	}

	/// <summary>Gets and Sets cosmosEnemies property.</summary>
	public HashSet<CosmosEnemy> cosmosEnemies
	{
		get { return _cosmosEnemies; }
		private set { _cosmosEnemies = value; }
	}

	/// <summary>Gets and Sets cosmosObjectsToRemove property.</summary>
	public List<CosmosObject> cosmosObjectsToRemove
	{
		get { return _cosmosObjectsToRemove; }
		set { _cosmosObjectsToRemove = value; }
	}

	/// <summary>Gets and Sets time property.</summary>
	public float time
	{
		get { return _time; }
		set { _time = value; }
	}

	/// <summary>Gets and Sets spawnTime property.</summary>
	public float spawnTime
	{
		get { return _spawnTime; }
		private set { _spawnTime = value; }
	}

	/// <summary>Gets and Sets area property.</summary>
	public float area
	{
		get { return _area; }
		private set { _area = value; }
	}

	/// <summary>Gets and Sets maxAreaCoveragePercentage property.</summary>
	public float maxAreaCoveragePercentage
	{
		get { return _maxAreaCoveragePercentage; }
		private set { _maxAreaCoveragePercentage = value; }
	}

	/// <summary>Gets maxDifficultyTime property.</summary>
	public float maxDifficultyTime { get { return _maxDifficultyTime; } }

	/// <summary>Gets rockScore property.</summary>
	public int rockScore { get { return _rockScore; } }

	/// <summary>Gets and Sets score property.</summary>
	public int score
	{
		get { return _score; }
		set { _score = value; }
	}

	/// <summary>Gets and Sets gameState property.</summary>
	public AsteroidsGameState gameState
	{
		get { return _gameState; }
		set { _gameState = value; }
	}

	/// <summary>Gets rockRadiusScoreScalarMap property.</summary>
	public FloatIntDictionary rockRadiusScoreScalarMap { get { return _rockRadiusScoreScalarMap; } }

	/// <summary>Gets boundariesContainer Component.</summary>
	public Boundaries2DContainer boundariesContainer
	{ 
		get
		{
			if(_boundariesContainer == null) _boundariesContainer = GetComponent<Boundaries2DContainer>();
			return _boundariesContainer;
		}
	}

	/// <summary>Gets lineRenderer Component.</summary>
	public LineRenderer lineRenderer
	{ 
		get
		{
			if(_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
			return _lineRenderer;
		}
	}
#endregion

	private void OnGUI()
	{
		GUILayout.Label("Position: " + camera.WorldToScreenPoint(player.transform.position).ToString());
		GUILayout.Label("Screen Dimension: " + (new Vector2(Screen.width, Screen.height)).ToString());
	}

	/// <summary>CosmosAsteroidsGameplayController's instance initialization when loaded [Before scene loads].</summary>
	private void Awake()
	{
		VTime.EqualizeFixedDeltaTime(15);
		cosmosObjects = new HashSet<CosmosObject>();
		outerCosmosObjects = new HashSet<CosmosObject>();
		cosmosEnemies = new HashSet<CosmosEnemy>();
		cosmosObjectsToRemove = new List<CosmosObject>();
		cosmosObjects.Add(player);
		player.gameObject.tag = playerTag;
		player.onEvent += OnCosmosObjectEvent;
		area = boundariesContainer.GetArea();
		time = 0.0f;
		clock.Reset();
		spawnTime = updateRate;
		gameState = AsteroidsGameState.Gameplay;

		UIController.EnableGameOverView(false);
	}

	/// <summary>Callback invoked when scene loads, one frame before the first Update's tick.</summary>
	private void Start()
	{
		//this.StartCoroutine(Loop(), ref loop);
	}

	/// <summary>Callback invoked when CosmosAsteroidsGameplayController's instance is going to be destroyed and passed to the Garbage Collector.</summary>
	private void OnDestroy()
	{
		foreach(CosmosObject cosmosObject in cosmosObjects)
		{
			cosmosObject.onEvent -= OnCosmosObjectEvent;
		}
	}

	/// <summary>Updates CosmosAsteroidsGameplayController's instance at the end of each frame.</summary>
	private void LateUpdate()
	{
		switch(gameState)
		{
			case AsteroidsGameState.Gameplay:
				EvaluateSpawning();
				HandleInnerCosmosObjects();
				HandleOuterCosmosObjects();
				HandleEnemyAI();
				UpdateTime();
			break;
		}
	}

	/// <summary>Updates Time and Clock.</summary>
	private void UpdateTime()
	{
		float dt = Time.deltaTime;

		time += dt;
		clock.Update(dt);
		UIController.UpdateTime(clock.ToString());
		UIController.UpdateFPS(VTime.GetFrameRate());
	}

	/// <summary>Evaluates Spawning.</summary>
	private void EvaluateSpawning()
	{
		if(spawnTime < updateRate) spawnTime += Time.deltaTime;
		else
		{
			spawnTime = 0.0f;	
			OnSpawn();
		}
	}

	/// <summary>Handles CosmosObjects inside boundaries.</summary>
	private void HandleInnerCosmosObjects()
	{
		foreach(CosmosObject cosmosObject in cosmosObjects)
		{
			if(boundariesContainer.Outside(cosmosObject.transform.position))
			RepositionCosmosObject(cosmosObject);
		}
	}

	/// <summary>Handles CosmosObjects outside boundaries.</summary>
	private void HandleOuterCosmosObjects()
	{
		cosmosObjectsToRemove.Clear();

		foreach(CosmosObject cosmosObject in outerCosmosObjects)
		{
			if(!boundariesContainer.Inside(cosmosObject.transform.position)) continue;

			cosmosObjectsToRemove.Add(cosmosObject);
			cosmosObjects.Add(cosmosObject);
			cosmosObject.interactable = true;
			cosmosObject.EnableColliders(true);
		}

		foreach(CosmosObject cosmosObject in cosmosObjectsToRemove)
		{
			outerCosmosObjects.Remove(cosmosObject);
		}
	}

	/// <summary>Handler Enemy's AI.</summary>
	private void HandleEnemyAI()
	{
		foreach(CosmosEnemy enemy in cosmosEnemies)
		{
			Vector2 seekForce = Vector2.zero;
			Vector2 evasionForce = Vector2.zero;

			foreach(CosmosObject cosmosObject in cosmosObjects)
			{
				if(cosmosObject == enemy || cosmosObject == player) continue;

				Vector2 d = cosmosObject.position - enemy.position;

				if(d.sqrMagnitude < (3.0f * 3.0f)) evasionForce += enemy.vehicle.GetFleeForce(cosmosObject.position + cosmosObject.velocity);
			}

			seekForce = enemy.vehicle.GetSeekForce(player.position + player.velocity);

			enemy.vehicle.ApplyForce(seekForce + evasionForce);
			enemy.vehicle.Displace(Time.deltaTime);
		}
	}

	[OnInspectorGUI]
	/// <summary>Updates LineRenderer.</summary>
	private void UpdateLineRenderer()
	{
		if(Application.isPlaying) return;

		float z = (lineRenderer.transform.position - camera.transform.position).magnitude;
		VCameraViewportHandler.UpdateBoundaries(camera, z, boundariesContainer);

		IEnumerator<Vector2> iterator = boundariesContainer.IterateThroughCorners2D();
		int i = 0;
		lineRenderer.positionCount = 4;
		lineRenderer.loop = true;
	
		while(iterator.MoveNext())
		{
			lineRenderer.SetPosition(i, iterator.Current);
			i++;
		}
	}

	/// <summary>Repositions CosmosObject inside Boundaries.</summary>
	/// <param name="cosmosObject">CosmosObject to reposition.</param>
	private void RepositionCosmosObject(CosmosObject cosmosObject)
	{
		Vector2 p = cosmosObject.transform.position;
		Vector2 c = boundariesContainer.Clamp(p);
		Vector2 d = p - c;

		cosmosObject.transform.position = boundariesContainer.GetReciprocalInversePoint(p) + d;
	}

	/// <summary>Callbak internally invoked to spawn a new obstacle.</summary>
	private void OnSpawn()
	{
		SpawnRock();
	}

	/// <returns>Area covered by all CosmosObjects. Outer-CosmosObjects are also taken into account as a potential area calculation.</returns>
	private float GetAreaCoveredByCosmosObjects()
	{
		float a = 0.0f;

		if(outerCosmosObjects != null) foreach(CosmosObject cosmosObject in outerCosmosObjects)
		{
			a += cosmosObject.area;
		}

		if(cosmosObjects != null) foreach(CosmosObject cosmosObject in cosmosObjects)
		{
			a += cosmosObject.area;
		}

		return a;
	}

	/// <summary>Spawns Rock.</summary>
	private void SpawnRock()
	{
		float a = GetAreaCoveredByCosmosObjects();
		float timeRatio = (time / timeForMaxAreaCoveragePercentage);
		timeRatio = VMath.EaseInCubic(timeRatio);

		if(a > (area * areaCoveragePercentage.Lerp(timeRatio))) return;

		float t = Random.Range(0.0f, time / maxDifficultyTime);
		FloatRange radiusRange = FloatRange.Lerp(minimumRockRadiusRange, maximumRockRadiusRange, t);
		Vector2 p = boundariesContainer.RandomOutside(outsideBoundariesSpawnSpace.Min(), outsideBoundariesSpawnSpace.Max());
		CosmosRock rock = CosmosPoolManager.RequestCosmosRock(p, Quaternion.identity, radiusRange);
		Vector2 d = (Vector2)boundariesContainer.Random() - p;
		
		rock.movementAxes = d.normalized;
		rock.maxSpeed = rockSpeedRange.Random();
		rock.acceleration = rock.maxSpeed / (float)Application.targetFrameRate; /// So they reach max speed in 1 second.
		rock.onEvent += OnCosmosObjectEvent;

		OnCosmosObjectSpawned(rock);
	}

	/// <summary>Updates Score and the UI.</summary>
	/// <param name="additionalScore">Score to add.</param>
	/// <param name="_position">Position of the score feedback.</param>
	/// <param name="_camera">Camera's Reference [null by default].</param>
	private void UpdateScore(int additionalScore, Vector2 _position, Camera _camera = null)
	{
		score += additionalScore;
		UIController.UpdateScore(score, additionalScore, _position, _camera);
	}

	/// <summary>Callback invoked when a CosmosObject invokes an event.</summary>
	/// <param name="_ID">Event's ID.</param>
	/// <param name="_sender">Event's invoker.</param>
	/// <param name="_other">Other CosmosObjects that may be involved in the event.</param>
	private void OnCosmosObjectEvent(int _ID, CosmosObject _sender, params CosmosObject[] _others)
	{
		CosmosObject causer = _others != null && _others.Length > 0 ? _others[0] : null;
		GameObject senderObj = _sender.gameObject;
		GameObject causerObj = causer != null ? causer.gameObject : null;

		switch(_ID)
		{
			case CosmosObject.ID_EVENT_HIT:
				if(senderObj.CompareTag(rockTag))
				{
					if(causerObj != null && causerObj.CompareTag(playerProjectileTag))
					{
						CosmosProjectile projectile = causer as CosmosProjectile;

						if(projectile != null && projectile.shooter == player.gameObject)
						{
							Debug.Log("[CosmosAsteroidsGameplayController] Rock Destroyed, wii...Came from Player");

							float min = 0.0f;
							float radius = VMath.RadiusFromArea(_sender.area);
							int scalar = 1;

							foreach(KeyValuePair<float, int> pair in rockRadiusScoreScalarMap)
							{
								if(radius >= min && radius <= pair.Key)
								{
									scalar = pair.Value;
									break;
								}

								min = pair.Key;
							}

							UpdateScore(rockScore * scalar, _sender.transform.position, camera);
						}
					}
				
				} else if(senderObj.CompareTag(playerTag))
				{
					UIController.UpdatePlayerHPBar(player.health.hpRatio);
					this.StartCoroutine(camera.transform.ShakePosition(playerHitShakeAttributes.duration, playerHitShakeAttributes.speed, playerHitShakeAttributes.magnitude, OnCameraShakeEnds), ref cameraShake);
				}
			break;

			case CosmosObject.ID_EVENT_DESTROYED:
				_sender.onEvent -= OnCosmosObjectEvent;
				cosmosObjects.Remove(_sender);

				if(senderObj.CompareTag(playerTag))
				{
					UIController.EnableGameOverView(true);
					gameState = AsteroidsGameState.GameOver;
					this.StartCoroutine(camera.transform.ShakePosition(playerHitShakeAttributes.duration, playerHitShakeAttributes.speed, playerHitShakeAttributes.magnitude, OnCameraShakeEnds), ref cameraShake);
				}			
			break;

			case CosmosObject.ID_EVENT_SHOTPROJECTILE:
			case CosmosObject.ID_EVENT_CREATEDCOSMOSOBJECT:
				foreach(CosmosObject cosmosObject in _others)
				{
					cosmosObjects.Add(cosmosObject);
					cosmosObject.onEvent += OnCosmosObjectEvent;
				}
			break;
		}
	}

	/// <summary>Callback internally invoked when spawning a CosmosObject.</summary>
	/// <param name="cosmosObject">CosmosObject that was spawned.</param>
	private void OnCosmosObjectSpawned(CosmosObject cosmosObject)
	{
		outerCosmosObjects.Add(cosmosObject);
		cosmosObject.interactable = false;
		cosmosObject.EnableColliders(false);
	}

	/// <summary>Callback called when the Camera's Shaking ends.</summary>
	private void OnCameraShakeEnds()
	{
		this.DispatchCoroutine(ref cameraShake);
	}

	/// <returns>String represeting Gameplay's Info.</returns>
	public override string ToString()
	{
		StringBuilder builder = new StringBuilder();
		float t = Mathf.Clamp(time / timeForMaxAreaCoveragePercentage, 0.0f, 1.0f);
		t = VMath.EaseInCubic(t);
		float p = areaCoveragePercentage.Lerp(t);
		float a = GetAreaCoveredByCosmosObjects();

		builder.Append("Game's Area: ");
		builder.AppendLine(area.ToString());
		builder.Append("Time's Ratio: ");
		builder.AppendLine(t.ToString());
		builder.Append("Current Area Limit's Percentage: ");
		builder.AppendLine(p.ToString());
		builder.Append("Current Area's Limit: ");
		builder.AppendLine((area * p).ToString());
		builder.Append("Current Area Covered by CosmosObjects: ");
		builder.AppendLine(a.ToString());

		return builder.ToString();
	}

	/// <summary>Loop's Routine.</summary>
	private IEnumerator Loop()
	{
		SecondsDelayWait wait = new SecondsDelayWait(updateRate);

		while(true)
		{
			OnSpawn();

			while(wait.MoveNext()) yield return null;
			wait.Reset();
		}
	}
}
}