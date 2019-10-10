using GameConfigurationID;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AnimationConfigurationData", menuName = "Configuration/CoreGame/AnimationConfiguration/AnimationConfigurationData", order = 1)]
    public class AnimationConfigurationData : ScriptableObject
    {
        public string AnimationName;
        [CustomEnum()]
        public AnimationLayerID AnimationLayer;

        public int GetLayerIndex(Animator animator)
        {
            return AnimationConfigurationData.GetLayerIndex(this.AnimationLayer, animator);
        }

        public static int GetLayerIndex(AnimationLayerID AnimationLayer, Animator animator)
        {
            return animator.GetLayerIndex(AnimationLayer.ToString());
        }
    }

}
