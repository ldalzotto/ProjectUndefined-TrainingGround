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
        PROJECTILE_TARGET_2 = 2,
        FAR_AWAY_POSITION_1 = 3,
        PITFAL_Z_POSITION_1 = 4,
        PITFAL_Z_POSITION_FAR_EDGE = 5
    }

}
