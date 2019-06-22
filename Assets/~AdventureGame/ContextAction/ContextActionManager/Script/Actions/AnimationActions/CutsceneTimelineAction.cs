using CoreGame;
using GameConfigurationID;
using System;
using System.Collections;
using UnityEngine;

namespace AdventureGame
{

    [System.Serializable]
    public class CutsceneTimelineAction : AContextAction
    {
        [SerializeField]
        private CutsceneId cutsceneId;
        [SerializeField]
        private bool DestroyPOIAtEnd;


        [NonSerialized]
        private CutsceneTimelineActionInput CutsceneTimelineActionInput;
        [NonSerialized]
        private bool isActionEnded;
        [NonSerialized]
        private bool isAgentDestinationReached;


        #region External Dependencies
        [NonSerialized]
        private APointOfInterestEventManager PointOfInterestEventManager;
        #endregion
        
        public CutsceneId CutsceneId { get => cutsceneId; }

        public CutsceneTimelineAction(CutsceneId cutsceneId, AContextAction nextContextAction, bool destroyPOIAtEnd = false) : base(nextContextAction)
        {
            this.cutsceneId = cutsceneId;
            this.DestroyPOIAtEnd = destroyPOIAtEnd;
        }

        public override void AfterFinishedEventProcessed()
        {
            isActionEnded = false;
            if (DestroyPOIAtEnd)
            {
                PointOfInterestEventManager.DisablePOI(CutsceneTimelineActionInput.TargetedPOI);
            }
        }

        public override bool ComputeFinishedConditions()
        {
            return isActionEnded;
        }

        public override void FirstExecutionAction(AContextActionInput ContextActionInput)
        {
            Coroutiner.Instance.StartCoroutine(this.PlayCutscene());
            #region External dependencies
            PointOfInterestEventManager = GameObject.FindObjectOfType<APointOfInterestEventManager>();
            #endregion
            var cutsceneTimelineContextActionInput = (CutsceneTimelineActionInput)ContextActionInput;
            this.CutsceneTimelineActionInput = cutsceneTimelineContextActionInput;


            /*
            if (cutsceneTimelineContextActionInput.PointOfInterestContextDataContainer != null && cutsceneTimelineContextActionInput.PointOfInterestContextDataContainer.CutsceneTimelinePOIDatas != null)
            {
                var cutscenePOIDatas = cutsceneTimelineContextActionInput.PointOfInterestContextDataContainer.CutsceneTimelinePOIDatas;
                for (var i = 0; i < cutscenePOIDatas.Length; i++)
                {
                    if (cutscenePOIDatas[i].CutsceneId == cutsceneId)
                    {
                        this.CutsceneTimelinePOIData = cutscenePOIDatas[i];
                        PlayerManagerEventHandler.StartCoroutine(SetAgentDestination(CutsceneTimelinePOIData.PlayerStartingTransform));
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
            */

        }

        public override void Tick(float d)
        {
            /*
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
            */

        }
        

        private IEnumerator PlayCutscene()
        {
            yield return GameObject.FindObjectOfType<CutscenePlayerManager>().PlayCutscene(this.cutsceneId);
            isActionEnded = true;
        }

    }

    public class CutsceneTimelineActionInput : AContextActionInput
    {
        private PointOfInterestType targetedPOI;

        public CutsceneTimelineActionInput(PointOfInterestType targetedPOI)
        {
            this.targetedPOI = targetedPOI;
        }
        
        public PointOfInterestType TargetedPOI { get => targetedPOI; }
    }
}