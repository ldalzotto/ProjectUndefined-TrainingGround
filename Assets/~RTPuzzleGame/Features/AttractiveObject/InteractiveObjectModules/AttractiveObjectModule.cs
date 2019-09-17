using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public interface IAttractiveObjectModuleDataRetriever
    {
        ModelObjectModule GetModelObjectModule();
        Transform GetTransform();
    }

    public class AttractiveObjectModule : InteractiveObjectModule, IAttractiveObjectModuleDataRetriever
    {

        public static AttractiveObjectModule GetAttractiveObjectFromCollisionType(CollisionType collisionType)
        {
            var sphereRange = RangeType.RetrieveFromCollisionType(collisionType);
            if (sphereRange != null)
            {
                return sphereRange.GetComponentInParent<AttractiveObjectModule>();
            }
            return null;
        }

        #region ModuleDependencies
        private ModelObjectModule modelObjectModule;
        #endregion

        #region Internal Dependencies
        private RangeTypeObject sphereRange;
        #endregion

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        #region Data Retrieval
        public RangeTypeObject SphereRange { get => sphereRange; }
        #endregion

        #region IAttractiveObjectModuleDataRetriever
        public ModelObjectModule GetModelObjectModule()
        {
            return this.modelObjectModule;
        }
        public Transform GetTransform() { return this.transform; }
        #endregion

        public AttractiveObjectId AttractiveObjectId;
        private AttractiveObjectLifetimeTimer AttractiveObjectLifetimeTimer;

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            AttractiveObjectInherentConfigurationData AttractiveObjectInherentConfigurationData = interactiveObjectInitializationObject.AttractiveObjectInherentConfigurationData;

            if (interactiveObjectInitializationObject.AttractiveObjectInherentConfigurationData == null)
            {
                AttractiveObjectInherentConfigurationData = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.AttractiveObjectsConfiguration()[this.AttractiveObjectId];
            }

            this.modelObjectModule = IInteractiveObjectTypeDataRetrieval.GetModelObjectModule();
            this.sphereRange = GetComponentInChildren<RangeTypeObject>();
            this.sphereRange.Init(new RangeTypeObjectInitializer(), null);
            this.sphereRange.SetIsAttractiveObject();
            this.AttractiveObjectLifetimeTimer = new AttractiveObjectLifetimeTimer(AttractiveObjectInherentConfigurationData.EffectiveTime);
            this.PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectLifetimeTimer.Tick(d, timeAttenuationFactor);
        }

        public override void OnInteractiveObjectDestroyed()
        {
            this.sphereRange.OnRangeDestroyed();
            this.PuzzleEventsManager.PZ_EVT_AttractiveObject_TpeDestroyed(this);
        }

        #region Logical Conditions
        public bool IsAskingToBeDestroyed()
        {
            return this.AttractiveObjectLifetimeTimer.IsTimeOver();
        }
        #endregion
    }

    class AttractiveObjectLifetimeTimer
    {
        private float effectiveTime;

        public AttractiveObjectLifetimeTimer(float effectiveTime)
        {
            this.effectiveTime = effectiveTime;
        }

        private float elapsedTime;

        #region Logical Condition
        public bool IsTimeOver()
        {
            return elapsedTime >= effectiveTime;
        }
        #endregion

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.elapsedTime += (d * timeAttenuationFactor);
        }

    }

    public static class AttractiveObjectTypeModuleEventHandling
    {
        public static void OnAttractiveObjectActionExecuted(RaycastHit attractiveObjectWorldPositionHit, InteractiveObjectType attractiveObject,
                    PuzzleGameConfigurationManager puzzleGameConfigurationManager)
        {
            attractiveObject.transform.position = attractiveObjectWorldPositionHit.point;

            //TODO make the rotation relative to the player
            attractiveObject.transform.LookAt(attractiveObject.transform.position + Vector3.forward);
        }
    }
}
