using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ContextActionWheel : MonoBehaviour
{

    private const string ACTION_NODE_CONTAINER_OBJECT_NAME = "ActionNodesContainer";

    public Material ImageMaterial;
    public ActionWheelNodePositionManagerComponent ActionWheelNodePositionManagerComponent;

    private ActionWheelNodePositionManager ActionWheelNodePositionManager;
    private ContextActionWhelleEnterExitAnimationManager ContextActionWhelleEnterExitAnimationManager;
    private WheelActionNode[] wheelActionNodes;

    private void Start()
    {
        ContextActionWhelleEnterExitAnimationManager = new ContextActionWhelleEnterExitAnimationManager(GetComponent<Animator>());
    }

    public void Init(AContextAction[] wheelContextActions)
    {
        #region External Dependencies
        GameInputManager GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        #endregion

        ActionWheelNodePositionManager = new ActionWheelNodePositionManager(ActionWheelNodePositionManagerComponent, GameInputManager);
        wheelActionNodes = new WheelActionNode[wheelContextActions.Length];
        var actionNodeContainerObject = transform.Find(ACTION_NODE_CONTAINER_OBJECT_NAME);
        for (var i = 0; i < wheelContextActions.Length; i++)
        {
            var actionNode = WheelActionNode.Instantiate(wheelContextActions[i], ImageMaterial);
            actionNode.transform.SetParent(actionNodeContainerObject, false);
            wheelActionNodes[i] = actionNode;
        }
        ActionWheelNodePositionManager.InitNodes(wheelActionNodes);
        ContextActionWhelleEnterExitAnimationManager.Init();
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
                Destroy(transformToDestroy[i].gameObject);
            }
        }));


    }


    public void Tick(float d)
    {
        ActionWheelNodePositionManager.Tick(d, wheelActionNodes);
    }

    public AContextAction GetSelectedAction()
    {
        for (var i = 0; i < wheelActionNodes.Length; i++)
        {
            if (wheelActionNodes[i].TargetWheelAngleDeg % 360 == 0)
            {
                return wheelActionNodes[i].AssociatedContextAction;
            }
        }
        return null;
    }
}

#region Node position
class ActionWheelNodePositionManager
{
    private ActionWheelNodePositionManagerComponent ActionWheelNodePositionManagerComponent;
    private GameInputManager GameInputManager;

    private bool isRotating = false;

    public ActionWheelNodePositionManager(ActionWheelNodePositionManagerComponent actionWheelNodePositionManagerComponent, GameInputManager gameInputManager)
    {
        ActionWheelNodePositionManagerComponent = actionWheelNodePositionManagerComponent;
        GameInputManager = gameInputManager;
    }

    public void Tick(float d, WheelActionNode[] wheelActionNodes)
    {
        if (!isRotating && wheelActionNodes.Length > 1)
        {
            var joystickAxis = GameInputManager.CurrentInput.LocomotionAxis();
            if (joystickAxis.x >= 0.5)
            {
                isRotating = true;
                for (var i = 0; i < wheelActionNodes.Length; i++)
                {
                    wheelActionNodes[i].TargetWheelAngleDeg += (360 / wheelActionNodes.Length);
                }
            }
            else if (joystickAxis.x <= -0.5)
            {
                isRotating = true;
                for (var i = 0; i < wheelActionNodes.Length; i++)
                {
                    wheelActionNodes[i].TargetWheelAngleDeg -= (360 / wheelActionNodes.Length);
                }
            }
        }

        if (RepositionNodesSmooth(wheelActionNodes, d))
        {
            isRotating = false;
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
#endregion

class WheelActionNode : MonoBehaviour
{

    private float targetWheelAngleDeg;
    private float currentAngleDeg;

    private AContextAction associatedContextAction;

    public float TargetWheelAngleDeg { get => targetWheelAngleDeg; set => targetWheelAngleDeg = value; }
    public float CurrentAngleDeg { get => currentAngleDeg; set => currentAngleDeg = value; }
    public AContextAction AssociatedContextAction { get => associatedContextAction; set => associatedContextAction = value; }

    public static WheelActionNode Instantiate(AContextAction contextAction, Material imageMaterial)
    {
        var obj = new GameObject(contextAction.name + "_node_wheel");
        var wheelActionNode = obj.AddComponent<WheelActionNode>();
        var imageComponent = obj.AddComponent<Image>();
        imageComponent.sprite = ContextActionIconResolver.ResolveIcon(contextAction.GetType());
        imageComponent.material = imageMaterial;
        wheelActionNode.AssociatedContextAction = contextAction;
        return wheelActionNode;
    }
}

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