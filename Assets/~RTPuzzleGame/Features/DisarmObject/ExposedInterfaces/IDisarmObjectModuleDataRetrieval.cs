using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public interface IDisarmObjectModuleDataRetrieval
    {
        Transform GetTransform();
        int GetInstanceID();
        PuzzleCutsceneGraph GetDisarmAnimation();
        Dictionary<CutsceneParametersName, object> GetDisarmAnimationInputParameters(IInteractiveObjectTypeDataRetrieval animatedInteractiveObject);

        IDisarmObjectModuleEvent GetIDisarmObjectModuleEvent();
    }
}
