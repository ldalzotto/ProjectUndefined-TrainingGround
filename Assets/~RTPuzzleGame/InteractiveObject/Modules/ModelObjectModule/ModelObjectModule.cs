using CoreGame;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTPuzzle
{
    public class ModelObjectModule : InteractiveObjectModule, IRenderBoundRetrievable
    {
        #region Properties
        private ExtendedBounds AverageModeBounds;
        private Animator animator;
        private Rigidbody associatedRigidbody;
        private List<MeshRenderer> meshRenderers;
        #endregion

        #region Data Retrieval
        public ExtendedBounds GetAverageModelBoundLocalSpace()
        {
            return this.AverageModeBounds;
        }
        public Animator Animator { get => animator; }
        public Rigidbody AssociatedRigidbody { get => associatedRigidbody; }
        public List<MeshRenderer> MeshRenderers { get => meshRenderers; }
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
            this.meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
        }

        public void CleanObjectForFeedbackIcon(CoreMaterialConfiguration CoreMaterialConfiguration)
        {
            this.transform.localScale = Vector3.one;
            this.gameObject.FindOneLevelDownChilds().ForEach(child => child.transform.ResetScale());

            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            var animatorsToDestroy = this.GetComponentsInCurrentAndChild<Animator>();
            if (animatorsToDestroy != null) { foreach (var animatorToDestroy in animatorsToDestroy) { MonoBehaviour.Destroy(animatorToDestroy); } }
            var particleSystemsToDestroy = this.GetComponentsInCurrentAndChild<ParticleSystem>();
            if (particleSystemsToDestroy != null) { foreach (var particleSystemToDestroy in particleSystemsToDestroy) { MonoBehaviour.Destroy(particleSystemToDestroy); } }
            var allMeshRenderes = this.GetComponentsInCurrentAndChild<MeshRenderer>();
            if (allMeshRenderes != null) { foreach (var meshRenderer in allMeshRenderes) { meshRenderer.materials = new Material[1] { CoreMaterialConfiguration.UnlitVertexColor }; } }
        }

        public static class ModelObjectModuleInstancer
        {
            public static void PopuplateFromDefinition(ModelObjectModule modelObjectModule, ModelObjectModuleDefinition modelObjectModuleDefinition)
            {
                if (modelObjectModuleDefinition.ModelPrefab != null)
                {
                    GameObject.Instantiate(modelObjectModuleDefinition.ModelPrefab, modelObjectModule.transform);
                }
            }
        }
    }
}
