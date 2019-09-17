using CoreGame;
using GameConfigurationID;
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

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, InteractiveObjectType interactiveObjectType)
        {
            this.ResolveInternalDependencies();
            this.AISightVisionTargetTracker = new AISightVisionTargetTracker(this);
            this.AISightInteresectionManager = new AISightIntersectionManager(this.AISightVisionTargetTracker, this);
            this.sightVisionRange.Init(new RangeTypeObjectInitializer(), new List<RangeTypeObjectEventListener>() { this.AISightVisionTargetTracker });
        }

        private void ResolveInternalDependencies()
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
        public void OnTargetTriggerExit(ColliderWithCollisionType ColliderWithCollisionType)
        {
            this.AISightInteresectionManager.OnTargetTriggerExit(ColliderWithCollisionType);
        }
        #endregion

        #region Logical Conditions
        public bool IsPlayerInSight() { return this.AISightInteresectionManager.IsPlayerInSight(); }
        #endregion

        public void RegisterSightTrackingListener(SightTrackingListener SightTrackingListener)
        {
            this.AISightInteresectionManager.RegisterSightTrackingListener(SightTrackingListener);
        }

#if UNITY_EDITOR
        public void HandlesTick()
        {
            foreach (var rangeType in GetComponentsInChildren<RangeType>().ToList())
            {
                rangeType.HandlesDraw();
            }
        }
#endif

        public static class ObjectSightModuleInstancer
        {
            public static void PopuplateFromDefinition(ObjectSightModule objectSightModule, ObjectSightModuleDefinition objectSightModuleDefinition, PuzzlePrefabConfiguration puzzlePrefabConfiguration)
            {
                objectSightModule.ResolveInternalDependencies();
                objectSightModule.transform.localPosition = objectSightModuleDefinition.LocalPosition;
                objectSightModule.transform.localRotation = objectSightModuleDefinition.LocalRotation;
                if (objectSightModuleDefinition.RangeTypeObjectDefinitionIDPicker)
                {
                    objectSightModule.sightVisionRange.RangeTypeObjectDefinitionID = objectSightModuleDefinition.RangeTypeObjectDefinitionID;
                }
                else
                {
                    objectSightModule.sightVisionRange.RangeTypeObjectDefinitionID = RangeTypeObjectDefinitionID.NONE;
                    objectSightModuleDefinition.RangeTypeObjectDefinitionInherentData.DefineRangeTypeObject(objectSightModule.sightVisionRange, puzzlePrefabConfiguration);
                }

            }
        }
    }

    public class AISightVisionTargetTracker : RangeTypeObjectEventListener
    {

        private ObjectSightModule AISightVisionRef;
        private Dictionary<Collider, ColliderWithCollisionType> trackedColliders;

        public AISightVisionTargetTracker(ObjectSightModule AISightVisionRef)
        {
            this.AISightVisionRef = AISightVisionRef;
            this.trackedColliders = new Dictionary<Collider, ColliderWithCollisionType>();
        }

        public List<ColliderWithCollisionType> GetTrackedColliders() { return trackedColliders.Values.ToList(); }

        public void OnRangeTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsPlayer)
            {
                this.trackedColliders.Add(other, new ColliderWithCollisionType(other, collisionType));
            }
        }

        public void OnRangeTriggerExit(Collider other)
        {
            if (this.trackedColliders.ContainsKey(other))
            {
                this.AISightVisionRef.OnTargetTriggerExit(this.trackedColliders[other]);
                this.trackedColliders.Remove(other);
            }
        }
    }

    public class AISightIntersectionManager
    {
        private AISightVisionTargetTracker aISightVisionTargetTracker;
        private List<SightTrackingListener> SightTrackingListeners;

        private TransformChangeListenerManager sightModuleMovementChangeTracker;
        private Dictionary<ColliderWithCollisionType, TransformChangeListenerManager> inRangeCollidersMovementChangeTrackers;
        private Dictionary<ColliderWithCollisionType, bool> isInside;

        public Dictionary<ColliderWithCollisionType, bool> IsInside { get => isInside; }

        public AISightIntersectionManager(AISightVisionTargetTracker aISightVisionTargetTracker, ObjectSightModule ObjectSightModule)
        {
            this.aISightVisionTargetTracker = aISightVisionTargetTracker;
            this.sightModuleMovementChangeTracker = new TransformChangeListenerManager(ObjectSightModule.transform, true, true);
            this.inRangeCollidersMovementChangeTrackers = new Dictionary<ColliderWithCollisionType, TransformChangeListenerManager>();
            this.isInside = new Dictionary<ColliderWithCollisionType, bool>();
            this.SightTrackingListeners = new List<SightTrackingListener>();
        }

        public void Tick(float d, ref RangeTypeObject sightVisionRange)
        {
            this.sightModuleMovementChangeTracker.Tick();

            foreach (var trackedCollider in this.aISightVisionTargetTracker.GetTrackedColliders())
            {
                if (this.inRangeCollidersMovementChangeTrackers.ContainsKey(trackedCollider))
                {
                    this.inRangeCollidersMovementChangeTrackers[trackedCollider].Tick();
                    //if target collider moves
                    if (this.inRangeCollidersMovementChangeTrackers[trackedCollider].TransformChangedThatFrame()
                         || this.sightModuleMovementChangeTracker.TransformChangedThatFrame())
                    {
                        this.SetIsInside(trackedCollider, sightVisionRange.IsInsideAndNotOccluded((BoxCollider)trackedCollider.collider));
                    }
                }
                else
                {
                    this.inRangeCollidersMovementChangeTrackers[trackedCollider] = new TransformChangeListenerManager(trackedCollider.collisionType.transform, true, true);
                    this.SetIsInside(trackedCollider, sightVisionRange.IsInsideAndNotOccluded((BoxCollider)trackedCollider.collider));
                }
            }
        }

        private void SetIsInside(ColliderWithCollisionType trackedCollider, bool value)
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
        public void OnTargetTriggerExit(ColliderWithCollisionType ColliderWithCollisionType)
        {
            this.SetIsInside(ColliderWithCollisionType, false);
        }
        #endregion

        #region Internal Event
        private void SightInRangeEnter(ColliderWithCollisionType trackedCollider)
        {
            foreach (var sightTrackingListener in this.SightTrackingListeners)
            {
                sightTrackingListener.SightInRangeEnter(trackedCollider);
            }
        }
        private void SightInRangeExit(ColliderWithCollisionType trackedCollider)
        {
            foreach (var sightTrackingListener in this.SightTrackingListeners)
            {
                sightTrackingListener.SightInRangeExit(trackedCollider);
            }
        }
        #endregion

        public void RegisterSightTrackingListener(SightTrackingListener SightTrackingListener)
        {
            this.SightTrackingListeners.Add(SightTrackingListener);
        }

        #region Logical Conditions
        public bool IsPlayerInSight()
        {
            foreach (var insideCollider in this.isInside.Keys)
            {
                if (insideCollider.collisionType.IsPlayer)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }

    public class ColliderWithCollisionType
    {
        public Collider collider;
        public CollisionType collisionType;

        public ColliderWithCollisionType(Collider collider, CollisionType collisionType)
        {
            this.collider = collider;
            this.collisionType = collisionType;
        }

        public override bool Equals(object obj)
        {
            var type = obj as ColliderWithCollisionType;
            return type != null &&
                   EqualityComparer<Collider>.Default.Equals(collider, type.collider) &&
                   EqualityComparer<CollisionType>.Default.Equals(collisionType, type.collisionType);
        }

        public override int GetHashCode()
        {
            var hashCode = 1177991260;
            hashCode = hashCode * -1521134295 + EqualityComparer<Collider>.Default.GetHashCode(collider);
            hashCode = hashCode * -1521134295 + EqualityComparer<CollisionType>.Default.GetHashCode(collisionType);
            return hashCode;
        }
    }

    public interface SightTrackingListener
    {
        void SightInRangeEnter(ColliderWithCollisionType trackedCollider);
        void SightInRangeExit(ColliderWithCollisionType trackedCollider);
    }
}
