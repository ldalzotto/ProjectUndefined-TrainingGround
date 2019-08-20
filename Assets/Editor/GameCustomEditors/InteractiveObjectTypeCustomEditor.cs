using Editor_MainGameCreationWizard;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RTPuzzle
{
    [ExecuteInEditMode]
    public class InteractiveObjectTypeCustomEditor : EditorWindow
    {
        private CommonGameConfigurations CommonGameConfigurations;

        [MenuItem("Test/InteractiveObjectTypeCustomEditor")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            InteractiveObjectTypeCustomEditor window = (InteractiveObjectTypeCustomEditor)EditorWindow.GetWindow(typeof(InteractiveObjectTypeCustomEditor));
            window.Show();
        }

        private InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration;

        private void OnEnable()
        {
            if (InteractiveObjectTypeDefinitionConfiguration == null) { this.InteractiveObjectTypeDefinitionConfiguration = AssetFinder.SafeSingleAssetFind<InteractiveObjectTypeDefinitionConfiguration>("t:" + typeof(InteractiveObjectTypeDefinitionConfiguration)); }
            if (DrawDisplay == null) { DrawDisplay = new Dictionary<string, EnableArea>(); }
            if (this.CommonGameConfigurations == null) { this.CommonGameConfigurations = new CommonGameConfigurations(); EditorInformationsHelper.InitProperties(ref this.CommonGameConfigurations); }
            SceneView.duringSceneGui += this.OnSceneGUI;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }

        private Dictionary<string, EnableArea> DrawDisplay;

        private InteractiveObjectType InteractiveObjectType;
        private InteractiveObjectTypeDefinitionInherentData InteractiveObjectTypeDefinitionConfigurationInherentData;

        private void OnGUI()
        {
            if (Selection.activeGameObject != null)
            {
                this.InteractiveObjectType = Selection.activeGameObject.GetComponent<InteractiveObjectType>();

                if (InteractiveObjectType != null && InteractiveObjectType.InteractiveObjectTypeDefinitionID != InteractiveObjectTypeDefinitionID.NONE)
                {
                    this.InteractiveObjectTypeDefinitionConfiguration.ConfigurationInherentData.TryGetValue(InteractiveObjectType.InteractiveObjectTypeDefinitionID, out InteractiveObjectTypeDefinitionInherentData interactiveObjectTypeDefinitionConfigurationInherentData);
                    this.InteractiveObjectTypeDefinitionConfigurationInherentData = interactiveObjectTypeDefinitionConfigurationInherentData;
                    if (interactiveObjectTypeDefinitionConfigurationInherentData != null)
                    {
                        foreach (var interacitveObjectDefinitionModule in interactiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules)
                        {
                            this.GetDrawDisplayOrCreate(interacitveObjectDefinitionModule.Key.Name).OnGUI(null);
                        }
                    }
                }
            }

        }


        private void OnSceneGUI(SceneView sceneView)
        {
            if (InteractiveObjectType != null)
            {
                Handles.BeginGUI();
                var oldHandlesColor = Handles.color;

                foreach (var drawDisplay in this.DrawDisplay)
                {
                    if (drawDisplay.Key == typeof(TargetZoneModuleDefinition).Name)
                    {
                        var drawArea = this.GetDrawDisplayOrCreate(typeof(TargetZoneModuleDefinition).Name);
                        if (drawArea.IsEnabled)
                        {
                            var TargetZoneModuleDefinition = (TargetZoneModuleDefinition)this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules[typeof(TargetZoneModuleDefinition)];

                            var targetZoneInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.TargetZoneConfiguration.ConfigurationInherentData[TargetZoneModuleDefinition.TargetZoneID];
                            Handles.color = Color.red;
                            Handles.Label(InteractiveObjectType.transform.position + Vector3.up * targetZoneInherentData.AIDistanceDetection, nameof(TargetZoneInherentData.AIDistanceDetection), MyEditorStyles.LabelRed);
                            Handles.DrawWireDisc(InteractiveObjectType.transform.position, Vector3.up, targetZoneInherentData.AIDistanceDetection);

                            Handles.color = Color.yellow;
                            Handles.Label(InteractiveObjectType.transform.position + Vector3.up * 5f, nameof(TargetZoneInherentData.EscapeFOVSemiAngle), MyEditorStyles.LabelYellow);
                            Handles.DrawWireArc(InteractiveObjectType.transform.position, Vector3.up, InteractiveObjectType.transform.forward, targetZoneInherentData.EscapeFOVSemiAngle, 5f);
                            Handles.DrawWireArc(InteractiveObjectType.transform.position, Vector3.up, InteractiveObjectType.transform.forward, -targetZoneInherentData.EscapeFOVSemiAngle, 5f);
                        }
                    }
                    else if (drawDisplay.Key == typeof(LevelCompletionTriggerModuleDefinition).Name)
                    {
                        var drawArea = this.GetDrawDisplayOrCreate(typeof(LevelCompletionTriggerModuleDefinition).Name);
                        if (drawArea.IsEnabled)
                        {
                            var LevelCompletionTriggerModuleDefinition = (LevelCompletionTriggerModuleDefinition)this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules[typeof(LevelCompletionTriggerModuleDefinition)];
                            var rangeDefinition = this.CommonGameConfigurations.PuzzleGameConfigurations.RangeTypeObjectDefinitionConfiguration.ConfigurationInherentData[LevelCompletionTriggerModuleDefinition.RangeTypeObjectDefinitionID];
                            this.DrawRangeDefinition(rangeDefinition, InteractiveObjectType.transform, Color.blue, "Test", MyEditorStyles.LabelBlue);
                        }
                    }
                    else if (drawDisplay.Key == typeof(ActionInteractableObjectModuleDefinition).Name)
                    {
                        var drawArea = this.GetDrawDisplayOrCreate(typeof(ActionInteractableObjectModuleDefinition).Name);
                        if (drawArea.IsEnabled)
                        {
                            var ActionInteractableObjectModuleDefinition = (ActionInteractableObjectModuleDefinition)this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules[typeof(ActionInteractableObjectModuleDefinition)];
                            var ActionInteractableInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.ActionInteractableObjectConfiguration.ConfigurationInherentData[ActionInteractableObjectModuleDefinition.ActionInteractableObjectID];
                            Handles.color = Color.magenta;
                            Handles.Label(InteractiveObjectType.transform.position + Vector3.up * ActionInteractableInherentData.InteractionRange, nameof(ActionInteractableInherentData.InteractionRange), MyEditorStyles.LabelMagenta);
                            Handles.DrawWireDisc(InteractiveObjectType.transform.position, Vector3.up, ActionInteractableInherentData.InteractionRange);
                        }
                    }
//${addNewEntry}
                }

                Handles.color = oldHandlesColor;
                Handles.EndGUI();
            }
        }

        private EnableArea GetDrawDisplayOrCreate(string key)
        {
            this.DrawDisplay.TryGetValue(key, out EnableArea EnableArea);
            if (EnableArea == null)
            {
                this.DrawDisplay[key] = new EnableArea(true, key);
                return this.DrawDisplay[key];
            }
            return EnableArea;
        }

        private void DrawRangeDefinition(RangeTypeObjectDefinitionConfigurationInherentData rangeTypeObjectDefinitionConfigurationInherentData, Transform transform, Color color, string label, GUIStyle labelStyle)
        {
            foreach (var rangeTypeDefinitionModule in rangeTypeObjectDefinitionConfigurationInherentData.RangeDefinitionModules)
            {
                if (rangeTypeDefinitionModule.Key == typeof(RangeTypeDefinition))
                {
                    var rangeTypeDefinition = ((RangeTypeDefinition)rangeTypeDefinitionModule.Value);
                    if (rangeTypeDefinition.RangeShapeConfiguration.GetType() == typeof(BoxRangeShapeConfiguration))
                    {
                        HandlesHelper.DrawBox(((BoxRangeShapeConfiguration)rangeTypeDefinition.RangeShapeConfiguration).Center,
                            ((BoxRangeShapeConfiguration)rangeTypeDefinition.RangeShapeConfiguration).Size, transform, color, label, labelStyle);
                    }
                }
            }
        }
    }

    public class EnableArea
    {
        private bool isEnabled;
        private string label;

        public EnableArea(bool isEnabled, string label)
        {
            this.isEnabled = isEnabled;
            this.label = label;
        }

        public bool IsEnabled { get => isEnabled; }

        public void OnGUI(Action guiAction)
        {
            GUILayout.BeginVertical(EditorStyles.textArea);
            GUILayout.BeginHorizontal();
            this.isEnabled = EditorGUILayout.Toggle(this.isEnabled, GUILayout.Width(30f));
            EditorGUILayout.LabelField(this.label);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (this.isEnabled && guiAction != null)
            {
                guiAction.Invoke();
            }
        }
    }
}
