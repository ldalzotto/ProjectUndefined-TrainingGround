using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
