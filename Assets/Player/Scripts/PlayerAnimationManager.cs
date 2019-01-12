using System.Collections;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [Header("Animation")]
    public PlayerIdleAnimationManagerComponent PlayerIdleAnimationManagerComponent;

    private PlayerAnimationDataManager PlayerAnimationDataManager;
    private PlayerIdleAnimationManager PlayerIdleAnimationManager;

    #region FX Instanciation handler
    private PlayerAnimationFXHandler PlayerAnimationFXHandler;
    #endregion

    private void Start()
    {
        var PlayerAnimator = GetComponentInChildren<Animator>();

        this.PlayerAnimationDataManager = new PlayerAnimationDataManager(PlayerAnimator);
        this.PlayerIdleAnimationManager = new PlayerIdleAnimationManager(PlayerIdleAnimationManagerComponent, PlayerAnimator, this);

        this.PlayerAnimationFXHandler = new PlayerAnimationFXHandler(FindObjectOfType<PlayerGlobalAnimationEventHandler>(), FindObjectOfType<FXContainerManager>());
    }

    public void Tick(float d, float PlayerSpeedMagnitude)
    {
        PlayerAnimationDataManager.Tick(PlayerSpeedMagnitude);
        PlayerIdleAnimationManager.Tick(d, PlayerSpeedMagnitude);
    }

    #region Logical Conditions
    public bool IsIdleAnimationRunnig()
    {
        return PlayerIdleAnimationManager.IsIdlingAnimationRuning;
    }
    #endregion

    public Animator GetPlayerAnimator()
    {
        return PlayerAnimationDataManager.Animator;
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
                PlayerAnimator.Play(AnimationConstants.PlayerAnimationConstants[PlayerAnimatioNnamesEnum.PLAYER_IDLE_OVERRIDE_LISTENING].AnimationName);
            }
        }
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

    private void ResetIdleTimer()
    {
        elapsedTime = 0f;
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