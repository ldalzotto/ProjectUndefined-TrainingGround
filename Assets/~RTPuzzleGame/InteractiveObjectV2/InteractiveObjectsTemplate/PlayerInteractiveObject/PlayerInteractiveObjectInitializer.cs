namespace InteractiveObjects
{
    [SceneHandleDraw]
    public class PlayerInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested] public InteractiveObjectLogicCollider InteractiveObjectLogicCollider;

        public override void Init()
        {
            var PlayerInteractiveObject = new PlayerInteractiveObject(InteractiveGameObjectFactory.Build(gameObject), InteractiveObjectLogicCollider);
            PlayerInteractiveObjectManager.Get().Init(PlayerInteractiveObject);
        }
    }
}