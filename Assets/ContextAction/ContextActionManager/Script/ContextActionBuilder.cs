﻿using UnityEngine;

public class ContextActionBuilder
{

    public static AContextActionInput BuildContextActionInput(AContextAction contextAction, PlayerManager playerManager)
    {
        if (contextAction.GetType() == typeof(DummyContextAction))
        {
            return new DummyContextActionInput("TEST");
        }
        else if (contextAction.GetType() == typeof(GrabAction))
        {
            return new GrabActionInput(playerManager.GetPlayerAnimator(), PlayerAnimatioNnamesEnum.PLAYER_ACTION_GRAB_DOWN,
                  ((GrabAction)contextAction).Item);
        }
        else if (contextAction.GetType() == typeof(GiveAction))
        {
            return new GiveActionInput(playerManager.GetCurrentTargetedPOI(), playerManager.GetPlayerAnimator());
        }
        else
        {
            Debug.LogError("The context action : " + contextAction.GetType() + " has no context action input builder implemented.");
            return null;
        }
    }

    public static ScenarioAction BuilScenarioAction(AContextAction contextAction)
    {
        if (contextAction.GetType() == typeof(GrabAction))
        {
            var grabAction = (GrabAction)contextAction;
            return new ScenarioAction(contextAction.GetType(), grabAction.Item.ItemID, grabAction.AssociatedPOI.PointOfInterestId);
        }
        else if (contextAction.GetType() == typeof(GiveAction))
        {
            var giveAction = (GiveAction)contextAction;
            return new ScenarioAction(contextAction.GetType(), giveAction.ItemGiven.ItemID, PointOfInterestId.NONE);
        }
        else
        {
            Debug.LogError("The context action : " + contextAction.GetType() + " has no scenario action builder implemented.");
            return null;
        }
    }

}
