using GameConfigurationID;
using RTPuzzle;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Tests
{
    public class AIAgentAlgorithmTest : AbstractPuzzleSceneTest
    {

        [UnityTest]
        public IEnumerator OnDestinationReached_NewPathIsCalculatedTheSameFrame()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var oldAgentDestination = mouseTestAIManager.GetAgent().destination;
            var oldAgentNextPosition = mouseTestAIManager.GetAgent().destination;
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent());
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(oldAgentNextPosition != mouseTestAIManager.GetAgent().nextPosition);
        }

        [UnityTest]
        public IEnumerator OnNewDestination_WhenCurrentDestinationhasNotBeenReached_NewPathIsCalculatedTheSameFrame()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            yield return null;
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            PuzzleSceneTestHelper.SetAIEscapeSemiAngle(mouseTestAIManager.GetAIBehavior().AIComponents, 30f);
            var oldAgentDestination = mouseTestAIManager.GetAgent().destination;
            var oldAgentNextPosition = mouseTestAIManager.GetAgent().destination;
            PuzzleSceneTestHelper.ProjectileYield(PuzzleSceneTestHelper.CreateProjectileInherentData(9999f, 10f, LaunchProjectileId.TEST_PROJECTILE_EXPLODE), mouseTestAIManager.transform.position,
                OnProjectileSpawn: (InteractiveObjectType LaunchProjectile) =>
                {
                    Assert.IsTrue(oldAgentNextPosition != mouseTestAIManager.GetAgent().nextPosition);
                    return null;
                },
                OnDistanceReached: null);
        }

        [UnityTest]
        public IEnumerator OnDestinationReached_NewPathIsCalculatedTheSameFrame_ButPositionIsNotUpdated()
        {
            yield return this.Before(SceneConstants.OneAINoTargetZone);
            var mouseTestAIManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(AiID.MOUSE_TEST);
            var oldAgentDestination = mouseTestAIManager.GetAgent().destination;
            var oldAgentNextPosition = mouseTestAIManager.GetAgent().destination;
            TestHelperMethods.SetAgentDestinationPositionReached(mouseTestAIManager.GetAgent());
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(oldAgentDestination == mouseTestAIManager.GetAgent().transform.position);
        }
    }
}

