﻿using UnityEngine;
using System.Collections;
using CoreGame;
using System;
using System.Collections.Generic;

namespace RTPuzzle
{
    [System.Serializable]
    public class PuzzleTutorialTextEdge : ACutsceneEdge<PuzzleTutorialTextAction>
    {
        [SerializeField]
        public override List<Type> AllowedConnectedNodeEdges => base.AllowedConnectedNodeEdges;
    }

}
