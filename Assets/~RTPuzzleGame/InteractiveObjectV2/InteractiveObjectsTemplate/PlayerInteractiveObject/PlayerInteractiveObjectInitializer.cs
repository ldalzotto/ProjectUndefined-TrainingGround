using InteractiveObjects_Interfaces;

namespace InteractiveObjects
{
    [SceneHandleDraw]
    public class PlayerInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested] public InteractiveObjectLogicColliderDefinition InteractiveObjectLogicCollider;

        public override void Init()
        {
            var PlayerInteractiveObject = new PlayerInteractiveObject(InteractiveGameObjectFactory.Build(gameObject), InteractiveObjectLogicCollider);
            PlayerInteractiveObjectManager.Get().Init(PlayerInteractiveObject);
        }
    }
}