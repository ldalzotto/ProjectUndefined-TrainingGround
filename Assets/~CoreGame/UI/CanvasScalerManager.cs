using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace CoreGame
{
    public class CanvasScalerManager : GameSingleton<CanvasScalerManager>
    {
        public CanvasScaler CanvasScaler;

        public CanvasScalerManager()
        {
            this.CanvasScaler = GameObject.FindObjectOfType<CanvasScaler>();
        }

        public void Init()
        {
        }
    }
}
