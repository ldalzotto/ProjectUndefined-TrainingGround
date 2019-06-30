using CoreGame;
using NodeGraph;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "CutsceneGraph", menuName = "Configuration/AdventureGame/CutsceneConfiguration/CutsceneGraph", order = 1)]
    public class CutsceneGraph : NodeEditorProfile
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
