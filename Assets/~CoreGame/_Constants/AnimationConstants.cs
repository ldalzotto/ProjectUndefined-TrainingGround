using System.Collections.Generic;
using UnityEngine;


public static class AnimationConstants
{

    public const string HAIR_OBJECT_NAME = "Hair";

    #region Bone Retriever
    public class BipedBoneRetriever
    {
        private static Dictionary<BipedBone, string> BoneNames = new Dictionary<BipedBone, string>()
    {
        {BipedBone.HEAD, "Head"},
        {BipedBone.RIGHT_HAND_CONTEXT, "HoldItem_R" },
        {BipedBone.RIGH_FINGERS, "Figers_R" },
        {BipedBone.HOOD, "Hood"},
        {BipedBone.CHEST, "Chest"}
    };

        public static GameObject GetPlayerBone(BipedBone playerBone, Animator playerAnimator)
        {
            if (playerAnimator != null)
            {
                return playerAnimator.gameObject.FindChildObjectRecursively(BoneNames[playerBone]);
            }
            return null;

        }
    }

    public enum BipedBone
    {
        HEAD, RIGHT_HAND_CONTEXT, RIGH_FINGERS, HOOD, CHEST
    }
    #endregion

    public class PlayerHairStrandBlendShapeNames
    {
        public const string FORWARD = "HairStrand_Bounce_Forward";
        public const string BACKWARD = "HairStrand_Bounce_Backward";
        public const string UP = "HairStrand_Bounce_Up";
        public const string DOWN = "HairStrand_Bounce_Down";
    }

    public class PlayerAnimatorParametersName
    {
        public const string Speed = "Speed";
        public const string JacketCordJitter = "Jacket_Cord_Jitter";
    }
}
