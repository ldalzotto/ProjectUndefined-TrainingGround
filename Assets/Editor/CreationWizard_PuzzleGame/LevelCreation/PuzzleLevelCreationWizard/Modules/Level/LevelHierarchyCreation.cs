using UnityEngine;
using System.Collections;
using CoreGame;
using Editor_PuzzleGameCreationWizard;
using UnityEditor;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelHierarchyCreation : CreateableScriptableObjectComponent<LevelHierarchyConfigurationData>
    {

        public LevelHierarchyCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string objectFieldLabel => typeof(LevelHierarchyConfigurationData).Name;

        public void OnGenerationClicked(EditorInformationsData editorInformationsData, AbstractCreationWizardEditorProfile editorProfile)
        {
            var generatedHierarchy = this.CreateAsset(editorInformationsData.InstancePath.PuzzleLevelHierarchyDataPath, editorInformationsData.LevelZonesID + NameConstants.LevelHierarchyConfigurationData);
            editorProfile.AddToGeneratedObjects(new Object[] { generatedHierarchy });

            editorInformationsData.PuzzleGameConfigurations.LevelHierarchyConfiguration.SetEntry(editorInformationsData.LevelZonesID, generatedHierarchy);
            editorProfile.GameConfigurationModified(editorInformationsData.PuzzleGameConfigurations.LevelHierarchyConfiguration, editorInformationsData.LevelZonesID, generatedHierarchy);
        }
    }
}