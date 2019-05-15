using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CoreGame
{
    public class LevelAvailabilityManager : MonoBehaviour
    {
        private LevelAvailability levelAvailability;

        public LevelAvailability LevelAvailability { get => levelAvailability; }

        public void Init()
        {
            this.levelAvailability = new LevelAvailability();
        }

        internal void UnlockLevel(LevelZoneChunkID levelZoneChunkToUnlock)
        {
            this.levelAvailability.LevelZoneChunkAvailability[levelZoneChunkToUnlock] = true;
        }

        #region Logical conditions
        public bool IsLevelAvailable(LevelZoneChunkID levelZoneChunkID)
        {
            return this.levelAvailability.LevelZoneChunkAvailability.ContainsKey(levelZoneChunkID) && this.levelAvailability.LevelZoneChunkAvailability[levelZoneChunkID];
        }
        #endregion
    }

    public class LevelAvailability
    {
        public Dictionary<LevelZonesID, bool> LevelZonesAvailability = new Dictionary<LevelZonesID, bool>();
        public Dictionary<LevelZoneChunkID, bool> LevelZoneChunkAvailability = new Dictionary<LevelZoneChunkID, bool>() {
            {LevelZoneChunkID.SEWER_RTP_1, true }
        };
    }

}
