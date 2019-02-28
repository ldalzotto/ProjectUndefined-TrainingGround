using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{

    public class CutsceneTimelineAction : AContextAction
    {
        private CutsceneId cutsceneId;
        private bool DestroyPOIAtEnd;

        private CutsceneTimelineActionInput CutsceneTimelineActionInput;
        private CutsceneTimelinePOIData CutsceneTimelinePOIData;
        private bool isActionEnded;
        private bool isAgentDestinationReached;


        #region External Dependencies
        private PlayerManagerEventHandler PlayerManagerEventHandler;
        #endregion

        #region Internal Dependecnies
        private PlayerInitialPositionerManager PlayerInitialPositionerManager;

        public CutsceneId CutsceneId { get => cutsceneId; }
        #endregion

        public CutsceneTimelineAction(CutsceneId cutsceneId, AContextAction nextContextAction, bool destroyPOIAtEnd = false) : base(nextContextAction)
        {
            this.cutsceneId = cutsceneId;
            this.DestroyPOIAtEnd = destroyPOIAtEnd;
        }

        public override void AfterFinishedEventProcessed()
        {
            isActionEnded = false;
            isAgentDestinationReached = false;
            PlayerInitialPositionerManager = null;

            if (DestroyPOIAtEnd)
            {
                MonoBehaviour.Destroy(CutsceneTimelineActionInput.TargetedPOI.gameObject);
            }
        }

        public override bool ComputeFinishedConditions()
        {
            return isActionEnded;
        }

        public override void FirstExecutionAction(AContextActionInput ContextActionInput)
        {
            PlayerManagerEventHandler = GameObject.FindObjectOfType<PlayerManagerEventHandler>();
            var cutsceneTimelineContextActionInput = (CutsceneTimelineActionInput)ContextActionInput;
            this.CutsceneTimelineActionInput = cutsceneTimelineContextActionInput;
            if (cutsceneTimelineContextActionInput.PointOfInterestContextDataContainer != null && cutsceneTimelineContextActionInput.PointOfInterestContextDataContainer.CutsceneTimelinePOIDatas != null)
            {
                var cutscenePOIDatas = cutsceneTimelineContextActionInput.PointOfInterestContextDataContainer.CutsceneTimelinePOIDatas;
                for (var i = 0; i < cutscenePOIDatas.Length; i++)
                {
                    if (cutscenePOIDatas[i].CutsceneId == cutsceneId)
                    {
                        this.CutsceneTimelinePOIData = cutscenePOIDatas[i];
                        PlayerManagerEventHandler.StartCoroutine(SetAgentDestination(CutsceneTimelinePOIData.PlayerStartingTransform.position));
                        this.PlayerInitialPositionerManager = new PlayerInitialPositionerManager(cutsceneTimelineContextActionInput.PlayerTransform, CutsceneTimelinePOIData.PlayerStartingTransform);
                        break;
                    }
                }
            }
            else
            {
                //exit directly
                isActionEnded = true;
            }

        }

        public override void Tick(float d)
        {
            if (isAgentDestinationReached)
            {
                if (!this.PlayerInitialPositionerManager.IsTargetReached)
                {
                    this.PlayerInitialPositionerManager.Tick(d);
                    if (this.PlayerInitialPositionerManager.IsTargetReached)
                    {
                        this.CutsceneTimelinePOIData.StartCoroutine(PlayCutscene(CutsceneTimelinePOIData.PlayableDirector));
                    }
                }
            }

        }

        private IEnumerator SetAgentDestination(Vector3 destination)
        {
            yield return PlayerManagerEventHandler.StartCoroutine(PlayerManagerEventHandler.OnSetDestinationCoRoutine(destination));
            isAgentDestinationReached = true;
        }

        private IEnumerator PlayCutscene(PlayableDirector playableDirector)
        {
            playableDirector.Play();
            yield return new WaitEndOfCutscene(playableDirector);
            isActionEnded = true;
        }

    }

    #region Player Initial Positioner
    public class PlayerInitialPositionerManager
    {
        private Transform playerTransform;
        private Transform targetTransform;

        private bool isTargetReached;

        public PlayerInitialPositionerManager(Transform playerTransform, Transform targetTransform)
        {
            this.isTargetReached = false;
            this.playerTransform = playerTransform;
            this.targetTransform = targetTransform;
        }

        public bool IsTargetReached { get => isTargetReached; }

        public void Tick(float d)
        {
            playerTransform.position = targetTransform.position;
            playerTransform.rotation = Quaternion.LookRotation(targetTransform.forward);

            bool positionReached = false;
            bool rotationReached = false;

            if (Vector3.Distance(targetTransform.position, playerTransform.position) < 0.1)
            {
                positionReached = true;
                playerTransform.position = targetTransform.position;
            }

            if (Vector3.Dot(playerTransform.transform.forward, targetTransform.transform.forward) >= 0.99)
            {
                rotationReached = true;
                playerTransform.rotation = targetTransform.rotation;
            }
            if (positionReached && rotationReached)
            {
                isTargetReached = true;
            }

        }
    }
    #endregion

    public class CutsceneTimelineActionInput : AContextActionInput
    {
        private PointOfInterestType targetedPOI;
        private PointOfInterestContextDataContainer pointOfInterestContextDataContainer;
        private Transform playerTransform;

        public CutsceneTimelineActionInput(PointOfInterestType targetedPOI, PointOfInterestContextDataContainer pointOfInterestContextDataContainer, Transform playerTransform)
        {
            this.targetedPOI = targetedPOI;
            this.pointOfInterestContextDataContainer = pointOfInterestContextDataContainer;
            this.playerTransform = playerTransform;
        }

        public Transform PlayerTransform { get => playerTransform; }
        public PointOfInterestContextDataContainer PointOfInterestContextDataContainer { get => pointOfInterestContextDataContainer; }
        public PointOfInterestType TargetedPOI { get => targetedPOI; }
    }
}