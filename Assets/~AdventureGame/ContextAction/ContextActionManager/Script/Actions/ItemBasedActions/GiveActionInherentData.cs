using UnityEngine;
using System.Collections;

namespace AdventureGame
{
    [System.Serializable]
    public class GiveActionInherentData : AContextActionInherentData
    {
        public ItemID ItemGiven;

        public override AContextAction BuildContextAction()
        {
            return new GiveAction(this.ItemGiven, null);
        }
    }

}
