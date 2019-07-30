using CoreGame;
using GameConfigurationID;

namespace RTPuzzle
{
    public class PuzzleCutsceneActionInput : SequencedActionInput
    {
        private PuzzleCutsceneId cutsceneId;
        private LevelManager levelManager;
        private NPCAIManagerContainer nPCAIManagerContainer;

        public PuzzleCutsceneActionInput(PuzzleCutsceneId cutsceneId, LevelManager levelManager, NPCAIManagerContainer NPCAIManagerContainer)
        {
            this.cutsceneId = cutsceneId;
            this.levelManager = levelManager;
            this.nPCAIManagerContainer = NPCAIManagerContainer;
        }

        public PuzzleCutsceneId CutsceneId { get => cutsceneId; }
        public LevelManager LevelManager { get => levelManager;  }
        public NPCAIManagerContainer NPCAIManagerContainer { get => nPCAIManagerContainer; }
    }
}
