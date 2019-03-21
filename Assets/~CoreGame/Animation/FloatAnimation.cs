using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CoreGame
{
    public class FloatAnimation
    {
        private float maxValue;
        private float growSpeed;
        private float currentValue;

        public FloatAnimation(float maxValue, float growSpeed, float initalValue)
        {
            this.maxValue = maxValue;
            this.growSpeed = growSpeed;
            this.currentValue = initalValue;
        }

        public float CurrentValue { get => currentValue; }

        public void Tick(float d)
        {
            this.currentValue += d * this.growSpeed;
            this.currentValue = Mathf.Min(this.currentValue, this.maxValue);
        }
    }
}
