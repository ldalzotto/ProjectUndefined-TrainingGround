using GameConfigurationID;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AnimationConfigurationData", menuName = "Configuration/CoreGame/AnimationConfiguration/AnimationConfigurationData", order = 1)]
    public class AnimationConfigurationData : ScriptableObject
    {
        public string AnimationName;
        [CustomEnum(isCreateable: true)]
        public AnimationLayerID AnimationLayer;

        public string GetLayerName()
        {
            return this.AnimationLayer.ToString();
        }

        public int GetLayerIndex(Animator animator)
        {
            return animator.GetLayerIndex(this.GetLayerName());
        }
    }

}
