using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class ContextMarkVisualFeedbackModule : InteractiveObjectModule, IContextMarkVisualFeedbackEvent
    {
        #region Module Dependencies
        private IFovModuleDataRetrieval IFovModuleDataRetrieval;
        #endregion

        #region External Dependencies
        private PuzzlePrefabConfiguration PuzzlePrefabConfiguration;
        private CoreMaterialConfiguration CoreMaterialConfiguration;
        #endregion

        private Vector3 fallbackPositionOffset;

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.DeleteOperation();
            this.visualFeedbackMark = null;
            this.IFovModuleDataRetrieval = IInteractiveObjectTypeDataRetrieval.GetIFovModuleDataRetrieval();
            this.PuzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration;
            this.CoreMaterialConfiguration = CoreGameSingletonInstances.CoreStaticConfigurationContainer.CoreStaticConfiguration.CoreMaterialConfiguration;

            this.fallbackPositionOffset = new Vector3(0, IInteractiveObjectTypeDataRetrieval.GetModelObjectModule().GetAverageModelBoundLocalSpace().Bounds.max.y, 0);
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

        #region IContextMarkVisualFeedbackEvent
        public void CreateGenericMark(ModelObjectModule modelObjectModule)
        {
            this.visualFeedbackMark = ContextMarkVisualFeedbackMarkType.Instanciate(this.PuzzlePrefabConfiguration.BaseAIFeedbackMarkType, (AIFeedbackMarkType) =>
            {
                var modelObjectModuleInstanciated = Object.Instantiate(modelObjectModule, AIFeedbackMarkType.transform);
                modelObjectModuleInstanciated.CleanObjectForFeedbackIcon(this.CoreMaterialConfiguration);
            });
        }

        public void CreateExclamationMark()
        {
            this.visualFeedbackMark = ContextMarkVisualFeedbackMarkType.Instanciate(PuzzlePrefabConfiguration.ExclamationMarkContextMarkPrefab);
        }

        public void CreateDoubleExclamationMark()
        {
            this.visualFeedbackMark = ContextMarkVisualFeedbackMarkType.Instanciate(PuzzlePrefabConfiguration.DoubleExclamationMarkPrefab);
        }

        public void Delete()
        {
            this.DeleteOperation();
        }
        #endregion
    }
}
