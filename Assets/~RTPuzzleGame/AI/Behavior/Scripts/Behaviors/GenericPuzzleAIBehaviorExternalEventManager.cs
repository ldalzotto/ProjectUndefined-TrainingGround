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
            {typeof(AttractiveObjectDestroyedAIBehaviorEvent).Name, 10 },
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

        private Dictionary<Type, Action<GenericPuzzleAIBehavior, GenericPuzzleAIBehaviorExternalEventManager, PuzzleAIBehaviorExternalEvent>> EventsLookup =
            new Dictionary<Type, Action<GenericPuzzleAIBehavior, GenericPuzzleAIBehaviorExternalEventManager, PuzzleAIBehaviorExternalEvent>>()
        {
            { typeof(ProjectileTriggerEnterAIBehaviorEvent), Projectile_TriggerEnter },
            { typeof(EscapeWithoutTriggerStartAIBehaviorEvent), EscapeWithoutTrigger_Start },
                {typeof(FearedStartAIBehaviorEvent),  Feared_Start},
                {typeof(FearedForcedAIBehaviorEvent), Feared_Forced },
                {typeof(FearedEndAIBehaviorEvent), Feared_End },
                {typeof(AttractiveObjectTriggerEnterAIBehaviorEvent), AttractiveObjectAIEvents.AttractiveObject_TriggerEnter },
                {typeof(AttractiveObjectTriggerStayAIBehaviorEvent), AttractiveObjectAIEvents.AttractiveObject_TriggerStay },
                {typeof(AttractiveObjectTriggerExitAIBehaviorEvent), AttractiveObjectAIEvents.AttractiveObject_TriggerExit },
                {typeof(AttractiveObjectDestroyedAIBehaviorEvent), AttractiveObjectAIEvents.AttractiveObject_Destroyed },
                {typeof(TargetZoneTriggerEnterAIBehaviorEvent), TargetZone_TriggerEnter },
                {typeof(TargetZoneTriggerStayAIBehaviorEvent), TargetZone_TriggerStay },
                {typeof(PlayerEscapeStartAIBehaviorEvent), PlayerEscape_Start },
                {typeof(SightInRangeEnterAIBehaviorEvent), SightInRange_Enter },
                {typeof(SightInRangeExitAIBehaviorEvent), SightInRange_Exit },
                {typeof(DisarmingObjectEnterAIbehaviorEvent), DisarmingObject_Enter },
                {typeof(DisarmingObjectExitAIbehaviorEvent), DisarmingObject_Exit },
        };

        public override void ProcessEvent(PuzzleAIBehaviorExternalEvent externalEvent, IPuzzleAIBehavior aiBehavior)
        {
            var genericAiBehavior = (GenericPuzzleAIBehavior)aiBehavior;

            this.EventsLookup.TryGetValue(externalEvent.GetType(), out Action<GenericPuzzleAIBehavior, GenericPuzzleAIBehaviorExternalEventManager, PuzzleAIBehaviorExternalEvent> eventProcessing);
            if (eventProcessing != null)
            {
                eventProcessing.Invoke(genericAiBehavior, this, externalEvent);
            }
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

        private static void Projectile_TriggerEnter(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
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

        private static void EscapeWithoutTrigger_Start(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager, 
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            Debug.Log(MyLog.Format("AI - EscapeWithoutTrigger_Start"));
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIEscapeWithoutTriggerManager>())
            {
                genericAiBehavior.GetAIManager<AbstractAIEscapeWithoutTriggerManager>().OnEscapeStart(PuzzleAIBehaviorExternalEvent.Cast<EscapeWithoutTriggerStartAIBehaviorEvent>());
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIEscapeWithoutTriggerManager>());
            }
        }

        private static void Feared_Start(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIFearStunManager>())
            {
                genericAiBehavior.GetAIManager<AbstractAIFearStunManager>().OnFearStarted(PuzzleAIBehaviorExternalEvent.Cast<FearedStartAIBehaviorEvent>());
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIFearStunManager>());
            }
        }

        private static void Feared_Forced(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIFearStunManager>())
            {
                genericAiBehavior.GetAIManager<AbstractAIFearStunManager>().OnFearedForced(PuzzleAIBehaviorExternalEvent.Cast<FearedForcedAIBehaviorEvent>());
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIFearStunManager>());
            }
        }

        private static void Feared_End(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIFearStunManager>())
            {
                genericAiBehavior.SetManagerState(null);
                // to not have inactive frame.
                genericAiBehavior.ForceUpdateAIBehavior();
            }
        }
        
        private static void TargetZone_TriggerEnter(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAITargetZoneManager>() && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>()))
            {
                var targetZoneTriggerEnterAIBehaviorEvent = PuzzleAIBehaviorExternalEvent.Cast<TargetZoneTriggerEnterAIBehaviorEvent>();
                if (targetZoneTriggerEnterAIBehaviorEvent.TargetZone != null)
                {
                    if (!genericAiBehavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>())
                    {
                        Debug.Log(MyLog.Format("Target zone reset FOV"));
                        genericAiBehavior.FovManagerCalcuation.ResetFOV();
                    }

                    Debug.Log(MyLog.Format("AI - OnTargetZoneTriggerEnter"));
                    genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>().TriggerTargetZoneEscape(targetZoneTriggerEnterAIBehaviorEvent.TargetZone);
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>());
                }
            }
        }

        private static void TargetZone_TriggerStay(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAITargetZoneManager>()
                        && !genericAiBehavior.IsCurrentManagerEquals(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>())
                        && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>()))
            {
                var targetZoneTriggerStayAIBehaviorEvent = PuzzleAIBehaviorExternalEvent.Cast<TargetZoneTriggerStayAIBehaviorEvent>();
                if (!genericAiBehavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>())
                {
                    Debug.Log(MyLog.Format("Target zone reset FOV"));
                    genericAiBehavior.FovManagerCalcuation.ResetFOV();
                }
                if (targetZoneTriggerStayAIBehaviorEvent.TargetZone != null)
                {
                    Debug.Log(MyLog.Format("AI - OnTargetZoneTriggerStay"));
                    genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>().TriggerTargetZoneEscape(targetZoneTriggerStayAIBehaviorEvent.TargetZone);
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>());
                }
            }
        }

        private static void PlayerEscape_Start(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractPlayerEscapeManager>())
            {
                if (genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractPlayerEscapeManager>())
                 || genericAiBehavior.IsPlayerEscapeAllowedToInterruptOtherStates())
                {
                    if (GenericPuzzleAIBehaviorExternalEventManager.GetBehaviorStateTrackerContainer().GetBehavior<EscapeWhileIgnoringTargetZoneTracker>().IsEscapingWhileIgnoringTargets)
                    {
                        Debug.Log(MyLog.Format("AI - Player escape without colliders."));
                        var playerEscapeStartAIBehaviorEvent = PuzzleAIBehaviorExternalEvent.Cast<PlayerEscapeStartAIBehaviorEvent>();
                        GenericPuzzleAIBehaviorExternalEventManager.ProcessEvent(new EscapeWithoutTriggerStartAIBehaviorEvent(playerEscapeStartAIBehaviorEvent.PlayerPosition,
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

        private static void SightInRange_Enter(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIMoveTowardPlayerManager>())
            {
                Debug.Log(MyLog.Format("AI - Sight in range enter."));
                if (genericAiBehavior.GetAIManager<AbstractAIMoveTowardPlayerManager>().OnSightInRangeEnter(PuzzleAIBehaviorExternalEvent.Cast<SightInRangeEnterAIBehaviorEvent>()))
                {
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIMoveTowardPlayerManager>());
                }
            }
        }

        private static void SightInRange_Exit(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIMoveTowardPlayerManager>())
            {
                Debug.Log(MyLog.Format("AI - Sight in range exit."));
                genericAiBehavior.GetAIManager<AbstractAIMoveTowardPlayerManager>().OnSightInRangeExit(PuzzleAIBehaviorExternalEvent.Cast<SightInRangeExitAIBehaviorEvent>());
            }
        }

        private static void DisarmingObject_Enter(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIDisarmObjectManager>())
            {
                if (!genericAiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>() && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAIDisarmObjectManager>()))
                {
                    genericAiBehavior.GetAIManager<AbstractAIDisarmObjectManager>().OnDisarmingObjectStart(PuzzleAIBehaviorExternalEvent.Cast<DisarmingObjectEnterAIbehaviorEvent>().DisarmObjectModule);
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIDisarmObjectManager>());
                }

            }
        }

        private static void DisarmingObject_Exit(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIDisarmObjectManager>())
            {
                if (genericAiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>())
                {
                    genericAiBehavior.GetAIManager<AbstractAIDisarmObjectManager>().OnDisarmingObjectExit(PuzzleAIBehaviorExternalEvent.Cast<DisarmingObjectEnterAIbehaviorEvent>().DisarmObjectModule);
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
