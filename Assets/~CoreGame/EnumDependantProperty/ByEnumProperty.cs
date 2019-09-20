using OdinSerializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreGame
{
    public interface IByEnumProperty
    {
    }

    [System.Serializable]
    public abstract class ByEnumProperty<K, V> : SerializedScriptableObject, IByEnumProperty where K : Enum
    {
        public Dictionary<K, V> Values = new Dictionary<K, V>();
    }
}