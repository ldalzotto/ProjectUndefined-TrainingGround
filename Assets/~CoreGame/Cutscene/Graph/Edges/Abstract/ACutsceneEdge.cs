using CoreGame;
using NodeGraph;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    public abstract class ACutsceneEdge<T> : NodeEdgeProfile where T : SequencedAction
    {
        public T associatedAction;

        public override List<Type> AllowedConnectedNodeEdges => new List<Type>();

#if UNITY_EDITOR
        protected override void GUI_Impl(Rect rect, ref NodeEditorProfile nodeEditorProfileRef)
        {
            this.associatedAction.ActionGUI();
        }

        protected override Color EdgeColor()
        {
            return Color.yellow;
        }
#endif
    }

}
