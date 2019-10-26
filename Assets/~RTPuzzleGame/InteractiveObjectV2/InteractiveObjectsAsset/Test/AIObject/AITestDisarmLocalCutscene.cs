using System;
using System.Collections.Generic;
using AnimatorPlayable;
using InteractiveObjects;
using InteractiveObjects_AnimationController;
using SequencedAction;
using UnityEngine;

namespace RTPuzzle
{
    [Serializable]
    [CreateAssetMenu(fileName = "AITestDisarmLocalCutscene", menuName = "Test/AITestDisarmLocalCutscene")]
    public class AITestDisarmLocalCutscene : LocalPuzzleCutsceneTemplate
    {
        public SequencedAnimationInput BaseAnimationInput;
        public float RepeatWaitForSeconds;

        public override List<ASequencedAction> GetSequencedActions(CoreInteractiveObject associatedInteractiveObject)
        {
            return new List<ASequencedAction>()
            {
                new BranchInfiniteLoopAction(new List<ASequencedAction>()
                {
                    new PlayContextAction(associatedInteractiveObject.AnimationController, this.BaseAnimationInput, () => new List<ASequencedAction>()
                    {
                        new CutsceneWorkflowWaitForSecondsAction(this.RepeatWaitForSeconds, null)
                    })
                })
            };
        }
    }
}