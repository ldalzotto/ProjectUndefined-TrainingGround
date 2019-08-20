using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class ModelObjectModule : InteractiveObjectModule, IRenderBoundRetrievable
    {
        #region Properties
        private ExtendedBounds AverageModeBounds;
        private Animator animator;
        private Rigidbody associatedRigidbody;
        #endregion

        #region Data Retrieval
        public ExtendedBounds GetAverageModelBoundLocalSpace()
        {
            return this.AverageModeBounds;
        }
        public Animator Animator { get => animator; }
        public Rigidbody AssociatedRigidbody { get => associatedRigidbody; }
        #endregion

        public void Init()
        {
            this.AverageModeBounds = BoundsHelper.GetAverageRendererBounds(this.GetComponentsInChildren<Renderer>());
            this.animator = GetComponent<Animator>();
            if (this.animator == null)
            {
                this.animator = GetComponentInChildren<Animator>();
            }

            this.associatedRigidbody = GetComponentInParent<Rigidbody>();
        }

        public static class ModelObjectModuleInstancer
        {
            public static void PopuplateFromDefinition(ModelObjectModule modelObjectModule, ModelObjectModuleDefinition modelObjectModuleDefinition)
            {
                GameObject.Instantiate(modelObjectModuleDefinition.ModelPrefab, modelObjectModule.transform);
            }
        }
    }
}
