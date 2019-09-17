using UnityEngine;

namespace RTPuzzle
{
    public interface IDisarmObjectModuleDataRetrieval
    {
        Transform GetTransform();
        PuzzleCutsceneGraph GetDisarmAnimation();
    }
}
