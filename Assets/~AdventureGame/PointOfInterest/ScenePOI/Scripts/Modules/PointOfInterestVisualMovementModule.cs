using CoreGame;
using System.Collections.Generic;
using UnityEngine;
using static AnimationConstants;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AdventureGame
{
    public class PointOfInterestVisualMovementModule : APointOfInterestModule
    {
        #region Modules
        private PointOfInterestTrackerModule PointOfInterestTrackerModule;
        #endregion

        private PointOfInterestVisualMovementManager PointOfInterestVisualMovementManager;
        private IVisualMovementPermission IVisualMovementPermission;

        public void Init(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModelObjectModule PointOfInterestModelObjectModule, PointOfInterestTrackerModule PointOfInterestTrackerModule)
        {
            #region Data Component Dependencies
            var PlayerPOIVisualMovementComponentV2 = pointOfInterestTypeRef.POIDataComponentContainer.GetDataComponent<PlayerPOIVisualMovementComponentV2>();
            #endregion

            #region Other Modules Dependencies
            this.PointOfInterestTrackerModule = PointOfInterestTrackerModule;
            #endregion

            this.PointOfInterestVisualMovementManager = new PointOfInterestVisualMovementManager(PlayerPOIVisualMovementComponentV2, PointOfInterestModelObjectModule.Animator);

            if (pointOfInterestTypeRef.IsPlayer())
            {
                this.IVisualMovementPermission = GameObject.FindObjectOfType<PlayerManager>();
            }
            else
            {
                this.IVisualMovementPermission = pointOfInterestTypeRef;
            }
        }

        public void LateTick(float d)
        {
            if (this.IVisualMovementPermission.IsVisualMovementAllowed())
            {
                this.PointOfInterestVisualMovementManager.LateTick(d, this.PointOfInterestTrackerModule.NearestInRangeInteractabledPointOfInterest());
            }
            else
            {
                this.PointOfInterestVisualMovementManager.LateTickNoFollowing();
            }
        }
    }

    public interface IVisualMovementPermission
    {
        bool IsVisualMovementAllowed();
    }

    class PointOfInterestVisualMovementManager
    {
        private PlayerPOIVisualMovementComponentV2 playerPOIVisualHeadMovementComponent;

        private List<Transform> bonesThatReactToPOI;
        private Transform headBone;

        private PointOfInterestType LastNearestInteractablePOI;
        private List<Quaternion> InterpolatedBoneRotations = new List<Quaternion>();

        private bool IsLookingToPOI;
        private bool hasEndedSmoothingOut;

        //To avoid unnescessary calculation when not active
        private bool lastFrameWasLateTickNoFollowing = false;

        public PointOfInterestVisualMovementManager(PlayerPOIVisualMovementComponentV2 playerPOIVisualHeadMovementComponent, Animator animator)
        {
            this.headBone = BipedBoneRetriever.GetPlayerBone(playerPOIVisualHeadMovementComponent.MovingBone, animator).transform;
            this.bonesThatReactToPOI = new List<Transform>() { this.headBone };
            this.playerPOIVisualHeadMovementComponent = playerPOIVisualHeadMovementComponent;
            for (var i = 0; i < this.bonesThatReactToPOI.Count; i++)
            {
                InterpolatedBoneRotations.Add(Quaternion.identity);
            }
        }

        public void LateTick(float d, PointOfInterestType NearestInteractablePOI)
        {
            LastNearestInteractablePOI = NearestInteractablePOI;
            if (LastNearestInteractablePOI != null)
            {
                if (!IsLookingToPOI)
                {
                    //first time looking
                    ResetInterpolatedBoneRotationToActual();
                }

                IsLookingToPOI = true;
                hasEndedSmoothingOut = false;

                for (var i = 0; i < this.bonesThatReactToPOI.Count; i++)
                {
                    var affectedBone = this.bonesThatReactToPOI[i];

                    // (1) - Target direction is the direction between bone and POI point.
                    var targetDirection = (LastNearestInteractablePOI.transform.position - affectedBone.transform.position).normalized;

                    // (2) - We clamp the bone rotation to a cone.
                    var coneClampedRotation = QuaternionHelper.ConeReduction(this.headBone.forward, targetDirection, this.playerPOIVisualHeadMovementComponent.RotationAngleLimit);

                    // (3) - We rotate the target direction to fit the cone constraint.
                    var adjustedDirection = (coneClampedRotation * targetDirection).normalized;

                    /*
                    Debug.DrawLine(playerPOIVisualHeadMovementComponent.HeadBone.transform.position, playerPOIVisualHeadMovementComponent.HeadBone.transform.position + (playerPOIVisualHeadMovementComponent.HeadBone.forward * 10), Color.blue);
                    Debug.DrawLine(playerPOIVisualHeadMovementComponent.HeadBone.transform.position, playerPOIVisualHeadMovementComponent.HeadBone.transform.position + (targetDirection.normalized * 10), Color.green);
                    Debug.DrawLine(playerPOIVisualHeadMovementComponent.HeadBone.transform.position, playerPOIVisualHeadMovementComponent.HeadBone.transform.position + (adjustedDirection.normalized * 10), Color.red);
                    */

                    affectedBone.rotation = Quaternion.Slerp(InterpolatedBoneRotations[i], Quaternion.LookRotation(adjustedDirection, affectedBone.transform.up),
                        playerPOIVisualHeadMovementComponent.SmoothMovementSpeed * d);
                    InterpolatedBoneRotations[i] = affectedBone.rotation;
                }
            }
            else
            {
                IsLookingToPOI = false;
                SmoothNoLookingTransition(d);
            }

            this.lastFrameWasLateTickNoFollowing = false;
        }

        private void SmoothNoLookingTransition(float d)
        {
            if (!hasEndedSmoothingOut)
            {
                for (var i = 0; i < this.bonesThatReactToPOI.Count; i++)
                {
                    var affectedBone = this.bonesThatReactToPOI[i];
                    var dotProductToTarget = Mathf.Abs(Quaternion.Dot(InterpolatedBoneRotations[i], affectedBone.rotation));

                    //too much angle to smooth -> direct transition
                    if (dotProductToTarget <= playerPOIVisualHeadMovementComponent.SmoothOutMaxDotProductLimit)
                    {
                        hasEndedSmoothingOut = true;
                    }
                    else if (dotProductToTarget <= 0.9999f)
                    {
                        affectedBone.rotation = Quaternion.Slerp(InterpolatedBoneRotations[i], affectedBone.rotation, playerPOIVisualHeadMovementComponent.SmoothMovementSpeed * d);
                        InterpolatedBoneRotations[i] = affectedBone.rotation;
                    }
                    else
                    {
                        hasEndedSmoothingOut = true;
                    }
                }
            }
        }

        public void LateTickNoFollowing()
        {
            if (!this.lastFrameWasLateTickNoFollowing)
            {
                ResetInterpolatedBoneRotationToActual();
                this.lastFrameWasLateTickNoFollowing = true;
            }
        }

        private void ResetInterpolatedBoneRotationToActual()
        {
            for (var i = 0; i < this.bonesThatReactToPOI.Count; i++)
            {
                var affectedBone = this.bonesThatReactToPOI[i];
                InterpolatedBoneRotations[i] = affectedBone.rotation;
            }
        }

        public void OnContextActionAdded()
        {
            hasEndedSmoothingOut = false;
        }

        public void GizmoTick()
        {
            if (LastNearestInteractablePOI != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(LastNearestInteractablePOI.transform.position, 1f);

                for (var i = 0; i < this.bonesThatReactToPOI.Count; i++)
                {
                    Gizmos.DrawLine(this.bonesThatReactToPOI[i].position, LastNearestInteractablePOI.transform.position);
#if UNITY_EDITOR
                    Handles.Label(LastNearestInteractablePOI.transform.position, "Targeted POI");
#endif
                }
            }
        }

    }

}
