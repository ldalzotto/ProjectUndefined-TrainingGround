using GameConfigurationID;
using RTPuzzle;
using System.Collections;
using System.Linq;
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
            yield return null;
        }

        [UnityTest]
        public IEnumerator OccludedIntersection_Test()
        {
            //RangeEffectTestAI
            yield return this.Before();
            var puzzleStaticConfiguration = GameObject.FindObjectOfType<PuzzleStaticConfigurationContainer>();

            var instanciatedRange = new SphereRangeObjectV2(null, RangeObjectInitializationDataBuilderV2.SphereRangeWithObstacleListener(99999f, RangeTypeID.ATTRACTIVE_OBJECT), null);
            instanciatedRange.ReceiveEvent(new SetWorldPositionEvent { WorldPosition = PuzzleSceneTestHelper.FindTestPosition(TestPositionID.ATTRACTIVE_OBJECT_NOMINAL).position });

            yield return new WaitForFixedUpdate();

            var ObstacleFrustumCalculationManager = GameObject.FindObjectOfType<ObstacleFrustumCalculationManager>();
            //wait for frustum calculation;
            yield return new WaitUntil(() =>
            {
                var emptyCalculation =
                ObstacleFrustumCalculationManager.CalculationResults.Values.ToList().SelectMany(r => r.Values).Select(r => r)
                    .Where(r => r.CalculatedFrustumPositions.Count == 0)
                    .ToList();

                return emptyCalculation.Count > 0;
            });

            //Point Intersection
            Assert.IsFalse(this.IsInsideAndNotOccluded(instanciatedRange, TestPositionID.OBSTACLE_LISTENER_POSITION_1));
            Assert.IsTrue(this.IsInsideAndNotOccluded(instanciatedRange, TestPositionID.OBSTACLE_LISTENER_POSITION_2));
            Assert.IsTrue(this.IsInsideAndNotOccluded(instanciatedRange, TestPositionID.OBSTACLE_LISTENER_POSITION_3));
            Assert.IsFalse(this.IsInsideAndNotOccluded(instanciatedRange, TestPositionID.OBSTACLE_LISTENER_POSITION_4));

            //BOX Intersection

        }

        private bool IsInsideAndNotOccluded(SphereRangeObjectV2 RangeTypeObject, TestPositionID testPosition)
        {
            var position = PuzzleSceneTestHelper.FindTestPosition(testPosition).position;
            return RangeIntersectionOperations.IsInsideAndNotOccluded(RangeTypeObject, position, forceObstacleOcclusionIfNecessary: true);
        }

    }

}
