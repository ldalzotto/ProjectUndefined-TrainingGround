using CoreGame;

namespace InteractiveObjectTest
{
    public class PlayerInteractiveObjectInitializer : A_InteractiveObjectInitializer
    {

        public override void Init()
        {
            base.Init();
        }

        protected override object GetInitializerDataObject()
        {
            return null;
        }

        protected override CoreInteractiveObject GetInteractiveObject()
        {
            var PlayerInteractiveObject =  new PlayerInteractiveObject(new InteractiveGameObject(this.gameObject));
            PlayerInteractiveObjectManager.Get().Init(PlayerInteractiveObject);
            return PlayerInteractiveObject;
        }
    }
}
