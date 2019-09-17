using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CoreGame;

namespace RTPuzzle
{
    public interface ILocalPuzzleCutsceneModuleEvent
    {
        void PlayLocalCutscene(int cutscenePlayerId, PuzzleCutsceneGraph PuzzleCutsceneGraph, Dictionary<CutsceneParametersName, object> PuzzleCutsceneActionInputParameters);
        void StopLocalCutscene(int cutscenePlayerId);
    }

}
