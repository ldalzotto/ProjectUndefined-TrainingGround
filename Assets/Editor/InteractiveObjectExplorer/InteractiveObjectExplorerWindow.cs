using CoreGame;
using Editor_MainGameCreationWizard;
using GameConfigurationID;
using RTPuzzle;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_InteractiveObjectExplorer
{
    [System.Serializable]
    public class InteractiveObjectExplorerWindow
    {
        private CommonGameConfigurations CommonGameConfigurations;
        private InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration;
        
        private List<InteractiveObjectType> SceneInteractiveObjects;
        private List<InteractiveObjectTypeDefinitionID> PlayerActionsInteractiveObjectDefinitions;


        private FoldableArea GizmoDisplayArea;
        private Dictionary<InteractiveObjectTypeDefinitionID, InteractiveObjectGizmoDisplay> GizmoDisplay;


        public void OnEnable()
        {
            if (this.CommonGameConfigurations == null) { this.CommonGameConfigurations = new CommonGameConfigurations(); EditorInformationsHelper.InitProperties(ref this.CommonGameConfigurations); }
            if (this.InteractiveObjectTypeDefinitionConfiguration == null) { this.InteractiveObjectTypeDefinitionConfiguration = AssetFinder.SafeSingleAssetFind<InteractiveObjectTypeDefinitionConfiguration>("t:" + typeof(InteractiveObjectTypeDefinitionConfiguration).Name); }
            if (this.GizmoDisplayArea == null) { this.GizmoDisplayArea = new FoldableArea(true, "Gizmos : ", false); }
            if (this.GizmoDisplay == null) { this.GizmoDisplay = new Dictionary<InteractiveObjectTypeDefinitionID, InteractiveObjectGizmoDisplay>(); }
            if (this.SceneInteractiveObjects == null)
            {
                this.SceneInteractiveObjects = GameObject.FindObjectsOfType<InteractiveObjectType>().ToList();
                this.SceneInteractiveObjects.ForEach(interactiveObject => this.GizmoDisplay[interactiveObject.InteractiveObjectTypeDefinitionID] = new InteractiveObjectGizmoDisplay(true, interactiveObject, this.CommonGameConfigurations, this.InteractiveObjectTypeDefinitionConfiguration));
            }
            if(this.PlayerActionsInteractiveObjectDefinitions == null)
            {
                var levelManager = GameObject.FindObjectOfType<LevelManager>();
                this.PlayerActionsInteractiveObjectDefinitions = new List<InteractiveObjectTypeDefinitionID>();
                if (levelManager != null)
                {
                    var LevelConfiguration = AssetFinder.SafeSingleAssetFind<LevelConfiguration>("t:" + typeof(LevelConfiguration).Name);
                    var PlayerActionConfiguration = AssetFinder.SafeSingleAssetFind<PlayerActionConfiguration>("t:" + typeof(PlayerActionConfiguration).Name);
                    LevelConfiguration.ConfigurationInherentData[levelManager.LevelID].PlayerActionIds.ForEach((pa) =>
                    {
                        var playerActionAsset = PlayerActionConfiguration.ConfigurationInherentData[pa.playerActionId];
                        if (playerActionAsset != null)
                        {
                            if (playerActionAsset.GetType() == typeof(LaunchProjectileActionInherentData))
                            {
                                var objectDefinitionID = ((LaunchProjectileActionInherentData)playerActionAsset).projectedObjectDefinitionID;
                                PlayerActionsInteractiveObjectDefinitions.Add(objectDefinitionID);
                            }
                            else if (playerActionAsset.GetType() == typeof(AttractiveObjectActionInherentData))
                            {
                                var objectDefinitionID = ((AttractiveObjectActionInherentData)playerActionAsset).AttractiveObjectDefinitionID;
                                PlayerActionsInteractiveObjectDefinitions.Add(objectDefinitionID);
                            }
                        }
                    });
                }
                SceneView.duringSceneGui += this.OnSceneGUI;
            }
        }

        public void OnDisabled()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }

        public void OnGUI()
        {
            this.OnEnable();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Scene : ");
            foreach (var SceneInteractiveObject in this.SceneInteractiveObjects)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(SceneInteractiveObject, typeof(InteractiveObjectType), false);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Player actions : ");
            foreach (var PlayerActionsInteractiveObjectDefinition in PlayerActionsInteractiveObjectDefinitions)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(this.InteractiveObjectTypeDefinitionConfiguration.ConfigurationInherentData[PlayerActionsInteractiveObjectDefinition], typeof(InteractiveObjectTypeDefinitionInherentData), false);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            this.GizmoDisplayArea.OnGUI(() =>
            {
                foreach (var gizmoDisplay in this.GizmoDisplay.Values)
                {
                    gizmoDisplay.OnGUI();
                }
            });
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (this.GizmoDisplayArea.IsEnabled)
            {
                foreach (var gizmoDisplay in this.GizmoDisplay.Values)
                {
                    gizmoDisplay.OnSceneGUI(sceneView);
                }
            }
        }
    }

    class InteractiveObjectGizmoDisplay
    {
        private InteractiveObjectType InteractiveObjectType;
        private InteractiveObjectTypeCustomEditor gizmoEditor;

        private FoldableArea GUIArea;

        public InteractiveObjectGizmoDisplay(bool isDisplayed, InteractiveObjectType InteractiveObjectType, CommonGameConfigurations CommonGameConfigurations, InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration)
        {
            this.InteractiveObjectType = InteractiveObjectType;
            this.gizmoEditor = new InteractiveObjectTypeCustomEditor(CommonGameConfigurations, InteractiveObjectTypeDefinitionConfiguration, InteractiveObjectType.InteractiveObjectTypeDefinitionID);
            this.GUIArea = new FoldableArea(true, InteractiveObjectType.name, isDisplayed);
        }

        public void OnGUI()
        {
            this.GUIArea.OnGUI(() =>
            {
                this.gizmoEditor.OnGUI(InteractiveObjectType.InteractiveObjectTypeDefinitionID);
            });
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (this.GUIArea.IsEnabled)
            {
                this.gizmoEditor.OnSceneGUI(sceneView, this.InteractiveObjectType);
            }
        }
    }
}