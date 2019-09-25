using UnityEngine;
using System.Collections;
using GameConfigurationID;

namespace RTPuzzle
{
    public interface ILineVisualFeedbackEvent
    {
        void CreateLineFollowingModelObject(DottedLineID DottedLineID, ModelObjectModule ModelObjectModule, MonoBehaviour sourceTriggeringObject);
        void CreateLineDirectionPositioning(DottedLineID DottedLineID, MonoBehaviour sourceTriggeringObject);
        void DestroyLine(MonoBehaviour sourceTriggeringObject);
    }
}
