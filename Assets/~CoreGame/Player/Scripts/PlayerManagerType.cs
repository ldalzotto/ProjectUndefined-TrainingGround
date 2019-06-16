using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public abstract class PlayerManagerType : MonoBehaviour
    {
        public abstract PlayerPosition GetPlayerPosition();
    }

}
