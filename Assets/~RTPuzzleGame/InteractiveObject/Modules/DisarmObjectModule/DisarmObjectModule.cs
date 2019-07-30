using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class DisarmObjectModule : InteractiveObjectModule
    {
        [CustomEnum()]
        public DisarmObjectID DisarmObjectID;

        #region Module Dependencies
        private ModelObjectModule ModelObjectModule;
        #endregion

        private DisarmObjectInherentData DisarmObjectInherentConfigurationData;

        private List<NPCAIManager> AiThatCanInteract;
        private float elapsedTime;

        public static InteractiveObjectType Instanciate(Vector3 worldPosition, DisarmObjectInherentData DisarmObjectInherentData)
        {
            InteractiveObjectType createdDisarmObject = MonoBehaviour.Instantiate(DisarmObjectInherentData.DisarmObjectPrefab);
            createdDisarmObject.Init(new InteractiveObjectInitializationObject(DisarmObjectInherentData: DisarmObjectInherentData));
            createdDisarmObject.transform.position = worldPosition;
            return createdDisarmObject;
        }

        public void Init(ModelObjectModule ModelObjectModule, DisarmObjectInherentData DisarmObjectInherentConfigurationData)
        {
            this.ModelObjectModule = ModelObjectModule;
            this.DisarmObjectInherentConfigurationData = DisarmObjectInherentConfigurationData;
            this.AiThatCanInteract = new List<NPCAIManager>();

            this.GetComponent<SphereCollider>().radius = this.DisarmObjectInherentConfigurationData.DisarmInteractionRange;
            this.elapsedTime = 0f;
        }

        #region Logical Conditions
        public bool IsAskingToBeDestroyed()
        {
            if (this.elapsedTime >= this.DisarmObjectInherentConfigurationData.DisarmTime)
            {
                if (this.DisarmObjectCutscenePlayer != null)
                {
                    this.DisarmObjectCutscenePlayer.InterruptAllActions();
                    this.DisarmObjectCutscenePlayer = null;
                }
                return true;
            }
            return false;
        }
        #endregion

        #region Data Retrieval
        public float GetDisarmPercentage01()
        {
            return this.elapsedTime / this.DisarmObjectInherentConfigurationData.DisarmTime;
        }
        public Vector3 GetProgressBarDisplayPosition()
        {
            return this.transform.position + IRenderBoundRetrievableStatic.GetDisarmProgressBarLocalOffset(this.ModelObjectModule);
        }
        #endregion

        private SequencedActionManager DisarmObjectCutscenePlayer;

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.DisarmObjectCutscenePlayer.IfNotNull(DisarmObjectCutscenePlayer => DisarmObjectCutscenePlayer.Tick(d * timeAttenuationFactor));
        }

        #region External Event
        public void IncreaseTimeElapsedBy(float increasedTime)
        {
            this.elapsedTime += increasedTime;
        }
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsAI)
            {
                var npcAIManager = NPCAIManager.FromCollisionType(collisionType);
                this.AiThatCanInteract.Add(npcAIManager);
                npcAIManager.OnDisarmObjectTriggerEnter(this);
                this.DisarmObjectCutscenePlayer = new SequencedActionManager(
                    OnActionAdd: (action) =>
                    {
                        this.DisarmObjectCutscenePlayer.OnAddAction(action, new PuzzleCutsceneActionInput(this.DisarmObjectInherentConfigurationData.PlayedCutsceneOnDisarm, GameObject.FindObjectOfType<LevelManager>(), GameObject.FindObjectOfType<NPCAIManagerContainer>()));
                    },
                    OnActionFinished: null,
                    OnNoMoreActionToPlay: () =>
                    {
                        this.DisarmObjectCutscenePlayer.InterruptAllActions();
                        this.DisarmObjectCutscenePlayer = null;
                    });
                ;
                this.DisarmObjectCutscenePlayer.OnAddAction(GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().PuzzleCutsceneConfiguration()[this.DisarmObjectInherentConfigurationData.PlayedCutsceneOnDisarm].PuzzleCutsceneGraph.GetRootAction(),
                           new PuzzleCutsceneActionInput(this.DisarmObjectInherentConfigurationData.PlayedCutsceneOnDisarm, GameObject.FindObjectOfType<LevelManager>(), GameObject.FindObjectOfType<NPCAIManagerContainer>()));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if (collisionType != null && collisionType.IsAI)
            {
                var npcAIManager = NPCAIManager.FromCollisionType(collisionType);
                this.AiThatCanInteract.Remove(npcAIManager);
                npcAIManager.OnDisarmObjectTriggerExit(this);
            }
        }

    }
}

