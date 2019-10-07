using System.Collections.Generic;

namespace RTPuzzle
{

    public class RangeIntersectionCalculatorV2Manager
    {
        private static RangeIntersectionCalculatorV2Manager Instance;
        public static RangeIntersectionCalculatorV2Manager Get()
        {
            if (Instance == null)
            {
                Instance = new RangeIntersectionCalculatorV2Manager();
            }
            return Instance;
        }

        private int CurrentRangeIntersectionCalculatorV2ManagerCounter = 0;
        public List<RangeIntersectionCalculatorV2> AllRangeIntersectionCalculatorV2 = new List<RangeIntersectionCalculatorV2>();

        public int OnRangeIntersectionCalculatorV2ManagerCreation(RangeIntersectionCalculatorV2 RangeIntersectionCalculatorV2)
        {
            this.AllRangeIntersectionCalculatorV2.Add(RangeIntersectionCalculatorV2);
            this.CurrentRangeIntersectionCalculatorV2ManagerCounter += 1;
            return this.CurrentRangeIntersectionCalculatorV2ManagerCounter;
        }

        public void OnRangeIntersectionCalculatorV2ManagerDestroyed(RangeIntersectionCalculatorV2 RangeIntersectionCalculatorV2)
        {
            this.AllRangeIntersectionCalculatorV2.Remove(RangeIntersectionCalculatorV2);
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}

