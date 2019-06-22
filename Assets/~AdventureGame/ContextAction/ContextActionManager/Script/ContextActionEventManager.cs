using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    public class ContextActionEventManager : MonoBehaviour
    {

        private ContextActionManager ContextActionManager;
        private PlayerManager PlayerManager;
        private InventoryManager InventoryManager;

        private Dictionary<AContextAction, List<IContextActionDyamicWorkflowListener>> dynamicContextActionListeners;
        private TimelinesEventManager ScenarioTimelineEventManager;


        private void Start()
        {
            ContextActionManager = GameObject.FindObjectOfType<ContextActionManager>();
            PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            InventoryManager = GameObject.FindObjectOfType<InventoryManager>();
            ScenarioTimelineEventManager = GameObject.FindObjectOfType<TimelinesEventManager>();
            this.dynamicContextActionListeners = new Dictionary<AContextAction, List<IContextActionDyamicWorkflowListener>>();
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

        public void OnContextActionAdd(AContextAction contextAction, IContextActionDyamicWorkflowListener dynamicListener)
        {
            this.OnContextActionAdd(contextAction);
            if (!this.dynamicContextActionListeners.ContainsKey(contextAction))
            {
                this.dynamicContextActionListeners[contextAction] = new List<IContextActionDyamicWorkflowListener>();
            }
            this.dynamicContextActionListeners[contextAction].Add(dynamicListener);
        }

        public void OnContextActionFinished(AContextAction finishedContextAction, AContextActionInput contextActionInput)
        {
            PlayerManager.OnContextActionFinished();
            InventoryManager.OnContextActionFinished();
            ScenarioTimelineEventManager.OnScenarioActionExecuted(ContextActionBuilder.BuildScenarioAction(finishedContextAction, contextActionInput));
            if (this.dynamicContextActionListeners.ContainsKey(finishedContextAction))
            {
                foreach (var listener in this.dynamicContextActionListeners[finishedContextAction])
                {
                    listener.OnContextActionFinished(finishedContextAction);
                }
                this.dynamicContextActionListeners.Remove(finishedContextAction);
            }
        }

    }

    public interface IContextActionDyamicWorkflowListener
    {
        void OnContextActionFinished(AContextAction contextAction);
    }

}