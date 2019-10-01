using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace RTPuzzle
{
    public class RangeTypeObject : MonoBehaviour
    {
        [CustomEnum(ConfigurationType = typeof(RangeTypeObjectDefinitionConfiguration))]
        public RangeTypeObjectDefinitionID RangeTypeObjectDefinitionID;

        #region Modules
        private RangeType rangeType;
        private RangeObstacleListenerModule rangeObstacleListener;
        #endregion

        #region Data Retrieval
        public RangeType RangeType { get => rangeType; }
        public RangeObstacleListenerModule RangeObstacleListener { get => rangeObstacleListener; }
        #endregion

        #region External Dependencies 
        private IRangeTypeObjectEventListener IRangeTypeObjectEventListener;
        #endregion

        #region Event Managements
        private List<RangeTypeObjectEventListener> eventListenersFromExterior;
        #endregion

        public void Init(RangeTypeObjectInitializer RangeTypeObjectInitializer, List<RangeTypeObjectEventListener> eventListenersFromExterior = null)
        {
            if (RangeTypeObjectDefinitionID != RangeTypeObjectDefinitionID.NONE)
            {
                PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.RangeTypeObjectDefinitionConfiguration()[this.RangeTypeObjectDefinitionID]
                        .DefineRangeTypeObject(this, PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration);
            }

            this.CommonInit(RangeTypeObjectInitializer, eventListenersFromExterior);
        }

        public void Init(RangeTypeObjectDefinitionInherentData rangeTypeObjectDefinitionConfigurationInherentData,
            RangeTypeObjectInitializer RangeTypeObjectInitializer, List<RangeTypeObjectEventListener> eventListenersFromExterior = null)
        {
            rangeTypeObjectDefinitionConfigurationInherentData.DefineRangeTypeObject(this, PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration);
            this.CommonInit(RangeTypeObjectInitializer, eventListenersFromExterior);
        }

        public void SetIsAttractiveObject()
        {
            this.rangeType.IfNotNull((RangeType rangeType) => rangeType.GetCollisionType().IsRTAttractiveObject = true);
        }

        private void CommonInit(RangeTypeObjectInitializer RangeTypeObjectInitializer, List<RangeTypeObjectEventListener> eventListenersFromExterior)
        {
            #region External Dependencies
            this.IRangeTypeObjectEventListener = PuzzleGameSingletonInstances.RangeEventsManager;
            #endregion

            this.eventListenersFromExterior = eventListenersFromExterior;
            this.PopulateModules();

            this.rangeType.IfNotNull((RangeType rangeType) => rangeType.Init(RangeTypeObjectInitializer, this));
            this.rangeObstacleListener.IfNotNull((RangeObstacleListenerModule rangeObstacleListener) => rangeObstacleListener.Init(this.rangeType));

            this.IRangeTypeObjectEventListener.RANGE_EVT_Range_Created(this);
        }

        private void PopulateModules()
        {
            #region Modules
            this.rangeType = GetComponentInChildren<RangeType>();
            this.rangeObstacleListener = GetComponentInChildren<RangeObstacleListenerModule>();
            #endregion
        }

        public void Tick(float d)
        {
            this.rangeType.Tick(d);
        }

        #region Internal Events
        public void OnRangeTriggerEnter(CollisionType other)
        {
            if (this.rangeObstacleListener != null)
            {
                this.rangeObstacleListener.OnRangeTriggerEnter(other);
            }
            if (this.eventListenersFromExterior != null)
            {
                foreach (var listener in this.eventListenersFromExterior)
                {
                    listener.OnRangeTriggerEnter(other);
                }
            }
        }

        public void OnRangeTriggerExit(CollisionType other)
        {
            if (this.rangeObstacleListener != null)
            {
                this.rangeObstacleListener.OnRangeTriggerExit(other);
            }
            if (this.eventListenersFromExterior != null)
            {
                foreach (var listener in this.eventListenersFromExterior)
                {
                    listener.OnRangeTriggerExit(other);
                }
            }
        }
        #endregion

        #region External Events
        public void OnRangeDestroyed()
        {
            this.rangeObstacleListener.IfNotNull((RangeObstacleListenerModule rangeObstacleListener) => rangeObstacleListener.OnRangeObstacleListenerDestroyed(this.rangeType));

            this.IRangeTypeObjectEventListener.RANGE_EVT_Range_Destroy(this);
            MonoBehaviour.Destroy(this.gameObject);
        }
        #endregion

        #region Data Setter
        public void SetRangeID(RangeTypeID rangeTypeID)
        {
            this.rangeType.RangeTypeID = rangeTypeID;
        }
        #endregion

        public static RangeTypeObject InstanciateSphereRange(RangeTypeID rangeTypeID, float sphereRange,
                                 Func<Vector3> originPositionProvider = null, Func<Color> rangeColorProvider = null)
        {
            var rangeTypeContainer = PuzzleGameSingletonInstances.RangeTypeObjectContainer;
            var sphereRangeTypeObject = MonoBehaviour.Instantiate(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration.BaseRangeTypeObject, rangeTypeContainer.transform);
            sphereRangeTypeObject.Init(RangeTypeObjectDefinitionConfigurationInherentDataBuilder.SphereRangeWithObstacleListener(sphereRange, rangeTypeID),
                new RangeTypeObjectInitializer(originPositionProvider, rangeColorProvider));
            return sphereRangeTypeObject;
        }

        #region Logical Conditions
        public bool IsOccludedByFrustum()
        {
            return this.rangeObstacleListener != null && this.rangeObstacleListener.ObstacleListener.IsListenerHaveObstaclesNearby();
        }
        public bool IsRangeConfigurationDefined()
        {
            return this.rangeType.IsRangeConfigurationDefined();
        }
        public bool IsInsideAndNotOccluded(BoxCollider boxCollider, bool forceObstacleOcclusionIfNecessary)
        {
            Profiler.BeginSample("ObstacleIsInsideAndNotOccluded");
            bool isInsideRange = this.rangeType.IsInside(boxCollider);
            if (this.rangeObstacleListener != null && isInsideRange)
            {
                isInsideRange = isInsideRange && !this.IsOccluded(boxCollider, forceObstacleOcclusionIfNecessary);
            }
            Profiler.EndSample();
            return isInsideRange;
        }

        public bool IsInsideAndNotOccluded(Vector3 worldPointComparison, bool forceObstacleOcclusionIfNecessary)
        {
            bool isInsideRange = this.rangeType.IsInside(worldPointComparison);
            if (this.rangeObstacleListener != null && isInsideRange)
            {
                isInsideRange = isInsideRange && !this.IsOccluded(worldPointComparison, forceObstacleOcclusionIfNecessary);
            }
            return isInsideRange;
        }
        #endregion

        private bool IsOccluded(BoxCollider boxCollider, bool forceObstacleOcclusionIfNecessary)
        {
            return this.rangeObstacleListener != null && this.rangeObstacleListener.IsBoxOccludedByObstacles(boxCollider, forceObstacleOcclusionIfNecessary);
        }
        private bool IsOccluded(Vector3 worldPointComparison, bool forceObstacleOcclusionIfNecessary)
        {
            return this.rangeObstacleListener != null && this.rangeObstacleListener.IsPointOccludedByObstacles(worldPointComparison, forceObstacleOcclusionIfNecessary);
        }

    }

    public class RangeTypeObjectInitializer
    {
        public Func<Vector3> OriginPositionProvider;
        public Func<Color> RangeColorProvider;

        public RangeTypeObjectInitializer(Func<Vector3> originPositionProvider = null, Func<Color> rangeColorProvider = null)
        {
            OriginPositionProvider = originPositionProvider;
            RangeColorProvider = rangeColorProvider;
        }
    }

    public interface RangeTypeObjectEventListener
    {
        void OnRangeTriggerEnter(CollisionType other);
        void OnRangeTriggerStay(CollisionType other);
        void OnRangeTriggerExit(CollisionType other);
    }
}
