using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Voidless
{
public class Boundaries2DContainer : MonoBehaviour
{
	[SerializeField] private Space _space; 					/// <summary>Space Relativeness.</summary>
	[SerializeField] private Vector3 _size; 				/// <summary>Boundaries' Size.</summary>
	[SerializeField] private Vector3 _center; 				/// <summary>Boundaries' Center.</summary>
#if UNITY_EDITOR
	[Space(5f)]
	[SerializeField] private Color color; 					/// <summary>Gizmos' Color.</summary>
#endif
	private Coroutine boundaries2DInterpolation; 			/// <summary>Coroutine's reference.</summary>

	/// <summary>Gets and Sets space property.</summary>
	public Space space
	{
		get { return _space; }
		set { _space = value; }
	}

	/// <summary>Gets and Sets size property.</summary>
	public Vector3 size
	{
		get { return _size; }
		set { _size = value; }
	}

	/// <summary>Gets and Sets center property.</summary>
	public Vector3 center
	{
		get { return space == Space.World ? _center : (transform.position + _center); }
		set { _center = value; }
	}

	/// <summary>Gets Non-Offseted Center.</summary>
	public Vector3 nonOffsetedCenter { get { return _center; } }

	/// <summary>Gets min property.</summary>
	public Vector3 min { get { return center - (size * 0.5f); } }

	/// <summary>Gets max property.</summary>
	public Vector3 max { get { return center + (size * 0.5f); } }

#if UNITY_EDITOR
	/// <summary>Draws Gizmos on Editor mode.</summary>
	private void OnDrawGizmos()
	{
		Gizmos.color = color;

		/// Draw Boundary's Limits:
		Vector3 bottomLeftPoint = min;
		Vector3 bottomRightPoint = new Vector3(max.x, min.y, min.z);
		Vector3 topLeftPoint = new Vector3(min.x, max.y, min.z);
		Vector3 topRightPoint = max;

		Gizmos.DrawLine(bottomLeftPoint, bottomRightPoint);
		Gizmos.DrawLine(bottomLeftPoint, topLeftPoint);
		Gizmos.DrawLine(topLeftPoint, topRightPoint);
		Gizmos.DrawLine(bottomRightPoint, topRightPoint);
		Gizmos.DrawCube(center, size);
	}

	/// <summary>Resets VCamera2DBoundariesContainer's instance to its default values.</summary>
	private void Reset()
	{
		space = Space.Self;
		color = Color.white.WithAlpha(0.5f);
	}
#endif

	/// <returns>Position of the Boundaries 2D in World-Space [center relative to the position of the container].</returns>
	public Vector3 GetPosition() { return transform.position + _center; }

	/// <returns>Random point inside boundaries.</returns>
	public Vector3 Random()
	{
		Vector3 m = min;
		Vector3 M = max;
		
		return new Vector3
		(
			UnityEngine.Random.Range(m.x, M.x),
			UnityEngine.Random.Range(m.y, M.y),
			UnityEngine.Random.Range(m.z, M.z)
		);
	}

	/// <summary>Gets containment steering forces of vehicle inside boundaries container.</summary>
	/// <param name="_vehicle">Vehicle to evaluate.</param>
	/// <param name="_toleranceDistance">Tolerance distance. If the distance between the vehicle and any of the borders is less than the tolerance, a flee force will be calculated.</param>
	public Vector2 GetContainmentForce(SteeringVehicle2D _vehicle, float _toleranceDistance)
	{
		ValueVTuple<Vector2, Vector2>[] segments = new ValueVTuple<Vector2, Vector2>[4];
		Vector3 bottomLeftPoint = min;
		Vector3 bottomRightPoint = new Vector3(max.x, min.y, min.z);
		Vector3 topLeftPoint = new Vector3(min.x, max.y, min.z);
		Vector3 topRightPoint = max;
		Vector2 sum = Vector2.zero;
		float distance = _toleranceDistance * _toleranceDistance;

		segments[0] = new ValueVTuple<Vector2, Vector2>(bottomRightPoint, bottomLeftPoint);
		segments[1] = new ValueVTuple<Vector2, Vector2>(bottomLeftPoint, topLeftPoint);
		segments[2] = new ValueVTuple<Vector2, Vector2>(topLeftPoint, topRightPoint);
		segments[3] = new ValueVTuple<Vector2, Vector2>(topRightPoint, bottomRightPoint);

		Vector2 projection = _vehicle.Project();

		foreach(ValueVTuple<Vector2, Vector2> pair in segments)
		{
			Vector2 a = projection - pair.Item2;
			Vector2 b = pair.Item1 - pair.Item2;
			Vector2 p = pair.Item2 + VVector2.VectorProjection(a, b);
			Vector2 d = projection - p;

#if UNITY_EDITOR
			/*Debug.DrawRay(pair.Item2, a, Color.white);
			Debug.DrawRay(pair.Item2, b, Color.white);*/
			Debug.DrawRay(p, d, Color.white);
#endif

			if(d.sqrMagnitude < distance) sum += _vehicle.GetFleeForce(p);
		}

		return sum;
	}

	/// <summary>Clamps a point inside the boundaries.</summary>
	/// <param name="_point">Point to contain.</param>
	/// <param name="_axes">Axes to Contain [X and Y by default].</param>
	/// <returns>Clamped point.</returns>
	public Vector3 Clamp(Vector3 _point, Axes3D _axes = Axes3D.XAndY)
	{
		Vector3 m = min;
		Vector3 M = max;

		return new Vector3(
			(_axes | Axes3D.X) == _axes ? Mathf.Clamp(_point.x, m.x, M.x) : _point.x,
			(_axes | Axes3D.Y) == _axes ? Mathf.Clamp(_point.y, m.y, M.y) : _point.y,
			(_axes | Axes3D.Z) == _axes ? Mathf.Clamp(_point.z, m.z, M.z) : _point.z
		);
	}

	/// <summary>Contains Transform inside Boundaries.</summary>
	/// <param name="_transform">Transform to contain.</param>
	/// <param name="_axes">Axes to Contain [X and Y by default].</param>
	public void ContainInsideBoundaries(Transform _transform, Axes3D _axes = Axes3D.XAndY)
	{
		Vector3 m = min;
		Vector3 M = max;

		_transform.position = new Vector3(
			(_axes | Axes3D.X) == _axes ? Mathf.Clamp(_transform.position.x, m.x, M.x) : _transform.position.x,
			(_axes | Axes3D.Y) == _axes ? Mathf.Clamp(_transform.position.y, m.y, M.y) : _transform.position.y,
			(_axes | Axes3D.Z) == _axes ? Mathf.Clamp(_transform.position.z, m.z, M.z) : _transform.position.z
		);
	}

	/// <summary>Contains Rigidbody2D inside Boundaries.</summary>
	/// <param name="_body">Rigidbody2D to contain.</param>
	/// <param name="_axes">Axes to Contain [X and Y by default].</param>
	public void ContainInsideBoundaries(Rigidbody2D _body, Axes3D _axes = Axes3D.XAndY)
	{
		Vector3 m = min;
		Vector3 M = max;

		_body.transform.position = new Vector3(
			(_axes | Axes3D.X) == _axes ? Mathf.Clamp(_body.position.x, m.x, M.x) : _body.position.x,
			(_axes | Axes3D.Y) == _axes ? Mathf.Clamp(_body.position.y, m.y, M.y) : _body.position.y,
			(_axes | Axes3D.Z) == _axes ? Mathf.Clamp(_body.transform.position.z, m.z, M.z) : _body.transform.position.z
		);
	}

	/// <param name="_space">Space Relativeness.</param>
	/// <returns>Data to Boundaries2D.</returns>
	public Boundaries2D ToBoundaries2D(Space _space = Space.World)
	{
		Vector3 c = Vector3.zero;

		if(space != _space)
		{
			switch(_space)
			{
				case Space.World:
					/// Convert from Self to World:
					c = center;
				break;

				case Space.Self:
					/// Convert from World to Self:
					c = transform.position + _center;
				break;
			}
		}

		return new Boundaries2D(size, c);
	}

	/// <summary>Sets Boundaries2D's data.</summary>
	/// <param name="b">Boundaries2DContainer's reference.</param>
	public void Set(Boundaries2DContainer b)
	{
		size = b.size;
		center = GetProperCenter(b);
	}

	/// <summary>Sets Boundaries2D's data.</summary>
	/// <param name="b">Boundaries2D's data.</param>
	public void Set(Boundaries2D b)
	{
		size = b.size;
		center = b.center;
	}

	/// <summary>Converts center from provided Boundaries2DContainer for this Coundaries2DContainer.</summary>
	/// <param name="b">Boundaries2DContainer's reference.</param>
	/// <returns>Converted center for this Container's Space.</returns>
	public Vector3 GetProperCenter(Boundaries2DContainer b)
	{
		Vector3 c = b.nonOffsetedCenter;

		switch(b.space)
		{
			case Space.World:
				switch(space)
				{
					case Space.World:
						return c;
					break;

					case Space.Self:
						return c - transform.position;
					break;
				}
			break;

			case Space.Self:
				switch(space)
				{
					case Space.World:
						return b.transform.position + c;
					break;

					case Space.Self:
						return c;
					break;
				}
			break;
		}

		return c;
	}

	/// <summary>Lerps Between Container A and B.</summary>
	/// <param name="a">Boundaries2DContainer A.</param>
	/// <param name="b">Boundaries2DContainer B.</param>
	/// <param name="t">Normalized time t [internally clamped].</param>
	/// <returns>Interpolation for Container A.</returns>
	public static Boundaries2D Lerp(Boundaries2DContainer a, Boundaries2DContainer b, float t)
	{
		t = Mathf.Clamp(t, 0.0f, 1.0f);

		return new Boundaries2D(
			Vector3.Lerp(a.size, b.size, t),
			Vector3.Lerp(a.center, a.GetProperCenter(b), t)
		);
	}

	/// \TODO DEPRECATE:
	/// <summary>Interpolates towards Boundaries2D.</summary>
	/// <param name="b">Boundaries2D's data.</param>
	/// <param name="d">Duration.</param>
	public void InterpolateTowards(Boundaries2D b, float d)
	{
		//VDebug.Log(LogType.Log, "Invoking InterpolateTowards(", b, ", ", d, ");");
		this.StartCoroutine(InterpolateTowardsBoundaries(b, d, OnInterpolationEnds), ref boundaries2DInterpolation);
	}

	/// <summary>Interpolates towards Boundaries2D.</summary>
	/// <param name="b">Boundaries2D's data.</param>
	/// <param name="d">Duration.</param>
	public void InterpolateTowards(Boundaries2DContainer b, float d)
	{
		//VDebug.Log(LogType.Log, "Invoking InterpolateTowards(", b, ", ", d, ");");
		this.StartCoroutine(InterpolateTowardsBoundaries(b, d, OnInterpolationEnds), ref boundaries2DInterpolation);
	}

	/// <summary>Callback invoked when the Boundaries2D's interpolation ends.</summary>
	public void OnInterpolationEnds()
	{
		this.DispatchCoroutine(ref boundaries2DInterpolation);
	}

	/// \TODO DEPRECATE:
	/// <summary>Interpolation's Coroutine.</summary>
	/// <param name="b">Boundaries2D's data.</param>
	/// <param name="d">Duration.</param>
	/// <param name="onInterpolationEnds">Callback invoked when interpolation ends.</param>
	private IEnumerator InterpolateTowardsBoundaries(Boundaries2D b, float d, Action onInterpolationEnds = null)
	{
		Boundaries2D a = ToBoundaries2D();
		float t = 0.0f;
		float iD = 1.0f / d;

		while(t < 1.0f)
		{
			Set(Boundaries2D.Lerp(a, b, t));

			t += (iD * Time.deltaTime);
			yield return null;
		}

		Set(b);
		if(onInterpolationEnds != null) onInterpolationEnds();
	}

	/// <summary>Interpolation's Coroutine.</summary>
	/// <param name="b">Boundaries2D's data.</param>
	/// <param name="d">Duration.</param>
	/// <param name="onInterpolationEnds">Callback invoked when interpolation ends.</param>
	private IEnumerator InterpolateTowardsBoundaries(Boundaries2DContainer b, float d, Action onInterpolationEnds = null)
	{
		Boundaries2DContainer a = this;
		float t = 0.0f;
		float iD = 1.0f / d;

		while(t < 1.0f)
		{
			Set(Boundaries2DContainer.Lerp(a, b, t));

			t += (iD * Time.deltaTime);
			yield return null;
		}

		Set(b);
		if(onInterpolationEnds != null) onInterpolationEnds();
	}
}
}