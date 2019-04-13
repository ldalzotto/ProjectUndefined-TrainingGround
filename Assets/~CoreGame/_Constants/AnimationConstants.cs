using System.Collections.Generic;

public class AnimationConstants
{
    public static Dictionary<PlayerAnimatioNamesEnum, PlayerAnimationConstantsData> PlayerAnimationConstants = new Dictionary<PlayerAnimatioNamesEnum, PlayerAnimationConstantsData>()
    {
        {PlayerAnimatioNamesEnum.PLAYER_ACTION_GRAB_DOWN, new PlayerAnimationConstantsData("Armature|Grab_Down", 2) },
        {PlayerAnimatioNamesEnum.PLAYER_ACTION_FORBIDDEN, new PlayerAnimationConstantsData("Armature|ActionForbidden", 2) },
        {PlayerAnimatioNamesEnum.PLAYER_ACTION_GIVE_OBJECT, new PlayerAnimationConstantsData("Armature|ItemGiven", 2) },
        {PlayerAnimatioNamesEnum.PLAYER_ACTION_CA_POCKET_ITEM, new PlayerAnimationConstantsData("Armature|CA_PocketItem", 2) },
        {PlayerAnimatioNamesEnum.PLAYER_ACTION_CA_POCKET_ITEM_LAY, new PlayerAnimationConstantsData("Armature|CA_PocketItem_Lay", 2) },
        {PlayerAnimatioNamesEnum.PLAYER_ACTION_LISTENING, new PlayerAnimationConstantsData("ContextActionOverrideListening", 2) },
        {PlayerAnimatioNamesEnum.PLAYER_IDLE_OVERRIDE_LISTENING, new PlayerAnimationConstantsData("IdleActionListening", 1) },
        {PlayerAnimatioNamesEnum.PLAYER_IDLE_SMOKE, new PlayerAnimationConstantsData("Armature|Idle_Action_Smoke", 1) },
        //procedural animations
        {PlayerAnimatioNamesEnum.PLAYER_JACKET_CORD_LISTENING, new PlayerAnimationConstantsData("JacketCord_Listening", 3) },
        {PlayerAnimatioNamesEnum.PLAYER_JACKET_CORD_JITTER_TREE, new PlayerAnimationConstantsData("JacketJitterTree", 3) }
    };

    public class PlayerAnimationConstantsData
    {

        private string animationName;
        private int layerIndex;

        public const string HAIR_OBJECT_NAME = "Hair";

        public PlayerAnimationConstantsData(string animationName, int layerIndex)
        {
            this.animationName = animationName;
            this.layerIndex = layerIndex;
        }

        public string AnimationName { get => animationName; }
        public int LayerIndex { get => layerIndex; }
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
    PLAYER_ACTION_GIVE_OBJECT,
    PLAYER_ACTION_CA_POCKET_ITEM,
    PLAYER_ACTION_CA_POCKET_ITEM_LAY,
    PLAYER_ACTION_LISTENING,
    PLAYER_IDLE_OVERRIDE_LISTENING,
    PLAYER_IDLE_SMOKE,
    //procedural animation
    PLAYER_JACKET_CORD_LISTENING,
    PLAYER_JACKET_CORD_JITTER_TREE
}