using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

namespace Voidless
{
[Serializable]
public class SerializableHashSet<T> : HashSet<T>, ISerializationCallbackReceiver
#if UNITY_EDITOR
        , ISerializable
#endif
{
    [SerializeField, HideInInspector] public List<T> _list;

    /// <summary>Gets and Sets list property.</summary>
    public List<T> list
    {
        get
        {
            if (_list == null) _list = new List<T>();
            return _list;
        }
        set { _list = value; }
    }

    /// <summary>SerializableHashSet's constructor.</summary>
    public SerializableHashSet() : base()
    {
        list = new List<T>();
    }

#if UNITY_EDITOR
    /// <summary>Constructor for Editor mode.</summary>
    public SerializableHashSet(SerializationInfo _information, StreamingContext _context) : base(_information, _context)
    {
        list = new List<T>();
    }
#endif

    /// <summary>Implement this method to receive a callback before Unity serializes your object.</summary>
    /// <summary>Saves HashSet to List.</summary>
    public void OnBeforeSerialize()
    {
        if (list == null) list = new List<T>();

        HashSet<T> copies = new HashSet<T>(list);

        foreach (T item in this)
        {
            if (!copies.Contains(item))
            {
                list.Add(item);
            }
        }
    }

    /// <summary>Implement this method to receive a callback after Unity deserializes your object.</summary>
    public void OnAfterDeserialize()
    {
        if (list != null)
        {
            this.Clear();

            foreach (T item in list)
            {
                this.Add(item);
            }
        }
    }

    /// <summary>Clears internal HashSet and List.</summary>
    public void ClearAll()
    {
        if (this != null)
        {
            Clear();
        }

        if (list != null)
        {
            list.Clear();
        }
    }

#if UNITY_EDITOR
    /// <summary>Populates a SerializationInfo with the data needed to serialize the target object.</summary>
    public override void GetObjectData(SerializationInfo _info, StreamingContext _context)
    {
        foreach (T item in this)
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
        foreach (T item in this)
        {
            builder.Append("\t[ ");
            builder.Append(item.ToString());
            builder.Append(" ]");
            builder.Append("\n");
        }
        builder.Append("}");

        return builder.ToString();
    }
}

[Serializable] public class PoolGameObjectHashSet : SerializableHashSet<PoolGameObject> { /*...*/ }
[Serializable] public class VCameraTargetHashSet : SerializableHashSet<VCameraTarget> { /*...*/ }
[Serializable] public class StringHashSet : SerializableHashSet<string> { /*...*/ }
}