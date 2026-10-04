using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
//using Voidless;

namespace Voidless.XRIT
{
/// <summary>Event invoked when axes change.</summary>
/// <param name="_axes">New Axes' value.</param>
/// <param name="_delta">Delta between the previous registered axes.</param>
public delegate void OnAxesChange(Vector2 _axes, Vector2 _delta);

public class XRITJoystick : XRITInteractable
    {
    public event OnAxesChange onAxesChange;

    [Space(5f)]
    [Header("Joystick's Attributes:")]
    [SerializeField] private Transform _leverBase;
    [SerializeField] private Transform _lever;
    [Space(5f)]
    [Header("Audio's Attributes:")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _movingSFX;
    [Space(5f)]
    [SerializeField] private float _breakDistance;
    [SerializeField] private float _leverAngularLimits;
    [SerializeField] private float _restitutionDuration;
    private Vector2 _previousAxes;
    private Vector2 _axes;
    private Quaternion _defaultLeverRotation;
    protected Coroutine deselectCoroutine;

    /// <summary>Gets leverBase property.</summary>
    public Transform leverBase { get { return _leverBase; } }

    /// <summary>Gets lever property.</summary>
    public Transform lever { get { return _lever; } }

    /// <summary>Gets audioSource property.</summary>
    public AudioSource audioSource { get { return _audioSource; } }

    /// <summary>Gets movingSFX property.</summary>
    public AudioClip movingSFX { get { return _movingSFX; } }

    /// <summary>Gets breakDistance property.</summary>
    public float breakDistance { get { return _breakDistance; } }

    /// <summary>Gets leverAngularLimits property.</summary>
    public float leverAngularLimits { get { return _leverAngularLimits; } }

    /// <summary>Gets restitutionDuration property.</summary>
    public float restitutionDuration { get { return _restitutionDuration; } }

    /// <summary>Gets and Sets previousAxes property.</summary>
    public Vector2 previousAxes
    {
        get { return _previousAxes; }
        protected set { _previousAxes = value; }
    }

    /// <summary>Gets and Sets axes property.</summary>
    public Vector2 axes
    {
        get { return _axes; }
        protected set { _axes = value; }
    }

    /// <summary>Gets and Sets defaultLeverRotation property.</summary>
    public Quaternion defaultLeverRotation
    {
        get { return _defaultLeverRotation; }
        set { _defaultLeverRotation = value; }
    }

    /// <summary>Gets restituting property.</summary>
    public bool restituting { get { return deselectCoroutine != null; } }

    /// <summary>Draws Gizmos on Editor mode.</summary>
    private void OnDrawGizmos()
    {
        if(leverBase == null || lever == null) return;

        Vector3 position = leverBase.position;
        float rayLength = 1.0f;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(position, leverBase.up * rayLength);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(position, leverBase.forward * rayLength);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(position, lever.forward * rayLength);

        if(interactor == null) return;

        Vector3 direction = interactor.transform.position - position;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(position, direction);
    }

    protected override void Awake()
    {
        base.Awake();
        defaultLeverRotation = lever.localRotation;
    }

    /// <summary>Updates XRJoystick's instance at each frame.</summary>
    private void LateUpdate()
    {
        if(!selected) return;
    
        UpdateLever();
    }

    /// <summary>Updates Lever.</summary>
    private void UpdateLever()
    {
        if(leverBase == null || lever == null || interactor == null) return;

        Vector3 o = leverBase.position;
        Vector3 up = leverBase.up;
        Vector3 direction = interactor.transform.position - o;
        Vector3 projection = Vector3.ProjectOnPlane(direction, up);
        float angle = Vector3.Angle(up, direction);
        float n = Mathf.Clamp(angle / leverAngularLimits, 0.0f, 1.0f);
        projection.Normalize();
        projection *= n;

        Vector3 a = Quaternion.Inverse(leverBase.rotation) * projection;
        previousAxes = axes;
        axes = new Vector2(a.x, a.z);

        Vector2 delta = axes - previousAxes;

        if(delta.sqrMagnitude > 0.0f)
        { /// The Joystick is moving, so lets play the sound and invoke a delta-change event:
            InvokeAxesChangeEvent(delta);
            PlaySound();
        }

        if(angle > leverAngularLimits)
        { /// If the angle between the direction of the interactor and the lever's up is higher, limit the stick:
            float t = leverAngularLimits / 90.0f;
            direction = Vector3.Slerp(up, projection, t);
        }

        direction.Normalize();
        lever.rotation = Quaternion.LookRotation(direction);
    }

    /// <summary>Invokes OnAxeschange event.</summary>
    /// <param name="_delta">Difference between previous registered axes and current axes.</param>
    private void InvokeAxesChangeEvent(Vector2 _change)
    {
        if(onAxesChange != null) onAxesChange(axes, _change);
    }

    /// <summary>Callbak invoked when the Lever is Selected.</summary>
    /// <param name="_args">Interactor that is starting the selection.</param>
    protected override void OnSelectEntered(SelectEnterEventArgs _args)
    {
        base.OnSelectEntered(_args);

        if(interactor == null) return;

        axes = Vector2.zero;
        previousAxes = Vector2.zero;
        this.DispatchCoroutine(ref deselectCoroutine);
    }

    /// <summary>Callbak invoked when the Lever is Deselected.</summary>
    /// <param name="_args">Interactor that is ending the selection.</param>
    protected override void OnSelectExited(SelectExitEventArgs _args)
    {
        base.OnSelectExited(_args);
        
        if(interactor != null) return;

        this.StartCoroutine(DeselectionRoutine(), ref deselectCoroutine);
    }

    /// <summary>Plays Sound.</summary>
    /// <param name="_play">Play? true by default.</param>
    private void PlaySound(bool _play = true)
    {
        if(audioSource == null || movingSFX == null) return;

        audioSource.PlayOneShot(movingSFX);

        /*switch(_play)
        {
            case true:
                if(audioSource.clip == movingSFX) return;

                audioSource.Play(movingSFX);
            break;

            case false:
                if(audioSource.clip != movingSFX) return;

                audioSource.Pause();
                audioSource.clip = null;
                audioSource.loop = false;
            break;
        }*/
    }

    /// <summary>Sets Axes.</summary>
    /// <param name="_axes">New Axes.</param>
    public void SetAxes(Vector2 _axes)
    {
        axes = _axes;
    }

    /// <returns>String representing this Joystick.</returns>
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("Selected: ");
        builder.AppendLine(selected.ToString());
        builder.Append("Axes: ");
        builder.AppendLine(axes.ToString());
        builder.Append("Previous Axes: ");
        builder.AppendLine(previousAxes.ToString());

        return builder.ToString();
    }

    /// <summary>De-selection's routine.</summary>
    private IEnumerator DeselectionRoutine()
    {
        Vector2 originalAxes = axes;
        Quaternion a = lever.localRotation;
        Quaternion b = defaultLeverRotation;
        float i = 1.0f / restitutionDuration;
        float t = 0.0f;

        while(t < 1.0f)
        {
            float st = t * t;
            st = EaseOutBounce(t);
            lever.localRotation = Quaternion.Slerp(a, b, st);
            previousAxes = axes;
            axes = Vector2.Lerp(originalAxes, Vector2.zero, st);
            if(previousAxes != axes) InvokeAxesChangeEvent(axes - previousAxes);
            t += (Time.deltaTime * i);
            yield return null;
        }

        lever.localRotation = b;
        previousAxes = axes;
        axes = Vector2.zero;
        if(previousAxes != axes) InvokeAxesChangeEvent(axes - previousAxes);
        previousAxes = axes;
        this.DispatchCoroutine(ref deselectCoroutine);
    }

    /// \TODO Replace by extension methods...
    private void StartCoroutine(IEnumerator _iterator, ref Coroutine _coroutine)
    {
        if(_iterator == null) return;

        DispatchCoroutine(ref _coroutine);
        _coroutine = StartCoroutine(_iterator);
    }

    private void DispatchCoroutine(ref Coroutine _coroutine)
    {
        if(_coroutine == null) return;

        StopCoroutine(_coroutine);
        _coroutine = null;
    }

    private Vector3 WithNormalizedComponents(Vector3 v)
    {
        return new Vector3(
            Mathf.Clamp(v.x, -1.0f, 1.0f),
            Mathf.Clamp(v.y, -1.0f, 1.0f),
            Mathf.Clamp(v.z, -1.0f, 1.0f)
        );
    }

    private Vector2 WithNormalizedComponents(Vector2 v)
    {
        return new Vector2(
            Mathf.Clamp(v.x, -1.0f, 1.0f),
            Mathf.Clamp(v.y, -1.0f, 1.0f)
        );
    }

    /// <summary>Ease-Out Bounce's function [from Easings.net].</summary>
    /// <param name="t">Normalized time t.</param>
    public static float EaseOutBounce(float t)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if(t <= 0.0f || t >= 1.0f)
        {
            return t;

        } else if (t < 1.0f / d1)
        {
            return n1 * t * t;

        } else if (t < 2.0f / d1)
        {
            return n1 * (t -= 1.5f / d1) * t + 0.75f;

        } else if (t < 2.5f / d1)
        {
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;

        } else
        {
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }
    }
}
}