using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class TimeFlowBarManager : GameSingleton<TimeFlowBarManager>
    {
        private const string FULL_BAR_OBJECT_NAME = "FullBar";

        private TimeFlowDepletingUIManager tmeFlowDepletingUIManager;
        private TimeFlowBarGameObject TimeFlowBarUIObject;

        public void Init(float availableTimeAmount)
        {
            this.TimeFlowBarUIObject = new TimeFlowBarGameObject(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzleGlobalStaticConfiguration);
            tmeFlowDepletingUIManager = new TimeFlowDepletingUIManager(this.TimeFlowBarUIObject, availableTimeAmount);
        }


        public void Tick(float currentTimeAmount)
        {
            tmeFlowDepletingUIManager.Tick(currentTimeAmount);
        }

        public Vector3 GetScreenPosition()
        {
            return this.TimeFlowBarUIObject.GetTransform().position;
        }


        class TimeFlowDepletingUIManager
        {

            private TimeFlowBarGameObject TimeFlowBarGameObject;
            private float availableTimeAmount;
            private TimeFlowManager TimeFlowManagerRef;

            public TimeFlowDepletingUIManager(TimeFlowBarGameObject TimeFlowBarGameObject, float availableTimeAmount)
            {
                this.TimeFlowBarGameObject = TimeFlowBarGameObject;
                this.availableTimeAmount = availableTimeAmount;
            }

            public void Tick(float currentTimeAmount)
            {
                this.TimeFlowBarGameObject.SetFullBarScale(new Vector3(currentTimeAmount / availableTimeAmount, 1, 1));
            }
        }
    }

}
