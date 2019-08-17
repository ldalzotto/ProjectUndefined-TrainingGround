using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class AISightVision : AbstractAIManager
    {
        #region Internal Dependencies
        private PuzzleAIBehaviorExternalEventManager PuzzleAIBehaviorExternalEventManager;
        #endregion

        private RangeTypeObject sightVisionRange;
        private AISightVisionTargetTracker AISightVisionTargetTracker;
        private AISightIntersectionManager AISightInteresectionManager;

        public void Init(PuzzleAIBehaviorExternalEventManager puzzleAIBehaviorExternalEventManager)
        {
            this.AISightVisionTargetTracker = new AISightVisionTargetTracker(this);
            this.AISightInteresectionManager = new AISightIntersectionManager(this.AISightVisionTargetTracker, puzzleAIBehaviorExternalEventManager);
            this.sightVisionRange = GetComponentInChildren<RangeTypeObject>();
            this.sightVisionRange.Init(new RangeTypeObjectInitializer(), new List<RangeTypeObjectEventListener>() { this.AISightVisionTargetTracker });
        }

        public void Tick(float d)
        {
            this.sightVisionRange.Tick(d);
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

        private AISightVision AISightVisionRef;
        private Dictionary<Collider, ColliderWithCollisionType> trackedColliders;

        public AISightVisionTargetTracker(AISightVision AISightVisionRef)
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
        private PuzzleAIBehaviorExternalEventManager PuzzleAIBehaviorExternalEventManager;

        private AISightVisionTargetTracker aISightVisionTargetTracker;

        private Dictionary<ColliderWithCollisionType, Vector3> lastFramePosition;
        private Dictionary<ColliderWithCollisionType, bool> isInside;

        public Dictionary<ColliderWithCollisionType, bool> IsInside { get => isInside; }

        public AISightIntersectionManager(AISightVisionTargetTracker aISightVisionTargetTracker, PuzzleAIBehaviorExternalEventManager PuzzleAIBehaviorExternalEventManager)
        {
            this.PuzzleAIBehaviorExternalEventManager = PuzzleAIBehaviorExternalEventManager;
            this.aISightVisionTargetTracker = aISightVisionTargetTracker;
            this.lastFramePosition = new Dictionary<ColliderWithCollisionType, Vector3>();
            this.isInside = new Dictionary<ColliderWithCollisionType, bool>();
        }

        public void Tick(float d, ref RangeTypeObject sightVisionRange)
        {
            foreach (var trackedCollider in this.aISightVisionTargetTracker.GetTrackedColliders())
            {
                if (this.lastFramePosition.ContainsKey(trackedCollider))
                {
                    if (this.lastFramePosition[trackedCollider] != trackedCollider.collider.transform.position)
                    {
                        this.SetIsInside(trackedCollider, sightVisionRange.IsInsideAndNotOccluded((BoxCollider)trackedCollider.collider));
                    }
                }
                else
                {
                    this.SetIsInside(trackedCollider, sightVisionRange.IsInsideAndNotOccluded((BoxCollider)trackedCollider.collider));
                }

                this.lastFramePosition[trackedCollider] = trackedCollider.collider.transform.position;
            }
        }

        private void SetIsInside(ColliderWithCollisionType trackedCollider, bool value)
        {
            if (this.isInside.ContainsKey(trackedCollider))
            {
                bool oldValue = this.isInside[trackedCollider];
                if (!oldValue && value)
                {
                    this.PuzzleAIBehaviorExternalEventManager.ReceiveEvent(new SightInRangeEnterAIBehaviorEvent(trackedCollider));
                }
                else if (oldValue && !value)
                {
                    this.PuzzleAIBehaviorExternalEventManager.ReceiveEvent(new SightInRangeExitAIBehaviorEvent(trackedCollider));
                }
            }
            else
            {
                if (value)
                {
                    this.PuzzleAIBehaviorExternalEventManager.ReceiveEvent(new SightInRangeEnterAIBehaviorEvent(trackedCollider));
                }
                else
                {
                    this.PuzzleAIBehaviorExternalEventManager.ReceiveEvent(new SightInRangeExitAIBehaviorEvent(trackedCollider));
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

        #region Logical Conditions
        public bool IsPlayerInSight()
        {
            foreach(var insideCollider in this.isInside.Keys)
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
}
