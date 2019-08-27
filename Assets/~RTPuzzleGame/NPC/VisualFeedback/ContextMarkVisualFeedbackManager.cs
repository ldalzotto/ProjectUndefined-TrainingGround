using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class ContextMarkVisualFeedbackManager
    {

        #region External Dependencies
        private AIObjectType NPCAIManagerRef;
        private NpcInteractionRingManager NpcInteractionRingManager;
        private PuzzlePrefabConfiguration PuzzlePrefabConfiguration;
        private CoreMaterialConfiguration CoreMaterialConfiguration;
        #endregion

        public ContextMarkVisualFeedbackManager(AIObjectType NPCAIManagerRef, NpcInteractionRingManager npcFOVRingManager,
                    PuzzlePrefabConfiguration PuzzlePrefabConfiguration, CoreMaterialConfiguration CoreMaterialConfiguration)
        {
            this.NPCAIManagerRef = NPCAIManagerRef;
            this.DeleteOperation();
            this.visualFeedbackMark = null;
            this.NpcInteractionRingManager = npcFOVRingManager;
            this.PuzzlePrefabConfiguration = PuzzlePrefabConfiguration;
            this.CoreMaterialConfiguration = CoreMaterialConfiguration;
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

        public void ReceiveEvent(AbstractContextMarkVisualFeedbackEvent contextMarkVisualFeedbackEvent, AIObjectID aiID)
        {
            if (contextMarkVisualFeedbackEvent.GetType() == typeof(AttractedStartEvent))
            {
                var AttractedStartEvent = (AttractedStartEvent)contextMarkVisualFeedbackEvent;
                this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(this.PuzzlePrefabConfiguration.BaseAIFeedbackMarkType, (AIFeedbackMarkType) =>
                {
                    var modelObjectModuleInstanciated = Object.Instantiate(AttractedStartEvent.ModelObjectModule, AIFeedbackMarkType.transform);
                    modelObjectModuleInstanciated.CleanObjectForFeedbackIcon(this.CoreMaterialConfiguration);
                });
            }
            else if (contextMarkVisualFeedbackEvent.GetType() == typeof(ProjectileHittedFirstTimeEvent))
            {
                this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(PuzzlePrefabConfiguration.ProjectileHitPrefab);
            }
            else if (contextMarkVisualFeedbackEvent.GetType() == typeof(EscapeWithoutTargetEvent))
            {
                this.visualFeedbackMark = AIFeedbackMarkType.Instanciate(PuzzlePrefabConfiguration.EscapeWithoutTargetPrefab);
            }
            else if (contextMarkVisualFeedbackEvent.GetType() == typeof(DeleteEvent))
            {
                this.DeleteOperation();
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
}
