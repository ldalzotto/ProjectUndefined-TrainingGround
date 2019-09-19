using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface ILevelCompletionManagerEvent
    {
        void ConditionRecalculationEvaluate();
    }
}
