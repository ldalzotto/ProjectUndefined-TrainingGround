using CoreGame;
using InteractiveObjectTest;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Tests
{
    public class PhysicsPlayerMovementTest : AbstractPlayerTest
    {

        [UnityTest]
        public IEnumerator PlayerMovement_StickToGround_Test()
        {
            yield return this.Before();
            var playerRigidBody = PlayerInteractiveObjectManager.Get().GetPlayerGameObject().Rigidbody;
            var MinimumDistanceToStick = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject.PlayerInteractiveObjectInitializerData.MinimumDistanceToStick;
            yield return new WaitForFixedUpdate();
            playerRigidBody.position = playerRigidBody.position + (Vector3.up * MinimumDistanceToStick * 1.1f);
            yield return new WaitForFixedUpdate();
            var beforeStickPosition = playerRigidBody.transform.position;
            playerRigidBody.position = playerRigidBody.position + (Vector3.up * MinimumDistanceToStick * 1.1f);
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(beforeStickPosition, playerRigidBody.position);
        }

        [UnityTest]
        public IEnumerator PlayerSpeed_AdjustmentToSlope_Test()
        {
            float slopeAngle = -45f;
            yield return this.Before(PlayerMovementTestPositionId.ON_SLOPE);
            var playerRigidBody = PlayerInteractiveObjectManager.Get().GetPlayerGameObject().Rigidbody;
            var MinimumDistanceToStick = PlayerInteractiveObjectManager.Get().PlayerInteractiveObject.PlayerInteractiveObjectInitializerData.MinimumDistanceToStick;
            var playerCommonComponents = GameObject.FindObjectOfType<PlayerCommonComponents>();
            yield return new WaitForFixedUpdate();
            playerRigidBody.position = playerRigidBody.position + (Vector3.up * MinimumDistanceToStick * 1.1f);
            yield return new WaitForFixedUpdate();
            this.SetLocomotionAxisWorld(new Vector3(-1, 0, 0));
            yield return new WaitForFixedUpdate(); //take into account the input
            //rotation will take place at the end of physics step https://docs.unity3d.com/ScriptReference/Rigidbody-rotation.html
            yield return new WaitForFixedUpdate();
            Assert.AreApproximatelyEqual(
                PlayerInteractiveObjectManager.Get().PlayerInteractiveObject.PlayerInteractiveObjectInitializerData.SpeedMultiplicationFactor
                        , playerRigidBody.velocity.magnitude);
            Assert.AreApproximatelyEqual(Mathf.Sin(Mathf.Deg2Rad * slopeAngle), playerRigidBody.velocity.normalized.x);
            Assert.AreApproximatelyEqual(Mathf.Cos(Mathf.Deg2Rad * slopeAngle), playerRigidBody.velocity.normalized.y);
        }

    }

}