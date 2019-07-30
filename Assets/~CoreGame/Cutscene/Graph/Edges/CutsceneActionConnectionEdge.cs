using NodeGraph;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    public class CutsceneActionConnectionEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() { typeof(CutsceneActionConnectionEdge) };

#if UNITY_EDITOR
        protected override Color EdgeColor()
        {
            return Color.red;
        }
#endif
    }

}
