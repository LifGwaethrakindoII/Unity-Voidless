using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Voidless
{
    public class PoolObject : IPoolObject
    {
        private bool _dontDestroyOnLoad;
        private bool _active;

        public Transform ReferenceTransform => throw new System.NotImplementedException();

        /// <summary>Gets and Sets dontDestroyOnLoad property.</summary>
		public bool dontDestroyOnLoad
        {
            get { return _dontDestroyOnLoad; }
            set { _dontDestroyOnLoad = value; }
        }

        /// <summary>Gets and Sets active property.</summary>
        public bool active
        {
            get { return _active; }
            set { _active = value; }
        }

        public event OnPoolObjectEvent onPoolObjectEvent;

        /// <summary>Independent Actions made when this Pool Object is being created.</summary>
		public virtual void OnObjectCreation() { /*...*/ }

        /// <summary>Callback invoked when this Pool Object is being recycled.</summary>
        public virtual void OnObjectRecycled() { /*...*/ }

        /// <summary>Callback invoked when the object is deactivated.</summary>
        public virtual void OnObjectDeactivation() { /*...*/ }

        /// <summary>Actions made when this Pool Object is being destroyed.</summary>
        public virtual void OnObjectDestruction() { /*...*/ }

        /// <summary>Invokes onPoolObjectEvent's callback. Use this only to invoke that event [this is used as a public method that would allow another class to invoke Pool-Object's events].</summary>
        /// <param name="_event">Event to invoke.</param>
        public void InvokeEvent(PoolObjectEvent _event)
        {
            if(onPoolObjectEvent != null) onPoolObjectEvent(this, _event);
        }
    }
}