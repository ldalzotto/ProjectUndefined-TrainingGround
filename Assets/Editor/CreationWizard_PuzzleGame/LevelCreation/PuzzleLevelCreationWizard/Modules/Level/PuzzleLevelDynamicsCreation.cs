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
    public class PuzzleLevelDynamicsCreation : CreateablePrefabComponent<LevelManager>
    {
        public PuzzleLevelDynamicsCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        public override Func<Dictionary<string, CreationModuleComponent>, LevelManager> BasePrefabProvider
        {
            get
            {
                return (Dictionary<string, CreationModuleComponent> imodules) =>
                {
                    var modules = PuzzleLevelCreationWizardEditorProfile.GetAllModules(imodules);
                    return modules.EditorInformations.EditorInformationsData.PuzzleLevelCommonPrefabs.BasePuzzleLevelDynamics;
                };
            }
        }

        internal void OnGenerationClicked(EditorInformationsData editorInformationsData, AbstractCreationWizardEditorProfile editorProfile)
        {
            var createdLevelManager = this.Create(editorInformationsData.InstancePath.PuzzleLevelDynamicsPath, editorInformationsData.LevelZonesID.ToString() + NameConstants.PuzzleLevelDynamics);
            createdLevelManager.LevelID = editorInformationsData.LevelZonesID;
            PrefabUtility.SavePrefabAsset(createdLevelManager.gameObject);
            editorProfile.AddToGeneratedObjects(new UnityEngine.Object[] { createdLevelManager });
        }
    }

}
