using GameConfigurationID;
using OdinSerializer;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ObjectRepelInherentData", menuName = "Configuration/PuzzleGame/InteractiveObjects/ObjectRpelConfiguration/ObjectRepelInherentData", order = 1)]
    public class ObjectRepelInherentData : SerializedScriptableObject
    {
        [Inline(CreateAtSameLevelIfAbsent = true, FileName = "RepelableObjectDistance")]
        public RepelableObjectDistance RepelableObjectDistance;

        public float GetRepelableObjectDistance(LaunchProjectileID launchProjectileId)
        {
            if (this.RepelableObjectDistance != null && this.RepelableObjectDistance.Values.ContainsKey(launchProjectileId))
            {
                return this.RepelableObjectDistance.Values[launchProjectileId];
            }
            else
            {
                return 0f; 
            }
        }
    }
}