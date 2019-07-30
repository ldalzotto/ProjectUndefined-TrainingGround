using UnityEngine;
using System.Collections;
using NodeGraph;

namespace CoreGame
{
    [System.Serializable]
    public abstract class AbstractCutsceneGraph : NodeEditorProfile
    {
        public SequencedAction GetRootAction()
        {
            SequencedAction rootAction = null;
            foreach (var node in this.Nodes.Values)
            {
                if (node.GetType() == typeof(CutsceneStartNode))
                {
                    var childNode = ((CutsceneStartNode)node).GetFirstNode();
                    childNode.BuildAction();
                    rootAction = childNode.GetAction();
                }
            }
            return rootAction;
        }
    }
}
