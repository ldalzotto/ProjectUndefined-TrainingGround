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

        public void Init(PointOfInterestType pointOfInterestTypeRef)
        {
            var trackerCollider = GetComponent<SphereCollider>();
            this.POITrackerManager = new POITrackerManager(pointOfInterestTypeRef, pointOfInterestTypeRef.POIDataComponentContainer.GetDataComponent<PlayerPOITrackerManagerComponentV2>(), trackerCollider);

        }

        #region Data Retrieval
        public PointOfInterestType NearestInRangeInteractabledPointOfInterest() { return this.POITrackerManager.NearestInRangeInteractabledPointOfInterest; }
        #endregion

        public void Tick(float d)
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
                    this.POITrackerManager.OnPOIObjectEnter(PointOfInterestTypeHelper.FromCollisionType(collisionType));
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
                    this.POITrackerManager.OnPOIObjectExit(PointOfInterestTypeHelper.FromCollisionType(collisionType));
                }
            }
        }

        public void OnPOIDisabled(APointOfInterestType pointOfInterestType)
        {
            this.POITrackerManager.OnPOIObjectExit((PointOfInterestType)pointOfInterestType);
        }
    }

    class POITrackerManager
    {
        private PointOfInterestType PointOfInterestTypeRef;
        private PlayerPOITrackerManagerComponentV2 PlayerPOITrackerManagerComponent;
        private SphereCollider TrackerCollider;
        private Transform ReferenceTransform;
        private List<PointOfInterestType> InRangePointOfInterests = new List<PointOfInterestType>();
        private PointOfInterestType nearestInRangeInteractabledPointOfInterest;
        
        public PointOfInterestType NearestInRangeInteractabledPointOfInterest { get => nearestInRangeInteractabledPointOfInterest; }

        public POITrackerManager(PointOfInterestType PointOfInterestTypeRef, PlayerPOITrackerManagerComponentV2 playerPOITrackerManagerComponent, SphereCollider TrackerCollider)
        {
            this.PointOfInterestTypeRef = PointOfInterestTypeRef;
            PlayerPOITrackerManagerComponent = playerPOITrackerManagerComponent;
            this.TrackerCollider = TrackerCollider;
            this.TrackerCollider.radius = PlayerPOITrackerManagerComponent.SphereDetectionRadius;
            this.ReferenceTransform = this.TrackerCollider.transform;
        }

        public void Tick(float d)
        {
            TrackerCollider.radius = PlayerPOITrackerManagerComponent.SphereDetectionRadius;
            nearestInRangeInteractabledPointOfInterest = GetNearestPOIInteractable();
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

        private PointOfInterestType GetNearestPOIInteractable()
        {
            this.InRangePointOfInterests.Sort((p1, p2) =>
            {
                return Vector3.Distance(p1.transform.position, TrackerCollider.transform.position).CompareTo(Vector3.Distance(p2.transform.position, TrackerCollider.transform.position));
            });

            //the first is the nearest
            PointOfInterestType nearestInteractivePoi = null;
            foreach (var POI in InRangePointOfInterests)
            {
                if(Vector3.Angle(this.PointOfInterestTypeRef.transform.forward, POI.transform.position - this.PointOfInterestTypeRef.transform.position) <= this.PointOfInterestTypeRef.PointOfInterestInherentData.POIDetectionAngleLimit)
                {
                    nearestInteractivePoi = POI;
                    break;
                }
            }
            
            return nearestInteractivePoi;
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