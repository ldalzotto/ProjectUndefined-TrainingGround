using UnityEngine;
using System.Collections;
using CoreGame;

namespace AdventureGame
{
    [System.Serializable]
    public class CutscenePersistPOINode : ACutsceneNode<CutscenePersistPOIAction, CutscenePersistPOIEdge>
    {
#if UNITY_EDITOR
        protected override Color NodeColor()
        {
            return MyColors.MintGreen;
        }
#endif
    }
}
