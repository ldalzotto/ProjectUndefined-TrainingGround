using System;
using System.Collections.Generic;
using NodeGraph;
using SequencedAction;

namespace CoreGame
{
    [Serializable]
    public abstract class AbstractCutsceneGraph : NodeEditorProfile
    {
        public List<ASequencedAction> GetRootActions()
        {
            foreach (var node in this.Nodes.Values)
            {
                if (node.GetType() == typeof(CutsceneStartNode))
                {
                    var childNodes = ((CutsceneStartNode) node).GetFirstNodes();
                    childNodes.ForEach((childNode) => childNode.BuildAction());
                    return childNodes.ConvertAll((childNode) => childNode.GetAction());
                }
            }

            return null;
        }
    }
}