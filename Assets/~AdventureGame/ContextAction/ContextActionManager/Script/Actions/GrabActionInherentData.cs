using UnityEngine;
using System.Collections;

namespace AdventureGame
{
    [System.Serializable]
    public class GrabActionInherentData : AContextActionInherentData
    {
        public ItemID Item;
        public bool DeletePOIOnGrab;

        public override AContextAction BuildContextAction()
        {
            return new GrabAction(this.Item, this.DeletePOIOnGrab, null);
        }
    }

}
