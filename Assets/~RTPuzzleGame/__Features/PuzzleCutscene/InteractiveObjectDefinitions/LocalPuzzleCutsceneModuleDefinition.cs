﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [ModuleMetadata("Interactive", "Play any PuzzleCutscene graph.")]
    public class LocalPuzzleCutsceneModuleDefinition : AbstractInteractiveObjectDefinition
    {
        public override void CreateObject(Transform parent)
        {
            var puzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration;
            MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseLocalPuzzleCutsceneModule, parent);
        }
    }
}

