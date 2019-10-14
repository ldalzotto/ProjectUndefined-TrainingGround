using CoreGame;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class RangeObjectV2Manager : GameSingleton<RangeObjectV2Manager>
    {
        public List<RangeObjectV2> RangeObjects { get; private set; } = new List<RangeObjectV2>();

        private event Action<RangeObjectV2> OnRangeObjectCreatedEvent;
        private event Action<RangeObjectV2> OnRangeObjectDestroyedEvent;
        public void RegisterOnRangeObjectCreatedEventListener(Action<RangeObjectV2> action) { this.OnRangeObjectCreatedEvent += action; }
        public void RegisterOnRangeObjectDestroyedEventListener(Action<RangeObjectV2> action) { this.OnRangeObjectDestroyedEvent += action; }

        public void Init()
        {
            var rangeInitializers = GameObject.FindObjectsOfType<RangeObjectInitializer>();
            for (var rangeInitializerIndex = 0; rangeInitializerIndex < rangeInitializers.Length; rangeInitializerIndex++)
            {
                rangeInitializers[rangeInitializerIndex].Init();
            }
        }

        public void Tick(float d)
        {
            for (var rangeObjectIndex = 0; rangeObjectIndex < this.RangeObjects.Count; rangeObjectIndex++)
            {
                this.RangeObjects[rangeObjectIndex].Tick(d);
            }
        }

        public void OnRangeObjectCreated(RangeObjectV2 rangeObjectV2)
        {
            this.RangeObjects.Add(rangeObjectV2);
            if (this.OnRangeObjectCreatedEvent != null) { this.OnRangeObjectCreatedEvent.Invoke(rangeObjectV2); }
        }

        public void OnRangeObjectDestroyed(RangeObjectV2 rangeObjectV2)
        {
            this.RangeObjects.Remove(rangeObjectV2);
            if (this.OnRangeObjectDestroyedEvent != null) { this.OnRangeObjectDestroyedEvent.Invoke(rangeObjectV2); }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.RangeObjects.Clear();
            this.RangeObjects = null;
        }

       
    }

}
