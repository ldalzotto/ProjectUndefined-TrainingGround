using UnityEngine;

namespace RTPuzzle
{
    public class TimeFlowManager : MonoBehaviour
    {

        private TimeFlowInputManager TimeFlowInputManager;

        public void Init()
        {
            #region External Dependencies
            var RTPlayerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            var RTPlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            #endregion

            TimeFlowInputManager = new TimeFlowInputManager(gameInputManager, RTPlayerManagerDataRetriever, RTPlayerManager);
        }

        public void Tick()
        {
            TimeFlowInputManager.Tick();
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
}
