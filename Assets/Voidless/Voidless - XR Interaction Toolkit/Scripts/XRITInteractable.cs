using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Voidless.XRIT
{
public class XRITInteractable : XRSimpleInteractable
{
    private XRBaseInteractor _interactor;
    private bool _selected;

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
    }
}
}