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
    public class InteractiveObjectExplorerModule
    {
        private CommonGameConfigurations CommonGameConfigurations;
        private InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration;
        private AIObjectTypeDefinitionConfiguration AIObjectTypeDefinitionConfiguration;

        private List<InteractiveObjectType> SceneInteractiveObjects;
        private List<InteractiveObjectTypeDefinitionID> PlayerActionsInteractiveObjectDefinitions;

        private List<AIObjectType> SceneAIObjectsType;


        private FoldableArea GizmoDisplayArea;
        private Dictionary<InteractiveObjectTypeDefinitionID, InteractiveObjectGizmoDisplay> GizmoDisplay;
        private Dictionary<AIObjectTypeDefinitionID, AIObjectGizmoDisplay> AIGizmoDisplay;


        public void OnEnable()
        {
            if (this.CommonGameConfigurations == null)
            {
                SceneView.duringSceneGui += this.OnSceneGUI;
                this.CommonGameConfigurations = new CommonGameConfigurations(); EditorInformationsHelper.InitProperties(ref this.CommonGameConfigurations);
            }
            if (this.InteractiveObjectTypeDefinitionConfiguration == null) { this.InteractiveObjectTypeDefinitionConfiguration = AssetFinder.SafeSingleAssetFind<InteractiveObjectTypeDefinitionConfiguration>("t:" + typeof(InteractiveObjectTypeDefinitionConfiguration).Name); }
            if (this.AIObjectTypeDefinitionConfiguration == null) { this.AIObjectTypeDefinitionConfiguration = AssetFinder.SafeSingleAssetFind<AIObjectTypeDefinitionConfiguration>("t:" + typeof(AIObjectTypeDefinitionConfiguration).Name); }
            if (this.GizmoDisplayArea == null) { this.GizmoDisplayArea = new FoldableArea(true, "Gizmos : ", false); }
            if (this.GizmoDisplay == null) { this.GizmoDisplay = new Dictionary<InteractiveObjectTypeDefinitionID, InteractiveObjectGizmoDisplay>(); }
            if (this.AIGizmoDisplay == null) { this.AIGizmoDisplay = new Dictionary<AIObjectTypeDefinitionID, AIObjectGizmoDisplay>(); }
            if (this.SceneInteractiveObjects == null)
            {
                this.SceneInteractiveObjects = GameObject.FindObjectsOfType<InteractiveObjectType>().ToList();
                this.SceneInteractiveObjects.ForEach(interactiveObject => this.GizmoDisplay[interactiveObject.InteractiveObjectTypeDefinitionID] = new InteractiveObjectGizmoDisplay(true, interactiveObject.transform, this.CommonGameConfigurations, interactiveObject.InteractiveObjectTypeDefinitionID, this.InteractiveObjectTypeDefinitionConfiguration));
            }
            if (this.PlayerActionsInteractiveObjectDefinitions == null)
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
            }
            if (this.SceneAIObjectsType == null)
            {
                this.SceneAIObjectsType = GameObject.FindObjectsOfType<AIObjectType>().ToList();
                this.SceneAIObjectsType.ForEach(sceneAIObjectType => this.AIGizmoDisplay[sceneAIObjectType.AIObjectTypeDefinitionID] = new AIObjectGizmoDisplay(true, sceneAIObjectType, this.AIObjectTypeDefinitionConfiguration, this.CommonGameConfigurations, InteractiveObjectTypeDefinitionConfiguration));
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
            EditorGUILayout.LabelField("Scene Interactive : ");
            foreach (var SceneInteractiveObject in this.SceneInteractiveObjects)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(SceneInteractiveObject, typeof(InteractiveObjectType), false);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Player actions Interactive : ");
            foreach (var PlayerActionsInteractiveObjectDefinition in PlayerActionsInteractiveObjectDefinitions)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(this.InteractiveObjectTypeDefinitionConfiguration.ConfigurationInherentData[PlayerActionsInteractiveObjectDefinition], typeof(InteractiveObjectTypeDefinitionInherentData), false);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Scene AI : ");
            foreach (var SceneAIObjectType in SceneAIObjectsType)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(this.AIObjectTypeDefinitionConfiguration.ConfigurationInherentData[SceneAIObjectType.AIObjectTypeDefinitionID], typeof(AIObjectTypeDefinitionInherentData), false);
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
                foreach (var gizmoDisplay in this.AIGizmoDisplay.Values)
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
                foreach (var gizmoDisplay in this.AIGizmoDisplay.Values)
                {
                    gizmoDisplay.OnSceneGUI(sceneView);
                }
            }
        }
    }

    class InteractiveObjectGizmoDisplay
    {
        private Transform ObjectTransform;
        private InteractiveObjectTypeGizmos gizmoEditor;

        private FoldableArea GUIArea;

        public InteractiveObjectGizmoDisplay(bool isDisplayed, Transform ObjectTransform, CommonGameConfigurations CommonGameConfigurations,
                InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID, InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration)
        {
            this.ObjectTransform = ObjectTransform;
            this.gizmoEditor = new InteractiveObjectTypeGizmos(CommonGameConfigurations, InteractiveObjectTypeDefinitionConfiguration, InteractiveObjectTypeDefinitionID);
            this.GUIArea = new FoldableArea(true, ObjectTransform.gameObject.name, isDisplayed);
        }

        public void OnGUI()
        {
            this.GUIArea.OnGUI(() =>
            {
                this.gizmoEditor.OnGUI();
            });
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (this.GUIArea.IsEnabled)
            {
                this.gizmoEditor.OnSceneGUI(sceneView, this.ObjectTransform);
            }
        }
    }

    class AIObjectGizmoDisplay
    {
        private AIObjectType AIObjectType;
        private AIObjectTypeGizmos gizmoEditor;

        private FoldableArea GUIArea;

        public AIObjectGizmoDisplay(bool isDisplayed, AIObjectType AIObjectType, AIObjectTypeDefinitionConfiguration AIObjectTypeDefinitionConfiguration, CommonGameConfigurations CommonGameConfigurations,
            InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration)
        {
            this.AIObjectType = AIObjectType;
            this.gizmoEditor = new AIObjectTypeGizmos(AIObjectType.transform, CommonGameConfigurations, AIObjectTypeDefinitionConfiguration, AIObjectType.AIObjectTypeDefinitionID,
                        InteractiveObjectTypeDefinitionConfiguration);
            this.GUIArea = new FoldableArea(true, AIObjectType.name, isDisplayed);
        }

        public void OnGUI()
        {
            this.GUIArea.OnGUI(() =>
            {
                this.gizmoEditor.OnGUI();
            });
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (this.GUIArea.IsEnabled)
            {
                this.gizmoEditor.OnSceneGUI(sceneView, this.AIObjectType.transform);
            }
        }
    }
}