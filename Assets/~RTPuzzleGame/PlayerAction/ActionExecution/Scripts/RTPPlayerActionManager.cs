using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class RTPPlayerActionManager : MonoBehaviour
    {
        public RTPPlayerSelectioNWheelPositionerComponent RTPPlayerSelectioNWheelPositionerComponent;

        private RTPPlayerActionExecutionManager RTPPlayerActionExecutionManager;
        private RTPPlayerActionsAvailableManager RTPPlayerActionsAvailableManager;
        private RTPPLayerSelectionWheelManager RTPPLayerSelectionWheelManager;
        private RTPPlayerSelectioNWheelPositioner RTPPlayerSelectioNWheelPositioner;

        private SelectionWheel SelectionWheel;

        public void Init(LevelZonesID puzzleId)
        {
            #region External Dependencies
            var RTPPlayerActionEventManager = GameObject.FindObjectOfType<RTPPlayerActionEventManager>();
            var RTPlayerManagerDataRetriever = GameObject.FindObjectOfType<RTPlayerManagerDataRetriever>();
            #endregion

            SelectionWheel = GameObject.FindObjectOfType<SelectionWheel>();

            RTPPlayerActionExecutionManager = new RTPPlayerActionExecutionManager(RTPPlayerActionEventManager);
            RTPPlayerActionsAvailableManager = new RTPPlayerActionsAvailableManager(puzzleId);
            RTPPLayerSelectionWheelManager = new RTPPLayerSelectionWheelManager(SelectionWheel);
            RTPPlayerSelectioNWheelPositioner = new RTPPlayerSelectioNWheelPositioner(RTPPlayerSelectioNWheelPositionerComponent, SelectionWheel, RTPlayerManagerDataRetriever.GetPlayerTransform(), Camera.main);
        }

        public void Tick(float d)
        {
            RTPPlayerActionExecutionManager.Tick(d);
            if (RTPPLayerSelectionWheelManager.WheelEnabled)
            {
                RTPPLayerSelectionWheelManager.Tick(d);
                RTPPlayerSelectioNWheelPositioner.Tick(d);
            }
        }

        public void GizmoTick()
        {
            RTPPlayerActionExecutionManager.GizmoTick();
        }

        public void GUITick()
        {
            RTPPlayerActionExecutionManager.GUITick();
        }

        #region External Events
        public void ExecuteAction(RTPPlayerAction rTPPlayerAction)
        {
            RTPPlayerActionExecutionManager.ExecuteAction(rTPPlayerAction);
        }
        internal void StopAction()
        {
            RTPPlayerActionExecutionManager.StopAction();
        }
        public void OnWheelAwake()
        {
            RTPPLayerSelectionWheelManager.OnWheelAwake(RTPPlayerActionsAvailableManager.CurrentAvailableActions);
        }
        public void OnWheelSleep()
        {
            RTPPLayerSelectionWheelManager.OnWheelSleep();
        }
        #endregion

        #region Logical Conditions
        public bool IsActionExecuting()
        {
            return RTPPlayerActionExecutionManager.IsActionExecuting;
        }
        public bool IsWheelEnabled()
        {
            return RTPPLayerSelectionWheelManager.WheelEnabled;
        }
        #endregion

        internal RTPPlayerAction GetCurrentSelectedAction()
        {
            return (SelectionWheel.GetSelectedNodeData() as RTPPlayerSelectionWheelNodeData).Data as RTPPlayerAction;
        }



    }


    #region Action execution
    class RTPPlayerActionExecutionManager
    {

        private RTPPlayerActionEventManager RTPPlayerActionEventManager;

        public RTPPlayerActionExecutionManager(RTPPlayerActionEventManager rTPPlayerActionEventManager)
        {
            RTPPlayerActionEventManager = rTPPlayerActionEventManager;
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
                    RTPPlayerActionEventManager.OnRTPPlayerActionStop(currentAction);
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

        public void ExecuteAction(RTPPlayerAction rTPPlayerAction)
        {
            currentAction = rTPPlayerAction;
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
    class RTPPlayerActionsAvailableManager
    {

        private List<RTPPlayerAction> currentAvailableActions;

        public RTPPlayerActionsAvailableManager(LevelZonesID puzzleId)
        {
            currentAvailableActions = RTPPlayerActionConfiguration.conf[puzzleId];
        }

        public List<RTPPlayerAction> CurrentAvailableActions { get => currentAvailableActions; }
    }
    #endregion

    #region Wheel Management
    class RTPPLayerSelectionWheelManager
    {
        private SelectionWheel SelectionWheel;

        public RTPPLayerSelectionWheelManager(SelectionWheel selectionWheel)
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
                availableActions.ConvertAll(rtpPlayerAction => new RTPPlayerSelectionWheelNodeData(rtpPlayerAction) as SelectionWheelNodeData),
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
    public class RTPPlayerSelectioNWheelPositionerComponent
    {
        public Vector3 DeltaDistanceFromTargetPoint;
    }

    class RTPPlayerSelectioNWheelPositioner
    {
        private RTPPlayerSelectioNWheelPositionerComponent RTPPlayerSelectioNWheelPositionerComponent;
        private SelectionWheel SelectionWheel;
        private Transform PlayerTransform;
        private Camera camera;

        public RTPPlayerSelectioNWheelPositioner(RTPPlayerSelectioNWheelPositionerComponent rTPPlayerSelectioNWheelPositionerComponent, SelectionWheel selectionWheel, Transform playerTransform, Camera camera)
        {
            RTPPlayerSelectioNWheelPositionerComponent = rTPPlayerSelectioNWheelPositionerComponent;
            SelectionWheel = selectionWheel;
            PlayerTransform = playerTransform;
            this.camera = camera;
        }

        public void Tick(float d)
        {
            SelectionWheel.transform.position = camera.WorldToScreenPoint(PlayerTransform.position) + RTPPlayerSelectioNWheelPositionerComponent.DeltaDistanceFromTargetPoint;
        }
    }
    #endregion

    #region RTPPlayer action wheel node data
    public class RTPPlayerSelectionWheelNodeData : SelectionWheelNodeData
    {

        private RTPPlayerAction playerAction;

        public RTPPlayerSelectionWheelNodeData(RTPPlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override object Data => playerAction;
    }
    #endregion
}

