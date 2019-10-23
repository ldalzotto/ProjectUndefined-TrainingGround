using SequencedAction;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    public class BranchInfiniteLoopNode : ACutsceneNode<BranchInfiniteLoopAction, BranchInfiniteLoopEdge>
    {
#if UNITY_EDITOR
        protected override Color NodeColor()
        {
            return MyColors.MayaBlue;
        }
#endif
    }
}