using UnityEngine;
using System.Collections;
using NodeGraph;
using System.Collections.Generic;

namespace CoreGame
{
    [System.Serializable]
    public abstract class AbstractCutsceneGraph : NodeEditorProfile
    {
        public List<SequencedAction> GetRootActions()
        {
            foreach (var node in this.Nodes.Values)
            {
                if (node.GetType() == typeof(CutsceneStartNode))
                {
                    var childNodes = ((CutsceneStartNode)node).GetFirstNodes();
                    childNodes.ForEach((childNode) => childNode.BuildAction());
                    return childNodes.ConvertAll((childNode) => childNode.GetAction());
                }
            }
            return null;
        }
    }
}
