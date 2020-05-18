﻿using OdinSerializer;
using System;
using System.Collections.Generic;

public interface IByEnumProperty { }
[System.Serializable]
public abstract class ByEnumProperty<K, V> : SerializedScriptableObject, IByEnumProperty where K : Enum
{
    public Dictionary<K, V> Values = new Dictionary<K, V>();

}