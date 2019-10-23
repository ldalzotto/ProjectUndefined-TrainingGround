using SequencedAction;
using UnityEngine;

namespace CoreGame
{
    public class BranchInfiniteLoopEdge : ACutsceneEdge<BranchInfiniteLoopAction>
    {
#if UNITY_EDITOR
        protected override Color EdgeColor()
        {
            return MyColors.MayaBlue;
        }

        protected override float DefaultGetEdgeHeight()
        {
            return 50f;
        }
#endif
    }
}