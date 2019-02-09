using UnityEngine;

public class LaunchProjectileRTPAction : RTPPlayerAction
{
    public override SelectionWheelNodeConfigurationId ActionWheelNodeConfigurationId => SelectionWheelNodeConfigurationId.THROW_PLAYER_PUZZLE_WHEEL_CONFIG;

    private LaunchProjectileScreenPositionManager LaunchProjectileScreenPositionManager;
    private LaunchProjectileRayPositionerManager LaunchProjectileRayPositionerManager;

    private bool isActionFinished = false;

    public override bool FinishedCondition()
    {
        return isActionFinished;
    }

    public override void FirstExecution()
    {
        isActionFinished = false;

        #region External Dependencies
        var gameInputMaganer = GameObject.FindObjectOfType<GameInputManager>();
        #endregion

        var configuration = GameObject.FindObjectOfType<RTPlayerActionConfigurationManager>();
        LaunchProjectileScreenPositionManager = new LaunchProjectileScreenPositionManager(configuration.LaunchProjectileScreenPositionManagerComponent, Vector2.zero, gameInputMaganer);
        LaunchProjectileRayPositionerManager = new LaunchProjectileRayPositionerManager(Camera.main, configuration.LaunchProjectileRayPositionerManagerComponent);
    }

    public override void Tick(float d)
    {
        LaunchProjectileScreenPositionManager.Tick(d);
        LaunchProjectileRayPositionerManager.Tick(d, LaunchProjectileScreenPositionManager.CurrentCursorScreenPosition);
    }

    public override void GizmoTick()
    {
    }

    public override void GUITick()
    {
        LaunchProjectileScreenPositionManager.GUITick();
    }
}

#region Screen Position Manager
class LaunchProjectileScreenPositionManager
{
    private LaunchProjectileScreenPositionManagerComponent LaunchProjectileScreenPositionManagerComponent;
    private GameInputManager GameInputManager;
    private Vector2 currentCursorScreenPosition;

    public LaunchProjectileScreenPositionManager(LaunchProjectileScreenPositionManagerComponent launchProjectileScreenPositionManagerComponent,
        Vector2 currentCursorScreenPosition, GameInputManager GameInputManager)
    {
        LaunchProjectileScreenPositionManagerComponent = launchProjectileScreenPositionManagerComponent;
        this.currentCursorScreenPosition = currentCursorScreenPosition;
        this.GameInputManager = GameInputManager;
        Debug.Log(currentCursorScreenPosition);
    }

    public Vector2 CurrentCursorScreenPosition { get => currentCursorScreenPosition; }

    public void Tick(float d)
    {
        var locomotionAxis = GameInputManager.CurrentInput.LocomotionAxis();
        currentCursorScreenPosition += (new Vector2(locomotionAxis.x, -locomotionAxis.z) * LaunchProjectileScreenPositionManagerComponent.CursorSpeed * d);
    }

    public void GUITick()
    {
        GUI.DrawTexture(new Rect(currentCursorScreenPosition - new Vector2(10, 10), new Vector2(20, 20)), LaunchProjectileScreenPositionManagerComponent.DebugTexture);
    }

}

[System.Serializable]
public class LaunchProjectileScreenPositionManagerComponent
{
    public float CursorSpeed;
    public Texture DebugTexture;
}
#endregion

#region Launch projectile ray manager
class LaunchProjectileRayPositionerManager
{
    private Camera camera;
    private LaunchProjectileRayPositionerManagerComponent LaunchProjectileRayPositionerManagerComponent;
    private GameObject currentCursor;

    public LaunchProjectileRayPositionerManager(Camera camera, LaunchProjectileRayPositionerManagerComponent launchProjectileRayPositionerManagerComponent)
    {
        this.camera = camera;
        LaunchProjectileRayPositionerManagerComponent = launchProjectileRayPositionerManagerComponent;
    }

    public void Tick(float d, Vector2 currentScreenPositionPoint)
    {
        Ray ray = camera.ScreenPointToRay(new Vector2(currentScreenPositionPoint.x, camera.pixelHeight - currentScreenPositionPoint.y));
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
            if (currentCursor == null)
            {
                currentCursor = MonoBehaviour.Instantiate(LaunchProjectileRayPositionerManagerComponent.ProjectileCursor);
            }

            currentCursor.transform.position = hit.point;
            currentCursor.transform.up = hit.normal;
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * Mathf.Infinity, Color.red);
            if (currentCursor != null)
            {
                MonoBehaviour.Destroy(currentCursor);
            }
        }
    }

}

[System.Serializable]
public class LaunchProjectileRayPositionerManagerComponent
{
    public GameObject ProjectileCursor;
}
#endregion