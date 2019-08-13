using UnityEngine;

namespace CoreGame
{
    public class PlayerCommonComponents : MonoBehaviour
    {
        [Header("Camera")]
        public CameraFollowManagerComponent CameraFollowManagerComponent;

        [Header("Procedural animations")]
        public PlayerHairStrandAnimationManagerComponent PlayerHairStrandAnimationManagerComponent;
        public PlayerHoodAnimationManagerComponent PlayerHoodAnimationManagerComponent;
        public PlayerJacketCordAnimationManagerComponent PlayerJacketCordAnimationManagerComponent;
    }
}
