using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class GenericPuzzleAIBehavior : PuzzleAIBehavior<GenericPuzzleAIComponents>
    {
        #region AI Manager data retrieval
        public AbstractAIPatrolComponentManager AIPatrolComponentManager()
        {
            return this.aIBehaviorManagerContainer.GetManager<AbstractAIPatrolComponentManager>(this.aIComponents.AIRandomPatrolComponent);
        }
        public AbstractAIProjectileEscapeManager AIProjectileEscapeWithCollisionManager()
        {
            return this.aIBehaviorManagerContainer.GetManager<AbstractAIProjectileEscapeManager>(this.aIComponents.AIProjectileEscapeWithCollisionComponent);
        }
        public AbstractAIEscapeWithoutTriggerManager AIEscapeWithoutTriggerManager()
        {
            return this.aIBehaviorManagerContainer.GetManager<AbstractAIEscapeWithoutTriggerManager>(this.aIComponents.AIEscapeWithoutTriggerComponent);
        }
        public AbstractAITargetZoneManager AITargetZoneManager()
        {
            return this.aIBehaviorManagerContainer.GetManager<AbstractAITargetZoneManager>(this.aIComponents.AITargetZoneComponent);
        }
        public AbstractAIFearStunManager AIFearStunManager()
        {
            return this.aIBehaviorManagerContainer.GetManager<AbstractAIFearStunManager>(this.aIComponents.AIFearStunComponent);
        }
        public AbstractAIAttractiveObjectManager AIAttractiveObjectManager()
        {
            return this.aIBehaviorManagerContainer.GetManager<AbstractAIAttractiveObjectManager>(this.aIComponents.AIAttractiveObjectComponent);
        }
        public AbstractPlayerEscapeManager AIPlayerEscapeManager()
        {
            return this.aIBehaviorManagerContainer.GetManager<AbstractPlayerEscapeManager>(this.aIComponents.AIPlayerEscapeComponent);
        }
        #endregion

        public GenericPuzzleAIBehavior(NavMeshAgent selfAgent, GenericPuzzleAIComponents aIComponents, Action<FOV> OnFOVChange, Action ForceUpdateAIBehavior,
            PuzzleEventsManager PuzzleEventsManager, TargetZoneContainer TargetZoneContainer,
            AiID aiID, Collider aiCollider, PlayerManagerDataRetriever playerManagerDataRetriever,
            AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent) : base(selfAgent, aIComponents, new GenericPuzzleAIBehaviorExternalEventManager(), OnFOVChange, ForceUpdateAIBehavior)
        {
            Func<AIRandomPatrolComponentMananger> aiPatrolComponentManagerSetterOperation = () => { return new AIRandomPatrolComponentMananger(selfAgent, aIComponents.AIRandomPatrolComponent, aIFOVManager); };
            Func<AIProjectileWithCollisionEscapeManager> AIEscapeWithoutTriggerManagerSetterOperation = () => { return new AIProjectileWithCollisionEscapeManager(selfAgent, aIComponents.AIProjectileEscapeWithCollisionComponent, aIFOVManager, PuzzleEventsManager, aiID, TargetZoneContainer.GetTargetZonesTriggerColliders, AIDestimationMoveManagerComponent); };
            Func<AIEscapeWithoutTriggerManager> AIProjectileEscapeWithoutCollisionManagerSetterOperation = () => { return new AIEscapeWithoutTriggerManager(selfAgent, aIFOVManager, PuzzleEventsManager, aiID, AIDestimationMoveManagerComponent); };
            Func<AIFearStunManager> AIFearStunManagerSetterOperation = () => { return new AIFearStunManager(selfAgent, aIComponents.AIFearStunComponent, PuzzleEventsManager, this.aIFOVManager, aiID); };
            Func<AIAttractiveObjectPersistantManager> AIAttractiveObjectPersistantSetterOperation = () => { return new AIAttractiveObjectPersistantManager(selfAgent, aiID, PuzzleEventsManager); };
            Func<AIAttractiveObjectLooseManager> AIAttractiveObjectLooseSetterOperation = () => { return new AIAttractiveObjectLooseManager(selfAgent, aiID, PuzzleEventsManager); };
            Func<AITargetZoneManager> AITargetZoneManagerSetterOperation = () => { return new AITargetZoneManager(selfAgent, aiCollider, aIComponents.AITargetZoneComponent, this.aIFOVManager, aiID); };
            Func<AIPlayerEscapeManager> AIPlayerEscapeManagerSetterOperation = () => { return new AIPlayerEscapeManager(selfAgent, this.puzzleAIBehaviorExternalEventManager, playerManagerDataRetriever, aIComponents.AIPlayerEscapeComponent, this.AIFOVManager, TargetZoneContainer.GetTargetZonesTriggerColliders, aiID, PuzzleEventsManager, AIDestimationMoveManagerComponent); };

            Func<Type, InterfaceAIManager> ForAllAIManagerTypesOperation = (Type managerType) => AIManagerTypeSafeOperation.ForAllAIManagerTypes(managerType, aiPatrolComponentManagerSetterOperation,
                AIEscapeWithoutTriggerManagerSetterOperation, AIProjectileEscapeWithoutCollisionManagerSetterOperation, AIFearStunManagerSetterOperation,
                AIAttractiveObjectPersistantSetterOperation, AIAttractiveObjectLooseSetterOperation, AITargetZoneManagerSetterOperation, AIPlayerEscapeManagerSetterOperation);

            aIBehaviorManagerContainer = new AIBehaviorManagerContainer(new SortedList<int, InterfaceAIManager>() {
                 { 1, ForAllAIManagerTypesOperation.Invoke(aIComponents.AIFearStunComponent.SelectedManagerType)},
                 { 2, ForAllAIManagerTypesOperation.Invoke(aIComponents.AIEscapeWithoutTriggerComponent.SelectedManagerType)},
                 { 3, ForAllAIManagerTypesOperation.Invoke(aIComponents.AITargetZoneComponent.SelectedManagerType)},
                 { 4, ForAllAIManagerTypesOperation.Invoke(aIComponents.AIPlayerEscapeComponent.SelectedManagerType) },
                 { 5, ForAllAIManagerTypesOperation.Invoke(aIComponents.AIProjectileEscapeWithCollisionComponent.SelectedManagerType) },
                 { 6, ForAllAIManagerTypesOperation.Invoke(aIComponents.AIAttractiveObjectComponent.SelectedManagerType) },
                 { 7, ForAllAIManagerTypesOperation.Invoke(aIComponents.AIRandomPatrolComponent.SelectedManagerType) }
            });

            this.aiBehaviorExternalEventInterruptionMatrix = new Dictionary<Type, List<Func<bool>>>() {
                { typeof(ProjectileTriggerEnterAIBehaviorEvent), new List<Func<bool>>(){
                                this.AIAttractiveObjectManager().IsManagerEnabled,
                                this.AITargetZoneManager().IsManagerEnabled,
                                this.AIPlayerEscapeManager().IsManagerEnabled }
                },
                {
                    //PlayerEscapeStart event interrupt target zone escape
                  typeof(PlayerEscapeStartAIBehaviorEvent), new List<Func<bool>>(){
                            this.AITargetZoneManager().IsManagerEnabled
                  }
                }
            };

        }

        #region External Events
        public override void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy)
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
                    this.ReceiveEvent(new AttractiveObjectTriggerEnterAIBehaviorEvent(collider.transform.position, AttractiveObjectType.GetAttractiveObjectFromCollisionType(collisionType)));
                }
                else if (collisionType.IsTargetZone)
                {
                    this.ReceiveEvent(new TargetZoneTriggerEnterAIBehaviorEvent(TargetZone.FromCollisionType(collisionType)));
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
                    this.ReceiveEvent(new AttractiveObjectTriggerStayAIBehaviorEvent(collider.transform.position, AttractiveObjectType.GetAttractiveObjectFromCollisionType(collisionType)));
                }
                else if (collisionType.IsTargetZone)
                {
                    this.ReceiveEvent(new TargetZoneTriggerStayAIBehaviorEvent(TargetZone.FromCollisionType(collisionType)));
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
                    this.ReceiveEvent(new AttractiveObjectTriggerExitAIBehaviorEvent(AttractiveObjectType.GetAttractiveObjectFromCollisionType(collisionType)));
                }
            }
        }

        public override void OnDestinationReached()
        {
            base.OnDestinationReached();

            if (!this.IsFeared() && !this.IsEscapingWithoutTarget() && !this.IsEscapingFromExitZone() && !this.IsEscapingFromProjectileWithTargetZones() && !this.IsEscapingFromPlayer())
            {
                this.aIFOVManager.ResetFOV();
            }

            this.puzzleAIBehaviorExternalEventManager.AfterDestinationReached(this);
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
            this.AIPatrolComponentManager().GizmoTick();
            this.AIProjectileEscapeWithCollisionManager().GizmoTick();
            aIFOVManager.GizmoTick();
        }

        #region State Retrieval
        public bool IsPatrolling() { return this.AIPatrolComponentManager().IsManagerEnabled(); }
        public bool IsFeared() { return this.AIFearStunManager().IsManagerEnabled(); }
        public bool IsEscapingFromProjectileWithTargetZones() { return this.AIProjectileEscapeWithCollisionManager().IsManagerEnabled(); }
        public bool IsEscapingWithoutTarget() { return this.AIEscapeWithoutTriggerManager().IsManagerEnabled(); }
        public bool IsEscapingFromExitZone() { return this.AITargetZoneManager().IsManagerEnabled(); }
        public bool IsInfluencedByAttractiveObject() { return this.AIAttractiveObjectManager().IsManagerEnabled(); }
        public bool IsEscapingFromPlayer() { return this.AIPlayerEscapeManager().IsManagerEnabled(); }
        #endregion

        public void DebugGUITick()
        {
            GUILayout.Label("StunFeared : " + this.AIFearStunManager().IsManagerEnabled());
            GUILayout.Label("EscapingWithoutTarget : " + this.AIEscapeWithoutTriggerManager().IsManagerEnabled());
            GUILayout.Label("EscapingFromTargetZone : " + this.AITargetZoneManager().IsManagerEnabled());
            GUILayout.Label("Escaping from player : " + this.AIPlayerEscapeManager().IsManagerEnabled());
            GUILayout.Label("ProjEscapingWithTarget : " + this.AIProjectileEscapeWithCollisionManager().IsManagerEnabled());
            GUILayout.Label("Attracted : " + this.AIAttractiveObjectManager().IsManagerEnabled());
            GUILayout.Label("Patrolling : " + this.AIPatrolComponentManager().IsManagerEnabled());
        }

        public override string ToString()
        {
            return String.Format("[StunFeared : {0}, EscapingWithoutTarget : {1}, EscapingFromTargetZone : {2}, ProjEscapingWithTarget : {3}, Attracted : {4}, Patrolling : {5}]",
                new string[] { this.AIFearStunManager().IsManagerEnabled().ToString(), this.AIEscapeWithoutTriggerManager().IsManagerEnabled().ToString(),
            this.AITargetZoneManager().IsManagerEnabled().ToString(), this.AIProjectileEscapeWithCollisionManager().IsManagerEnabled().ToString(),
        this.AIAttractiveObjectManager().IsManagerEnabled().ToString(), this.AIPatrolComponentManager().IsManagerEnabled().ToString() });
        }
    }

}
