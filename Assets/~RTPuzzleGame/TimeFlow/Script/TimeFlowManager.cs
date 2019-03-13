using UnityEngine;

namespace RTPuzzle
{
    public class TimeFlowManager : MonoBehaviour
    {

        private TimeFlowInputManager TimeFlowInputManager;
        private TimeFlowValueTracker TimeFlowValueTracker;

        #region External Dependencies
        private TimeFlowBarManager TimeFlowBarManager;
        private PuzzleEventsManager PuzzleEventsManager;
        private LevelManager LevelManager;
        #endregion

        public void Init(LevelZonesID puzzleId)
        {
            #region External Dependencies
            var RTPlayerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            var RTPlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            var puzzleConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            TimeFlowBarManager = GameObject.FindObjectOfType<TimeFlowBarManager>();
            TimeFlowBarManager.Init(puzzleConfigurationManager.LevelConfiguration()[puzzleId].AvailableTimeAmount, this);
            LevelManager = GameObject.FindObjectOfType<LevelManager>();
            PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            #endregion

            TimeFlowInputManager = new TimeFlowInputManager(gameInputManager, RTPlayerManagerDataRetriever, RTPlayerManager);
            TimeFlowValueTracker = new TimeFlowValueTracker(puzzleConfigurationManager.LevelConfiguration()[puzzleId].AvailableTimeAmount);
        }

        public void Tick(float d)
        {
            TimeFlowInputManager.Tick();
            if (TimeFlowInputManager.IsAbleToFlowTime())
            {
                TimeFlowValueTracker.Tick(d, TimeFlowInputManager.GetTimeAttenuation());
                TimeFlowBarManager.Tick(TimeFlowValueTracker.CurrentAvailableTime);
            }

            if (TimeFlowValueTracker.NoMoreTimeAvailable())
            {
                PuzzleEventsManager.OnGameOver(LevelManager.GetCurrentLevel());
            }
        }

        #region Logical Conditions
        public bool IsAbleToFlowTime()
        {
            return TimeFlowInputManager.IsAbleToFlowTime();
        }
        #endregion

        public float GetTimeAttenuation()
        {
            return TimeFlowInputManager.GetTimeAttenuation();
        }

        public float GetCurrentAvailableTime()
        {
            return TimeFlowValueTracker.CurrentAvailableTime;
        }
    }

    class TimeFlowInputManager
    {
        private GameInputManager GameInputManager;
        private PlayerManagerDataRetriever RTPlayerManagerDataRetriever;
        private PlayerManager RTPlayerManager;

        public TimeFlowInputManager(GameInputManager gameInputManager, PlayerManagerDataRetriever rTPlayerManagerDataRetriever, PlayerManager rTPlayerManager)
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
    }

}
