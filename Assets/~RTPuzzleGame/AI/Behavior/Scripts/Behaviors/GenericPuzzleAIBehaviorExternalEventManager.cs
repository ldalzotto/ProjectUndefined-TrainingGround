using System;
using System.Collections.Generic;
using UnityEngine;
using static RTPuzzle.AIBehaviorManagerContainer;

namespace RTPuzzle
{
    public class GenericPuzzleAIBehaviorExternalEventManager : PuzzleAIBehaviorExternalEventManager
    {

        private Dictionary<string, int> GenericPuzzleAIEventProcessingOrder = new Dictionary<string, int>()
        {
            {typeof(FearedEndAIBehaviorEvent).Name, 1 },
            {typeof(FearedStartAIBehaviorEvent).Name, 2 },
            {typeof(FearedForcedAIBehaviorEvent).Name, 3 },
            {typeof(TargetZoneTriggerStayAIBehaviorEvent).Name, 4 },
            {typeof(TargetZoneTriggerEnterAIBehaviorEvent).Name, 5 },
            {typeof(ProjectileTriggerEnterAIBehaviorEvent).Name, 6 },
            {typeof(AttractiveObjectTriggerExitAIBehaviorEvent).Name, 7 },
            {typeof(AttractiveObectDestroyedAIBehaviorEvent).Name, 8 },
            {typeof(AttractiveObjectTriggerStayAIBehaviorEvent).Name, 9 },
            {typeof(AttractiveObjectTriggerEnterAIBehaviorEvent).Name, 10 }
        };

        private BehaviorStateTrackerContainer trackerContainer = new BehaviorStateTrackerContainer(new Dictionary<Type, BehaviorStateTracker>()
            {
                {typeof(EscapeWhileIgnoringTargetZoneTracker), new EscapeWhileIgnoringTargetZoneTracker() }
            }
        );

        protected override Dictionary<string, int> EventProcessingOrder => GenericPuzzleAIEventProcessingOrder;

        protected override BehaviorStateTrackerContainer BehaviorStateTrackerContainer => trackerContainer;

        public override void ProcessEvent(PuzzleAIBehaviorExternalEvent externalEvent, IPuzzleAIBehavior<AbstractAIComponents> aiBehavior)
        {
            var genericAiBehavior = (GenericPuzzleAIBehavior)aiBehavior;
            EventTypeCheck<ProjectileTriggerEnterAIBehaviorEvent>(genericAiBehavior, externalEvent, Projectile_TriggerEnter);
            EventTypeCheck<FearedStartAIBehaviorEvent>(genericAiBehavior, externalEvent, Feared_Start);
            EventTypeCheck<FearedForcedAIBehaviorEvent>(genericAiBehavior, externalEvent, Feared_Forced);
            EventTypeCheck<FearedEndAIBehaviorEvent>(genericAiBehavior, externalEvent, Feared_End);
            EventTypeCheck<AttractiveObjectTriggerEnterAIBehaviorEvent>(genericAiBehavior, externalEvent, AttractiveObject_TriggerEnter);
            EventTypeCheck<AttractiveObjectTriggerStayAIBehaviorEvent>(genericAiBehavior, externalEvent, AttractiveObject_TriggerStay);
            EventTypeCheck<AttractiveObjectTriggerExitAIBehaviorEvent>(genericAiBehavior, externalEvent, AttractiveObject_TriggerExit);
            EventTypeCheck<AttractiveObectDestroyedAIBehaviorEvent>(genericAiBehavior, externalEvent, AttractiveObject_Destroyed);
            EventTypeCheck<TargetZoneTriggerEnterAIBehaviorEvent>(genericAiBehavior, externalEvent, TargetZone_TriggerEnter);
            EventTypeCheck<TargetZoneTriggerStayAIBehaviorEvent>(genericAiBehavior, externalEvent, TargetZone_TriggerStay);
            EventTypeCheck<PlayerEscapeStartAIBehaviorEvent>(genericAiBehavior, externalEvent, PlayerEscape_Start);
        }

        private void EventTypeCheck<T>(GenericPuzzleAIBehavior genericAiBehavior, PuzzleAIBehaviorExternalEvent ev, Action<GenericPuzzleAIBehavior, T> processEventAction) where T : PuzzleAIBehaviorExternalEvent
        {
            if (ev.GetType() == typeof(T))
            {
                if (processEventAction != null)
                {
                    processEventAction.Invoke(genericAiBehavior, (T)ev);
                }
            }
        }

        private void Projectile_TriggerEnter(GenericPuzzleAIBehavior genericAiBehavior, ProjectileTriggerEnterAIBehaviorEvent projectileTriggerEnterEvent)
        {
            if (!genericAiBehavior.EvaluateAIManagerAvailabilityToTheFirst(genericAiBehavior.AIProjectileIgnoringTargetZoneEscapeManager(), EvaluationType.EXCLUDED) &&
                             this.trackerContainer.GetBehavior<EscapeWhileIgnoringTargetZoneTracker>().IsEscapingWhileIgnoringTargets)
            {
                Debug.Log(MyLog.Format("AI - OnProjectileTriggerEnter"));
                genericAiBehavior.ManagersStateReset();
                genericAiBehavior.AIProjectileIgnoringTargetZoneEscapeManager().OnTriggerEnter(projectileTriggerEnterEvent.CollisionPosition, projectileTriggerEnterEvent.LaunchProjectileInherentData);
            }
            else if ((!genericAiBehavior.EvaluateAIManagerAvailabilityToTheFirst(genericAiBehavior.AIProjectileEscapeWithCollisionManager(), EvaluationType.EXCLUDED)
                && !this.trackerContainer.GetBehavior<EscapeWhileIgnoringTargetZoneTracker>().IsEscapingWhileIgnoringTargets)
                    || genericAiBehavior.DoesEventInteruptManager(projectileTriggerEnterEvent.GetType()))
            {
                Debug.Log(MyLog.Format("AI - OnProjectileTriggerEnter"));
                genericAiBehavior.ManagersStateReset();
                genericAiBehavior.AIProjectileEscapeWithCollisionManager().OnTriggerEnter(projectileTriggerEnterEvent.CollisionPosition, projectileTriggerEnterEvent.LaunchProjectileInherentData);
            }
        }

        private void Feared_Start(GenericPuzzleAIBehavior genericAiBehavior, FearedStartAIBehaviorEvent fearedStartAIBehaviorEvent)
        {
            genericAiBehavior.ManagersStateReset();
        }

        private void Feared_Forced(GenericPuzzleAIBehavior genericAiBehavior, FearedForcedAIBehaviorEvent fearedForcedAIBehaviorEvent)
        {
            genericAiBehavior.ManagersStateReset();
            genericAiBehavior.AIFearStunManager().OnFearedForced(fearedForcedAIBehaviorEvent);
        }

        private void Feared_End(GenericPuzzleAIBehavior genericAiBehavior, FearedEndAIBehaviorEvent fearedEndAIBehaviorEvent)
        {
            genericAiBehavior.ManagersStateReset();
            // to not have inactive frame.
            genericAiBehavior.ForceUpdateAIBehavior.Invoke();
        }

        private void AttractiveObject_TriggerEnter(GenericPuzzleAIBehavior genericAiBehavior, AttractiveObjectTriggerEnterAIBehaviorEvent attractiveObjectTriggerEnterAIBehaviorEvent)
        {
            if (!genericAiBehavior.EvaluateAIManagerAvailabilityToTheFirst(genericAiBehavior.AIAttractiveObjectManager()))
            {
                Debug.Log(MyLog.Format("AI - OnAttractiveObjectTriggerEnter"));
                genericAiBehavior.ManagersStateReset();
                genericAiBehavior.AIAttractiveObjectManager().OnTriggerEnter(attractiveObjectTriggerEnterAIBehaviorEvent.AttractivePosition, attractiveObjectTriggerEnterAIBehaviorEvent.AttractiveObjectType);
            }
        }

        private void AttractiveObject_TriggerStay(GenericPuzzleAIBehavior genericAiBehavior, AttractiveObjectTriggerStayAIBehaviorEvent attractiveObjectTriggerStayAIBehaviorEvent)
        {
            if (!genericAiBehavior.EvaluateAIManagerAvailabilityToTheFirst(genericAiBehavior.AIAttractiveObjectManager()))
            {
                Debug.Log(MyLog.Format("AI - OnAttractiveObjectTriggerStay"));
                genericAiBehavior.ManagersStateReset();
                genericAiBehavior.AIAttractiveObjectManager().OnTriggerStay(attractiveObjectTriggerStayAIBehaviorEvent.AttractivePosition, attractiveObjectTriggerStayAIBehaviorEvent.AttractiveObjectType);
            }
        }

        private void AttractiveObject_TriggerExit(GenericPuzzleAIBehavior genericAiBehavior, AttractiveObjectTriggerExitAIBehaviorEvent attractiveObjectTriggerExitAIBehaviorEvent)
        {
            Debug.Log(MyLog.Format("AI - OnAttractiveObjectTriggerExit"));
        }

        private void AttractiveObject_Destroyed(GenericPuzzleAIBehavior genericAiBehavior, AttractiveObectDestroyedAIBehaviorEvent attractiveObectDestroyedAIBehaviorEvent)
        {
            genericAiBehavior.AIAttractiveObjectManager().OnAttractiveObjectDestroyed(attractiveObectDestroyedAIBehaviorEvent.DestroyedAttractiveObject);
            // to not have inactive frame.
            genericAiBehavior.ForceUpdateAIBehavior.Invoke();
        }

        private void TargetZone_TriggerEnter(GenericPuzzleAIBehavior genericAiBehavior, TargetZoneTriggerEnterAIBehaviorEvent targetZoneTriggerEnterAIBehaviorEvent)
        {
            if (!genericAiBehavior.EvaluateAIManagerAvailabilityToTheFirst(genericAiBehavior.AITargetZoneManager()))
            {
                if (targetZoneTriggerEnterAIBehaviorEvent.TargetZone != null)
                {
                    if (!genericAiBehavior.IsEscapingFromProjectileWithTargetZones())
                    {
                        Debug.Log(MyLog.Format("Target zone reset FOV"));
                        genericAiBehavior.AIFOVManager.ResetFOV();
                    }

                    Debug.Log(MyLog.Format("AI - OnTargetZoneTriggerEnter"));
                    genericAiBehavior.ManagersStateReset();
                    genericAiBehavior.AITargetZoneManager().TriggerTargetZoneEscape(targetZoneTriggerEnterAIBehaviorEvent.TargetZone);
                }
            }
        }

        private void TargetZone_TriggerStay(GenericPuzzleAIBehavior genericAiBehavior, TargetZoneTriggerStayAIBehaviorEvent targetZoneTriggerStayAIBehaviorEvent)
        {
            if (!genericAiBehavior.EvaluateAIManagerAvailabilityToTheFirst(genericAiBehavior.AITargetZoneManager()))
            {
                if (!genericAiBehavior.IsEscapingFromProjectileWithTargetZones())
                {
                    Debug.Log(MyLog.Format("Target zone reset FOV"));
                    genericAiBehavior.AIFOVManager.ResetFOV();
                }
                if (targetZoneTriggerStayAIBehaviorEvent.TargetZone != null)
                {
                    Debug.Log(MyLog.Format("AI - OnTargetZoneTriggerStay"));
                    genericAiBehavior.ManagersStateReset();
                    genericAiBehavior.AITargetZoneManager().TriggerTargetZoneEscape(targetZoneTriggerStayAIBehaviorEvent.TargetZone);
                }
            }
        }

        private void PlayerEscape_Start(GenericPuzzleAIBehavior genericAiBehavior, PlayerEscapeStartAIBehaviorEvent playerEscapeStartAIBehaviorEvent)
        {
            if (!genericAiBehavior.EvaluateAIManagerAvailabilityToTheFirst(genericAiBehavior.AIPlayerEscapeManager(), EvaluationType.EXCLUDED)
                 || genericAiBehavior.DoesEventInteruptManager(playerEscapeStartAIBehaviorEvent.GetType()))
            {
                genericAiBehavior.ManagersStateReset();
                if (this.trackerContainer.GetBehavior<EscapeWhileIgnoringTargetZoneTracker>().IsEscapingWhileIgnoringTargets)
                {
                    Debug.Log(MyLog.Format("AI - Player escape without colliders."));
                    genericAiBehavior.AIPlayerEscapeManager().OnPlayerEscapeStart(AIPlayerEscapeDestinationCalculationType.FAREST);
                }
                else
                {
                    Debug.Log(MyLog.Format("AI - Player escape with colliders."));
                    genericAiBehavior.AIPlayerEscapeManager().OnPlayerEscapeStart(AIPlayerEscapeDestinationCalculationType.WITH_COLLIDERS);
                }

            }
        }

    }

    public class ProjectileTriggerEnterAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private Vector3 collisionPosition;
        private ProjectileInherentData launchProjectileInherentData;

        public ProjectileTriggerEnterAIBehaviorEvent(LaunchProjectile launchProjectile)
        {
            this.collisionPosition = launchProjectile.SphereCollider.transform.position;
            if (launchProjectile != null)
            {
                launchProjectileInherentData = launchProjectile.LaunchProjectileInherentData;
            }
        }

        public Vector3 CollisionPosition { get => collisionPosition; }
        public ProjectileInherentData LaunchProjectileInherentData { get => launchProjectileInherentData; }
    }

    public class FearedStartAIBehaviorEvent : PuzzleAIBehaviorExternalEvent { }

    public class FearedForcedAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private float fearedTime;

        public FearedForcedAIBehaviorEvent(float fearedTime)
        {
            this.fearedTime = fearedTime;
        }

        public float FearedTime { get => fearedTime; set => fearedTime = value; }
    }

    public class FearedEndAIBehaviorEvent : PuzzleAIBehaviorExternalEvent { }

    public class AttractiveObjectTriggerEnterAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private Vector3 attractivePosition;
        private AttractiveObjectType attractiveObjectType;

        public AttractiveObjectTriggerEnterAIBehaviorEvent(Vector3 attractivePosition, AttractiveObjectType attractiveObjectType)
        {
            this.attractivePosition = attractivePosition;
            this.attractiveObjectType = attractiveObjectType;
        }

        public Vector3 AttractivePosition { get => attractivePosition; }
        public AttractiveObjectType AttractiveObjectType { get => attractiveObjectType; }
    }

    public class AttractiveObjectTriggerStayAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private Vector3 attractivePosition;
        private AttractiveObjectType attractiveObjectType;

        public AttractiveObjectTriggerStayAIBehaviorEvent(Vector3 attractivePosition, AttractiveObjectType attractiveObjectType)
        {
            this.attractivePosition = attractivePosition;
            this.attractiveObjectType = attractiveObjectType;
        }

        public Vector3 AttractivePosition { get => attractivePosition; }
        public AttractiveObjectType AttractiveObjectType { get => attractiveObjectType; }
    }

    public class AttractiveObjectTriggerExitAIBehaviorEvent : PuzzleAIBehaviorExternalEvent { }

    public class AttractiveObectDestroyedAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private AttractiveObjectType destroyedAttractiveObject;

        public AttractiveObectDestroyedAIBehaviorEvent(AttractiveObjectType destroyedAttractiveObject)
        {
            this.destroyedAttractiveObject = destroyedAttractiveObject;
        }

        public AttractiveObjectType DestroyedAttractiveObject { get => destroyedAttractiveObject; }
    }

    public class TargetZoneTriggerEnterAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private TargetZone targetZone;

        public TargetZoneTriggerEnterAIBehaviorEvent(TargetZone targetZone)
        {
            this.targetZone = targetZone;
        }

        public TargetZone TargetZone { get => targetZone; }
    }

    public class TargetZoneTriggerStayAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private TargetZone targetZone;

        public TargetZoneTriggerStayAIBehaviorEvent(TargetZone targetZone)
        {
            this.targetZone = targetZone;
        }

        public TargetZone TargetZone { get => targetZone; }
    }

    public class PlayerEscapeStartAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {

    }

}
