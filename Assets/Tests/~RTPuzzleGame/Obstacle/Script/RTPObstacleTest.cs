using RTPuzzle;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class RTPObstacleTest
    {

        private IEnumerator Before()
        {
            SceneManager.LoadScene("RTP_TEST_OBSTACLES", LoadSceneMode.Single);
            yield return new WaitForFixedUpdate();
        }

        [UnityTest]
        public IEnumerator OccludedIntersection_Test()
        {
            yield return this.Before();
            var testObstacleRangObject = AssetFinder.SafeSingleAssetFind<RangeTypeObject>("TEST_OBSTACLE_RangeObject");
            var instanciatedRange = MonoBehaviour.Instantiate(testObstacleRangObject);
            instanciatedRange.transform.position = PuzzleSceneTestHelper.FindTestPosition(TestPositionID.ATTRACTIVE_OBJECT_NOMINAL).position;
            instanciatedRange.Init(new RangeTypeObjectInitializer(sphereRadius: 99999f));
            
            yield return new WaitForFixedUpdate(); //Wait for obstacle collider tracker to detect obstacles
            Assert.IsFalse(this.TestIntersection(instanciatedRange, TestPositionID.OBSTACLE_LISTENER_POSITION_1));
            Assert.IsTrue(this.TestIntersection(instanciatedRange, TestPositionID.OBSTACLE_LISTENER_POSITION_2));
            Assert.IsTrue(this.TestIntersection(instanciatedRange, TestPositionID.OBSTACLE_LISTENER_POSITION_3));
        }

        private bool TestIntersection(RangeTypeObject RangeTypeObject, TestPositionID testPosition)
        {
            var position = PuzzleSceneTestHelper.FindTestPosition(testPosition).position;
            return RangeTypeObject.IsInsideAndNotOccluded(position);
        }

    }

}
