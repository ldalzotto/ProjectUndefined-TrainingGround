﻿using System;
using System.Collections.Generic;
using SequencedAction;
using UnityEngine;

namespace CoreGame
{
    [Serializable]
    public class CutsceneWorkflowAbortEdge : ACutsceneEdge<CutsceneWorkflowAbortAction>
    {
        [SerializeField] public override List<Type> AllowedConnectedNodeEdges => new List<Type>() {typeof(CutsceneWorkflowAbortEdge)};

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