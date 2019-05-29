using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AdventureGame
{
    public class AdventureGameConfigurationManager : MonoBehaviour
    {
        public AdventureGameConfiguration AdventureGameConfiguration;

        public Dictionary<ItemID, ItemInherentData> ItemConf()
        {
            return AdventureGameConfiguration.ItemConfiguration.ConfigurationInherentData;
        }

    }

}
