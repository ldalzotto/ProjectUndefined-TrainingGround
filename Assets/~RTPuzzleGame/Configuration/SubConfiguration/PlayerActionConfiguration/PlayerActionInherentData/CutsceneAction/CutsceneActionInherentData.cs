﻿using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "CutsceneActionInherentData", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/CutsceneActionInherentData", order = 1)]
    public class CutsceneActionInherentData : PlayerActionInherentData
    {
        [CustomEnum(ConfigurationType = typeof(PuzzleCutsceneConfiguration))]
        public PuzzleCutsceneID PuzzleCutsceneId;

        public CutsceneActionInherentData(PuzzleCutsceneID PuzzleCutsceneId, SelectionWheelNodeConfigurationId actionWheelNodeConfigurationId, float coolDownTime) : base(actionWheelNodeConfigurationId, coolDownTime)
        {
            this.PuzzleCutsceneId = PuzzleCutsceneId;
        }
    }
}
