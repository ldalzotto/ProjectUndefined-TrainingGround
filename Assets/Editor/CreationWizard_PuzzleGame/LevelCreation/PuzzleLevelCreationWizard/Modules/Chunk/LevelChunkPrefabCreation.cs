using UnityEngine;
using System.Collections;
using CoreGame;
using System;
using System.Collections.Generic;
using Editor_PuzzleGameCreationWizard;
using UnityEditor;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelChunkPrefabCreation : CreateablePrefabComponent<LevelChunkType>
    {
        public LevelChunkPrefabCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public override Func<Dictionary<string, CreationModuleComponent>, LevelChunkType> BasePrefabProvider
        {
            get
            {
                return (Dictionary<string, CreationModuleComponent> imodules) =>
                {
                    var modules = PuzzleLevelCreationWizardEditorProfile.GetAllModules(imodules);
                    return modules.EditorInformations.EditorInformationsData.PuzzleLevelCommonPrefabs.BaseLevelChunkPrefab;
                };
            }
        }

        public void OnGenerationClicked(EditorInformationsData editorInformationsData, AbstractCreationWizardEditorProfile editorProfile)
        {
            var createdBaseChunk = this.Create(editorInformationsData.InstancePath.LevelChunkBaseLevelPrefabPath, editorInformationsData.LevelZonesID.ToString() + NameConstants.BaseLevelChunkPrefab);
            createdBaseChunk.LevelZoneChunkID = editorInformationsData.LevelZoneChunkID;
            PrefabUtility.SavePrefabAsset(createdBaseChunk.gameObject);
            editorProfile.AddToGeneratedObjects(new UnityEngine.Object[] { createdBaseChunk });
        }
    }
}