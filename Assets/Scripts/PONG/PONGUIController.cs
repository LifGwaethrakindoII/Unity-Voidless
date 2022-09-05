using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Voidless
{
public class PONGUIController : Singleton<PONGUIController>
{
	[Header("Gameplay UI:")]
	[SerializeField] private Text _scoreA; 	/// <summary>Score for Paddle A [Left].</summary>
	[SerializeField] private Text _scoreB; 	/// <summary>Score for Paddle B [Right].</summary>

	/// <summary>Gets scoreA property.</summary>
	public Text scoreA { get { return _scoreA; } }

	/// <summary>Gets scoreB property.</summary>
	public Text scoreB { get { return _scoreB; } }

	/// <summary>Updates Score.</summary>
	/// <param name="sA">Score A.</param>
	/// <param name="sB">Score B.</param>
	public void UpdateScore(int sA, int sB)
	{
		scoreA.text = sA.ToString();
		scoreB.text = sB.ToString();
	}
}
}