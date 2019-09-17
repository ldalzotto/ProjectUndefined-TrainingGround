using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
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

        #region Events Listener
        private IDisarmObjectModuleEventListener IAIDisarmObjectEventListener;
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
            this.IAIDisarmObjectEventListener = PuzzleGameSingletonInstances.PuzzleEventsManager;
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

        public void TickBeforeAIUpdate(float d, float timeAttenuationFactor)
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
            foreach (var involvedAI in this.AIDisarmingTheObject)
            {
                this.IAIDisarmObjectEventListener.PZ_DisarmObject_TriggerExit(this, involvedAI);
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
                this.IAIDisarmObjectEventListener.PZ_DisarmObject_TriggerEnter(this, AIObjectDataRetriever);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsAI)
            {
                var AIObjectDataRetriever = AILogicColliderModule.FromCollisionType(collisionType);
                this.IAIDisarmObjectEventListener.PZ_DisarmObject_TriggerExit(this, AIObjectDataRetriever);
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

