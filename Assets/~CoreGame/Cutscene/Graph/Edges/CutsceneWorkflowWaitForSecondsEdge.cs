using System;
using System.Collections.Generic;
using UnityEngine;
using CoreGame;

namespace CoreGame
{
    [System.Serializable]
    public class CutsceneWorkflowWaitForSecondsEdge : ACutsceneEdge<CutsceneWorkflowWaitForSecondsAction>
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() { typeof(CutsceneWorkflowAbortEdge) };


#if UNITY_EDITOR
        protected override Color EdgeColor()
        {
            return MyColors.HotPink;
        }

        protected override float DefaultGetEdgeHeight()
        {
            return 50f;
        }
#endif
    }
}
