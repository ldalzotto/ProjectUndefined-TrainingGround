using UnityEngine;
using System.Collections;
using NodeGraph;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelCompletionCondition", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelCompletion/LevelCompletionCondition", order = 1)]
    public class ConditionGraphEditorProfile : NodeEditorProfile
    {
        public ConditionGraphEditorProfile()
        {
        }

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
        private TargetZoneContainer targetZoneContainer;

        public LevelCompletionConditionResolutionInput(NPCAIManagerContainer nPCAIManagerContainer, TargetZoneContainer targetZoneContainer)
        {
            this.nPCAIManagerContainer = nPCAIManagerContainer;
            this.targetZoneContainer = targetZoneContainer;
        }

        public NPCAIManagerContainer NPCAIManagerContainer { get => nPCAIManagerContainer; }
        public TargetZoneContainer TargetZoneContainer { get => targetZoneContainer; }
    }
}