using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameConfigurationID;
using CoreGame;

namespace AdventureGame
{
    public class AdventureGameConfigurationManager : MonoBehaviour
    {
        public AdventureGameConfiguration AdventureGameConfiguration;

        public Dictionary<ItemID, ItemInherentData> ItemConf()
        {
            return AdventureGameConfiguration.ItemConfiguration.ConfigurationInherentData;
        }

        public Dictionary<PointOfInterestId, PointOfInterestInherentData> POIConf()
        {
            return AdventureGameConfiguration.PointOfInterestConfiguration.ConfigurationInherentData;
        }

        public Dictionary<CutsceneId, CutsceneInherentData> CutsceneConf()
        {
            return AdventureGameConfiguration.CutsceneConfiguration.ConfigurationInherentData;
        }
//${addNewEntry}

        
    }

}
