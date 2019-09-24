using CoreGame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class ObjectSightModule : InteractiveObjectModule
    {
        private RangeTypeObject sightVisionRange;
        private AISightVisionTargetTracker AISightVisionTargetTracker;
        private AISightIntersectionManager AISightInteresectionManager;

        public RangeTypeObject SightVisionRange { get => sightVisionRange; }

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.ResolveInternalDependencies();
            this.AISightVisionTargetTracker = new AISightVisionTargetTracker(this);
            this.AISightInteresectionManager = new AISightIntersectionManager(this.AISightVisionTargetTracker, this, interactiveObjectInitializationObject.ParentAIObjectTypeReference);
            this.sightVisionRange.Init(new RangeTypeObjectInitializer(), new List<RangeTypeObjectEventListener>() { this.AISightVisionTargetTracker });
        }

        public void ResolveInternalDependencies()
        {
            this.sightVisionRange = GetComponentInChildren<RangeTypeObject>();
        }

        public void TickBeforeAIUpdate(float d)
        {
            //Ranges are update in container
            //  this.sightVisionRange.Tick(d);
            this.AISightInteresectionManager.Tick(d, ref this.sightVisionRange);
        }

        #region Internal Events    
        public void OnTargetTriggerExit(CollisionType CollisionType)
        {
            this.AISightInteresectionManager.OnTargetTriggerExit(CollisionType);
        }
        #endregion

        #region Logical Conditions
        public bool IsPlayerInSight() { return this.AISightInteresectionManager.IsPlayerInSight(); }
        #endregion

#if UNITY_EDITOR
        public void HandlesTick()
        {
            foreach (var rangeType in GetComponentsInChildren<RangeType>().ToList())
            {
                rangeType.HandlesDraw();
            }
        }
#endif

    }

    public class AISightVisionTargetTracker : RangeTypeObjectEventListener
    {

        private ObjectSightModule AISightVisionRef;

        public AISightVisionTargetTracker(ObjectSightModule AISightVisionRef)
        {
            this.AISightVisionRef = AISightVisionRef;
        }

        public void OnRangeTriggerEnter(CollisionType other)
        {
        }

        public void OnRangeTriggerExit(CollisionType collisionType)
        {
            if (collisionType != null && collisionType.IsPlayer)
            {
                this.AISightVisionRef.OnTargetTriggerExit(collisionType);
            }

        }
    }

    public class AISightIntersectionManager
    {
        private AIObjectDataRetriever associatedAI;
        private AISightVisionTargetTracker aISightVisionTargetTracker;

        private TransformChangeListenerManager sightModuleMovementChangeTracker;
        private Dictionary<CollisionType, TransformChangeListenerManager> inRangeCollidersMovementChangeTrackers;
        private Dictionary<CollisionType, bool> isInside;

        public Dictionary<CollisionType, bool> IsInside { get => isInside; }

        public AISightIntersectionManager(AISightVisionTargetTracker aISightVisionTargetTracker, ObjectSightModule ObjectSightModule, AIObjectDataRetriever associatedAI)
        {
            this.associatedAI = associatedAI;
            this.aISightVisionTargetTracker = aISightVisionTargetTracker;
            this.sightModuleMovementChangeTracker = new TransformChangeListenerManager(ObjectSightModule.transform, true, true);
            this.inRangeCollidersMovementChangeTrackers = new Dictionary<CollisionType, TransformChangeListenerManager>();
            this.isInside = new Dictionary<CollisionType, bool>();
        }

        public void Tick(float d, ref RangeTypeObject sightVisionRange)
        {
            this.sightModuleMovementChangeTracker.Tick();

            foreach (var trackedCollider in sightVisionRange.RangeColliderTrackerModule.GetTrackedPlayerColliders())
            {
                if (this.inRangeCollidersMovementChangeTrackers.ContainsKey(trackedCollider))
                {
                    this.inRangeCollidersMovementChangeTrackers[trackedCollider].Tick();
                    //if target collider moves
                    if (this.inRangeCollidersMovementChangeTrackers[trackedCollider].TransformChangedThatFrame()
                         || this.sightModuleMovementChangeTracker.TransformChangedThatFrame())
                    {
                        this.SetIsInside(trackedCollider, sightVisionRange.IsInsideAndNotOccluded((BoxCollider)trackedCollider.GetAssociatedCollider(), forceObstacleOcclusionIfNecessary: true));
                    }
                }
                else
                {
                    this.inRangeCollidersMovementChangeTrackers[trackedCollider] = new TransformChangeListenerManager(trackedCollider.GetAssociatedCollider().transform, true, true);
                    this.SetIsInside(trackedCollider, sightVisionRange.IsInsideAndNotOccluded((BoxCollider)trackedCollider.GetAssociatedCollider(), forceObstacleOcclusionIfNecessary: true));
                }
            }
        }

        private void SetIsInside(CollisionType trackedCollider, bool value)
        {
            if (this.isInside.ContainsKey(trackedCollider))
            {
                bool oldValue = this.isInside[trackedCollider];
                if (!oldValue && value)
                {
                    this.SightInRangeEnter(trackedCollider);
                }
                else if (oldValue && !value)
                {
                    this.SightInRangeExit(trackedCollider);
                }
            }
            else
            {
                if (value)
                {
                    this.SightInRangeEnter(trackedCollider);
                }
                else
                {
                    this.SightInRangeExit(trackedCollider);
                }
            }
            this.isInside[trackedCollider] = value;
        }

        #region External Event
        public void OnTargetTriggerExit(CollisionType ColliderWithCollisionType)
        {
            this.SetIsInside(ColliderWithCollisionType, false);
        }
        #endregion

        #region Internal Event
        private void SightInRangeEnter(CollisionType trackedCollider)
        {
            associatedAI.GetAIBehavior().ReceiveEvent(new SightInRangeEnterAIBehaviorEvent(trackedCollider));
        }
        private void SightInRangeExit(CollisionType trackedCollider)
        {
            associatedAI.GetAIBehavior().ReceiveEvent(new SightInRangeExitAIBehaviorEvent(trackedCollider));
        }
        #endregion

        #region Logical Conditions
        public bool IsPlayerInSight()
        {
            foreach (var insideCollider in this.isInside.Keys)
            {
                if (insideCollider.IsPlayer)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }


}
