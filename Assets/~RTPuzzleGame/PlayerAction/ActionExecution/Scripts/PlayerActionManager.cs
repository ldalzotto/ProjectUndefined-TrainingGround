using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class PlayerActionManager : MonoBehaviour
    {
        public PlayerSelectioNWheelPositionerComponent PlayerSelectioNWheelPositionerComponent;

        private PlayerActionExecutionManager PlayerActionExecutionManager;
        private PlayerActionsAvailableManager PlayerActionsAvailableManager;
        private PLayerSelectionWheelManager PLayerSelectionWheelManager;
        private PlayerSelectioNWheelPositioner PlayerSelectioNWheelPositioner;

        private SelectionWheel SelectionWheel;

        public void Init(LevelZonesID puzzleId)
        {
            #region External Dependencies
            var PlayerActionEventManager = GameObject.FindObjectOfType<PlayerActionEventManager>();
            var PlayerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            #endregion

            SelectionWheel = GameObject.FindObjectOfType<SelectionWheel>();

            PlayerActionExecutionManager = new PlayerActionExecutionManager(PlayerActionEventManager);
            PlayerActionsAvailableManager = new PlayerActionsAvailableManager(puzzleId);
            PLayerSelectionWheelManager = new PLayerSelectionWheelManager(SelectionWheel);
            PlayerSelectioNWheelPositioner = new PlayerSelectioNWheelPositioner(PlayerSelectioNWheelPositionerComponent, SelectionWheel, PlayerManagerDataRetriever.GetPlayerTransform(), Camera.main);
        }

        public void Tick(float d)
        {
            PlayerActionExecutionManager.Tick(d);
            if (PLayerSelectionWheelManager.WheelEnabled)
            {
                PLayerSelectionWheelManager.Tick(d);
                PlayerSelectioNWheelPositioner.Tick(d);
            }
        }

        public void GizmoTick()
        {
            PlayerActionExecutionManager.GizmoTick();
        }

        public void GUITick()
        {
            PlayerActionExecutionManager.GUITick();
        }

        #region External Events
        public void ExecuteAction(RTPPlayerAction rTPPlayerAction)
        {
            PlayerActionExecutionManager.ExecuteAction(rTPPlayerAction);
        }
        internal void StopAction()
        {
            PlayerActionExecutionManager.StopAction();
        }
        public void OnWheelAwake()
        {
            PLayerSelectionWheelManager.OnWheelAwake(PlayerActionsAvailableManager.CurrentAvailableActions);
        }
        public void OnWheelSleep()
        {
            PLayerSelectionWheelManager.OnWheelSleep();
        }
        #endregion

        #region Logical Conditions
        public bool IsActionExecuting()
        {
            return PlayerActionExecutionManager.IsActionExecuting;
        }
        public bool IsWheelEnabled()
        {
            return PLayerSelectionWheelManager.WheelEnabled;
        }
        #endregion

        internal RTPPlayerAction GetCurrentSelectedAction()
        {
            return (SelectionWheel.GetSelectedNodeData() as PlayerSelectionWheelNodeData).Data as RTPPlayerAction;
        }



    }


    #region Action execution
    class PlayerActionExecutionManager
    {

        private PlayerActionEventManager PlayerActionEventManager;

        public PlayerActionExecutionManager(PlayerActionEventManager PlayerActionEventManager)
        {
            this.PlayerActionEventManager = PlayerActionEventManager;
        }

        private RTPPlayerAction currentAction;
        private bool isActionExecuting;

        public bool IsActionExecuting { get => isActionExecuting; }

        public void Tick(float d)
        {
            if (currentAction != null)
            {
                if (currentAction.FinishedCondition())
                {
                    PlayerActionEventManager.OnRTPPlayerActionStop(currentAction);
                }
                else
                {
                    currentAction.Tick(d);
                }
            }
        }

        public void GizmoTick()
        {
            if (currentAction != null)
            { currentAction.GizmoTick(); }
        }

        public void GUITick()
        {
            if (currentAction != null)
            {
                currentAction.GUITick();
            }
        }

        public void ExecuteAction(RTPPlayerAction PlayerAction)
        {
            currentAction = PlayerAction;
            isActionExecuting = true;
            currentAction.FirstExecution();
        }

        internal void StopAction()
        {
            currentAction = null;
            isActionExecuting = false;
        }
    }
    #endregion

    #region RTPPlayer actions availability
    class PlayerActionsAvailableManager
    {

        private List<RTPPlayerAction> currentAvailableActions;

        public PlayerActionsAvailableManager(LevelZonesID puzzleId)
        {
            currentAvailableActions = PlayerActionConfiguration.conf[puzzleId];
        }

        public List<RTPPlayerAction> CurrentAvailableActions { get => currentAvailableActions; }
    }
    #endregion

    #region Wheel Management
    class PLayerSelectionWheelManager
    {
        private SelectionWheel SelectionWheel;

        public PLayerSelectionWheelManager(SelectionWheel selectionWheel)
        {
            SelectionWheel = selectionWheel;
        }

        private bool wheelEnabled;

        public bool WheelEnabled { get => wheelEnabled; }

        public void Tick(float d)
        {
            SelectionWheel.Tick(d);
        }

        public void OnWheelAwake(List<RTPPlayerAction> availableActions)
        {
            SelectionWheel.Init(
                availableActions.ConvertAll(rtpPlayerAction => new PlayerSelectionWheelNodeData(rtpPlayerAction) as SelectionWheelNodeData),
                ResolveWheelNodeSpriteFromNodeData
            );
            wheelEnabled = true;
        }
        public void OnWheelSleep()
        {
            SelectionWheel.Exit();
            wheelEnabled = false;
        }

        private Sprite ResolveWheelNodeSpriteFromNodeData(SelectionWheelNodeData selectionWheelNodeData)
        {
            return SelectionWheelNodeConfiguration.selectionWheelNodeConfiguration[(selectionWheelNodeData.Data as RTPPlayerAction).ActionWheelNodeConfigurationId].ContextActionWheelIcon;
        }
    }

    [System.Serializable]
    public class PlayerSelectioNWheelPositionerComponent
    {
        public Vector3 DeltaDistanceFromTargetPoint;
    }

    class PlayerSelectioNWheelPositioner
    {
        private PlayerSelectioNWheelPositionerComponent PlayerSelectioNWheelPositionerComponent;
        private SelectionWheel SelectionWheel;
        private Transform PlayerTransform;
        private Camera camera;

        public PlayerSelectioNWheelPositioner(PlayerSelectioNWheelPositionerComponent PlayerSelectioNWheelPositionerComponent, SelectionWheel selectionWheel, Transform playerTransform, Camera camera)
        {
            this.PlayerSelectioNWheelPositionerComponent = PlayerSelectioNWheelPositionerComponent;
            SelectionWheel = selectionWheel;
            PlayerTransform = playerTransform;
            this.camera = camera;
        }

        public void Tick(float d)
        {
            SelectionWheel.transform.position = camera.WorldToScreenPoint(PlayerTransform.position) + PlayerSelectioNWheelPositionerComponent.DeltaDistanceFromTargetPoint;
        }
    }
    #endregion

    #region RTPPlayer action wheel node data
    public class PlayerSelectionWheelNodeData : SelectionWheelNodeData
    {

        private RTPPlayerAction playerAction;

        public PlayerSelectionWheelNodeData(RTPPlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override object Data => playerAction;
    }
    #endregion
}

