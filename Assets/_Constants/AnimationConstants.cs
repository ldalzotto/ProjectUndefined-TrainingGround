using System.Collections.Generic;

public class AnimationConstants
{
    public static Dictionary<PlayerAnimatioNnamesEnum, PlayerAnimationConstantsData> PlayerAnimationConstants = new Dictionary<PlayerAnimatioNnamesEnum, PlayerAnimationConstantsData>()
    {
        {PlayerAnimatioNnamesEnum.PLAYER_ACTION_GRAB_DOWN, new PlayerAnimationConstantsData("Armature|Grab_Down", 1) },
        {PlayerAnimatioNnamesEnum.PLAYER_ACTION_FORBIDDEN, new PlayerAnimationConstantsData("Armature|ActionForbidden", 1) },
        {PlayerAnimatioNnamesEnum.PLAYER_ACTION_GIVE_OBJECT, new PlayerAnimationConstantsData("Armature|ItemGiven", 1) }
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
}

public enum PlayerAnimatioNnamesEnum
{
    PLAYER_ACTION_GRAB_DOWN,
    PLAYER_ACTION_FORBIDDEN,
    PLAYER_ACTION_GIVE_OBJECT
}