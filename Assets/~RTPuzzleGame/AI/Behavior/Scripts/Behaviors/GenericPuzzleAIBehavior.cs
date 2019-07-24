using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class GenericPuzzleAIBehavior : PuzzleAIBehavior<GenericPuzzleAIComponents>
    {
        private AbstractAIPatrolComponentManager aIPatrolComponentManager;
        private AbstractAIProjectileEscapeManager aIProjectileEscapeManager;
        private AbstractAIEscapeWithoutTriggerManager aIEscapeWithoutTriggerManager;
        private AbstractAITargetZoneManager aITargetZoneManager;
        private AbstractAIFearStunManager aIFearStunManager;
        private AbstractAIAttractiveObjectManager aIAttractiveObjectManager;
        private AbstractPlayerEscapeManager playerEscapeManager;
        private AIPlayerAttractiveManager aIPlayerAttractiveManager;

        #region Data Retrieval
        public AbstractAIPatrolComponentManager AIPatrolComponentManager { get => aIPatrolComponentManager; }
        public AbstractAIProjectileEscapeManager AIProjectileEscapeManager { get => aIProjectileEscapeManager; }
        public AbstractAIEscapeWithoutTriggerManager AIEscapeWithoutTriggerManager { get => aIEscapeWithoutTriggerManager; }
        public AbstractAITargetZoneManager AITargetZoneManager { get => aITargetZoneManager; }
        public AbstractAIFearStunManager AIFearStunManager { get => aIFearStunManager; }
        public AbstractAIAttractiveObjectManager AIAttractiveObjectManager { get => aIAttractiveObjectManager; }
        public AbstractPlayerEscapeManager PlayerEscapeManager { get => playerEscapeManager; }
        public AIPlayerAttractiveManager AIPlayerAttractiveManager { get => aIPlayerAttractiveManager; }
        #endregion

        public void Init(NavMeshAgent selfAgent, GenericPuzzleAIComponents aIComponents, Action<FOV> OnFOVChange, Action ForceUpdateAIBehavior,
            PuzzleEventsManager PuzzleEventsManager, InteractiveObjectContainer InteractiveObjectContainer,
            AiID aiID, Collider aiCollider, PlayerManagerDataRetriever playerManagerDataRetriever,
            AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent)
        {
            this.BaseInit(selfAgent, aIComponents, new GenericPuzzleAIBehaviorExternalEventManager(), OnFOVChange, ForceUpdateAIBehavior);

            this.GetComponentInChildren<AIRandomPatrolComponentMananger>().IfNotNull(AIRandomPatrolComponentMananger =>
            {
                AIRandomPatrolComponentMananger.Init(selfAgent, aIComponents.AIRandomPatrolComponent, aIFOVManager);
                this.aIPatrolComponentManager = AIRandomPatrolComponentMananger;
            });
            this.GetComponentInChildren<AIScriptedPatrolComponentManager>().IfNotNull(AIScriptedPatrolComponentManager =>
            {
                this.aIPatrolComponentManager = AIScriptedPatrolComponentManager;
            });
            this.GetComponentInChildren<AIProjectileWithCollisionEscapeManager>().IfNotNull(AIProjectileWithCollisionEscapeManager =>
            {
                AIProjectileWithCollisionEscapeManager.Init(selfAgent, aIComponents.AIProjectileEscapeWithCollisionComponent, aIFOVManager, PuzzleEventsManager, aiID, () => { return TargetZoneHelper.GetTargetZonesTriggerColliders(InteractiveObjectContainer); }, AIDestimationMoveManagerComponent);
                this.aIProjectileEscapeManager = AIProjectileWithCollisionEscapeManager;
            });
            this.GetComponentInChildren<AIEscapeWithoutTriggerManager>().IfNotNull(AIEscapeWithoutTriggerManager =>
            {
                AIEscapeWithoutTriggerManager.Init(selfAgent, aIFOVManager, PuzzleEventsManager, aiID, AIDestimationMoveManagerComponent);
                this.aIEscapeWithoutTriggerManager = AIEscapeWithoutTriggerManager;
            });
            this.GetComponentInChildren<AIFearStunManager>().IfNotNull(AIFearStunManager =>
            {
                AIFearStunManager.Init(selfAgent, aIComponents.AIFearStunComponent, PuzzleEventsManager, this.aIFOVManager, aiID);
                this.aIFearStunManager = AIFearStunManager;
            });
            this.GetComponentInChildren<AIAttractiveObjectPersistantManager>().IfNotNull(AIAttractiveObjectPersistantManager =>
            {
                AIAttractiveObjectPersistantManager.Init(selfAgent, aiID, PuzzleEventsManager);
                this.aIAttractiveObjectManager = AIAttractiveObjectPersistantManager;
            });
            this.GetComponentInChildren<AIAttractiveObjectLooseManager>().IfNotNull(AIAttractiveObjectLooseManager =>
            {
                AIAttractiveObjectLooseManager.Init(selfAgent, aiID, PuzzleEventsManager);
                this.aIAttractiveObjectManager = AIAttractiveObjectLooseManager;
            });
            this.GetComponentInChildren<AITargetZoneEscapeManager>().IfNotNull(AITargetZoneEscapeManager =>
            {
                AITargetZoneEscapeManager.Init(selfAgent, aiCollider, aIComponents.AITargetZoneComponent, this.aIFOVManager, aiID);
                this.aITargetZoneManager = AITargetZoneEscapeManager;
            });
            this.GetComponentInChildren<AIPlayerEscapeManager>().IfNotNull(AIPlayerEscapeManager =>
            {
                AIPlayerEscapeManager.Init(selfAgent, this.puzzleAIBehaviorExternalEventManager, playerManagerDataRetriever, aIComponents.AIPlayerEscapeComponent, this.AIFOVManager, () => { return TargetZoneHelper.GetTargetZonesTriggerColliders(InteractiveObjectContainer); }, aiID, PuzzleEventsManager, AIDestimationMoveManagerComponent);
                this.playerEscapeManager = AIPlayerEscapeManager;
            });
            this.GetComponentInChildren<AIPlayerAttractiveManager>().IfNotNull(AIPlayerAttractiveManager =>
            {
                AIPlayerAttractiveManager.Init(this.aiSightVision);
                this.aIPlayerAttractiveManager = AIPlayerAttractiveManager;
            });

            var dic = new Dictionary<int, InterfaceAIManager>()
            {
                 { 1, this.aIFearStunManager },
                 { 2, this.aIEscapeWithoutTriggerManager },
                 { 3, this.aITargetZoneManager },
                 { 4, this.playerEscapeManager },
                 { 5, this.aIProjectileEscapeManager },
                 { 6, this.aIPlayerAttractiveManager},
                 { 7, this.aIAttractiveObjectManager },
                 { 8, this.aIPatrolComponentManager }
            };
            this.aIBehaviorManagerContainer = new AIBehaviorManagerContainer(new SortedList<int, InterfaceAIManager>(
                dic.Select(s => s).Where(s => s.Value != null).ToDictionary(s => s.Key, s => s.Value)
                ));

            this.AfterChildInit();

        }


        #region Logical Conditions
        public bool IsProjectileTriggerAllowedToInterruptOtherStates()
        {
            return (this.IsAttractiveObjectsEnabled() && this.aIAttractiveObjectManager.IsManagerEnabled())
                        || (this.IsEscapeFromTargetZoneEnabled() && this.aITargetZoneManager.IsManagerEnabled())
                        || (this.IsPlayerEscapeEnabled() && this.playerEscapeManager.IsManagerEnabled()
                        || (this.IsPlayerAttractiveEnabled() && this.aIPlayerAttractiveManager.IsManagerEnabled()));
        }
        public bool IsPlayerEscapeAllowedToInterruptOtherStates()
        {
            return ((this.IsEscapeFromTargetZoneEnabled() && this.aITargetZoneManager.IsManagerEnabled())
                  || (this.IsPlayerAttractiveEnabled() && this.aIPlayerAttractiveManager.IsManagerEnabled()));
        }
        #endregion

        protected override void BeforeManagersUpdate(float d, float timeAttenuationFactor)
        {
            this.aIFearStunManager.IfNotNull((AbstractAIFearStunManager AbstractAIFearStunManager) => AbstractAIFearStunManager.BeforeManagersUpdate(d, timeAttenuationFactor));
            this.playerEscapeManager.IfNotNull((AbstractPlayerEscapeManager AbstractPlayerEscapeManager) => AbstractPlayerEscapeManager.BeforeManagersUpdate(d, timeAttenuationFactor));
        }

        public override void OnDestinationReached()
        {
            this.aIPatrolComponentManager.IfNotNull((aIPatrolComponentManager) => aIPatrolComponentManager.OnDestinationReached());
            this.aIProjectileEscapeManager.IfNotNull((aIProjectileEscapeManager) => aIProjectileEscapeManager.OnDestinationReached());
            this.aIEscapeWithoutTriggerManager.IfNotNull((aIEscapeWithoutTriggerManager) => aIEscapeWithoutTriggerManager.OnDestinationReached());
            this.aITargetZoneManager.IfNotNull((aITargetZoneManager) => aITargetZoneManager.OnDestinationReached());
            this.aIAttractiveObjectManager.IfNotNull((aIAttractiveObjectManager) => aIAttractiveObjectManager.OnDestinationReached());
            this.playerEscapeManager.IfNotNull((playerEscapeManager) => playerEscapeManager.OnDestinationReached());
            this.aIPlayerAttractiveManager.IfNotNull((aIPlayerAttractiveManager) => aIPlayerAttractiveManager.OnDestinationReached());

            if (!this.IsFeared() && !this.IsEscapingWithoutTarget() && !this.IsEscapingFromExitZone() && !this.IsEscapingFromProjectileWithTargetZones() && !this.IsEscapingFromPlayer())
            {
                this.aIFOVManager.ResetFOV();
            }

            this.puzzleAIBehaviorExternalEventManager.AfterDestinationReached(this);
        }

        #region External Events
        public override void OnAttractiveObjectDestroyed(AttractiveObjectTypeModule attractiveObjectToDestroy)
        {
            this.ReceiveEvent(new AttractiveObectDestroyedAIBehaviorEvent(attractiveObjectToDestroy));
        }
        public override void OnTriggerEnter(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsRTAttractiveObject)
                {
                    this.ReceiveEvent(new AttractiveObjectTriggerEnterAIBehaviorEvent(collider.transform.position, AttractiveObjectTypeModule.GetAttractiveObjectFromCollisionType(collisionType)));
                }
                else if (collisionType.IsTargetZone)
                {
                    this.ReceiveEvent(new TargetZoneTriggerEnterAIBehaviorEvent(TargetZoneObjectModule.FromCollisionType(collisionType)));
                }
            }
        }

        public override void OnTriggerStay(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsRTAttractiveObject)
                {
                    this.ReceiveEvent(new AttractiveObjectTriggerStayAIBehaviorEvent(collider.transform.position, AttractiveObjectTypeModule.GetAttractiveObjectFromCollisionType(collisionType)));
                }
                else if (collisionType.IsTargetZone)
                {
                    this.ReceiveEvent(new TargetZoneTriggerStayAIBehaviorEvent(TargetZoneObjectModule.FromCollisionType(collisionType)));
                }
            }
        }

        public override void OnTriggerExit(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsRTAttractiveObject)
                {
                    this.ReceiveEvent(new AttractiveObjectTriggerExitAIBehaviorEvent(AttractiveObjectTypeModule.GetAttractiveObjectFromCollisionType(collisionType)));
                }
            }
        }
        #endregion

        #region Data Retrieval
        public FOV GetFOV()
        {
            return this.aIFOVManager.GetFOV();
        }
        #endregion

        public override void TickGizmo()
        {
            this.aIPatrolComponentManager.IfNotNull(aIPatrolComponentManager => aIPatrolComponentManager.GizmoTick());
            this.aIProjectileEscapeManager.IfNotNull(aIProjectileEscapeManager => aIProjectileEscapeManager.GizmoTick());
            aIFOVManager.GizmoTick();
        }

        #region Module Fonctionality
        public bool IsPatrollingEnabled() { return this.aIPatrolComponentManager != null; }
        public bool IsEscapingFromProjectileWithTargetZonesEnabled() { return this.aIProjectileEscapeManager != null; }
        public bool IsEscapeWithoutTriggerEnabled() { return this.aIEscapeWithoutTriggerManager != null; }
        public bool IsFearEnabled() { return this.aIFearStunManager != null; }
        public bool IsAttractiveObjectsEnabled() { return this.aIAttractiveObjectManager != null; }
        public bool IsEscapeFromTargetZoneEnabled() { return this.aITargetZoneManager != null; }
        public bool IsPlayerEscapeEnabled() { return this.playerEscapeManager != null; }
        public bool IsPlayerAttractiveEnabled() { return this.aIPlayerAttractiveManager != null; }
        #endregion

        #region State Retrieval
        public bool IsPatrolling() { if (this.IsPatrollingEnabled()) { return this.aIPatrolComponentManager.IsManagerEnabled(); } else { return false; } }
        public bool IsFeared() { if (this.IsFearEnabled()) { return this.aIFearStunManager.IsManagerEnabled(); } else { return false; } }
        public bool IsEscapingFromProjectileWithTargetZones() { if (this.IsEscapingFromProjectileWithTargetZonesEnabled()) { return this.aIProjectileEscapeManager.IsManagerEnabled(); } else { return false; } }
        public bool IsEscapingWithoutTarget() { if (this.IsEscapeWithoutTriggerEnabled()) { return this.aIEscapeWithoutTriggerManager.IsManagerEnabled(); } else { return false; } }
        public bool IsEscapingFromExitZone() { if (this.IsEscapeFromTargetZoneEnabled()) { return this.aITargetZoneManager.IsManagerEnabled(); } else { return false; } }
        public bool IsInfluencedByAttractiveObject() { if (this.IsAttractiveObjectsEnabled()) { return this.aIAttractiveObjectManager.IsManagerEnabled(); } else { return false; } }
        public bool IsEscapingFromPlayer() { if (this.IsPlayerEscapeEnabled()) { return this.playerEscapeManager.IsManagerEnabled(); } else { return false; } }
        public bool IsAttractedFromPlayer() { if (this.IsPlayerAttractiveEnabled()) { return this.aIPlayerAttractiveManager.IsManagerEnabled(); } else { return false; } }
        #endregion

        public void DebugGUITick()
        {
            GUILayout.Label("State : " + this.currentManagerState.GetType().Name);
        }

        public override string ToString()
        {
            return String.Format("[StunFeared : {0}, EscapingWithoutTarget : {1}, EscapingFromTargetZone : {2}, ProjEscapingWithTarget : {3}, Attracted : {4}, Patrolling : {5}]",
                new string[] { this.aIFearStunManager.IsManagerEnabled().ToString(), this.aIEscapeWithoutTriggerManager.IsManagerEnabled().ToString(),
            this.aITargetZoneManager.IsManagerEnabled().ToString(), this.aIProjectileEscapeManager.IsManagerEnabled().ToString(),
        this.aIAttractiveObjectManager.IsManagerEnabled().ToString(), this.aIPatrolComponentManager.IsManagerEnabled().ToString() });
        }
    }

}
