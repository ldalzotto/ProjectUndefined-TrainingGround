using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{

    public class CutsceneAction : RTPPlayerAction
    {
        private PuzzleCutsceneID puzzleCutsceneId;

        private SequencedActionManager cutscenePlayer;

        #region State
        private bool isCutsceneActionFinished;
        #endregion
        
        public CutsceneAction(PuzzleCutsceneID PuzzleCutsceneID, CorePlayerActionDefinition CorePlayerActionDefinition) : base(CorePlayerActionDefinition)
        {
            this.puzzleCutsceneId = PuzzleCutsceneID;
        }

        public override void FirstExecution()
        {
            Debug.Log(MyLog.Format("CutsceneAction : FirstExecution"));
            base.FirstExecution();

            #region External Dependencies
            var puzzleConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            #endregion
            
            this.cutscenePlayer = new SequencedActionManager((action) => this.cutscenePlayer.OnAddAction(action, null), null, OnNoMoreActionToPlay: this.OnCutsceneEnded);
            this.cutscenePlayer.OnAddActions(puzzleConfigurationManager.PuzzleCutsceneConfiguration()[this.puzzleCutsceneId].PuzzleCutsceneGraph.GetRootActions(), null);
            this.isCutsceneActionFinished = false;
        }

        #region Internal Events
        private void OnCutsceneEnded()
        {
            this.isCutsceneActionFinished = true;
            this.PlayerActionConsumed();
        }
        #endregion

        public override bool FinishedCondition()
        {
            return this.isCutsceneActionFinished;
        }

        public override void Tick(float d)
        {
            if (this.cutscenePlayer != null)
            {
                this.cutscenePlayer.Tick(d);
            }
        }

        public override void LateTick(float d) { }
        public override void GizmoTick() { }
        public override void GUITick() { }
    }
}
