using CoreGame;

namespace InteractiveObjectTest
{
    [SceneHandleDraw]
    public class PlayerInteractiveObjectInitializer : InteractiveObjectInitializer
    {
        [DrawNested]
        public InteractiveObjectLogicCollider InteractiveObjectLogicCollider;

        public override void Init()
        {
            var PlayerInteractiveObject = new PlayerInteractiveObject(new InteractiveGameObject(this.gameObject), InteractiveObjectLogicCollider);
            PlayerInteractiveObjectManager.Get().Init(PlayerInteractiveObject);
        }
    }
}
