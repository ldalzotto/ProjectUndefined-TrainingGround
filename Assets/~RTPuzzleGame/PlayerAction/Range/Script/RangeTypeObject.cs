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
        public RangeTypeObjectDefinitionID RangeTypeObjectDefinitionID;

        #region Modules
        private RangeType rangeType;
        private RangeObstacleListener rangeObstacleListener;
        #endregion

        #region Data Retrieval
        public RangeType RangeType { get => rangeType; }
        public RangeObstacleListener RangeObstacleListener { get => rangeObstacleListener; }
        #endregion

        #region External Dependencies 
        private RangeEventsManager RangeEventsManager;
        #endregion

        #region Event Managements
        private List<RangeTypeObjectEventListener> eventListenersFromExterior;
        #endregion

        public void Init(RangeTypeObjectInitializer RangeTypeObjectInitializer, List<RangeTypeObjectEventListener> eventListenersFromExterior = null)
        {
            if (RangeTypeObjectDefinitionID != RangeTypeObjectDefinitionID.NONE)
            {
                GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().RangeTypeObjectDefinitionConfiguration()[this.RangeTypeObjectDefinitionID]
                        .DefineRangeTypeObject(this, GameObject.FindObjectOfType<PuzzleStaticConfigurationContainer>().PuzzleStaticConfiguration.PuzzlePrefabConfiguration);
            }

            this.CommonInit(RangeTypeObjectInitializer, eventListenersFromExterior);
        }

        public void Init(RangeTypeObjectDefinitionConfigurationInherentData rangeTypeObjectDefinitionConfigurationInherentData,
            RangeTypeObjectInitializer RangeTypeObjectInitializer, List<RangeTypeObjectEventListener> eventListenersFromExterior = null)
        {
            rangeTypeObjectDefinitionConfigurationInherentData.DefineRangeTypeObject(this, GameObject.FindObjectOfType<PuzzleStaticConfigurationContainer>().PuzzleStaticConfiguration.PuzzlePrefabConfiguration);
            this.CommonInit(RangeTypeObjectInitializer, eventListenersFromExterior);
        }

        public void PopuplateSphereRangeData(float sphereRange, RangeTypeID rangeTypeID, RangeTypeObjectInitializer RangeTypeObjectInitializer, List<RangeTypeObjectEventListener> eventListenersFromExterior = null)
        {
            this.PopulateModules();
            this.SetRangeID(rangeTypeID);
            ((SphereRangeType)this.rangeType).PopupulateFromData(sphereRange);
            this.CommonInit(RangeTypeObjectInitializer, eventListenersFromExterior);
        }

        private void CommonInit(RangeTypeObjectInitializer RangeTypeObjectInitializer, List<RangeTypeObjectEventListener> eventListenersFromExterior)
        {
            #region External Dependencies
            this.RangeEventsManager = GameObject.FindObjectOfType<RangeEventsManager>();
            #endregion

            this.eventListenersFromExterior = eventListenersFromExterior;
            this.PopulateModules();

            this.rangeType.IfNotNull((RangeType rangeType) => rangeType.Init(RangeTypeObjectInitializer, this));
            this.rangeObstacleListener.IfNotNull((RangeObstacleListener rangeObstacleListener) => rangeObstacleListener.Init(this.rangeType));

            this.RangeEventsManager.RANGE_EVT_Range_Created(this);
        }

        private void PopulateModules()
        {
            #region Modules
            this.rangeType = GetComponentInChildren<RangeType>();
            this.rangeObstacleListener = GetComponentInChildren<RangeObstacleListener>();
            #endregion
        }

        public void Tick(float d)
        {
            this.rangeType.Tick(d);
        }

        #region Internal Events
        public void OnRangeTriggerEnter(Collider other)
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
        public void OnRangeTriggerExit(Collider other)
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
            this.rangeObstacleListener.IfNotNull((RangeObstacleListener rangeObstacleListener) => rangeObstacleListener.OnRangeObstacleListenerDestroyed(this.rangeType));

            this.RangeEventsManager.RANGE_EVT_Range_Destroy(this);
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
            var rangeTypeContainer = GameObject.FindObjectOfType<RangeTypeObjectContainer>();
            var sphereRangeTypeObject = MonoBehaviour.Instantiate(PrefabContainer.Instance.BaseRangeTypeObject, rangeTypeContainer.transform);
            sphereRangeTypeObject.Init(RangeTypeObjectDefinitionConfigurationInherentDataBuilder.SphereRangeWithObstacleListener(sphereRange, rangeTypeID, GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().PuzzleGameConfiguration.RangeTypeObjectDefinitionConfiguration),
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
        public bool IsInsideAndNotOccluded(BoxCollider boxCollider)
        {
            Profiler.BeginSample("ObstacleIsInsideAndNotOccluded");
            bool isInsideRange = this.rangeType.IsInside(boxCollider);
            if (this.rangeObstacleListener != null && isInsideRange)
            {
                isInsideRange = isInsideRange && !this.IsOccluded(boxCollider);
            }
            Profiler.EndSample();
            return isInsideRange;
        }

        public bool IsInsideAndNotOccluded(Vector3 worldPointComparison)
        {
            bool isInsideRange = this.rangeType.IsInside(worldPointComparison);
            if (this.rangeObstacleListener != null && isInsideRange)
            {
                isInsideRange = isInsideRange && !this.IsOccluded(worldPointComparison);
            }
            return isInsideRange;
        }

        public bool IsOccluded(BoxCollider boxCollider)
        {
            return this.rangeObstacleListener != null && this.rangeObstacleListener.IsBoxOccludedByObstacles(boxCollider);
        }
        public bool IsOccluded(Vector3 worldPointComparison)
        {
            return this.rangeObstacleListener != null && this.rangeObstacleListener.IsPointOccludedByObstacles(worldPointComparison);
        }
        #endregion

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
        void OnRangeTriggerEnter(Collider other);
        void OnRangeTriggerExit(Collider other);
    }
}
