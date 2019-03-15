using UnityEngine;

namespace RTPuzzle
{
    public class TimeFlowBarManager : MonoBehaviour
    {
        private const string FULL_BAR_OBJECT_NAME = "FullBar";

        private TimeFlowDepletingUIManager tmeFlowDepletingUIManager;

        public void Init(float availableTimeAmount)
        {
            var fullBarObject = gameObject.FindChildObjectRecursively(FULL_BAR_OBJECT_NAME);
            tmeFlowDepletingUIManager = new TimeFlowDepletingUIManager(fullBarObject.transform as RectTransform, availableTimeAmount);
        }


        public void Tick(float currentTimeAmount)
        {
            tmeFlowDepletingUIManager.Tick(currentTimeAmount);
        }


        class TimeFlowDepletingUIManager
        {

            private RectTransform fullBarTransform;
            private float availableTimeAmount;
            private TimeFlowManager TimeFlowManagerRef;

            public TimeFlowDepletingUIManager(RectTransform fullBarTransform, float availableTimeAmount)
            {
                this.fullBarTransform = fullBarTransform;
                this.availableTimeAmount = availableTimeAmount;
            }

            public void Tick(float currentTimeAmount)
            {
                fullBarTransform.localScale = new Vector3(currentTimeAmount / availableTimeAmount, 1, 1);
            }
        }
    }

}
