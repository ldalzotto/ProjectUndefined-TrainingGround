using CoreGame;
using RTPuzzle;

namespace InteractiveObjects
{
    public class PlayerInteractiveObjectManager : GameSingleton<PlayerInteractiveObjectManager>
    {
        public PlayerInteractiveObject PlayerInteractiveObject { get; private set; }

        public void Init(PlayerInteractiveObject PlayerInteractiveObject)
        {
            this.PlayerInteractiveObject = PlayerInteractiveObject;
        }

        public void FixedTick(float d) { this.PlayerInteractiveObject.FixedTick(d); }
        public void TickAlways(float d) { this.PlayerInteractiveObject.TickAlways(d); }
        public void LateTick(float d) { this.PlayerInteractiveObject.LateTick(d); }
        
        public IInteractiveGameObject GetPlayerGameObject()
        {
            return this.PlayerInteractiveObject.InteractiveGameObject;
        }

        public float GetNormalizedSpeed()
        {
            return this.PlayerInteractiveObject.GetNormalizedSpeed();
        }
        public bool HasPlayerMovedThisFrame()
        {
            return this.PlayerInteractiveObject.HasPlayerMovedThisFrame();
        }
    }
}
