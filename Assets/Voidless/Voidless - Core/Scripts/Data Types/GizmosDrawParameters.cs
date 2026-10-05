using System;
using System.Text;
using UnityEngine;

namespace Voidless
{
    public enum GizmosDrawMode { Wired, Solid };

    [Serializable]
    public struct GizmosDrawParameters
    {
        public Color color;
        public GizmosDrawMode drawMode;

        /// <summary>GizmosDrawParameters constructor.</summary>
        /// <param name="_color">Color.</param>
        /// <param name="_drawMode">Draw Mode [GizmosDrawMode.Wired by default].</param>
        public GizmosDrawParameters(Color _color, GizmosDrawMode _drawMode = GizmosDrawMode.Wired)
        {
            color = _color;
            drawMode = _drawMode;
        }

        /// <returns>String representing this set of parameters.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("{ Color: ");
            builder.Append(color.ToString());
            builder.Append(", Draw Mode: ");
            builder.Append(drawMode.ToString());
            builder.Append(" }");

            return builder.ToString();
        }
    }
}