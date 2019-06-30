using NodeGraph;
using System;
using System.Collections.Generic;

namespace AdventureGame
{
    [System.Serializable]
    public class CutsceneActionConnectionEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() { typeof(CutsceneActionConnectionEdge) };
    }

}
