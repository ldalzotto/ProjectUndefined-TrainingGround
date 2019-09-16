using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

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
            {typeof(DisarmingObjectEnterAIbehaviorEvent).Name, 7 },
            {typeof(DisarmingObjectExitAIbehaviorEvent).Name, 8 },
            {typeof(AttractiveObjectTriggerExitAIBehaviorEvent).Name, 9 },
            {typeof(AttractiveObectDestroyedAIBehaviorEvent).Name, 10 },
            {typeof(AttractiveObjectTriggerStayAIBehaviorEvent).Name, 11 },
            {typeof(AttractiveObjectTriggerEnterAIBehaviorEvent).Name, 12 },
            {typeof(SightInRangeEnterAIBehaviorEvent).Name, 13 },
            {typeof(SightInRangeExitAIBehaviorEvent).Name, 14 }
        };

        private BehaviorStateTrackerContainer trackerContainer = new BehaviorStateTrackerContainer(new Dictionary<Type, BehaviorStateTracker>()
            {
                {typeof(EscapeWhileIgnoringTargetZoneTracker), new EscapeWhileIgnoringTargetZoneTracker() }
            }
        );

        protected override Dictionary<string, int> EventProcessingOrder => GenericPuzzleAIEventProcessingOrder;

        protected override BehaviorStateTrackerContainer BehaviorStateTrackerContainer => trackerContainer;

        public override void ProcessEvent(PuzzleAIBehaviorExternalEvent externalEvent, IPuzzleAIBehavior aiBehavior)
        {
            // Debug.Log(MyLog.Format("AI - ProcessingEvent - " + externalEvent.GetType().Name));
            var genericAiBehavior = (GenericPuzzleAIBehavior)aiBehavior;
            EventTypeCheck<ProjectileTriggerEnterAIBehaviorEvent>(genericAiBehavior, externalEvent, Projectile_TriggerEnter);
            EventTypeCheck<EscapeWithoutTriggerStartAIBehaviorEvent>(genericAiBehavior, externalEvent, EscapeWithoutTrigger_Start);
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
            EventTypeCheck<SightInRangeEnterAIBehaviorEvent>(genericAiBehavior, externalEvent, SightInRange_Enter);
            EventTypeCheck<SightInRangeExitAIBehaviorEvent>(genericAiBehavior, externalEvent, SightInRange_Exit);
            EventTypeCheck<DisarmingObjectEnterAIbehaviorEvent>(genericAiBehavior, externalEvent, DisarmingObject_Enter);
            EventTypeCheck<DisarmingObjectExitAIbehaviorEvent>(genericAiBehavior, externalEvent, DisarmingObject_Exit);
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
            Debug.Log(MyLog.Format("AI - OnProjectileTriggerEnter"));

            if (genericAiBehavior.IsManagerInstanciated<AbstractAIProjectileEscapeManager>())
            {
                // If the player is already escaping without taking into account colliders
                if (genericAiBehavior.IsManagerInstanciated<AbstractAIEscapeWithoutTriggerManager>()
                        && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAIEscapeWithoutTriggerManager>())
                        && this.trackerContainer.GetBehavior<EscapeWhileIgnoringTargetZoneTracker>().IsEscapingWhileIgnoringTargets)
                {
                    genericAiBehavior.SetManagerState(null);
                    this.ProcessEvent(new EscapeWithoutTriggerStartAIBehaviorEvent(projectileTriggerEnterEvent.CollisionPosition,
                             genericAiBehavior.GetAIManager<AbstractAIProjectileEscapeManager>().GetSemiAngle(projectileTriggerEnterEvent.LaunchProjectileId),
                             genericAiBehavior.GetAIManager<AbstractAIProjectileEscapeManager>().GetMaxEscapeDistance(projectileTriggerEnterEvent.LaunchProjectileId)),
                             genericAiBehavior);
                }
                else if ((genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAIProjectileEscapeManager>())
                    && !this.trackerContainer.GetBehavior<EscapeWhileIgnoringTargetZoneTracker>().IsEscapingWhileIgnoringTargets)
                        || genericAiBehavior.IsProjectileTriggerAllowedToInterruptOtherStates())
                {
                    genericAiBehavior.SetManagerState(null);
                    genericAiBehavior.GetAIManager<AbstractAIProjectileEscapeManager>().ComponentTriggerEnter(projectileTriggerEnterEvent.CollisionPosition, projectileTriggerEnterEvent);
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIProjectileEscapeManager>());
                }
            }
        }

        private void EscapeWithoutTrigger_Start(GenericPuzzleAIBehavior genericAiBehavior, EscapeWithoutTriggerStartAIBehaviorEvent escapeWithoutTriggerStartAIBehaviorEvent)
        {
            Debug.Log(MyLog.Format("AI - EscapeWithoutTrigger_Start"));
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIEscapeWithoutTriggerManager>())
            {
                genericAiBehavior.GetAIManager<AbstractAIEscapeWithoutTriggerManager>().OnEscapeStart(escapeWithoutTriggerStartAIBehaviorEvent);
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIEscapeWithoutTriggerManager>());
            }
        }

        private void Feared_Start(GenericPuzzleAIBehavior genericAiBehavior, FearedStartAIBehaviorEvent fearedStartAIBehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIFearStunManager>())
            {
                genericAiBehavior.GetAIManager<AbstractAIFearStunManager>().OnFearStarted(fearedStartAIBehaviorEvent);
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIFearStunManager>());
            }
        }

        private void Feared_Forced(GenericPuzzleAIBehavior genericAiBehavior, FearedForcedAIBehaviorEvent fearedForcedAIBehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIFearStunManager>())
            {
                genericAiBehavior.GetAIManager<AbstractAIFearStunManager>().OnFearedForced(fearedForcedAIBehaviorEvent);
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIFearStunManager>());
            }
        }

        private void Feared_End(GenericPuzzleAIBehavior genericAiBehavior, FearedEndAIBehaviorEvent fearedEndAIBehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIFearStunManager>())
            {
                genericAiBehavior.SetManagerState(null);
                // to not have inactive frame.
                genericAiBehavior.ForceUpdateAIBehavior();
            }
        }

        private void AttractiveObject_TriggerEnter(GenericPuzzleAIBehavior genericAiBehavior, AttractiveObjectTriggerEnterAIBehaviorEvent attractiveObjectTriggerEnterAIBehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIAttractiveObjectManager>() && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>()))
            {
                Debug.Log(MyLog.Format("AI - OnAttractiveObjectTriggerEnter"));
                genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>().ComponentTriggerEnter(attractiveObjectTriggerEnterAIBehaviorEvent.AttractivePosition, attractiveObjectTriggerEnterAIBehaviorEvent.AttractiveObjectType);
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>());
            }
        }

        private void AttractiveObject_TriggerStay(GenericPuzzleAIBehavior genericAiBehavior, AttractiveObjectTriggerStayAIBehaviorEvent attractiveObjectTriggerStayAIBehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIAttractiveObjectManager>() && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>()))
            {
                //Debug.Log(MyLog.Format("AI - OnAttractiveObjectTriggerStay"));
                genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>().ComponentTriggerStay(attractiveObjectTriggerStayAIBehaviorEvent.AttractivePosition, attractiveObjectTriggerStayAIBehaviorEvent.AttractiveObjectType);
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>());
            }
        }

        private void AttractiveObject_TriggerExit(GenericPuzzleAIBehavior genericAiBehavior, AttractiveObjectTriggerExitAIBehaviorEvent attractiveObjectTriggerExitAIBehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIAttractiveObjectManager>() && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>()))
            {
                Debug.Log(MyLog.Format("AI - OnAttractiveObjectTriggerExit"));
                genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>().ComponentTriggerExit(attractiveObjectTriggerExitAIBehaviorEvent.AttractiveObjectType);
                if (!genericAiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>())
                {
                    genericAiBehavior.SetManagerState(null);
                    genericAiBehavior.ForceUpdateAIBehavior();
                }
            }
        }

        private void AttractiveObject_Destroyed(GenericPuzzleAIBehavior genericAiBehavior, AttractiveObectDestroyedAIBehaviorEvent attractiveObectDestroyedAIBehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIAttractiveObjectManager>())
            {
                genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>().OnAttractiveObjectDestroyed(attractiveObectDestroyedAIBehaviorEvent.DestroyedAttractiveObject);
                // to not have inactive frame.
                genericAiBehavior.ForceUpdateAIBehavior();
            }
        }

        private void TargetZone_TriggerEnter(GenericPuzzleAIBehavior genericAiBehavior, TargetZoneTriggerEnterAIBehaviorEvent targetZoneTriggerEnterAIBehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAITargetZoneManager>() && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>()))
            {
                if (targetZoneTriggerEnterAIBehaviorEvent.TargetZone != null)
                {
                    if (!genericAiBehavior.IsManagerEnabled<AbstractAITargetZoneManager>())
                    {
                        Debug.Log(MyLog.Format("Target zone reset FOV"));
                        genericAiBehavior.AIFOVManager.ResetFOV();
                    }

                    Debug.Log(MyLog.Format("AI - OnTargetZoneTriggerEnter"));
                    genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>().TriggerTargetZoneEscape(targetZoneTriggerEnterAIBehaviorEvent.TargetZone);
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>());
                }
            }
        }

        private void TargetZone_TriggerStay(GenericPuzzleAIBehavior genericAiBehavior, TargetZoneTriggerStayAIBehaviorEvent targetZoneTriggerStayAIBehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAITargetZoneManager>()
                        && !genericAiBehavior.IsCurrentManagerEquals(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>())
                        && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>()))
            {
                if (!genericAiBehavior.IsManagerEnabled<AbstractAITargetZoneManager>())
                {
                    Debug.Log(MyLog.Format("Target zone reset FOV"));
                    genericAiBehavior.AIFOVManager.ResetFOV();
                }
                if (targetZoneTriggerStayAIBehaviorEvent.TargetZone != null)
                {
                    Debug.Log(MyLog.Format("AI - OnTargetZoneTriggerStay"));
                    genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>().TriggerTargetZoneEscape(targetZoneTriggerStayAIBehaviorEvent.TargetZone);
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>());
                }
            }
        }

        private void PlayerEscape_Start(GenericPuzzleAIBehavior genericAiBehavior, PlayerEscapeStartAIBehaviorEvent playerEscapeStartAIBehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractPlayerEscapeManager>())
            {
                if (genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractPlayerEscapeManager>())
                 || genericAiBehavior.IsPlayerEscapeAllowedToInterruptOtherStates())
                {
                    if (this.trackerContainer.GetBehavior<EscapeWhileIgnoringTargetZoneTracker>().IsEscapingWhileIgnoringTargets)
                    {
                        Debug.Log(MyLog.Format("AI - Player escape without colliders."));
                        this.ProcessEvent(new EscapeWithoutTriggerStartAIBehaviorEvent(playerEscapeStartAIBehaviorEvent.PlayerPosition,
                            playerEscapeStartAIBehaviorEvent.AIPlayerEscapeComponent.EscapeSemiAngle,
                            playerEscapeStartAIBehaviorEvent.AIPlayerEscapeComponent.EscapeDistance), genericAiBehavior);
                    }
                    else
                    {
                        Debug.Log(MyLog.Format("AI - Player escape with colliders."));
                        genericAiBehavior.GetAIManager<AbstractPlayerEscapeManager>().OnPlayerEscapeStart();
                        genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractPlayerEscapeManager>());
                    }

                }
            }
        }

        private void SightInRange_Enter(GenericPuzzleAIBehavior genericAiBehavior, SightInRangeEnterAIBehaviorEvent sightInRangeEnterAIBehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIMoveTowardPlayerManager>())
            {
                Debug.Log(MyLog.Format("AI - Sight in range enter."));
                if (genericAiBehavior.GetAIManager<AbstractAIMoveTowardPlayerManager>().OnSightInRangeEnter(sightInRangeEnterAIBehaviorEvent))
                {
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIMoveTowardPlayerManager>());
                }
            }
        }

        private void SightInRange_Exit(GenericPuzzleAIBehavior genericAiBehavior, SightInRangeExitAIBehaviorEvent sightInRangeExitAIBehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIMoveTowardPlayerManager>())
            {
                Debug.Log(MyLog.Format("AI - Sight in range exit."));
                genericAiBehavior.GetAIManager<AbstractAIMoveTowardPlayerManager>().OnSightInRangeExit(sightInRangeExitAIBehaviorEvent);
            }
        }

        private void DisarmingObject_Enter(GenericPuzzleAIBehavior genericAiBehavior, DisarmingObjectEnterAIbehaviorEvent disarmingObjectEnterAIbehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIDisarmObjectManager>())
            {
                if (!genericAiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>() && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAIDisarmObjectManager>()))
                {
                    genericAiBehavior.GetAIManager<AbstractAIDisarmObjectManager>().OnDisarmingObjectStart(disarmingObjectEnterAIbehaviorEvent.DisarmObjectModule);
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIDisarmObjectManager>());
                }

            }
        }

        private void DisarmingObject_Exit(GenericPuzzleAIBehavior genericAiBehavior, DisarmingObjectExitAIbehaviorEvent disarmingObjectExitAIbehaviorEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIDisarmObjectManager>())
            {
                if (genericAiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>())
                {
                    genericAiBehavior.GetAIManager<AbstractAIDisarmObjectManager>().OnDisarmingObjectExit(disarmingObjectExitAIbehaviorEvent.DisarmObjectModule);
                    if (!genericAiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>())
                    {
                        genericAiBehavior.SetManagerState(null);
                    }
                }
            }
        }

    }

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

    public class FearedStartAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        public FearedStartAIBehaviorEvent(Action eventProcessedCallback)
        {
            this.eventProcessedCallback = eventProcessedCallback;
        }
    }

    public class FearedForcedAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private float fearedTime;

        public FearedForcedAIBehaviorEvent(float fearedTime)
        {
            this.fearedTime = fearedTime;
        }

        public float FearedTime { get => fearedTime; set => fearedTime = value; }
    }

    public class FearedEndAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        public FearedEndAIBehaviorEvent(Action eventProcessedCallback)
        {
            this.eventProcessedCallback = eventProcessedCallback;
        }
    }

    public class AttractiveObjectTriggerEnterAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private Vector3 attractivePosition;
        private AttractiveObjectModule attractiveObjectType;

        public AttractiveObjectTriggerEnterAIBehaviorEvent(Vector3 attractivePosition, AttractiveObjectModule attractiveObjectType)
        {
            this.attractivePosition = attractivePosition;
            this.attractiveObjectType = attractiveObjectType;
        }

        public Vector3 AttractivePosition { get => attractivePosition; }
        public AttractiveObjectModule AttractiveObjectType { get => attractiveObjectType; }
    }

    public class AttractiveObjectTriggerStayAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private Vector3 attractivePosition;
        private AttractiveObjectModule attractiveObjectType;

        public AttractiveObjectTriggerStayAIBehaviorEvent(Vector3 attractivePosition, AttractiveObjectModule attractiveObjectType)
        {
            this.attractivePosition = attractivePosition;
            this.attractiveObjectType = attractiveObjectType;
        }

        public Vector3 AttractivePosition { get => attractivePosition; }
        public AttractiveObjectModule AttractiveObjectType { get => attractiveObjectType; }
    }

    public class AttractiveObjectTriggerExitAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {

        private AttractiveObjectModule attractiveObjectType;

        public AttractiveObjectTriggerExitAIBehaviorEvent(AttractiveObjectModule attractiveObjectType)
        {
            this.attractiveObjectType = attractiveObjectType;
        }

        public AttractiveObjectModule AttractiveObjectType { get => attractiveObjectType; }
    }

    public class AttractiveObectDestroyedAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private AttractiveObjectModule destroyedAttractiveObject;

        public AttractiveObectDestroyedAIBehaviorEvent(AttractiveObjectModule destroyedAttractiveObject)
        {
            this.destroyedAttractiveObject = destroyedAttractiveObject;
        }

        public AttractiveObjectModule DestroyedAttractiveObject { get => destroyedAttractiveObject; }
    }

    public class TargetZoneTriggerEnterAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private TargetZoneModule targetZone;

        public TargetZoneTriggerEnterAIBehaviorEvent(TargetZoneModule targetZone)
        {
            this.targetZone = targetZone;
        }

        public TargetZoneModule TargetZone { get => targetZone; }
    }

    public class TargetZoneTriggerStayAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private TargetZoneModule targetZone;

        public TargetZoneTriggerStayAIBehaviorEvent(TargetZoneModule targetZone)
        {
            this.targetZone = targetZone;
        }

        public TargetZoneModule TargetZone { get => targetZone; }
    }

    public class PlayerEscapeStartAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private Vector3 playerPosition;
        private AIPlayerEscapeComponent aIPlayerEscapeComponent;

        public PlayerEscapeStartAIBehaviorEvent(Vector3 playerPosition, AIPlayerEscapeComponent AIPlayerEscapeComponent)
        {
            this.playerPosition = playerPosition;
            this.aIPlayerEscapeComponent = AIPlayerEscapeComponent;
        }

        public Vector3 PlayerPosition { get => playerPosition; }
        public AIPlayerEscapeComponent AIPlayerEscapeComponent { get => aIPlayerEscapeComponent; }
    }

    public class SightInRangeEnterAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private ColliderWithCollisionType colliderWithCollisionType;

        public SightInRangeEnterAIBehaviorEvent(ColliderWithCollisionType colliderWithCollisionType)
        {
            this.colliderWithCollisionType = colliderWithCollisionType;
        }

        public ColliderWithCollisionType ColliderWithCollisionType { get => colliderWithCollisionType; }
    }

    public class SightInRangeExitAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private ColliderWithCollisionType colliderWithCollisionType;

        public SightInRangeExitAIBehaviorEvent(ColliderWithCollisionType colliderWithCollisionType)
        {
            this.colliderWithCollisionType = colliderWithCollisionType;
        }

        public ColliderWithCollisionType ColliderWithCollisionType { get => colliderWithCollisionType; }
    }

    public class DisarmingObjectEnterAIbehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        public DisarmObjectModule DisarmObjectModule;

        public DisarmingObjectEnterAIbehaviorEvent(DisarmObjectModule disarmObjectModule)
        {
            DisarmObjectModule = disarmObjectModule;
        }
    }

    public class DisarmingObjectExitAIbehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        public DisarmObjectModule DisarmObjectModule;

        public DisarmingObjectExitAIbehaviorEvent(DisarmObjectModule disarmObjectModule)
        {
            DisarmObjectModule = disarmObjectModule;
        }
    }

}
