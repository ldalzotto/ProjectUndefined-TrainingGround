using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "CutsceneActionInherentData", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/CutsceneActionInherentData", order = 1)]
    public class CutsceneActionInherentData : PlayerActionInherentData
    {
        [CustomEnum()]
        public PuzzleCutsceneId PuzzleCutsceneId;

        public CutsceneActionInherentData(PuzzleCutsceneId PuzzleCutsceneId, SelectionWheelNodeConfigurationId actionWheelNodeConfigurationId, float coolDownTime) : base(actionWheelNodeConfigurationId, coolDownTime)
        {
            this.PuzzleCutsceneId = PuzzleCutsceneId;
        }
    }
}
