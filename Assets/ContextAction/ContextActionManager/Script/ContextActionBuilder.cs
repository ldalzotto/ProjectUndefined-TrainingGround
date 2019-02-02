using UnityEngine;

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
            return new GrabActionInput(playerManager.GetCurrentTargetedPOI(), ((GrabAction)contextAction).Item);
        }
        else if (contextAction.GetType() == typeof(AnimatorAction))
        {
            return new AnimatorActionInput(playerManager.GetPlayerAnimator(), playerManager);
        }
        else if (contextAction.GetType() == typeof(GiveAction))
        {
            return new GiveActionInput(playerManager.GetCurrentTargetedPOI(), playerManager.GetPlayerAnimator());
        }
        else if (contextAction.GetType() == typeof(TalkAction))
        {
            return new TalkActionInput(playerManager.GetCurrentTargetedPOI().GetAssociatedDiscussionTree());
        }
        else if (contextAction.GetType() == typeof(CutsceneTimelineAction))
        {
            var targetedPOI = playerManager.GetCurrentTargetedPOI();
            return new CutsceneTimelineActionInput(targetedPOI, targetedPOI.GetContextData(), playerManager.transform);
        }
        else if (contextAction.GetType() == typeof(InteractAction))
        {
            return new InteractActionInput(playerManager.GetCurrentTargetedPOI(), playerManager);
        }
        else
        {
            Debug.LogError("The context action : " + contextAction.GetType() + " has no context action input builder implemented.");
            return null;
        }
    }

    public static ScenarioAction BuildScenarioAction(AContextAction contextAction, AContextActionInput contextActionInput)
    {
        if (contextAction.GetType() == typeof(GrabAction))
        {
            var grabAction = (GrabAction)contextAction;
            return new GrabScenarioAction(grabAction.Item.ItemID, grabAction.AssociatedPOI.PointOfInterestId);
        }
        else if (contextAction.GetType() == typeof(GiveAction))
        {
            var giveAction = (GiveAction)contextAction;
            var giveActionInput = (GiveActionInput)contextActionInput;
            if (giveAction.ItemGiven != null && giveActionInput.TargetPOI != null)
            {
                return new GiveScenarioAction(giveAction.ItemGiven.ItemID, giveActionInput.TargetPOI.PointOfInterestId);
            }
            else
            {
                return null;
            }
        }
        else if (contextAction.GetType() == typeof(TalkAction))
        {
            return null;
        }
        else
        {
            Debug.LogWarning("The context action : " + contextAction.GetType() + " has no scenario action builder implemented.");
            return null;
        }
    }
}
