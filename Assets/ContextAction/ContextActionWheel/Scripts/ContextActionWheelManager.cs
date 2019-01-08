using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextActionWheelManager : MonoBehaviour
{

    public WheelPositionManagerComponent WheelPositionManagerComponent;
    public PointOnInteresetSelectedEffectComponent PointOnInteresetSelectedEffectComponent;

    private ContextActionEventManager ContextActionEventManager;

    #region External dependecies
    private ContextActionWheel ContextActionWheel;
    private PlayerManager PlayerManager;
    private ContextActionWheelEventManager ContextActionWheelEventManager;
    #endregion
    private WheelActivityManager WheelActivityManager;
    private WheelActionActivationManager WheelActionActivationManager;
    private PointOnInteresetSelectedEffectManager PointOnInteresetSelectedEffectManager;
    private WheelPositionManager WheelPositionManager;

    private void Start()
    {
        #region External Dependencies
        ContextActionEventManager = GameObject.FindObjectOfType<ContextActionEventManager>();
        ContextActionWheel = GameObject.FindObjectOfType<ContextActionWheel>();
        PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
        ContextActionWheelEventManager = GameObject.FindObjectOfType<ContextActionWheelEventManager>();
        GameInputManager GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        #endregion

        WheelActivityManager = new WheelActivityManager(GameInputManager);
        WheelActionActivationManager = new WheelActionActivationManager(GameInputManager, ContextActionWheel);
        WheelPositionManager = new WheelPositionManager(GameObject.FindGameObjectWithTag(TagConstants.PLAYER_TAG).transform, WheelPositionManagerComponent, ContextActionWheel.transform);
        PointOnInteresetSelectedEffectManager = new PointOnInteresetSelectedEffectManager(PointOnInteresetSelectedEffectComponent);
    }

    public void Tick(float d)
    {
        if (WheelActivityManager.IsEnabled)
        {

            WheelPositionManager.Tick();

            if (WheelActivityManager.TickCancelInput())
            {
                SleepWheel();
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
        SleepWheel();
        var actionInput = ContextActionBuilder.BuildContextActionInput(contextAction, PlayerManager);
        if (actionInput != null)
        {
            ContextActionEventManager.OnContextActionAdded(contextAction, actionInput);
        }
    }

    #region External Events
    public void OnAwakeWheel(AContextAction[] contextActions, WheelTriggerSource wheelTriggerSource, PointOfInterestType currentTargetedPOI)
    {
        ContextActionWheel.Init(contextActions);
        PointOnInteresetSelectedEffectManager.OnWheelEnabled(currentTargetedPOI);
        WheelActivityManager.AwakeWheel(wheelTriggerSource);
    }
    public void SleepWheel()
    {
        ContextActionWheel.Exit();
        WheelActivityManager.SleepWheel();
        ContextActionWheelEventManager.OnWheelDisabled();
        PointOnInteresetSelectedEffectManager.OnWheelDisabled();
    }
    #endregion
}


class WheelActionActivationManager
{
    private GameInputManager GameInputManager;
    private ContextActionWheel ContextActionWheel;

    public WheelActionActivationManager(GameInputManager gameInputManager, ContextActionWheel contextActionWheel)
    {
        GameInputManager = gameInputManager;
        ContextActionWheel = contextActionWheel;
    }

    public AContextAction Tick()
    {
        if (GameInputManager.CurrentInput.ActionButtonD())
        {
            return ContextActionWheel.GetSelectedAction();
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

#region 
class PointOnInteresetSelectedEffectManager
{
    private PointOnInteresetSelectedEffectComponent PointOnInteresetSelectedEffectComponent;

    private Dictionary<int, Material> originalMaterials = new Dictionary<int, Material>();
    private PointOfInterestType cachedSelectedPointOfInterest;

    public PointOnInteresetSelectedEffectManager(PointOnInteresetSelectedEffectComponent pointOnInteresetSelectedEffectComponent)
    {
        PointOnInteresetSelectedEffectComponent = pointOnInteresetSelectedEffectComponent;
    }

    public void OnWheelEnabled(PointOfInterestType selectedPointOfInterest)
    {
        if (selectedPointOfInterest != null)
        {
            cachedSelectedPointOfInterest = selectedPointOfInterest;
            var poiMeshRenderers = selectedPointOfInterest.GetRenderers();
            for (var i = 0; i < poiMeshRenderers.Length; i++)
            {
                originalMaterials[poiMeshRenderers[i].GetInstanceID()] = poiMeshRenderers[i].material;
                var selectedMaterial = new Material(PointOnInteresetSelectedEffectComponent.POISelectedGenericMaterial);
                selectedMaterial.CopyPropertiesFromMaterial(poiMeshRenderers[i].material);
                selectedMaterial.SetFloat("_RimMaxInfluenceFactor", PointOnInteresetSelectedEffectComponent.RimMaxInfluenceFactor);
                selectedMaterial.SetFloat("_RimInfluenceSpeed", PointOnInteresetSelectedEffectComponent.RimInfluenceSpeed);
                poiMeshRenderers[i].material = selectedMaterial;
            }
        }

    }

    public void OnWheelDisabled()
    {
        if (cachedSelectedPointOfInterest != null)
        {
            var poiMeshRenderers = cachedSelectedPointOfInterest.GetRenderers();
            for (var i = 0; i < poiMeshRenderers.Length; i++)
            {
                poiMeshRenderers[i].material = originalMaterials[poiMeshRenderers[i].GetInstanceID()];
            }
            cachedSelectedPointOfInterest = null;
        }

    }

}

[System.Serializable]
public class PointOnInteresetSelectedEffectComponent
{
    public Material POISelectedGenericMaterial;

    private float _RimMaxInfluenceFactor = 0.5f;
    private float _RimInfluenceSpeed = 50f;

    public float RimMaxInfluenceFactor { get => _RimMaxInfluenceFactor; }
    public float RimInfluenceSpeed { get => _RimInfluenceSpeed; }
}

#endregion