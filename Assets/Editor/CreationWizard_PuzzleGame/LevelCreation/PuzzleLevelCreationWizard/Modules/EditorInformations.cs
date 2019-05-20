using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using CreationWizard;
using RTPuzzle;
using System.Linq;
using System;
using CoreGame;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class EditorInformations : CreationModuleComponent
    {
        [SerializeField]
        public EditorInformationsData EditorInformationsData;

        public EditorInformations(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string headerDescriptionLabel => "Base informations used by the creation wizard.";

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            this.InitProperties();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.EditorInformationsData)), true);
        }

        private void InitProperties()
        {
            #region Puzzle Common Prefabs
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance, "_GameManagers_Persistance_Instanciater");
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements, "CorePuzzleSceneElements");
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleLevelCommonPrefabs.EventSystem, "EventSystem");
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleLevelCommonPrefabs.PuzzleDebugModule, "PuzzleDebugModule");
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleLevelCommonPrefabs.BasePuzzleLevelDynamics, "BasePuzzleLevelDynamics");
            #endregion

            #region Game Configuration
            AssetFinder.SafeSingleAssetFind(ref this.EditorInformationsData.PuzzleGameConfigurations.LevelConfiguration, "t:" + typeof(LevelConfiguration).Name);
            #endregion
        }

        public override string ComputeErrorState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            this.InitProperties();
            return new List<string>()
            {
                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleGameConfigurations.LevelConfiguration, nameof(this.EditorInformationsData.PuzzleGameConfigurations.LevelConfiguration)),

                ErrorHelper.NonNullity(this.EditorInformationsData.ScenePath, nameof(this.EditorInformationsData.ScenePath)),

                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance, nameof(this.EditorInformationsData.PuzzleLevelCommonPrefabs.GameManagerPersistanceInstance)),
                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements, nameof(this.EditorInformationsData.PuzzleLevelCommonPrefabs.CorePuzzleSceneElements) ),
                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleLevelCommonPrefabs.EventSystem, nameof(this.EditorInformationsData.PuzzleLevelCommonPrefabs.EventSystem) ),
                ErrorHelper.NonNullity(this.EditorInformationsData.PuzzleLevelCommonPrefabs.PuzzleDebugModule, nameof(this.EditorInformationsData.PuzzleLevelCommonPrefabs.PuzzleDebugModule) )
            }
            .Find((s) => !string.IsNullOrEmpty(s));

            // ErrorHelper.NonNullity(X, nameof(X)),
        }

        public override string ComputeWarningState(ref Dictionary<string, CreationModuleComponent> editorModules)
        {
            this.InitProperties();
            return ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.LevelZonesID,
                        this.EditorInformationsData.PuzzleGameConfigurations.LevelConfiguration.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e),
                        nameof(this.EditorInformationsData.PuzzleGameConfigurations.LevelConfiguration));
        }

    }

    [System.Serializable]
    public class EditorInformationsData
    {
        public PuzzleGameConfigurations PuzzleGameConfigurations;
        [SearchableEnum]
        public LevelZonesID LevelZonesID;
        [ReadOnly]
        public string ScenePath = "Assets/_Scenes/RTPuzzles";
        [ReadOnly]
        public string LevelConfigurationDataPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/LevelConfiguration/LevelConfigurationData";
        [ReadOnly]
        public string LevelCompletionDataPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/LevelConfiguration/LevelCompletionConfiguration/LevelCompletionConfigurationData";
        [ReadOnly]
        public string LevelCompletionConditionDataPath = "Assets/~RTPuzzleGame/Configuration/SubConfiguration/LevelConfiguration/LevelCompletionCondition/ConfigurationData";
        [ReadOnly]
        public string PuzzleLevelDynamicsPath = "Assets/~CoreGame/LevelManagement/Prefab";
        public PuzzleLevelCommonPrefabs PuzzleLevelCommonPrefabs;
    }

    [System.Serializable]
    public class PuzzleGameConfigurations
    {
        [ReadOnly]
        public LevelConfiguration LevelConfiguration;
    }

    [System.Serializable]
    public class PuzzleLevelCommonPrefabs
    {
        [ReadOnly]
        public GameManagerPersistanceInstance GameManagerPersistanceInstance;
        [ReadOnly]
        public GameObject CorePuzzleSceneElements;
        [ReadOnly]
        public EventSystem EventSystem;
        [ReadOnly]
        public PuzzleDebugModule PuzzleDebugModule;
        [ReadOnly]
        public LevelManager BasePuzzleLevelDynamics;
    }

}
