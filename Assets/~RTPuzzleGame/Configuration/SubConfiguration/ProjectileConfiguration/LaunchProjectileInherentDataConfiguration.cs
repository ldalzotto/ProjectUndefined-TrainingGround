using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "LaunchProjectileInherentDataConfiguration", menuName = "Configuration/PuzzleGame/LaunchProjectileInherentDataConfiguration", order = 1)]
    public class LaunchProjectileInherentDataConfiguration : ScriptableObject, ISerializationCallbackReceiver
    {

        public Dictionary<LaunchProjectileId, ProjectileInherentData> LaunchProjectileInherentDatas = new Dictionary<LaunchProjectileId, ProjectileInherentData>() { };

        [SerializeField]
        private List<LaunchProjectileInherentDataWithProjectileId> LaunchProjectileInherentSerializableDatas;



        public void OnBeforeSerialize()
        {
            if (LaunchProjectileInherentSerializableDatas != null)
            {
                LaunchProjectileInherentSerializableDatas.Clear();
            }

            foreach (var launchProjectileConfigData in LaunchProjectileInherentDatas)
            {
                LaunchProjectileInherentSerializableDatas.Add(new LaunchProjectileInherentDataWithProjectileId(launchProjectileConfigData.Key, launchProjectileConfigData.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            foreach (var launchProjectileSerializableData in LaunchProjectileInherentSerializableDatas)
            {
                LaunchProjectileInherentDatas.Add(launchProjectileSerializableData.LaunchProjectileId, launchProjectileSerializableData.LaunchProjectileInherentData);
            }
        }

        [System.Serializable]
        class LaunchProjectileInherentDataWithProjectileId
        {
            public LaunchProjectileId LaunchProjectileId;
            public ProjectileInherentData LaunchProjectileInherentData;

            public LaunchProjectileInherentDataWithProjectileId(LaunchProjectileId launchProjectileId, ProjectileInherentData launchProjectileInherentData)
            {
                LaunchProjectileId = launchProjectileId;
                LaunchProjectileInherentData = launchProjectileInherentData;
            }
        }
    }

    [System.Serializable]
    public enum LaunchProjectileId
    {
        STONE = 0,
        STONE_1 = 1
    }


}
