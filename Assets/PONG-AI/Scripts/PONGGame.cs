using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
[RequireComponent(typeof(AudioSource))]
public class PONGGame : Singleton<PONGGame>
{
	public const string TAG_PLAYER = "Player"; 					/// <summary>Player's Tag.</summary>
	public const string TAG_BALL = "Ball"; 						/// <summary>Ball's Tag.</summary>

	public const int LAYER_INTERACTABLES = 1 << 1; 				/// <summary>Interactables Layer.</summary>
	public const int LAYER_BALL = 1 << 2; 						/// <summary>Ball's Layer.</summary>

	[Space(5f)]
	[Header("Game's Actors:")]
	[SerializeField] private PONGPaddle _paddleA; 				/// <summary>Paddle A.</summary>
	[SerializeField] private PONGPaddle _paddleB; 				/// <summary>Paddle B.</summary>
	[SerializeField] private PONGBall _ball; 					/// <summary>Ball.</summary>
	[Space(5f)]
	[Header("Scenario's Zones:")]
	[SerializeField] private Collider _upperWall; 				/// <summary>Upper Wall's Collider.</summary>
	[SerializeField] private Collider _lowerWall; 				/// <summary>Lower Wall's Collider.</summary>
	[SerializeField] private HitCollider _goalZoneA; 			/// <summary>Goal Zone A [Left].</summary>
	[SerializeField] private HitCollider _goalZoneB; 			/// <summary>Goal Zone B [Right].</summary>
	[Space(5f)]
	[Header("UI:")]
	[SerializeField] private PONGUIController _UIController; 	/// <summary>UI's Controller.</summary>
	[Space(5f)]
	[Header("SFXs:")]
	[SerializeField] private AudioClip _scoreSFX; 				/// <summary>Score's Sound-Effect.</summary>
	private int _scoreA; 										/// <summary>Score for Paddle A.</summary>
	private int _scoreB; 										/// <summary>Score for Paddle B.</summary>
	private AudioSource _audioSource; 							/// <summary>AudioSource's Component.</summary>

	/// <summary>Gets paddleA property.</summary>
	public static PONGPaddle paddleA { get { return Instance._paddleA; } }

	/// <summary>Gets paddleB property.</summary>
	public static PONGPaddle paddleB { get { return Instance._paddleB; } }

	/// <summary>Gets ball property.</summary>
	public static PONGBall ball { get { return Instance._ball; } }

	/// <summary>Gets upperWall property.</summary>
	public Collider upperWall { get { return _upperWall; } }

	/// <summary>Gets lowerWall property.</summary>
	public Collider lowerWall { get { return _lowerWall; } }

	/// <summary>Gets goalZoneA property.</summary>
	public HitCollider goalZoneA { get { return _goalZoneA; } }

	/// <summary>Gets goalZoneB property.</summary>
	public HitCollider goalZoneB { get { return _goalZoneB; } }

	/// <summary>Gets UIController property.</summary>
	public PONGUIController UIController { get { return _UIController; } }

	/// <summary>Gets scoreSFX property.</summary>
	public AudioClip scoreSFX { get { return _scoreSFX; } }

	/// <summary>Gets and Sets scoreA property.</summary>
	public int scoreA
	{
		get { return _scoreA; }
		private  set { _scoreA = value; }
	}

	/// <summary>Gets and Sets scoreB property.</summary>
	public int scoreB
	{
		get { return _scoreB; }
		private set { _scoreB = value; }
	}

	/// <summary>Gets audioSource Component.</summary>
	public AudioSource audioSource
	{ 
		get
		{
			if(_audioSource == null) _audioSource = GetComponent<AudioSource>();
			return _audioSource;
		}
	}

	/// <summary>PONGGame's instance initialization.</summary>
	protected override void OnAwake()
	{
		goalZoneA.onTriggerEvent += OnGoalZoneATriggerEvent;
		goalZoneB.onTriggerEvent += OnGoalZoneBTriggerEvent;
		scoreA = 0;
		scoreB = 0;
		UIController.UpdateScore(scoreA, scoreB);
		ResetBall(Vector3.left);

		paddleA.gameObject.layer = LAYER_INTERACTABLES;
		paddleB.gameObject.layer = LAYER_INTERACTABLES;
		upperWall.gameObject.layer = LAYER_INTERACTABLES;
		lowerWall.gameObject.layer = LAYER_INTERACTABLES;
		ball.gameObject.layer = LAYER_BALL;
	}

	/// <summary>PONGGame's starting actions before 1st Update frame.</summary>
	private void Start ()
	{
		
	}
	
	/// <summary>PONGGame's tick at each frame.</summary>
	private void Update ()
	{
		
	}

	/// <summary>Callback invoked when PONGGame's instance is going to be destroyed and passed to the Garbage Collector.</summary>
	private void OnDestroy()
	{
		goalZoneA.onTriggerEvent -= OnGoalZoneATriggerEvent;
		goalZoneB.onTriggerEvent -= OnGoalZoneBTriggerEvent;
	}

	/// <summary>Resets Paddles.</summary>
	private void ResetPaddles()
	{
		paddleA.transform.position = paddleA.transform.position.WithY(0.0f);
		paddleB.transform.position = paddleB.transform.position.WithY(0.0f);
	}

	/// <summary>Resets Ball.</summary>
	/// <param name="v">Ball's new velolcity.</param>
	private void ResetBall(Vector3 v)
	{
		ball.SetVelocity(v);
	}

	/// <summary>Plays Sound-Effect's Clip.</summary>
	/// <param name="_clip">AudioClip's reference.</param>
	public static void PlaySFX(AudioClip _clip)
	{
		if(_clip != null) Instance.audioSource.PlaySound(_clip);
	}

	/// <summary>Callback Invoked when Goal-Zone A Triggers with another Collider.</summary>
	/// <param name="_collider">Collider involved on the Trigger Event.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_ID">Optional ID of the HitCollider2D.</param>
	public void OnGoalZoneATriggerEvent(Collider _collider, HitColliderEventTypes _eventType, int _ID = 0)
	{
		switch(_eventType)
		{
			case HitColliderEventTypes.Enter:
				if(!_collider.gameObject.CompareTag(TAG_BALL)) return;

				scoreB++;
				UIController.UpdateScore(scoreA, scoreB);
				ResetBall(Vector3.right);
				PlaySFX(scoreSFX);
			break;
		}
	}

	/// <summary>Callback Invoked when Goal-Zone B Triggers with another Collider.</summary>
	/// <param name="_collider">Collider involved on the Trigger Event.</param>
	/// <param name="_eventType">Type of the event.</param>
	/// <param name="_ID">Optional ID of the HitCollider2D.</param>
	public void OnGoalZoneBTriggerEvent(Collider _collider, HitColliderEventTypes _eventType, int _ID = 0)
	{
		switch(_eventType)
		{
			case HitColliderEventTypes.Enter:
				if(!_collider.gameObject.CompareTag(TAG_BALL)) return;

				scoreA++;
				UIController.UpdateScore(scoreA, scoreB);
				ResetBall(Vector3.left);
				PlaySFX(scoreSFX);
			break;
		}
	}
}
}