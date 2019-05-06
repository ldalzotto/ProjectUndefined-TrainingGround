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
            {typeof(TargetZoneTriggerStayAIBehaviorEvent).Name, 3 },
            {typeof(TargetZoneTriggerEnterAIBehaviorEvent).Name, 4 },
            {typeof(ProjectileTriggerEnterAIBehaviorEvent).Name, 5 },
            {typeof(AttractiveObjectTriggerExitAIBehaviorEvent).Name, 6 },
            {typeof(AttractiveObectDestroyedAIBehaviorEvent).Name, 7 },
            {typeof(AttractiveObjectTriggerStayAIBehaviorEvent).Name, 8 },
            {typeof(AttractiveObjectTriggerEnterAIBehaviorEvent).Name, 9 }
        };

        private BehaviorStateTrackerContainer trackerContainer = new BehaviorStateTrackerContainer(new Dictionary<Type, BehaviorStateTracker>()
            {
                { typeof(ProjectileStateTracker), new ProjectileStateTracker()}
            }
        );

        protected override Dictionary<string, int> EventProcessingOrder => GenericPuzzleAIEventProcessingOrder;

        protected override BehaviorStateTrackerContainer BehaviorStateTrackerContainer => trackerContainer;

        public override void ProcessEvent(PuzzleAIBehaviorExternalEvent externalEvent, IPuzzleAIBehavior<AbstractAIComponents> aiBehavior)
        {
            var genericAiBehavior = (GenericPuzzleAIBehavior)aiBehavior;
            EventTypeCheck<ProjectileTriggerEnterAIBehaviorEvent>(genericAiBehavior, externalEvent, Projectile_TriggerEnter);
            EventTypeCheck<FearedStartAIBehaviorEvent>(genericAiBehavior, externalEvent, Feared_Start);
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
                             this.trackerContainer.GetBehavior<ProjectileStateTracker>().HasFirstProjectileHitted)
            {
                Debug.Log(Time.frameCount + "AI - OnProjectileTriggerEnter");
                genericAiBehavior.ManagersStateReset();
                genericAiBehavior.AIProjectileIgnoringTargetZoneEscapeManager().OnTriggerEnter(projectileTriggerEnterEvent.CollisionPosition, projectileTriggerEnterEvent.LaunchProjectileInherentData);
            }
            else if ((!genericAiBehavior.EvaluateAIManagerAvailabilityToTheFirst(genericAiBehavior.AIProjectileEscapeWithCollisionManager(), EvaluationType.EXCLUDED) && !this.trackerContainer.GetBehavior<ProjectileStateTracker>().HasFirstProjectileHitted)
                    || genericAiBehavior.DoesEventInteruptManager(projectileTriggerEnterEvent.GetType()))
            {
                Debug.Log(Time.frameCount + "AI - OnProjectileTriggerEnter");
                genericAiBehavior.ManagersStateReset();
                genericAiBehavior.AIProjectileEscapeWithCollisionManager().OnTriggerEnter(projectileTriggerEnterEvent.CollisionPosition, projectileTriggerEnterEvent.LaunchProjectileInherentData);
            }
        }

        private void Feared_Start(GenericPuzzleAIBehavior genericAiBehavior, FearedStartAIBehaviorEvent fearedStartAIBehaviorEvent)
        {
            genericAiBehavior.ManagersStateReset();
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
                Debug.Log(Time.frameCount + "AI - OnAttractiveObjectTriggerEnter");
                genericAiBehavior.ManagersStateReset();
                genericAiBehavior.AIAttractiveObjectManager().OnTriggerEnter(attractiveObjectTriggerEnterAIBehaviorEvent.AttractivePosition, attractiveObjectTriggerEnterAIBehaviorEvent.AttractiveObjectType);
            }
        }

        private void AttractiveObject_TriggerStay(GenericPuzzleAIBehavior genericAiBehavior, AttractiveObjectTriggerStayAIBehaviorEvent attractiveObjectTriggerStayAIBehaviorEvent)
        {
            if (!genericAiBehavior.EvaluateAIManagerAvailabilityToTheFirst(genericAiBehavior.AIAttractiveObjectManager()))
            {
                Debug.Log(Time.frameCount + "AI - OnAttractiveObjectTriggerStay");
                genericAiBehavior.ManagersStateReset();
                genericAiBehavior.AIAttractiveObjectManager().OnTriggerStay(attractiveObjectTriggerStayAIBehaviorEvent.AttractivePosition, attractiveObjectTriggerStayAIBehaviorEvent.AttractiveObjectType);
            }
        }

        private void AttractiveObject_TriggerExit(GenericPuzzleAIBehavior genericAiBehavior, AttractiveObjectTriggerExitAIBehaviorEvent attractiveObjectTriggerExitAIBehaviorEvent)
        {
            Debug.Log(Time.frameCount + "AI - OnAttractiveObjectTriggerExit");
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
                        Debug.Log("Target zone reset FOV");
                        genericAiBehavior.AIFOVManager.ResetFOV();
                    }

                    Debug.Log(Time.frameCount + "AI - OnTargetZoneTriggerEnter");
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
                    Debug.Log("Target zone reset FOV");
                    genericAiBehavior.AIFOVManager.ResetFOV();
                }
                if (targetZoneTriggerStayAIBehaviorEvent.TargetZone != null)
                {
                    Debug.Log(Time.frameCount + "AI - OnTargetZoneTriggerStay");
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
                genericAiBehavior.AIPlayerEscapeManager().OnPlayerEscapeStart();
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
