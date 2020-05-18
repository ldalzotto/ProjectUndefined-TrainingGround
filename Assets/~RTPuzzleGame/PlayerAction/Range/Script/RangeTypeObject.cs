﻿using CoreGame;
using GameConfigurationID;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public class RangeTypeObject : MonoBehaviour
    {
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

        public void Init(RangeTypeObjectInitializer RangeTypeObjectInitializer)
        {
            #region External Dependencies
            this.RangeEventsManager = GameObject.FindObjectOfType<RangeEventsManager>();
            #endregion

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
        }
        public void OnRangeTriggerExit(Collider other)
        {
            if (this.rangeObstacleListener != null)
            {
                this.rangeObstacleListener.OnRangeTriggerExit(other);
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

        public static RangeTypeObject Instanciate(RangeTypeID rangeTypeID, float sphereRadius, Func<Vector3> originPositionProvider = null, Func<Color> rangeColorProvider = null)
        {
            var rangeTypeContainer = GameObject.FindObjectOfType<RangeTypeObjectContainer>();
            var sphereRangeTypeObject = MonoBehaviour.Instantiate(PrefabContainer.Instance.BaseSphereRangePrefab, rangeTypeContainer.transform);
            sphereRangeTypeObject.PopulateModules();
            sphereRangeTypeObject.SetRangeID(rangeTypeID);
            sphereRangeTypeObject.Init(new RangeTypeObjectInitializer(sphereRadius, originPositionProvider, rangeColorProvider));
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
        public bool IsInsideAndNotOccluded(Vector3 worldPointComparison)
        {
            bool isInsideRange = this.rangeType.IsInside(worldPointComparison);
            if (this.rangeObstacleListener != null)
            {
                isInsideRange = isInsideRange && !this.IsOccluded(worldPointComparison);
            }
            return isInsideRange;
        }
        public bool IsOccluded(Vector3 worldPointComparison)
        {
            return this.rangeObstacleListener != null && this.rangeObstacleListener.IsPointOccludedByObstacles(worldPointComparison);
        }
        #endregion

    }

    public class RangeTypeObjectInitializer
    {
        public float SphereRadius;
        public Func<Vector3> OriginPositionProvider;
        public Func<Color> RangeColorProvider;

        public RangeTypeObjectInitializer(float sphereRadius = 0f, Func<Vector3> originPositionProvider = null, Func<Color> rangeColorProvider = null)
        {
            SphereRadius = sphereRadius;
            OriginPositionProvider = originPositionProvider;
            RangeColorProvider = rangeColorProvider;
        }
    }
}
