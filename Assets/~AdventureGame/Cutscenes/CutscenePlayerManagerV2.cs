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

        #region Internal Dependencies
        private CutsceneDeferredPersistanceManager CutsceneDeferredPersistanceManager;
        #endregion

        public void Init()
        {
            this.AdventureGameConfigurationManager = AdventureGameSingletonInstances.AdventureGameConfigurationManager;
            this.PointOfInterestManager = AdventureGameSingletonInstances.PointOfInterestManager;
            this.CutscenePositionsManager = AdventureGameSingletonInstances.CutscenePositionsManager;
            this.ContextActionEventManager = AdventureGameSingletonInstances.ContextActionEventManager;
            this.CutsceneGlobalController = AdventureGameSingletonInstances.CutsceneGlobalController;
            this.GhostsPOIManager = AdventureGameSingletonInstances.GhostsPOIManager;
            this.LevelManager = CoreGameSingletonInstances.LevelManager;
            this.CutsceneEventManager = AdventureGameSingletonInstances.CutsceneEventManager;

            this.CutsceneDeferredPersistanceManager = new CutsceneDeferredPersistanceManager();

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
            this.CutsceneDeferredPersistanceManager.OnCutsceneEnd();
            this.CutsceneEventManager.OnCutsceneEnded();
        }
        
        #region External Event
        public void ManualCutsceneStart(CutsceneId cutsceneId)
        {
            Coroutiner.Instance.StartCoroutine(this.ManualPlayCutscene(cutsceneId));
        }

        public void PushDeferredPersistance(CutsceneDeferredPOIpersistanceInput CutsceneDeferredPOIpersistanceInput)
        {
            this.CutsceneDeferredPersistanceManager.PushDeferredPersistance(CutsceneDeferredPOIpersistanceInput);
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
                this.ContextActionEventManager, this.AdventureGameConfigurationManager, this.CutsceneGlobalController, this.GhostsPOIManager, this.LevelManager, this.CutsceneEventManager);
            yield return base.PlayCutscene(cutsceneGraph, cutsceneInput);
        }
    }

}
