using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    public class ContextActionEventManager : MonoBehaviour
    {
        #region External Dependencies      
        private ContextActionManager ContextActionManager;
        private PlayerManager PlayerManager;
        private InventoryManager InventoryManager;
        private TimelinesEventManager ScenarioTimelineEventManager;
        private AnimationConfiguration AnimationConfiguration;
        #endregion

        private Dictionary<AContextAction, List<IContextActionDyamicWorkflowListener>> dynamicContextActionListeners;


        private void Start()
        {
            #region External Dependencies    
            this.ContextActionManager = GameObject.FindObjectOfType<ContextActionManager>();
            this.PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            this.InventoryManager = GameObject.FindObjectOfType<InventoryManager>();
            this.ScenarioTimelineEventManager = GameObject.FindObjectOfType<TimelinesEventManager>();
            this.AnimationConfiguration = GameObject.FindObjectOfType<CoreConfigurationManager>().AnimationConfiguration();
            #endregion

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
                    ContextActionManager.OnAddAction(contextAction, ContextActionBuilder.BuildContextActionInput(contextAction, PlayerManager, this.AnimationConfiguration));
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