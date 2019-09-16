using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class ContextMarkVisualFeedbackModule : InteractiveObjectModule
    {
        #region Module Dependencies
        private IFovModuleDataRetrieval IFovModuleDataRetrieval;
        #endregion

        #region External Dependencies
        private PuzzlePrefabConfiguration PuzzlePrefabConfiguration;
        private CoreMaterialConfiguration CoreMaterialConfiguration;
        #endregion

        private Vector3 fallbackPositionOffset;

        public void Init(IFovModuleDataRetrieval IFovModuleDataRetrieval, IRenderBoundRetrievable IRenderBoundRetrievable)
        {
            this.DeleteOperation();
            this.visualFeedbackMark = null;
            this.IFovModuleDataRetrieval = IFovModuleDataRetrieval;
            this.PuzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration;
            this.CoreMaterialConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CoreMaterialConfiguration;

            this.fallbackPositionOffset = new Vector3(0, IRenderBoundRetrievable.GetAverageModelBoundLocalSpace().Bounds.max.y, 0);
        }

        private ContextMarkVisualFeedbackMarkType visualFeedbackMark;

        public void TickAlways(float d)
        {
            if (this.visualFeedbackMark != null)
            {
                Vector3 visualMarkPosition;
                if (this.IFovModuleDataRetrieval != null)
                {
                    visualMarkPosition = this.transform.position + this.IFovModuleDataRetrieval.GetRingPositionOffset();
                    visualMarkPosition.y += (this.IFovModuleDataRetrieval.GetInteractionRingHeight() * 4);
                }
                else
                {
                    visualMarkPosition = this.transform.position + this.fallbackPositionOffset;
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
                this.visualFeedbackMark = ContextMarkVisualFeedbackMarkType.Instanciate(this.PuzzlePrefabConfiguration.BaseAIFeedbackMarkType, (AIFeedbackMarkType) =>
                {
                    var modelObjectModuleInstanciated = Object.Instantiate(AttractedStartEvent.ModelObjectModule, AIFeedbackMarkType.transform);
                    modelObjectModuleInstanciated.CleanObjectForFeedbackIcon(this.CoreMaterialConfiguration);
                });
            }
            else if (contextMarkVisualFeedbackEvent.GetType() == typeof(ProjectileHittedFirstTimeEvent))
            {
                this.visualFeedbackMark = ContextMarkVisualFeedbackMarkType.Instanciate(PuzzlePrefabConfiguration.ProjectileHitPrefab);
            }
            else if (contextMarkVisualFeedbackEvent.GetType() == typeof(EscapeWithoutTargetEvent))
            {
                this.visualFeedbackMark = ContextMarkVisualFeedbackMarkType.Instanciate(PuzzlePrefabConfiguration.EscapeWithoutTargetPrefab);
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

        private void CreateOperation(ContextMarkVisualFeedbackMarkType visualFeedbackMarkPrefab)
        {
            this.visualFeedbackMark = ContextMarkVisualFeedbackMarkType.Instanciate(visualFeedbackMarkPrefab);
        }
    }
}
