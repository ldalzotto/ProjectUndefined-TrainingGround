using System.Collections;
using System.Collections.Generic;
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

        this.PlayerAnimationFXHandler = new PlayerAnimationFXHandler(FindObjectOfType<PlayerGlobalAnimationEventHandler>(), FindObjectOfType<FXContainerManager>(), PlayerAnimator);
    }


    #region Logical Conditions
    public bool IsIdleAnimationRunnig()
    {
        return playerIdleAnimationManager.IsIdlingAnimationRuning;
    }
    #endregion

    #region External Events
    public void OnIdleAnimationReset()
    {
        PlayerIdleAnimationManager.ResetIdleTimer();
        PlayerAnimationFXHandler.OnAnimationKilled();
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
                    animationCoroutine = PlayerManagerReference.StartCoroutine(PlayIdleAnimation(PlayerAnimatioNamesEnum.PLAYER_IDLE_SMOKE));
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
        PlayerAnimator.Play(AnimationConstants.PlayerAnimationConstants[PlayerAnimatioNamesEnum.PLAYER_IDLE_OVERRIDE_LISTENING].AnimationName);
    }

    private IEnumerator PlayIdleAnimation(PlayerAnimatioNamesEnum playerAnimatioNnamesEnum)
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
    private Animator PlayerAnimator;

    private TriggerableEffect CurrentEffectPlaying;

    public PlayerAnimationFXHandler(PlayerGlobalAnimationEventHandler playerGlobalAnimationEventHandler, FXContainerManager fXContainerManager, Animator playerAnimator)
    {
        FXContainerManager = fXContainerManager;
        this.PlayerAnimator = playerAnimator;
        playerGlobalAnimationEventHandler.OnIdleOverideTriggerSmokeEffect += SpawnFireSmoke;
    }

    #region Idle animations
    private void SpawnFireSmoke()
    {
        CurrentEffectPlaying = FXContainerManager.TriggerFX(PrefabContainer.Instance.PlayerSmokeEffectPrefab, PlayerBoneRetriever.GetPlayerBone(PlayerBone.HEAD, PlayerAnimator).transform);
    }
    #endregion

    #region External Events
    public void OnAnimationKilled()
    {
        if (CurrentEffectPlaying != null)
        {
            MonoBehaviour.Destroy(CurrentEffectPlaying.gameObject);
        }
    }
    #endregion
}
#endregion

#region Bone Retriever
class PlayerBoneRetriever
{
    private static Dictionary<PlayerBone, string> BoneNames = new Dictionary<PlayerBone, string>()
    {
        {PlayerBone.HEAD, "Head"},
        {PlayerBone.RIGHT_HAND_CONTEXT, "HoldItem.R" },
        {PlayerBone.RIGH_FINGERS, "Figers.R" }
    };

    public static GameObject GetPlayerBone(PlayerBone playerBone, Animator playerAnimator)
    {
        if (playerAnimator != null)
        {
            return playerAnimator.gameObject.FindChildObjectRecursively(BoneNames[playerBone]);
        }
        return null;

    }
}

public enum PlayerBone
{
    HEAD, RIGHT_HAND_CONTEXT, RIGH_FINGERS
}
#endregion