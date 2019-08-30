using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class ObjectRepelLineVisualFeedbackManager : MonoBehaviour
    {
        #region External Dependencies
        private PuzzleGameConfigurationManager puzzleGameConfigurationManager;
        private DottedLineContainer dottedLineContainer;
        #endregion

        private Dictionary<RangeType, List<ObjectRepelModule>> inRangeRepelableObjects;
        private Dictionary<ObjectRepelModule, List<InRangeRepelableObjectFXManager>> inRangeRepelableObjectFXManagers;

        public void Init()
        {
            #region External Dependencies
            this.puzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            this.dottedLineContainer = GameObject.FindObjectOfType<DottedLineContainer>();
            #endregion

            this.inRangeRepelableObjects = new Dictionary<RangeType, List<ObjectRepelModule>>();
            this.inRangeRepelableObjectFXManagers = new Dictionary<ObjectRepelModule, List<InRangeRepelableObjectFXManager>>();
        }

        public void Tick(float d)
        {
            foreach (var inRangeRepelableObject in this.inRangeRepelableObjects)
            {
                foreach (var inRangeColliderTracker in inRangeRepelableObject.Value)
                {
                    foreach (var inRangeRepelableFXManager in inRangeRepelableObjectFXManagers[inRangeColliderTracker])
                    {
                        inRangeRepelableFXManager.Tick(d, inRangeRepelableObject.Key, inRangeColliderTracker);
                    }
                }
            }
        }

        #region External Events
        public void OnRangeDestroyed(RangeType rangeType)
        {
            if (rangeType.RangeTypeID == RangeTypeID.LAUNCH_PROJECTILE_CURSOR)
            {
                if (this.inRangeRepelableObjects.ContainsKey(rangeType))
                {
                    foreach (var inRangeColliderTrackerRemoved in this.inRangeRepelableObjects[rangeType])
                    {
                        if (this.inRangeRepelableObjectFXManagers.ContainsKey(inRangeColliderTrackerRemoved))
                        {
                            foreach (var inRangeRepelableObjectFXManager in this.inRangeRepelableObjectFXManagers[inRangeColliderTrackerRemoved])
                            {
                                inRangeRepelableObjectFXManager.OnRangeExit();
                            }
                            this.inRangeRepelableObjectFXManagers.Remove(inRangeColliderTrackerRemoved);
                        }
                    }
                    this.inRangeRepelableObjects.Remove(rangeType);
                }
            }
        }

        public void OnRangeInsideRangeTracker(InRangeColliderTracker InRangeColliderTracker, RangeType rangeType)
        {
            var objectRepelType = this.IsElligibleToRepelLineVisualEffect(InRangeColliderTracker, rangeType);
            if (objectRepelType != null)
            {
                if (!this.inRangeRepelableObjects.ContainsKey(rangeType))
                {
                    this.inRangeRepelableObjects[rangeType] = new List<ObjectRepelModule>();
                }
                this.inRangeRepelableObjects[rangeType].Add(objectRepelType);

                if (!this.inRangeRepelableObjectFXManagers.ContainsKey(objectRepelType))
                {
                    this.inRangeRepelableObjectFXManagers[objectRepelType] = new List<InRangeRepelableObjectFXManager>();
                }

                this.inRangeRepelableObjectFXManagers[objectRepelType].Add(new InRangeRepelableObjectFXManager(DottedLine.CreateInstance(DottedLineID.REPELABLE_OBJECT_FEEDBACK, this.puzzleGameConfigurationManager, this.dottedLineContainer)));
            }
        }

        public void OnRangeOutsideRangeTracker(InRangeColliderTracker InRangeColliderTracker, RangeType rangeType)
        {
            var objectRepelType = this.IsElligibleToRepelLineVisualEffect(InRangeColliderTracker, rangeType);
            if (objectRepelType != null)
            {
                if (this.inRangeRepelableObjects.ContainsKey(rangeType))
                {
                    if (this.inRangeRepelableObjects[rangeType].Contains(objectRepelType))
                    {
                        if (this.inRangeRepelableObjectFXManagers.ContainsKey(objectRepelType))
                        {
                            foreach (var e in this.inRangeRepelableObjectFXManagers[objectRepelType])
                            {
                                e.OnRangeExit();
                            }
                            this.inRangeRepelableObjectFXManagers.Remove(objectRepelType);
                        }
                        this.inRangeRepelableObjects[rangeType].Remove(objectRepelType);
                    }
                }
            }
        }

        #endregion

        #region Logical Conditions
        private ObjectRepelModule IsElligibleToRepelLineVisualEffect(InRangeColliderTracker InRangeColliderTracker, RangeType rangeType)
        {
            if (rangeType.RangeTypeID == RangeTypeID.LAUNCH_PROJECTILE_CURSOR)
            {
                return InRangeColliderTracker.GetTrackedRepelableObject();
            }
            return null;
        }
        #endregion
    }

    class InRangeRepelableObjectFXManager
    {
        private DottedLine repelableFeedbackLine;

        public InRangeRepelableObjectFXManager(DottedLine repelableFeedbackLine)
        {
            this.repelableFeedbackLine = repelableFeedbackLine;
        }

        public void Tick(float d, RangeType rangeType, ObjectRepelModule associatedObjectRepelType)
        {
            if (repelableFeedbackLine != null && rangeType != null)
            {
                var startObject = associatedObjectRepelType.GetModelBounds();
                var startObjectBound = startObject.GetAverageModelBoundLocalSpace();
                var maxRadius = Mathf.Max(startObjectBound.SideDistances.x, startObjectBound.SideDistances.z) * 0.5f;
                var startObjectTransform = IRenderBoundRetrievableStatic.FromIRenderBoundRetrievable(startObject).transform;
                var projectedDirection = Vector3.ProjectOnPlane((startObjectTransform.position - rangeType.GetCenterWorldPos()), startObjectTransform.up).normalized;
                var startPosition = associatedObjectRepelType.transform.position + IRenderBoundRetrievableStatic.GetRepelLineRenderPointLocalOffset(startObject) + (maxRadius * projectedDirection);

                this.repelableFeedbackLine.Tick(d, startPosition, startPosition + (5f * projectedDirection));
            }
        }

        public void OnRangeExit()
        {
            if (this.repelableFeedbackLine != null)
            {
                this.repelableFeedbackLine.DestroyInstance();
            }
        }
    }
}
