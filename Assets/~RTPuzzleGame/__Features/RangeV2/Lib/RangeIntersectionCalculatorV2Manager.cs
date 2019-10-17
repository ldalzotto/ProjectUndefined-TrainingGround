using System.Collections.Generic;
using UnityEngine;
using CoreGame;
using InteractiveObjects;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{

    public class RangeIntersectionCalculatorV2Manager : GameSingleton<RangeIntersectionCalculatorV2Manager>
    {
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

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.AllRangeIntersectionCalculatorV2.Clear();
        }

        public void GizmoTick()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                foreach (var RangeIntersectionCalculatorV2 in this.AllRangeIntersectionCalculatorV2)
                {
                    RangeIntersectionCalculationManagerV2.Get().TryGetRangeintersectionResult(RangeIntersectionCalculatorV2, out bool isInsideRange);
                    Color lineColor = isInsideRange ? Color.green : Color.red;
                    var oldColor = Handles.color;
                    Handles.color = lineColor;
                    Handles.DrawLine(RangeIntersectionCalculatorV2.GetAssociatedRangeObject().GetTransform().WorldPosition, RangeIntersectionCalculatorV2.TrackedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition);
                    Handles.color = oldColor;
                }
            }
#endif
        }
    }
}

