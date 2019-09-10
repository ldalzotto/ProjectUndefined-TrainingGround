using UnityEngine;
using System.Collections;
using CoreGame;
using System;
using System.Collections.Generic;

namespace CoreGame
{
    [System.Serializable]
    public class TutorialTextEdge : ACutsceneEdge<TutorialTextAction>
    {
        [SerializeField]
        public override List<Type> AllowedConnectedNodeEdges => base.AllowedConnectedNodeEdges;
    }

}
