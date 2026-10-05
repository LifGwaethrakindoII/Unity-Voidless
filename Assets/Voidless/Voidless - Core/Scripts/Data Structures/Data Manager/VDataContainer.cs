using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

namespace Voidless
{
[Serializable]
public class VDataContainer
{
    [SerializeField] private StringIntDictionary _intMap;
    [SerializeField] private StringFloatDictionary _floatMap;
    [SerializeField] private StringBoolDictionary _boolMap;
    [SerializeField] private StringStringDictionary _stringMap;
    [SerializeField] private StringVector2Dictionary _vector2Map;
    [SerializeField] private StringVector3Dictionary _vector3Map;
    [SerializeField] private StringVector4Dictionary _vector4Map;
    [SerializeField] private StringQuaternionDictionary _quaternionMap;
    [SerializeField] private StringColorDictionary _colorMap;
    [SerializeField] private StringEulerRotationDictionary _eulerRotationMap;
    [SerializeField] private StringTransformDataDictionary _transformDataMap;
    [SerializeField] private StringGameObjectTagDictionary _gameObjectTagMap;
    [SerializeField] private StringLayerValueDictionary _layerMap;

    /// <summary>Gets and Sets intMap property.</summary>
    public StringIntDictionary intMap
    {
        get { return _intMap; }
        private set { _intMap = value; }
    }

    /// <summary>Gets and Sets floatMap property.</summary>
    public StringFloatDictionary floatMap
    {
        get { return _floatMap; }
        private set { _floatMap = value; }
    }

    /// <summary>Gets and Sets boolMap property.</summary>
    public StringBoolDictionary boolMap
    {
        get { return _boolMap; }
        private set { _boolMap = value; }
    }

    /// <summary>Gets and Sets stringMap property.</summary>
    public StringStringDictionary stringMap
    {
        get { return _stringMap; }
        private set { _stringMap = value; }
    }

    /// <summary>Gets and Sets vector2Map property.</summary>
    public StringVector2Dictionary vector2Map
    {
        get { return _vector2Map; }
        private set { _vector2Map = value; }
    }

    /// <summary>Gets and Sets vector3Map property.</summary>
    public StringVector3Dictionary vector3Map
    {
        get { return _vector3Map; }
        private set { _vector3Map = value; }
    }

    /// <summary>Gets and Sets vector4Map property.</summary>
    public StringVector4Dictionary vector4Map
    {
        get { return _vector4Map; }
        private set { _vector4Map = value; }
    }

    /// <summary>Gets and Sets quaternionMap property.</summary>
    public StringQuaternionDictionary quaternionMap
    {
        get { return _quaternionMap; }
        private set { _quaternionMap = value; }
    }

    /// <summary>Gets and Sets colorMap property.</summary>
    public StringColorDictionary colorMap
    {
        get { return _colorMap; }
        private set { _colorMap = value; }
    }

    /// <summary>Gets and Sets eulerRotationMap property.</summary>
    public StringEulerRotationDictionary eulerRotationMap
    {
        get { return _eulerRotationMap; }
        private set { _eulerRotationMap = value; }
    }

    /// <summary>Gets and Sets transformDataMap property.</summary>
    public StringTransformDataDictionary transformDataMap
    {
        get { return _transformDataMap; }
        private set { _transformDataMap = value; }
    }

    /// <summary>Gets and Sets gameObjectTagMap property.</summary>
    public StringGameObjectTagDictionary gameObjectTagMap
    {
        get { return _gameObjectTagMap; }
        private set { _gameObjectTagMap = value; }
    }

    /// <summary>Gets and Sets layerMap property.</summary>
    public StringLayerValueDictionary layerMap
    {
        get { return _layerMap; }
        private set { _layerMap = value; }
    }

    /// <summary>VDataContainer's Contructor.</summary>
    public VDataContainer()
    {
        intMap = new StringIntDictionary();
        floatMap = new StringFloatDictionary();
        boolMap = new StringBoolDictionary();
        stringMap = new StringStringDictionary();
        vector2Map = new StringVector2Dictionary();
        vector3Map = new StringVector3Dictionary();
        vector4Map = new StringVector4Dictionary();
        quaternionMap = new StringQuaternionDictionary();
        colorMap = new StringColorDictionary();
        eulerRotationMap = new StringEulerRotationDictionary();
        transformDataMap = new StringTransformDataDictionary();
        gameObjectTagMap = new StringGameObjectTagDictionary();
        layerMap = new StringLayerValueDictionary();
    }

    /// <summary>VDataContainer's Contructor.</summary>
    /// <param name="container">VDataContainer to copy from.</param>
    public VDataContainer(VDataContainer container) : this()
    {
        if(container != null) CopyFrom(container);
    }

    /// <summary>Copies data from another VDataContainer.</summary>
    /// <param name="container">Other VDataContainer to copy from.</param>
    public void CopyFrom(VDataContainer container)
    {
        if(container == null || container == this) return;

        intMap = container.intMap;
        floatMap = container.floatMap;
        boolMap = container.boolMap;
        stringMap = container.stringMap;
        vector2Map = container.vector2Map;
        vector3Map = container.vector3Map;
        vector4Map = container.vector4Map;
        quaternionMap = container.quaternionMap;
        colorMap = container.colorMap;
        eulerRotationMap = container.eulerRotationMap;
        transformDataMap = container.transformDataMap;
        gameObjectTagMap = container.gameObjectTagMap;
        layerMap = container.layerMap;
    }
}
}