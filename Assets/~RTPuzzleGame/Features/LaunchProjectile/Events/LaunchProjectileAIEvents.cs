using UnityEngine;
using System.Collections;
using GameConfigurationID;

namespace RTPuzzle
{
    public class ProjectileTriggerEnterAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private Vector3 collisionPosition;
        private LaunchProjectileID launchProjectileId;
        private LaunchProjectileInherentData launchProjectileInherentData;

        public ProjectileTriggerEnterAIBehaviorEvent(ILaunchProjectileModuleDataRetrieval ILaunchProjectileModuleDataRetrieval)
        {
            this.collisionPosition = ILaunchProjectileModuleDataRetrieval.GetGroundCollisionTrackingCollider().transform.position;
            if (ILaunchProjectileModuleDataRetrieval != null)
            {
                this.launchProjectileId = ILaunchProjectileModuleDataRetrieval.GetLaunchProjectileID();
                launchProjectileInherentData = ILaunchProjectileModuleDataRetrieval.GetLaunchProjectileInherentData();
            }
        }

        public Vector3 CollisionPosition { get => collisionPosition; }
        public LaunchProjectileInherentData LaunchProjectileInherentData { get => launchProjectileInherentData; }
        public LaunchProjectileID LaunchProjectileId { get => launchProjectileId; }
    }
    

    public static class LaunchProjectileAIEvents
    {

        public static void Projectile_TriggerEnter(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
                PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            Debug.Log(MyLog.Format("AI - OnProjectileTriggerEnter"));

            if (genericAiBehavior.IsManagerInstanciated<AbstractAIProjectileEscapeManager>())
            {
                var projectileTriggerEnterEvent = PuzzleAIBehaviorExternalEvent.Cast<ProjectileTriggerEnterAIBehaviorEvent>();

                // If the player is already escaping without taking into account colliders
                if (genericAiBehavior.IsManagerInstanciated<AbstractAIEscapeWithoutTriggerManager>()
                        && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAIEscapeWithoutTriggerManager>())
                        && GenericPuzzleAIBehaviorExternalEventManager.GetBehaviorStateTrackerContainer().GetBehavior<EscapeWhileIgnoringTargetZoneTracker>().IsEscapingWhileIgnoringTargets)
                {
                    genericAiBehavior.SetManagerState(null);
                    GenericPuzzleAIBehaviorExternalEventManager.ProcessEvent(new EscapeWithoutTriggerStartAIBehaviorEvent(projectileTriggerEnterEvent.CollisionPosition,
                             genericAiBehavior.GetAIManager<AbstractAIProjectileEscapeManager>().GetSemiAngle(projectileTriggerEnterEvent.LaunchProjectileId),
                             genericAiBehavior.GetAIManager<AbstractAIProjectileEscapeManager>().GetMaxEscapeDistance(projectileTriggerEnterEvent.LaunchProjectileId)),
                             genericAiBehavior);
                }
                else if ((genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAIProjectileEscapeManager>())
                    && !GenericPuzzleAIBehaviorExternalEventManager.GetBehaviorStateTrackerContainer().GetBehavior<EscapeWhileIgnoringTargetZoneTracker>().IsEscapingWhileIgnoringTargets)
                        || genericAiBehavior.IsProjectileTriggerAllowedToInterruptOtherStates())
                {
                    genericAiBehavior.SetManagerState(null);
                    genericAiBehavior.GetAIManager<AbstractAIProjectileEscapeManager>().ComponentTriggerEnter(projectileTriggerEnterEvent.CollisionPosition, projectileTriggerEnterEvent);
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIProjectileEscapeManager>());
                }
            }
        }

    }

}
