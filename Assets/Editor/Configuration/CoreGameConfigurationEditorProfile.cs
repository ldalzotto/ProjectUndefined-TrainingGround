using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using System.Collections.Generic;
using CoreGame;
using System;
using GameConfigurationID;

[System.Serializable]
[CreateAssetMenu(fileName = "CoreGameConfigurationEditorProfile", menuName = "Configuration/CoreGameConfigurationEditorProfile", order = 1)]
public class CoreGameConfigurationEditorProfile : TreeChoiceHeaderTab<IGenericConfigurationEditor>
{
    public override Dictionary<string, IGenericConfigurationEditor> Configurations => this.myConfig;

    public static Dictionary<string, IGenericConfigurationEditor> ConfigurationProfile = new Dictionary<string, IGenericConfigurationEditor>()
    {
        {"LevelManagement//" + typeof(LevelZonesSceneConfiguration).Name, new GenericConfigurationEditor<LevelZonesID, LevelZonesSceneConfigurationData>("t:"+ typeof(LevelZonesSceneConfiguration).Name) },
        {"LevelManagement//" + typeof(ChunkZonesSceneConfiguration).Name, new GenericConfigurationEditor<LevelZoneChunkID, LevelZonesSceneConfigurationData>("t:"+ typeof(ChunkZonesSceneConfiguration).Name) },
        {"LevelManagement//" + typeof(LevelHierarchyConfiguration).Name, new GenericConfigurationEditor<LevelZonesID, LevelHierarchyConfigurationData>("t:"+ typeof(LevelHierarchyConfiguration).Name) },
        {"Timeline//" + typeof(TimelineConfiguration).Name, new GenericConfigurationEditor<TimelineID, TimelineInitializerScriptableObject>("t:"+ typeof(TimelineConfiguration).Name) }
    };

    [SerializeField]
    private Dictionary<string, IGenericConfigurationEditor> myConfig = ConfigurationProfile;

}
