using GameConfigurationID;
using System.Collections.Generic;

namespace RTPuzzle
{
    public interface IVisualFeedbackEmitterModuleDataRetriever
    {
        List<ModelObjectModule> GetInRangeModelObjectsForVisual();
        RangeTypeID GetAssociatedRangeTypeID();
    }

}
