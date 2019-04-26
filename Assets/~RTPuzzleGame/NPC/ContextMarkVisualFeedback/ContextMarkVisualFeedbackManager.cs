using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class ContextMarkVisualFeedbackManager
    {

        private ContextMarkVisualFeedbackManagerComponent ContextMarkVisualFeedbackManagerComponent;

        #region External Dependencies
        private NPCAIManager NPCAIManagerRef;
        private NpcInteractionRingManager NpcInteractionRingManager;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        public ContextMarkVisualFeedbackManager(ContextMarkVisualFeedbackManagerComponent ContextMarkVisualFeedbackManagerComponent,
        NPCAIManager NPCAIManagerRef, NpcInteractionRingManager npcFOVRingManager, PuzzleGameConfigurationManager PuzzleGameConfigurationManager)
        {
            this.ContextMarkVisualFeedbackManagerComponent = ContextMarkVisualFeedbackManagerComponent;
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
                if (this.NpcInteractionRingManager.IsRingEnabled())
                {
                    visualMarkPosition.y += this.ContextMarkVisualFeedbackManagerComponent.YOffsetWhenInteractionRingIsDisplayed;
                }
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
                    this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(this.PuzzleGameConfigurationManager.ContextMarkVisualFeedbackConfiguration()[aiID].AttractedPrafab, this.NPCAIManagerRef.transform);
                    break;
                case ContextMarkVisualFeedbackEvent.PROJECTILE_HITTED_FIRST_TIME:
                    this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(this.PuzzleGameConfigurationManager.ContextMarkVisualFeedbackConfiguration()[aiID].ProjectileHitPrefab, this.NPCAIManagerRef.transform);
                    break;
                case ContextMarkVisualFeedbackEvent.PROJECTILE_HITTED_2_IN_A_ROW:
                    this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(this.PuzzleGameConfigurationManager.ContextMarkVisualFeedbackConfiguration()[aiID].ProjectileSecondHitPrefab, this.NPCAIManagerRef.transform);
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
            this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(visualFeedbackMarkPrefab, this.NPCAIManagerRef.transform);
        }

    }

    [System.Serializable]
    public class ContextMarkVisualFeedbackManagerComponent
    {
        public float YOffsetWhenInteractionRingIsDisplayed;
    }

    public enum ContextMarkVisualFeedbackEvent
    {
        PROJECTILE_HITTED_FIRST_TIME = 0,
        PROJECTILE_HITTED_2_IN_A_ROW = 1,
        ATTRACTED_START = 2,
        DELETE = 3
    }

}
