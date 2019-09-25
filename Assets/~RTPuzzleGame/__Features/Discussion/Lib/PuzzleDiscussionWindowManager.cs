using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleDiscussionWindowManager : AbstractDiscussionWindowManager
    {
        public PuzzleDiscussionWindowManager(DiscussionTreeId DiscussionTreeId)
        {
            this.BaseInit(DiscussionTreeId, new WaitForSecondsDiscussionWindowManagerStrategy(1f));
        }

        protected override bool GetAbstractTextOnlyNodePosition(AbstractDiscussionTextOnlyNode abstractDiscussionTextOnlyNode, out Vector3 worldPosition, out WindowPositionType WindowPositionType)
        {
            if (!base.GetAbstractTextOnlyNodePosition(abstractDiscussionTextOnlyNode, out worldPosition, out WindowPositionType))
            {
                if (abstractDiscussionTextOnlyNode.GetType() == typeof(PuzzleDiscussionTextOnlyNode))
                {
                    var PuzzleDiscussionTextOnlyNode = (PuzzleDiscussionTextOnlyNode)abstractDiscussionTextOnlyNode;
                    var modelObject = PuzzleGameSingletonInstances.InteractiveObjectContainer.GetInteractiveObjectFirst(PuzzleDiscussionTextOnlyNode.Talker).GetModelObjectModule();
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
