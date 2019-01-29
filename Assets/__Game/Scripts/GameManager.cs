using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //mobile input
    private MobileInputJoystickManager MobileInputJoystickManager;

    private ContextActionManager ContextActionManager;
    private ContextActionWheelManager ContextActionWheelManager;
    private PlayerManager PlayerManager;
    private InventoryManager InventoryManager;
    private DiscussionWindowManager DiscussionWindowManager;

    //timelines
    private ScenarioTimelineManagerV2 ScenarioTimelineManager;
    private DiscussionTimelineManagerV2 DiscussionTimelineManager;

    void Start()
    {
        MobileInputJoystickManager = FindObjectOfType<MobileInputJoystickManager>();

        ContextActionManager = FindObjectOfType<ContextActionManager>();
        ContextActionWheelManager = FindObjectOfType<ContextActionWheelManager>();
        PlayerManager = FindObjectOfType<PlayerManager>();
        InventoryManager = FindObjectOfType<InventoryManager>();
        DiscussionWindowManager = FindObjectOfType<DiscussionWindowManager>();

        ScenarioTimelineManager = FindObjectOfType<ScenarioTimelineManagerV2>();
        DiscussionTimelineManager = FindObjectOfType<DiscussionTimelineManagerV2>();


        PlayerManager.Init();
        StartCoroutine(ScenarioTimelinesInitialisationAtEndOfFrame());
    }

    void Update()
    {
        var d = Time.deltaTime;

        MobileInputJoystickManager.Tick(d);

        ContextActionWheelManager.Tick(d);
        ContextActionManager.Tick(d);
        PlayerManager.Tick(d);
        InventoryManager.Tick(d);
        DiscussionWindowManager.Tick(d);
    }

    private void FixedUpdate()
    {
        var d = Time.fixedDeltaTime;
        PlayerManager.FixedTick(d);
    }

    private void LateUpdate()
    {
        var d = Time.deltaTime;
        PlayerManager.LateTick(d);
    }

    private void OnGUI()
    {
        DiscussionWindowManager.GUITick();
    }

    private void OnDrawGizmos()
    {
        if (PlayerManager != null)
        {
            PlayerManager.OnGizmoTick();
        }
    }

    private IEnumerator ScenarioTimelinesInitialisationAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        ScenarioTimelineManager.Init();
        DiscussionTimelineManager.Init();
    }
}
