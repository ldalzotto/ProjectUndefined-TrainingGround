using CoreGame;
using UnityEngine;
using static AnimationConstants;

namespace AdventureGame
{
    [System.Serializable]
    public class PlayerPOIVisualMovementComponentV2 : ADataComponent
    {
        [Tooltip("The bone that is having visual movmenet")]
        public BipedBone MovingBone = BipedBone.HEAD;
        [Tooltip("This angle is the maximum value for the look system to be enabled. The angle is Ang(player forward, player to POI)")]
        public float POIDetectionAngleLimit = 90f;
        [Tooltip("This angle is the maximum angle where player actually rotate.")]
        public float RotationAngleLimit = 55f;

        public float SmoothMovementSpeed = 5f;
        [Range(0.0f, 1.0f)]
        [Tooltip("When head exits POI interest, indicates the minimum dot product from current head rotation and target to smooth out." +
            "If calculated dot < SmoothOutMaxDotProductLimit -> no smooth out, head is instantly rotating towards animation rotation.")]
        public float SmoothOutMaxDotProductLimit = 0.4f;
    }
}