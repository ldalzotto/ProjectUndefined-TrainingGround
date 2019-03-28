﻿using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class MouseAIBehavior : PuzzleAIBehavior<AIComponents>
    {

        #region AI Components
        private AbstractAIPatrolComponentManager AIPatrolComponentManager;
        private AbstractAIProjectileEscapeManager AIProjectileEscapeManager;
        private AbstractAITargetZoneManager AITargetZoneComponentManager;
        private AIFearStunManager AIFearStunComponentManager;
        private AbstractAIAttractiveObjectManager AIAttractiveObjectManager;
        #endregion

        public MouseAIBehavior(NavMeshAgent selfAgent, AIComponents aIComponents, Action<FOV> OnFOVChange, PuzzleEventsManager PuzzleEventsManager, AiID aiID) : base(selfAgent, aIComponents, OnFOVChange)
        {
            AITargetZoneComponentManager = new AITargetZoneManager(selfAgent, aIComponents.AITargetZoneComponent);

            var aiManagerSetter = new AIManagerTypeSafeOperation
            {
                AIRandomPatrolComponentManangerOperation = () => { this.AIPatrolComponentManager = new AIRandomPatrolComponentMananger(selfAgent, aIComponents.AIRandomPatrolComponent, AIFOVManager); return null; },
                AIProjectileEscapeManagerOperation = () =>
                {
                    this.AIProjectileEscapeManager = new AIProjectileEscapeManager(selfAgent, aIComponents.AIProjectileEscapeComponent, aIComponents.AITargetZoneComponent, AIFOVManager, PuzzleEventsManager, aiID,
                   AITargetZoneComponentManager.AITargetZoneComponentManagerDataRetrieval); return null;
                },
                AIFearStunManagerOperation = () => { this.AIFearStunComponentManager = new AIFearStunManager(selfAgent, aIComponents.AIFearStunComponent, PuzzleEventsManager, aiID); return null; },
                AIAttractiveObjectOperation = () => { this.AIAttractiveObjectManager = new AIAttractiveObjectManager(selfAgent, aiID, PuzzleEventsManager); return null; },
                AITargetZoneManagerOperation = () => { this.AITargetZoneComponentManager = new AITargetZoneManager(selfAgent, aIComponents.AITargetZoneComponent); return null; }
            };


            aiManagerSetter.ForAllAIManagerTypes(aIComponents.AIAttractiveObjectComponent.SelectedManagerType);
            aiManagerSetter.ForAllAIManagerTypes(aIComponents.AIFearStunComponent.SelectedManagerType);
            aiManagerSetter.ForAllAIManagerTypes(aIComponents.AIProjectileEscapeComponent.SelectedManagerType);
            aiManagerSetter.ForAllAIManagerTypes(aIComponents.AIRandomPatrolComponent.SelectedManagerType);
            aiManagerSetter.ForAllAIManagerTypes(aIComponents.AITargetZoneComponent.SelectedManagerType);

        }

        #region External Events
        public override void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy)
        {
            if (AIAttractiveObjectManager.IsDestructedAttractiveObjectEqualsToCurrent(attractiveObjectToDestroy))
            {
                AIAttractiveObjectManager.ClearAttractedObject();
                this.OnDestinationReached();
            }
        }
        #endregion

        #region Logical Conditions
        private bool IsReactingToProjectile()
        {
            return !this.IsFeared();
        }
        #endregion

        #region Data Retrieval
        public FOV GetFOV()
        {
            return this.AIFOVManager.GetFOV();
        }
        #endregion

        public override Nullable<Vector3> TickAI(in float d, in float timeAttenuationFactor)
        {
            AITargetZoneComponentManager.TickComponent();

            Vector3? newDirection = null;

            if (this.IsFeared())
            {
                AIFearStunComponentManager.TickWhileFeared(d, timeAttenuationFactor);
            }
            else
            {
                var fearStunPosition = AIFearStunComponentManager.TickComponent(AIFOVManager);
                if (this.IsFeared())
                {
                    newDirection = fearStunPosition.Value; //position is the agent itself
                }
                else
                {
                    if (this.IsEscaping())
                    {
                        newDirection = AIProjectileEscapeManager.GetCurrentEscapeDirection();
                    }
                    else
                    {
                        if (this.IsInTargetZone())
                        {
                            newDirection = AIProjectileEscapeManager.ForceComputeEscapePoint();
                        }
                        else
                        {
                            if (this.IsInfluencedByAttractiveObject())
                            {
                                newDirection = AIAttractiveObjectManager.TickComponent();
                            }
                            else
                            {
                                newDirection = AIPatrolComponentManager.TickComponent();
                            }
                        }
                    }
                }
            }

            return newDirection;
        }

        public override void TickGizmo()
        {
            AIPatrolComponentManager.GizmoTick();
            AIProjectileEscapeManager.GizmoTick();
            AIFOVManager.GizmoTick();
        }

        public override void OnProjectileTriggerEnter(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            this.OnDestinationReached(true, false, true); //no reset of projectile fov FOV intersection
            AIProjectileEscapeManager.OnTriggerEnter(collider, collisionType);
        }

        public override void OnTriggerEnter(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsRTAttractiveObject)
                {
                    if (!AIProjectileEscapeManager.IsEscaping() && !this.IsFeared())
                    {
                        this.OnDestinationReached();
                        AIAttractiveObjectManager.OnTriggerEnter(collider, collisionType);
                    }
                }
            }
        }

        public override void OnTriggerStay(Collider collider)
        {
            var collisionType = collider.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (!this.IsEscaping() && !this.IsFeared())
                {
                    AIAttractiveObjectManager.OnTriggerStay(collider, collisionType);
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
                    if (!AIProjectileEscapeManager.IsEscaping() && !this.IsFeared())
                    {
                        AIAttractiveObjectManager.OnTriggerExit(collider, collisionType);
                    }
                }
            }
        }

        private void OnDestinationReached(bool randomPatrolReset, bool projectileEscapeReset, bool attractiveObjectReset)
        {
            if (randomPatrolReset)
            {
                AIPatrolComponentManager.OnDestinationReached();
            }

            if (projectileEscapeReset)
            {
                AIProjectileEscapeManager.OnDestinationReached();
            }

            if (attractiveObjectReset)
            {
                AIAttractiveObjectManager.OnDestinationReached();
            }
        }

        public override void OnDestinationReached()
        {
            bool resetAttractiveObjectComponent = !this.AIAttractiveObjectManager.HasSensedThePresenceOfAnAttractiveObject(); //reset only if attractive object
            this.OnDestinationReached(true, true, resetAttractiveObjectComponent);
        }

        #region State Retrieval
        public bool IsPatrolling() { return AIPatrolComponentManager.IsPatrolling(); }
        public bool IsFeared() { return AIFearStunComponentManager.IsFeared; }
        public bool IsEscaping() { return AIProjectileEscapeManager.IsEscaping(); }
        public bool IsInTargetZone() { return AITargetZoneComponentManager.IsInTargetZone(); }
        public bool IsInfluencedByAttractiveObject() { return AIAttractiveObjectManager.IsInfluencedByAttractiveObject(); }
        #endregion

        public void DebugGUITick()
        {
            GUILayout.Label("IsPatrolling : " + AIPatrolComponentManager.IsPatrolling());
            GUILayout.Label("IsEscaping : " + AIProjectileEscapeManager.IsEscaping());
            GUILayout.Label("IsInTargetZone : " + AITargetZoneComponentManager.IsInTargetZone());
            GUILayout.Label("IsStunFeared : " + AIFearStunComponentManager.IsFeared);
            GUILayout.Label("IsAttracted : " + AIAttractiveObjectManager.IsInfluencedByAttractiveObject());
        }
    }

}
