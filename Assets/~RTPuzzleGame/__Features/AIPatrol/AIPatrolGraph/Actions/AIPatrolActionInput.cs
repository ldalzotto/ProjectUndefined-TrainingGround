using CoreGame;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class AIPatrolActionInput : SequencedActionInput
    {
        private IAIPatrolGraphEventListener aIPatrolGraphEventListener;

        public AIPatrolActionInput(IAIPatrolGraphEventListener iAIPatrolGraphEventListener, Dictionary<CutsceneParametersName, object> graphParameters)
        {
            aIPatrolGraphEventListener = iAIPatrolGraphEventListener;
            this.graphParameters = graphParameters;
        }

        public IAIPatrolGraphEventListener AIPatrolGraphEventListener { get => aIPatrolGraphEventListener; }

        public static Dictionary<CutsceneParametersName, object> BuildParameters(InteractiveObjectType associatedInteractiveObject)
        {
            return new Dictionary<CutsceneParametersName, object>() {

                { CutsceneParametersName.AIPatrol_InteractiveObject, associatedInteractiveObject },
            };
        }
        
    }

}
