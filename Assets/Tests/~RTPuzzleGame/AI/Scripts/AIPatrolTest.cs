using GameConfigurationID;
using RTPuzzle;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Tests
{
    public class AIPatrolTest : AbstractPuzzleSceneTest
    {
        [UnityTest]
        public IEnumerator AI_WhenPatrolGraphIsRunning_IsPatrolling()
        {
            AIPositionMarker position1 = null;
            AIPositionMarker position2 = null;
            AIObjectType aiObject = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                position1 = new AIPositionMarkerInitialization(new Vector3(5, 0, 5), Quaternion.identity, AIPositionMarkerID.TEST_1).InstanciateInScene();
                position2 = new AIPositionMarkerInitialization(new Vector3(10, 0, 10), Quaternion.identity, AIPositionMarkerID.TEST_2).InstanciateInScene();
                aiObject = AIObjectDefinition.AIPathTest(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1, false, AIObjectDefinition.TwoDestinationGraph(AIPositionMarkerID.TEST_1, AIPositionMarkerID.TEST_2))
                    .Instanciate(Vector3.zero);
            });
            var initialAIObjectPosition = aiObject.GetAgent().transform.position;

            yield return new WaitForFixedUpdate();

            var agent = aiObject.GetAgent();
            var AIBehavior = (GenericPuzzleAIBehavior)aiObject.GetAIBehavior();
            Assert.IsTrue(AIBehavior.IsPatrolling());
            Assert.IsTrue(agent.destination - initialAIObjectPosition == position1.transform.position);
        }

        [UnityTest]
        public IEnumerator AI_WhenPatrolGraphNodeHasEnded_IsPatrollingToNextNode()
        {
            AIPositionMarker position1 = null;
            AIPositionMarker position2 = null;
            AIObjectType aiObject = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                position1 = new AIPositionMarkerInitialization(new Vector3(5, 0, 5), Quaternion.identity, AIPositionMarkerID.TEST_1).InstanciateInScene();
                position2 = new AIPositionMarkerInitialization(new Vector3(10, 0, 10), Quaternion.identity, AIPositionMarkerID.TEST_2).InstanciateInScene();
                aiObject = AIObjectDefinition.AIPathTest(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1, false, AIObjectDefinition.TwoDestinationGraph(AIPositionMarkerID.TEST_1, AIPositionMarkerID.TEST_2))
                    .Instanciate(Vector3.zero);
            });
            var initialAIObjectPosition = aiObject.GetAgent().transform.position;

            yield return new WaitForFixedUpdate();

            var agent = aiObject.GetAgent();
            var AIBehavior = (GenericPuzzleAIBehavior)aiObject.GetAIBehavior();

            TestHelperMethods.SetAgentDestinationPositionReached(agent);
            yield return null;

            Assert.IsTrue(AIBehavior.IsPatrolling());
            Assert.IsTrue(agent.destination - initialAIObjectPosition == position2.transform.position);
        }

        [UnityTest]
        public IEnumerator AI_WhenPatrolGraphIsInterrupted_KeepTargetNodeInMemory()
        {
            AIPositionMarker position1 = null;
            AIPositionMarker position2 = null;
            AIObjectType aiObject = null;
            yield return this.Before(SceneConstants.OneAINoTargetZone, () =>
            {
                position1 = new AIPositionMarkerInitialization(new Vector3(5, 0, 5), Quaternion.identity, AIPositionMarkerID.TEST_1).InstanciateInScene();
                position2 = new AIPositionMarkerInitialization(new Vector3(10, 0, 10), Quaternion.identity, AIPositionMarkerID.TEST_2).InstanciateInScene();
                aiObject = AIObjectDefinition.AIPathTest(AIObjectTestID.TEST_1, InteractiveObjectTestID.TEST_1, hasAttractive: true, AIObjectDefinition.TwoDestinationGraph(AIPositionMarkerID.TEST_1, AIPositionMarkerID.TEST_2))
                    .Instanciate(Vector3.zero);
            });
            var initialAIObjectPosition = aiObject.GetAgent().transform.position;

            yield return new WaitForFixedUpdate();

            var InteractiveObjectContainer = GameObject.FindObjectOfType<InteractiveObjectContainer>();

            var agent = aiObject.GetAgent();
            var AIBehavior = (GenericPuzzleAIBehavior)aiObject.GetAIBehavior();

            TestHelperMethods.SetAgentDestinationPositionReached(agent);
            yield return null;

            yield return PuzzleSceneTestHelper.AttractiveObjectYield(AttractiveObjectDefinition.AttractiveObjectOnly(InteractiveObjectTestID.TEST_2, 9999f, 0.1f), Vector3.zero,
               OnAttractiveObjectSpawn: (o) =>
              {
                  Assert.IsTrue(AIBehavior.IsInfluencedByAttractiveObject());
                  return null;
              },
                OnAttractiveObjectDestroyed: () =>
                {
                    Debug.Log("TEST OnAttractiveObjectDestroyed");
                    Assert.IsTrue(AIBehavior.IsPatrolling());
                    Assert.IsTrue(agent.destination - initialAIObjectPosition == position2.transform.position);
                    return null;
                });

        }
    }
}