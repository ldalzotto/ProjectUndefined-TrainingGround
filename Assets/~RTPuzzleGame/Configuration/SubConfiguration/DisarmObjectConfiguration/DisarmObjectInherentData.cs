using GameConfigurationID;
using UnityEngine;
using UnityEngine.Serialization;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DisarmObjectInherentData", menuName = "Configuration/PuzzleGame/DisarmObjectConfiguration/DisarmObjectInherentData", order = 1)]
    public class DisarmObjectInherentData : ScriptableObject
    {
        public float DisarmTime;
        public float DisarmInteractionRange = 2.5f;

        [Header("Animation")]
        [CustomEnum()]
        public AnimationID DisarmObjectAnimationLooped = AnimationID.CA_CrushDownItem;
        
    }
}
