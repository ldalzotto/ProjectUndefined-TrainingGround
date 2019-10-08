//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [ModuleMetadata("Interactive", "Attract object in range.")]
    public class AttractiveObjectModuleDefinition : AbstractInteractiveObjectDefinition
    {
        [DrawConfiguration(ConfigurationType = typeof(AttractiveObjectConfiguration))]
        [CustomEnum(ConfigurationType = typeof(AttractiveObjectConfiguration))]
        public AttractiveObjectId AttractiveObjectId;

        public override void CreateObject(Transform parent)
        {
            var puzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration;
            var attractiveObjectModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseAttractiveObjectModule, parent);
            attractiveObjectModule.AttractiveObjectId = this.AttractiveObjectId;
        }
    }
}