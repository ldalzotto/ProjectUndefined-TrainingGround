using UnityEngine;
using System.Collections;

namespace AdventureGame
{
    [System.Serializable]
    public class TalkActionInherentData : AContextActionInherentData
    {
        public override AContextAction BuildContextAction()
        {
            return new TalkAction(null);
        }
    }

}
