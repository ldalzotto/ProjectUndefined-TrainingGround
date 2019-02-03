using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //mobile input
    private MobileInputJoystickManager MobileInputJoystickManager;

    private ContextActionManager ContextActionManager;
    private ContextActionWheelManager ContextActionWheelManager;
    private PlayerManager PlayerManager;
    private NPCManager NPCManager;
    private InventoryManager InventoryManager;
    private DiscussionWindowManager DiscussionWindowManager;

    //timelines
    private ScenarioTimelineManagerV2 ScenarioTimelineManager;
    private DiscussionTimelineManagerV2 DiscussionTimelineManager;

    void Start()
    {
        Debug.Log("Start GameManager : " + name);
        // Debug.Break();
        MobileInputJoystickManager = FindObjectOfType<MobileInputJoystickManager>();

        ContextActionManager = FindObjectOfType<ContextActionManager>();
        ContextActionWheelManager = FindObjectOfType<ContextActionWheelManager>();
        PlayerManager = FindObjectOfType<PlayerManager>();
        NPCManager = FindObjectOfType<NPCManager>();
        InventoryManager = FindObjectOfType<InventoryManager>();
        DiscussionWindowManager = FindObjectOfType<DiscussionWindowManager>();

        ScenarioTimelineManager = FindObjectOfType<ScenarioTimelineManagerV2>();
        DiscussionTimelineManager = FindObjectOfType<DiscussionTimelineManagerV2>();

        //initialization
        PlayerManager.Init();
        FindObjectOfType<PointOfInterestPersistanceManager>().Init();
        StartCoroutine(ScenarioTimelinesInitialisationAtEndOfFrame());
        InventoryManager.Init();
        FindObjectOfType<InventoryEventManager>().Init();

        var WayPointPathContainer = FindObjectOfType<WayPointPathContainer>();
        if (WayPointPathContainer != null)
        {
            WayPointPathContainer.Init();
        }

    }

    void Update()
    {
        var d = Time.deltaTime;

        MobileInputJoystickManager.Tick(d);

        ContextActionWheelManager.Tick(d);
        ContextActionManager.Tick(d);
        PlayerManager.Tick(d);
        NPCManager.Tick(d);
        InventoryManager.Tick(d);
        DiscussionWindowManager.Tick(d);
    }

    private void FixedUpdate()
    {
        var d = Time.fixedDeltaTime;
        PlayerManager.FixedTick(d);
        NPCManager.FixedTick(d);
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
