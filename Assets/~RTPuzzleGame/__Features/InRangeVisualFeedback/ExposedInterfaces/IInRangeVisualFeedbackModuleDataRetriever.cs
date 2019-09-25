using GameConfigurationID;
using System.Collections.Generic;

namespace RTPuzzle
{
    public interface IInRangeVisualFeedbackModuleDataRetriever
    {
        IAILogicColliderModuleDataRetriever[] GetInRangeCollidersForVisual();
        RangeTypeID GetAssociatedRangeTypeID();
    }

}
