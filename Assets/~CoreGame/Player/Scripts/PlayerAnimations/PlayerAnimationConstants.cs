using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class PlayerAnimationConstants : MonoBehaviour
    {
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
    }

}
