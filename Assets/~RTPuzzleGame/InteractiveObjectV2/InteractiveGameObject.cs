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
            this.AverageModelBounds = BoundsHelper.GetAverageRendererBounds(InteractiveGameObjectParent.GetComponentsInChildren<Renderer>());
            this.Animator = InteractiveGameObjectParent.GetComponent<Animator>();
            if (this.Animator == null)
            {
                this.Animator = InteractiveGameObjectParent.GetComponentInChildren<Animator>();
            }

            this.InteractiveGameObjectParent = InteractiveGameObjectParent;
            this.Renderers = RendererRetrievableHelper.GetAllRederers(this.InteractiveGameObjectParent, particleRenderers: false);
            this.LogicCollider = InteractiveGameObjectParent.GetComponentInChildren<BoxCollider>();
            this.Agent = InteractiveGameObjectParent.GetComponent<NavMeshAgent>();
        }

        public InteractiveGameObjectTransform GetTransform()
        {
            return new InteractiveGameObjectTransform()
            {
                WorldPosition = this.InteractiveGameObjectParent.transform.position,
                WorldRotation = this.InteractiveGameObjectParent.transform.rotation,
                LossyScale = this.InteractiveGameObjectParent.transform.lossyScale
            };
        }

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

    public struct InteractiveGameObjectTransform
    {
        public Vector3 WorldPosition;
        public Quaternion WorldRotation;
        public Vector3 LossyScale;
    }
}
