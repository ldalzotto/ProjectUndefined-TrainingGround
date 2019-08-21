//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class NearPlayerGameOverTriggerModule : RTPuzzle.InteractiveObjectModule
    {
        public NearPlayerGameOverTriggerID NearPlayerGameOverTriggerID;

        #region External Dependencies
        private AISightVision AISightVision;
        private BlockingCutscenePlayer BlockingCutscenePlayer;
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        private InteractiveObjectType InteractiveObjectTypeRef;
        private NearPlayerGameOverTriggerInherentData NearPlayerGameOverTriggerInherentData;

        public void Init(AISightVision AISightVision, InteractiveObjectType InteractiveObjectTypeRef)
        {
            this.AISightVision = AISightVision;
            this.BlockingCutscenePlayer = GameObject.FindObjectOfType<BlockingCutscenePlayer>();
            this.PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();

            var PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            this.NearPlayerGameOverTriggerInherentData = PuzzleGameConfigurationManager.NearPlayerGameOverTriggerConfiguration()[this.NearPlayerGameOverTriggerID];

            this.GetComponent<SphereCollider>().radius = this.NearPlayerGameOverTriggerInherentData.NearPlayerDetectionRadius;

            this.InteractiveObjectTypeRef = InteractiveObjectTypeRef;
        }

        private void OnTriggerEnter(Collider other)
        {
            var collisionType = other.GetComponent<CollisionType>();
            if ((collisionType != null && collisionType.IsPlayer) &&
                ((this.AISightVision == null) || (this.AISightVision != null && this.AISightVision.IsPlayerInSight())))
            {
                if (this.NearPlayerGameOverTriggerInherentData.AnimationGraph != null)
                {
                    this.BlockingCutscenePlayer.Play(new PuzzleCutsceneActionInput(GameObject.FindObjectOfType<InteractiveObjectContainer>(), new System.Collections.Generic.Dictionary<PuzzleCutsceneParametersName, object>() {
                    {PuzzleCutsceneParametersName.INTERACTIVE_OBJECT_0, this.InteractiveObjectTypeRef }
                        }), this.NearPlayerGameOverTriggerInherentData.AnimationGraph, this.OnCutsceneEnd);
                }
                else
                {
                    this.OnCutsceneEnd();
                }
            }
        }

        private void OnCutsceneEnd()
        {
            this.PuzzleEventsManager.PZ_EVT_LevelReseted();
        }

        public static class NearPlayerGameOverTriggerModuleInstancer
        {
            public static void PopuplateFromDefinition(NearPlayerGameOverTriggerModule nearPlayerGameOverTriggerModule, NearPlayerGameOverTriggerModuleDefinition nearPlayerGameOverTriggerModuleDefinition)
            {
                nearPlayerGameOverTriggerModule.NearPlayerGameOverTriggerID = nearPlayerGameOverTriggerModuleDefinition.NearPlayerGameOverTriggerID;
            }
        }
    }
}