using RTPuzzle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Tests.AbstractPuzzleSceneTest;

namespace Tests
{
    public abstract class AbstractPlayerTest
    {
        protected Vector3 currentLocomotionAxisWorld;

        protected IEnumerator Before(Nullable<PlayerMovementTestPositionId> playerMovementTestPositionId = null)
        {
            SceneManager.LoadScene("PlayerMovement", LoadSceneMode.Single);
            yield return new WaitForFixedUpdate();
            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
            var mockedInputManager = new MockedInputManager(() => { return cameraPivotPoint.transform.InverseTransformDirection(this.currentLocomotionAxisWorld); });
            var timeflowManager = GameObject.FindObjectOfType<TimeFlowManager>();
            timeflowManager.Init(
                    GameObject.FindObjectOfType<PlayerManagerDataRetriever>(),
                    GameObject.FindObjectOfType<PlayerManager>(),
                   mockedInputManager,
                    GameObject.FindObjectOfType<PuzzleGameConfigurationManager>(),
                    GameObject.FindObjectOfType<TimeFlowBarManager>(),
                    GameObject.FindObjectOfType<LevelManager>(),
                    GameObject.FindObjectOfType<PuzzleEventsManager>());
            var playerManager = GameObject.FindObjectOfType<PlayerManager>();
            playerManager.Init(mockedInputManager);

            if (playerMovementTestPositionId.HasValue)
            {
                GameObject.FindObjectOfType<PlayerManagerDataRetriever>().GetPlayerRigidBody()
                      .MovePosition(GameObject.FindObjectsOfType<PlayerMovementTestPosition>().Select((m) => m).Where((m) => m.PlayerMovementTestPositionId == playerMovementTestPositionId.Value).First().transform.position);
            }
            yield return null;
        }
    }

    class MockedInputManager : IGameInputManager
    {
        private Func<Vector3> locomotionAxisProvider;

        public MockedInputManager(Func<Vector3> locomotionAxisProvider)
        {
            this.locomotionAxisProvider = locomotionAxisProvider;
        }

        public XInput CurrentInput => new PlayerMovementInput(this.locomotionAxisProvider);
    }

    public class PlayerMovementInput : XInput
    {
        private Func<Vector3> locomotionAxisProvider;

        public PlayerMovementInput(Func<Vector3> locomotionAxisProvider)
        {
            this.locomotionAxisProvider = locomotionAxisProvider;
        }

        public bool ActionButtonD()
        {
            return false;
        }

        public bool ActionButtonDH()
        {
            return false;
        }

        public bool CancelButtonD()
        {
            return false;
        }

        public bool CancelButtonDH()
        {
            return false;
        }

        public bool InventoryButtonD()
        {
            return false;
        }

        public float LeftRotationCameraDH()
        {
            return 0f;
        }

        public Vector3 LocomotionAxis()
        {
            return locomotionAxisProvider.Invoke();
        }

        public float RightRotationCameraDH()
        {
            return 0f;
        }

        public bool TimeForwardButtonDH()
        {
            return true;
        }
    }

}
