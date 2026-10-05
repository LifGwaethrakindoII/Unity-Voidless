using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

/*===========================================================================
**
** Class:  PoolText
**
** Purpose: Poolable Text container.
**
**
** Author: Lîf Gwaethrakindo
**
===========================================================================*/
namespace Voidless
{
    public class PoolText : PoolGameObject
    {
        [SerializeField] private Text _textMesh;
        private Func<string> contentPointer;

        /// <summary>Gets textMesh property.</summary>
        public Text textMesh { get { return _textMesh; } }

        /// <summary>Updates each frame.</summary>
        protected override void Update()
        {
            if(contentPointer != null) SetText(contentPointer());
        }

        /// <summary>Sets Text's content.</summary>
        /// <param name="_content">Text's content.</param>
        public void SetText(string _content)
        {
            textMesh.text = _content;
        }

        /// <summary>Sets contentPointer.</summary>
        /// <param name="_contentPointer">Content's pointer.</param>
        public void SetContentPointer(Func<string> _contentPointer)
        {
            contentPointer = _contentPointer;
        }

        /// <summary>Sets Text's color.</summary>
        /// <param name="color">Text's color.</param>
        public void SetColor(Color color = default)
        {
            textMesh.color = color == default ? Color.white : color;
        }

        /// <summary>Actions made when this Pool Object is being recycled.</summary>
        public override void OnObjectRecycled()
        {
            base.OnObjectRecycled();
            SetContentPointer(null);
            SetText(string.Empty);
        }

        /// <returns>String representing this TextMesh's content.</returns>
        public override string ToString()
        {
            return textMesh.text;
        }
    }
}
