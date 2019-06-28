using CoreGame;
using GameConfigurationID;
using System.Collections;
using UnityEngine;
using static AnimationConstants;

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
            var AnimationConfiguration = GameObject.FindObjectOfType<CoreConfigurationManager>().AnimationConfiguration();
            var PlayerAnimator = GetComponentInChildren<Animator>();

            this.playerAnimationDataManager = new PlayerAnimationDataManager(PlayerAnimator);
            this.playerIdleAnimationManager = new PlayerIdleAnimationManager(PlayerIdleAnimationManagerComponent, PlayerAnimator, this, AnimationConfiguration);
            this.PlayerAnimationFXHandler = new PlayerAnimationFXHandler(FindObjectOfType<FXContainerManager>(), PlayerAnimator);

            GenericAnimatorHelper.SetMovementLayer(PlayerAnimator, AnimationConfiguration, LevelType.ADVENTURE);
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

        #region Internal Events
        public void OnSpawnFireSmoke()
        {
            PlayerAnimationFXHandler.OnSpawnFireSmoke();
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
        private PlayerAnimationManager PlayerManagerReference;
        private AnimationConfiguration AnimationConfiguration;

        private float elapsedTime;
        private bool isIdlingAnimationRuning;

        private Coroutine animationCoroutine;

        public bool IsIdlingAnimationRuning { get => isIdlingAnimationRuning; }

        public PlayerIdleAnimationManager(PlayerIdleAnimationManagerComponent playerIdleAnimationManagerComponent, Animator playerAnimator, PlayerAnimationManager playerManagerReference, AnimationConfiguration AnimationConfiguration)
        {
            this.PlayerIdleAnimationManagerComponent = playerIdleAnimationManagerComponent;
            this.PlayerAnimator = playerAnimator;
            this.PlayerManagerReference = playerManagerReference;
            this.AnimationConfiguration = AnimationConfiguration;
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
            PlayerAnimator.Play(this.AnimationConfiguration.ConfigurationInherentData[AnimationID.ACTION_LISTENING].AnimationName);
        }

        private IEnumerator PlayPlayerIdleSmokeAnimation()
        {
            isIdlingAnimationRuning = true;
            yield return PlayerAnimationPlayer.PlayIdleSmokeAnimation(PlayerAnimator, this.AnimationConfiguration,
                onTriggerSmokeEffect: PlayerManagerReference.OnSpawnFireSmoke,
                onAnimationEnd: null);
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

        public PlayerAnimationFXHandler(FXContainerManager fXContainerManager, Animator playerAnimator)
        {
            FXContainerManager = fXContainerManager;
            this.PlayerAnimator = playerAnimator;
        }

        #region Idle animations
        public void OnSpawnFireSmoke()
        {
            CurrentEffectPlaying = FXContainerManager.TriggerFX(PrefabContainer.Instance.PlayerSmokeEffectPrefab, AnimationConstants.BipedBoneRetriever.GetPlayerBone(BipedBone.HEAD, PlayerAnimator).transform);
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
