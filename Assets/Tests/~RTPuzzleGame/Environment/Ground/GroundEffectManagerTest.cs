using GameConfigurationID;
using RTPuzzle;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Tests
{
    public class GroundEffectManagerTest : AbstractPuzzleSceneTest
    {
        [UnityTest]
        public IEnumerator RangeBufferSentToShader()
        {
            AIObjectType aiObject = null;
            InteractiveObjectType attractiveObject = null;
            yield return this.Before("RTP_TEST_RangeEffectManager", () =>
            {
                aiObject = AIObjectDefinition.RangeEffectTestAI(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.AI_INITIAL_POSITION_1));
                attractiveObject = AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_2, 20.99f, 999999f).Instanciate(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.ATTRACTIVE_OBJECT_NOMINAL).position);
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

            Assert.IsTrue(GroundEffectsManagerV2.RangeRenderDatas.Values.SelectMany(s => s).ToList().Count == 2);

            Assert.IsFalse(true);
            //TODO

            //  var attractiveObjectRangerenderData = (CircleRangeRenderData)this.GetAbstractRangeRenderData(GroundEffectsManagerV2.RangeRenderDatas, attractiveObject.GetModule<AttractiveObjectModule>().SphereRange);

            //  Assert.IsTrue(attractiveObjectRangerenderData != null);
            //The attractive object is only occluded by one obstacle and only two faces are concerned -> 2 frustum
            //  Assert.IsTrue(attractiveObjectRangerenderData.ObstacleFrustumBuffer.GetSize() == 2);
            //  Assert.IsTrue(attractiveObjectRangerenderData.CircleRangeBuffer.GetSize() == 1);

            Assert.IsFalse(true);
            //TODO
            //var aiObjectSightRangeRenderData = (RoundedFrustumRenderData)this.GetAbstractRangeRenderData(GroundEffectsManagerV2.RangeRenderDatas, aiObject.GetComponent<InteractiveObjectType>().GetModule<ObjectSightModule>().SightVisionRange);

            //Assert.IsTrue(aiObjectSightRangeRenderData != null);
            //Debug.Log(aiObjectSightRangeRenderData.ObstacleFrustumBuffer.GetSize());
            //Assert.IsTrue(aiObjectSightRangeRenderData.ObstacleFrustumBuffer.GetSize() == 4);
            //Assert.IsTrue(aiObjectSightRangeRenderData.RoundedFrustumRangeBuffer.GetSize() == 1);
        }

        /*
        private AbstractRangeRenderData GetAbstractRangeRenderData(Dictionary<RangeTypeID, Dictionary<int, AbstractRangeRenderData>> rangeRenderDatas, RangeTypeObject rangeTypeObject)
        {
            rangeRenderDatas.TryGetValue(rangeTypeObject.RangeType.RangeTypeID, out Dictionary<int, AbstractRangeRenderData> rangeRenderDatasByInstance);
            if (rangeRenderDatasByInstance != null)
            {
                rangeRenderDatasByInstance.TryGetValue(rangeTypeObject.GetInstanceID(), out AbstractRangeRenderData AbstractRangeRenderData);
                return AbstractRangeRenderData;
            }
            return null;
        }
        */
    }
}
