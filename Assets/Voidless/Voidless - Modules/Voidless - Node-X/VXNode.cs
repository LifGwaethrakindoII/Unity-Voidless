using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XNode;
using UnityEditor.Build.Pipeline;

using IO = XNode.NodePort.IO;

/*===========================================================================
**
** Class:  VXNode
**
** Purpose: Static functions & methods for X-Node objects (from the XNode
** package).
**
** Author: Lîf Gwaethrakindo
**
===========================================================================*/
namespace Voidless.XNode
{
public static class VXNode
{
	/// <summary>Gets object from port casted into the specified generic type.</summary>
	/// <param name="_node">X-Node's reference.</param>
    /// <param name="port">Port's reference.</param>
    /// <returns>Object casted into T, if it is from that type.</returns>
    public static T GetObject<T>(this Node _node, NodePort port)
    {
        object val = _node.GetValue(port);
        T obj = default(T);
        
        try { obj = (T)val; }
        catch(Exception exception) { Debug.LogError("[VXNode] Error while trying to cast: " + exception.Message); }

        return obj.Equals(default(T)) ? obj : default(T);
    }

	/// <summary>Gets Connection Node.</summary>
	/// <param name="_node">X-Node's reference.</param>
    /// <param name="_portName">Name of the NodePort containing the Node.</param>
    /// <param name="_direction">Direction of the Node [Input by default].</param>
    /// <param name="_default">Default value to return if no Node could be retreived [null by default].</param>
    /// <returns>Connection Node, default otherwise.</returns>
    public static T GetNode<T>(this Node _node, string _portName, IO _direction = IO.Input, T _default = default) where T : Node
    {
        NodePort nodePort =  _direction == IO.Input ? _node.GetInputPort(_portName) : _node.GetOutputPort(_portName);

        if(nodePort == null) return _default;

        T result = null;

        try { result = nodePort.Connection.node as T; }
        catch(Exception exception) { Debug.LogWarning("[VXNode] Caught Exception: " + exception.Message); }
        
        bool success = result != null;

        return success ? result : _default;
    }

    /// <summary>Gets Input Connection Node.</summary>
    /// <param name="_node">X-Node's reference.</param>
    /// <param name="_portName">Name of the NodePort containing the Node.</param>
    /// <param name="_default">Default value to return if no Node could be retreived [null by default].</param>
    /// <returns>Input Connection Node, default otherwise.</returns>
    public static T GetInputNode<T>(this Node _node, string _portName, T _default = default) where T : Node
    {
        return _node.GetNode<T>(_portName, IO.Input, _default);
    }

    /// <summary>Gets Output Connection Node.</summary>
    /// <param name="_node">X-Node's reference.</param>
    /// <param name="_portName">Name of the NodePort containing the Node.</param>
    /// <param name="_default">Default value to return if no Node could be retreived [null by default].</param>
    /// <returns>Output Connection Node, default otherwise.</returns>
    public static T GetOutputNode<T>(this Node _node, string _portName, T _default = default) where T : Node
    {
        return _node.GetNode<T>(_portName, IO.Output, _default);
    }

	/// <summary>Gets Connection Nodes.</summary>
	/// <param name="_node">X-Node's reference.</param>
    /// <param name="_portName">Name of the NodePort containing the Nodes.</param>
    /// <param name="_direction">Direction of the Nodes [Input by default].</param>
    /// <param name="_default">Default value to return if no Nodes could be retreived [null by default].</param>
    /// <returns>Connection Nodes, default otherwise.</returns>
    public static T[] GetNodes<T>(this Node _node, string _portName, IO _direction = IO.Input, T[] _default = default) where T : Node
    {
        NodePort nodePort =  _direction == IO.Input ? _node.GetInputPort(_portName) : _node.GetOutputPort(_portName);

        if(nodePort == null) return _default;

        int length = nodePort.ConnectionCount;

        if(length == 0) return _default;

        List<T> nodes = new List<T>();

        for(int i = 0; i < length; i++)
        {
            NodePort port = nodePort.GetConnection(i);
            
            if(port == null) continue;

            T node = port.node as T;

            if(node == null) continue;

            nodes.Add(node);
        }

        return nodes.Count > 0 ? nodes.ToArray() : _default;
    }

    /// <summary>Gets Input Connection Nodes.</summary>
    /// <param name="_node">X-Node's reference.</param>
    /// <param name="_portName">Name of the NodePort containing the Nodes.</param>
    /// <param name="_default">Default value to return if no Nodes could be retreived [null by default].</param>
    /// <returns>Connection Nodes, default otherwise.</returns>
    public static T[] GetInputNodes<T>(this Node _node, string _portName, T[] _default = default) where T : Node
    {
        return _node.GetNodes<T>(_portName, IO.Input, _default);
    }

    /// <summary>Gets Output Connection Nodes.</summary>
    /// <param name="_node">X-Node's reference.</param>
    /// <param name="_portName">Name of the NodePort containing the Nodes.</param>
    /// <param name="_default">Default value to return if no Nodes could be retreived [null by default].</param>
    /// <returns>Connection Nodes, default otherwise.</returns>
    public static T[] GetOutputNodes<T>(this Node _node, string _portName, T[] _default = default) where T : Node
    {
        return _node.GetNodes<T>(_portName, IO.Output, _default);
    }

	/// <summary>Gets information of NodePort.</summary>
	/// <param name="_node">X-Node's reference.</param>
    /// <param name="_portName">Name of the NodePort.</param>
    /// <returns>NodePort's info.</returns>
    public static string GetInputNodePortInfo(this Node _node, string _portName)
    {
        NodePort port = _node.GetInputPort(_portName);

        if (port == null) return string.Empty;

        StringBuilder builder = new StringBuilder();

        builder.Append("{ Field Name: ");
        builder.Append(port.fieldName);
        builder.Append(", Type: ");
        builder.Append(port.GetType().Name);
        builder.Append(", Direction: ");
        builder.Append(port.direction);
        builder.Append(", Connection Type: ");
        builder.Append(port.connectionType.ToString());
        builder.Append(", Type Constraint: ");
        builder.Append(port.typeConstraint.ToString());
        builder.Append(", Dynamic: ");
        builder.Append(port.IsDynamic.ToString());
        builder.Append(" }");

        return builder.ToString();
    }
}
}