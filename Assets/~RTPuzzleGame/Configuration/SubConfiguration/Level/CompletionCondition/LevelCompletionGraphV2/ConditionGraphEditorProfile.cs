using NodeGraph;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelCompletionCondition", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelCompletion/LevelCompletionCondition", order = 1)]
    public class ConditionGraphEditorProfile : NodeEditorProfile
    {

        public bool Resolve(ref LevelCompletionConditionResolutionInput ConditionGraphResolutionInput)
        {
            OutputResultNode resultNode = null;
            foreach (var node in this.Nodes.Values)
            {
                if (node.GetType() == typeof(OutputResultNode))
                {
                    resultNode = (OutputResultNode)node;
                }
            }

            if (resultNode != null)
            {
                return resultNode.Resolve(ref ConditionGraphResolutionInput);
            }
            return false;
        }
    }

    public class LevelCompletionConditionResolutionInput
    {
        private NPCAIManagerContainer nPCAIManagerContainer;
        private InteractiveObjectContainer interactiveObjectContainer;

        public LevelCompletionConditionResolutionInput(NPCAIManagerContainer nPCAIManagerContainer, InteractiveObjectContainer interactiveObjectContainer)
        {
            this.nPCAIManagerContainer = nPCAIManagerContainer;
            this.interactiveObjectContainer = interactiveObjectContainer;
        }

        public NPCAIManagerContainer NPCAIManagerContainer { get => nPCAIManagerContainer; }
        public InteractiveObjectContainer InteractiveObjectContainer { get => interactiveObjectContainer; }
    }
}