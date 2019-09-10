using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    public class CutscenePersistPOIEdge : ACutsceneEdge<CutscenePersistPOIAction>
    {
#if UNITY_EDITOR
        protected override Color EdgeColor()
        {
            return MyColors.MintGreen;
        }
#endif
    }

}
