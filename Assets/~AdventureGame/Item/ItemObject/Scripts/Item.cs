using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class Item : MonoBehaviour
    {

        public ItemID ItemID;

        private ItemInherentData itemInherentData;
        private List<AContextAction> contextActions;

        public List<AContextAction> ContextActions { get => contextActions; }
        public ItemInherentData ItemInherentData { get => itemInherentData; }

        private void Start()
        {
            var AdventureConfigurationManager = GameObject.FindObjectOfType<AdventureGameConfigurationManager>();

            this.itemInherentData = AdventureConfigurationManager.ItemConf()[this.ItemID];
            contextActions = ItemContextActionBuilder.BuilItemContextActions(this);
        }
    }

}