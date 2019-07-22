using UnityEngine;

namespace AdventureGame
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
