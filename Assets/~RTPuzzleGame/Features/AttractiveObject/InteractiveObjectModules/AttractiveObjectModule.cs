using GameConfigurationID;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public interface IAttractiveObjectModuleDataRetriever
    {
        ModelObjectModule GetModelObjectModule();
        Transform GetTransform();
        IAttractiveObjectModuleEvent GetIAttractiveObjectModuleEvent();
    }

    public partial class AttractiveObjectModule : InteractiveObjectModule, IAttractiveObjectModuleDataRetriever
    {

        public static IAttractiveObjectModuleDataRetriever GetAttractiveObjectFromCollisionType(CollisionType collisionType)
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
        public IAttractiveObjectModuleEvent GetIAttractiveObjectModuleEvent() { return this; }
        #endregion

        public AttractiveObjectId AttractiveObjectId;

        private HashSet<AIObjectDataRetriever> CurrentlyAttractedAI;
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
            this.CurrentlyAttractedAI = new HashSet<AIObjectDataRetriever>();
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.AttractiveObjectLifetimeTimer.Tick(d, timeAttenuationFactor);
        }

        public override void OnInteractiveObjectDestroyed()
        {
            this.sphereRange.OnRangeDestroyed();

            if (this.CurrentlyAttractedAI.Count > 0)
            {
                var attractiveObjectDestroyedAIEvent = new AttractiveObjectDestroyedAIBehaviorEvent(this);
                AIObjectDataRetriever currentlyAttractedAI = this.CurrentlyAttractedAI.First();
                while (currentlyAttractedAI != null)
                {
                    currentlyAttractedAI.GetAIBehavior().ReceiveEvent(attractiveObjectDestroyedAIEvent);
                    this.CurrentlyAttractedAI.Remove(currentlyAttractedAI);
                    currentlyAttractedAI = null;
                    if (this.CurrentlyAttractedAI.Count > 0)
                    {
                        currentlyAttractedAI = this.CurrentlyAttractedAI.First();
                    }
                }
            }
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
}
