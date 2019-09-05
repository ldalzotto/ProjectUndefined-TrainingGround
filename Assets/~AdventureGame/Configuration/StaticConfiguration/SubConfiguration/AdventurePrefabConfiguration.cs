using OdinSerializer;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AdventurePrefabConfiguration", menuName = "Configuration/AdventureGame/AdventureStaticConfiguration/AdventurePrefabConfiguration", order = 1)]
    public class AdventurePrefabConfiguration : SerializedScriptableObject
    {
        [Header("POI base modules")]
        public PointOfInterestCutsceneControllerModule BasePointOfInterestCutsceneControllerModule;
    }
}
