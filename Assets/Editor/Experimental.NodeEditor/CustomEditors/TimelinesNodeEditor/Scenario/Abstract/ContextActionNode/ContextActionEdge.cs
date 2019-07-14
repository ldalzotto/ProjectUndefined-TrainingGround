using UnityEngine;
using System.Collections;
using NodeGraph;
using AdventureGame;
using System;
using System.Collections.Generic;
using CoreGame;

namespace Editor_ScenarioNodeEditor
{
    [System.Serializable]
    public abstract class ContextActionEdge<T> : NodeEdgeProfile where T : ISequencedActionDrawable
    {
        [SerializeField]
        public T ContextAction;
        
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>();

        protected override void GUI_Impl(Rect rect, ref NodeEditorProfile nodeEditorProfileRef)
        {
            this.ContextAction.ActionGUI();
        }

        protected override Color EdgeColor()
        {
            return Color.yellow;
        }

    }
}