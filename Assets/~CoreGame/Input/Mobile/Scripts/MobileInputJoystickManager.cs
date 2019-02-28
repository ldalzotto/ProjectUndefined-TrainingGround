using UnityEngine;

[ExecuteInEditMode]
public class MobileInputJoystickManager : MonoBehaviour
{
    private static string JoystickTextureObjectName = "JoystickTexture";
    private bool isActive = false;



    public JoystickDimensionComponent JoystickDimensionComponent;

    private JoystickInputTrackerManager JoystickInputTrackerManager;
    private OuterJoystickTrackerManager OuterJoystickTrackerManager;
    private MobileInventoryInputManager MobileInventoryInputManager;

    private void Awake()
    {
        gameObject.FindChildObjectRecursively(JoystickTextureObjectName).SetActive(false);
#if UNITY_IOS
        this.isActive = true;
        gameObject.FindChildObjectRecursively(JoystickTextureObjectName).SetActive(true);
#endif
    }

    private void Start()
    {
        if (isActive)
        {
            #region External Dependencies
            InventoryMenu InventoryMenu = GameObject.FindObjectOfType<InventoryMenu>();
            #endregion

            var JoystickDimensionManager = new JoystickDimensionManager(JoystickDimensionComponent, (RectTransform)transform);
            this.JoystickInputTrackerManager = new JoystickInputTrackerManager((RectTransform)transform, JoystickDimensionComponent);
            OuterJoystickTrackerManager = new OuterJoystickTrackerManager((RectTransform)transform);
            MobileInventoryInputManager = new MobileInventoryInputManager(InventoryMenu);

            JoystickDimensionManager.Init();
        }
    }

    public void Tick(float d)
    {
        if (isActive)
        {
            JoystickInputTrackerManager.Tick(d);
            OuterJoystickTrackerManager.Tick(d);
            MobileInventoryInputManager.Tick(d);
        }
    }

    public Vector2 GetCurrentJoystickValue()
    {
        return JoystickInputTrackerManager.CurrentJoystickValue;
    }

    public Vector2 GetOuterJoystickDragDelta()
    {
        return OuterJoystickTrackerManager.CurrentDeltaPosition;
    }
    public bool InventoryHeadPressed()
    {
        return MobileInventoryInputManager.InventoryHeadPressed;
    }
}


#region Joystick Dimension
[System.Serializable]
public class JoystickDimensionComponent
{
    public float CircleRadius;
}

class JoystickDimensionManager
{
    private JoystickDimensionComponent JoystickDimensionComponent;
    private RectTransform JoystickTransform;

    public JoystickDimensionManager(JoystickDimensionComponent joystickDimensionComponent, RectTransform joystickTransform)
    {
        JoystickDimensionComponent = joystickDimensionComponent;
        JoystickTransform = joystickTransform;
    }

    public void Init()
    {
        JoystickTransform.sizeDelta = new Vector2(JoystickDimensionComponent.CircleRadius * 2, JoystickDimensionComponent.CircleRadius * 2);
    }

}
#endregion

#region Joystick Tracker
class JoystickInputTrackerManager
{

    private RectTransform JoystickCenterPoint;
    private JoystickDimensionComponent JoystickDimensionComponent;

    private Vector2 currentJoystickValue;

    public JoystickInputTrackerManager(RectTransform joystickCenterPoint, JoystickDimensionComponent JoystickDimensionComponent)
    {
        JoystickCenterPoint = joystickCenterPoint;
        this.JoystickDimensionComponent = JoystickDimensionComponent;
    }

    public Vector2 CurrentJoystickValue { get => currentJoystickValue; }

    public void Tick(float d)
    {
        for (var i = 0; i < Input.touches.Length; i++)
        {
            var touch = Input.touches[i];
            if (RectTransformUtility.RectangleContainsScreenPoint(JoystickCenterPoint, touch.position))
            {
                if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                {
                    var touchPosition = touch.position;
                    currentJoystickValue = new Vector2(
                        (touchPosition.x - JoystickCenterPoint.position.x) / JoystickDimensionComponent.CircleRadius,
                       (touchPosition.y - JoystickCenterPoint.position.y) / JoystickDimensionComponent.CircleRadius);
                }
                else
                {
                    currentJoystickValue = Vector2.zero;
                }
            }
        }
    }
}
#endregion

#region Outer Joystick Tracker
class OuterJoystickTrackerManager
{
    private RectTransform JoystickCenterPoint;

    public OuterJoystickTrackerManager(RectTransform joystickCenterPoint)
    {
        JoystickCenterPoint = joystickCenterPoint;
    }

    private Vector2 initialPosition;

    private Vector2 currentDeltaPosition;
    private Vector2 currentPosition;

    public Vector2 CurrentDeltaPosition { get => currentDeltaPosition; }

    public void Tick(float d)
    {
        if (Input.touches.Length == 1)
        {
            var touch = Input.touches[0];
            if (!RectTransformUtility.RectangleContainsScreenPoint(JoystickCenterPoint, touch.position))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    initialPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    currentPosition = touch.position;
                    currentDeltaPosition = currentPosition - initialPosition;
                }
                else
                {
                    currentDeltaPosition = Vector2.zero;
                }
            }
            else
            {
                currentDeltaPosition = Vector2.zero;
            }

        }
    }
}
#endregion

#region Iventory Input
class MobileInventoryInputManager
{
    private InventoryMenu InventoryMenu;

    private bool inventoryHeadPressed;

    public MobileInventoryInputManager(InventoryMenu InventoryMenu)
    {
        this.InventoryMenu = InventoryMenu;
    }

    public bool InventoryHeadPressed { get => inventoryHeadPressed; }

    public void Tick(float d)
    {
        if (Input.touches.Length == 1)
        {
            var touch = Input.touches[0];
            if (RectTransformUtility.RectangleContainsScreenPoint(InventoryMenu.GetInventoryHead(), touch.position))
            {
                Debug.Log("true");
                inventoryHeadPressed = true;
            }
            else
            {

                inventoryHeadPressed = false;
            }

        }
        else
        {
            inventoryHeadPressed = false;
        }
    }
}
#endregion