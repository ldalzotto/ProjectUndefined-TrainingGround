using UnityEngine;
using System.Collections;

namespace CoreGame
{
    [System.Serializable]
    public class PlayerPhysicsMovementComponent : ADataComponent
    {
        public float MinimumDistanceToStick = 0.01f;
    }
}
