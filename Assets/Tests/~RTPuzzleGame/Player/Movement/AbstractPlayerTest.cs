using RTPuzzle;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests
{
    public abstract class AbstractPlayerTest
    {
        protected IEnumerator Before(Nullable<PlayerMovementTestPositionId> playerMovementTestPositionId = null)
        {
            SceneManager.LoadScene("PlayerMovement", LoadSceneMode.Single);
            yield return new WaitForFixedUpdate();
            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);

            //always timeline
            GameObject.FindObjectOfType<GameTestMockedInputManager>().GetGameTestMockedXInput().timeForwardButtonDH = true;

            if (playerMovementTestPositionId.HasValue)
            {
                GameObject.FindObjectOfType<PlayerManagerDataRetriever>().GetPlayerRigidBody()
                      .MovePosition(GameObject.FindObjectsOfType<PlayerMovementTestPosition>().Select((m) => m).Where((m) => m.PlayerMovementTestPositionId == playerMovementTestPositionId.Value).First().transform.position);
            }
            yield return null;
        }

        protected void SetLocomotionAxisWorld(Vector3 worldLocomotionAxis)
        {
            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
            GameObject.FindObjectOfType<GameTestMockedInputManager>().GetGameTestMockedXInput().locomotionAxis = cameraPivotPoint.transform.InverseTransformDirection(worldLocomotionAxis);
        }
    }
}
