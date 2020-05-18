﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class LevelCompleteVisualFeedbackFX : TriggerableParticleSystemFX
    {
        public override void Init()
        {
            base.Init();
            var playerPosition = PuzzleGameSingletonInstances.PlayerManagerDataRetriever.GetPlayerTransform();
            this.transform.position = playerPosition.transform.position;
        }
    }

}
