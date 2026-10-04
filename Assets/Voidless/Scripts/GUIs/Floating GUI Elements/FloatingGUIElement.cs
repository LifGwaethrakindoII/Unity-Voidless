using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Voidless
{
    public abstract class FloatingGUIElement : GUIElement
    {
        [SerializeField] private Vector3 _displacement;
        [SerializeField] private float _displacementDuration;
        [SerializeField] private float _fadeInDuration;
        [SerializeField] private float _fadeOutDuration;
        protected Coroutine floatingRoutine;

        /// <summary>Gets and Sets displacement property.</summary>
        public Vector3 displacement
        {
            get { return _displacement; }
            set { _displacement = value; }
        }

        /// <summary>Gets fadeInDuration property.</summary>
        public float fadeInDuration { get { return _fadeInDuration; } }

        /// <summary>Gets fadeOutDuration property.</summary>
        public float fadeOutDuration { get { return _fadeOutDuration; } }

        /// <summary>Gets displacementDuration property.</summary>
        public float displacementDuration { get { return _displacementDuration; } }

        public override void OnObjectRecycled()
        {
            base.OnObjectRecycled();
            this.StartCoroutine(FloatingRoutine(), ref floatingRoutine);
        }

        public override void OnObjectDeactivation()
        {
            base.OnObjectDeactivation();
            this.DispatchCoroutine(ref floatingRoutine);
        }

        protected abstract IEnumerator FadeAlphaRoutine(float a, float t);

        /// <summary>Floating Element Routine.</summary>
        protected virtual IEnumerator FloatingRoutine()
        {
            IEnumerator fadeRoutine = FadeAlphaRoutine(1.0f, fadeInDuration);
            Vector3 a = rectTransform.position;
            Vector3 b = a + displacement;
            float i = 1.0f / displacementDuration;
            float t = 0f;

            while(fadeRoutine.MoveNext()) yield return null;

            while (t < 1.0f)
            {
                float dt = Time.deltaTime;
                rectTransform.position = Vector3.Lerp(a, b, VMath.EaseInEaseOut(t, 2.0f));
                t += (i * dt);
                yield return null;
            }

            fadeRoutine = FadeAlphaRoutine(0.0f, fadeOutDuration);

            while (fadeRoutine.MoveNext()) yield return null;

            OnObjectDeactivation();
        }
    }
}