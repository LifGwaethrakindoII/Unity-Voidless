using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Voidless
{
    public class FloatingText : FloatingGUIElement
    {
        [SerializeField] private Text _textMesh;

        public Text textMesh { get { return _textMesh; } }

        protected override IEnumerator FadeAlphaRoutine(float a, float t)
        {
            Color b = textMesh.color;
            Color c = b;
            float i = 1.0f / t;
            float n = 0.0f;

            c.a = a;

            while(t < 1.0f)
            {
                textMesh.color = Color.Lerp(b, c, VMath.EaseInEaseOut(t, 2.0f));
                t += (Time.deltaTime * i);
                yield return null;
            }

            textMesh.color = c;
        }
    }
}