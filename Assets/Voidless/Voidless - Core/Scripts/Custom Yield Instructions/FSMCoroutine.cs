using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Voidless;

/*===========================================================================
**
** Class:  FSMCoroutine
**
** Purpose: Coroutine wrapper that lets you have a little bit more control
** of a Coroutine. You can pause it, restart it, and track its states.
**
**
** Author: Lîf Gwaethrakindo
**
===========================================================================*/
namespace Voidless
{
    public enum CoroutineState
    {
        Ready,
        Running,
        Paused,
        Restarted,
        Finished
    }

    /// <summary>Event invoked when the Coroutine changes state.</summary>
    /// <param name="_coroutine">Coroutine that changed state.</param>
    /// <param name="_state">New Coroutine's state.</param>
    public delegate void OnCoroutineEvent(FSMCoroutine _coroutine, CoroutineState _state);

    public class FSMCoroutine : IEnumerator, IFiniteStateMachine<CoroutineState>
    {
        public event OnCoroutineEvent onCoroutineEvent;

        private IEnumerator _enumerator;
        private Coroutine _coroutine;
        private MonoBehaviour _monoBehaviour;
        private bool _monoBehaviourDependency;

        public CoroutineState previousState { get; set; }
        public CoroutineState state { get; set; }

        /// <summary>Gets and Sets enumerator property.</summary>
        public virtual IEnumerator enumerator
        {
            get { return _enumerator; }
            protected set { _enumerator = value; }
        }

        /// <summary>Gets and Sets coroutine property.</summary>
        public Coroutine coroutine
        {
            get { return _coroutine; }
            protected set { _coroutine = value; }
        }

        public MonoBehaviour monoBehaviour
        {
            get { return _monoBehaviour; }
            protected set
            {
                _monoBehaviour = value;
                monoBehaviourDependency = (_monoBehaviour != null);
            }
        }

        /// <summary>Gets and Sets monoBehaviourDependency property.</summary>
        public bool monoBehaviourDependency
        {
            get { return _monoBehaviourDependency; }
            protected set { _monoBehaviourDependency = value; }
        }

        /// <summary>Gets Current property.</summary>
        public System.Object Current { get { return enumerator != null ? enumerator.Current : null; } }

        #region FiniteStateMachine:
        /// <summary>Enters CoroutineState State.</summary>
        /// <param name="_state">CoroutineState State that will be entered.</param>
        public virtual void OnEnterState(CoroutineState _state)
        {
            if (onCoroutineEvent != null) onCoroutineEvent(this, _state);
        }

        /// <summary>Leaves CoroutineState State.</summary>
        /// <param name="_state">CoroutineState State that will be left.</param>
        public virtual void OnExitState(CoroutineState _state)
        {

        }
        #endregion

        /// <summary>Parameterless Coroutine's constructor.</summary>
        protected FSMCoroutine() { }

        /// <summary>Coroutine class Constructor.</summary>
        /// <param name="_monoBehaviour">MonoBehaviour from where the coroutine belongs.</param>
        /// <param name="_enumerator">Coroutine that will be initialized.</param>
        /// <param name="_startAutomagically">Start this coroutine as soon as you instantiate this behavior? Automatically set to true.</param>
        public FSMCoroutine(MonoBehaviour _monoBehaviour, IEnumerator _enumerator, bool _startAutomagically = true)
        {
            state = CoroutineState.Ready;
            monoBehaviour = _monoBehaviour;
            enumerator = _enumerator;

            if (_startAutomagically) StartCoroutine();
        }

        /// <summary>Starts the Coroutine's Coroutine.</summary>
        public virtual void StartCoroutine()
        {
            if (state == CoroutineState.Ready)
            {
                if (monoBehaviourDependency) coroutine = monoBehaviour.StartCoroutine(this);
                this.ChangeState(CoroutineState.Running);
            }
        }

        /// <summary>Pauses the Coroutine's Coroutine.</summary>
        public virtual void PauseCoroutine()
        {
            if (state == CoroutineState.Running || state == CoroutineState.Ready || state == CoroutineState.Restarted) this.ChangeState(CoroutineState.Paused);
        }

        /// <summary>Resumes the Coroutine [if it was paused].</summary>
        public virtual void ResumeCoroutine()
        {
            if (state == CoroutineState.Paused) this.ChangeState(CoroutineState.Running);
        }

        /// <summary>Stops the current Coroutine, then it starts it again.</summary>
        public virtual void ResetCoroutine()
        {
            if (state != CoroutineState.Restarted)
            {
                this.ChangeState(CoroutineState.Restarted);
                EndCoroutine();
                StartCoroutine();
            }
        }

        /// <summary>Ends the Coroutine.</summary>
        public virtual void EndCoroutine()
        {
            if (state != CoroutineState.Finished)
            {
                if (monoBehaviourDependency && coroutine != null) monoBehaviour.StopCoroutine(coroutine);
                coroutine = null;
                this.ChangeState(CoroutineState.Finished);
                this.ChangeState(CoroutineState.Ready);
            }
        }

        #region IEnumeratorMethods:
        /// <summary>Advances the enumerator to the next element of the collection.</summary>
        public virtual bool MoveNext()
        {
            if (enumerator != null)
            {
                switch (state)
                {
                    case CoroutineState.Ready:
                        return true;

                    case CoroutineState.Running:
                        return enumerator.MoveNext();

                    case CoroutineState.Paused:
                        return true;

                    case CoroutineState.Finished:
                        return false;
                }

                return true;
            }
            else return false;
        }

        /// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
        public virtual void Reset()
        {
            enumerator.Reset();
        }
        #endregion

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object..</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Coroutine State: ");
            builder.Append(state.ToString());

            return builder.ToString();
        }
    }
}