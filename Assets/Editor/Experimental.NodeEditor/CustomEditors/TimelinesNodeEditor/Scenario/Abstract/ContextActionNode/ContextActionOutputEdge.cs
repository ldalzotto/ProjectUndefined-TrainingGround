using AdventureGame;
using NodeGraph;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor_ScenarioNodeEditor
{
    [System.Serializable]
    public class ContextActionOutputEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() {
            typeof(TimelineContextActionLinkedEdge),
            typeof(ContextActionInputEdge)
        };

        public AContextAction GetContextAction()
        {
            return (AContextAction)((IContextActionNodeProfile)this.NodeProfileRef).GetContextAction();
        }

        protected override Color EdgeColor()
        {
            return MyColors.HotPink;
        }
    }
}