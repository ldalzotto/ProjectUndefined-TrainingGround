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

    protected override void DrawGizmo(Type moduleDefinitionType, Transform objectTransform)
    {
        var drawArea = this.GetDrawDisplay(moduleDefinitionType);
        if (drawArea.IsEnabled)
        {
            this.PointOfInterestDefinitionInherentData.RangeDefinitionModules.TryGetValue(moduleDefinitionType, out ScriptableObject definitionSO);
            if (definitionSO != null)
            {
                SceneHandlerDrawer.Draw(this.PointOfInterestDefinitionConfiguration, objectTransform, this.CommonGameConfigurations, drawArea);
            }
        }
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
