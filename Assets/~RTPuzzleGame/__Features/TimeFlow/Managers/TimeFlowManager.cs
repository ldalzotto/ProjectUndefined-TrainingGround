using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public interface ITimeFlowManagerDataRetrieval
    {
        bool NoMoreTimeAvailable();
    }

    public class TimeFlowManager : MonoBehaviour, ITimeFlowManagerDataRetrieval
    {

        private TimeFlowInputManager TimeFlowInputManager;
        private TimeFlowValueTracker TimeFlowValueTracker;

        #region External Dependencies
        private TimeFlowBarManager TimeFlowBarManager;
        private LevelManager LevelManager;
        #endregion

        public void Init(PlayerManagerDataRetriever RTPlayerManagerDataRetriever, PlayerManager RTPlayerManager,
               IGameInputManager gameInputManager, PuzzleGameConfigurationManager puzzleConfigurationManager, TimeFlowBarManager TimeFlowBarManager,
              LevelManager LevelManager)
        {
            this.TimeFlowBarManager = TimeFlowBarManager;
            this.LevelManager = LevelManager;
            TimeFlowInputManager = new TimeFlowInputManager(gameInputManager, RTPlayerManagerDataRetriever, RTPlayerManager);
            TimeFlowValueTracker = new TimeFlowValueTracker(puzzleConfigurationManager.LevelConfiguration()[this.LevelManager.GetCurrentLevel()].AvailableTimeAmount);
        }

        public void Tick(float d)
        {
            TimeFlowInputManager.Tick();
            if (TimeFlowInputManager.IsAbleToFlowTime())
            {
                TimeFlowValueTracker.Tick(d, TimeFlowInputManager.GetTimeAttenuation());
                TimeFlowBarManager.Tick(TimeFlowValueTracker.CurrentAvailableTime);
            }
        }

        #region Logical Conditions
        public bool IsAbleToFlowTime()
        {
            return TimeFlowInputManager.IsAbleToFlowTime();
        }
        public bool NoMoreTimeAvailable()
        {
            return TimeFlowValueTracker.NoMoreTimeAvailable();
        }
        #endregion

        #region Data Retrieval
        public float GetTimeAttenuation()
        {
            return TimeFlowInputManager.GetTimeAttenuation();
        }
        public float GetCurrentAvailableTime()
        {
            return this.TimeFlowValueTracker.CurrentAvailableTime;
        }
        #endregion
        

#if UNITY_EDITOR
        #region Debug CHEAT
        public void CHEAT_SetInfiniteTime()
        {
            this.TimeFlowValueTracker.CHEAT_SetInfiniteTime();
        }
        #endregion
#endif

    }

    class TimeFlowInputManager
    {
        private IGameInputManager GameInputManager;
        private PlayerManagerDataRetriever RTPlayerManagerDataRetriever;
        private PlayerManager RTPlayerManager;

        public TimeFlowInputManager(IGameInputManager gameInputManager, PlayerManagerDataRetriever rTPlayerManagerDataRetriever, PlayerManager rTPlayerManager)
        {
            GameInputManager = gameInputManager;
            RTPlayerManagerDataRetriever = rTPlayerManagerDataRetriever;
            RTPlayerManager = rTPlayerManager;
        }

        private float currentTimeAttenuation;

        public void Tick()
        {
            if (IsTimeFlowPressed())
            {
                currentTimeAttenuation = 1;
            }
            else if (RTPlayerManager.HasPlayerMovedThisFrame())
            {
                currentTimeAttenuation = RTPlayerManagerDataRetriever.GetPlayerSpeedMagnitude();
            }
            else
            {
                currentTimeAttenuation = 0f;
            }
        }

        public bool IsAbleToFlowTime()
        {
            return RTPlayerManager.HasPlayerMovedThisFrame() || IsTimeFlowPressed();
        }

        private bool IsTimeFlowPressed()
        {
            return GameInputManager.CurrentInput.TimeForwardButtonDH();
        }

        public float GetTimeAttenuation()
        {
            return currentTimeAttenuation;
        }
    }

    class TimeFlowValueTracker
    {
        private float currentAvailableTime;

        public TimeFlowValueTracker(float availableTimeAmount)
        {
            this.currentAvailableTime = availableTimeAmount;
        }

        public float CurrentAvailableTime { get => currentAvailableTime; }

        public void Tick(float d, float currentTimeAttenuation)
        {
            currentAvailableTime -= (d * currentTimeAttenuation);
            currentAvailableTime = Mathf.Max(currentAvailableTime, 0f);
        }

        public bool NoMoreTimeAvailable()
        {
            return currentAvailableTime <= 0f;
        }

#if UNITY_EDITOR
        internal void CHEAT_SetInfiniteTime()
        {
            this.currentAvailableTime = 99999999f;
        }
#endif
    }

}
