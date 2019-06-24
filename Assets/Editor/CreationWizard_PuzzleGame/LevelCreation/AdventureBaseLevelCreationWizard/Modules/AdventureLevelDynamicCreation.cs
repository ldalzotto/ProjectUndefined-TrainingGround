using UnityEngine;
using System.Collections;
using CoreGame;
using System;
using System.Collections.Generic;
using Editor_MainGameCreationWizard;
using UnityEditor;

namespace Editor_AdventureBaseLevelCreationWizard
{
    [System.Serializable]
    public class AdventureLevelDynamicCreation : CreateablePrefabComponent<LevelManager>
    {
        public override Func<AbstractCreationWizardEditorProfile, LevelManager> BasePrefabProvider
        {
            get
            {
                return (AbstractCreationWizardEditorProfile editorProfile) =>
                {
                    return editorProfile.GetModule<EditorInformations>().EditorInformationsData.CommonGameConfigurations.AdventureCommonPrefabs.BaseAdventureLevelDynamics;
                };
            }
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var createdLevelManager = this.Create(editorInformationsData.CommonGameConfigurations.InstancePath.PuzzleLevelDynamicsPath, editorInformationsData.LevelZonesID.ToString() + NameConstants.AdventureLevelDynamics, editorProfile);
            createdLevelManager.LevelID = editorInformationsData.LevelZonesID;
            PrefabUtility.SavePrefabAsset(createdLevelManager.gameObject);
        }
    }

}
