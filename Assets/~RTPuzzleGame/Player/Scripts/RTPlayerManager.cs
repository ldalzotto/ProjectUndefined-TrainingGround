using UnityEngine;

public class RTPlayerManager : MonoBehaviour
{

    private CoreGame.PlayerInputMoveManager PlayerInputMoveManager;

    public void Init()
    {
        var playerRigidBody = GetComponent<Rigidbody>();
        var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();

        var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
        PlayerInputMoveManager = new CoreGame.PlayerInputMoveManager(cameraPivotPoint.transform, gameInputManager, playerRigidBody);
    }

    public void Tick(float d)
    {
        PlayerInputMoveManager.Tick(d, 1);
    }

    public void FixedTick(float d)
    {
        PlayerInputMoveManager.FixedTick(d, 1);
    }


}
