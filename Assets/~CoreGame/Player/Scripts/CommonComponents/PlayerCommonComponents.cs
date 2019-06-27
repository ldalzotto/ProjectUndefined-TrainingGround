using UnityEngine;

namespace CoreGame
{
    public class PlayerCommonComponents : MonoBehaviour
    {
        [Header("Camera")]
        public CameraOrientationManagerComponent CameraOrientationManagerComponent;
        public CameraFollowManagerComponent CameraFollowManagerComponent;

        [Header("Procedural animations")]
        public PlayerHairStrandAnimationManagerComponent PlayerHairStrandAnimationManagerComponent;
        public PlayerHoodAnimationManagerComponent PlayerHoodAnimationManagerComponent;
        public PlayerJacketCordAnimationManagerComponent PlayerJacketCordAnimationManagerComponent;
    }
}
