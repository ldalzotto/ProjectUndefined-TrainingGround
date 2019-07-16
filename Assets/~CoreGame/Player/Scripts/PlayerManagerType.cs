using UnityEngine;

namespace CoreGame
{
    public abstract class PlayerManagerType : MonoBehaviour
    {
        public PlayerPosition GetPlayerPosition()
        {
            return new PlayerPosition(this.transform.position, this.transform.rotation);
        }
    }

}
