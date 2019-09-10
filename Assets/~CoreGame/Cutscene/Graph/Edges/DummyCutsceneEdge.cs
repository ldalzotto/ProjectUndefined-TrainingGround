using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    [System.Serializable]
    public class DummyCutsceneEdge : ACutsceneEdge<DummyCutsceneAction>
    {
#if UNITY_EDITOR
        protected override Color EdgeColor()
        {
            return Color.black;
        }

        protected override float DefaultGetEdgeHeight()
        {
            return 50f;
        }
#endif
    }
}
