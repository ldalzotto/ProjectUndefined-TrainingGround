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
        private PlayerManager PlayerManager;

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
            if (this.PlayerManager == null) { this.PlayerManager = GameObject.FindObjectOfType<PlayerManager>(); }
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
                            this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(TargetZoneModuleDefinition), out ScriptableObject definitionSO);
                            if (definitionSO != null)
                            {
                                var TargetZoneModuleDefinition = (TargetZoneModuleDefinition)definitionSO;

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
                    }
                    else if (drawDisplay.Key == typeof(LevelCompletionTriggerModuleDefinition).Name)
                    {
                        var drawArea = this.GetDrawDisplayOrCreate(typeof(LevelCompletionTriggerModuleDefinition).Name);
                        if (drawArea.IsEnabled)
                        {
                            this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(LevelCompletionTriggerModuleDefinition), out ScriptableObject definitionSO);
                            if (definitionSO != null)
                            {
                                var LevelCompletionTriggerModuleDefinition = (LevelCompletionTriggerModuleDefinition)definitionSO;
                                var rangeDefinition = this.CommonGameConfigurations.PuzzleGameConfigurations.RangeTypeObjectDefinitionConfiguration.ConfigurationInherentData[LevelCompletionTriggerModuleDefinition.RangeTypeObjectDefinitionID];
                                this.DrawRangeDefinition(rangeDefinition, InteractiveObjectType.transform, Color.blue, "Test", MyEditorStyles.LabelBlue);
                            }
                        }
                    }
                    else if (drawDisplay.Key == typeof(ActionInteractableObjectModuleDefinition).Name)
                    {
                        var drawArea = this.GetDrawDisplayOrCreate(typeof(ActionInteractableObjectModuleDefinition).Name);
                        if (drawArea.IsEnabled)
                        {
                            this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(ActionInteractableObjectModuleDefinition), out ScriptableObject definitionSO);
                            if (definitionSO != null)
                            {
                                var ActionInteractableObjectModuleDefinition = (ActionInteractableObjectModuleDefinition)definitionSO;
                                var ActionInteractableInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.ActionInteractableObjectConfiguration.ConfigurationInherentData[ActionInteractableObjectModuleDefinition.ActionInteractableObjectID];
                                Handles.color = Color.magenta;
                                Handles.Label(InteractiveObjectType.transform.position + Vector3.up * ActionInteractableInherentData.InteractionRange, nameof(ActionInteractableInherentData.InteractionRange), MyEditorStyles.LabelMagenta);
                                Handles.DrawWireDisc(InteractiveObjectType.transform.position, Vector3.up, ActionInteractableInherentData.InteractionRange);
                            }
                        }
                    }
                    else if (drawDisplay.Key == typeof(NearPlayerGameOverTriggerModuleDefinition).Name)
                    {
                        var drawArea = this.GetDrawDisplayOrCreate(typeof(NearPlayerGameOverTriggerModuleDefinition).Name);
                        if (drawArea.IsEnabled)
                        {
                            this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(NearPlayerGameOverTriggerModuleDefinition), out ScriptableObject definitionSO);
                            if (definitionSO != null)
                            {
                                var NearPlayerGameOverTriggerModuleDefinition = (NearPlayerGameOverTriggerModuleDefinition)definitionSO;
                                var NearPlayerGameOverTriggerInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.NearPlayerGameOverTriggerConfiguration.ConfigurationInherentData[NearPlayerGameOverTriggerModuleDefinition.NearPlayerGameOverTriggerID];

                                Handles.color = Color.magenta;
                                var labelStyle = new GUIStyle(EditorStyles.label);
                                labelStyle.normal.textColor = Color.magenta;
                                Handles.Label(InteractiveObjectType.transform.position + (Vector3.up * NearPlayerGameOverTriggerInherentData.NearPlayerDetectionRadius), "Near player game over radius.", labelStyle);
                                Handles.DrawWireDisc(InteractiveObjectType.transform.position, Vector3.up, NearPlayerGameOverTriggerInherentData.NearPlayerDetectionRadius);
                            }
                        }
                    }
                    else if (drawDisplay.Key == typeof(LaunchProjectileModuleDefinition).Name)
                    {
                        var drawArea = this.GetDrawDisplayOrCreate(typeof(LaunchProjectileModuleDefinition).Name);
                        if (drawArea.IsEnabled)
                        {
						    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(LaunchProjectileModuleDefinition), out ScriptableObject definitionSO);
                            if (definitionSO != null)
                            {
								var LaunchProjectileModuleDefinition = (LaunchProjectileModuleDefinition)definitionSO;
                                var LaunchProjectileInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.LaunchProjectileConfiguration.ConfigurationInherentData[LaunchProjectileModuleDefinition.LaunchProjectileID];

                                Handles.color = Color.magenta;
                                var position = InteractiveObjectType.transform.position;
                                if (this.PlayerManager != null)
                                {
                                    position = this.PlayerManager.transform.position;
                                }
                                Handles.Label(position + Vector3.up * LaunchProjectileInherentData.ProjectileThrowRange, nameof(LaunchProjectileInherentData.ProjectileThrowRange), MyEditorStyles.LabelMagenta);
                                Handles.DrawWireDisc(position, Vector3.up, LaunchProjectileInherentData.ProjectileThrowRange);


                                Handles.color = Color.red;
                                Handles.Label(InteractiveObjectType.transform.position + Vector3.up * LaunchProjectileInherentData.ExplodingEffectRange, nameof(LaunchProjectileInherentData.ExplodingEffectRange), MyEditorStyles.LabelRed);
                                Handles.DrawWireDisc(InteractiveObjectType.transform.position, Vector3.up, LaunchProjectileInherentData.ExplodingEffectRange);
                            }
                        }
                    }
                    else if (drawDisplay.Key == typeof(AttractiveObjectModuleDefinition).Name)
                    {
                        var drawArea = this.GetDrawDisplayOrCreate(typeof(AttractiveObjectModuleDefinition).Name);
                        if (drawArea.IsEnabled)
                        {
						    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(AttractiveObjectModuleDefinition), out ScriptableObject definitionSO);
                            if (definitionSO != null)
                            {
								var AttractiveObjectModuleDefinition = (AttractiveObjectModuleDefinition)definitionSO;
                                var AttractiveObjectInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.AttractiveObjectConfiguration.ConfigurationInherentData[AttractiveObjectModuleDefinition.AttractiveObjectId];

                                Handles.color = Color.magenta;
                                var position = InteractiveObjectType.transform.position;
                                Handles.Label(position + Vector3.up * AttractiveObjectInherentData.EffectRange, nameof(AttractiveObjectInherentData.EffectRange), MyEditorStyles.LabelMagenta);
                                Handles.DrawWireDisc(position, Vector3.up, AttractiveObjectInherentData.EffectRange);
                            }
                        }
                    }
                    else if (drawDisplay.Key == typeof(ModelObjectModuleDefinition).Name)
                    {
                        var drawArea = this.GetDrawDisplayOrCreate(typeof(ModelObjectModuleDefinition).Name);
                        if (drawArea.IsEnabled)
                        {
						    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(ModelObjectModuleDefinition), out ScriptableObject definitionSO);
                            if (definitionSO != null)
                            {
								 var ModelObjectModuleDefinition = (ModelObjectModuleDefinition)definitionSO;
							}
                        }
                    }
                    else if (drawDisplay.Key == typeof(DisarmObjectModuleDefinition).Name)
                    {
                        var drawArea = this.GetDrawDisplayOrCreate(typeof(DisarmObjectModuleDefinition).Name);
                        if (drawArea.IsEnabled)
                        {
						    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(DisarmObjectModuleDefinition), out ScriptableObject definitionSO);
                            if (definitionSO != null)
                            {
								 var DisarmObjectModuleDefinition = (DisarmObjectModuleDefinition)definitionSO;
                                var DisarmObjectInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.DisarmObjectConfiguration.ConfigurationInherentData[DisarmObjectModuleDefinition.DisarmObjectID];

                                Handles.color = Color.magenta;
                                var position = InteractiveObjectType.transform.position;
                                Handles.Label(position + Vector3.up * DisarmObjectInherentData.DisarmInteractionRange, nameof(DisarmObjectInherentData.DisarmInteractionRange), MyEditorStyles.LabelMagenta);
                                Handles.DrawWireDisc(position, Vector3.up, DisarmObjectInherentData.DisarmInteractionRange);
                            }
                        }
                    }
                    else if (drawDisplay.Key == typeof(GrabObjectModuleDefinition).Name)
                    {
                        var drawArea = this.GetDrawDisplayOrCreate(typeof(GrabObjectModuleDefinition).Name);
                        if (drawArea.IsEnabled)
                        {
						    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(GrabObjectModuleDefinition), out ScriptableObject definitionSO);
                            if (definitionSO != null)
                            {
								 var GrabObjectModuleDefinition = (GrabObjectModuleDefinition)definitionSO;
                                var GrabObjectInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.GrabObjectConfiguration.ConfigurationInherentData[GrabObjectModuleDefinition.GrabObjectID];
                                Handles.Label(InteractiveObjectType.transform.position + Vector3.up * GrabObjectInherentData.EffectRadius, nameof(GrabObjectInherentData.EffectRadius), MyEditorStyles.LabelMagenta);
                                Handles.DrawWireDisc(InteractiveObjectType.transform.position, Vector3.up, GrabObjectInherentData.EffectRadius);
                            }
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

        private void DrawRangeDefinition(RangeTypeObjectDefinitionInherentData rangeTypeObjectDefinitionConfigurationInherentData, Transform transform, Color color, string label, GUIStyle labelStyle)
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
