﻿using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public class CutscenePersistPOIAction : SequencedAction
    {
        [CustomEnum()]
        [SerializeField]
        public PointOfInterestId PointOfInterestId;

        public CutscenePersistPOIAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            var cutsceneActionInput = (CutsceneActionInput)ContextActionInput;
            var ghostPOI = cutsceneActionInput.GhostsPOIManager.GetGhostPOI(this.PointOfInterestId);
            if (ghostPOI != null)
            {
                var pointOfInterestType = cutsceneActionInput.PointOfInterestManager.GetActivePointOfInterest(this.PointOfInterestId);
                ghostPOI.OnPositionChanged(pointOfInterestType.GetRootObject().transform, cutsceneActionInput.LevelManager.CurrentLevelZoneChunkID);
            }
        }


        public override void Tick(float d)
        {

        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.PointOfInterestId = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.PointOfInterestId);
        }
#endif
    }

}
