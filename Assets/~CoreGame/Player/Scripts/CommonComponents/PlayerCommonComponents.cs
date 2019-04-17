using UnityEngine;

namespace CoreGame
{
    public class PlayerCommonComponents : MonoBehaviour
    {
        [Header("Movement")]
        public PlayerInputMoveManagerComponent PlayerInputMoveManagerComponent;

        [Header("Procedural animations")]
        public PlayerHairStrandAnimationManagerComponent PlayerHairStrandAnimationManagerComponent;
        public PlayerHoodAnimationManagerComponent PlayerHoodAnimationManagerComponent;
        public PlayerJacketCordAnimationManagerComponent PlayerJacketCordAnimationManagerComponent;
    }
}
