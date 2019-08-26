using RTPuzzle;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class GroundEffectManagerTest : AbstractPuzzleSceneTest
    {
        [UnityTest]
        public IEnumerator RangeBufferSentToShader()
        {
            yield return this.Before("RTP_TEST_RangeEffectManager", () => {
                AIObjectDefinition.RangeEffectTestAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
            });

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

            yield return new WaitForEndOfFrame();
            var GroundEffectsManagerV2 = GameObject.FindObjectOfType<GroundEffectsManagerV2>();

            Assert.IsTrue(GroundEffectsManagerV2.RangeExecutionOrderBufferDataValues.Count == 2);

            List<RangeToFrustumBufferLink> attractiveObjectRangeToBufferLink = null;
            List<RangeToFrustumBufferLink> aiSightRangeToBufferLink = null;

            bool circleRangeVerificationDone = false;
            bool roundedRangeVerificationDone = false;

            foreach (var rangeExecutionOrder in GroundEffectsManagerV2.RangeExecutionOrderBufferDataValues)
            {
                if (rangeExecutionOrder.RangeType == 0)
                {
                    circleRangeVerificationDone = true;

                    Assert.IsTrue(GroundEffectsManagerV2.CircleRangeBufferValues.Count - 1 >= rangeExecutionOrder.Index);
                    Assert.IsTrue(GroundEffectsManagerV2.CircleRangeBufferValues[rangeExecutionOrder.Index].OccludedByFrustums == 1);

                    attractiveObjectRangeToBufferLink =
                        GroundEffectsManagerV2.RangeToFrustumBufferLinkValues.Select(l => l)
                                .Where(RangeToFrustumBufferLinkValue => RangeToFrustumBufferLinkValue.RangeType == rangeExecutionOrder.RangeType && RangeToFrustumBufferLinkValue.RangeIndex == rangeExecutionOrder.Index)
                                .ToList();

                    //The attractive object is only occluded by one obstacle and only two faces are concerned -> 2 frustum linked
                    Assert.IsTrue(attractiveObjectRangeToBufferLink.Count == 2);
                }
                else if (rangeExecutionOrder.RangeType == 3)
                {
                    roundedRangeVerificationDone = true;

                    Assert.IsTrue(GroundEffectsManagerV2.RoundedFrustumRangeBufferValues.Count - 1 >= rangeExecutionOrder.Index);
                    Assert.IsTrue(GroundEffectsManagerV2.RoundedFrustumRangeBufferValues[rangeExecutionOrder.Index].OccludedByFrustums == 1);

                    aiSightRangeToBufferLink =
                      GroundEffectsManagerV2.RangeToFrustumBufferLinkValues.Select(l => l)
                              .Where(RangeToFrustumBufferLinkValue => RangeToFrustumBufferLinkValue.RangeType == rangeExecutionOrder.RangeType && RangeToFrustumBufferLinkValue.RangeIndex == rangeExecutionOrder.Index)
                              .ToList();

                    //The all frustum associated to AI sight 
                    Assert.IsTrue(aiSightRangeToBufferLink.Count == 6);
                }
            }

            Assert.IsTrue(circleRangeVerificationDone);
            Assert.IsTrue(roundedRangeVerificationDone);

            Assert.IsTrue(GroundEffectsManagerV2.CircleRangeBufferValues.Count == 1);
            Assert.IsTrue(GroundEffectsManagerV2.FrustumRangeBufferValues.Count == 0);
            Assert.IsTrue(GroundEffectsManagerV2.RoundedFrustumRangeBufferValues.Count == 1);
            Assert.IsTrue(GroundEffectsManagerV2.BoxRangeBufferValues.Count == 0);


            //Ensure that all associated frustum are distincs between ranges
            foreach (var attractiveObjectFrustumIndex in attractiveObjectRangeToBufferLink.ConvertAll(l => l.FrustumIndex))
            {
                Assert.IsTrue(!aiSightRangeToBufferLink.ConvertAll(l => l.FrustumIndex).Contains(attractiveObjectFrustumIndex));
            }

            var groupedFrustums = attractiveObjectRangeToBufferLink.ConvertAll(l => l.FrustumIndex).GroupBy(i => i).Select(g => g).Where(g => g.Count() > 1).ToList();
            Assert.IsTrue(groupedFrustums.Count == 0);
            groupedFrustums = aiSightRangeToBufferLink.ConvertAll(l => l.FrustumIndex).GroupBy(i => i).Select(g => g).Where(g => g.Count() > 1).ToList();
            Assert.IsTrue(groupedFrustums.Count == 0);
        }

    }

}
