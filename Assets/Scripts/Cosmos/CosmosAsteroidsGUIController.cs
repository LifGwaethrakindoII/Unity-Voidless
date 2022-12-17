using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;

using Random = UnityEngine.Random;

namespace Voidless
{
public class CosmosAsteroidsGUIController : MonoBehaviour
{
	[Header("Score Settings:")]
	[SerializeField] private string _scoreFormat; 			/// <summary>Score's Text Format.</summary>
	[SerializeField] private float _scoreFeedbackDuration; 	/// <summary>Score Feedback's Duration.</summary>
	[SerializeField] private float _scoreFeedbackSpeed; 	/// <summary>Score Feedback's Displacement Speed.</summary>
	[Space(5f)]
	[Header("Panels:")]
	[SerializeField] private GameObject _overlayCanvas; 	/// <summary>Overlay Canvas' GameObject.</summary>
	[SerializeField] private GameObject _gameOverPanel; 	/// <summary>Game Over Screen's Panel.</summary>
	[Space(5f)]
	[Header("UI Elements:")]
	[SerializeField] private Button _replayButton; 			/// <summary>Replay's Button.</summary>
	[SerializeField] private Button _quitButton; 			/// <summary>Quit's Button.</summary>
	[SerializeField] private Text _scoreAmountText; 		/// <summary>Score Amount's Text.</summary>
	[SerializeField] private Text _timeAmountText; 			/// <summary>Time Amount's Text.</summary>
	[SerializeField] private Text _FPSAmountText; 			/// <summary>FPS Amount's Text.</summary>
	[SerializeField] private BarUI _playerHPBar; 			/// <summary>Player's HP Bar.</summary>

#region Getters/Setters:
	/// <summary>Gets scoreFormat property.</summary>
	public string scoreFormat { get { return _scoreFormat; } }

	/// <summary>Gets scoreFeedbackDuration property.</summary>
	public float scoreFeedbackDuration { get { return _scoreFeedbackDuration; } }

	/// <summary>Gets scoreFeedbackSpeed property.</summary>
	public float scoreFeedbackSpeed { get { return _scoreFeedbackSpeed; } }

	/// <summary>Gets overlayCanvas property.</summary>
	public GameObject overlayCanvas { get { return _overlayCanvas; } }

	/// <summary>Gets gameOverPanel property.</summary>
	public GameObject gameOverPanel { get { return _gameOverPanel; } }

	/// <summary>Gets replayButton property.</summary>
	public Button replayButton { get { return _replayButton; } }

	/// <summary>Gets quitButton property.</summary>
	public Button quitButton { get { return _quitButton; } }

	/// <summary>Gets scoreAmountText property.</summary>
	public Text scoreAmountText { get { return _scoreAmountText; } }

	/// <summary>Gets timeAmountText property.</summary>
	public Text timeAmountText { get { return _timeAmountText; } }

	/// <summary>Gets FPSAmountText property.</summary>
	public Text FPSAmountText { get { return _FPSAmountText; } }

	/// <summary>Gets playerHPBar property.</summary>
	public BarUI playerHPBar { get { return _playerHPBar; } }
#endregion

	/// <summary>Updates Score to the UI.</summary>
	/// <param name="score">Score's Amount.</param>
	/// <param name="_scoreObtained">Score obtained.</param>
	/// <param name="_position">Position of the score feedback.</param>
	/// <param name="_camera">Camera's Reference [null by default].</param>
	public void UpdateScore(int _score, int _scoreObtained, Vector2 _position, Camera _camera = null)
	{
		scoreAmountText.text = _score.ToString(scoreFormat);

		if(_camera != null) StartCoroutine(ScoreObtainedFeedback(_scoreObtained, _position, _camera));
	}

	/// <summary>Updates Time to the UI.</summary>
	/// <param name="time">Time's Amount.</param>
	public void UpdateTime(string timeText)
	{
		timeAmountText.text = timeText;
	}

	/// <summary>Updates Player's HP Bar.</summary>
	public void UpdatePlayerHPBar(float t)
	{
		playerHPBar.UpdateBar(t);
	}

	/// <summary>Updates FPS to the UI.</summary>
	/// <param name="fps">FPS' Amount.</param>
	public void UpdateFPS(int fps)
	{
		FPSAmountText.text = fps.ToString();
	}

	/// <summary>Enables Game Over's Panel.</summary>
	/// <param name="_enable">Enable? true by default.</param>
	public void EnableGameOverView(bool _enable = true)
	{
		gameOverPanel.SetActive(_enable);
		EventSystem.current.SetSelectedGameObject(_enable ? replayButton.gameObject : null);
	}

	/// <summary>Score Feedback's Routine.</summary>
	/// <param name="score">Score Obtained's Amount.</param>
	/// <param name="_position">Position of the score feedback.</param>
	/// <param name="_camera">Camera's Reference [null by default].</param>
	private IEnumerator ScoreObtainedFeedback(int _score, Vector2 _position, Camera _camera)
	{
		PoolUIElement UIElement = CosmosPoolManager.RequestPoolText(_camera.WorldToScreenPoint(_position), "+" + _score.ToString());

		if(UIElement == null) yield break;

		Color a = Color.white;
		Color b = a.WithAlpha(0.0f);
		float inverseDuration = 1.0f / scoreFeedbackDuration;
		float t = 0.0f;

		UIElement.rectTransform.parent = overlayCanvas.transform;
		Text text = UIElement.GetComponent<Text>();

		while(t < 1.0f)
		{
			UIElement.rectTransform.position += (Vector3.up * scoreFeedbackSpeed * Time.deltaTime);
			text.color = Color.Lerp(a, b, t);
			t += (Time.deltaTime * inverseDuration);
			yield return null;
		}

		text.color = b;
		UIElement.OnObjectDeactivation();
	}
}
}