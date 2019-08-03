using CoreGame;
using GameConfigurationID;

namespace RTPuzzle
{
    public class PuzzleCutsceneActionInput : SequencedActionInput
    {
        private InteractiveObjectContainer interactiveObjectContainer;

        public PuzzleCutsceneActionInput(InteractiveObjectContainer interactiveObjectContainer)
        {
            this.interactiveObjectContainer = interactiveObjectContainer;
        }
        
        public InteractiveObjectContainer InteractiveObjectContainer { get => interactiveObjectContainer;  }
    }
}
