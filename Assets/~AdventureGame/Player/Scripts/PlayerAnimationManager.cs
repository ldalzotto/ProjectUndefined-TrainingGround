using CoreGame;
using System.Collections;
using UnityEngine;
using static CoreGame.PlayerAnimationConstants;

namespace AdventureGame
{
    public class PlayerAnimationManager : MonoBehaviour
    {
        [Header("Animation")]
        public PlayerIdleAnimationManagerComponent PlayerIdleAnimationManagerComponent;

        private PlayerAnimationDataManager playerAnimationDataManager;
        private PlayerIdleAnimationManager playerIdleAnimationManager;

        #region FX Instanciation handler
        private PlayerAnimationFXHandler PlayerAnimationFXHandler;

        public PlayerAnimationDataManager PlayerAnimationDataManager { get => playerAnimationDataManager; }
        public PlayerIdleAnimationManager PlayerIdleAnimationManager { get => playerIdleAnimationManager; }
        #endregion

        private void Start()
        {
            var playerGlobalAnimationEventHandler = GameObject.FindObjectOfType<PlayerGlobalAnimationEventHandler>();

            var PlayerAnimator = GetComponentInChildren<Animator>();

            this.playerAnimationDataManager = new PlayerAnimationDataManager(PlayerAnimator);
            this.playerIdleAnimationManager = new PlayerIdleAnimationManager(PlayerIdleAnimationManagerComponent, PlayerAnimator, this, playerGlobalAnimationEventHandler);

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
    public class PlayerIdleAnimationManager
    {
        private PlayerIdleAnimationManagerComponent PlayerIdleAnimationManagerComponent;
        private Animator PlayerAnimator;
        private MonoBehaviour PlayerManagerReference;
        private PlayerGlobalAnimationEventHandler PlayerGlobalAnimationEventHandler;

        private float elapsedTime;
        private bool isIdlingAnimationRuning;

        private Coroutine animationCoroutine;

        public bool IsIdlingAnimationRuning { get => isIdlingAnimationRuning; }

        public PlayerIdleAnimationManager(PlayerIdleAnimationManagerComponent playerIdleAnimationManagerComponent, Animator playerAnimator, MonoBehaviour playerManagerReference, PlayerGlobalAnimationEventHandler PlayerGlobalAnimationEventHandler)
        {
            PlayerIdleAnimationManagerComponent = playerIdleAnimationManagerComponent;
            PlayerAnimator = playerAnimator;
            PlayerManagerReference = playerManagerReference;
            this.PlayerGlobalAnimationEventHandler = PlayerGlobalAnimationEventHandler;
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
                        animationCoroutine = PlayerManagerReference.StartCoroutine(PlayPlayerIdleSmokeAnimation());
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

        private IEnumerator PlayPlayerIdleSmokeAnimation()
        {
            isIdlingAnimationRuning = true;
            yield return PlayerAnimationPlayer.PlayIdleSmokeAnimation(PlayerAnimator, PlayerGlobalAnimationEventHandler);
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
            CurrentEffectPlaying = FXContainerManager.TriggerFX(PrefabContainer.Instance.PlayerSmokeEffectPrefab, PlayerAnimationConstants.PlayerBoneRetriever.GetPlayerBone(PlayerBone.HEAD, PlayerAnimator).transform);
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
}
