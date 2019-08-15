using UnityEngine;

namespace CoreGame
{

    public class PlayerAdventurePositionManager : MonoBehaviour
    {

        private PlayerPositionPersister playerPositionPersister;

        private PlayerPosition playerPositionBeforeLevelLoad;
        
        public PlayerPosition PlayerPositionBeforeLevelLoad { get => playerPositionBeforeLevelLoad; }
        
        public void Init()
        {
            this.playerPositionPersister = new PlayerPositionPersister();

            if (this.playerPositionBeforeLevelLoad == null)
            {
                var loadedPlayerPositionBeforeLevelLoad = this.playerPositionPersister.Load();
                if (loadedPlayerPositionBeforeLevelLoad == null)
                {
                    this.OnAdventureToPuzzleLevel();
                }
                else
                {
                    this.playerPositionBeforeLevelLoad = loadedPlayerPositionBeforeLevelLoad;
                }
            }
        }

        #region External Events
        public void OnAdventureToPuzzleLevel()
        {
            this.playerPositionBeforeLevelLoad = CoreGameSingletonInstances.PlayerManagerType.GetPlayerPosition();
            this.playerPositionPersister.SaveAsync(this.playerPositionBeforeLevelLoad);
        }
        #endregion
    }

    [System.Serializable]
    public class PlayerPosition
    {
        [SerializeField]
        public Vector3Binarry Position;
        [SerializeField]
        public QuaternionBinarry Quaternion;

        public PlayerPosition(Vector3 position, Quaternion quaternion)
        {
            Position = new Vector3Binarry(position);
            Quaternion = new QuaternionBinarry(quaternion);
        }

        public Vector3 GetPosition()
        {
            return new Vector3(Position.x, Position.y, Position.z);
        }

        public Quaternion GetQuaternion()
        {
            return new Quaternion(Quaternion.x, Quaternion.y, Quaternion.z, Quaternion.w);
        }
    }

    class PlayerPositionPersister : AbstractGamePersister<PlayerPosition>
    {
        public PlayerPositionPersister() : base("PlayerPosition", ".pl", "PlayerPosition")
        {
        }
    }
}
