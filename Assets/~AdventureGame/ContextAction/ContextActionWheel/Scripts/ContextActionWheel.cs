using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextActionWheel : MonoBehaviour
{

    private const string ACTION_NODE_CONTAINER_OBJECT_NAME = "ActionNodesContainer";

    public Material NonSelectedMaterial;
    public Material SelectedMaterial;

    public ActionWheelNodePositionManagerComponent ActionWheelNodePositionManagerComponent;

    private ActionWheelNodePositionManager ActionWheelNodePositionManager;
    private ActionWheelActiveNodeManager ActionWheelActiveNodeManager;
    private ContextActionWhelleEnterExitAnimationManager ContextActionWhelleEnterExitAnimationManager;
    private WheelActionNode[] wheelActionNodes;

    private void Start()
    {
        ContextActionWhelleEnterExitAnimationManager = new ContextActionWhelleEnterExitAnimationManager(GetComponent<Animator>());
        ActionWheelActiveNodeManager = new ActionWheelActiveNodeManager(NonSelectedMaterial, SelectedMaterial);
    }

    public void Init(List<AContextAction> wheelContextActions)
    {
        #region External Dependencies
        GameInputManager GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        #endregion

        ActionWheelNodePositionManager = new ActionWheelNodePositionManager(ActionWheelNodePositionManagerComponent, GameInputManager, ActionWheelActiveNodeManager);
        wheelActionNodes = new WheelActionNode[wheelContextActions.Count];
        var actionNodeContainerObject = transform.Find(ACTION_NODE_CONTAINER_OBJECT_NAME);
        for (var i = 0; i < wheelContextActions.Count; i++)
        {
            var actionNode = WheelActionNode.Instantiate(wheelContextActions[i]);
            actionNode.transform.SetParent(actionNodeContainerObject, false);
            wheelActionNodes[i] = actionNode;
        }
        ActionWheelNodePositionManager.InitNodes(wheelActionNodes);
        ContextActionWhelleEnterExitAnimationManager.Init();
        ActionWheelActiveNodeManager.SelectedNodeChanged(wheelActionNodes);
    }

    public void Exit()
    {
        var actionNodeContainerObject = transform.Find(ACTION_NODE_CONTAINER_OBJECT_NAME);
        var transformToDestroy = new Transform[actionNodeContainerObject.childCount];
        for (var i = 0; i < actionNodeContainerObject.childCount; i++)
        {
            transformToDestroy[i] = actionNodeContainerObject.GetChild(i);
        }

        StartCoroutine(ContextActionWhelleEnterExitAnimationManager.ExitCoroutine(() =>
        {
            for (var i = 0; i < transformToDestroy.Length; i++)
            {
                if (transformToDestroy[i] != null)
                {
                    Destroy(transformToDestroy[i].gameObject);
                }
            }
        }));


    }


    public void Tick(float d)
    {
        ActionWheelNodePositionManager.Tick(d, wheelActionNodes);
    }


    public AContextAction GetSelectedAction()
    {
        if (ActionWheelActiveNodeManager.ActiveNode != null)
        {
            return ActionWheelActiveNodeManager.ActiveNode.AssociatedContextAction;
        }
        return null;

    }
}

#region Node position
class ActionWheelNodePositionManager
{
    private ActionWheelNodePositionManagerComponent ActionWheelNodePositionManagerComponent;
    private GameInputManager GameInputManager;
    private ActionWheelActiveNodeManager ActionWheelActiveNodeManager;

    private bool isRotating = false;

    public ActionWheelNodePositionManager(ActionWheelNodePositionManagerComponent actionWheelNodePositionManagerComponent, GameInputManager gameInputManager, ActionWheelActiveNodeManager actionWheelActiveNodeManager)
    {
        ActionWheelNodePositionManagerComponent = actionWheelNodePositionManagerComponent;
        GameInputManager = gameInputManager;
        ActionWheelActiveNodeManager = actionWheelActiveNodeManager;
    }

    public void Tick(float d, WheelActionNode[] wheelActionNodes)
    {
        if (wheelActionNodes.Length > 1)
        {
            if (!isRotating)
            {
                var joystickAxis = GameInputManager.CurrentInput.LocomotionAxis();
                if (joystickAxis.x >= 0.5)
                {
                    isRotating = true;
                    for (var i = 0; i < wheelActionNodes.Length; i++)
                    {
                        wheelActionNodes[i].TargetWheelAngleDeg += (360 / wheelActionNodes.Length);
                    }
                    ActionWheelActiveNodeManager.SelectedNodeChanged(wheelActionNodes);
                }
                else if (joystickAxis.x <= -0.5)
                {
                    isRotating = true;
                    for (var i = 0; i < wheelActionNodes.Length; i++)
                    {
                        wheelActionNodes[i].TargetWheelAngleDeg -= (360 / wheelActionNodes.Length);
                    }
                    ActionWheelActiveNodeManager.SelectedNodeChanged(wheelActionNodes);
                }
            }

            if (RepositionNodesSmooth(wheelActionNodes, d))
            {
                isRotating = false;
            }
        }


    }

    private bool RepositionNodesSmooth(WheelActionNode[] wheelActionNodes, float d)
    {
        for (var i = 0; i < wheelActionNodes.Length; i++)
        {
            wheelActionNodes[i].CurrentAngleDeg = Mathf.Lerp(wheelActionNodes[i].CurrentAngleDeg, wheelActionNodes[i].TargetWheelAngleDeg, d * ActionWheelNodePositionManagerComponent.RotationSpeed);
            Vector3 nodePosition = Vector3.up;
            nodePosition = Quaternion.Euler(0, 0, wheelActionNodes[i].CurrentAngleDeg) * nodePosition;
            nodePosition *= ActionWheelNodePositionManagerComponent.DistanceFromCenter;
            wheelActionNodes[i].transform.localPosition = nodePosition;
        }

        if (Mathf.Abs(wheelActionNodes[0].TargetWheelAngleDeg - wheelActionNodes[0].CurrentAngleDeg) < 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void InitNodes(WheelActionNode[] wheelActionNodes)
    {
        for (var i = 0; i < wheelActionNodes.Length; i++)
        {
            wheelActionNodes[i].TargetWheelAngleDeg = (360 / wheelActionNodes.Length) * i;
            wheelActionNodes[i].CurrentAngleDeg = wheelActionNodes[i].TargetWheelAngleDeg;
            Vector3 nodePosition = Vector3.up;
            nodePosition = Quaternion.Euler(0, 0, wheelActionNodes[i].TargetWheelAngleDeg) * nodePosition;
            nodePosition *= ActionWheelNodePositionManagerComponent.DistanceFromCenter;
            wheelActionNodes[i].transform.localPosition = nodePosition;
        }

    }
}

[System.Serializable]
public class ActionWheelNodePositionManagerComponent
{
    public float DistanceFromCenter;
    public float RotationSpeed;
}

class ActionWheelActiveNodeManager
{
    private WheelActionNode activeNode;

    public WheelActionNode ActiveNode { get => activeNode; }

    private Material nonSelectedMaterial;
    private Material selectedMaterial;

    public ActionWheelActiveNodeManager(Material nonSelectedMaterial, Material selectedMaterial)
    {
        this.nonSelectedMaterial = nonSelectedMaterial;
        this.selectedMaterial = selectedMaterial;
    }

    public void SelectedNodeChanged(WheelActionNode[] wheelActionNodes)
    {
        if (activeNode != null)
        {
            activeNode.SetMaterial(nonSelectedMaterial);
        }

        for (var i = 0; i < wheelActionNodes.Length; i++)
        {
            if (wheelActionNodes[i].TargetWheelAngleDeg % 360 == 0)
            {
                activeNode = wheelActionNodes[i];
                activeNode.SetMaterial(selectedMaterial);
                return;
            }
        }

    }
}
#endregion

#region Wheel Enter/Exit animations
class ContextActionWhelleEnterExitAnimationManager
{
    private Animator ContextActionWheelAnimator;

    public ContextActionWhelleEnterExitAnimationManager(Animator contextActionWHeelAnimator)
    {
        ContextActionWheelAnimator = contextActionWHeelAnimator;
    }

    public void Init()
    {
        ContextActionWheelAnimator.Play("Enter");
    }

    public IEnumerator ExitCoroutine(Action afterAnimationEndCallback)
    {
        ContextActionWheelAnimator.Play("Exit");
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfAnimation(ContextActionWheelAnimator, "Exit", 0);
        afterAnimationEndCallback.Invoke();
    }

}
#endregion