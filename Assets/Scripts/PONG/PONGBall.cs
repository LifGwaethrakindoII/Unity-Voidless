using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Voidless
{
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(TrailRenderer))]
public class PONGBall : MonoBehaviour
{
	[SerializeField] private float _speed; 						/// <summary>Ball's Speed.</summary>
	[SerializeField] private float _correctionAngle; 			/// <summary>Correction rotation's angle applied.</summary>
	[Space(5f)]
	[Header("Additional Forces:")]
	[SerializeField] private float _wallHitAdditionalForce; 	/// <summary>Additional force added when hitting a wall.</summary>
	[SerializeField] private float _paddleHitAdditionalForce; 	/// <summary>Additional force added when hitting a Paddle.</summary>
	[Space(5f)]
	[Header("Weights:")]
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _wallHitNormalWeight; 	/// <summary>Wall hit normal's weight.</summary>
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _paddleHitNormalWeight; 	/// <summary>Paddle hit normal's weight.</summary>
	[SerializeField]
	[Range(0.0f, 1.0f)] private float _paddleVelocityWeight; 	/// <summary>Paddle velocity's weight.</summary>
	[Space(5f)]
	[Header("Particle-Effects:")]
	[SerializeField] private ParticleEffect _wallHitEffect; 	/// <summary>Particle-Effect to emit when hitting a wall.</summary>
	[SerializeField] private ParticleEffect _paddleHitEffect; 	/// <summary>Particle-Effect to emit when hitting a paddle.</summary>
	[Space(5f)]
	[Header("SFXs:")]
	[SerializeField] private AudioClip _wallHitSFX; 			/// <summary>Wall-Hit's Sound-Effect.</summary>
	[SerializeField] private AudioClip _paddleHitSFX; 			/// <summary>Paddle-Hit's Sound-Effect.</summary>
	private Vector3 _velocity; 									/// <summary>Ball's Velocity.</summary>
	private float _additionalSpeed; 							/// <summary>Ball's Addittional Acceleration.</summary>
	private CharacterController _characterController; 			/// <summary>CharacterController's Component.</summary>
	private TrailRenderer _trailRenderer; 						/// <summary>TrailRenderer's Component.</summary>

	/// <summary>Gets speed property.</summary>
	public float speed { get { return _speed; } }

	/// <summary>Gets wallHitAdditionalForce property.</summary>
	public float wallHitAdditionalForce { get { return _wallHitAdditionalForce; } }

	/// <summary>Gets paddleHitAdditionalForce property.</summary>
	public float paddleHitAdditionalForce { get { return _paddleHitAdditionalForce; } }

	/// <summary>Gets correctionAngle property.</summary>
	public float correctionAngle { get { return _correctionAngle; } }

	/// <summary>Gets wallHitNormalWeight property.</summary>
	public float wallHitNormalWeight { get { return _wallHitNormalWeight; } }

	/// <summary>Gets paddleHitNormalWeight property.</summary>
	public float paddleHitNormalWeight { get { return _paddleHitNormalWeight; } }

	/// <summary>Gets paddleVelocityWeight property.</summary>
	public float paddleVelocityWeight { get { return _paddleVelocityWeight; } }

	/// <summary>Gets and Sets additionalSpeed property.</summary>
	public float additionalSpeed
	{
		get { return _additionalSpeed; }
		set { _additionalSpeed = value; }
	}

	/// <summary>Gets wallHitEffect property.</summary>
	public ParticleEffect wallHitEffect { get { return _wallHitEffect; } }

	/// <summary>Gets paddleHitEffect property.</summary>
	public ParticleEffect paddleHitEffect { get { return _paddleHitEffect; } }

	/// <summary>Gets wallHitSFX property.</summary>
	public AudioClip wallHitSFX { get { return _wallHitSFX; } }

	/// <summary>Gets paddleHitSFX property.</summary>
	public AudioClip paddleHitSFX { get { return _paddleHitSFX; } }

	/// <summary>Gets and Sets velocity property.</summary>
	public Vector3 velocity
	{
		get { return _velocity; }
		private set { _velocity = value; }
	}

	/// <summary>Gets characterController Component.</summary>
	public CharacterController characterController
	{ 
		get
		{
			if(_characterController == null) _characterController = GetComponent<CharacterController>();
			return _characterController;
		}
	}

	/// <summary>Gets trailRenderer Component.</summary>
	public TrailRenderer trailRenderer
	{ 
		get
		{
			if(_trailRenderer == null) _trailRenderer = GetComponent<TrailRenderer>();
			return _trailRenderer;
		}
	}

	/// <summary>PONGBall's instance initialization when loaded [Before scene loads].</summary>
	private void Awake()
	{
		characterController.enableOverlapRecovery = true;
	}

	/// <summary>Updates PONGBall's instance at each frame.</summary>
	private void Update()
	{
		characterController.Move(velocity * (speed + additionalSpeed) * Time.deltaTime);
	}

	/// <summary>Sets velocity and resets additional speed.</summary>
	public void SetVelocity(Vector3 v)
	{
		gameObject.SetActive(false);
		gameObject.SetActive(true);
		trailRenderer.enabled = false;
		trailRenderer.enabled = true;
		velocity = v.normalized;
		additionalSpeed = 0.0f;
		characterController.SetPosition(Vector3.zero);
	}

	/// <summary>Project the Ball N steps.</summary>
	/// <param name="steps">Number of Steps [1 by default].</param>
	public Vector3 Projection(float t, int steps = 1)
	{
		steps = Mathf.Max(steps, 1);

		Ray ray = default(Ray);

		ray.origin = transform.position;
		ray.direction = velocity;

		for(int i = 0; i < steps; i++)
		{
			RaycastHit hitInfo = default(RaycastHit);

			Debug.DrawRay(ray.origin, ray.direction * ((speed + additionalSpeed) * t), Color.magenta, t);

			if(Physics.Raycast(ray.origin, ray.direction, out hitInfo, Mathf.Infinity, PONGGame.LAYER_INTERACTABLES))
			{
				ray.origin = hitInfo.point;
				ray.direction = hitInfo.normal;
			}
			else return transform.position;
		}

		return ray.origin + (ray.direction * ((speed + additionalSpeed) * Time.deltaTime * t));
	}

	/// <summary>OnControllerColliderHit is called when the controller hits a collider while performing a Move.</summary>
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
    	if(hit.collider.isTrigger) return;

    	GameObject obj = hit.collider.gameObject;
    	ParticleEffect hitEffect = null;
    	Vector3 addVelocity = Vector3.zero;
    	AudioClip clip = null;
    	float addSpeed = 0.0f;

    	switch(obj.CompareTag(PONGGame.TAG_PLAYER))
	    {
	    	case true:
	    	{
	    		CharacterController paddleController = obj.GetComponent<CharacterController>();

	    		if(paddleController != null) addVelocity = paddleController.velocity * paddleVelocityWeight;
	    		addVelocity += (transform.position - hit.collider.transform.position).normalized * paddleHitNormalWeight;
	    		addSpeed = paddleHitAdditionalForce;
	    		velocity *= -1.0f;

	    		/// Apply correction rotation:
	    		if(Vector3.Dot(velocity, addVelocity) == 1.0f)
	    		addVelocity = addVelocity.Rotate(Random.Range(-correctionAngle, correctionAngle));

	    		clip = paddleHitSFX;
	    		hitEffect = paddleHitEffect;
	    		if(hitEffect != null) Instantiate(hitEffect, hit.point, Quaternion.identity);
	    	}
	    	break;

	    	case false:
	    	{
	    		addVelocity = hit.normal * wallHitNormalWeight;
	    		addSpeed = wallHitAdditionalForce;
	    		clip = wallHitSFX;
	    		hitEffect = wallHitEffect;
	    		if(hitEffect != null) Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
	    	}
	    	break;
	    }

    	additionalSpeed += (addSpeed * Time.deltaTime);
    	velocity = velocity.WithY(0.0f);
    	velocity += (addVelocity);
    	velocity = velocity.WithZ(0.0f);
    	velocity.Normalize();

    	if(clip != null) PONGGame.PlaySFX(clip);

    	Debug.DrawRay(hit.point, hit.normal * 5f, Color.magenta, 3f);
    }
}
}