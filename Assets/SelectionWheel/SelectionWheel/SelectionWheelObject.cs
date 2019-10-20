using System;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using UnityEngine;

namespace SelectionWheel
{
    public class SelectionWheelObject : GameSingleton<SelectionWheelObject>
    {
        private const string ACTION_NODE_CONTAINER_OBJECT_NAME = "ActionNodesContainer";
        private ActionWheelActiveNodeManager ActionWheelActiveNodeManager;
        private ActionWheelNodePositionManager ActionWheelNodePositionManager;
        private ContextActionWhelleEnterExitAnimationManager ContextActionWhelleEnterExitAnimationManager;

        private SelectionWheelPositionManager SelectionWheelPositionManager;
        private SelectionWheelNode[] wheelNodes;
        public GameObject SelectionWheelGameObject { get; private set; }

        #region State

        public bool IsWheelEnabled { get; private set; }

        #endregion

        public void Init()
        {
            #region Event Registering

            SelectionWheelEventsManager.Get().RegisterOnWheelAwakeEventListener(AwakeWheel);
            SelectionWheelEventsManager.Get().RegisterOnWheelSleepEventListener(SleepWheel);
            SelectionWheelEventsManager.Get().RegisterOnWheelRefreshEvent(RefreshWheel);

            #endregion
        }

        private void AwakeWheel(List<SelectionWheelNodeData> wheelNodeDatas, Transform followingTransform)
        {
            if (!IsWheelEnabled)
            {
                IsWheelEnabled = true;

                var SelectionWheelGlobalConfiguration = SelectionWheelGlobalConfigurationGameObject.Get().SelectionWheelGlobalConfiguration;

                if (SelectionWheelGameObject == null) SelectionWheelGameObject = SelectionWheelInstancer.CreateSelectionWheelGameObject(SelectionWheelGlobalConfiguration.SelectionWheelPrefab, CoreGameSingletonInstances.GameCanvas);

                #region External Dependencies

                var GameInputManager = CoreGameSingletonInstances.GameInputManager;

                #endregion

                SelectionWheelPositionManager = new SelectionWheelPositionManager(this, SelectionWheelGlobalConfiguration, followingTransform);
                ContextActionWhelleEnterExitAnimationManager = new ContextActionWhelleEnterExitAnimationManager(SelectionWheelGameObject.GetComponent<Animator>());
                ActionWheelActiveNodeManager = new ActionWheelActiveNodeManager(SelectionWheelGlobalConfiguration.NonSelectedMaterial, SelectionWheelGlobalConfiguration.SelectedMaterial);
                ActionWheelNodePositionManager = new ActionWheelNodePositionManager(SelectionWheelGlobalConfiguration.ActionWheelNodePositionManagerComponent, GameInputManager, ActionWheelActiveNodeManager);
                wheelNodes = new SelectionWheelNode[wheelNodeDatas.Count];
                var actionNodeContainerObject = SelectionWheelGameObject.transform.Find(ACTION_NODE_CONTAINER_OBJECT_NAME);
                for (var i = 0; i < wheelNodeDatas.Count; i++)
                {
                    var wheelNode = SelectionWheelNode.Instantiate(wheelNodeDatas[i]);
                    wheelNode.transform.SetParent(actionNodeContainerObject, false);
                    wheelNodes[i] = wheelNode;
                }

                ActionWheelNodePositionManager.InitNodes(wheelNodes);
                ContextActionWhelleEnterExitAnimationManager.Init();
                ActionWheelActiveNodeManager.SelectedNodeChanged(wheelNodes);
            }
        }

        private void SleepWheel(bool destroyImmediate = false)
        {
            if (IsWheelEnabled)
            {
                IsWheelEnabled = false;

                var actionNodeContainerObject = SelectionWheelGameObject.transform.Find(ACTION_NODE_CONTAINER_OBJECT_NAME);
                var transformToDestroy = new Transform[actionNodeContainerObject.childCount];
                for (var i = 0; i < actionNodeContainerObject.childCount; i++) transformToDestroy[i] = actionNodeContainerObject.GetChild(i);

                if (transformToDestroy != null && transformToDestroy.Length > 0)
                {
                    if (destroyImmediate)
                    {
                        ContextActionWhelleEnterExitAnimationManager.PlayExitAnimation();
                        for (var i = 0; i < transformToDestroy.Length; i++)
                            if (transformToDestroy[i] != null)
                                MonoBehaviour.Destroy(transformToDestroy[i].gameObject);
                    }
                    else
                    {
                        Coroutiner.Instance.StartCoroutine(ContextActionWhelleEnterExitAnimationManager.ExitCoroutine(() =>
                        {
                            for (var i = 0; i < transformToDestroy.Length; i++)
                                if (transformToDestroy[i] != null)
                                    MonoBehaviour.Destroy(transformToDestroy[i].gameObject);
                        }));
                    }
                }
            }
        }

        private void RefreshWheel(List<SelectionWheelNodeData> wheelNodeDatas, Transform followingTransform)
        {
            if (IsWheelEnabled)
            {
                SelectionWheelEventsManager.Get().OnWheelSleep(true);
                SelectionWheelEventsManager.Get().OnWheelAwake(wheelNodeDatas, followingTransform);
            }
        }

        public void Tick(float d)
        {
            if (IsWheelEnabled)
            {
                SelectionWheelPositionManager.Tick(d);
                ActionWheelNodePositionManager.Tick(d, wheelNodes);

                foreach (var wheelNode in wheelNodes)
                    wheelNode.Tick(d);
            }
        }

        public SelectionWheelNodeData GetSelectedNodeData()
        {
            if (ActionWheelActiveNodeManager.ActiveNode != null) return ActionWheelActiveNodeManager.ActiveNode.WheelNodeData;

            return null;
        }
    }

    #region Wheel Position

    internal class SelectionWheelPositionManager
    {
        private Transform FollowWorldTransform;
        private SelectionWheelGlobalConfiguration SelectionWheelGlobalConfigurationRef;
        private SelectionWheelObject SelectionWheelObjectRef;

        public SelectionWheelPositionManager(SelectionWheelObject selectionWheelObjectRef, SelectionWheelGlobalConfiguration selectionWheelGlobalConfigurationRef, Transform FollowWorldTransform)
        {
            SelectionWheelObjectRef = selectionWheelObjectRef;
            SelectionWheelGlobalConfigurationRef = selectionWheelGlobalConfigurationRef;
            this.FollowWorldTransform = FollowWorldTransform;
        }

        public void Tick(float d)
        {
            SelectionWheelObjectRef.SelectionWheelGameObject.transform.position
                = Camera.main.WorldToScreenPoint(FollowWorldTransform.position) + new Vector3(0, SelectionWheelGlobalConfigurationRef.ActionWheelNodePositionManagerComponent.DistanceFromCenter, 0);
        }
    }

    #endregion

    #region Node position

    internal class ActionWheelNodePositionManager
    {
        private ActionWheelActiveNodeManager ActionWheelActiveNodeManager;
        private ActionWheelNodePositionManagerComponent ActionWheelNodePositionManagerComponent;
        private GameInputManager GameInputManager;

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
                        for (var i = 0; i < wheelActionNodes.Length; i++) wheelActionNodes[i].TargetWheelAngleDeg += 360 / wheelActionNodes.Length;

                        ActionWheelActiveNodeManager.SelectedNodeChanged(wheelActionNodes);
                    }
                    else if (joystickAxis.x <= -0.5)
                    {
                        isRotating = true;
                        for (var i = 0; i < wheelActionNodes.Length; i++) wheelActionNodes[i].TargetWheelAngleDeg -= 360 / wheelActionNodes.Length;

                        ActionWheelActiveNodeManager.SelectedNodeChanged(wheelActionNodes);
                    }
                }

                if (RepositionNodesSmooth(wheelActionNodes, d)) isRotating = false;
            }
        }

        private bool RepositionNodesSmooth(SelectionWheelNode[] wheelActionNodes, float d)
        {
            for (var i = 0; i < wheelActionNodes.Length; i++)
            {
                wheelActionNodes[i].CurrentAngleDeg = Mathf.Lerp(wheelActionNodes[i].CurrentAngleDeg, wheelActionNodes[i].TargetWheelAngleDeg, d * ActionWheelNodePositionManagerComponent.RotationSpeed);
                var nodePosition = Vector3.up;
                nodePosition = Quaternion.Euler(0, 0, wheelActionNodes[i].CurrentAngleDeg) * nodePosition;
                nodePosition *= ActionWheelNodePositionManagerComponent.DistanceFromCenter;
                wheelActionNodes[i].transform.localPosition = nodePosition;
            }

            if (Mathf.Abs(wheelActionNodes[0].TargetWheelAngleDeg - wheelActionNodes[0].CurrentAngleDeg) < 5)
                return true;
            else
                return false;
        }

        public void InitNodes(SelectionWheelNode[] wheelActionNodes)
        {
            for (var i = 0; i < wheelActionNodes.Length; i++)
            {
                wheelActionNodes[i].TargetWheelAngleDeg = 360 / wheelActionNodes.Length * i;
                wheelActionNodes[i].CurrentAngleDeg = wheelActionNodes[i].TargetWheelAngleDeg;
                var nodePosition = Vector3.up;
                nodePosition = Quaternion.Euler(0, 0, wheelActionNodes[i].TargetWheelAngleDeg) * nodePosition;
                nodePosition *= ActionWheelNodePositionManagerComponent.DistanceFromCenter;
                wheelActionNodes[i].transform.localPosition = nodePosition;
            }
        }
    }


    internal class ActionWheelActiveNodeManager
    {
        private SelectionWheelNode activeNode;

        private Material nonSelectedMaterial;
        private Material selectedMaterial;

        public ActionWheelActiveNodeManager(Material nonSelectedMaterial, Material selectedMaterial)
        {
            this.nonSelectedMaterial = nonSelectedMaterial;
            this.selectedMaterial = selectedMaterial;
        }

        public SelectionWheelNode ActiveNode => activeNode;

        public void SelectedNodeChanged(SelectionWheelNode[] wheelActionNodes)
        {
            if (activeNode != null)
            {
                activeNode.SetMaterial(nonSelectedMaterial);
                activeNode.SetActiveText(false);
            }

            for (var i = 0; i < wheelActionNodes.Length; i++)
                if (wheelActionNodes[i].TargetWheelAngleDeg % 360 == 0)
                {
                    activeNode = wheelActionNodes[i];
                    activeNode.SetMaterial(selectedMaterial);
                    activeNode.SetActiveText(true);
                    return;
                }
        }
    }

    #endregion

    #region Wheel Enter/Exit animations

    internal class ContextActionWhelleEnterExitAnimationManager
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
            PlayExitAnimation();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfAnimation(ContextActionWheelAnimator, "Exit", 0);
            afterAnimationEndCallback.Invoke();
        }
    }

    #endregion
}