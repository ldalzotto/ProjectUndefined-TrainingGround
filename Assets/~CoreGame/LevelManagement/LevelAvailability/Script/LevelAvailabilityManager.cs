﻿using UnityEngine;
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
            if (this.SynchLevelAvailabilityFromAllLevel())
            {
                this.Persist();
            }
        }

        private bool SynchLevelAvailabilityFromAllLevel()
        {
            if (this.levelAvailability.LevelZoneChunkAvailability.Count < Enum.GetNames(typeof(LevelZoneChunkID)).Length)
            {
                foreach(var levelChunkId in Enum.GetValues(typeof(LevelZoneChunkID)))
                {
                    if (!this.levelAvailability.LevelZoneChunkAvailability.ContainsKey((LevelZoneChunkID)levelChunkId))
                    {
                        this.levelAvailability.LevelZoneChunkAvailability[(LevelZoneChunkID)levelChunkId] = false;
                    }
                }
                return true;
            }
            return false;
        }

        internal void UnlockLevel(LevelZoneChunkID levelZoneChunkToUnlock)
        {
            this.levelAvailability.LevelZoneChunkAvailability[levelZoneChunkToUnlock] = true;
            this.Persist();
        }

        private void Persist()
        {
            this.LevelAvailabilityPersistanceManager.SaveAsync(this.levelAvailability);
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
            this.LevelZoneChunkAvailability = new Dictionary<LevelZoneChunkID, bool>();
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
