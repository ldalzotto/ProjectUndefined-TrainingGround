using GameConfigurationID;
using InteractiveObjectTest;

namespace RTPuzzle
{
    [System.Serializable]
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