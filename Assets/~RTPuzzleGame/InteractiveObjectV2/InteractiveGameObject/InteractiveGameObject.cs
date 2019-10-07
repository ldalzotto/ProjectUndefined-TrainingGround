using CoreGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace InteractiveObjectTest
{
    public class InteractiveGameObject
    {
        public GameObject InteractiveGameObjectParent { get; private set; }

        #region Properties
        public ExtendedBounds AverageModelBounds { get; private set; }
        public Animator Animator { get; private set; }
        public List<Renderer> Renderers { get; private set; }
        public BoxCollider LogicCollider { get; private set; }
        public NavMeshAgent Agent { get; private set; }
        #endregion

        public InteractiveGameObject(GameObject InteractiveGameObjectParent)
        {
            var childRenderers = InteractiveGameObjectParent.GetComponentsInChildren<Renderer>();
            if (childRenderers != null)
            {
                this.AverageModelBounds = BoundsHelper.GetAverageRendererBounds(childRenderers);
            }

            this.Animator = InteractiveGameObjectParent.GetComponent<Animator>();
            if (this.Animator == null)
            {
                this.Animator = InteractiveGameObjectParent.GetComponentInChildren<Animator>();
            }

            this.InteractiveGameObjectParent = InteractiveGameObjectParent;
            this.Renderers = RendererRetrievableHelper.GetAllRederers(this.InteractiveGameObjectParent, particleRenderers: false);
            var InteractiveGameObjectLogicColliderTag = InteractiveGameObjectParent.GetComponentInChildren<InteractiveGameObjectLogicColliderTag>();
            if (InteractiveGameObjectLogicColliderTag != null)
            {
                this.LogicCollider = InteractiveGameObjectLogicColliderTag.GetComponent<BoxCollider>();
                MonoBehaviour.Destroy(InteractiveGameObjectLogicColliderTag);
            }
            this.Agent = InteractiveGameObjectParent.GetComponent<NavMeshAgent>();
        }

        public TransformStruct GetTransform()
        {
            return new TransformStruct(this.InteractiveGameObjectParent.transform);
        }

        public Matrix4x4 GetLocalToWorld() { return this.InteractiveGameObjectParent.transform.localToWorldMatrix; }

        public void CleanObjectForFeedbackIcon(CoreMaterialConfiguration CoreMaterialConfiguration)
        {
            this.InteractiveGameObjectParent.transform.localScale = Vector3.one;
            this.InteractiveGameObjectParent.FindOneLevelDownChilds().ForEach(child => child.transform.ResetScale());

            this.InteractiveGameObjectParent.transform.localPosition = Vector3.zero;
            this.InteractiveGameObjectParent.transform.localRotation = Quaternion.identity;
            var animatorsToDestroy = InteractiveGameObjectParent.transform.GetComponentsInCurrentAndChild<Animator>();
            if (animatorsToDestroy != null) { foreach (var animatorToDestroy in animatorsToDestroy) { MonoBehaviour.Destroy(animatorToDestroy); } }
            var particleSystemsToDestroy = InteractiveGameObjectParent.transform.GetComponentsInCurrentAndChild<ParticleSystem>();
            if (particleSystemsToDestroy != null) { foreach (var particleSystemToDestroy in particleSystemsToDestroy) { MonoBehaviour.Destroy(particleSystemToDestroy); } }
            var allMeshRenderes = InteractiveGameObjectParent.transform.GetComponentsInCurrentAndChild<MeshRenderer>();
            if (allMeshRenderes != null) { foreach (var meshRenderer in allMeshRenderes) { meshRenderer.materials = new Material[1] { CoreMaterialConfiguration.UnlitVertexColor }; } }
        }
    }
}
