using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IContextMarkVisualFeedbackEvent
    {
        void CreateGenericMark(ModelObjectModule modelObjectModule);
        void CreateExclamationMark();
        void CreateDoubleExclamationMark();
        void Delete();
    }

}
