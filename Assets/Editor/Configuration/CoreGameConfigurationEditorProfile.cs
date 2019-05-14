using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using System.Collections.Generic;
using CoreGame;

[System.Serializable]
[CreateAssetMenu(fileName = "CoreGameConfigurationEditorProfile", menuName = "Configuration/CoreGameConfigurationEditorProfile", order = 1)]
public class CoreGameConfigurationEditorProfile : TreeChoiceHeaderTab<IGenericConfigurationEditor>
{
    public override Dictionary<string, IGenericConfigurationEditor> Configurations => this.myConfig;

    [SerializeField]
    private Dictionary<string, IGenericConfigurationEditor> myConfig = new Dictionary<string, IGenericConfigurationEditor>()
    {
        {typeof(LevelZonesSceneConfiguration).Name, new GenericConfigurationEditor<LevelZonesID, LevelZonesSceneConfigurationData>("t:"+ typeof(LevelZonesSceneConfiguration).Name) },
        {typeof(ChunkZonesSceneConfiguration).Name, new GenericConfigurationEditor<LevelZoneChunkID, LevelZonesSceneConfigurationData>("t:"+ typeof(ChunkZonesSceneConfiguration).Name) }
    };
}
