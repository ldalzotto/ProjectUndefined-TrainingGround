using CoreGame;
using UnityEngine;

namespace AdventureGame
{

    public class ContextActionEventManager : MonoBehaviour
    {

        private ContextActionManager ContextActionManager;
        private PlayerManager PlayerManager;
        private InventoryManager InventoryManager;
        private TimelinesEventManager ScenarioTimelineEventManager;

        private void Start()
        {
            ContextActionManager = GameObject.FindObjectOfType<ContextActionManager>();
            PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            InventoryManager = GameObject.FindObjectOfType<InventoryManager>();
            ScenarioTimelineEventManager = GameObject.FindObjectOfType<TimelinesEventManager>();
        }

        public void OnContextActionAdd(AContextAction contextAction)
        {
            try
            {
                if (contextAction != null)
                {
                    PlayerManager.OnContextActionAdded(contextAction);
                    InventoryManager.OnContextActionAdded();
                    ContextActionManager.OnAddAction(contextAction, ContextActionBuilder.BuildContextActionInput(contextAction, PlayerManager));
                }
                //TODO send event to inventory to close if necessary
            }
            catch (System.Exception e)
            {
                Debug.LogError("An error occured while trying to execute ActionContext : " + contextAction.GetType() + " : " + e.Message, this);
                Debug.LogError(e.GetBaseException().StackTrace);
            }

        }

        public void OnContextActionFinished(AContextAction finishedContextAction, AContextActionInput contextActionInput)
        {
            PlayerManager.OnContextActionFinished();
            InventoryManager.OnContextActionFinished();
            ScenarioTimelineEventManager.OnScenarioActionExecuted(ContextActionBuilder.BuildScenarioAction(finishedContextAction, contextActionInput));
        }

    }

}