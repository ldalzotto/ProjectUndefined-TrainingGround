using UnityEngine;
using CoreGame;
using SequencedAction;

namespace CoreGame
{
    [System.Serializable]
    public class CutsceneWorkflowWaitForSecondsNode : ACutsceneNode<CutsceneWorkflowWaitForSecondsAction, CutsceneWorkflowWaitForSecondsEdge>
    {
#if UNITY_EDITOR
        public override bool DisplayWorkflowEdge()
        {
            return false;
        }

        protected override Color NodeColor()
        {
            return MyColors.HotPink;
        }
#endif
    }
}