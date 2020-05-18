﻿using CoreGame;
using System.Collections.Generic;
using UnityEngine;
using static AnimationConstants;
using GameConfigurationID;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AdventureGame
{
    public class PointOfInterestVisualMovementModule : APointOfInterestModule
    {
        private PointOfInterestVisualMovementID PointOfInterestVisualMovementID;

        private PointOfInterestType pointOfInterestTypeRef;
        private PointOfInterestTrackerModule PointOfInterestTrackerModule;
        #region player POISelection
        private PlayerPointOfInterestSelectionManager PlayerPointOfInterestSelectionManager;
        #endregion

        private PointOfInterestVisualMovementManager PointOfInterestVisualMovementManager;
        private IVisualMovementPermission IVisualMovementPermission;

        public void Init(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModelObjectModule PointOfInterestModelObjectModule,
            PointOfInterestTrackerModule PointOfInterestTrackerModule,
            PlayerPointOfInterestSelectionManager PlayerPointOfInterestSelectionManager)
        {
            var PointOfInterestVisualMovementInherentData = GameObject.FindObjectOfType<AdventureGameConfigurationManager>().PointOfInterestVisualMovementConfiguration()[this.PointOfInterestVisualMovementID];

            this.pointOfInterestTypeRef = pointOfInterestTypeRef;
            this.PointOfInterestTrackerModule = PointOfInterestTrackerModule;
            this.PlayerPointOfInterestSelectionManager = PlayerPointOfInterestSelectionManager;

            this.PointOfInterestVisualMovementManager = new PointOfInterestVisualMovementManager(PointOfInterestVisualMovementInherentData, PointOfInterestModelObjectModule.Animator);

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
                if (this.pointOfInterestTypeRef.IsPlayer())
                {
                    this.PointOfInterestVisualMovementManager.LateTick(d, this.PlayerPointOfInterestSelectionManager.GetCurrentSelectedPOI());
                }
                else
                {
                    this.PointOfInterestVisualMovementManager.LateTick(d, this.PointOfInterestTrackerModule.NearestInRangeInteractabledPointOfInterest());
                }
            }
            else
            {
                this.PointOfInterestVisualMovementManager.LateTickNoFollowing();
            }
        }

        public static class PointOfInterestVisualMovementModuleInstancer
        {
            public static void PopuplateFromDefinition(PointOfInterestVisualMovementModule pointOfInterestVisualMovementModule, PointOfInterestVisualMovementModuleDefinition pointOfInterestVisualMovementModuleDefinition)
            {
                pointOfInterestVisualMovementModule.PointOfInterestVisualMovementID = pointOfInterestVisualMovementModuleDefinition.PointOfInterestVisualMovementID;
            }
        }

        private void OnDrawGizmos()
        {
            if (this.PointOfInterestVisualMovementManager != null)
            {
                this.PointOfInterestVisualMovementManager.GizmoTick();
            }
        }
    }

    public interface IVisualMovementPermission
    {
        bool IsVisualMovementAllowed();
    }

    class PointOfInterestVisualMovementManager
    {
        private PointOfInterestVisualMovementInherentData PointOfInterestVisualMovementInherentData;

        private List<Transform> bonesThatReactToPOI;
        private Transform headBone;

        private PointOfInterestType LastNearestNearestInteractablePOI;
        private List<Quaternion> InterpolatedBoneRotations = new List<Quaternion>();

        private bool IsLookingToPOI;
        private bool hasEndedSmoothingOut;

        //To avoid unnescessary calculation when not active
        private bool lastFrameWasLateTickNoFollowing = false;

        public PointOfInterestVisualMovementManager(PointOfInterestVisualMovementInherentData PointOfInterestVisualMovementInherentData, Animator animator)
        {
            this.headBone = BipedBoneRetriever.GetPlayerBone(PointOfInterestVisualMovementInherentData.MovingBone, animator).transform;
            this.bonesThatReactToPOI = new List<Transform>() { this.headBone };
            this.PointOfInterestVisualMovementInherentData = PointOfInterestVisualMovementInherentData;
            for (var i = 0; i < this.bonesThatReactToPOI.Count; i++)
            {
                InterpolatedBoneRotations.Add(Quaternion.identity);
            }
        }

        public void LateTick(float d, PointOfInterestType NearestInteractablePOI)
        {
            this.LastNearestNearestInteractablePOI = NearestInteractablePOI;

            if (this.LastNearestNearestInteractablePOI != null)
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
                    var targetDirection = (this.LastNearestNearestInteractablePOI.GetLogicColliderWorldPosition() - affectedBone.transform.position).normalized;

                    // (2) - We clamp the bone rotation to a cone.
                    var coneClampedRotation = QuaternionHelper.ConeReduction(this.headBone.forward, targetDirection, this.PointOfInterestVisualMovementInherentData.RotationAngleLimit);

                    // (3) - We rotate the target direction to fit the cone constraint.
                    var adjustedDirection = (coneClampedRotation * targetDirection).normalized;

                    /*
                    Debug.DrawLine(playerPOIVisualHeadMovementComponent.HeadBone.transform.position, playerPOIVisualHeadMovementComponent.HeadBone.transform.position + (playerPOIVisualHeadMovementComponent.HeadBone.forward * 10), Color.blue);
                    Debug.DrawLine(playerPOIVisualHeadMovementComponent.HeadBone.transform.position, playerPOIVisualHeadMovementComponent.HeadBone.transform.position + (targetDirection.normalized * 10), Color.green);
                    Debug.DrawLine(playerPOIVisualHeadMovementComponent.HeadBone.transform.position, playerPOIVisualHeadMovementComponent.HeadBone.transform.position + (adjustedDirection.normalized * 10), Color.red);
                    */

                    affectedBone.rotation = Quaternion.Slerp(InterpolatedBoneRotations[i], Quaternion.LookRotation(adjustedDirection, affectedBone.transform.up),
                        PointOfInterestVisualMovementInherentData.SmoothMovementSpeed * d);
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
                    if (dotProductToTarget <= PointOfInterestVisualMovementInherentData.SmoothOutMaxDotProductLimit)
                    {
                        hasEndedSmoothingOut = true;
                    }
                    else if (dotProductToTarget <= 0.9999f)
                    {
                        affectedBone.rotation = Quaternion.Slerp(InterpolatedBoneRotations[i], affectedBone.rotation, PointOfInterestVisualMovementInherentData.SmoothMovementSpeed * d);
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
            if (this.LastNearestNearestInteractablePOI != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(this.LastNearestNearestInteractablePOI.GetLogicColliderWorldPosition(), 1f);

                for (var i = 0; i < this.bonesThatReactToPOI.Count; i++)
                {
                    Gizmos.DrawLine(this.bonesThatReactToPOI[i].position, this.LastNearestNearestInteractablePOI.GetLogicColliderWorldPosition());
#if UNITY_EDITOR
                    Handles.Label(this.LastNearestNearestInteractablePOI.GetLogicColliderWorldPosition(), "Targeted POI");
#endif
                }
            }
        }

    }

}
