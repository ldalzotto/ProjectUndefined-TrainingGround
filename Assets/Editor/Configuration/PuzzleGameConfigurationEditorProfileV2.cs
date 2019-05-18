using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using UnityEditor;
using System.Collections.Generic;
using ConfigurationEditor;
using RTPuzzle;
using System;

[System.Serializable]
[CreateAssetMenu(fileName = "PuzzleGameConfigurationEditorProfileV2", menuName = "Configuration/PuzzleGameConfigurationEditorProfileV2", order = 1)]
public class PuzzleGameConfigurationEditorProfileV2 : TreeChoiceHeaderTab<IGenericConfigurationEditor>
{
    public override Dictionary<string, IGenericConfigurationEditor> Configurations => this.myConf;

    public static Dictionary<string, IGenericConfigurationEditor> ConfigurationProfile = new Dictionary<string, IGenericConfigurationEditor>() {
            {"ActionObjects//" + typeof(ProjectileConfiguration).Name, new GenericConfigurationEditor<LaunchProjectileId, ProjectileInherentData>("t:" + typeof(ProjectileConfiguration).Name) },
            {typeof(TargetZonesConfiguration).Name, new GenericConfigurationEditor<TargetZoneID, TargetZoneInherentData>("t:" + typeof(TargetZonesConfiguration).Name) },
            {"ActionObjects//" + typeof(AttractiveObjectConfiguration).Name, new GenericConfigurationEditor<AttractiveObjectId, AttractiveObjectInherentConfigurationData>("t:" + typeof(AttractiveObjectConfiguration).Name) },
            {typeof(LevelConfiguration).Name, new GenericConfigurationEditor<LevelZonesID, LevelConfigurationData>("t:" + typeof(LevelConfiguration).Name) },
            {typeof(SelectionWheelNodeConfiguration).Name, new GenericConfigurationEditor<SelectionWheelNodeConfigurationId, RTPuzzle.SelectionWheelNodeConfigurationData>("t:" + typeof(SelectionWheelNodeConfiguration).Name) },
            {typeof(AIComponentsConfiguration).Name, new GenericConfigurationEditor<AiID, AIBehaviorInherentData>("t:" + typeof(AIComponentsConfiguration).Name) },
            {typeof(PlayerActionConfiguration).Name, new GenericConfigurationEditor<PlayerActionId, PlayerActionInherentData>("t:" + typeof(PlayerActionConfiguration).Name) },
            {typeof(ContextMarkVisualFeedbackConfiguration).Name, new GenericConfigurationEditor<AiID, ContextMarkVisualFeedbackInherentData>("t:" + typeof(ContextMarkVisualFeedbackConfiguration).Name) },
            {typeof(RangeTypeConfiguration).Name, new GenericConfigurationEditor<RangeTypeID, RangeTypeInherentConfigurationData>("t:" + typeof(RangeTypeConfiguration).Name) },
    };

    [SerializeField]
    private Dictionary<string, IGenericConfigurationEditor> myConf = ConfigurationProfile;

    public static string GetConfigurationID(Type configurationDataType)
    {
        foreach (var configuration in ConfigurationProfile)
        {
            if (configuration.Key.Contains(configurationDataType.Name))
            {
                return configuration.Key;
            }
        }
        return string.Empty;
    }

}
