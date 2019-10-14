using CoreGame;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class RangeObjectV2Manager : GameSingleton<RangeObjectV2Manager>
    {
        public List<RangeObjectV2> RangeObjects { get; private set; } = new List<RangeObjectV2>();

        public void Init()
        {
            var rangeInitializers = GameObject.FindObjectsOfType<RangeObjectInitializer>();
            for (var rangeInitializerIndex = 0; rangeInitializerIndex < rangeInitializers.Length; rangeInitializerIndex++)
            {
                rangeInitializers[rangeInitializerIndex].Init();
            }

            #region Event Register
            RangeEventsManager.Get().RegisterOnRangeObjectCreatedEventListener(this.OnRangeObjectCreated);
            RangeEventsManager.Get().RegisterOnRangeObjectDestroyedEventListener(this.OnRangeObjectDestroyed);
            #endregion
        }

        public void Tick(float d)
        {
            for (var rangeObjectIndex = 0; rangeObjectIndex < this.RangeObjects.Count; rangeObjectIndex++)
            {
                this.RangeObjects[rangeObjectIndex].Tick(d);
            }
        }

        private void OnRangeObjectCreated(RangeObjectV2 rangeObjectV2)
        {
            this.RangeObjects.Add(rangeObjectV2);
        }

        private void OnRangeObjectDestroyed(RangeObjectV2 rangeObjectV2)
        {
            this.RangeObjects.Remove(rangeObjectV2);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.RangeObjects.Clear();
            this.RangeObjects = null;
        }

       
    }

}
