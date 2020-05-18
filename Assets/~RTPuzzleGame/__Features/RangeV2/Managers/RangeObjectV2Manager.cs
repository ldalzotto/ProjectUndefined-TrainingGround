﻿using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class RangeObjectV2Manager
    {
        private static RangeObjectV2Manager RangeObjectV2ManagerInstance;
        public static RangeObjectV2Manager Get()
        {
            if (RangeObjectV2ManagerInstance == null) { RangeObjectV2ManagerInstance = new RangeObjectV2Manager(); }
            return RangeObjectV2ManagerInstance;
        }

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

        public void OnDestroy()
        {
            RangeObjectV2ManagerInstance = null;
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
