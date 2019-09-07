using CoreGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionWheel : MonoBehaviour
{

    private const string ACTION_NODE_CONTAINER_OBJECT_NAME = "ActionNodesContainer";

    public Material NonSelectedMaterial;
    public Material SelectedMaterial;

    public ActionWheelNodePositionManagerComponent ActionWheelNodePositionManagerComponent;

    private ActionWheelNodePositionManager ActionWheelNodePositionManager;
    private ActionWheelActiveNodeManager ActionWheelActiveNodeManager;
    private ContextActionWhelleEnterExitAnimationManager ContextActionWhelleEnterExitAnimationManager;
    private SelectionWheelNode[] wheelNodes;

    private void Start()
    {
        ContextActionWhelleEnterExitAnimationManager = new ContextActionWhelleEnterExitAnimationManager(GetComponent<Animator>());
        ActionWheelActiveNodeManager = new ActionWheelActiveNodeManager(NonSelectedMaterial, SelectedMaterial);
    }

    public void Init(List<SelectionWheelNodeData> wheelNodeDatas, WheelNodeSpriteResolver NodeSpriteResolver)
    {
        #region External Dependencies
        GameInputManager GameInputManager = CoreGameSingletonInstances.GameInputManager;
        #endregion

        ActionWheelNodePositionManager = new ActionWheelNodePositionManager(ActionWheelNodePositionManagerComponent, GameInputManager, ActionWheelActiveNodeManager);
        wheelNodes = new SelectionWheelNode[wheelNodeDatas.Count];
        var actionNodeContainerObject = transform.Find(ACTION_NODE_CONTAINER_OBJECT_NAME);
        for (var i = 0; i < wheelNodeDatas.Count; i++)
        {
            var wheelNode = SelectionWheelNode.Instantiate(wheelNodeDatas[i], NodeSpriteResolver);
            wheelNode.transform.SetParent(actionNodeContainerObject, false);
            wheelNodes[i] = wheelNode;
        }
        ActionWheelNodePositionManager.InitNodes(wheelNodes);
        ContextActionWhelleEnterExitAnimationManager.Init();
        ActionWheelActiveNodeManager.SelectedNodeChanged(wheelNodes);
    }

    public void Exit(bool destroyImmediate = false)
    {
        var actionNodeContainerObject = transform.Find(ACTION_NODE_CONTAINER_OBJECT_NAME);
        var transformToDestroy = new Transform[actionNodeContainerObject.childCount];
        for (var i = 0; i < actionNodeContainerObject.childCount; i++)
        {
            transformToDestroy[i] = actionNodeContainerObject.GetChild(i);
        }

        if (transformToDestroy != null && transformToDestroy.Length > 0)
        {
            if (destroyImmediate)
            {
                ContextActionWhelleEnterExitAnimationManager.PlayExitAnimation();
                for (var i = 0; i < transformToDestroy.Length; i++)
                {
                    if (transformToDestroy[i] != null)
                    {
                        Debug.Log(MyLog.Format("Destroy : " + transformToDestroy[i].gameObject.name));
                        Destroy(transformToDestroy[i].gameObject);
                    }
                }
            }
            else
            {
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
        }
    }


    public void Tick(float d)
    {
        ActionWheelNodePositionManager.Tick(d, wheelNodes);

        foreach (var wheelNode in wheelNodes)
        {
            wheelNode.Tick(d);
        }

    }


    public SelectionWheelNodeData GetSelectedNodeData()
    {
        if (ActionWheelActiveNodeManager.ActiveNode != null)
        {
            return ActionWheelActiveNodeManager.ActiveNode.WheelNodeData;
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

    public void Tick(float d, SelectionWheelNode[] wheelActionNodes)
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

    private bool RepositionNodesSmooth(SelectionWheelNode[] wheelActionNodes, float d)
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

    public void InitNodes(SelectionWheelNode[] wheelActionNodes)
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
    private SelectionWheelNode activeNode;

    public SelectionWheelNode ActiveNode { get => activeNode; }

    private Material nonSelectedMaterial;
    private Material selectedMaterial;

    public ActionWheelActiveNodeManager(Material nonSelectedMaterial, Material selectedMaterial)
    {
        this.nonSelectedMaterial = nonSelectedMaterial;
        this.selectedMaterial = selectedMaterial;
    }

    public void SelectedNodeChanged(SelectionWheelNode[] wheelActionNodes)
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

    public void PlayExitAnimation()
    {
        ContextActionWheelAnimator.Play("Exit");
    }

    public IEnumerator ExitCoroutine(Action afterAnimationEndCallback)
    {
        this.PlayExitAnimation();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfAnimation(ContextActionWheelAnimator, "Exit", 0);
        afterAnimationEndCallback.Invoke();
    }

}
#endregion