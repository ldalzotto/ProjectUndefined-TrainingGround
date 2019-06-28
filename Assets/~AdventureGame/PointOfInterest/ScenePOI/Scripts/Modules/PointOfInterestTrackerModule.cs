using System.Collections.Generic;
using UnityEngine;
using CoreGame;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AdventureGame
{
    public class PointOfInterestTrackerModule : APointOfInterestModule
    {
        private POITrackerManager POITrackerManager;

        public override void Init(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModules pointOfInteresetModules)
        {
            base.Init(pointOfInterestTypeRef, pointOfInteresetModules);

            var trackerCollider = GetComponent<SphereCollider>();
            this.POITrackerManager = new POITrackerManager(pointOfInterestTypeRef.POIDataComponentContainer.GetDataComponent<PlayerPOITrackerManagerComponentV2>(), trackerCollider);

        }

        #region Data Retrieval
        public PointOfInterestType NearestInRangePointOfInterest() { return this.POITrackerManager.NearestInRangePointOfInterest; }
        public PointOfInterestType NearestInRangeInteractabledPointOfInterest() { return this.POITrackerManager.NearestInRangeInteractabledPointOfInterest; }
        #endregion

        public override void Tick(float d)
        {
            this.POITrackerManager.Tick(d);
        }

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsPoi)
                {
                    this.POITrackerManager.OnPOIObjectEnter(PointOfInterestType.FromCollisionType(collisionType));
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsPoi)
                {
                    this.POITrackerManager.OnPOIObjectExit(PointOfInterestType.FromCollisionType(collisionType));
                }
            }
        }

        public override void OnPOIDisabled(APointOfInterestType pointOfInterestType)
        {
            this.POITrackerManager.OnPOIObjectExit((PointOfInterestType)pointOfInterestType);
        }
    }

    class POITrackerManager
    {
        private PlayerPOITrackerManagerComponentV2 PlayerPOITrackerManagerComponent;
        private SphereCollider TrackerCollider;
        private Transform ReferenceTransform;
        private List<PointOfInterestType> InRangePointOfInterests = new List<PointOfInterestType>();
        private PointOfInterestType nearestInRangePointOfInterest;
        private PointOfInterestType nearestInRangeInteractabledPointOfInterest;

        public PointOfInterestType NearestInRangePointOfInterest { get => nearestInRangePointOfInterest; }
        public PointOfInterestType NearestInRangeInteractabledPointOfInterest { get => nearestInRangeInteractabledPointOfInterest; }

        public POITrackerManager(PlayerPOITrackerManagerComponentV2 playerPOITrackerManagerComponent, SphereCollider TrackerCollider)
        {
            PlayerPOITrackerManagerComponent = playerPOITrackerManagerComponent;
            this.TrackerCollider = TrackerCollider;
            this.TrackerCollider.radius = PlayerPOITrackerManagerComponent.SphereDetectionRadius;
            this.ReferenceTransform = this.TrackerCollider.transform;
        }

        public void Tick(float d)
        {
            TrackerCollider.radius = PlayerPOITrackerManagerComponent.SphereDetectionRadius;
            nearestInRangePointOfInterest = GetNearestPOI();
            nearestInRangeInteractabledPointOfInterest = null;
            if (nearestInRangePointOfInterest != null)
            {
                if (Vector3.Distance(ReferenceTransform.position, nearestInRangePointOfInterest.transform.position) <= nearestInRangePointOfInterest.GetMaxDistanceToInteractWithPlayer())
                {
                    nearestInRangeInteractabledPointOfInterest = nearestInRangePointOfInterest;
                }
            }
        }

        public void OnPOIObjectEnter(PointOfInterestType pointOfInterestType)
        {
            if (pointOfInterestType != null)
            {
                InRangePointOfInterests.Add(pointOfInterestType);
            }
        }

        public void OnPOIObjectExit(PointOfInterestType pointOfInterestType)
        {
            if (pointOfInterestType != null)
            {
                InRangePointOfInterests.Remove(pointOfInterestType);
            }
        }

        private PointOfInterestType GetNearestPOI()
        {
            PointOfInterestType nearestPoi = null;
            foreach (var POI in InRangePointOfInterests)
            {
                if (nearestPoi == null)
                {
                    nearestPoi = POI;
                }
                else
                {
                    if (Vector3.Distance(POI.transform.position, TrackerCollider.transform.position) <= Vector3.Distance(nearestPoi.transform.position, TrackerCollider.transform.position))
                    {
                        nearestPoi = POI;
                    }
                }
            }
            return nearestPoi;
        }

        public void POIDeleted(PointOfInterestType deletedPOI)
        {
            if (InRangePointOfInterests.Contains(deletedPOI))
            {
                InRangePointOfInterests.Remove(deletedPOI);
            }
        }

        public void OnGizmoTick()
        {
            if (PlayerPOITrackerManagerComponent != null && TrackerCollider != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(TrackerCollider.transform.position, PlayerPOITrackerManagerComponent.SphereDetectionRadius);
                var labelStyle = GUI.skin.GetStyle("Label");
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.normal.textColor = Color.blue;
#if UNITY_EDITOR
                Handles.Label(TrackerCollider.transform.position + new Vector3(0, PlayerPOITrackerManagerComponent.SphereDetectionRadius, 0), "POI Trigger Sphere Detection", labelStyle);
#endif
            }
        }
    }
}