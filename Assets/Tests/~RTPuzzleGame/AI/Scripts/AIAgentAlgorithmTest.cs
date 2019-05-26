using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;
using RTPuzzle;
using UnityEngine.Assertions;

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

