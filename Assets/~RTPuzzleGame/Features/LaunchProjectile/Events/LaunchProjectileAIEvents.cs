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

        public ProjectileTriggerEnterAIBehaviorEvent(LaunchProjectileModule launchProjectile)
        {
            this.collisionPosition = launchProjectile.GetGroundCollisionTrackingCollider().transform.position;
            if (launchProjectile != null)
            {
                this.launchProjectileId = launchProjectile.LaunchProjectileID;
                launchProjectileInherentData = launchProjectile.LaunchProjectileInherentData;
            }
        }

        public Vector3 CollisionPosition { get => collisionPosition; }
        public LaunchProjectileInherentData LaunchProjectileInherentData { get => launchProjectileInherentData; }
        public LaunchProjectileID LaunchProjectileId { get => launchProjectileId; }
    }

    public class EscapeWithoutTriggerStartAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private Vector3 threatStartPoint;
        private float escapeSemiAngle;
        private float escapeDistance;

        public EscapeWithoutTriggerStartAIBehaviorEvent(Vector3 threatStartPoint, float escapeSemiAngle, float escapeDistance)
        {
            this.threatStartPoint = threatStartPoint;
            this.escapeSemiAngle = escapeSemiAngle;
            this.escapeDistance = escapeDistance;
        }

        public Vector3 ThreatStartPoint { get => threatStartPoint; }
        public float EscapeSemiAngle { get => escapeSemiAngle; }
        public float EscapeDistance { get => escapeDistance; }
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

        public static void EscapeWithoutTrigger_Start(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            Debug.Log(MyLog.Format("AI - EscapeWithoutTrigger_Start"));
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIEscapeWithoutTriggerManager>())
            {
                genericAiBehavior.GetAIManager<AbstractAIEscapeWithoutTriggerManager>().OnEscapeStart(PuzzleAIBehaviorExternalEvent.Cast<EscapeWithoutTriggerStartAIBehaviorEvent>());
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIEscapeWithoutTriggerManager>());
            }
        }

    }

}
