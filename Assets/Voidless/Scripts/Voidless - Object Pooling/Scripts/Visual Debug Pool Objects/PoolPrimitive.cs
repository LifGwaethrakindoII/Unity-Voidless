using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*===========================================================================
**
** Class:  PoolPrimitive
**
** Purpose: Just a poolable wrapper for Unity Primitive's GameObjects to
** change their colours.
**
**
** Author: Lîf Gwaethrakindo
**
===========================================================================*/
namespace Voidless
{
    [RequireComponent(typeof(Renderer))]
    public class PoolPrimitive : PoolGameObject
    {
        public const float ALPHA_MIN = 0.2f;
        public const float ALPHA_MAX = 0.5f;

        [SerializeField] private UnityHash _colorHash;
        private Renderer _renderer;

        /// <summary>Gets colorHash property.</summary>
        public UnityHash colorHash { get { return _colorHash; } }

        /// <summary>Gets renderer Component.</summary>
        public Renderer renderer
        { 
            get
            {
                if(_renderer == null) _renderer = GetComponent<Renderer>();
                return _renderer;
            }
        }

        /// <summary>Sets Renderer's Color.</summary>
        /// <param name="color">New Color.</param>
        public void SetColor(Color color)
        {
            color.a = Mathf.Clamp(color.a, ALPHA_MIN, ALPHA_MAX);
            renderer.material.SetColor(colorHash, color);
        }
    }
}