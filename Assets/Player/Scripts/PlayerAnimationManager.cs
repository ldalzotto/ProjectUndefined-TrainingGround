﻿using System.Collections;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [Header("Animation")]
    public PlayerIdleAnimationManagerComponent PlayerIdleAnimationManagerComponent;

    private PlayerAnimationDataManager playerAnimationDataManager;
    private PlayerIdleAnimationManager playerIdleAnimationManager;

    #region FX Instanciation handler
    private PlayerAnimationFXHandler PlayerAnimationFXHandler;

    internal PlayerAnimationDataManager PlayerAnimationDataManager { get => playerAnimationDataManager; }
    internal PlayerIdleAnimationManager PlayerIdleAnimationManager { get => playerIdleAnimationManager; }
    #endregion

    private void Start()
    {
        var PlayerAnimator = GetComponentInChildren<Animator>();

        this.playerAnimationDataManager = new PlayerAnimationDataManager(PlayerAnimator);
        this.playerIdleAnimationManager = new PlayerIdleAnimationManager(PlayerIdleAnimationManagerComponent, PlayerAnimator, this);

        this.PlayerAnimationFXHandler = new PlayerAnimationFXHandler(FindObjectOfType<PlayerGlobalAnimationEventHandler>(), FindObjectOfType<FXContainerManager>());
    }


    #region Logical Conditions
    public bool IsIdleAnimationRunnig()
    {
        return playerIdleAnimationManager.IsIdlingAnimationRuning;
    }
    #endregion

    public Animator GetPlayerAnimator()
    {
        return playerAnimationDataManager.Animator;
    }

}

#region Animation
class PlayerAnimationDataManager
{

    public const string SpeedMagnitude = "Speed";

    private Animator animator;

    public PlayerAnimationDataManager(Animator animator)
    {
        this.animator = animator;
    }

    public Animator Animator { get => animator; }

    public void Tick(float unscaledSpeedMagnitude)
    {
        animator.SetFloat(SpeedMagnitude, unscaledSpeedMagnitude);
    }

}

class PlayerIdleAnimationManager
{
    private PlayerIdleAnimationManagerComponent PlayerIdleAnimationManagerComponent;
    private Animator PlayerAnimator;
    private MonoBehaviour PlayerManagerReference;

    private float elapsedTime;
    private bool isIdlingAnimationRuning;

    private Coroutine animationCoroutine;

    public bool IsIdlingAnimationRuning { get => isIdlingAnimationRuning; }

    public PlayerIdleAnimationManager(PlayerIdleAnimationManagerComponent playerIdleAnimationManagerComponent, Animator playerAnimator, MonoBehaviour playerManagerReference)
    {
        PlayerIdleAnimationManagerComponent = playerIdleAnimationManagerComponent;
        PlayerAnimator = playerAnimator;
        PlayerManagerReference = playerManagerReference;
    }

    public void Tick(float delta, float unscaledSpeedMagnitude)
    {
        if (unscaledSpeedMagnitude <= float.Epsilon)
        {
            if (!isIdlingAnimationRuning)
            {
                elapsedTime += delta;
                if (elapsedTime >= PlayerIdleAnimationManagerComponent.TimeThresholdTriggerIdleAnimation)
                {
                    animationCoroutine = PlayerManagerReference.StartCoroutine(PlayIdleAnimation(PlayerAnimatioNnamesEnum.PLAYER_IDLE_SMOKE));
                }
            }
        }
        else
        {
            if (isIdlingAnimationRuning)
            {
                PlayerManagerReference.StopCoroutine(animationCoroutine);
                ResetIdleTimer();
                SwitchToListeningAnimation();
            }
        }
    }

    private void SwitchToListeningAnimation()
    {
        PlayerAnimator.Play(AnimationConstants.PlayerAnimationConstants[PlayerAnimatioNnamesEnum.PLAYER_IDLE_OVERRIDE_LISTENING].AnimationName);
    }

    private IEnumerator PlayIdleAnimation(PlayerAnimatioNnamesEnum playerAnimatioNnamesEnum)
    {
        isIdlingAnimationRuning = true;
        var animationName = AnimationConstants.PlayerAnimationConstants[playerAnimatioNnamesEnum].AnimationName;
        var layerIndex = AnimationConstants.PlayerAnimationConstants[playerAnimatioNnamesEnum].LayerIndex;
        PlayerAnimator.Play(animationName);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfAnimation(PlayerAnimator, animationName, layerIndex);
        ResetIdleTimer();
    }

    public void ResetIdleTimer()
    {
        elapsedTime = 0f;

        if (isIdlingAnimationRuning)
        {
            SwitchToListeningAnimation();
        }
        isIdlingAnimationRuning = false;
    }

    private void OnAnimationEnd()
    {
        elapsedTime = 0f;
    }
}

[System.Serializable]
public class PlayerIdleAnimationManagerComponent
{
    public float TimeThresholdTriggerIdleAnimation;
}
#endregion

#region Player Animation FX handler
class PlayerAnimationFXHandler
{
    private FXContainerManager FXContainerManager;

    public PlayerAnimationFXHandler(PlayerGlobalAnimationEventHandler playerGlobalAnimationEventHandler, FXContainerManager fXContainerManager)
    {
        FXContainerManager = fXContainerManager;
        playerGlobalAnimationEventHandler.OnIdleOverideTriggerSmokeEffect += SpawnFireSmoke;
    }

    #region Idle animations
    private void SpawnFireSmoke()
    {
        //TODO wrong transform
        FXContainerManager.TriggerFX(PrefabContainer.Instance.PlayerSmokeEffectPrefab, FXContainerManager.transform);
    }
    #endregion
}
#endregion