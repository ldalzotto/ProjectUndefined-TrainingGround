using GameConfigurationID;
using System.Collections.Generic;

namespace RTPuzzle
{
    public interface IInRangeVisualFeedbackModuleDataRetriever
    {
        ModelObjectModule[] GetInRangeModelObjectsForVisual();
        RangeTypeID GetAssociatedRangeTypeID();
    }

}
