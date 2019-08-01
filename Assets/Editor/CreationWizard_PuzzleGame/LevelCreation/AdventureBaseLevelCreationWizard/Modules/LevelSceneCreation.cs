using Editor_LevelCreation;
using Editor_MainGameCreationWizard;
using System.IO;
using UnityEditor;

namespace Editor_AdventureBaseLevelCreationWizard
{
    public class LevelSceneCreation : CreateableSceneComponent
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var adventureLevelDynamicsCreation = editorProfile.GetModule<AdventureLevelDynamicCreation>();
            this.CreateNewScene();

            var scenePath = LevelPathHelper.BuildBaseLevelPath(InstancePath.LevelBasePath, editorInformationsData.LevelZonesID, editorInformationsData.LevelZonesID);
            if (this.SaveScene(scenePath))
            {
                PrefabUtility.InstantiatePrefab(editorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance);
                PrefabUtility.InstantiatePrefab(editorInformationsData.CommonGameConfigurations.AdventureCommonPrefabs.CommonAdventureObjects);
                PrefabUtility.InstantiatePrefab(editorInformationsData.CommonGameConfigurations.AdventureCommonPrefabs.AdventureGameManagersNonPersistant);
                PrefabUtility.InstantiatePrefab(adventureLevelDynamicsCreation.CreatedPrefab);
                this.SaveScene(scenePath);
                editorProfile.AddToGeneratedObjects(new UnityEngine.Object[] { this.CreatedSceneAsset });
            }
        }
    }
}