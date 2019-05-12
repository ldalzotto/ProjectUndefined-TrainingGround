﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class AdventureLevelZonesEventManager : MonoBehaviour
    {
        #region External Dependencies
        private PointOfInterestManager PointOfInterestManager;
        private Coroutiner Coroutiner;
        #endregion

        private bool isNewZoneLoading;

        void Start()
        {
            PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            Coroutiner = GameObject.FindObjectOfType<Coroutiner>();
        }

        #region External Events
        public void OnLevelZoneChange(LevelZonesID nextZone)
        {
            isNewZoneLoading = true;
            PointOfInterestManager.OnActualZoneSwitched();
            SceneLoadHelper.LoadScene(Coroutiner, nextZone);
            isNewZoneLoading = false;
        }
        #endregion

        #region Logical Conditions
        public bool IsNewZoneLoading() { return isNewZoneLoading; }
        #endregion
    }

}