using GameConfigurationID;
using RTPuzzle;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Tests
{
    public class _1_Town_StartTutorial_AIBehaviorTest : AbstractPuzzleSceneTest
    {

        [UnityTest]
        public IEnumerator AI_MoveTowardPlayer_NominalTest()
        {
            yield return this.Before(SceneConstants._1_Level_StartTutorial_AIBehaviorTest, AiID._1_Town_StartTutorial_AITest);
            yield return new WaitForFixedUpdate();
            var aiManager = FindObjectOfType<NPCAIManagerContainer>().GetNPCAiManager(this.chosenId);
            var aiBehavior = (GenericPuzzleAIBehavior)aiManager.GetAIBehavior();
            Assert.IsFalse(aiBehavior.IsMovingTowardPlayer());
            Assert.IsTrue(aiBehavior.IsPatrolling());

            //The player move to ai sight

            var playerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            playerManagerDataRetriever.GetPlayerRigidBody().MovePosition(PuzzleSceneTestHelper.FindTestPosition(TestPositionID.MOVE_TOWARDS_PLAYER_INSIGHT).position);
            // yield return new WaitForSeconds(5f);
            yield return null;
            yield return new WaitForFixedUpdate();

            Assert.IsTrue(aiBehavior.IsMovingTowardPlayer());
            Assert.IsFalse(aiBehavior.IsPatrolling());
            Assert.AreEqual(aiBehavior.AIPlayerMoveTowardPlayerManager.GetCurrentTarget().collider, playerManagerDataRetriever.GetPlayerPuzzleLogicRootCollier()); 
        }

    }
}
