using InteractiveObjects;

namespace AdventureGame
{
    [SceneHandleDraw]
    public class AdventurePlayerInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested] public AIAgentDefinition AIAgentDefinition;
        [DrawNested] public InteractiveObjectLogicCollider InteractiveObjectLogicCollider;

        public override void Init()
        {
            var PlayerInteractiveObject = new AdventurePlayerInteractiveObject(InteractiveGameObjectFactory.Build(gameObject), InteractiveObjectLogicCollider, AIAgentDefinition);
            //  PlayerInteractiveObjectManager.Get().Init(PlayerInteractiveObject);
        }
    }
}