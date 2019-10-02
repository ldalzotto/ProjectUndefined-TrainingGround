using UnityEngine;
using System.Collections;
using GameConfigurationID;

namespace RTPuzzle
{
    public interface ILineVisualFeedbackEvent
    {
        void CreateLineFollowingModelObject(DottedLineID DottedLineID, ModelObjectModule ModelObjectModule, Component sourceTriggeringObject);
        void CreateLineDirectionPositioning(DottedLineID DottedLineID, Component sourceTriggeringObject);
        void DestroyLine(Component sourceTriggeringObject);
    }
}
