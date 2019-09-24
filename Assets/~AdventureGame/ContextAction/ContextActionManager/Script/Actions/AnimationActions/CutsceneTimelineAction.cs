using CoreGame;
using GameConfigurationID;
using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace AdventureGame
{

    [System.Serializable]
    public class CutsceneTimelineAction : AContextAction
    {
        [SerializeField]
        private CutsceneId cutsceneId;
        [SerializeField]
        private bool DestroyPOIAtEnd;


        [NonSerialized]
        private CutsceneTimelineActionInput CutsceneTimelineActionInput;
        [NonSerialized]
        private bool isActionEnded;
        [NonSerialized]
        private bool isAgentDestinationReached;


        #region External Dependencies
        [NonSerialized]
        private APointOfInterestEventManager PointOfInterestEventManager;
        #endregion

        public CutsceneId CutsceneId { get => cutsceneId; }

        public CutsceneTimelineAction(CutsceneId cutsceneId, List<SequencedAction> nextContextActions, bool destroyPOIAtEnd = false) : base(nextContextActions)
        {
            this.cutsceneId = cutsceneId;
            this.DestroyPOIAtEnd = destroyPOIAtEnd;
        }

        public override void AfterFinishedEventProcessed()
        {
            isActionEnded = false;
            if (DestroyPOIAtEnd)
            {
                PointOfInterestEventManager.DisablePOI(CutsceneTimelineActionInput.TargetedPOI);
            }
        }

        public override bool ComputeFinishedConditions()
        {
            return isActionEnded;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            Coroutiner.Instance.StartCoroutine(this.PlayCutscene());
            #region External dependencies
            PointOfInterestEventManager = CoreGameSingletonInstances.APointOfInterestEventManager;
            #endregion
            var cutsceneTimelineContextActionInput = (CutsceneTimelineActionInput)ContextActionInput;
            this.CutsceneTimelineActionInput = cutsceneTimelineContextActionInput;
        }

        public override void Tick(float d)
        {
        }


        private IEnumerator PlayCutscene()
        {
            yield return AdventureGameSingletonInstances.CutscenePlayerManagerV2.PlayCutscene(this.cutsceneId);
            isActionEnded = true;
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.cutsceneId = (CutsceneId)NodeEditorGUILayout.EnumField("Cutscene : ", string.Empty, this.cutsceneId);
            this.DestroyPOIAtEnd = NodeEditorGUILayout.BoolField("Destroy POI at end : ", string.Empty, this.DestroyPOIAtEnd);
        }
#endif

    }

    public class CutsceneTimelineActionInput : AContextActionInput
    {
        private PointOfInterestType targetedPOI;

        public CutsceneTimelineActionInput(PointOfInterestType targetedPOI)
        {
            this.targetedPOI = targetedPOI;
        }

        public PointOfInterestType TargetedPOI { get => targetedPOI; }
    }
}