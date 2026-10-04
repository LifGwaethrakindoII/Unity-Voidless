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
/// <summary>Event invoked when Lever's progress value has changed.</summary>
/// <param name="_progress">Progress value.</param>
public delegate void OnLeverValueChanged(float _progress);

public class XRITBaseLever : XRSimpleInteractable
{
    public event OnLeverValueChanged onLeverValueChanged;

    [Space(5f)]
    [Header("Lever's Attributes:")]
    [SerializeField] private Transform _leverBase;
    [SerializeField] private Transform _lever;
    [Space(5f)]
    [Header("Audio's Attributes:")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _movingSFX;
    [Space(5f)]
    [SerializeField][Range(0.0f, -90.0f)] private float _minAngularLimits;
    [SerializeField][Range(0.0f, 90.0f)] private float _maxAngularLimits;
    [SerializeField] private float _restitutionDuration;
    [Space(5f)]
    [SerializeField] private XRInteractionUpdateOrder.UpdatePhase phase;
    private XRBaseInteractor _interactor;
    private bool _selected;
    private float _progress;
    private float _previousProgress;
    protected Coroutine deselectCoroutine;

#region Getters/Setters:
    /// <summary>Gets leverBase property.</summary>
    public Transform leverBase { get { return _leverBase; } }

    /// <summary>Gets lever property.</summary>
    public Transform lever { get { return _lever; } }

    /// <summary>Gets audioSource property.</summary>
    public AudioSource audioSource { get { return _audioSource; } }

    /// <summary>Gets movingSFX property.</summary>
    public AudioClip movingSFX { get { return _movingSFX; } }

    /// <summary>Gets minAngularLimits property.</summary>
    public float minAngularLimits { get { return _minAngularLimits; } }

    /// <summary>Gets maxAngularLimits property.</summary>
    public float maxAngularLimits { get { return _maxAngularLimits; } }

    /// <summary>Gets restitutionDuration property.</summary>
    public float restitutionDuration { get { return _restitutionDuration; } }

    /// <summary>Gets and Sets interactor property.</summary>
    public XRBaseInteractor interactor
    {
        get { return _interactor; }
        protected set { _interactor = value; }
    }

    /// <summary>Gets and Sets selected property.</summary>
    public bool selected
    {
        get { return _selected; }
        protected set { _selected = value; }
    }

    /// <summary>Gets and Sets progress property.</summary>
    public float progress
    {
        get { return _progress; }
        protected set { _progress = value; }
    }

    /// <summary>Gets and Sets previousProgress property.</summary>
    public float previousProgress
    {
        get { return _previousProgress; }
        protected set { _previousProgress = value; }
    }
#endregion

    /// <summary>Draws Gizmos on Editor mode.</summary>
    protected virtual void OnDrawGizmos()
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
    }

    /// <summary>This method is called by the interaction manager to update the interactable. Please see the interaction manager documentation for more details on update order.</summary>
    /// <param name="_updatePhase">Update's Phase.</param>
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase _updatePhase)
    {
        base.ProcessInteractable(_updatePhase);

        if(_updatePhase == phase) UpdateLever();

        /*switch(_updatePhase)
        {
            case XRInteractionUpdateOrder.UpdatePhase.Late:
                if(!selected) return;
    
                UpdateLever();
            break;
        }*/
    }

    /*/// <summary>Updates XRJoystick's instance at each frame.</summary>
    protected virtual void Update()
    {
        if(!selected) return;
    
        UpdateLever();
    }*/

    /// <summary>Updates Lever.</summary>
    protected virtual void UpdateLever()
    {
        if(leverBase == null || lever == null || interactor == null) return;

        Vector3 o = leverBase.position;
        Vector3 up = leverBase.up;
        Vector3 forward = leverBase.forward;
        Vector3 direction = interactor.transform.position - o;
        direction.Normalize();
        float angle = Vector3.Angle(up, direction);
        float sign = Vector3.Dot(direction, forward);
        float limits = 0.0f;

        if(sign < 0.0f)
        {
            forward *= -1.0f;
            limits = Mathf.Abs(minAngularLimits);
        }
        else limits = maxAngularLimits;

        if(angle > limits)
        {
            float t = limits / 90.0f;
            direction = Vector3.Slerp(up, forward, t);
        }
        else
        {
            direction = Quaternion.Inverse(leverBase.rotation) * direction;
            direction.x = 0.0f;
            direction = transform.rotation * direction;
            direction.Normalize();
        }

        /// Calculate the progress:
        float range = maxAngularLimits - minAngularLimits;
        float min = Mathf.Abs(minAngularLimits);
        angle = sign < 0.0f ? Mathf.Min(angle, min) : Mathf.Min(angle, maxAngularLimits);
        angle = sign < 0.0f ? min - angle : angle + min;
        progress = angle / range;
        float delta = previousProgress -= progress;

        if(delta != 0.0f)
        {
            InvokeLeverProgressChangeEvent();
            PlaySound();
        }

        previousProgress = progress;
        lever.rotation = Quaternion.LookRotation(direction);
    }

    /// <summary>Onvokes OnLeverValueChanged event.</summary>
    protected virtual void InvokeLeverProgressChangeEvent()
    {
        if(onLeverValueChanged != null) onLeverValueChanged(progress);
    }

    /// <summary>Callbak invoked when the Lever is Selected.</summary>
    /// <param name="_args">Interactor that is starting the selection.</param>
    protected override void OnSelectEntered(SelectEnterEventArgs _args)
    {
        IXRSelectInteractor interactorObject = _args.interactorObject;

        if(interactorObject == null) return;

        XRBaseInteractor temp =  interactorObject as XRBaseInteractor;

        if(interactor != null) return;

        base.OnSelectEntered(_args);
        selected = true;
        interactor = temp;
        this.DispatchCoroutine(ref deselectCoroutine);
    }

    /// <summary>Callbak invoked when the Lever is Deselected.</summary>
    /// <param name="_args">Interactor that is ending the selection.</param>
    protected override void OnSelectExited(SelectExitEventArgs _args)
    {
        IXRSelectInteractor interactorObject = _args.interactorObject;

        if(interactorObject == null) return;

        XRBaseInteractor temp =  interactorObject as XRBaseInteractor;

        if(interactor != temp) return;

        base.OnSelectExited(_args);
        selected = false;
        interactor = null;
        this.StartCoroutine(DeselectionRoutine(), ref deselectCoroutine);

        interactionManager.UnregisterInteractable((IXRInteractable)this);
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

    /// <returns>String representing this Joystick.</returns>
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("Lever = { ");
        builder.Append("Progress: ");
        builder.Append(progress.ToString());
        builder.Append(", Previous Progress: ");
        builder.Append(previousProgress.ToString());
        builder.Append(" }");

        return builder.ToString();
    }

    /// <summary>De-selection's routine.</summary>
    protected virtual IEnumerator DeselectionRoutine()
    {
        yield return null;
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
}
}

/*
/// Calculate Progress:

min = -30
max = 80
range = max - min = 80 + 30 = 110

if(sign >= 0.0f) angle += |min|;
else angle = |min| - angle;

progress = angle / range;

example = angle = -10
angle = |min| - |angle| = 30 - 10 = 20
progress = angle / range = 20 / 110


/// Angle from Progress:
progress = angle / range
angle = progress * range
sign = angle > |min| ? 1.0f : -1.0f
*/