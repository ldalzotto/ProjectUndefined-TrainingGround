using UnityEngine;
using System.Collections;

namespace CoreGame
{
    [System.Serializable]
    public class PlayerPhysicsMovementComponent : ADataComponent
    {
        public float ContactDistance = 1f;
    }
}
