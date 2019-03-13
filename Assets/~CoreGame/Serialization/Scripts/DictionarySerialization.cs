using OdinSerializer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConfigurationEditor
{
    [System.Serializable]
    public abstract class ConfigurationSerialization<K, V> : SerializedScriptableObject where K : Enum where V : ScriptableObject
    {
        public Dictionary<K, V> ConfigurationInherentData = new Dictionary<K, V>() { };
    }
}
