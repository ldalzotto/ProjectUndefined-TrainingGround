using UnityEngine;
using System.Collections;

namespace AdventureGame
{
    [System.Serializable]
    public class InteractActionInherentData : AContextActionInherentData
    {
        public ItemID InvolvedItem;
        public override AContextAction BuildContextAction()
        {
            return new InteractAction(InvolvedItem, null);
        }
    }

}
