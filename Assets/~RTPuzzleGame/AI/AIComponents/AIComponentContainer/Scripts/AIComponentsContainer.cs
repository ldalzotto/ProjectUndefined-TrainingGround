using UnityEngine;

namespace RTPuzzle
{

    public class AIComponentsContainer : MonoBehaviour
    {
        public AiID AiID;

        public AIComponents InitAIComponents()
        {
            var aiComponents = new AIComponents();
            var initializeMethodName = nameof(AIComponentInitializerMessageReceiver.InitializeContainer);
            BroadcastMessage(initializeMethodName, aiComponents);
            return aiComponents;
        }

    }


    public class AIComponents
    {
        public AIRandomPatrolComponent AIRandomPatrolComponent;
        public AIProjectileEscapeComponent AIProjectileEscapeComponent;
        public AITargetZoneComponent AITargetZoneComponent;
        public AIFearStunComponent AIFearStunComponent;
    }
}