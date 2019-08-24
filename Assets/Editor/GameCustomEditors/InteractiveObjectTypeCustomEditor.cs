using Editor_MainGameCreationWizard;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RTPuzzle
{
    [ExecuteInEditMode]
    public class InteractiveObjectTypeCustomEditor
    {
        private CommonGameConfigurations CommonGameConfigurations;
        private PlayerManager PlayerManager;

        private InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration;

        private void OnEnable(InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID)
        {
            if (DrawDisplay == null)
            {
                DrawDisplay = new Dictionary<string, InteractiveObjectDisaplyEnableArea>();
                this.InteractiveObjectTypeDefinitionConfiguration.ConfigurationInherentData.TryGetValue(InteractiveObjectTypeDefinitionID, out InteractiveObjectTypeDefinitionInherentData interactiveObjectTypeDefinitionConfigurationInherentData);
                this.InteractiveObjectTypeDefinitionConfigurationInherentData = interactiveObjectTypeDefinitionConfigurationInherentData;
                foreach (var rangeDefinitionModule in this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules)
                {
                    this.GetDrawDisplayOrCreate(rangeDefinitionModule.Key.Name, rangeDefinitionModule.Value);
                }
            }
            if (this.PlayerManager == null) { this.PlayerManager = GameObject.FindObjectOfType<PlayerManager>(); }
        }

        public InteractiveObjectTypeCustomEditor(CommonGameConfigurations commonGameConfigurations, InteractiveObjectTypeDefinitionConfiguration interactiveObjectTypeDefinitionConfiguration, InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID)
        {
            CommonGameConfigurations = commonGameConfigurations;
            InteractiveObjectTypeDefinitionConfiguration = interactiveObjectTypeDefinitionConfiguration;
            this.OnEnable(InteractiveObjectTypeDefinitionID);
        }

        private Dictionary<string, InteractiveObjectDisaplyEnableArea> DrawDisplay;

        private InteractiveObjectTypeDefinitionInherentData InteractiveObjectTypeDefinitionConfigurationInherentData;


        public void OnGUI(InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID)
        {
            this.OnEnable(InteractiveObjectTypeDefinitionID);
            if (this.InteractiveObjectTypeDefinitionConfigurationInherentData != null)
            {
                foreach (var interacitveObjectDefinitionModule in this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules)
                {
                    this.GetDrawDisplayOrCreate(interacitveObjectDefinitionModule.Key.Name, interacitveObjectDefinitionModule.Value).OnGUI(null);
                }
            }
        }

        public void OnSceneGUI(SceneView sceneView, InteractiveObjectType InteractiveObjectType)
        {
            this.OnEnable(InteractiveObjectType.InteractiveObjectTypeDefinitionID);
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
                else if (drawDisplay.Key == typeof(ObjectRepelModuleDefinition).Name)
                {
                    var drawArea = this.GetDrawDisplayOrCreate(typeof(ObjectRepelModuleDefinition).Name);
                    if (drawArea.IsEnabled)
                    {
                        this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(ObjectRepelModuleDefinition), out ScriptableObject definitionSO);
                        if (definitionSO != null)
                        {
                            var ObjectRepelModuleDefinition = (ObjectRepelModuleDefinition)definitionSO;
                            var ObjectRepelInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.ObjectRepelConfiguration.ConfigurationInherentData[ObjectRepelModuleDefinition.ObjectRepelID];
                            Handles.color = Color.yellow;
                            var selectedProjectile = drawArea.GetEnumParameter<LaunchProjectileID>();
                            Handles.Label(InteractiveObjectType.transform.position + Vector3.up * ObjectRepelInherentData.GetRepelableObjectDistance(selectedProjectile), nameof(ObjectRepelInherentData.RepelableObjectDistance), MyEditorStyles.LabelYellow);
                            Handles.DrawWireDisc(InteractiveObjectType.transform.position, Vector3.up, ObjectRepelInherentData.GetRepelableObjectDistance(selectedProjectile));
                        }
                    }
                }
                else if (drawDisplay.Key == typeof(ObjectSightModuleDefinition).Name)
                {
                    var drawArea = this.GetDrawDisplayOrCreate(typeof(ObjectSightModuleDefinition).Name);
                    if (drawArea.IsEnabled)
                    {
                        this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(ObjectSightModuleDefinition), out ScriptableObject definitionSO);
                        if (definitionSO != null)
                        {
                            var ObjectSightModuleDefinition = (ObjectSightModuleDefinition)definitionSO;
                            var rangeObjectDefinition = this.CommonGameConfigurations.PuzzleGameConfigurations.RangeTypeObjectDefinitionConfiguration.ConfigurationInherentData[ObjectSightModuleDefinition.RangeTypeObjectDefinitionID];
                            rangeObjectDefinition.GetDefinitionModule<RangeTypeDefinition>().RangeShapeConfiguration.HandleDraw(InteractiveObjectType.transform.TransformPoint(ObjectSightModuleDefinition.LocalPosition),
                            InteractiveObjectType.transform.rotation, InteractiveObjectType.transform.lossyScale);
                        }
                    }
                }
                //${addNewEntry}
            }

            Handles.color = oldHandlesColor;
            Handles.EndGUI();
        }

        private InteractiveObjectDisaplyEnableArea GetDrawDisplayOrCreate(string key, ScriptableObject definitionSO = null)
        {
            this.DrawDisplay.TryGetValue(key, out InteractiveObjectDisaplyEnableArea EnableArea);
            if (EnableArea == null)
            {
                if (key == typeof(ObjectRepelModuleDefinition).Name && definitionSO != null)
                {
                    var ObjectRepelModuleDefinition = (ObjectRepelModuleDefinition)definitionSO;
                    var ObjectRepelInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.ObjectRepelConfiguration.ConfigurationInherentData[ObjectRepelModuleDefinition.ObjectRepelID];
                    this.DrawDisplay[key] = new InteractiveObjectDisaplyEnableArea(true, key, new List<AdditionalEnumParameter>() { new AdditionalEnumParameter(typeof(LaunchProjectileID), ObjectRepelInherentData.RepelableObjectDistance.Values.Keys.ToList().ConvertAll(e => (Enum)e)) });
                }
                else
                {
                    this.DrawDisplay[key] = new InteractiveObjectDisaplyEnableArea(true, key);
                }

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

    public class InteractiveObjectDisaplyEnableArea
    {
        private bool isEnabled;
        private string label;
        private Dictionary<Type, AdditionalEnumParameter> AdditionalEnumerationParameters;

        public InteractiveObjectDisaplyEnableArea(bool isEnabled, string label, List<AdditionalEnumParameter> AdditionalEnumerationParameters = null)
        {
            this.isEnabled = isEnabled;
            this.label = label;

            if (AdditionalEnumerationParameters != null)
            {
                this.AdditionalEnumerationParameters = new Dictionary<Type, AdditionalEnumParameter>();
                foreach (var enumParam in AdditionalEnumerationParameters)
                {
                    this.AdditionalEnumerationParameters.Add(enumParam.EnumType, enumParam);
                }
            }
        }

        public bool IsEnabled { get => isEnabled; }

        public void OnGUI(Action guiAction)
        {
            GUILayout.BeginVertical(EditorStyles.textArea);
            GUILayout.BeginHorizontal();
            this.isEnabled = EditorGUILayout.Toggle(this.isEnabled, GUILayout.Width(30f));
            EditorGUILayout.LabelField(this.label);

            if (this.AdditionalEnumerationParameters != null)
            {
                foreach (var additionalEnumerationParameter in this.AdditionalEnumerationParameters.ToList())
                {
                    this.AdditionalEnumerationParameters[additionalEnumerationParameter.Key].SelectedEnum = EditorGUILayout.EnumPopup(new GUIContent(""), this.AdditionalEnumerationParameters[additionalEnumerationParameter.Key].SelectedEnum, (value) => additionalEnumerationParameter.Value.AvailableEnums.Contains(value), false);
                }

            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (this.isEnabled && guiAction != null)
            {
                guiAction.Invoke();
            }
        }

        public T GetEnumParameter<T>() where T : Enum
        {
            return (T)this.AdditionalEnumerationParameters[typeof(T)].SelectedEnum;
        }
    }

    public class AdditionalEnumParameter
    {
        public Type EnumType;
        public List<Enum> AvailableEnums;
        public Enum SelectedEnum;

        public AdditionalEnumParameter(Type enumType, List<Enum> availableEnums)
        {
            EnumType = enumType;
            AvailableEnums = availableEnums;
            var enumerator = Enum.GetValues(enumType).GetEnumerator();
            enumerator.MoveNext();
            this.SelectedEnum = (Enum)enumerator.Current;
        }
    }
}
