using CoreGame;
using NodeGraph;
using System;
using System.Collections.Generic;
using SequencedAction;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    public abstract class ACutsceneEdge<T> : NodeEdgeProfile where T : ASequencedAction
    {
        public T associatedAction;

        [SerializeField] public override List<Type> AllowedConnectedNodeEdges => new List<Type>();

#if UNITY_EDITOR
        protected override void GUI_Impl(Rect rect, ref NodeEditorProfile nodeEditorProfileRef)
        {
        }

        protected override Color EdgeColor()
        {
            return Color.yellow;
        }
#endif
    }
}