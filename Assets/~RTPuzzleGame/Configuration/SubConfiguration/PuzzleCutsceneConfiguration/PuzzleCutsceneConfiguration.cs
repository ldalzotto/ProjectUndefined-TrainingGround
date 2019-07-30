using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzleCutsceneConfiguration", menuName = "Configuration/PuzzleGame/PuzzleCutsceneConfiguration/PuzzleCutsceneConfiguration", order = 1)]
    public class PuzzleCutsceneConfiguration : ConfigurationSerialization<PuzzleCutsceneId, PuzzleCutsceneInherentData>
    {  }
}