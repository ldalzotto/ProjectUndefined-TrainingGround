using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "TargetZonesConfiguration", menuName = "Configuration/PuzzleGame/TargetZonesConfiguration/TargetZonesConfiguration", order = 1)]
    public class TargetZonesConfiguration : ScriptableObject, ISerializationCallbackReceiver
    {
        public Dictionary<TargetZoneID, TargetZoneInherentData> conf = new Dictionary<TargetZoneID, TargetZoneInherentData>() { };

        [SerializeField]
        private List<TargetZoneInherentDataWithLevelId> TargetZoneInherentSerializableDatas;

        public void OnBeforeSerialize()
        {
            if (TargetZoneInherentSerializableDatas != null)
            {
                TargetZoneInherentSerializableDatas.Clear();
            }

            foreach (var targetZoneConfigData in conf)
            {
                TargetZoneInherentSerializableDatas.Add(new TargetZoneInherentDataWithLevelId(targetZoneConfigData.Key, targetZoneConfigData.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            foreach (var targetZoneSerializedData in TargetZoneInherentSerializableDatas)
            {
                conf.Add(targetZoneSerializedData.TargetZoneId, targetZoneSerializedData.TargetZoneConfigurationData);
            }
        }

        [System.Serializable]
        class TargetZoneInherentDataWithLevelId
        {
            public TargetZoneID TargetZoneId;
            public TargetZoneInherentData TargetZoneConfigurationData;

            public TargetZoneInherentDataWithLevelId(TargetZoneID targetZoneId, TargetZoneInherentData targetZoneConfigurationData)
            {
                TargetZoneId = targetZoneId;
                TargetZoneConfigurationData = targetZoneConfigurationData;
            }
        }
    }
   
}
