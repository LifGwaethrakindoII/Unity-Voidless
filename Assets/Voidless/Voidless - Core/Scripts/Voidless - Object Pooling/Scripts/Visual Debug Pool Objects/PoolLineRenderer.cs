using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/*===========================================================================
**
** Class:  PoolLineRenderer
**
** Purpose: Poolable LineRenderer Wrapper.
**
**
** Author: Lîf Gwaethrakindo
**
===========================================================================*/
namespace Voidless
{
    [RequireComponent(typeof(LineRenderer))]
    public class PoolLineRenderer : PoolGameObject
    {
        public const int SAMPLES = 50;

        [Space(5f)]
        [SerializeField] private UnityHash _colorHash;
        private LineRenderer _lineRenderer;

        /// <summary>Gets colorHash property.</summary>
        public UnityHash ColorHash { get { return _colorHash; } }

        /// <summary>Gets lineRenderer Component.</summary>
        public LineRenderer LineRenderer
        { 
            get
            {
                if(_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
                return _lineRenderer;
            }
        }

        /// <summary>PoolLineRenderer to LineRenderer implicit operator.</summary>
        public static implicit operator LineRenderer(PoolLineRenderer _poolLineRenderer) { return _poolLineRenderer.LineRenderer; }

        /// <summary>Sets LineRenderer's Color.</summary>
        /// <param name="color">New Color.</param>
        public void SetColor(Color color)
        {
            LineRenderer.material.SetColor(ColorHash, color);
        }

        /// <summary>Sets LineRenderer as a Line.</summary>
        /// <param name="o">Point A.</param>
        /// <param name="b">Point B.</param>
        public void SetAsLine(Vector3 a, Vector3 b)
        {
            LineRenderer.positionCount = 2;
            LineRenderer.SetPosition(0, a);
            LineRenderer.SetPosition(1, b);
        }

        /// <summary>Sets LineRenderer as a Line.</summary>
        /// <param name="o">Point A.</param>
        /// <param name="b">Point B.</param>
        /// <param name="f">Line Function.</param>
        /// <param name="s">Sample count [50 by default].</param>
        public void SetAsLine(Vector3 a, Vector3 b, Func<Vector3, Vector3, float, Vector3> f, int s = SAMPLES)
        {
            if(f == null || s <= 0)
            {
                SetAsLine(a, b);
                return;
            }

            LineRenderer.positionCount = s;
            float x = (float)s;

            for(int i = 0; i < s; i++)
            {
                float t = i / (x - 1.0f);
                Vector3 point = f(a, b, t);
                LineRenderer.SetPosition(i, point);
            }
        }

        /// <summary>Sets LineRenderer as a Ray.</summary>
        /// <param name="o">Ray's Origin.</param>
        /// <param name="d">Ray's direction.</param>
        public void SetAsRay(Vector3 o, Vector3 d)
        {
            SetAsLine(o, o + d);
        }

        /// <summary>Sets LineRenderer as a Quadratic Bezier curve.</summary>
        /// <param name="a">Point A.</param>
        /// <param name="b">Point B.</param>
        /// <param name="c">Tangent point C.</param>
        /// <param name="s">Sample count [50 by default].</param>
        public void SetAsQuadraticBezierCurve(Vector3 a, Vector3 b, Vector3 c, int s = SAMPLES)
        {
            if(s <= 0)
            {
                LineRenderer.positionCount = 3;                
                LineRenderer.SetPosition(0, a);
                LineRenderer.SetPosition(1, c);
                LineRenderer.SetPosition(2, b);
                return;
            }

            LineRenderer.positionCount = s;
            float x = (float)s;

            for(int i = 0; i < s; i++)
            {
                float t = i / (x - 1.0f);
                Vector3 point = VMath.CuadraticBeizer(a, b, c, t);
                LineRenderer.SetPosition(i, point);
            }
        }

        /// <summary>Sets Line Renderer as Projectile Projection.</summary>
        /// <param name="p0">Initial position.</param>
        /// <param name="pf">Final Position.</param>
        /// <param name="t">Time.</param>
        /// <param name="g">Gravity's force.</param>
        /// <param name="s">Sample count [50 by default].</param>
        public void SetAsProjectileProjection(Vector3 p0, Vector3 pf, float t, Vector3 g, int s = SAMPLES)
        {
            if(s <= 0)
            {
                SetAsLine(p0, pf);
                return;
            }

            LineRenderer.positionCount = s;
            Vector3 v0 = VPhysics.ProjectileDesiredVelocity(t, p0, pf, g);
            float x = (float)s;
            float timeSplit = t / (float)s;

            for(int i = 0; i < s; i++)
            {
                float n = i / (x - 1.0f);
                Vector3 p = VPhysics.ProjectileProjection(t * n, v0, p0, g);
                LineRenderer.SetPosition(i, p);
            }
        }

        /// <summary>Actions made when this Pool Object is being recycled.</summary>
        public override void OnObjectRecycled()
        {
            base.OnObjectRecycled();
            LineRenderer.enabled = true;
        }
        
        /// <summary>Callback invoked when the object is deactivated.</summary>
        public override void OnObjectDeactivation()
        {
            base.OnObjectDeactivation();
            LineRenderer.enabled = false;   
        }
    }
}