using UnityEngine;
using System.Collections;

namespace AdventureGame
{
    [System.Serializable]
    public class LevelZoneTransitionActionInherentData : AContextActionInherentData
    {
        public LevelZonesID NextZone;
        public override AContextAction BuildContextAction()
        {
            return new LevelZoneTransitionAction(this.NextZone);
        }
    }

}
