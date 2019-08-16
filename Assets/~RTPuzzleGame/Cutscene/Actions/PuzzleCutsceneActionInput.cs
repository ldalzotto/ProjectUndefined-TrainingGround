using CoreGame;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class PuzzleCutsceneActionInput : SequencedActionInput
    {
        private InteractiveObjectContainer interactiveObjectContainer;

        public InteractiveObjectContainer InteractiveObjectContainer { get => interactiveObjectContainer; }

        public Dictionary<PuzzleCutsceneParametersName, object> PuzzleCutsceneGraphParameters;

        public PuzzleCutsceneActionInput(InteractiveObjectContainer interactiveObjectContainer, Dictionary<PuzzleCutsceneParametersName, object> PuzzleCutsceneGraphParameters = null)
        {
            this.interactiveObjectContainer = interactiveObjectContainer;
            this.PuzzleCutsceneGraphParameters = PuzzleCutsceneGraphParameters;
        }
    }

    public enum PuzzleCutsceneParametersName
    {
        INTERACTIVE_OBJECT_0 = 0
    }
}
