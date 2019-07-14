using GameConfigurationID;
using OdinSerializer;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RepelableObjectsInherentData", menuName = "Configuration/PuzzleGame/InteractiveObjects/RepelableObjetsConfiguration/RepelableObjectsInherentData", order = 1)]
    public class RepelableObjectsInherentData : SerializedScriptableObject
    {
        [Inline(createSubIfAbsent: true, FileName = "RepelableObjectDistance")]
        public RepelableObjectDistance RepelableObjectDistance;

        public float GetRepelableObjectDistance(LaunchProjectileId launchProjectileId)
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