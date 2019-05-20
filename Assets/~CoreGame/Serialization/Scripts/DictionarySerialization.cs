using OdinSerializer;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{
    public interface IConfigurationSerialization
    {
#if UNITY_EDITOR
        void ClearEntry(Enum key);
#endif
    }

    [System.Serializable]
    public abstract class ConfigurationSerialization<K, V> : SerializedScriptableObject, IConfigurationSerialization where K : Enum where V : ScriptableObject
    {
        public Dictionary<K, V> ConfigurationInherentData = new Dictionary<K, V>() { };

#if UNITY_EDITOR
        public void SetEntry(K key, V value)
        {
            this.ClearEntry(key);
            ConfigurationInherentData.Add(key, value);
            EditorUtility.SetDirty(this);
        }

        private void ClearEntry(K key)
        {
            if (ConfigurationInherentData.ContainsKey(key))
            {
                ConfigurationInherentData.Remove(key);
            }
            EditorUtility.SetDirty(this);
        }

        public void ClearEntry(Enum key)
        {
            var catedKey = (K)key;
            if (ConfigurationInherentData.ContainsKey(catedKey))
            {
                ConfigurationInherentData.Remove(catedKey);
            }
            EditorUtility.SetDirty(this);
        }

#endif
    }
}
