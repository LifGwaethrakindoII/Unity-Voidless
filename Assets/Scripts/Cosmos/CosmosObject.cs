using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Voidless
{
public enum Collider2DType
{
	Box,
	Circle,
	Polygon,
}

/// <summary>Event invoked when a CosmosObject invokes an event.</summary>
/// <param name="_ID">Event's ID.</param>
/// <param name="_sender">Event's invoker.</param>
/// <param name="_other">Other CosmosObjects that may be involved in the event.</param>
public delegate void OnCosmosObjectEvent(int _ID, CosmosObject _sender, params CosmosObject[] _others);

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DisplacementAccumulator2D))]
public class CosmosObject : PoolGameObject
{
	public const int ID_EVENT_HIT = 1 << 0;
	public const int ID_EVENT_DESTROYED = 1 << 1;
	public const int ID_EVENT_SHOTPROJECTILE = 1 << 2;
	public const int ID_EVENT_CREATEDCOSMOSOBJECT = 1 << 3;

	public event OnCosmosObjectEvent onEvent; 																		/// <summary>OnCosmosObjectEvent's delegate.</summary>

	[Header("Displacement's Attributes:")]
	[InfoBox("@ToString()")]
	[SerializeField] private float _acceleration; 																	/// <summary>Acceleration's Magnitude.</summary>
	[SerializeField] private float _maxSpeed; 																		/// <summary>Maximum's Speed.</summary>
	[Space(5f)]
	[Header("Rotation's Attributes:")]
	[SerializeField] private bool _rotateTowardsVelocity; 															/// <summary>Rotate Towards Velocity?.</summary>
	[SerializeField] private float _rotationSpeed; 																	/// <summary>Rotation's Speed.</summary>
	[SerializeField][HideInInspector]private float _area; 															/// <summary>CosmosObject's Area.</summary>
	[SerializeField][HideInInspector]private float _density; 														/// <summary>CosmosObject's Density.</summary>
	[Space(5f)]
	[Header("Colliders:")]
	[TabGroup("Components")][SerializeField] private PolygonCollider2D _polygonCollider; 							/// <summary>PolygonCollider's Component.</summary>
	[TabGroup("Components")][SerializeField] private CircleCollider2D _circleCollider; 								/// <summary>CircleCollider's Component.</summary>
	[Space(5f)]
	[Header("Visual Feedback:")]
	[TabGroup("Components")][SerializeField] private Renderer[] _renderers; 										/// <summary>CosmosObhject's Renderers.</summary>
	[TabGroup("Components")][SerializeField] private LineRenderer _lineRenderer; 									/// <summary>LineRenderer's Component.</summary>
	[Space(5f)]
	[Header("Tags:")]
	[SerializeField] private GameObjectTag[] _impactTags; 															/// <summary>Impact's Tags.</summary>
	[Space(5f)]
	[Header("Particle-Effects:")]
	[TabGroup("FXsGroup", "Particle-Effects")][SerializeField] private ParticleEffect _hitParticleEffect; 			/// <summary>Particle-Effect to emit when hit.</summary>
	[TabGroup("FXsGroup", "Particle-Effects")][SerializeField] private ParticleEffect _destructionParticleEffect; 	/// <summary>Particle-Effect to emit when destroyed.</summary>
	private Vector2  _velocity; 																					/// <summary>Velocity.</summary>
	private Vector2 _center; 																						/// <summary>Rock's Center.</summary>
	private bool _interactable; 																					/// <summary>Is This CosmosObject Physically interactable?.</summary>
	private Rigidbody2D _rigidbody; 																				/// <summary>Rigidbody2D's Component.</summary>
	private DisplacementAccumulator2D _displacementAccumulator; 													/// <summary>DisplacementAccumulator2D's Component.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets acceleration property.</summary>
	public float acceleration
	{
		get { return _acceleration; }
		set { _acceleration = value; }
	}

	/// <summary>Gets and Sets maxSpeed property.</summary>
	public float maxSpeed
	{
		get { return _maxSpeed; }
		set { _maxSpeed = value; }
	}

	/// <summary>Gets and Sets rotationSpeed property.</summary>
	public float rotationSpeed
	{
		get { return _rotationSpeed; }
		set { _rotationSpeed = value; }
	}

	/// <summary>Gets and Sets area property.</summary>
	public float area
	{
		get { return _area; }
		protected set
		{
			_area = value;
			density = rigidbody.mass / area;
		}
	}

	/// <summary>Gets and Sets density property.</summary>
	public float density
	{
		get { return _density; }
		private set { _density = value; }
	}

	/// <summary>Gets and Sets rotateTowardsVelocity property.</summary>
	public bool rotateTowardsVelocity
	{
		get { return _rotateTowardsVelocity; }
		set { _rotateTowardsVelocity = value; }
	}

	/// <summary>Gets position property.</summary>
	public Vector2 position { get { return new Vector2(transform.position.x, transform.position.y); } }

	/// <summary>Gets and Sets velocity property.</summary>
	public Vector2 velocity
	{
		get { return _velocity; }
		protected set { _velocity = value; }
	}

	/// <summary>Gets and Sets center property.</summary>
	public Vector2 center
	{
		get { return _center; }
		private set { _center = value; }
	}

	/// <summary>Gets and Sets interactable property.</summary>
	public bool interactable
	{
		get { return _interactable; }
		set { _interactable = value; }
	}

	/// <summary>Gets rigidbody Component.</summary>
	public Rigidbody2D rigidbody
	{ 
		get
		{
			if(_rigidbody == null) _rigidbody = GetComponent<Rigidbody2D>();
			return _rigidbody;
		}
	}

	/// <summary>Gets displacementAccumulator Component.</summary>
	public DisplacementAccumulator2D displacementAccumulator
	{ 
		get
		{
			if(_displacementAccumulator == null) _displacementAccumulator = GetComponent<DisplacementAccumulator2D>();
			return _displacementAccumulator;
		}
	}

	/// <summary>Gets polygonCollider property.</summary>
	public PolygonCollider2D polygonCollider { get { return _polygonCollider; } }

	/// <summary>Gets circleCollider property.</summary>
	public CircleCollider2D circleCollider { get { return _circleCollider; } }

	/// <summary>Gets and Sets renderers property.</summary>
	public Renderer[] renderers
	{
		get { return _renderers; }
		private set { _renderers = value; }
	}

	/// <summary>Gets lineRenderer property.</summary>
	public LineRenderer lineRenderer { get { return _lineRenderer; } }

	/// <summary>Gets and Sets impactTags property.</summary>
	public GameObjectTag[] impactTags
	{
		get { return _impactTags; }
		set { _impactTags = value; }
	}

	/// <summary>Gets hitParticleEffect property.</summary>
	public ParticleEffect hitParticleEffect { get { return _hitParticleEffect; } }

	/// <summary>Gets destructionParticleEffect property.</summary>
	public ParticleEffect destructionParticleEffect { get { return _destructionParticleEffect; } }
#endregion

	/// <summary>Resets CosmosObject's instance to its default values.</summary>
	protected virtual void Reset()
	{
		rigidbody.gravityScale = 0.0f;
	}

	/// <summary>CosmosObject's instance initialization.</summary>
	protected virtual void Awake()
	{
		interactable = true;
		CalculateAreaAndDensity();
	}

	/// <summary>CosmosObject's starting actions before 1st Update frame.</summary>
	protected virtual void Start() { /*...*/ }

	/// <summary>Callback invoked when CosmosObject's instance is going to be destroyed and passed to the Garbage Collector.</summary>
	protected virtual void OnDestroy() { /*...*/ }
	
	/// <summary>CosmosObject's tick at each frame.</summary>
	protected virtual void Update()
	{
		UpdateLineRenderer();
	}

	/// <summary>Updates CosmosObject's instance at each Physics Thread's frame.</summary>
	protected virtual void FixedUpdate()
	{
		if(rotateTowardsVelocity && velocity.sqrMagnitude > 0.0f)
		transform.rotation = Quaternion.RotateTowards(transform.rotation, VQuaternion.RightLookRotation(velocity), rotationSpeed * Time.fixedDeltaTime);
	}

	/// <summary>Calculates Area and Density.</summary>
	protected virtual void CalculateAreaAndDensity()
	{
		if(polygonCollider != null)
		{
			center = polygonCollider.GetCenter();
			area = polygonCollider.GetArea();

		} else if(circleCollider != null)
		{
			center = (Vector2)transform.position + circleCollider.offset;
			area = VMath.AreaOfCircle(circleCollider.radius);
		}
		else
		{
			Debug.LogError("[CosmosObject] This CosmosObject does not have any collider referenced. Center, Area and Density will be set to their respective defaults.");
			center = Vector2.zero;
			area = 0.0f;
		}
	}

	/// <summary>Event triggered when this Collider/Rigidbody begun having contact with another Collider/Rigidbody.</summary>
	/// <param name="col">The Collision data associated with this collision Event.</param>
	protected virtual void OnCollisionEnter2D(Collision2D col)
	{
		if(!interactable) return;

		GameObject obj = col.gameObject;

		GameObjectTag.DoIfGameObjectTagMatches(obj, impactTags, ()=>
		{
			CosmosObject cosmosObject = obj.GetComponentHereOrInParent<CosmosObject>();

			if(cosmosObject != null) OnImpact(cosmosObject);
		});
	}

	/// <summary>Event triggered when this Collider enters another Collider trigger.</summary>
	/// <param name="col">The other Collider involved in this Event.</param>
	protected virtual void OnTriggerEnter2D(Collider2D col)
	{
		if(!interactable) return;

		GameObject obj = col.gameObject;

		GameObjectTag.DoIfGameObjectTagMatches(obj, impactTags, ()=>
		{
			CosmosObject cosmosObject = obj.GetComponentHereOrInParent<CosmosObject>();

			if(cosmosObject != null) OnImpact(cosmosObject);
		});
	}

	/// <summary>Callback internally invoked when impacting.</summary>
	/// <param name="_cosmosObject">CosmosObject that impacted with this CosmosObject.</param>
	protected virtual void OnImpact(CosmosObject _cosmosObject) { /*...*/ }

	/// <summary>Actions made when this Pool Object is being reseted.</summary>
	public override void OnObjectReset()
	{
		base.OnObjectReset();
		ResetObject();
	}

	/// <summary>Resets CosmosObject [Different than MonoBehaviour's Reset()].</summary>
	public virtual void ResetObject()
	{
		velocity = Vector2.zero;
	}

	/// <summary>Invokes OnCosmosObjectEvent.</summary>
	/// <param name="_ID">Event's ID.</param>
	/// <param name="_cosmosObjects">Other CosmosObjects [besides this] that may ve involved in the event.</param>
	public void InvokeEvent(int _ID, params CosmosObject[] _cosmosObjects)
	{
		if(onEvent != null) onEvent(_ID, this, _cosmosObjects);
	}

	[Button("Get Renderers")]
	/// <summary>Gets Renderers.</summary>
	private void GetRenderers()
	{
		renderers = GetComponentsInChildren<Renderer>();
	}

	[OnInspectorGUI]
	/// <summary>Updates LineRenderer to match with PolygonCollider2D's points.</summary>
	private void UpdateLineRenderer()
	{
		if(lineRenderer  == null) return;

		Vector2[] points = polygonCollider.GetPath(0);
		Vector2 offset = polygonCollider.offset;
		int length = points.Length;
		lineRenderer.positionCount = length + 1;

		for(int i = 0; i < length; i++)
		{
			lineRenderer.SetPosition(i, transform.position + (transform.rotation * (offset + points[i])));
		}

		lineRenderer.SetPosition(length, transform.position + (transform.rotation * (offset + points[0])));
	}

	/// <summary>Enables Colliders.</summary>
	/// <param name="_enable">Enable? true by default.</param>
	public void EnableColliders(bool _enable = true)
	{
		if(polygonCollider != null) polygonCollider.enabled = _enable;
		if(circleCollider != null) circleCollider.enabled = _enable;
	}

	/// <summary>Displaces CosmosObject with its own velocity.</summary>
	/// <param name="axes">Displacement's Axes.</param>
	public void Move(Vector2 axes)
	{
		velocity += axes.normalized * (acceleration * Time.deltaTime);

		if(velocity.sqrMagnitude > (maxSpeed * maxSpeed)) velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

		displacementAccumulator.AddDisplacement(velocity);
	}

	/// <summary>Adds External Force.</summary>
	/// <param name="force">External Force.</param>
	public void AddExternalForce(Vector2 force)
	{
		displacementAccumulator.AddDisplacement(force);
	}

	/// <summary>Calculates resulting velocity of 2 colliding CosmosObjects.</summary>
	/// <param name="a">CosmosObject A.</param>
	/// <param name="b">CosmosObject B.</param>
	/// <returns>Resulting velocity for each CosmosObject [as a Vector2 Tuple].</returns>
	public static ValueVTuple<Vector2, Vector2> CollisionResolution(CosmosObject a, CosmosObject b)
	{
		return VPhysics2D.CollisionResolution(a.velocity, b.velocity, a.rigidbody.mass, b.rigidbody.mass);
	}

	/// <returns>String representing this CosmosObject.</returns>
	public override string ToString()
	{
		StringBuilder builder = new StringBuilder();

		builder.Append("Velocity: ");
		builder.AppendLine(velocity.ToString());
		builder.Append("Velocity's Magnitude: ");
		builder.AppendLine(velocity.magnitude.ToString());
		builder.Append("Area: ");
		builder.AppendLine(area.ToString());
		builder.Append("Density: ");
		builder.Append(density.ToString());

		return builder.ToString();
	}
}
}