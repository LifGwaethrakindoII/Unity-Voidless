using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;

namespace Voidless.XRIT
{
    [RequireComponent(typeof(ClimbInteractable))]
    public class XRITMantable : MonoBehaviour
    {
        private ClimbInteractable _climbInteractable;
        private XRBaseInteractor _interactor;
        
        /// <summary>Gets and Sets interactor property.</summary>
        public XRBaseInteractor interactor
        {
            get { return _interactor; }
            protected set { _interactor = value; }
        }

        /// <summary>Gets climbInteractable Component.</summary>
        public ClimbInteractable climbInteractable
        { 
            get
            {
                if(_climbInteractable == null) _climbInteractable = GetComponent<ClimbInteractable>();
                return _climbInteractable;
            }
        }

        private void Awake()
        {
            climbInteractable.firstSelectEntered.AddListener(OnClimbed);
        }

        private void OnClimbed(SelectEnterEventArgs _args)
        {
            IXRSelectInteractor interactorObject = _args.interactorObject;

            if (interactorObject == null) return;

            XRBaseInteractor temp = interactorObject as XRBaseInteractor;

            if(interactor != null) return;
        }
    }
}