using UnityEngine;

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
}