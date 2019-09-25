using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleDiscussionWindowManager : AbstractDiscussionWindowManager
    {
        public PuzzleDiscussionWindowManager(DiscussionTreeId DiscussionTreeId, Dictionary<CutsceneParametersName, object> discussionGraphParameters)
        {
            this.BaseInit(DiscussionTreeId, discussionGraphParameters, new WaitForSecondsDiscussionWindowManagerStrategy(1f));
        }

        protected override bool GetAbstractTextOnlyNodePosition(AbstractDiscussionTextOnlyNode abstractDiscussionTextOnlyNode, out Vector3 worldPosition, out WindowPositionType WindowPositionType)
        {
            if (!base.GetAbstractTextOnlyNodePosition(abstractDiscussionTextOnlyNode, out worldPosition, out WindowPositionType))
            {
                if (abstractDiscussionTextOnlyNode.GetType() == typeof(PuzzleDiscussionTextOnlyNode))
                {
                    var PuzzleDiscussionTextOnlyNode = (PuzzleDiscussionTextOnlyNode)abstractDiscussionTextOnlyNode;
                    var modelObject = PuzzleDiscussionTextOnlyNode.ParametrizedTalker.Resolve(this.discussionGraphParameters, PuzzleGameSingletonInstances.InteractiveObjectContainer).GetModelObjectModule();
                    worldPosition = modelObject.transform.position + new Vector3(0, modelObject.GetAverageModelBoundLocalSpace().SideDistances.y, 0);
                    WindowPositionType = WindowPositionType.WORLD;
                    return true;
                }
                return false;
            }
            return true;
        }
    }
}
