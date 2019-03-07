using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{

    [System.Serializable]
    public class DictionarySerialization<K, V> : ScriptableObject, ISerializationCallbackReceiver where K : struct, IConvertible where V : ScriptableObject
    {
        public Dictionary<K, V> LaunchProjectileInherentDatas = new Dictionary<K, V>() { };

        [SerializeField]
        private List<ConfigurationDataWithId> LaunchProjectileInherentSerializableDatas;

        public void OnBeforeSerialize()
        {
            if (LaunchProjectileInherentSerializableDatas == null)
            {
                LaunchProjectileInherentSerializableDatas = new List<ConfigurationDataWithId>();
            }
            if (LaunchProjectileInherentSerializableDatas != null)
            {
                LaunchProjectileInherentSerializableDatas.Clear();
            }

            foreach (var launchProjectileConfigData in LaunchProjectileInherentDatas)
            {
                K parsedEnum;
                Enum.TryParse(launchProjectileConfigData.Key.ToString(), out parsedEnum);
                LaunchProjectileInherentSerializableDatas.Add(new ConfigurationDataWithId(Convert.ToInt16(parsedEnum), launchProjectileConfigData.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            foreach (var launchProjectileSerializableData in LaunchProjectileInherentSerializableDatas)
            {
                LaunchProjectileInherentDatas.Add((K)Enum.Parse(typeof(K), launchProjectileSerializableData.id.ToString()), (V)launchProjectileSerializableData.obj);
            }
        }
    }


    [System.Serializable]
    public class ConfigurationDataWithId
    {
        public int id;
        public ScriptableObject obj;

        public ConfigurationDataWithId(int id, ScriptableObject obj)
        {
            this.id = id;
            this.obj = obj;
        }
    }

}
