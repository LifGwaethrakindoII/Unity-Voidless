using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
public enum AIControlMode
{
	Scripted,
	NeuralNetworks
}

public class PONGPaddleAIController : MonoBehaviour
{
	[SerializeField] private AIControlMode _controlMode; 	/// <summary>AI's Control Mode.</summary>
	[SerializeField] private PONGPaddle _paddle; 			/// <summary>Paddle that this controller moves.</summary>
	[SerializeField] private float _updateRate; 			/// <summary>Update's Rate.</summary>
	private float _time; 									/// <summary>Current's Update Time.</summary>
	private Vector3 _ballProjection; 						/// <summary>Ball's Projection.</summary>
	private Perceptron _perceptron; 						/// <summary>AI Controller's Brain.</summary>

	/// <summary>Gets and Sets controlMode property.</summary>
	public AIControlMode controlMode
	{
		get { return _controlMode; }
		set { _controlMode = value; }
	}

	/// <summary>Gets and Sets paddle property.</summary>
	public PONGPaddle paddle
	{
		get { return _paddle; }
		set { _paddle = value; }
	}

	/// <summary>Gets updateRate property.</summary>
	public float updateRate { get { return _updateRate; } }

	/// <summary>Gets and Sets time property.</summary>
	public float time
	{
		get { return _time; }
		set { _time = value; }
	}

	/// <summary>Gets and Sets ballProjection property.</summary>
	public Vector3 ballProjection
	{
		get { return _ballProjection; }
		set { _ballProjection = value; }
	}

	/// <summary>Gets and Sets perceptron property.</summary>
	public Perceptron perceptron
	{
		get { return _perceptron; }
		set { _perceptron = value; }
	}

	private void OnGUI()
	{
		if(controlMode != AIControlMode.NeuralNetworks || perceptron == null) return;
		GUILayout.Label(perceptron.ToString());
	}

	/// <summary>PONGPaddleAIController's instance initialization.</summary>
	private void Awake()
	{
		time = updateRate;

		perceptron = new Perceptron();
		perceptron.inputs = new PerceptronData[]
		{
			new PerceptronData("Position_X"),
			new PerceptronData("Position_Y"),
			new PerceptronData("BallPosition_X"),
			new PerceptronData("BallPosition_Y"),
			new PerceptronData("RivalPosition_X"),
			new PerceptronData("RivalPosition_Y")
		};
		perceptron.weights = new float[]
		{
			0.5f,
			0.5f,
			1.0f,
			1.0f,
			0.0f,
			0.0f
		};
		perceptron.bias = 1.0f;
		perceptron.learningRate = 0.2f;
		perceptron.activationFunction = ActivationFunction.ReLU;
	}
	
	/// <summary>PONGPaddleAIController's tick at each frame.</summary>
	private void Update ()
	{
		if(paddle == null) return;

		switch(controlMode)
		{
			case AIControlMode.Scripted:
				if(time >= updateRate)
				{
					UpdateBallProjection();
					time = 0.0f;
				}
				else time += Time.deltaTime;

				MovePaddle();
			break;

			case AIControlMode.NeuralNetworks:
				perceptron.SetInputValue(0, paddle.transform.position.x);
				perceptron.SetInputValue(1, paddle.transform.position.y);
				perceptron.SetInputValue(2, PONGGame.ball.transform.position.x);
				perceptron.SetInputValue(3, PONGGame.ball.transform.position.y);
				perceptron.SetInputValue(4, PONGGame.paddleA.transform.position.x);
				perceptron.SetInputValue(5, PONGGame.paddleA.transform.position.x);
				perceptron.ForwardPropagation();

				if(time >= updateRate)
				{
					UpdateBallProjection();
					time = 0.0f;
				}
				else time += Time.deltaTime;

				MovePaddle();
			break;
		}
	}

	/// <summary>Updates Ball Projection's value.</summary>
	private void UpdateBallProjection()
	{
		ballProjection = PONGGame.ball.Projection(updateRate);
	}

	/// <summary>Moves Paddle.</summary>
	private void MovePaddle()
	{
		float dy = ballProjection.y - paddle.transform.position.y;

		if(dy != 0.0f) paddle.Move(Mathf.Sign(dy));
	}
}
}