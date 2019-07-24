using CoreGame;
using GameConfigurationID;
using System;
using System.Collections;
using UnityEngine;

namespace AdventureGame
{
    public class CutscenePlayerManagerV2 : SequencedActionManager
    {
        protected override Action<SequencedAction> OnActionAdd
        {
            get
            {
                return (SequencedAction SequencedAction) =>
                {
                    this.OnAddAction(SequencedAction, this.currentInput);
                };
            }
        }

        protected override Action<SequencedAction, SequencedActionInput> OnActionFinished => null;

        #region External Dependencies
        private AdventureGameConfigurationManager AdventureGameConfigurationManager;
        private PointOfInterestManager PointOfInterestManager;
        private CutscenePositionsManager CutscenePositionsManager;
        private ContextActionEventManager ContextActionEventManager;
        private CutsceneGlobalController CutsceneGlobalController;
        private GhostsPOIManager GhostsPOIManager;
        private LevelManager LevelManager;
        private CutsceneEventManager CutsceneEventManager;
        #endregion

        public void Init()
        {
            this.AdventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();
            this.PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            this.CutscenePositionsManager = GameObject.FindObjectOfType<CutscenePositionsManager>();
            this.ContextActionEventManager = GameObject.FindObjectOfType<ContextActionEventManager>();
            this.CutsceneGlobalController = GameObject.FindObjectOfType<CutsceneGlobalController>();
            this.GhostsPOIManager = GameObject.FindObjectOfType<GhostsPOIManager>();
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
            this.CutsceneEventManager = GameObject.FindObjectOfType<CutsceneEventManager>();
        }

        #region state
        private bool isCutscenePlaying = false;
        public bool IsCutscenePlaying { get => isCutscenePlaying; }
        #endregion

        #region External Event
        public void ManualCutsceneStart(CutsceneId cutsceneId)
        {
            Coroutiner.Instance.StartCoroutine(this.ManualPlayCutscene(cutsceneId));
        }
        #endregion

        private CutsceneActionInput currentInput;

        private IEnumerator ManualPlayCutscene(CutsceneId cutsceneId)
        {
            yield return this.PlayCutscene(cutsceneId);
            this.ContextActionEventManager.OnContextActionFinished(new CutsceneTimelineAction(cutsceneId, null, false), new CutsceneTimelineActionInput(null));
        }

        public IEnumerator PlayCutscene(CutsceneId cutsceneId)
        {
            this.isCutscenePlaying = true;
            var cutsceneGraph = this.AdventureGameConfigurationManager.CutsceneConf()[cutsceneId].CutsceneGraph;
            this.currentInput = new CutsceneActionInput(cutsceneId, this.PointOfInterestManager, this.CutscenePositionsManager,
                this.ContextActionEventManager, this.AdventureGameConfigurationManager, this.CutsceneGlobalController, this.GhostsPOIManager, this.LevelManager);
            this.CutsceneEventManager.OnCutscneStarted(cutsceneId);
            this.OnAddAction(cutsceneGraph.GetRootAction(), this.currentInput);
            yield return new WaitUntil(() => { return !this.isCutscenePlaying; });

            //Reset some state to ensure that nothing wroong persist
            this.CutsceneGlobalController.SetCameraFollow(PointOfInterestId.PLAYER);
        }

        protected override void OnNoMoreActionToPlay()
        {
            this.isCutscenePlaying = false;
        }
    }

}
