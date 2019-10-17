using CoreGame;

namespace InteractiveObjects
{
    public class PlayerInteractiveObjectManager : GameSingleton<PlayerInteractiveObjectManager>
    {
        public PlayerInteractiveObject PlayerInteractiveObject { get; private set; }

        public void Init(PlayerInteractiveObject PlayerInteractiveObject)
        {
            this.PlayerInteractiveObject = PlayerInteractiveObject;
        }

        public void FixedTick(float d)
        {
            PlayerInteractiveObject.FixedTick(d);
        }

        public void TickAlways(float d)
        {
            PlayerInteractiveObject.TickAlways(d);
        }

        public void LateTick(float d)
        {
            PlayerInteractiveObject.LateTick(d);
        }

        public IInteractiveGameObject GetPlayerGameObject()
        {
            return PlayerInteractiveObject.InteractiveGameObject;
        }

        public float GetNormalizedSpeed()
        {
            return PlayerInteractiveObject.GetNormalizedSpeed();
        }

        public bool HasPlayerMovedThisFrame()
        {
            return PlayerInteractiveObject.HasPlayerMovedThisFrame();
        }
    }
}