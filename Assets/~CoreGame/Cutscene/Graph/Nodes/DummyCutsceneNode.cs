using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using NodeGraph;

namespace CoreGame
{
    [System.Serializable]
    public class DummyCutsceneNode : ACutsceneNode<DummyCutsceneAction, DummyCutsceneEdge>
    {
#if UNITY_EDITOR
        public override bool DisplayWorkflowEdge()
        {
            return false;
        }

        protected override Color NodeColor()
        {
            return Color.black;
        }
#endif
    }
}
