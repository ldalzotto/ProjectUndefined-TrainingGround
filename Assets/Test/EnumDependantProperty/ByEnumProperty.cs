using OdinSerializer;
using System;
using System.Collections.Generic;

[System.Serializable]
public abstract class ByEnumProperty<K, V> : SerializedScriptableObject where K : Enum
{
    public Dictionary<K, V> Values = new Dictionary<K, V>();

}