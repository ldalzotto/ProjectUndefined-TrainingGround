using OdinSerializer;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{
    [System.Serializable]
    public abstract class ConfigurationSerialization<K, V> : SerializedScriptableObject where K : Enum where V : ScriptableObject
    {
        public Dictionary<K, V> ConfigurationInherentData = new Dictionary<K, V>() { };

#if UNITY_EDITOR
        public void SetEntry(K key, V value)
        {
            this.ClearEntry(key);
            ConfigurationInherentData.Add(key, value);
            EditorUtility.SetDirty(this);
        }

        public void ClearEntry(K key)
        {
            if (ConfigurationInherentData.ContainsKey(key))
            {
                ConfigurationInherentData.Remove(key);
            }
            EditorUtility.SetDirty(this);
        }

#endif
    }
}
