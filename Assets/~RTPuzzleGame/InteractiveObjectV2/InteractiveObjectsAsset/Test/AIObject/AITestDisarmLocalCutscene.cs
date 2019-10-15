using CoreGame;
using InteractiveObjectTest;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AITestDisarmLocalCutscene", menuName = "Test/AITestDisarmLocalCutscene")]
    public class AITestDisarmLocalCutscene : LocalPuzzleCutsceneTemplate
    {
        public BaseCutsceneAnimationActionInput BaseCutsceneAnimationActionInput;
        public float RepeatWaitForSeconds;

        public override List<SequencedAction> GetSequencedActions(CoreInteractiveObject associatedInteractiveObject)
        {
            return new List<SequencedAction>() {
                new BranchInfiniteLoopAction(new List<SequencedAction>(){
                     new BaseCutsceneAnimationAction(this.BaseCutsceneAnimationActionInput, associatedInteractiveObject.CutsceneController, new List<SequencedAction>(){
                         new CutsceneWorkflowWaitForSecondsAction(this.RepeatWaitForSeconds, null)
                     })
                })
            };
        }
    }
}
