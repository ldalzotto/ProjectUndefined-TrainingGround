using UnityEngine;

public class ContextActionEventManager : MonoBehaviour
{

    private ContextActionManager ContextActionManager;
    private PlayerManager PlayerManager;
    private InventoryManager InventoryManager;
    private ScenarioTimelineManager ScenarioTimelineManager;

    private void Start()
    {
        ContextActionManager = GameObject.FindObjectOfType<ContextActionManager>();
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        InventoryManager = GameObject.FindObjectOfType<InventoryManager>();
        ScenarioTimelineManager = GameObject.FindObjectOfType<ScenarioTimelineManager>();
    }

    public void OnContextActionAdded(AContextAction contextAction, AContextActionInput contextActionInput)
    {
        try
        {
            PlayerManager.OnContextActionAdded();
            InventoryManager.OnContextActionAdded();
            ContextActionManager.OnAddAction(contextAction, contextActionInput);

            //TODO send event to inventory to close if necessary
        }
        catch (System.Exception e)
        {
            Debug.LogError("An error occured while trying to execute ActionContext : " + contextAction.GetType() + " : " + e.Message, this);
            Debug.LogError(e.GetBaseException().StackTrace);
        }

    }

    public void OnContextActionFinished(AContextAction finishedContextAction)
    {
        PlayerManager.OnContextActionFinished();
        InventoryManager.OnContextActionFinished();
        ScenarioTimelineManager.OnScenarioActionExecuted(ContextActionBuilder.BuilScenarioAction(finishedContextAction));
    }

}
