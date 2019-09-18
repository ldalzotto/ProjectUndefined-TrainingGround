using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{

    public class DisarmObjectModule : InteractiveObjectModule, IDisarmObjectModuleDataRetrieval, IDisarmObjectModuleEvent
    {
        [CustomEnum()]
        public DisarmObjectID DisarmObjectID;

        #region Module Dependencies
        private ModelObjectModule ModelObjectModule;
        private IInteractiveObjectTypeEvents associatedIInteractiveObjectTypeEvents;
        #endregion

        #region Internal Dependencies
        private CircleFillBarType progressbar;
        #endregion

        #region State
        private HashSet<AIObjectDataRetriever> AIDisarmingTheObject;
        #endregion

        #region IDisarmObjectModuleDataRetrieval
        public Transform GetTransform() { return this.transform; }
        public PuzzleCutsceneGraph GetDisarmAnimation()
        {
            var disarmCutsceneID = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.DisarmObjectsConfiguration()[this.DisarmObjectID].DisarmObjectAnimationGraph;
            return PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.PuzzleCutsceneConfiguration()[disarmCutsceneID].PuzzleCutsceneGraph;
        }
        public Dictionary<CutsceneParametersName, object> GetDisarmAnimationInputParameters(IInteractiveObjectTypeDataRetrieval animatedInteractiveObject)
        {
            return PuzzleCutsceneActionInput.Build_1_Town_StartTutorial_Speaker_DisarmAnimation(animatedInteractiveObject);
        }

        public IDisarmObjectModuleEvent GetIDisarmObjectModuleEvent() { return this; }
        #endregion

        private DisarmObjectInherentData disarmObjectInherentConfigurationData;
        private SphereCollider disarmObjectRange;

        private float elapsedTime;

        public DisarmObjectInherentData DisarmObjectInherentConfigurationData { get => disarmObjectInherentConfigurationData; }

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.disarmObjectInherentConfigurationData = interactiveObjectInitializationObject.DisarmObjectInherentData;
            if (this.disarmObjectInherentConfigurationData == null)
            {
                this.disarmObjectInherentConfigurationData = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.DisarmObjectsConfiguration()[this.DisarmObjectID];
            }

            this.ModelObjectModule = IInteractiveObjectTypeDataRetrieval.GetModelObjectModule();

            this.disarmObjectRange = this.GetComponent<SphereCollider>();
            this.disarmObjectRange.radius = this.disarmObjectInherentConfigurationData.DisarmInteractionRange;

            this.progressbar = this.GetComponentInChildren<CircleFillBarType>();
            this.progressbar.Init(Camera.main);
            this.progressbar.transform.position = this.GetProgressBarDisplayPosition();
            this.progressbar.gameObject.SetActive(false);

            this.elapsedTime = 0f;
            this.associatedIInteractiveObjectTypeEvents = IInteractiveObjectTypeEvents;
            this.AIDisarmingTheObject = new HashSet<AIObjectDataRetriever>();
        }

        #region Logical Conditions
        public bool IsAskingToBeDestroyed()
        {
            return (this.elapsedTime >= this.disarmObjectInherentConfigurationData.DisarmTime);
        }
        #endregion

        #region Data Retrieval
        public float GetDisarmPercentage01()
        {
            return this.elapsedTime / this.disarmObjectInherentConfigurationData.DisarmTime;
        }
        private Vector3 GetProgressBarDisplayPosition()
        {
            return this.transform.position + IRenderBoundRetrievableStatic.GetDisarmProgressBarLocalOffset(this.ModelObjectModule);
        }
        public float GetEffectRange()
        {
            return this.disarmObjectRange.radius;
        }
        #endregion

        public void TickAlways(float d)
        {
            if (this.progressbar.gameObject.activeSelf)
            {
                this.progressbar.Tick(this.GetDisarmPercentage01());
            }
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            if (this.AIDisarmingTheObject.Count > 0)
            {
                for (var i = 0; i < this.AIDisarmingTheObject.Count; i++)
                {
                    this.IncreaseTimeElapsedBy(d * timeAttenuationFactor);
                }
            }
        }

        public override void OnInteractiveObjectDestroyed()
        {
            var disarmingTriggerExitAIEvent = new DisarmingObjectExitAIbehaviorEvent(this);
            var firstElement = this.AIDisarmingTheObject.First();
            while (firstElement != null)
            {
                firstElement.GetAIBehavior().ReceiveEvent(disarmingTriggerExitAIEvent);
                firstElement = null;
                if (this.AIDisarmingTheObject.Count > 0)
                {
                    firstElement = this.AIDisarmingTheObject.First();
                }
            }
            this.AIDisarmingTheObject.Clear();
        }

        #region External Event
        public void OnDisarmObjectStart(AIObjectDataRetriever InvolvedAI)
        {
            this.AIDisarmingTheObject.Add(InvolvedAI);
            this.associatedIInteractiveObjectTypeEvents.DisableModule(typeof(GrabObjectModule));
        }

        public void OnDisarmObjectEnd(AIObjectDataRetriever InvolvedAI)
        {
            this.AIDisarmingTheObject.Remove(InvolvedAI);
            if (this.AIDisarmingTheObject.Count == 0)
            {
                this.associatedIInteractiveObjectTypeEvents.EnableModule(typeof(GrabObjectModule), new InteractiveObjectInitializationObject());
            }
        }
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsAI)
            {
                var AIObjectDataRetriever = AILogicColliderModule.FromCollisionType(collisionType);
                AIObjectDataRetriever.GetAIBehavior().ReceiveEvent(new DisarmingObjectEnterAIbehaviorEvent(this));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsAI)
            {
                var AIObjectDataRetriever = AILogicColliderModule.FromCollisionType(collisionType);
                AIObjectDataRetriever.GetAIBehavior().ReceiveEvent(new DisarmingObjectExitAIbehaviorEvent(this));
            }
        }

        private void IncreaseTimeElapsedBy(float increasedTime)
        {
            this.elapsedTime += increasedTime;

            if (this.GetDisarmPercentage01() > 0 && !this.progressbar.gameObject.activeSelf)
            {
                CircleFillBarType.EnableInstace(this.progressbar);
            }
        }

        public static class DisarmObjectModuleInstancer
        {
            public static void PopuplateFromDefinition(DisarmObjectModuleDefinition DisarmObjectModuleDefinition, Transform parent)
            {
                var DisarmObjectModule = MonoBehaviour.Instantiate(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration.BaseDisarmObjectModule, parent);
                DisarmObjectModule.DisarmObjectID = DisarmObjectModuleDefinition.DisarmObjectID;
            }
        }
    }
}

