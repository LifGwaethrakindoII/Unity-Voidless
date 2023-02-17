using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using Random = UnityEngine.Random;

namespace Voidless
{
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(LineRenderer))]
public class CosmosRock : CosmosObject
{
	[Space(5f)]
	[Header("Rock's Attributes:")]
	[SerializeField] private IntRange _segmentsRange; 						/// <summary>Segment's Range.</summary>
	[SerializeField] private FloatRange _radiusRange; 						/// <summary>Radius' Range.</summary>
	[SerializeField] private float _minimumDivisionArea; 					/// <summary>Minimum Area the rock must have to be divided.</summary>
	[SerializeField][Range(0.0f, 1.0f)] private float _divisionCoeficient; 	/// <summary>How much the rock is divided.</summary>
	private Vector2 _movementAxes; 											/// <summary>Movement's Axes.</summary>

#region Getters/Setters:
	/// <summary>Gets and Sets segmentsRange property.</summary>
	public IntRange segmentsRange
	{
		get { return _segmentsRange; }
		set { _segmentsRange = value; }
	}

	/// <summary>Gets and Sets radiusRange property.</summary>
	public FloatRange radiusRange
	{
		get { return _radiusRange; }
		set { _radiusRange = value; }
	}

	/// <summary>Gets minimumDivisionArea property.</summary>
	public float minimumDivisionArea { get { return _minimumDivisionArea; } }

	/// <summary>Gets divisionCoeficient property.</summary>
	public float divisionCoeficient { get { return _divisionCoeficient; } }

	/// <summary>Gets and Sets movementAxes property.</summary>
	public Vector2 movementAxes
	{
		get { return _movementAxes; }
		set { _movementAxes = value; }
	}
#endregion

	/// <summary>Draws Gizmos on Editor mode when CosmosRock's instance is selected.</summary>
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawRay(transform.position + (transform.rotation * center), Vector3.forward);
	}

	/// <summary>CosmosRock's instance initialization when loaded [Before scene loads].</summary>
	protected override void Awake()
	{
		base.Awake();
		lineRenderer.enabled = false;
	}

	/// <summary>CosmosObject's tick at each frame.</summary>
	protected override void Update()
	{
		base.Update();
		Move(movementAxes);
	}

	/// <summary>Event triggered when this Collider/Rigidbody begun having contact with another Collider/Rigidbody.</summary>
	/// <param name="col">The Collision data associated with this collision Event.</param>
	protected override void OnCollisionEnter2D(Collision2D col)
	{
		base.OnCollisionEnter2D(col);

		if(!interactable) return;

		foreach(ContactPoint2D contact in col.contacts)
		{
			CosmosPoolManager.RequestParticleEffect(hitParticleEffect, contact.point, contact.normal);
		}
	}

	/// <summary>Event triggered when this Collider enters another Collider trigger.</summary>
	/// <param name="col">The other Collider involved in this Event.</param>
	protected override void OnTriggerEnter2D(Collider2D col)
	{
		base.OnTriggerEnter2D(col);

		if(!interactable) return;

		GameObject obj = col.gameObject;
		CosmosObject cosmosObject = obj.GetComponentHereOrInParent<CosmosObject>();

		if(cosmosObject != null)
		{
			ValueVTuple<Vector2, Vector2> collisionResolution = CollisionResolution(this, cosmosObject);
			rigidbody.velocity = collisionResolution.Item1;
		}
	}

	/// <summary>Independent Actions made when this Pool Object is being created.</summary>
	public override void OnObjectCreation()
	{
		lineRenderer.enabled = false;
		base.OnObjectCreation();
	}

	/// <summary>Actions made when this Pool Object is being reseted.</summary>
	public override void OnObjectReset()
	{
		base.OnObjectReset();
		lineRenderer.enabled = true;
	}

	[Button("Generate Random Rock")]
	/// <summary>Generates Random Rock.</summary>
	public void GenerateRandomRock()
	{
		int length = segmentsRange.Random();
		float l = (float)length;
		float[] angles = new float[length];
		Vector2[] octaves = new Vector2[length];
		Vector2[] points = new Vector2[length];
		float sum = 0.0f;
		float divisions = 360.0f / l;
		float min = radiusRange.Min();
		float max = radiusRange.Max();

		for(int i = 0; i < length; i++)
		{
			float x = Random.Range(i * divisions, (i + 1.0f) * divisions);
			sum += x;
			angles[i] = x;
			octaves[i] = new Vector2(
				radiusRange.Random(),
				radiusRange.Random()
			);
		}

		for(int i = 0; i < length; i++)
		{
			Vector3 n = Vector2.right.Rotate(angles[i]);
			Vector2 o = octaves[i];
			float r = radiusRange.Random();
			float iR = 1.0f / r;
			float x = o.x * iR;
			float y = o.y * iR;

			n *= (r + Mathf.PerlinNoise(x, y));
			points[i] = n;
		}

		polygonCollider.SetPath(0, points);
		CalculateAreaAndDensity();
	}

	/// <summary>Callback internally invoked when impacting.</summary>
	/// <param name="_cosmosObject">CosmosObject that impacted with this CosmosObject.</param>
	protected override void OnImpact(CosmosObject _cosmosObject)
	{
		Vector3 origin = _cosmosObject.transform.position;
		Vector2 direction = transform.position - origin;
		RaycastHit2D[] hitsInfo = null;
		float initialDistance = 0.1f;
		float augment = 0.5f;
		float limit = 3.0f;
		float radius = initialDistance;

		while(radius < limit)
		{
			hitsInfo = Physics2D.CircleCastAll(origin, radius, direction, radius);

			if(hitsInfo != null)
			{
				foreach(RaycastHit2D hitInfo in hitsInfo)
				{
					if(hitInfo.transform != transform) continue;

					Debug.DrawRay(hitInfo.point, hitInfo.normal * 5.0f, Color.magenta, 3.0f);

					Vector2 force = _cosmosObject.velocity * _cosmosObject.rigidbody.mass; // f = m * a
					float a = area;
					float r = VMath.RadiusFromArea(a);
					FloatRange range = radiusRange;
					ParticleEffect particleEffect = null;
					Vector2 hitDirection = Vector2.zero;

					radiusRange = range * divisionCoeficient;

					if(a < minimumDivisionArea)
					{
						OnObjectDeactivation();
						InvokeEvent(ID_EVENT_HIT, _cosmosObject);
						InvokeEvent(ID_EVENT_DESTROYED, _cosmosObject);
						particleEffect = destructionParticleEffect;
					}
					else
					{
						FloatRange duplicateRadiusRange = range - radiusRange;
						CosmosRock duplicate = CosmosPoolManager.RequestCosmosRock(hitInfo.point, Quaternion.identity, duplicateRadiusRange);
						duplicate.velocity = hitInfo.normal * force.magnitude;
						InvokeEvent(ID_EVENT_HIT, _cosmosObject);
						InvokeEvent(ID_EVENT_CREATEDCOSMOSOBJECT, duplicate);
						particleEffect = hitParticleEffect;
						hitDirection = hitInfo.normal;
					}

					GenerateRandomRock();
					CosmosPoolManager.RequestParticleEffect(particleEffect, hitInfo.point, hitDirection);

					return;
				}
			}

			radius += augment;
		}
	}

	/// <returns>String representing this CosmosRock.</returns>
	public override string ToString()
	{
		StringBuilder builder = new StringBuilder();

		builder.AppendLine(base.ToString());

		return builder.ToString();
	}
}
}