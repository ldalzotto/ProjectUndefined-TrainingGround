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
            this.ContextActionManager = AdventureGameSingletonInstances.ContextActionManager;
            this.PlayerManager = AdventureGameSingletonInstances.PlayerManager;
            this.InventoryManager = AdventureGameSingletonInstances.InventoryManager;
            this.ScenarioTimelineEventManager = CoreGameSingletonInstances.TimelinesEventManager;
            this.AnimationConfiguration = CoreGameSingletonInstances.CoreConfigurationManager.AnimationConfiguration();
            #endregion

            this.dynamicContextActionListeners = new Dictionary<AContextAction, List<IContextActionDyamicWorkflowListener>>();
        }

        public void OnContextActionAdd(SequencedAction contextAction)
        {
            try
            {
                if (contextAction != null)
                {
                    PlayerManager.OnContextActionAdded((AContextAction)contextAction);
                    InventoryManager.OnContextActionAdded();
                    ContextActionManager.OnAddAction(contextAction, ContextActionBuilder.BuildContextActionInput((AContextAction)contextAction, PlayerManager, this.AnimationConfiguration));
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

        public void OnContextActionFinished(SequencedAction finishedContextAction, SequencedActionInput contextActionInput)
        {
            AContextAction finishedContextActionCasted = (AContextAction) finishedContextAction;
            AContextActionInput contextActionInputCasted = (AContextActionInput)contextActionInput;

            PlayerManager.OnContextActionFinished();
            InventoryManager.OnContextActionFinished();
            ScenarioTimelineEventManager.OnScenarioActionExecuted(ContextActionBuilder.BuildScenarioAction(finishedContextActionCasted, contextActionInputCasted));
            if (this.dynamicContextActionListeners.ContainsKey(finishedContextActionCasted))
            {
                foreach (var listener in this.dynamicContextActionListeners[finishedContextActionCasted])
                {
                    listener.OnContextActionFinished(finishedContextActionCasted);
                }
                this.dynamicContextActionListeners.Remove(finishedContextActionCasted);
            }
        }

    }

    public interface IContextActionDyamicWorkflowListener
    {
        void OnContextActionFinished(AContextAction contextAction);
    }

}