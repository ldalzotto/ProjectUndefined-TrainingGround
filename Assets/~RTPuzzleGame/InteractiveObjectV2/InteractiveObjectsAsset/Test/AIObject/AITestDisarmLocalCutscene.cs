using CoreGame;
using InteractiveObjects;
using System.Collections.Generic;
using SequencedAction;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AITestDisarmLocalCutscene", menuName = "Test/AITestDisarmLocalCutscene")]
    public class AITestDisarmLocalCutscene : LocalPuzzleCutsceneTemplate
    {
        public BaseCutsceneAnimationActionInput BaseCutsceneAnimationActionInput;
        public float RepeatWaitForSeconds;

        public override List<ASequencedAction> GetSequencedActions(CoreInteractiveObject associatedInteractiveObject)
        {
            return new List<ASequencedAction>()
            {
                new BranchInfiniteLoopAction(new List<ASequencedAction>()
                {
                    new BaseCutsceneAnimationAction(this.BaseCutsceneAnimationActionInput, associatedInteractiveObject.CutsceneController, () => new List<ASequencedAction>()
                    {
                        new CutsceneWorkflowWaitForSecondsAction(this.RepeatWaitForSeconds, null)
                    })
                })
            };
        }
    }
}