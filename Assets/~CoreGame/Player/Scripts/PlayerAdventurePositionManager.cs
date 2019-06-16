using UnityEngine;

namespace CoreGame
{

    public class PlayerAdventurePositionManager : MonoBehaviour
    {

        private PlayerPositionPersister playerPositionPersister;

        private PlayerPosition playerPositionBeforeLevelLoad;

        #region External Dependencies
        private PlayerManagerType PlayerManagerType;

        public PlayerPosition PlayerPositionBeforeLevelLoad { get => playerPositionBeforeLevelLoad; }

        #endregion
        public void Init()
        {
            this.playerPositionPersister = new PlayerPositionPersister();
            this.PlayerManagerType = GameObject.FindObjectOfType<PlayerManagerType>();

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
            this.playerPositionBeforeLevelLoad = this.PlayerManagerType.GetPlayerPosition();
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
        [SerializeField]
        public QuaternionBinarry CameraPivotPointQuaternion;

        public PlayerPosition(Vector3 position, Quaternion quaternion, Quaternion cameraPivotPointQuaterion)
        {
            Position = new Vector3Binarry(position);
            Quaternion = new QuaternionBinarry(quaternion);
            CameraPivotPointQuaternion = new QuaternionBinarry(cameraPivotPointQuaterion);
        }

        public Vector3 GetPosition()
        {
            return new Vector3(Position.x, Position.y, Position.z);
        }

        public Quaternion GetQuaternion()
        {
            return new Quaternion(Quaternion.x, Quaternion.y, Quaternion.z, Quaternion.w);
        }

        public Quaternion GetCameraQuaternion()
        {
            return new Quaternion(CameraPivotPointQuaternion.x, CameraPivotPointQuaternion.y, CameraPivotPointQuaternion.z, CameraPivotPointQuaternion.w);
        }
    }

    class PlayerPositionPersister : AbstractGamePersister<PlayerPosition>
    {
        public PlayerPositionPersister() : base("PlayerPosition", ".pl", "PlayerPosition")
        {
        }
    }
}
