using CoreGame;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace AdventureGame
{
    public class PointOfInterestTrackerModule : APointOfInterestModule
    {
        #region External Dependencies
        private PlayerPointOfInterestSelectionManager PlayerPointOfInterestSelectionManager;
        #endregion

        private PointOfInterestType PointOfInterestTypeRef;
        private POITrackerManager POITrackerManager;
        public void Init(PointOfInterestType pointOfInterestTypeRef)
        {
            this.PlayerPointOfInterestSelectionManager = GameObject.FindObjectOfType<PlayerPointOfInterestSelectionManager>();

            this.PointOfInterestTypeRef = pointOfInterestTypeRef;
            var trackerCollider = GetComponent<SphereCollider>();
            this.POITrackerManager = new POITrackerManager(pointOfInterestTypeRef, trackerCollider);
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
                    var otherPointOfInterestType = PointOfInterestTypeHelper.FromCollisionType(collisionType);
                    this.POITrackerManager.OnPOIObjectEnter(otherPointOfInterestType);
                    if (this.PointOfInterestTypeRef.IsPlayer())
                    {
                        this.PlayerPointOfInterestSelectionManager.OnPointOfInterestInRange(otherPointOfInterestType);
                    }
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
                    var otherPointOfInterestType = PointOfInterestTypeHelper.FromCollisionType(collisionType);
                    this.POITrackerManager.OnPOIObjectExit(otherPointOfInterestType);
                    if (this.PointOfInterestTypeRef.IsPlayer())
                    {
                        this.PlayerPointOfInterestSelectionManager.OnPointOfInterestExitRange(otherPointOfInterestType);
                    }
                }
            }
        }

        public void OnPOIDisabled(APointOfInterestType pointOfInterestType)
        {
            this.POITrackerManager.OnPOIObjectExit((PointOfInterestType)pointOfInterestType);
        }

        public static class PointOfInterestTrackerModuleInstancer
        {
            public static void PopuplateFromDefinition(PointOfInterestTrackerModule pointOfInterestTrackerModule, PointOfInterestTrackerModuleDefinition pointOfInterestTrackerModuleDefinition)
            {
                var sphereCollider = pointOfInterestTrackerModule.GetComponent<SphereCollider>();
                sphereCollider.radius = pointOfInterestTrackerModuleDefinition.SphereDetectionRadius;
            }
        }
    }

    class POITrackerManager
    {
        private PointOfInterestType PointOfInterestTypeRef;
        private SphereCollider TrackerCollider;
        private Transform ReferenceTransform;
        private List<PointOfInterestType> InRangePointOfInterests = new List<PointOfInterestType>();
        private PointOfInterestType nearestInRangeInteractabledPointOfInterest;

        public PointOfInterestType NearestInRangeInteractabledPointOfInterest { get => nearestInRangeInteractabledPointOfInterest; }

        public POITrackerManager(PointOfInterestType PointOfInterestTypeRef, SphereCollider TrackerCollider)
        {
            this.PointOfInterestTypeRef = PointOfInterestTypeRef;
            this.TrackerCollider = TrackerCollider;
            this.ReferenceTransform = this.TrackerCollider.transform;
        }

        public void Tick(float d)
        {
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
                if (Vector3.Angle(this.PointOfInterestTypeRef.transform.forward, POI.transform.position - this.PointOfInterestTypeRef.transform.position) <= this.PointOfInterestTypeRef.PointOfInterestInherentData.POIDetectionAngleLimit)
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

        /*
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
        */
    }
}