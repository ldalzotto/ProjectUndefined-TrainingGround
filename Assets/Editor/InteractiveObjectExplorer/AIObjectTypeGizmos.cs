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

    protected override List<AbstractObjectGizmoDisplay> GetAdditionalDrawAreas()
    {
        return new List<AbstractObjectGizmoDisplay>() { this.AssociatedInteractiveObjectGizmoDisplay };
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

    protected override void DrawGizmo(Type moduleDefinitionType, Transform objectTransform)
    {
        var drawArea = this.GetDrawDisplay(moduleDefinitionType);
        if (drawArea.IsEnabled)
        {
            this.AIObjectTypeDefinitionInherentData.GenericPuzzleAIComponents.RangeDefinitionModules.TryGetValue(moduleDefinitionType, out ScriptableObject definitionSO);
            if (definitionSO != null)
            {
                SceneHandlerDrawer.Draw(definitionSO, objectTransform, this.CommonGameConfigurations, drawArea);
            }
        }
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
