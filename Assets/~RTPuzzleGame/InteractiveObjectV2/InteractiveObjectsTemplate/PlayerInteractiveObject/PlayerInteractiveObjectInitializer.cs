using CoreGame;

namespace InteractiveObjects
{
    [SceneHandleDraw]
    public class PlayerInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested]
        public InteractiveObjectLogicCollider InteractiveObjectLogicCollider;

        public override void Init()
        {
            var PlayerInteractiveObject = new PlayerInteractiveObject(InteractiveGameObjectFactory.Build(this.gameObject), InteractiveObjectLogicCollider);
            PlayerInteractiveObjectManager.Get().Init(PlayerInteractiveObject);
        }
    }
}
