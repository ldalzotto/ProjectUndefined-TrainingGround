using CoreGame;
using GameConfigurationID;
using UnityEngine;
using UnityEngine.Serialization;

namespace RTPuzzle
{
    public interface IObjectRepelModuleDataRetrieval
    {
        ObjectRepelID GetObjectRepelID();
        Transform GetTransform();
        Collider GetObjectRepelCollider();
        IObjectRepelModuleEvent GetIObjectRepelModuleEvent();
        IRenderBoundRetrievable GetModelBounds();
    }

    public class ObjectRepelModule : InteractiveObjectModule, IObjectRepelModuleEvent, IObjectRepelModuleDataRetrieval
    {
        [FormerlySerializedAs("RepelableObjectID")]
        public ObjectRepelID ObjectRepelID;

        #region Module Dependencies
        private ModelObjectModule ModelObjectModule;
        #endregion

        #region Internal Dependencies
        private Collider objectRepelCollider;
        #endregion

        private ObjectRepelTypeAnimationComponent objectRepelTypeAnimationComponent = new ObjectRepelTypeAnimationComponent();

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.ModelObjectModule = IInteractiveObjectTypeDataRetrieval.GetModelObjectModule();
            this.objectRepelCollider = GetComponent<Collider>();
        }

        public static ObjectRepelModule FromCollisionType(CollisionType collisionType)
        {
            if (collisionType.IsRepelable)
            {
                return collisionType.GetComponent<ObjectRepelModule>();
            }
            return null;
        }

        public void Tick(float d, float timeAttenuation)
        {
            if (this.objectRepelTypeAnimationComponent.isMoving)
            {
                this.objectRepelTypeAnimationComponent.elapsedTime += d * timeAttenuation;
                this.objectRepelTypeAnimationComponent.elapsedTime = Mathf.Min(this.objectRepelTypeAnimationComponent.elapsedTime, 1f);
                this.SetPosition(this.objectRepelTypeAnimationComponent.path.ResolvePoint(this.objectRepelTypeAnimationComponent.elapsedTime));
                if (this.objectRepelTypeAnimationComponent.elapsedTime >= 1f)
                {
                    this.SetPosition(this.objectRepelTypeAnimationComponent.path.P3);
                    this.objectRepelTypeAnimationComponent.isMoving = false;
                }
            }
        }

        private void SetPosition(Vector3 worldPosition)
        {
            transform.parent.transform.position = worldPosition;
        }

        public void OnObjectRepelRepelled(Vector3 targetWorldPosition)
        {
            this.objectRepelTypeAnimationComponent.path = BeziersControlPoints.Build(this.transform.position, targetWorldPosition, this.transform.up, BeziersControlPointsShape.CURVED);
            objectRepelTypeAnimationComponent.isMoving = true;
            objectRepelTypeAnimationComponent.elapsedTime = 0.1f;
            this.Tick(0, 0);
        }

        #region Data Retrieval
        public IRenderBoundRetrievable GetModelBounds()
        {
            return this.ModelObjectModule;
        }
        public IObjectRepelModuleEvent GetIObjectRepelModuleEvent() { return this; }
        public Collider GetObjectRepelCollider()
        {
            return this.objectRepelCollider;
        }
        public ObjectRepelID GetObjectRepelID() { return this.ObjectRepelID; }
        public Transform GetTransform() { return this.transform; }
        #endregion

        public static class ObjectRepelModuleInstancer
        {
            public static void PopuplateFromDefinition(ObjectRepelModule objectRepelModule, ObjectRepelModuleDefinition objectRepelModuleDefinition)
            {
                objectRepelModule.ObjectRepelID = objectRepelModuleDefinition.ObjectRepelID;
            }
        }
    }

    public class ObjectRepelTypeAnimationComponent
    {
        public bool isMoving;
        public float elapsedTime;
        public BeziersControlPoints path;
    }
}

