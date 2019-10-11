using GameConfigurationID;
using InteractiveObjectTest;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "CutsceneActionInherentData", menuName = "Test/CutsceneActionInherentData", order = 1)]
    public class CutsceneActionInherentData : PlayerActionInherentData
    {
        public PuzzleCutsceneID PuzzleCutsceneID;
        public CutsceneActionInherentData(CorePlayerActionDefinition corePlayerActionDefinition) : base(corePlayerActionDefinition)
        {
        }

        public override RTPPlayerAction BuildPlayerAction(PlayerInteractiveObject PlayerInteractiveObject)
        {
            return new CutsceneAction(this.PuzzleCutsceneID, this.CorePlayerActionDefinition);
        }
    }
}