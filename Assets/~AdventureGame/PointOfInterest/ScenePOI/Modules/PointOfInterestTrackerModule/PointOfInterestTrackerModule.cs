using CoreGame;
using System.Collections.Generic;
using UnityEngine;

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
        public PointOfInterestType NearestInRangeInteractabledPointOfInterest()
        {
            var poiInteractableSorted = this.GetAllPointOfInterestsInRangeAndInteractable();
            if (poiInteractableSorted.Count > 0) { return poiInteractableSorted[0]; }
            return null;
        }

        public List<PointOfInterestType> GetAllPointOfInterestsInRangeAndInteractable()
        {
            return this.POITrackerManager.GetAllPOIInRangeAndInteractableOrderedByDistance();
        }
        #endregion

        public void Tick(float d)
        { }

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null)
            {
                if (collisionType.IsPoi)
                {
                    var otherPointOfInterestType = PointOfInterestTypeHelper.FromCollisionType(collisionType);
                    if (otherPointOfInterestType != null && otherPointOfInterestType != this.PointOfInterestTypeRef)
                    {
                        this.POITrackerManager.OnPOIObjectEnter(otherPointOfInterestType);
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
                    if (otherPointOfInterestType != null && otherPointOfInterestType != this.PointOfInterestTypeRef)
                    {
                        this.POITrackerManager.OnPOIObjectExit(otherPointOfInterestType);
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

        public POITrackerManager(PointOfInterestType PointOfInterestTypeRef, SphereCollider TrackerCollider)
        {
            this.PointOfInterestTypeRef = PointOfInterestTypeRef;
            this.TrackerCollider = TrackerCollider;
            this.ReferenceTransform = this.TrackerCollider.transform;
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

        public List<PointOfInterestType> GetAllPOIInRangeAndInteractableOrderedByDistance()
        {
            List<PointOfInterestType> foundPOI = new List<PointOfInterestType>();
            this.InRangePointOfInterests.Sort((p1, p2) =>
            {
                return Vector3.Distance(p1.transform.position, TrackerCollider.transform.position).CompareTo(Vector3.Distance(p2.transform.position, TrackerCollider.transform.position));
            });
            foreach (var POI in InRangePointOfInterests)
            {
                if (Vector3.Angle(this.PointOfInterestTypeRef.transform.forward, POI.transform.position - this.PointOfInterestTypeRef.transform.position) <= this.PointOfInterestTypeRef.PointOfInterestDefinitionInherentData.PointOfInterestSharedDataTypeInherentData.POIDetectionAngleLimit)
                {
                    foundPOI.Add(POI);
                }
            }
            return foundPOI;
        }

        public void POIDeleted(PointOfInterestType deletedPOI)
        {
            if (InRangePointOfInterests.Contains(deletedPOI))
            {
                InRangePointOfInterests.Remove(deletedPOI);
            }
        }

    }
}