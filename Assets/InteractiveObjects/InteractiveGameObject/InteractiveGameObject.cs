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
        private Collider LogicCollider { get; set; }
        public Rigidbody LogicRigidBody { get; private set; }
        
        public Rigidbody PhysicsRigidbody { get; private set; }
        public NavMeshAgent Agent { get; private set; }
        #endregion

        public Collider GetLogicCollider() { return this.LogicCollider; }
        public BoxCollider GetLogicColliderAsBox() { return (BoxCollider)this.LogicCollider; }

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

            this.Agent = InteractiveGameObjectParent.GetComponent<NavMeshAgent>();
            InitAgent();

            this.PhysicsRigidbody = this.InteractiveGameObjectParent.GetComponent<Rigidbody>();
        }

        private void InitAgent()
        {
            if (this.Agent != null)
            {
                this.Agent.updatePosition = false;
                this.Agent.updateRotation = false;
            }
        }

        public void CreateAgent(AIAgentDefinition AIAgentDefinition)
        {
            if (this.Agent == null)
            {
                this.Agent = this.InteractiveGameObjectParent.AddComponent<NavMeshAgent>();
                this.Agent.stoppingDistance = AIAgentDefinition.AgentStoppingDistance;
                this.Agent.height = AIAgentDefinition.AgentHeight;
                this.Agent.acceleration = 99999999f;
                this.Agent.angularSpeed = 99999999f;
            }
            this.InitAgent();
        }

        public void CreateLogicCollider(InteractiveObjectLogicCollider InteractiveObjectLogicCollider)
        {
            if (InteractiveObjectLogicCollider.Enabled)
            {
                var LogicColliderObject = new GameObject("LogicCollider");
                LogicColliderObject.transform.parent = this.InteractiveGameObjectParent.transform;
                LogicColliderObject.transform.localPosition = Vector3.zero;
                LogicColliderObject.transform.localRotation = Quaternion.identity;
                LogicColliderObject.transform.localScale = Vector3.one;

                this.LogicCollider = LogicColliderObject.AddComponent<BoxCollider>();
                this.LogicCollider.isTrigger = true;
                ((BoxCollider)this.LogicCollider).center = InteractiveObjectLogicCollider.LocalCenter;
                ((BoxCollider)this.LogicCollider).size = InteractiveObjectLogicCollider.LocalSize;

                if (InteractiveObjectLogicCollider.HasRigidBody)
                {
                    var rb = LogicColliderObject.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }
        }

        public TransformStruct GetTransform()
        {
            return new TransformStruct(this.InteractiveGameObjectParent.transform);
        }

        public Matrix4x4 GetLocalToWorld() { return this.InteractiveGameObjectParent.transform.localToWorldMatrix; }

        public BoxDefinition GetLogicColliderBoxDefinition()
        {
            return new BoxDefinition(this.GetLogicColliderAsBox());
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
}
