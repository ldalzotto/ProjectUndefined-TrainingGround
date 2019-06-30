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
        #endregion
        public void Init()
        {
            this.AdventureGameConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();
            this.PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            this.CutscenePositionsManager = GameObject.FindObjectOfType<CutscenePositionsManager>();
            this.ContextActionEventManager = GameObject.FindObjectOfType<ContextActionEventManager>();
        }

        #region state
        private bool isCutscenePlaying = false;
        public bool IsCutscenePlaying { get => isCutscenePlaying; }
        #endregion

        #region External Event
        public void OnCutsceneStart(CutsceneId cutsceneId)
        {
            Coroutiner.Instance.StartCoroutine(this.PlayCutscene(cutsceneId));
        }
        #endregion

        private CutsceneActionInput currentInput;

        public IEnumerator PlayCutscene(CutsceneId cutsceneId)
        {
            this.isCutscenePlaying = true;
            var cutsceneGraph = this.AdventureGameConfigurationManager.CutsceneConf()[cutsceneId].CutsceneGraph;
            this.currentInput = new CutsceneActionInput(cutsceneId, this.PointOfInterestManager, this.CutscenePositionsManager, this.ContextActionEventManager, this.AdventureGameConfigurationManager);
            this.OnAddAction(cutsceneGraph.GetRootAction(), this.currentInput);
            yield return new WaitUntil(() => { return this.isCutscenePlaying; });
        }

        protected override void OnNoMoreActionToPlay()
        {
            this.isCutscenePlaying = false;
        }
    }

}
