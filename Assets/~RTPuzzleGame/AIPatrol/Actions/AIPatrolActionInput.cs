using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

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
    }

}
