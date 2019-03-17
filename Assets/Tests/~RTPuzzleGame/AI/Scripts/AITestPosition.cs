using UnityEngine;

namespace Tests
{
    public class AITestPosition : MonoBehaviour
    {
        public AITestPositionID aITestPositionID;
    }

    public enum AITestPositionID
    {
        ATTRACTIVE_OBJECT_NOMINAL = 0,
        PROJECTILE_TARGET_1 = 1,
        PROJECTILE_TARGET_2 = 2
    }

}
