using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class Item : MonoBehaviour
    {

        public ItemID ItemID;

        private ItemInherentData itemInherentData;

        #region Internal Dependencies
        private PointOfInterestType PointOfInterestType;
        #endregion

        public ItemInherentData ItemInherentData { get => itemInherentData; }

        #region Data Retrieval 
        public List<AContextAction> GetContextActions()
        {
            return this.PointOfInterestType.GetContextActions();
        }
        #endregion

        private void Start()
        {
            this.PointOfInterestType = GetComponent<PointOfInterestType>();
            this.PointOfInterestType.Init();
            AdventureGameSingletonInstances.PointOfInterestAdventureEventManager.OnPOICreated(this.PointOfInterestType);

            var AdventureConfigurationManager = AdventureGameSingletonInstances.AdventureGameConfigurationManager;

            this.itemInherentData = AdventureConfigurationManager.ItemConf()[this.ItemID];
        }

    }

}