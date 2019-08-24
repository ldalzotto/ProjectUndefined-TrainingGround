using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameConfigurationID;

namespace RTPuzzle
{
    public class ContextMarkVisualFeedbackManager
    {

        #region External Dependencies
        private AIObjectType NPCAIManagerRef;
        private NpcInteractionRingManager NpcInteractionRingManager;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        public ContextMarkVisualFeedbackManager(AIObjectType NPCAIManagerRef, NpcInteractionRingManager npcFOVRingManager, PuzzleGameConfigurationManager PuzzleGameConfigurationManager)
        {
            this.NPCAIManagerRef = NPCAIManagerRef;
            this.DeleteOperation();
            this.visualFeedbackMark = null;
            this.NpcInteractionRingManager = npcFOVRingManager;
            this.PuzzleGameConfigurationManager = PuzzleGameConfigurationManager;
        }

        private AIFeedbackMarkType visualFeedbackMark;

        public void Tick(float d)
        {
            if (this.visualFeedbackMark != null)
            {
                var visualMarkPosition = this.NPCAIManagerRef.GetAgent().transform.position + this.NPCAIManagerRef.GetInteractionRingOffset();
                visualMarkPosition.y += (this.NpcInteractionRingManager.GetInteractionRingHeight() * 4);
                this.visualFeedbackMark.transform.position = visualMarkPosition;
                this.visualFeedbackMark.Tick(d);
            }
        }

        public void ReceiveEvent(ContextMarkVisualFeedbackEvent contextMarkVisualFeedbackEvent, AiID aiID)
        {
            this.DeleteOperation();
            switch (contextMarkVisualFeedbackEvent)
            {
                case ContextMarkVisualFeedbackEvent.ATTRACTED_START:
                    this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(this.PuzzleGameConfigurationManager.ContextMarkVisualFeedbackConfiguration()[aiID].AttractedPrafab);
                    break;
                case ContextMarkVisualFeedbackEvent.PROJECTILE_HITTED_FIRST_TIME:
                    this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(this.PuzzleGameConfigurationManager.ContextMarkVisualFeedbackConfiguration()[aiID].ProjectileHitPrefab);
                    break;
                case ContextMarkVisualFeedbackEvent.ESCAPE_WITHOUT_TARGET:
                    this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(this.PuzzleGameConfigurationManager.ContextMarkVisualFeedbackConfiguration()[aiID].EscapeWithoutTargetPrefab);
                    break;
            }
        }

        private void DeleteOperation()
        {
            if (this.visualFeedbackMark != null)
            {
                MonoBehaviour.Destroy(this.visualFeedbackMark.gameObject);
            }
        }

        private void CreateOperation(AIFeedbackMarkType visualFeedbackMarkPrefab)
        {
            this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(visualFeedbackMarkPrefab);
        }

    }


    public enum ContextMarkVisualFeedbackEvent
    {
        PROJECTILE_HITTED_FIRST_TIME = 0,
        ESCAPE_WITHOUT_TARGET = 1,
        ATTRACTED_START = 2,
        DELETE = 3
    }

}
