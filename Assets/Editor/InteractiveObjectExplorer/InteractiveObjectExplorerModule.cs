using CoreGame;
using Editor_MainGameCreationWizard;
using GameConfigurationID;
using RTPuzzle;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor_InteractiveObjectExplorer
{
    [System.Serializable]
    public class InteractiveObjectExplorerModule : EditorWindow
    {
        [MenuItem("GameDesigner/InteractiveObjectExplorerModule")]
        public static void ShowWindow()
        {
            var window = EditorWindow.GetWindow(typeof(InteractiveObjectExplorerModule));
            window.Show();
        }

        private CommonGameConfigurations CommonGameConfigurations;
        private InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration;
        private AIObjectTypeDefinitionConfiguration AIObjectTypeDefinitionConfiguration;

        private List<InteractiveObjectType> SceneInteractiveObjects;
        private List<InteractiveObjectTypeDefinitionID> PlayerActionsInteractiveObjectDefinitions;

        private List<AIObjectType> SceneAIObjectsType;


        private FoldableArea GizmoDisplayArea;
        private List<InteractiveObjectGizmoDisplay> GizmoDisplay;
        private List<AIObjectGizmoDisplay> AIGizmoDisplay;

        private RegexTextFinder RegexTextFinder;
        private Vector2 ScrollPosition;

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
            if (this.GizmoDisplay == null) { this.GizmoDisplay = new List<InteractiveObjectGizmoDisplay>(); }
            if (this.AIGizmoDisplay == null) { this.AIGizmoDisplay = new List<AIObjectGizmoDisplay>(); }
            if (this.SceneInteractiveObjects == null)
            {
                this.SceneInteractiveObjects = GameObject.FindObjectsOfType<InteractiveObjectType>().ToList();
                this.SceneInteractiveObjects.ForEach(interactiveObject => this.GizmoDisplay.Add(new InteractiveObjectGizmoDisplay(true, interactiveObject.transform, this.CommonGameConfigurations, interactiveObject.InteractiveObjectTypeDefinitionID, this.InteractiveObjectTypeDefinitionConfiguration)));
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
                this.SceneAIObjectsType.ForEach(sceneAIObjectType => this.AIGizmoDisplay.Add(new AIObjectGizmoDisplay(true, sceneAIObjectType, this.AIObjectTypeDefinitionConfiguration, this.CommonGameConfigurations, InteractiveObjectTypeDefinitionConfiguration)));
            }
            if (this.RegexTextFinder == null) { this.RegexTextFinder = new RegexTextFinder(); }
            this.ClearNullElements();
        }

        private void ClearNullElements()
        {
            this.GizmoDisplay.Select(g => g).Where(g => g.IsNull()).ToList().ForEach(g => this.GizmoDisplay.Remove(g));
            this.AIGizmoDisplay.Select(g => g).Where(g => g.IsNull()).ToList().ForEach(g => this.AIGizmoDisplay.Remove(g));
            this.SceneAIObjectsType.Select(o => o).Where(o => o == null).ToList().ForEach(o => this.SceneAIObjectsType.Remove(o));
        }

        public void OnDisabled()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }

        private void Reset()
        {
            this.OnDisabled();
            this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList().ForEach(f => f.SetValue(this, null));
        }

        public void OnGUI()
        {
            this.ScrollPosition = EditorGUILayout.BeginScrollView(this.ScrollPosition, false, false);
            this.OnEnable();
            EditorGUILayout.BeginVertical();
            this.RegexTextFinder.GUITick();
            EditorGUILayout.LabelField("Scene Interactive : ");
            foreach (var SceneInteractiveObject in this.SceneInteractiveObjects.Select(o => o).Where(o => this.RegexTextFinder.IsMatchingWith(o.InteractiveObjectTypeDefinitionID.ToString())))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(SceneInteractiveObject, typeof(InteractiveObjectType), false);
                EditorGUILayout.ObjectField(this.InteractiveObjectTypeDefinitionConfiguration.ConfigurationInherentData[SceneInteractiveObject.InteractiveObjectTypeDefinitionID], typeof(InteractiveObjectTypeDefinitionInherentData), false);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Player actions Interactive : ");
            foreach (var PlayerActionsInteractiveObjectDefinition in PlayerActionsInteractiveObjectDefinitions.Select(o => o).Where(o => this.RegexTextFinder.IsMatchingWith(o.ToString())))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(this.InteractiveObjectTypeDefinitionConfiguration.ConfigurationInherentData[PlayerActionsInteractiveObjectDefinition], typeof(InteractiveObjectTypeDefinitionInherentData), false);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Scene AI : ");
            foreach (var SceneAIObjectType in SceneAIObjectsType.Select(o => o).Where(o => this.RegexTextFinder.IsMatchingWith(o.AIObjectTypeDefinitionID.ToString())))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(SceneAIObjectType, typeof(AIObjectType), false);
                EditorGUILayout.ObjectField(this.AIObjectTypeDefinitionConfiguration.ConfigurationInherentData[SceneAIObjectType.AIObjectTypeDefinitionID], typeof(AIObjectTypeDefinitionInherentData), false);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            this.GizmoDisplayArea.OnGUI(() =>
            {
                foreach (var gizmoDisplay in this.GizmoDisplay.Select(o => o).Where(o => this.RegexTextFinder.IsMatchingWith(o.InteractiveObjectTypeDefinitionID.ToString())))
                {
                    gizmoDisplay.OnGUI();
                }
                foreach (var gizmoDisplay in this.AIGizmoDisplay.Select(o => o).Where(o => this.RegexTextFinder.IsMatchingWith(o.GetAIObjectTypeDefinitionID().ToString())))
                {
                    gizmoDisplay.OnGUI();
                }
            });

            EditorGUILayout.Separator();
            if (GUILayout.Button("RESET"))
            {
                this.Reset();
            }

            EditorGUILayout.EndScrollView();
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            if (this.GizmoDisplayArea.IsEnabled)
            {
                foreach (var gizmoDisplay in this.GizmoDisplay)
                {
                    gizmoDisplay.OnSceneGUI(sceneView);
                }
                foreach (var gizmoDisplay in this.AIGizmoDisplay)
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
        private InteractiveObjectTypeDefinitionID interactiveObjectTypeDefinitionID;
        private FoldableArea GUIArea;

        public InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID { get => interactiveObjectTypeDefinitionID; }

        public InteractiveObjectGizmoDisplay(bool isDisplayed, Transform ObjectTransform, CommonGameConfigurations CommonGameConfigurations,
                InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID, InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration)
        {
            this.ObjectTransform = ObjectTransform;
            this.interactiveObjectTypeDefinitionID = InteractiveObjectTypeDefinitionID;
            this.gizmoEditor = new InteractiveObjectTypeGizmos(CommonGameConfigurations, InteractiveObjectTypeDefinitionConfiguration, InteractiveObjectTypeDefinitionID);
            this.GUIArea = new FoldableArea(true, interactiveObjectTypeDefinitionID.ToString(), isDisplayed);
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

        public bool IsNull() { return this.ObjectTransform == null; }
    }

    class AIObjectGizmoDisplay
    {
        private AIObjectType aIObjectType;
        private AIObjectTypeGizmos gizmoEditor;

        private FoldableArea GUIArea;


        public AIObjectGizmoDisplay(bool isDisplayed, AIObjectType AIObjectType, AIObjectTypeDefinitionConfiguration AIObjectTypeDefinitionConfiguration, CommonGameConfigurations CommonGameConfigurations,
            InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration)
        {
            this.aIObjectType = AIObjectType;
            this.gizmoEditor = new AIObjectTypeGizmos(AIObjectType.transform, CommonGameConfigurations, AIObjectTypeDefinitionConfiguration, AIObjectType.AIObjectTypeDefinitionID,
                        InteractiveObjectTypeDefinitionConfiguration);
            this.GUIArea = new FoldableArea(true, AIObjectType.AIObjectTypeDefinitionID.ToString(), isDisplayed);
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
                this.gizmoEditor.OnSceneGUI(sceneView, this.aIObjectType.transform);
            }
        }

        public bool IsNull() { return this.aIObjectType == null; }
        public AIObjectTypeDefinitionID GetAIObjectTypeDefinitionID() { return this.aIObjectType.AIObjectTypeDefinitionID; }
    }
}