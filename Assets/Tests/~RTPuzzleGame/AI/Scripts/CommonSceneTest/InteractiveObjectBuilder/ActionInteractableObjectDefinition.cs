using CoreGame;
using GameConfigurationID;
using NodeGraph;
using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public static class ActionInteractableObjectDefinition
    {
        public static InteractiveObjectInitialization ActionInteractableObjectOnly(InteractiveObjectTestID interactiveObjectTestID,
                  ActionInteractableObjectInitialization ActionInteractableObjectInitialization)
        {
            var InteractiveObjectInitialization =
                new InteractiveObjectInitialization()
                {
                    InteractiveObjectID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].InteractiveObjectID,
                    InteractiveObjectTypeDefinitionInherentData = new InteractiveObjectTypeDefinitionInherentData()
                    {
                        InteractiveObjectID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].InteractiveObjectID,
                        RangeDefinitionModules = new Dictionary<Type, ScriptableObject>()
                    {
                        {typeof(ActionInteractableObjectModuleDefinition), new ActionInteractableObjectModuleDefinition(){ ActionInteractableObjectID = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID].ActionInteractableObjectID } }
                    },
                        RangeDefinitionModulesActivation = new Dictionary<Type, bool>()
                    {
                        {typeof(ActionInteractableObjectModuleDefinition), true }
                    }
                    },
                    InteractiveObjectInitializationObject = new InteractiveObjectInitializationObject()
                    {
                        ActionInteractableObjectInherentData = ActionInteractableObjectInitialization.ActionInteractableObjectInherentData
                    }
                };
            ActionInteractableObjectInitialization.InitializeTestConfigurations(interactiveObjectTestID);
            InteractiveObjectInitialization.InitializeTestConfigurations(interactiveObjectTestID);
            return InteractiveObjectInitialization;
        }

        public static PuzzleCutsceneGraph DoNothingCutsceneGraph()
        {
            return new PuzzleCutsceneGraph()
            {
                Nodes = new Dictionary<int, NodeProfile>()
                {
                    {0, new CutsceneStartNode() }
                }
            };
        }
    }

    public class ActionInteractableObjectInitialization
    {
        private float interactionRange;
        private PlayerActionId playerActionId;
        private float cooldownTime;
        private int executionAmount;
        private PuzzleCutsceneID puzzleCutsceneID;
        private PuzzleCutsceneGraph playerActionCutsceneGraph;

        public PuzzleCutsceneInherentData PuzzleCutsceneInherentData;
        public CutsceneActionInherentData CutsceneActionInherentData;
        public ActionInteractableObjectInherentData ActionInteractableObjectInherentData;

        public ActionInteractableObjectInitialization(
            float interactionRange,
            PlayerActionId playerActionId, float cooldownTime, int executionAmount,
            PuzzleCutsceneID puzzleCutsceneID, PuzzleCutsceneGraph playerActionCutsceneGraph)
        {
            this.interactionRange = interactionRange;
            this.playerActionId = playerActionId;
            this.cooldownTime = cooldownTime;
            this.executionAmount = executionAmount;
            this.puzzleCutsceneID = puzzleCutsceneID;
            this.playerActionCutsceneGraph = playerActionCutsceneGraph;

            this.PuzzleCutsceneInherentData = new PuzzleCutsceneInherentData() { PuzzleCutsceneGraph = playerActionCutsceneGraph };
            this.CutsceneActionInherentData = new CutsceneActionInherentData()
            {
                PuzzleCutsceneId = puzzleCutsceneID,
                ActionWheelNodeConfigurationId = SelectionWheelNodeConfigurationId.ATTRACTIVE_OBJECT_LAY_WHEEL_CONFIG,
                CoolDownTime = cooldownTime,
                ExecutionAmount = executionAmount
            };
            this.ActionInteractableObjectInherentData = new ActionInteractableObjectInherentData()
            {
                InteractionRange = interactionRange,
                PlayerActionId = playerActionId
            };
        }

        public void InitializeTestConfigurations(InteractiveObjectTestID interactiveObjectTestID)
        {
            var interactiveObjectTestIDTree = InteractiveObjectTestIDTree.InteractiveObjectTestIDs[interactiveObjectTestID];
            var puzzleGameConfiguration = AssetFinder.SafeSingleAssetFind<PuzzleGameConfiguration>("t:" + typeof(PuzzleGameConfiguration));

            puzzleGameConfiguration.PuzzleCutsceneConfiguration.ConfigurationInherentData[this.puzzleCutsceneID] = this.PuzzleCutsceneInherentData;
            puzzleGameConfiguration.PlayerActionConfiguration.ConfigurationInherentData[this.playerActionId] = this.CutsceneActionInherentData;
            puzzleGameConfiguration.ActionInteractableObjectConfiguration.ConfigurationInherentData[interactiveObjectTestIDTree.ActionInteractableObjectID] = ActionInteractableObjectInherentData;
        }
    }
}
