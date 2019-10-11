using CoreGame;
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
        }

        public void Tick(float d)
        {
            for (var rangeObjectIndex = 0; rangeObjectIndex < this.RangeObjects.Count; rangeObjectIndex++)
            {
                this.RangeObjects[rangeObjectIndex].Tick(d);
            }
        }

        public void ReceiveEvent(RangeObjectV2ManagerAddRangeEvent RangeObjectV2ManagerAddRangeEvent)
        {
            this.RangeObjects.Add(RangeObjectV2ManagerAddRangeEvent.AddedRangeObject);
        }
        public void ReceiveEvent(RangeObjectV2ManagerRemoveRangeEvent RangeObjectV2ManagerRemoveRangeEvent)
        {
            this.RangeObjects.Remove(RangeObjectV2ManagerRemoveRangeEvent.RemovedRangeObject);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            this.RangeObjects.Clear();
            this.RangeObjects = null;
        }
    }

    public struct RangeObjectV2ManagerAddRangeEvent
    {
        public RangeObjectV2 AddedRangeObject;
    }
    public struct RangeObjectV2ManagerRemoveRangeEvent
    {
        public RangeObjectV2 RemovedRangeObject;
    }

}
