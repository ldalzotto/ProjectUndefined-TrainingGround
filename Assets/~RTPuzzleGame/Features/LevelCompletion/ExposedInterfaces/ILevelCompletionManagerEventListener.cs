using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface ILevelCompletionManagerEventListener
    {
        void PZ_EVT_LevelCompleted();
    }
}
