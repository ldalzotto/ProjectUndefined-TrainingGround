using GameConfigurationID;
using System.Collections.Generic;

namespace RTPuzzle
{
    public interface IVisualFeedbackEmitterModuleDataRetriever
    {
        ModelObjectModule[] GetInRangeModelObjectsForVisual();
        RangeTypeID GetAssociatedRangeTypeID();
    }

}
