using CoreGame;

namespace InteractiveObjectTest
{
    public class PlayerInteractiveObjectInitializer : InteractiveObjectInitializer
    {

        public override void Init()
        {
            var PlayerInteractiveObject = new PlayerInteractiveObject(new InteractiveGameObject(this.gameObject));
            PlayerInteractiveObjectManager.Get().Init(PlayerInteractiveObject);
        }
    }
}
