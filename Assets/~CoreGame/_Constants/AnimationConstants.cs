using System.Collections.Generic;
using UnityEngine;

public class AnimationConstants
{
    public static Dictionary<PlayerAnimatioNamesEnum, PlayerAnimationConstantsData> PlayerAnimationConstants = new Dictionary<PlayerAnimatioNamesEnum, PlayerAnimationConstantsData>()
    {
        {PlayerAnimatioNamesEnum.PLAYER_ACTION_GRAB_DOWN, new PlayerAnimationConstantsData("Armature|Grab_Down", PlyaerAnimationLayerNames.CONTEXT_ACTION_OVERRIDE) },
        {PlayerAnimatioNamesEnum.PLAYER_ACTION_FORBIDDEN, new PlayerAnimationConstantsData("Armature|ActionForbidden", PlyaerAnimationLayerNames.CONTEXT_ACTION_OVERRIDE) },
        {PlayerAnimatioNamesEnum.PLAYER_ACTION_GIVE_OBJECT_0, new PlayerAnimationConstantsData("ItemGiven_0", PlyaerAnimationLayerNames.CONTEXT_ACTION_OVERRIDE) },
        {PlayerAnimatioNamesEnum.PLAYER_ACTION_GIVE_OBJECT_1, new PlayerAnimationConstantsData("ItemGiven_1", PlyaerAnimationLayerNames.CONTEXT_ACTION_OVERRIDE) },
        {PlayerAnimatioNamesEnum.PLAYER_ACTION_CA_POCKET_ITEM, new PlayerAnimationConstantsData("Armature|CA_PocketItem", PlyaerAnimationLayerNames.CONTEXT_ACTION_OVERRIDE) },
        {PlayerAnimatioNamesEnum.PLAYER_ACTION_CA_POCKET_ITEM_LAY, new PlayerAnimationConstantsData("Armature|CA_PocketItem_Lay", PlyaerAnimationLayerNames.CONTEXT_ACTION_OVERRIDE) },
        {PlayerAnimatioNamesEnum.PLAYER_ACTION_LISTENING, new PlayerAnimationConstantsData("ContextActionOverrideListening", PlyaerAnimationLayerNames.CONTEXT_ACTION_OVERRIDE) },
        {PlayerAnimatioNamesEnum.PLAYER_IDLE_OVERRIDE_LISTENING, new PlayerAnimationConstantsData("IdleActionListening", PlyaerAnimationLayerNames.IDLE_ACTION_OVERRIDE) },
        {PlayerAnimatioNamesEnum.PLAYER_IDLE_SMOKE_0, new PlayerAnimationConstantsData("Idle_Action_Smoke_0", PlyaerAnimationLayerNames.IDLE_ACTION_OVERRIDE) },
        {PlayerAnimatioNamesEnum.PLAYER_IDLE_SMOKE_1, new PlayerAnimationConstantsData("Idle_Action_Smoke_1", PlyaerAnimationLayerNames.IDLE_ACTION_OVERRIDE) },
        //procedural animations
        {PlayerAnimatioNamesEnum.PLAYER_JACKET_CORD_LISTENING, new PlayerAnimationConstantsData("JacketCord_Listening", PlyaerAnimationLayerNames.JACKET_CORD) },
        {PlayerAnimatioNamesEnum.PLAYER_JACKET_CORD_JITTER_TREE, new PlayerAnimationConstantsData("JacketJitterTree", PlyaerAnimationLayerNames.JACKET_CORD) }
    };

    public class PlyaerAnimationLayerNames
    {
        public const string BASE_LAYER = "Base Layer";
        public const string EMPTY_LAYER = "Empty_Layer";
        public const string CONTEXT_ACTION_OVERRIDE = "Context_Action_Override";
        public const string JACKET_CORD = "Jacket_Cord";
        public const string IDLE_ACTION_OVERRIDE = "Idle_Action_Override";
    }

    public class PlayerAnimationConstantsData
    {

        private string animationName;
        private string layerName;

        public const string HAIR_OBJECT_NAME = "Hair";

        public PlayerAnimationConstantsData(string animationName, string layerName)
        {
            this.animationName = animationName;
            this.layerName = layerName;
        }

        public int GetLayerIndex(Animator animator)
        {
            return animator.GetLayerIndex(this.layerName);
        }

        public string AnimationName { get => animationName; }
        public string LayerName { get => layerName; }
    }

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

public enum PlayerAnimatioNamesEnum
{
    PLAYER_ACTION_GRAB_DOWN,
    PLAYER_ACTION_FORBIDDEN,
    PLAYER_ACTION_GIVE_OBJECT_0,
    PLAYER_ACTION_GIVE_OBJECT_1,
    PLAYER_ACTION_CA_POCKET_ITEM,
    PLAYER_ACTION_CA_POCKET_ITEM_LAY,
    PLAYER_ACTION_LISTENING,
    PLAYER_IDLE_OVERRIDE_LISTENING,
    PLAYER_IDLE_SMOKE_0,
    PLAYER_IDLE_SMOKE_1,
    //procedural animation
    PLAYER_JACKET_CORD_LISTENING,
    PLAYER_JACKET_CORD_JITTER_TREE
}