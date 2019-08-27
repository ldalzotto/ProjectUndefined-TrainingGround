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
        private PuzzleCutsceneActionInput cutscenePlayerInput;

        #region State
        private bool isCutsceneActionFinished;
        #endregion

        #region External Dependencies
        private InteractiveObjectContainer InteractiveObjectContainer;
        #endregion

        public CutsceneAction(CutsceneActionInherentData CutsceneActionInherentData) : base(CutsceneActionInherentData)
        {
            this.puzzleCutsceneId = CutsceneActionInherentData.PuzzleCutsceneId;
        }

        public override void FirstExecution()
        {
            base.FirstExecution();

            #region External Dependencies
            this.InteractiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();
            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            #endregion

            this.cutscenePlayerInput = new PuzzleCutsceneActionInput(this.InteractiveObjectContainer);

            this.cutscenePlayer = new SequencedActionManager((action) => this.cutscenePlayer.OnAddAction(action, this.cutscenePlayerInput), null, OnNoMoreActionToPlay: this.OnCutsceneEnded);
            this.cutscenePlayer.OnAddActions(puzzleConfigurationManager.PuzzleCutsceneConfiguration()[this.puzzleCutsceneId].PuzzleCutsceneGraph.GetRootActions(), this.cutscenePlayerInput);
            this.isCutsceneActionFinished = false;
        }

        #region Internal Events
        private void OnCutsceneEnded()
        {
            this.isCutsceneActionFinished = true;
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
