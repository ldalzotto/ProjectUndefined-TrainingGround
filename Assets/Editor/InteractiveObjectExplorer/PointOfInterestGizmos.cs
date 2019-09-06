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
        if (moduleDefinitionType == typeof(PointOfInterestSharedDataTypeInherentData).Name)
        {
            var drawArea = this.GetDrawDisplay(typeof(PointOfInterestSharedDataTypeInherentData).Name);
            if (drawArea.IsEnabled)
            {
                var PointOfInterestSharedDataTypeInherentData = this.PointOfInterestDefinitionInherentData.PointOfInterestSharedDataTypeInherentData;
                Handles.color = Color.yellow;
                Handles.Label(objectTransform.position + Vector3.up * 10, nameof(PointOfInterestSharedDataTypeInherentData.POIDetectionAngleLimit), MyEditorStyles.LabelYellow);
                Handles.DrawWireArc(objectTransform.position, objectTransform.up, objectTransform.forward, PointOfInterestSharedDataTypeInherentData.POIDetectionAngleLimit, 10);
                Handles.DrawWireArc(objectTransform.position, objectTransform.up, objectTransform.forward, -PointOfInterestSharedDataTypeInherentData.POIDetectionAngleLimit, 10);
            }
        }
        else if (moduleDefinitionType == typeof(PointOfInterestCutsceneControllerModuleDefinition).Name) { }
        else if (moduleDefinitionType == typeof(PointOfInterestTrackerModuleDefinition).Name)
        {
            var drawArea = this.GetDrawDisplay(typeof(PointOfInterestTrackerModuleDefinition).Name);
            if (drawArea.IsEnabled)
            {
                var PointOfInterestTrackerModuleDefinition = this.PointOfInterestDefinitionInherentData.GetDefinitionModule<PointOfInterestTrackerModuleDefinition>();
                Handles.color = Color.green;
                Handles.Label(objectTransform.position + Vector3.up * PointOfInterestTrackerModuleDefinition.SphereDetectionRadius, "POI Tracker range", MyEditorStyles.LabelGreen);
                Handles.DrawWireDisc(objectTransform.position, objectTransform.up, PointOfInterestTrackerModuleDefinition.SphereDetectionRadius);
            }
        }
        else if (moduleDefinitionType == typeof(PointOfInterestVisualMovementModuleDefinition).Name) { }
        else if (moduleDefinitionType == typeof(PointOfInterestModelObjectModuleDefinition).Name) { }
    }

    protected override Dictionary<Type, ScriptableObject> GetDefinitionModules()
    {
        var pointOfInterestDefinitionInherentData = this.PointOfInterestDefinitionConfiguration.ConfigurationInherentData[this.PointOfInterestDefinitionID];
        KeyValuePair<Type, ScriptableObject> PointOfInterestSharedDataTypeInherentData = new KeyValuePair<Type, ScriptableObject>(
               pointOfInterestDefinitionInherentData.PointOfInterestSharedDataTypeInherentData.GetType(), pointOfInterestDefinitionInherentData.PointOfInterestSharedDataTypeInherentData);
        return pointOfInterestDefinitionInherentData.RangeDefinitionModules
                    .Select(m => m).Where(m => m.Value != null)
                    .Union(new List<KeyValuePair<Type, ScriptableObject>>() { PointOfInterestSharedDataTypeInherentData })
                    .ToDictionary(m => m.Key, m => m.Value);
    }

    protected override IObjectGizmoDisplayEnableArea HandleGizmoDisplayAreaCreation(string moduleDefinitionType, ScriptableObject definitionSO)
    {
        return new IObjectGizmoDisplayEnableArea(true, moduleDefinitionType);
    }
}
