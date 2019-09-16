using UnityEngine;
using System.Collections;
using GameConfigurationID;

namespace RTPuzzle
{
    public interface ILineVisualFeedbackEvent
    {
        void CreateLine(DottedLineID DottedLineID, ModelObjectModule ModelObjectModule);
        void DestroyLine();
    }
}
