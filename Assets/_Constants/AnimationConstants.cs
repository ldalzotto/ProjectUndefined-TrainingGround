using System.Collections.Generic;

public class AnimationConstants
{
    public static Dictionary<PlayerAnimatioNnamesEnum, PlayerAnimationConstantsData> PlayerAnimationConstants = new Dictionary<PlayerAnimatioNnamesEnum, PlayerAnimationConstantsData>()
    {
        {PlayerAnimatioNnamesEnum.PLAYER_ACTION_GRAB_DOWN, new PlayerAnimationConstantsData("Armature|Grab_Down", 2) },
        {PlayerAnimatioNnamesEnum.PLAYER_ACTION_FORBIDDEN, new PlayerAnimationConstantsData("Armature|ActionForbidden", 2) },
        {PlayerAnimatioNnamesEnum.PLAYER_ACTION_GIVE_OBJECT, new PlayerAnimationConstantsData("Armature|ItemGiven", 2) },
        {PlayerAnimatioNnamesEnum.PLAYER_IDLE_OVERRIDE_LISTENING, new PlayerAnimationConstantsData("IdleActionListening", 1) },
        {PlayerAnimatioNnamesEnum.PLAYER_IDLE_SMOKE, new PlayerAnimationConstantsData("Armature|Idle_Action_Smoke", 1) }
    };

    public class PlayerAnimationConstantsData
    {

        private string animationName;
        private int layerIndex;

        public PlayerAnimationConstantsData(string animationName, int layerIndex)
        {
            this.animationName = animationName;
            this.layerIndex = layerIndex;
        }

        public string AnimationName { get => animationName; }
        public int LayerIndex { get => layerIndex; }
    }

    public const string RIGHT_HAND_PLAYER_BONE_NAME = "HoldItem.R";
}

public enum PlayerAnimatioNnamesEnum
{
    PLAYER_ACTION_GRAB_DOWN,
    PLAYER_ACTION_FORBIDDEN,
    PLAYER_ACTION_GIVE_OBJECT,
    PLAYER_IDLE_OVERRIDE_LISTENING,
    PLAYER_IDLE_SMOKE
}