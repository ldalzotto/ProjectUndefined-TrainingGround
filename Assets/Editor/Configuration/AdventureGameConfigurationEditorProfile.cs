using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using System.Collections.Generic;
using AdventureGame;

[System.Serializable]
[CreateAssetMenu(fileName = "AdventureGameConfigurationEditorProfile", menuName = "Configuration/AdventureGameConfigurationEditorProfile", order = 1)]
public class AdventureGameConfigurationEditorProfile : TreeChoiceHeaderTab<IGenericConfigurationEditor>
{

    public static Dictionary<string, IGenericConfigurationEditor> ConfigurationProfile = new Dictionary<string, IGenericConfigurationEditor>()
    {
        {"Item//" + typeof(ItemConfiguration).Name, new GenericConfigurationEditor<ItemID, ItemInherentData>("t:"+ typeof(ItemConfiguration).Name) }
    };

    public override Dictionary<string, IGenericConfigurationEditor> Configurations => this.myConfig;

    [SerializeField]
    private Dictionary<string, IGenericConfigurationEditor> myConfig = ConfigurationProfile;
}
