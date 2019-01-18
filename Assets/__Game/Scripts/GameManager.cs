using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private ContextActionManager ContextActionManager;
    private ContextActionWheelManager ContextActionWheelManager;
    private PlayerManager PlayerManager;
    private InventoryManager InventoryManager;
    private DiscussionWindowManager DiscussionWindowManager;

    //timelines
    private ScenarioTimelineManager ScenarioTimelineManager;
    private DiscussionTimelineManager DiscussionTimelineManager;

    void Start()
    {
        ContextActionManager = FindObjectOfType<ContextActionManager>();
        ContextActionWheelManager = FindObjectOfType<ContextActionWheelManager>();
        PlayerManager = FindObjectOfType<PlayerManager>();
        InventoryManager = FindObjectOfType<InventoryManager>();
        DiscussionWindowManager = FindObjectOfType<DiscussionWindowManager>();

        ScenarioTimelineManager = FindObjectOfType<ScenarioTimelineManager>();
        DiscussionTimelineManager = FindObjectOfType<DiscussionTimelineManager>();
        StartCoroutine(ScenarioTimelinesInitialisationAtEndOfFrame());
    }

    void Update()
    {
        var d = Time.deltaTime;

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
