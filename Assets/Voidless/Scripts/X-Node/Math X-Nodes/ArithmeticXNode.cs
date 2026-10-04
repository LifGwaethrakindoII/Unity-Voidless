using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

/*===========================================================================
**
** Class:  ArithmeticXNode
**
** Purpose: Base Generic X-Node class for performing arithmetic operations.
**
** NOTE: It needs for X-Node's package to be installed into the project.
**
** Author: Lîf Gwaethrakindo
**
===========================================================================*/
namespace Voidless.XNode.Math
{
public enum ArithmeticOperation { Sum, Subtract, Multiply, Divide }

[NodeTint("#4CAF50")]
[Serializable]
public abstract class ArithmeticXNode<T> : Node
{
    [Input] public T a;   
    [Input] public T b;   
    [Output] public T result;
    public ArithmeticOperation operation;

    /// <summary>Sums 2 values.</summary>
    /// <param name="a">Value A.</param>
    /// <param name="b">Value B.</param>
    /// <returns>Result of the Sum.</returns>
    protected abstract T Sum(T a, T b);

    /// <summary>Subtracts 2 values.</summary>
    /// <param name="a">Value A.</param>
    /// <param name="b">Value B.</param>
    /// <returns>Result of the Subtraction.</returns>
    protected abstract T Subtract(T a, T b);

    /// <summary>Multiplies 2 values.</summary>
    /// <param name="a">Value A.</param>
    /// <param name="b">Value B.</param>
    /// <returns>Result of the Multiplication.</returns>
    protected abstract T Multiply(T a, T b);

    /// <summary>Divides 2 values.</summary>
    /// <param name="a">Value A.</param>
    /// <param name="b">Value B.</param>
    /// <returns>Result of the Division.</returns>
    protected abstract T Divide(T a, T b);

    /// <summary>GetValue should be overridden to return a value for any specified output port.</summary>
    public override object GetValue(NodePort port)
    {
        if(port.fieldName != "result") return default(object);

        T A = GetInputValue<T>("a", a);
        T B = GetInputValue<T>("b", b);

        switch(operation)
        {
            case ArithmeticOperation.Sum:       return Sum(A, B);
            case ArithmeticOperation.Subtract:  return Subtract(A, B);
            case ArithmeticOperation.Multiply:  return Multiply(A, B);
            case ArithmeticOperation.Divide:    return Divide(A, B);
            default:                            return default(T);
        }
    }
}

[Serializable]
[NodeTint("#4CAF50")]
public class IntArithmeticXNode : ArithmeticXNode<int>
{
    /// <summary>Sums 2 values.</summary>
    /// <param name="a">Value A.</param>
    /// <param name="b">Value B.</param>
    /// <returns>Result of the Sum.</returns>
    protected override int Sum(int a, int b) { return a + b; }

    /// <summary>Subtracts 2 values.</summary>
    /// <param name="a">Value A.</param>
    /// <param name="b">Value B.</param>
    /// <returns>Result of the Subtraction.</returns>
    protected override int Subtract(int a, int b) { return a - b; }

    /// <summary>Multiplies 2 values.</summary>
    /// <param name="a">Value A.</param>
    /// <param name="b">Value B.</param>
    /// <returns>Result of the Multiplication.</returns>
    protected override int Multiply(int a, int b) { return a * b; }

    /// <summary>Divides 2 values.</summary>
    /// <param name="a">Value A.</param>
    /// <param name="b">Value B.</param>
    /// <returns>Result of the Division.</returns>
    protected override int Divide(int a, int b) { return a / b; }
}
}