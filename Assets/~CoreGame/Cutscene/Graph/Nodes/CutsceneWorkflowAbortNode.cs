﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using NodeGraph;
using SequencedAction;

namespace CoreGame
{
    [System.Serializable]
    public class CutsceneWorkflowAbortNode : ACutsceneNode<CutsceneWorkflowAbortAction, CutsceneWorkflowAbortEdge>
    {
        public override void AfterActionInitialized()
        {
            List<ASequencedAction> actionsToAbort = new List<ASequencedAction>();
            if (this.actionEdge.BackwardConnectedNodeEdges != null)
            {
                actionsToAbort.AddRange(this.actionEdge.BackwardConnectedNodeEdges.ConvertAll(e => ((ICutsceneNode) e.NodeProfileRef).GetAction()));
            }

            this.actionEdge.associatedAction.SequencedActionsToInterrupt = actionsToAbort;
        }

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