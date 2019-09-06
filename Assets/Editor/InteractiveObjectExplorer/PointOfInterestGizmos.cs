using AdventureGame;
using Editor_MainGameCreationWizard;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PointOfInterestGizmos : ObjectModulesGizmo
{
    private PointOfInterestDefinitionConfiguration PointOfInterestDefinitionConfiguration;
    private PointOfInterestDefinitionID PointOfInterestDefinitionID;
    private PointOfInterestDefinitionInherentData PointOfInterestDefinitionInherentData;

    public PointOfInterestGizmos(PointOfInterestDefinitionConfiguration PointOfInterestDefinitionConfiguration,
        PointOfInterestDefinitionID PointOfInterestDefinitionID, CommonGameConfigurations commonGameConfigurations) : base(commonGameConfigurations)
    {
        this.PointOfInterestDefinitionConfiguration = PointOfInterestDefinitionConfiguration;
        this.PointOfInterestDefinitionID = PointOfInterestDefinitionID;
        this.PointOfInterestDefinitionInherentData = this.PointOfInterestDefinitionConfiguration.ConfigurationInherentData[this.PointOfInterestDefinitionID];

        base.Init();
    }

    protected override void DrawGizmo(string moduleDefinitionType, Transform objectTransform)
    {
        if (moduleDefinitionType == typeof(PointOfInterestCutsceneControllerModuleDefinition).Name) { }
        else if (moduleDefinitionType == typeof(PointOfInterestTrackerModuleDefinition).Name)
        {
            var drawArea = this.GetDrawDisplay(typeof(PointOfInterestTrackerModuleDefinition).Name);
            if (drawArea.IsEnabled)
            {
                Handles.color = Color.green;
                Handles.DrawWireDisc(objectTransform.position, objectTransform.up, this.PointOfInterestDefinitionInherentData.GetDefinitionModule<PointOfInterestTrackerModuleDefinition>().SphereDetectionRadius);
            }
        }
        else if (moduleDefinitionType == typeof(PointOfInterestVisualMovementModuleDefinition).Name) { }
        else if (moduleDefinitionType == typeof(PointOfInterestModelObjectModuleDefinition).Name) { }
    }

    protected override Dictionary<Type, ScriptableObject> GetDefinitionModules()
    {
        return this.PointOfInterestDefinitionConfiguration.ConfigurationInherentData[this.PointOfInterestDefinitionID].RangeDefinitionModules
                    .Select(m => m).Where(m => m.Value != null).ToDictionary(m => m.Key, m => m.Value);
    }

    protected override IObjectGizmoDisplayEnableArea HandleGizmoDisplayAreaCreation(string moduleDefinitionType, ScriptableObject definitionSO)
    {
        return new IObjectGizmoDisplayEnableArea(true, moduleDefinitionType);
    }
}
