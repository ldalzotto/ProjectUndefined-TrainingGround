using CoreGame;
using NodeGraph;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    public abstract class ACutsceneEdge<T> : NodeEdgeProfile where T : SequencedAction
    {
        [SerializeField]
        private T associatedAction;

        public override List<Type> AllowedConnectedNodeEdges => new List<Type>();

        public T AssociatedAction { get => associatedAction; }

#if UNITY_EDITOR
        protected override void GUI_Impl(Rect rect)
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
