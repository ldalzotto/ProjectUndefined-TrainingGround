using UnityEngine;

public class ContextActionInputBuilder
{

    public static AContextActionInput Build(AContextAction contextAction, PlayerManager playerManager)
    {
        if (contextAction.GetType() == typeof(DummyContextAction))
        {
            return new DummyContextActionInput("TEST");
        }
        else if (contextAction.GetType() == typeof(GrabAction))
        {
            return new GrabActionInput(playerManager.GetPlayerAnimator(),
                  AnimationConstants.PlayerAnimationConstants[PlayerAnimatioNnamesEnum.PLAYER_ACTION_GRAB_DOWN].AnimationName,
                  AnimationConstants.PlayerAnimationConstants[PlayerAnimatioNnamesEnum.PLAYER_ACTION_GRAB_DOWN].LayerIndex,
                  ((GrabAction)contextAction).Item);
        }
        else if (contextAction.GetType() == typeof(GiveAction))
        {
            return new GiveActionInput(playerManager.GetCurrentTargetedPOI(), playerManager.GetPlayerAnimator());
        }
        else
        {
            Debug.LogError("The context action : " + contextAction.GetType() + " has no builder implemented.");
            return null;
        }

    }

}
