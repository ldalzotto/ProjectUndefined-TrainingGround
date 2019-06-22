using System.Collections.Generic;
using UnityEngine;


public static class PlayerAnimationConstants
{

    public const string HAIR_OBJECT_NAME = "Hair";

    #region Bone Retriever
    public class PlayerBoneRetriever
    {
        private static Dictionary<PlayerBone, string> BoneNames = new Dictionary<PlayerBone, string>()
    {
        {PlayerBone.HEAD, "Head"},
        {PlayerBone.RIGHT_HAND_CONTEXT, "HoldItem_R" },
        {PlayerBone.RIGH_FINGERS, "Figers_R" },
        {PlayerBone.HOOD, "Hood"},
        {PlayerBone.CHEST, "Chest"}
    };

        public static GameObject GetPlayerBone(PlayerBone playerBone, Animator playerAnimator)
        {
            if (playerAnimator != null)
            {
                return playerAnimator.gameObject.FindChildObjectRecursively(BoneNames[playerBone]);
            }
            return null;

        }
    }

    public enum PlayerBone
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
