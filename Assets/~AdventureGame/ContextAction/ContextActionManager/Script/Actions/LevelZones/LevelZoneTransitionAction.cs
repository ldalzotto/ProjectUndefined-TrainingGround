﻿using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    public class LevelZoneTransitionAction : AContextAction
    {

        [SerializeField]
        private LevelZonesID nextZone;

        public LevelZoneTransitionAction(LevelZonesID nextZone) : base(null)
        {
            this.nextZone = nextZone;
        }

        public LevelZonesID NextZone { get => nextZone; }

        public override void AfterFinishedEventProcessed()
        {

        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void FirstExecutionAction(AContextActionInput ContextActionInput)
        {
            var levelTransitionManager = GameObject.FindObjectOfType<LevelTransitionManager>();
            levelTransitionManager.OnAdventureToPuzzleLevel(nextZone);
        }

        public override void Tick(float d)
        {

        }
    }

}