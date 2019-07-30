using UnityEngine;
using GameConfigurationID;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DisarmObjectInherentData", menuName = "Configuration/PuzzleGame/DisarmObjectConfiguration/DisarmObjectInherentData", order = 1)]
    public class DisarmObjectInherentData : ScriptableObject
    {
        public float DisarmTime;
        public float DisarmInteractionRange = 2.5f;
        public InteractiveObjectType DisarmObjectPrefab;

        [CustomEnum()]
        public PuzzleCutsceneId PlayedCutsceneOnDisarm;

        public void Init(float DisarmTime, float DisarmInteractionRange, InteractiveObjectType DisarmObjectPrefab)
        {
            this.DisarmTime = DisarmTime;
            this.DisarmInteractionRange = DisarmInteractionRange;
            this.DisarmObjectPrefab = DisarmObjectPrefab;
        }
                
    }
}
