using System;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voidless
{
[Serializable]
public class SerializableHashSet<T> : HashSet<T>, ISerializationCallbackReceiver
#if UNITY_EDITOR
        , ISerializable
#endif
{
    [SerializeField, HideInInspector] public List<T> _items;

    /// <summary>Gets and Sets items property.</summary>
    public List<T> items
    {
        get { return _items; }
        set { _items = value; }
    }

    /// <summary>SerializableHashSet default constructor.</summary>
    public SerializableHashSet() : base()
    {
        items = new List<T>();
    }

#if UNITY_EDITOR
    /// <summary>SerializableHashSet default constructor.</summary>
    public SerializableHashSet(SerializationInfo _information, StreamingContext _context) : base(_information, _context)
    {
        items = new List<T>();
    }
#endif

    /// <summary>Implement this method to receive a callback before Unity serializes your object.</summary>
    /// <summary>Saves Dictionary to both Lists.</summary>
    public void OnBeforeSerialize()
    {
        items.Clear();

        foreach(T item in this)
        {
            items.Add(item);
        }
    }

    /// <summary>Implement this method to receive a callback after Unity deserializes your object.</summary>
    public void OnAfterDeserialize()
    {
        Clear();

        for(int i = 0; i < items.Count; i++)
        {
            Add(items[i]);
        }
    }

    /// <summary>Clears internal HashSet and Items' List.</summary>
    public void ClearAll()
    {
        Clear();
        items.Clear();
    }

#if UNITY_EDITOR
    /// <summary>Populates a SerializationInfo with the data needed to serialize the target object.</summary>
    public override void GetObjectData(SerializationInfo _info, StreamingContext _context)
    {
        foreach(T item in this)
        {
            _info.AddValue(item.GetHashCode().ToString(), item);
        }
    }
#endif

    /// <returns>String representing this Dictionary's Entries.</returns>
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("HashSet: ");
        builder.Append("\n{");
        builder.Append("\n");
        foreach(T item in this)
        {
            builder.Append("\t");
            builder.Append(item.ToString());
            builder.Append("\n");
        }
        builder.Append("}");

        return builder.ToString();
    }
}

[Serializable] public class StringHashSet : SerializableHashSet<string> { /*...*/ }
[Serializable] public class IntHashSet : SerializableHashSet<int> { /*...*/ }
[Serializable] public class FloatHashSet : SerializableHashSet<float> { /*...*/ }
}