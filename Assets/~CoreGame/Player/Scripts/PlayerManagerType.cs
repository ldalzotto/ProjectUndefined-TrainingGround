using UnityEngine;

namespace CoreGame
{
    //TODO -> To remove when PlayerObject module is created !
    public class PlayerManagerType : MonoBehaviour
    {
        public PlayerPosition GetPlayerPosition()
        {
            return new PlayerPosition(transform.position, transform.rotation);
        }
    }
}