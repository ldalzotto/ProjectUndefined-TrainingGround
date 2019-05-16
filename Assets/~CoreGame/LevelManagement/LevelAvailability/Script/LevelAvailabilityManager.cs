using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CoreGame
{
    [System.Serializable]
    public class LevelAvailabilityManager : MonoBehaviour
    {
        private LevelAvailability levelAvailability;
        private LevelAvailabilityPersistanceManager LevelAvailabilityPersistanceManager;

        public LevelAvailability LevelAvailability { get => levelAvailability; }

        public void Init()
        {
            this.LevelAvailabilityPersistanceManager = new LevelAvailabilityPersistanceManager();
            var loadedLevelAvailability = this.LevelAvailabilityPersistanceManager.Load();
            if (loadedLevelAvailability == null)
            {
                this.levelAvailability = new LevelAvailability();
            }
            else
            {
                this.levelAvailability = loadedLevelAvailability;
            }
        }

        internal void UnlockLevel(LevelZoneChunkID levelZoneChunkToUnlock)
        {
            this.levelAvailability.LevelZoneChunkAvailability[levelZoneChunkToUnlock] = true;
            this.LevelAvailabilityPersistanceManager.Save(this.levelAvailability);
        }

        #region Logical conditions
        public bool IsLevelAvailable(LevelZoneChunkID levelZoneChunkID)
        {
            return this.levelAvailability.LevelZoneChunkAvailability.ContainsKey(levelZoneChunkID) && this.levelAvailability.LevelZoneChunkAvailability[levelZoneChunkID];
        }
        #endregion
    }

    [System.Serializable]
    public class LevelAvailability
    {
        public Dictionary<LevelZoneChunkID, bool> LevelZoneChunkAvailability;

        public LevelAvailability()
        {
            //New game configuration
            this.LevelZoneChunkAvailability = new Dictionary<LevelZoneChunkID, bool>() {
                     {LevelZoneChunkID.SEWER_RTP_1, true }
            };
        }
    }

    public class LevelAvailabilityPersistanceManager : AbstractGamePersister<LevelAvailability>
    {
        public const string FolderName = "LevelAvailability";
        public const string FileExtension = ".lvl";
        public const string FileName = "LevelAvailability";

        public LevelAvailabilityPersistanceManager() : base(FolderName, FileExtension, FileName)
        {
        }
    }

}
