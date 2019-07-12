using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
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

        #region Data Retrieval
        public AbstractAIPatrolComponentManager AIPatrolComponentManager { get => aIPatrolComponentManager; }
        public AbstractAIProjectileEscapeManager AIProjectileEscapeManager { get => aIProjectileEscapeManager; }
        public AbstractAIEscapeWithoutTriggerManager AIEscapeWithoutTriggerManager { get => aIEscapeWithoutTriggerManager; }
        public AbstractAITargetZoneManager AITargetZoneManager { get => aITargetZoneManager; }
        public AbstractAIFearStunManager AIFearStunManager { get => aIFearStunManager; }
        public AbstractAIAttractiveObjectManager AIAttractiveObjectManager { get => aIAttractiveObjectManager; }
        public AbstractPlayerEscapeManager PlayerEscapeManager { get => playerEscapeManager; }
        #endregion

        public GenericPuzzleAIBehavior(NavMeshAgent selfAgent, GenericPuzzleAIComponents aIComponents, Action<FOV> OnFOVChange, Action ForceUpdateAIBehavior,
            PuzzleEventsManager PuzzleEventsManager, InteractiveObjectContainer InteractiveObjectContainer,
            AiID aiID, Collider aiCollider, PlayerManagerDataRetriever playerManagerDataRetriever,
            AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent) : base(selfAgent, aIComponents, new GenericPuzzleAIBehaviorExternalEventManager(), OnFOVChange, ForceUpdateAIBehavior)
        {

            aIComponents.AIRandomPatrolComponent.IfSelectedTypeDefined((selectedType) => this.aIPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, aIComponents.AIRandomPatrolComponent, aIFOVManager));
            aIComponents.AIProjectileEscapeWithCollisionComponent.IfSelectedTypeDefined((selectedType) => this.aIProjectileEscapeManager = new AIProjectileWithCollisionEscapeManager(selfAgent, aIComponents.AIProjectileEscapeWithCollisionComponent, aIFOVManager, PuzzleEventsManager, aiID, () => { return TargetZoneHelper.GetTargetZonesTriggerColliders(InteractiveObjectContainer); }, AIDestimationMoveManagerComponent));
            aIComponents.AIEscapeWithoutTriggerComponent.IfSelectedTypeDefined((selectedType) => this.aIEscapeWithoutTriggerManager = new AIEscapeWithoutTriggerManager(selfAgent, aIFOVManager, PuzzleEventsManager, aiID, AIDestimationMoveManagerComponent));
            aIComponents.AIFearStunComponent.IfSelectedTypeDefined((selectedType) => this.aIFearStunManager = new AIFearStunManager(selfAgent, aIComponents.AIFearStunComponent, PuzzleEventsManager, this.aIFOVManager, aiID));
            aIComponents.AIAttractiveObjectComponent.IfSelectedTypeDefined((selectedType) =>
            {
                if (selectedType == typeof(AIAttractiveObjectPersistantManager))
                {
                    this.aIAttractiveObjectManager = new AIAttractiveObjectPersistantManager(selfAgent, aiID, PuzzleEventsManager);
                }
                else
                {
                    this.aIAttractiveObjectManager = new AIAttractiveObjectLooseManager(selfAgent, aiID, PuzzleEventsManager);
                }
            });
            aIComponents.AITargetZoneComponent.IfSelectedTypeDefined((selectedType) => this.aITargetZoneManager = new AITargetZoneManager(selfAgent, aiCollider, aIComponents.AITargetZoneComponent, this.aIFOVManager, aiID));
            aIComponents.AIPlayerEscapeComponent.IfSelectedTypeDefined((selectedType) => this.playerEscapeManager = new AIPlayerEscapeManager(selfAgent, this.puzzleAIBehaviorExternalEventManager, playerManagerDataRetriever, aIComponents.AIPlayerEscapeComponent, this.AIFOVManager, () => { return TargetZoneHelper.GetTargetZonesTriggerColliders(InteractiveObjectContainer); }, aiID, PuzzleEventsManager, AIDestimationMoveManagerComponent));

            this.aIBehaviorManagerContainer = new AIBehaviorManagerContainer(new SortedList<int, InterfaceAIManager>() {
                 { 1, this.aIFearStunManager },
                 { 2, this.aIEscapeWithoutTriggerManager },
                 { 3, this.aITargetZoneManager },
                 { 4, this.playerEscapeManager },
                 { 5, this.aIProjectileEscapeManager },
                 { 6, this.aIAttractiveObjectManager },
                 { 7, this.aIPatrolComponentManager }
            });

            this.aiBehaviorExternalEventInterruptionMatrix = new Dictionary<Type, List<Func<bool>>>() {
                { typeof(ProjectileTriggerEnterAIBehaviorEvent), new List<Func<bool>>(){
                                this.aIAttractiveObjectManager.IsManagerEnabled,
                                this.aITargetZoneManager.IsManagerEnabled,
                                this.playerEscapeManager.IsManagerEnabled }
                },
                {
                    //PlayerEscapeStart event interrupt target zone escape
                  typeof(PlayerEscapeStartAIBehaviorEvent), new List<Func<bool>>(){
                            this.aITargetZoneManager.IsManagerEnabled
                  }
                }
            };

            this.AfterChildInit();
        }

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
            this.aIPatrolComponentManager.GizmoTick();
            this.aIProjectileEscapeManager.GizmoTick();
            aIFOVManager.GizmoTick();
        }

        #region State Retrieval
        public bool IsPatrolling() { return this.aIPatrolComponentManager.IsManagerEnabled(); }
        public bool IsFeared() { return this.aIFearStunManager.IsManagerEnabled(); }
        public bool IsEscapingFromProjectileWithTargetZones() { return this.aIProjectileEscapeManager.IsManagerEnabled(); }
        public bool IsEscapingWithoutTarget() { return this.aIEscapeWithoutTriggerManager.IsManagerEnabled(); }
        public bool IsEscapingFromExitZone() { return this.aITargetZoneManager.IsManagerEnabled(); }
        public bool IsInfluencedByAttractiveObject() { return this.aIAttractiveObjectManager.IsManagerEnabled(); }
        public bool IsEscapingFromPlayer() { return this.playerEscapeManager.IsManagerEnabled(); }
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
