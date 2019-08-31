using CoreGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    public class ContextActionWheelManager : MonoBehaviour
    {

        public WheelPositionManagerComponent WheelPositionManagerComponent;

        private ContextActionEventManager ContextActionEventManager;

        #region External dependecies
        private SelectionWheel ContextActionWheel;
        private PlayerManager PlayerManager;
        private ContextActionWheelEventManager ContextActionWheelEventManager;
        #endregion

        private WheelActivityManager WheelActivityManager;
        private WheelActionActivationManager WheelActionActivationManager;
        private WheelPositionManager WheelPositionManager;

        private void Start()
        {
            #region External Dependencies
            ContextActionEventManager = GameObject.FindObjectOfType<ContextActionEventManager>();
            ContextActionWheel = GameObject.FindObjectOfType<SelectionWheel>();
            PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            ContextActionWheelEventManager = GameObject.FindObjectOfType<ContextActionWheelEventManager>();
            GameInputManager GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            #endregion

            WheelActivityManager = new WheelActivityManager(GameInputManager);
            WheelActionActivationManager = new WheelActionActivationManager(GameInputManager, ContextActionWheel);
            WheelPositionManager = new WheelPositionManager(GameObject.FindGameObjectWithTag(TagConstants.PLAYER_TAG).transform, WheelPositionManagerComponent, ContextActionWheel.transform);
        }

        public void Tick(float d)
        {
            if (WheelActivityManager.IsEnabled)
            {

                WheelPositionManager.Tick();

                if (WheelActivityManager.TickCancelInput())
                {
                    ContextActionWheelEventManager.OnWheelDisabled();
                }
                else
                {
                    var triggeredContextAction = WheelActionActivationManager.Tick();
                    if (triggeredContextAction != null)
                    {
                        StartCoroutine(TriggerContextAction(triggeredContextAction));
                    }
                }

                ContextActionWheel.Tick(d);
            }
        }

        private IEnumerator TriggerContextAction(AContextAction contextAction)
        {
            yield return new WaitForEndOfFrame();
            ContextActionWheelEventManager.OnWheelDisabled();
            ContextActionEventManager.OnContextActionAdd(contextAction);
        }

        #region External Events
        public void OnAwakeWheel(List<AContextAction> contextActions, WheelTriggerSource wheelTriggerSource, PointOfInterestType currentTargetedPOI)
        {
            ContextActionWheel.Init(contextActions.ConvertAll(contextAction => new ContextActionSelectionWheelNodeData(contextAction) as SelectionWheelNodeData), ResolveWheelNodeSpriteFromNodeData);
            WheelActivityManager.AwakeWheel(wheelTriggerSource);
        }
        public void SleepWheel()
        {
            WheelActivityManager.SleepWheel();
        }
        #endregion

        #region Logical Conditions
        public bool IsWheelEnabled()
        {
            return this.WheelActivityManager.IsEnabled;
        }
        #endregion

        private Sprite ResolveWheelNodeSpriteFromNodeData(SelectionWheelNodeData selectionWheelNodeData)
        {
            return SelectionWheelNodeConfiguration.selectionWheelNodeConfiguration[(selectionWheelNodeData.Data as AContextAction).ContextActionWheelNodeConfigurationId].ContextActionWheelIcon;
        }
    }


    class WheelActionActivationManager
    {
        private GameInputManager GameInputManager;
        private SelectionWheel ContextActionWheel;

        public WheelActionActivationManager(GameInputManager gameInputManager, SelectionWheel contextActionWheel)
        {
            GameInputManager = gameInputManager;
            ContextActionWheel = contextActionWheel;
        }

        public AContextAction Tick()
        {
            if (GameInputManager.CurrentInput.ActionButtonD())
            {
                return ContextActionWheel.GetSelectedNodeData().Data as AContextAction;
            }
            return null;
        }
    }

    class WheelPositionManager
    {
        private Transform PlayerTransform;
        private WheelPositionManagerComponent WheelPositionManagerComponent;
        private Transform ContextActionWheelTransform;

        public WheelPositionManager(Transform playerTransform, WheelPositionManagerComponent wheelPositionManagerComponent, Transform contextActionWheelTransform)
        {
            PlayerTransform = playerTransform;
            WheelPositionManagerComponent = wheelPositionManagerComponent;
            ContextActionWheelTransform = contextActionWheelTransform;
        }

        public void Tick()
        {
            ContextActionWheelTransform.position = Camera.main.WorldToScreenPoint(PlayerTransform.position + new Vector3(0, WheelPositionManagerComponent.UpDistanceFromPlayer, 0));
        }
    }

    [System.Serializable]
    public class WheelPositionManagerComponent
    {
        public float UpDistanceFromPlayer;
    }

    #region wheel activity
    class WheelActivityManager
    {
        private bool isEnabled;
        private WheelTriggerSource currentWheelTriggerSource;

        private GameInputManager GameInputManager;

        public WheelActivityManager(GameInputManager gameInputManager)
        {
            GameInputManager = gameInputManager;
        }

        public bool IsEnabled { get => isEnabled; }

        public void AwakeWheel(WheelTriggerSource wheelTriggerSource)
        {
            isEnabled = true;
            currentWheelTriggerSource = wheelTriggerSource;
        }

        public void SleepWheel()
        {
            isEnabled = false;
        }

        public bool TickCancelInput()
        {
            return GameInputManager.CurrentInput.CancelButtonDH();
        }

    }

    public enum WheelTriggerSource
    {
        PLAYER, INVENTORY_MENU
    }
    #endregion
    
    #region Wheel node Context Action data
    public class ContextActionSelectionWheelNodeData : SelectionWheelNodeData
    {
        private AContextAction nodeContextAction;

        public ContextActionSelectionWheelNodeData(AContextAction nodeContextAction)
        {
            this.nodeContextAction = nodeContextAction;
        }

        public override object Data { get => nodeContextAction; }

        public override bool IsOnCoolDown => false;
        public override float GetRemainingCooldownTime => 0f;
        public override int GetRemainingExecutionAmount => -2;
        public override bool CanNodeBeExecuted => true;
    }
    #endregion
}