using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class ContextMarkVisualFeedbackManager
    {

        #region External Dependencies
        private AIObjectType NPCAIManagerRef;
        private FovModule FovModule;
        private PuzzlePrefabConfiguration PuzzlePrefabConfiguration;
        private CoreMaterialConfiguration CoreMaterialConfiguration;
        #endregion

        private Vector3 fallbackPositionOffset;

        public ContextMarkVisualFeedbackManager(AIObjectType NPCAIManagerRef, FovModule fovModule, ModelObjectModule ModelObjectModule,
                    PuzzlePrefabConfiguration PuzzlePrefabConfiguration, CoreMaterialConfiguration CoreMaterialConfiguration)
        {
            this.NPCAIManagerRef = NPCAIManagerRef;
            this.DeleteOperation();
            this.visualFeedbackMark = null;
            this.FovModule = fovModule;
            this.PuzzlePrefabConfiguration = PuzzlePrefabConfiguration;
            this.CoreMaterialConfiguration = CoreMaterialConfiguration;

            this.fallbackPositionOffset = new Vector3(0, ModelObjectModule.GetAverageModelBoundLocalSpace().Bounds.max.y, 0);
        }

        private AIFeedbackMarkType visualFeedbackMark;

        public void Tick(float d)
        {
            if (this.visualFeedbackMark != null)
            {
                Vector3 visualMarkPosition;
                if (this.FovModule != null)
                {
                    visualMarkPosition = this.NPCAIManagerRef.GetAgent().transform.position + this.FovModule.RingPositionOffset;
                    visualMarkPosition.y += (this.FovModule.GetInteractionRingHeight() * 4);
                } else
                {
                    visualMarkPosition = this.NPCAIManagerRef.GetAgent().transform.position + this.fallbackPositionOffset;
                }
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
