using System.Collections.Generic;
using CoreGame;
using UnityEngine;
using UnityEngine.AI;

namespace InteractiveObjects
{
    public interface IInteractiveGameObject
    {
        GameObject InteractiveGameObjectParent { get; }
        ExtendedBounds AverageModelBounds { get; }
        Animator Animator { get; }
        List<Renderer> Renderers { get; }
        Collider LogicCollider { get; }
        Rigidbody LogicRigidBody { get; }
        Rigidbody PhysicsRigidbody { get; }
        NavMeshAgent Agent { get; }

        BoxCollider GetLogicColliderAsBox();
        TransformStruct GetTransform();
        TransformStruct GetLogicColliderCenterTransform();
        Matrix4x4 GetLocalToWorld();
        BoxDefinition GetLogicColliderBoxDefinition();

        void CreateAgent(AIAgentDefinition AIAgentDefinition);
        void CreateLogicCollider(InteractiveObjectLogicCollider InteractiveObjectLogicCollider);
    }

    public static class InteractiveGameObjectFactory
    {
        public static IInteractiveGameObject Build(GameObject InteractiveGameObjectParent)
        {
            return new InteractiveGameObject(InteractiveGameObjectParent);
        }
    }

    internal class InteractiveGameObject : IInteractiveGameObject
    {
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

        public GameObject InteractiveGameObjectParent { get; private set; }

        public BoxCollider GetLogicColliderAsBox()
        {
            return (BoxCollider) this.LogicCollider;
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
                ((BoxCollider) this.LogicCollider).center = InteractiveObjectLogicCollider.LocalCenter;
                ((BoxCollider) this.LogicCollider).size = InteractiveObjectLogicCollider.LocalSize;

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

        public TransformStruct GetLogicColliderCenterTransform()
        {
            var returnTransform = this.GetTransform();
            if (this.LogicCollider != null)
            {
                switch (this.LogicCollider)
                {
                    case BoxCollider logicBoxCollider:
                        returnTransform = new TransformStruct() {WorldPosition = returnTransform.WorldPosition + logicBoxCollider.center, WorldRotation = returnTransform.WorldRotation, LossyScale = returnTransform.LossyScale};
                        break;
                    default:
                        break;
                }
            }

            return returnTransform;
        }

        public Matrix4x4 GetLocalToWorld()
        {
            return this.InteractiveGameObjectParent.transform.localToWorldMatrix;
        }

        public BoxDefinition GetLogicColliderBoxDefinition()
        {
            return new BoxDefinition(this.GetLogicColliderAsBox());
        }

        private void InitAgent()
        {
            if (this.Agent != null)
            {
                this.Agent.updatePosition = false;
                this.Agent.updateRotation = false;
            }
        }

        #region Properties

        public ExtendedBounds AverageModelBounds { get; private set; }
        public Animator Animator { get; private set; }
        public List<Renderer> Renderers { get; private set; }
        public Collider LogicCollider { get; private set; }
        public Rigidbody LogicRigidBody { get; private set; }

        public Rigidbody PhysicsRigidbody { get; private set; }
        public NavMeshAgent Agent { get; private set; }

        #endregion
    }
}