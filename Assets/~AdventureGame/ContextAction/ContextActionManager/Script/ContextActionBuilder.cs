using CoreGame;
using UnityEngine;

namespace AdventureGame
{

    public class ContextActionBuilder
    {

        public static AContextActionInput BuildContextActionInput(AContextAction contextAction, PlayerManager playerManager, AnimationConfiguration animationConfiguration)
        {
            if (contextAction.GetType() == typeof(DummyContextAction))
            {
                return new DummyContextActionInput("TEST");
            }
            else if (contextAction.GetType() == typeof(GrabAction))
            {
                return new GrabActionInput(playerManager.GetCurrentTargetedPOI(), ((GrabAction)contextAction).ItemInvolved);
            }
            else if (contextAction.GetType() == typeof(AnimatorAction))
            {
                return new AnimatorActionInput(playerManager.GetPlayerAnimator(), playerManager, animationConfiguration);
            }
            else if (contextAction.GetType() == typeof(GiveAction))
            {
                return new GiveActionInput(playerManager.GetCurrentTargetedPOI(), playerManager.GetPlayerAnimator());
            }
            else if (contextAction.GetType() == typeof(CutsceneTimelineAction))
            {
                var targetedPOI = playerManager.GetCurrentTargetedPOI();
                return new CutsceneTimelineActionInput(targetedPOI, targetedPOI.GetContextData(), playerManager.transform);
            }
            else if (contextAction.GetType() == typeof(ItemInteractAction))
            {
                return new InteractActionInput(playerManager.GetCurrentTargetedPOI(), playerManager);
            }
            else if (contextAction.GetType() == typeof(LevelZoneTransitionAction))
            {
                return null;
            }
            else
            {
                Debug.LogWarning("The context action : " + contextAction.GetType() + " has no context action input builder implemented.");
                return null;
            }
        }

        public static TimeLineAction BuildScenarioAction(AContextAction contextAction, AContextActionInput contextActionInput)
        {
            if (contextAction.GetType() == typeof(GrabAction))
            {
                var grabAction = (GrabAction)contextAction;
                return new GrabScenarioAction(grabAction.ItemInvolved, grabAction.AssociatedPOI.PointOfInterestId);
            }
            else if (contextAction.GetType() == typeof(GiveAction))
            {
                var giveAction = (GiveAction)contextAction;
                var giveActionInput = (GiveActionInput)contextActionInput;
                if (giveActionInput.TargetPOI != null)
                {
                    return new GiveScenarioAction(giveAction.ItemGiven, giveActionInput.TargetPOI.PointOfInterestId);
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
            else if (contextAction.GetType() == typeof(CutsceneTimelineAction))
            {
                var cutsceneTimelineAction = (CutsceneTimelineAction)contextAction;
                var cutsceneTimelineActionInput = (CutsceneTimelineActionInput)contextActionInput;
                return new CutsceneTimelineScenarioAction(cutsceneTimelineAction.CutsceneId, cutsceneTimelineActionInput.TargetedPOI.PointOfInterestId);
            }
            else
            {
                Debug.LogWarning("The context action : " + contextAction.GetType() + " has no scenario action builder implemented.");
                return null;
            }
        }
    }

}