using UnityEngine;
using System.Collections;

namespace AdventureGame
{
    [System.Serializable]
    public class CutsceneTimelineActionInherentData : AContextActionInherentData
    {
        public CutsceneId CutsceneId;
        public bool DestroyPOIAtEnd;
        public override AContextAction BuildContextAction()
        {
            return new CutsceneTimelineAction(CutsceneId, null, DestroyPOIAtEnd);
        }
    }

}
