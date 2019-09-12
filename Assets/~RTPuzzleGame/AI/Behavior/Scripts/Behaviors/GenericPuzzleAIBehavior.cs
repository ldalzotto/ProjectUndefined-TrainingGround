using CoreGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class GenericPuzzleAIBehavior : PuzzleAIBehavior
    {
        private GenericPuzzleAIBehaviorContainer GenericPuzzleAIBehaviorContainer;

        #region Data Retrieval
        public AbstractAIPatrolComponentManager AIPatrolComponentManager { get => GenericPuzzleAIBehaviorContainer.AIPatrolComponentManager; }
        public AbstractAIProjectileEscapeManager AIProjectileEscapeManager { get => GenericPuzzleAIBehaviorContainer.AIProjectileEscapeManager; }
        public AbstractAIEscapeWithoutTriggerManager AIEscapeWithoutTriggerManager { get => GenericPuzzleAIBehaviorContainer.AIEscapeWithoutTriggerManager; }
        public AbstractAITargetZoneManager AITargetZoneManager { get => GenericPuzzleAIBehaviorContainer.AITargetZoneManager; }
        public AbstractAIFearStunManager AIFearStunManager { get => GenericPuzzleAIBehaviorContainer.AIFearStunManager; }
        public AbstractAIAttractiveObjectManager AIAttractiveObjectManager { get => GenericPuzzleAIBehaviorContainer.AIAttractiveObjectManager; }
        public AbstractPlayerEscapeManager PlayerEscapeManager { get => GenericPuzzleAIBehaviorContainer.PlayerEscapeManager; }
        public AIMoveTowardPlayerManager AIPlayerMoveTowardPlayerManager { get => GenericPuzzleAIBehaviorContainer.AIMoveTowardPlayerManager; }
        public AbstractAIDisarmObjectManager AIDisarmObjectManager { get => GenericPuzzleAIBehaviorContainer.AIDisarmObjectManager; }
        #endregion

        public void Init(GenericPuzzleAIBehaviorContainer GenericPuzzleAIBehaviorContainer, AIBheaviorBuildInputData AIBheaviorBuildInputData)
        {
            this.GenericPuzzleAIBehaviorContainer = GenericPuzzleAIBehaviorContainer;
            this.BaseInit(AIBheaviorBuildInputData);

            GenericPuzzleAIBehaviorContainer.AIPatrolComponentManager.IfNotNull(AIRandomPatrolComponentMananger =>
            {
                if (AIPatrolComponentManager.GetType() == typeof(AIRandomPatrolComponentMananger))
                {
                    ((AIRandomPatrolComponentMananger)AIRandomPatrolComponentMananger).Init(selfAgent, aIFOVManager, AIBheaviorBuildInputData.aiID);
                }
                else if (AIPatrolComponentManager.GetType() == typeof(AIScriptedPatrolComponentManager))
                {
                    ((AIScriptedPatrolComponentManager)AIRandomPatrolComponentMananger).Init(selfAgent, aIFOVManager, AIBheaviorBuildInputData.aiID, AIBheaviorBuildInputData.AIPositionsManager,
                            AIBheaviorBuildInputData.AssociatedInteractiveObject, AIBheaviorBuildInputData.AIObjectTypeSpeedSetter);
                }
            });

            GenericPuzzleAIBehaviorContainer.AIProjectileEscapeManager.IfNotNull(AIProjectileWithCollisionEscapeManager =>
            {
                ((AIProjectileWithCollisionEscapeManager)AIProjectileWithCollisionEscapeManager).Init(selfAgent, aIFOVManager, AIBheaviorBuildInputData.PuzzleEventsManager, AIBheaviorBuildInputData.aiID, () => { return TargetZoneHelper.GetTargetZonesTriggerColliders(AIBheaviorBuildInputData.InteractiveObjectContainer); }, AIBheaviorBuildInputData.TransformMoveManagerComponent);
            });

            GenericPuzzleAIBehaviorContainer.AIEscapeWithoutTriggerManager.IfNotNull(AIEscapeWithoutTriggerManager =>
            {
                ((AIEscapeWithoutTriggerManager)AIEscapeWithoutTriggerManager).Init(selfAgent, aIFOVManager, AIBheaviorBuildInputData.PuzzleEventsManager, AIBheaviorBuildInputData.aiID, AIBheaviorBuildInputData.TransformMoveManagerComponent);
            });

            GenericPuzzleAIBehaviorContainer.AIFearStunManager.IfNotNull(AIFearStunManager =>
            {
                ((AIFearStunManager)AIFearStunManager).Init(selfAgent, AIBheaviorBuildInputData.PuzzleEventsManager, this.aIFOVManager, AIBheaviorBuildInputData.aiID);
            });

            GenericPuzzleAIBehaviorContainer.AIAttractiveObjectManager.IfNotNull(AIAttractiveObjectManager =>
            {
                if (AIAttractiveObjectManager.GetType() == typeof(AIAttractiveObjectPersistantManager))
                {
                    ((AIAttractiveObjectPersistantManager)AIAttractiveObjectManager).Init(selfAgent, AIBheaviorBuildInputData.aiID, AIBheaviorBuildInputData.PuzzleEventsManager, AIBheaviorBuildInputData.AIObjectTypeSpeedSetter);
                }
                else if (AIAttractiveObjectManager.GetType() == typeof(AIAttractiveObjectLooseManager))
                {
                    ((AIAttractiveObjectLooseManager)AIAttractiveObjectManager).Init(selfAgent, AIBheaviorBuildInputData.aiID, AIBheaviorBuildInputData.PuzzleEventsManager, AIBheaviorBuildInputData.AIObjectTypeSpeedSetter);
                }
            });
            GenericPuzzleAIBehaviorContainer.AITargetZoneManager.IfNotNull(AITargetZoneEscapeManager =>
            {
                ((AITargetZoneEscapeManager)AITargetZoneEscapeManager).Init(selfAgent, AIBheaviorBuildInputData.aiCollider, this.aIFOVManager, AIBheaviorBuildInputData.aiID);
            });
            GenericPuzzleAIBehaviorContainer.PlayerEscapeManager.IfNotNull(AIPlayerEscapeManager =>
            {
                ((AIPlayerEscapeManager)AIPlayerEscapeManager).Init(selfAgent, this.puzzleAIBehaviorExternalEventManager, AIBheaviorBuildInputData.PlayerManagerDataRetriever, this.AIFOVManager, () => { return TargetZoneHelper.GetTargetZonesTriggerColliders(AIBheaviorBuildInputData.InteractiveObjectContainer); }, AIBheaviorBuildInputData.aiID, AIBheaviorBuildInputData.PuzzleEventsManager, AIBheaviorBuildInputData.TransformMoveManagerComponent);
            });
            GenericPuzzleAIBehaviorContainer.AIDisarmObjectManager.IfNotNull(AIDisarmObjectManager =>
            {
                ((AIDisarmObjectManager)AIDisarmObjectManager).Init(selfAgent, AIBheaviorBuildInputData.PuzzleEventsManager, AIBheaviorBuildInputData.aiID);
            });
            GenericPuzzleAIBehaviorContainer.AIMoveTowardPlayerManager.IfNotNull(AIMoveTowardPlayerManager =>
            {
                ((AIMoveTowardPlayerManager)AIMoveTowardPlayerManager).Init(AIBheaviorBuildInputData.AIObjectTypeSpeedSetter);
            });

            var dic = new Dictionary<int, InterfaceAIManager>()
            {
                 { 1, this.AIFearStunManager },
                 { 2, this.AIEscapeWithoutTriggerManager },
                 { 3, this.AITargetZoneManager },
                 { 4, this.PlayerEscapeManager },
                 { 5, this.AIProjectileEscapeManager },
                 { 6, this.AIPlayerMoveTowardPlayerManager},
                 { 7, this.AIDisarmObjectManager },
                 { 8, this.AIAttractiveObjectManager },
                 { 9, this.AIPatrolComponentManager }
            };
            this.aIBehaviorManagerContainer = new AIBehaviorManagerContainer(new SortedList<int, InterfaceAIManager>(
                dic.Select(s => s).Where(s => s.Value != null).ToDictionary(s => s.Key, s => s.Value)
                ));

            this.AfterChildInit();

        }


        #region Logical Conditions
        public bool IsProjectileTriggerAllowedToInterruptOtherStates()
        {
            return (this.IsAttractiveObjectsEnabled() && this.AIAttractiveObjectManager.IsManagerEnabled())
                        || (this.IsEscapeFromTargetZoneEnabled() && this.AITargetZoneManager.IsManagerEnabled())
                        || (this.IsPlayerEscapeEnabled() && this.PlayerEscapeManager.IsManagerEnabled()
                        || (this.IsMovignTowardPlayerEnabled() && this.AIPlayerMoveTowardPlayerManager.IsManagerEnabled()));
        }
        public bool IsPlayerEscapeAllowedToInterruptOtherStates()
        {
            return ((this.IsEscapeFromTargetZoneEnabled() && this.AITargetZoneManager.IsManagerEnabled())
                  || (this.IsMovignTowardPlayerEnabled() && this.AIPlayerMoveTowardPlayerManager.IsManagerEnabled()));
        }
        #endregion

        protected override void BeforeManagersUpdate(float d, float timeAttenuationFactor)
        {
            this.AIFearStunManager.IfNotNull((AbstractAIFearStunManager AbstractAIFearStunManager) => AbstractAIFearStunManager.BeforeManagersUpdate(d, timeAttenuationFactor));
            this.PlayerEscapeManager.IfNotNull((AbstractPlayerEscapeManager AbstractPlayerEscapeManager) => AbstractPlayerEscapeManager.BeforeManagersUpdate(d, timeAttenuationFactor));
        }

        public override void OnDestinationReached()
        {
            this.AIPatrolComponentManager.IfNotNull((aIPatrolComponentManager) => aIPatrolComponentManager.OnDestinationReached());
            this.AIProjectileEscapeManager.IfNotNull((aIProjectileEscapeManager) => aIProjectileEscapeManager.OnDestinationReached());
            this.AIEscapeWithoutTriggerManager.IfNotNull((aIEscapeWithoutTriggerManager) => aIEscapeWithoutTriggerManager.OnDestinationReached());
            this.AITargetZoneManager.IfNotNull((aITargetZoneManager) => aITargetZoneManager.OnDestinationReached());
            this.AIAttractiveObjectManager.IfNotNull((aIAttractiveObjectManager) => aIAttractiveObjectManager.OnDestinationReached());
            this.PlayerEscapeManager.IfNotNull((playerEscapeManager) => playerEscapeManager.OnDestinationReached());
            this.AIPlayerMoveTowardPlayerManager.IfNotNull((aIPlayerAttractiveManager) => aIPlayerAttractiveManager.OnDestinationReached());

            if (!this.IsFeared() && !this.IsEscapingWithoutTarget() && !this.IsEscapingFromExitZone() && !this.IsEscapingFromProjectileWithTargetZones() && !this.IsEscapingFromPlayer())
            {
                this.aIFOVManager.ResetFOV();
            }

            this.puzzleAIBehaviorExternalEventManager.AfterDestinationReached(this);
        }

        #region External Events
        public override void OnAttractiveObjectDestroyed(AttractiveObjectModule attractiveObjectToDestroy)
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
                    this.ReceiveEvent(new AttractiveObjectTriggerEnterAIBehaviorEvent(collider.transform.position, AttractiveObjectModule.GetAttractiveObjectFromCollisionType(collisionType)));
                }
                else if (collisionType.IsTargetZone)
                {
                    this.ReceiveEvent(new TargetZoneTriggerEnterAIBehaviorEvent(TargetZoneModule.FromCollisionType(collisionType)));
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
                    this.ReceiveEvent(new AttractiveObjectTriggerStayAIBehaviorEvent(collider.transform.position, AttractiveObjectModule.GetAttractiveObjectFromCollisionType(collisionType)));
                }
                else if (collisionType.IsTargetZone)
                {
                    this.ReceiveEvent(new TargetZoneTriggerStayAIBehaviorEvent(TargetZoneModule.FromCollisionType(collisionType)));
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
                    this.ReceiveEvent(new AttractiveObjectTriggerExitAIBehaviorEvent(AttractiveObjectModule.GetAttractiveObjectFromCollisionType(collisionType)));
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
            this.AIPatrolComponentManager.IfNotNull(aIPatrolComponentManager => aIPatrolComponentManager.GizmoTick());
            this.AIProjectileEscapeManager.IfNotNull(aIProjectileEscapeManager => aIProjectileEscapeManager.GizmoTick());
            aIFOVManager.GizmoTick();
        }

        #region Module Fonctionality
        public bool IsPatrollingEnabled() { return this.AIPatrolComponentManager != null; }
        public bool IsEscapingFromProjectileWithTargetZonesEnabled() { return this.AIProjectileEscapeManager != null; }
        public bool IsEscapeWithoutTriggerEnabled() { return this.AIEscapeWithoutTriggerManager != null; }
        public bool IsFearEnabled() { return this.AIFearStunManager != null; }
        public bool IsAttractiveObjectsEnabled() { return this.AIAttractiveObjectManager != null; }
        public bool IsEscapeFromTargetZoneEnabled() { return this.AITargetZoneManager != null; }
        public bool IsPlayerEscapeEnabled() { return this.PlayerEscapeManager != null; }
        public bool IsMovignTowardPlayerEnabled() { return this.AIPlayerMoveTowardPlayerManager != null; }
        public bool IsDisarmingObjectEnabled() { return this.AIDisarmObjectManager != null; }
        #endregion

        #region State Retrieval
        public bool IsPatrolling() { if (this.IsPatrollingEnabled()) { return this.AIPatrolComponentManager.IsManagerEnabled(); } else { return false; } }
        public bool IsFeared() { if (this.IsFearEnabled()) { return this.AIFearStunManager.IsManagerEnabled(); } else { return false; } }
        public bool IsEscapingFromProjectileWithTargetZones() { if (this.IsEscapingFromProjectileWithTargetZonesEnabled()) { return this.AIProjectileEscapeManager.IsManagerEnabled(); } else { return false; } }
        public bool IsEscapingWithoutTarget() { if (this.IsEscapeWithoutTriggerEnabled()) { return this.AIEscapeWithoutTriggerManager.IsManagerEnabled(); } else { return false; } }
        public bool IsEscapingFromExitZone() { if (this.IsEscapeFromTargetZoneEnabled()) { return this.AITargetZoneManager.IsManagerEnabled(); } else { return false; } }
        public bool IsInfluencedByAttractiveObject() { if (this.IsAttractiveObjectsEnabled()) { return this.AIAttractiveObjectManager.IsManagerEnabled(); } else { return false; } }
        public bool IsEscapingFromPlayer() { if (this.IsPlayerEscapeEnabled()) { return this.PlayerEscapeManager.IsManagerEnabled(); } else { return false; } }
        public bool IsMovingTowardPlayer() { if (this.IsMovignTowardPlayerEnabled()) { return this.AIPlayerMoveTowardPlayerManager.IsManagerEnabled(); } else { return false; } }
        public bool IsDisarmingObject() { if (this.IsDisarmingObjectEnabled()) { return this.AIDisarmObjectManager.IsManagerEnabled(); } else { return false; } }
        #endregion

        public void DebugGUITick()
        {
            GUILayout.Label("State : " + this.currentManagerState.GetType().Name);
        }

        public override string ToString()
        {
            return String.Format("[StunFeared : {0}, EscapingWithoutTarget : {1}, EscapingFromTargetZone : {2}, ProjEscapingWithTarget : {3}, Attracted : {4}, Patrolling : {5}]",
                new string[] { this.AIFearStunManager.IsManagerEnabled().ToString(), this.AIEscapeWithoutTriggerManager.IsManagerEnabled().ToString(),
            this.AITargetZoneManager.IsManagerEnabled().ToString(), this.AIProjectileEscapeManager.IsManagerEnabled().ToString(),
        this.AIAttractiveObjectManager.IsManagerEnabled().ToString(), this.AIPatrolComponentManager.IsManagerEnabled().ToString() });
        }
    }

    public class GenericPuzzleAIBehaviorContainer
    {
        private AbstractAIPatrolComponentManager aIPatrolComponentManager;
        private AbstractAIProjectileEscapeManager aIProjectileEscapeManager;
        private AbstractAIEscapeWithoutTriggerManager aIEscapeWithoutTriggerManager;
        private AbstractAITargetZoneManager aITargetZoneManager;
        private AbstractAIFearStunManager aIFearStunManager;
        private AbstractAIAttractiveObjectManager aIAttractiveObjectManager;
        private AbstractPlayerEscapeManager playerEscapeManager;
        private AIMoveTowardPlayerManager aIMoveTowardPlayerManager;
        private AbstractAIDisarmObjectManager aIDisarmObjectManager;

        public AbstractAIPatrolComponentManager AIPatrolComponentManager { get => aIPatrolComponentManager; set => aIPatrolComponentManager = value; }
        public AbstractAIProjectileEscapeManager AIProjectileEscapeManager { get => aIProjectileEscapeManager; set => aIProjectileEscapeManager = value; }
        public AbstractAIEscapeWithoutTriggerManager AIEscapeWithoutTriggerManager { get => aIEscapeWithoutTriggerManager; set => aIEscapeWithoutTriggerManager = value; }
        public AbstractAITargetZoneManager AITargetZoneManager { get => aITargetZoneManager; set => aITargetZoneManager = value; }
        public AbstractAIFearStunManager AIFearStunManager { get => aIFearStunManager; set => aIFearStunManager = value; }
        public AbstractAIAttractiveObjectManager AIAttractiveObjectManager { get => aIAttractiveObjectManager; set => aIAttractiveObjectManager = value; }
        public AbstractPlayerEscapeManager PlayerEscapeManager { get => playerEscapeManager; set => playerEscapeManager = value; }
        public AIMoveTowardPlayerManager AIMoveTowardPlayerManager { get => aIMoveTowardPlayerManager; set => aIMoveTowardPlayerManager = value; }
        public AbstractAIDisarmObjectManager AIDisarmObjectManager { get => aIDisarmObjectManager; set => aIDisarmObjectManager = value; }
    }
}
