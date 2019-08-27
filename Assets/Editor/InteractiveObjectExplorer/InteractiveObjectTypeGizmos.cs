using Editor_MainGameCreationWizard;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RTPuzzle
{
    public class InteractiveObjectTypeGizmos : ObjectModulesGizmo
    {
        private PlayerManager PlayerManager;

        private InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID;
        private InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration;
        private InteractiveObjectTypeDefinitionInherentData InteractiveObjectTypeDefinitionConfigurationInherentData;

        protected override IObjectGizmoDisplayEnableArea HandleGizmoDisplayAreaCreation(string moduleDefinitionType, ScriptableObject definitionSO)
        {
            if (moduleDefinitionType == typeof(ObjectRepelModuleDefinition).Name && definitionSO != null)
            {
                var ObjectRepelModuleDefinition = (ObjectRepelModuleDefinition)definitionSO;
                var ObjectRepelInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.ObjectRepelConfiguration.ConfigurationInherentData[ObjectRepelModuleDefinition.ObjectRepelID];
                return new IObjectGizmoDisplayEnableArea(false, moduleDefinitionType, new List<AdditionalEnumParameter>() { new AdditionalEnumParameter(ObjectRepelInherentData.RepelableObjectDistance.GetType(), typeof(LaunchProjectileID), ObjectRepelInherentData.RepelableObjectDistance.Values.Keys.ToList().ConvertAll(e => (Enum)e)) });
            }
            else
            {
               return new IObjectGizmoDisplayEnableArea(false, moduleDefinitionType);
            }
        }

        protected override Dictionary<Type, ScriptableObject> GetDefinitionModules()
        {
            this.InteractiveObjectTypeDefinitionConfiguration.ConfigurationInherentData.TryGetValue(InteractiveObjectTypeDefinitionID, out InteractiveObjectTypeDefinitionInherentData interactiveObjectTypeDefinitionConfigurationInherentData);
            this.InteractiveObjectTypeDefinitionConfigurationInherentData = interactiveObjectTypeDefinitionConfigurationInherentData;
            return this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules;
        }
        
        public InteractiveObjectTypeGizmos(CommonGameConfigurations commonGameConfigurations, InteractiveObjectTypeDefinitionConfiguration interactiveObjectTypeDefinitionConfiguration, InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID) 
                : base(commonGameConfigurations)
        {
            CommonGameConfigurations = commonGameConfigurations;
            InteractiveObjectTypeDefinitionConfiguration = interactiveObjectTypeDefinitionConfiguration;
            this.InteractiveObjectTypeDefinitionID = InteractiveObjectTypeDefinitionID;
            base.Init();
        }
        
        protected override void DrawGizmo(string moduleDefinitionType, Transform objectTransform)
        {
            if (moduleDefinitionType == typeof(TargetZoneModuleDefinition).Name)
            {
                var drawArea = this.GetDrawDisplay(typeof(TargetZoneModuleDefinition).Name);
                if (drawArea.IsEnabled)
                {
                    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(TargetZoneModuleDefinition), out ScriptableObject definitionSO);
                    if (definitionSO != null)
                    {
                        var TargetZoneModuleDefinition = (TargetZoneModuleDefinition)definitionSO;

                        var targetZoneInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.TargetZoneConfiguration.ConfigurationInherentData[TargetZoneModuleDefinition.TargetZoneID];
                        Handles.color = Color.red;
                        Handles.Label(objectTransform.position + Vector3.up * targetZoneInherentData.AIDistanceDetection, nameof(TargetZoneInherentData.AIDistanceDetection), MyEditorStyles.LabelRed);
                        Handles.DrawWireDisc(objectTransform.position, Vector3.up, targetZoneInherentData.AIDistanceDetection);

                        Handles.color = Color.yellow;
                        Handles.Label(objectTransform.position + Vector3.up * 5f, nameof(TargetZoneInherentData.EscapeFOVSemiAngle), MyEditorStyles.LabelYellow);
                        Handles.DrawWireArc(objectTransform.position, Vector3.up, objectTransform.forward, targetZoneInherentData.EscapeFOVSemiAngle, 5f);
                        Handles.DrawWireArc(objectTransform.position, Vector3.up, objectTransform.forward, -targetZoneInherentData.EscapeFOVSemiAngle, 5f);
                    }
                }
            }
            else if (moduleDefinitionType == typeof(LevelCompletionTriggerModuleDefinition).Name)
            {
                var drawArea = this.GetDrawDisplay(typeof(LevelCompletionTriggerModuleDefinition).Name);
                if (drawArea.IsEnabled)
                {
                    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(LevelCompletionTriggerModuleDefinition), out ScriptableObject definitionSO);
                    if (definitionSO != null)
                    {
                        var LevelCompletionTriggerModuleDefinition = (LevelCompletionTriggerModuleDefinition)definitionSO;
                        var rangeDefinition = this.CommonGameConfigurations.PuzzleGameConfigurations.RangeTypeObjectDefinitionConfiguration.ConfigurationInherentData[LevelCompletionTriggerModuleDefinition.RangeTypeObjectDefinitionID];
                        this.DrawRangeDefinition(rangeDefinition, objectTransform, Color.blue, "Test", MyEditorStyles.LabelBlue);
                    }
                }
            }
            else if (moduleDefinitionType == typeof(ActionInteractableObjectModuleDefinition).Name)
            {
                var drawArea = this.GetDrawDisplay(typeof(ActionInteractableObjectModuleDefinition).Name);
                if (drawArea.IsEnabled)
                {
                    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(ActionInteractableObjectModuleDefinition), out ScriptableObject definitionSO);
                    if (definitionSO != null)
                    {
                        var ActionInteractableObjectModuleDefinition = (ActionInteractableObjectModuleDefinition)definitionSO;
                        var ActionInteractableInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.ActionInteractableObjectConfiguration.ConfigurationInherentData[ActionInteractableObjectModuleDefinition.ActionInteractableObjectID];
                        Handles.color = Color.magenta;
                        Handles.Label(objectTransform.position + Vector3.up * ActionInteractableInherentData.InteractionRange, nameof(ActionInteractableInherentData.InteractionRange), MyEditorStyles.LabelMagenta);
                        Handles.DrawWireDisc(objectTransform.position, Vector3.up, ActionInteractableInherentData.InteractionRange);
                    }
                }
            }
            else if (moduleDefinitionType == typeof(NearPlayerGameOverTriggerModuleDefinition).Name)
            {
                var drawArea = this.GetDrawDisplay(typeof(NearPlayerGameOverTriggerModuleDefinition).Name);
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
                        Handles.Label(objectTransform.position + (Vector3.up * NearPlayerGameOverTriggerInherentData.NearPlayerDetectionRadius), "Near player game over radius.", labelStyle);
                        Handles.DrawWireDisc(objectTransform.position, Vector3.up, NearPlayerGameOverTriggerInherentData.NearPlayerDetectionRadius);
                    }
                }
            }
            else if (moduleDefinitionType == typeof(LaunchProjectileModuleDefinition).Name)
            {
                var drawArea = this.GetDrawDisplay(typeof(LaunchProjectileModuleDefinition).Name);
                if (drawArea.IsEnabled)
                {
                    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(LaunchProjectileModuleDefinition), out ScriptableObject definitionSO);
                    if (definitionSO != null)
                    {
                        var LaunchProjectileModuleDefinition = (LaunchProjectileModuleDefinition)definitionSO;
                        var LaunchProjectileInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.LaunchProjectileConfiguration.ConfigurationInherentData[LaunchProjectileModuleDefinition.LaunchProjectileID];

                        Handles.color = Color.magenta;
                        var position = objectTransform.position;
                        if (this.PlayerManager != null)
                        {
                            position = this.PlayerManager.transform.position;
                        }
                        Handles.Label(position + Vector3.up * LaunchProjectileInherentData.ProjectileThrowRange, nameof(LaunchProjectileInherentData.ProjectileThrowRange), MyEditorStyles.LabelMagenta);
                        Handles.DrawWireDisc(position, Vector3.up, LaunchProjectileInherentData.ProjectileThrowRange);


                        Handles.color = Color.red;
                        Handles.Label(objectTransform.position + Vector3.up * LaunchProjectileInherentData.ExplodingEffectRange, nameof(LaunchProjectileInherentData.ExplodingEffectRange), MyEditorStyles.LabelRed);
                        Handles.DrawWireDisc(objectTransform.position, Vector3.up, LaunchProjectileInherentData.ExplodingEffectRange);
                    }
                }
            }
            else if (moduleDefinitionType == typeof(AttractiveObjectModuleDefinition).Name)
            {
                var drawArea = this.GetDrawDisplay(typeof(AttractiveObjectModuleDefinition).Name);
                if (drawArea.IsEnabled)
                {
                    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(AttractiveObjectModuleDefinition), out ScriptableObject definitionSO);
                    if (definitionSO != null)
                    {
                        var AttractiveObjectModuleDefinition = (AttractiveObjectModuleDefinition)definitionSO;
                        var AttractiveObjectInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.AttractiveObjectConfiguration.ConfigurationInherentData[AttractiveObjectModuleDefinition.AttractiveObjectId];

                        Handles.color = Color.magenta;
                        var position = objectTransform.position;
                        Handles.Label(position + Vector3.up * AttractiveObjectInherentData.EffectRange, nameof(AttractiveObjectInherentData.EffectRange), MyEditorStyles.LabelMagenta);
                        Handles.DrawWireDisc(position, Vector3.up, AttractiveObjectInherentData.EffectRange);
                    }
                }
            }
            else if (moduleDefinitionType == typeof(ModelObjectModuleDefinition).Name)
            {
                var drawArea = this.GetDrawDisplay(typeof(ModelObjectModuleDefinition).Name);
                if (drawArea.IsEnabled)
                {
                    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(ModelObjectModuleDefinition), out ScriptableObject definitionSO);
                    if (definitionSO != null)
                    {
                        var ModelObjectModuleDefinition = (ModelObjectModuleDefinition)definitionSO;
                    }
                }
            }
            else if (moduleDefinitionType == typeof(DisarmObjectModuleDefinition).Name)
            {
                var drawArea = this.GetDrawDisplay(typeof(DisarmObjectModuleDefinition).Name);
                if (drawArea.IsEnabled)
                {
                    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(DisarmObjectModuleDefinition), out ScriptableObject definitionSO);
                    if (definitionSO != null)
                    {
                        var DisarmObjectModuleDefinition = (DisarmObjectModuleDefinition)definitionSO;
                        var DisarmObjectInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.DisarmObjectConfiguration.ConfigurationInherentData[DisarmObjectModuleDefinition.DisarmObjectID];

                        Handles.color = Color.magenta;
                        var position = objectTransform.position;
                        Handles.Label(position + Vector3.up * DisarmObjectInherentData.DisarmInteractionRange, nameof(DisarmObjectInherentData.DisarmInteractionRange), MyEditorStyles.LabelMagenta);
                        Handles.DrawWireDisc(position, Vector3.up, DisarmObjectInherentData.DisarmInteractionRange);
                    }
                }
            }
            else if (moduleDefinitionType == typeof(GrabObjectModuleDefinition).Name)
            {
                var drawArea = this.GetDrawDisplay(typeof(GrabObjectModuleDefinition).Name);
                if (drawArea.IsEnabled)
                {
                    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(GrabObjectModuleDefinition), out ScriptableObject definitionSO);
                    if (definitionSO != null)
                    {
                        var GrabObjectModuleDefinition = (GrabObjectModuleDefinition)definitionSO;
                        var GrabObjectInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.GrabObjectConfiguration.ConfigurationInherentData[GrabObjectModuleDefinition.GrabObjectID];
                        Handles.Label(objectTransform.position + Vector3.up * GrabObjectInherentData.EffectRadius, nameof(GrabObjectInherentData.EffectRadius), MyEditorStyles.LabelMagenta);
                        Handles.DrawWireDisc(objectTransform.position, Vector3.up, GrabObjectInherentData.EffectRadius);
                    }
                }
            }
            else if (moduleDefinitionType == typeof(ObjectRepelModuleDefinition).Name)
            {
                var drawArea = this.GetDrawDisplay(typeof(ObjectRepelModuleDefinition).Name);
                if (drawArea.IsEnabled)
                {
                    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(ObjectRepelModuleDefinition), out ScriptableObject definitionSO);
                    if (definitionSO != null)
                    {
                        var ObjectRepelModuleDefinition = (ObjectRepelModuleDefinition)definitionSO;
                        var ObjectRepelInherentData = this.CommonGameConfigurations.PuzzleGameConfigurations.ObjectRepelConfiguration.ConfigurationInherentData[ObjectRepelModuleDefinition.ObjectRepelID];
                        Handles.color = Color.yellow;
                        var selectedProjectile = (LaunchProjectileID)drawArea.GetEnumParameter<RepelableObjectDistance>();
                        Handles.Label(objectTransform.position + Vector3.up * ObjectRepelInherentData.GetRepelableObjectDistance(selectedProjectile), nameof(ObjectRepelInherentData.RepelableObjectDistance), MyEditorStyles.LabelYellow);
                        Handles.DrawWireDisc(objectTransform.position, Vector3.up, ObjectRepelInherentData.GetRepelableObjectDistance(selectedProjectile));
                    }
                }
            }
            else if (moduleDefinitionType == typeof(ObjectSightModuleDefinition).Name)
            {
                var drawArea = this.GetDrawDisplay(typeof(ObjectSightModuleDefinition).Name);
                if (drawArea.IsEnabled)
                {
                    this.InteractiveObjectTypeDefinitionConfigurationInherentData.RangeDefinitionModules.TryGetValue(typeof(ObjectSightModuleDefinition), out ScriptableObject definitionSO);
                    if (definitionSO != null)
                    {
                        var ObjectSightModuleDefinition = (ObjectSightModuleDefinition)definitionSO;
                        var rangeObjectDefinition = this.CommonGameConfigurations.PuzzleGameConfigurations.RangeTypeObjectDefinitionConfiguration.ConfigurationInherentData[ObjectSightModuleDefinition.RangeTypeObjectDefinitionID];
                        rangeObjectDefinition.GetDefinitionModule<RangeTypeDefinition>().RangeShapeConfiguration.HandleDraw(objectTransform.TransformPoint(ObjectSightModuleDefinition.LocalPosition),
                        objectTransform.rotation, objectTransform.lossyScale);
                    }
                }
            }
//${addNewEntry}
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

}
