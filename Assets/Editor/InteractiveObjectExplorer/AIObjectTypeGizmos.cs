using Editor_InteractiveObjectExplorer;
using Editor_MainGameCreationWizard;
using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AIObjectTypeGizmos : ObjectModulesGizmo
{
    private AIObjectTypeDefinitionConfiguration aIObjectTypeDefinitionConfiguration;
    private AIObjectTypeDefinitionID aIObjectTypeDefinitionID;
    private AIObjectTypeDefinitionInherentData AIObjectTypeDefinitionInherentData;

    private InteractiveObjectGizmoDisplay AssociatedInteractiveObjectGizmoDisplay;

    public AIObjectTypeGizmos(Transform objectTransform, CommonGameConfigurations commonGameConfigurations, AIObjectTypeDefinitionConfiguration aIObjectTypeDefinitionConfiguration, AIObjectTypeDefinitionID aIObjectTypeDefinitionID,
        InteractiveObjectTypeDefinitionConfiguration InteractiveObjectTypeDefinitionConfiguration) : base(commonGameConfigurations)
    {
        this.aIObjectTypeDefinitionConfiguration = aIObjectTypeDefinitionConfiguration;
        this.aIObjectTypeDefinitionID = aIObjectTypeDefinitionID;
        this.AIObjectTypeDefinitionInherentData = this.aIObjectTypeDefinitionConfiguration.ConfigurationInherentData[this.aIObjectTypeDefinitionID];

        this.AssociatedInteractiveObjectGizmoDisplay = new InteractiveObjectGizmoDisplay(true, objectTransform, commonGameConfigurations, this.AIObjectTypeDefinitionInherentData.InteractiveObjectTypeDefinitionID,
            InteractiveObjectTypeDefinitionConfiguration);

        this.Init();
    }

    public override void OnGUI()
    {
        base.OnGUI();
        this.AssociatedInteractiveObjectGizmoDisplay.OnGUI();
    }

    public override void OnSceneGUI(SceneView sceneView, Transform objectTransform)
    {
        base.OnSceneGUI(sceneView, objectTransform);
        this.AssociatedInteractiveObjectGizmoDisplay.OnSceneGUI(sceneView);
    }

    protected override void DrawGizmo(string moduleDefinitionType, Transform objectTransform)
    {
        if (moduleDefinitionType == typeof(AIPatrolComponent).Name)
        {
            var drawArea = this.GetDrawDisplay(typeof(AIPatrolComponent).Name);
            if (drawArea.IsEnabled)
            {
                Handles.color = Color.magenta;
                var labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = Color.magenta;
                Handles.Label(objectTransform.position + (Vector3.up * this.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIPatrolComponent>().MaxDistance), "AI Patrol distance.", labelStyle);
                Handles.DrawWireDisc(objectTransform.position, Vector3.up, this.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIPatrolComponent>().MaxDistance);
            }
        }
        else if (moduleDefinitionType == typeof(AIProjectileEscapeComponent).Name)
        {
            var drawArea = this.GetDrawDisplay(typeof(AIProjectileEscapeComponent).Name);
            if (drawArea.IsEnabled)
            {
                var AIProjectileEscapeComponent = this.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIProjectileEscapeComponent>();
                var projectileIDEscapeRange = (LaunchProjectileID)drawArea.GetEnumParameter<ProjectileEscapeRange>();
                AIProjectileEscapeComponent.EscapeDistanceV2.Values.TryGetValue(projectileIDEscapeRange, out float projectileEscapeRange);
                var projectileIDEscapeSemiAngle = (LaunchProjectileID)drawArea.GetEnumParameter<ProjectileEscapeSemiAngle>();
                AIProjectileEscapeComponent.EscapeSemiAngleV2.Values.TryGetValue(projectileIDEscapeSemiAngle, out float projectileEscapeSemiAngle);

                Handles.color = Color.blue;
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = Handles.color;
                Handles.Label(objectTransform.position + Vector3.up * projectileEscapeRange,
                    this.GetType().Name + "_" + projectileIDEscapeRange.ToString(), labelStyle);
                Handles.DrawWireDisc(objectTransform.position, Vector3.up, projectileEscapeRange);

                Handles.color = Color.yellow;
                Handles.Label(objectTransform.position + Vector3.up * 5f, nameof(AIProjectileEscapeComponent.EscapeSemiAngleV2) + "_" + projectileIDEscapeSemiAngle.ToString(), MyEditorStyles.LabelYellow);
                Handles.DrawWireArc(objectTransform.position, Vector3.up, objectTransform.forward, projectileEscapeSemiAngle, 5f);
                Handles.DrawWireArc(objectTransform.position, Vector3.up, objectTransform.forward, -projectileEscapeSemiAngle, 5f);
            }

        }
        else if (moduleDefinitionType == typeof(AIEscapeWithoutTriggerComponent).Name) { }
        else if (moduleDefinitionType == typeof(AITargetZoneComponent).Name)
        {
            var drawArea = this.GetDrawDisplay(typeof(AIPlayerEscapeComponent).Name);
            if (drawArea.IsEnabled)
            {
                var AITargetZoneComponent = this.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AITargetZoneComponent>();
                Handles.color = Color.green;
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = Handles.color;
                Handles.Label(objectTransform.position + Vector3.up * AITargetZoneComponent.TargetZoneEscapeDistance, nameof(AITargetZoneComponent.TargetZoneEscapeDistance), labelStyle);
                Handles.DrawWireDisc(objectTransform.position, Vector3.up, AITargetZoneComponent.TargetZoneEscapeDistance);
            }
        }
        else if (moduleDefinitionType == typeof(AIAttractiveObjectComponent).Name) { }
        else if (moduleDefinitionType == typeof(AIFearStunComponent).Name) { }
        else if (moduleDefinitionType == typeof(AIPlayerEscapeComponent).Name)
        {
            var drawArea = this.GetDrawDisplay(typeof(AIPlayerEscapeComponent).Name);
            if (drawArea.IsEnabled)
            {
                var AIPlayerEscapeComponent = this.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIPlayerEscapeComponent>();
                Handles.color = Color.yellow;

                var labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = Handles.color;

                Handles.Label(objectTransform.position + Vector3.up * AIPlayerEscapeComponent.EscapeDistance, nameof(AIPlayerEscapeComponent.EscapeDistance), labelStyle);
                Handles.DrawWireDisc(objectTransform.position, Vector3.up, AIPlayerEscapeComponent.EscapeDistance);

                Handles.Label(objectTransform.position + Vector3.up * AIPlayerEscapeComponent.PlayerDetectionRadius, nameof(AIPlayerEscapeComponent.PlayerDetectionRadius), labelStyle);
                Handles.DrawWireDisc(objectTransform.position, Vector3.up, AIPlayerEscapeComponent.PlayerDetectionRadius);

                Handles.Label(objectTransform.position + objectTransform.forward * 4, "Escape angle.", labelStyle);
                Handles.DrawWireArc(objectTransform.position, Vector3.up, objectTransform.forward, AIPlayerEscapeComponent.EscapeSemiAngle, 5f);
                Handles.DrawWireArc(objectTransform.position, Vector3.up, objectTransform.forward, -AIPlayerEscapeComponent.EscapeSemiAngle, 5f);
            }
        }
        else if (moduleDefinitionType == typeof(AIMoveTowardPlayerComponent).Name) { }
        else if (moduleDefinitionType == typeof(AIDisarmObjectComponent).Name) { }
    }

    protected override Dictionary<Type, ScriptableObject> GetDefinitionModules()
    {
        return this.aIObjectTypeDefinitionConfiguration.ConfigurationInherentData[this.aIObjectTypeDefinitionID].GenericPuzzleAIComponents.RangeDefinitionModules
                    .Select(m => m).Where(m => m.Value != null).ToDictionary(m => m.Key, m => m.Value);
    }

    protected override IObjectGizmoDisplayEnableArea HandleGizmoDisplayAreaCreation(string moduleDefinitionType, ScriptableObject definitionSO)
    {
        if (moduleDefinitionType == typeof(AIProjectileEscapeComponent).Name)
        {
            var AIProjectileEscapeComponent = AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.GetDefinitionModule<AIProjectileEscapeComponent>();
            return new IObjectGizmoDisplayEnableArea(true, moduleDefinitionType, new List<AdditionalEnumParameter>()
                    { new AdditionalEnumParameter(AIProjectileEscapeComponent.EscapeDistanceV2.GetType(), typeof(LaunchProjectileID), AIProjectileEscapeComponent.EscapeDistanceV2.Values.Keys.ToList().ConvertAll(e => (Enum)e)),
                      new AdditionalEnumParameter(AIProjectileEscapeComponent.EscapeSemiAngleV2.GetType(), typeof(LaunchProjectileID), AIProjectileEscapeComponent.EscapeSemiAngleV2.Values.Keys.ToList().ConvertAll(e => (Enum) e))
                     }
                    );
        }
        else
        {
            return new IObjectGizmoDisplayEnableArea(true, moduleDefinitionType);
        }
    }
}
