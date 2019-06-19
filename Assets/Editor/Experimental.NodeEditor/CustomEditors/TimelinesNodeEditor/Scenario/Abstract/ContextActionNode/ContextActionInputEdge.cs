using UnityEngine;
using System.Collections;
using NodeGraph;
using System.Collections.Generic;
using System;
using AdventureGame;

namespace Editor_ScenarioNodeEditor
{
    [System.Serializable]
    public class ContextActionInputEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>();

        public AContextAction GetAContextAction()
        {
            if(this.BackwardConnectedNodeEdges!=null && this.BackwardConnectedNodeEdges.Count > 0)
            {
                return ((ContextActionOutputEdge)this.BackwardConnectedNodeEdges[0]).GetContextAction();
            }
            return null;
        }

        protected override Color EdgeColor()
        {
            return MyColors.HotPink;
        }
    }
}