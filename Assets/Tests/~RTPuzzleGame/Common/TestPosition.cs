using UnityEngine;

namespace Tests
{
    public class TestPosition : MonoBehaviour
    {
        public TestPositionID aITestPositionID;
    }

    public enum TestPositionID
    {
        ATTRACTIVE_OBJECT_NOMINAL = 0,
        PROJECTILE_TARGET_1 = 1,
        PROJECTILE_TARGET_2 = 2,
        FAR_AWAY_POSITION_1 = 3,
        PITFAL_Z_POSITION_1 = 4,
        PITFAL_Z_POSITION_FAR_EDGE = 5,
        OBSTACLE_LISTENER_POSITION_1 = 6,
        OBSTACLE_LISTENER_POSITION_2 = 7,
        OBSTACLE_LISTENER_POSITION_3 = 8
    }

}
