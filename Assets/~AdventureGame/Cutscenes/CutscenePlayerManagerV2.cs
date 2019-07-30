using CoreGame;
using GameConfigurationID;
using System.Collections;
using UnityEngine;

namespace AdventureGame
{
    
    public class CutscenePlayerManagerV2 : AbstractCutscenePlayerManager
    {
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
            this.BaseInit(this.OnCutsceneStart, this.OnCutsceneEnd);
        }

        private void OnCutsceneStart()
        {
            this.CutsceneEventManager.OnCutscneStarted();
        }

        private void OnCutsceneEnd()
        {
            //Reset some state to ensure that nothing wrong persist
            this.CutsceneGlobalController.SetCameraFollow(PointOfInterestId.PLAYER);
        }

        #region External Event
        public void ManualCutsceneStart(CutsceneId cutsceneId)
        {
            Coroutiner.Instance.StartCoroutine(this.ManualPlayCutscene(cutsceneId));
        }
        #endregion

        private IEnumerator ManualPlayCutscene(CutsceneId cutsceneId)
        {
            yield return this.PlayCutscene(cutsceneId);
            this.ContextActionEventManager.OnContextActionFinished(new CutsceneTimelineAction(cutsceneId, null, false), new CutsceneTimelineActionInput(null));
        }

        public IEnumerator PlayCutscene(CutsceneId cutsceneId)
        {
            var cutsceneGraph = this.AdventureGameConfigurationManager.CutsceneConf()[cutsceneId].CutsceneGraph;
            var cutsceneInput = new CutsceneActionInput(cutsceneId, this.PointOfInterestManager, this.CutscenePositionsManager,
                this.ContextActionEventManager, this.AdventureGameConfigurationManager, this.CutsceneGlobalController, this.GhostsPOIManager, this.LevelManager);
            yield return base.PlayCutscene(cutsceneGraph, cutsceneInput);
        }
    }

}
