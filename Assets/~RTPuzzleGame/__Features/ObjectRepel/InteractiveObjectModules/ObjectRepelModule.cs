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
        BoxDefinition GetObjectRepelColliderDefinition();
        IObjectRepelModuleEvent GetIObjectRepelModuleEvent();
        IRenderBoundRetrievable GetModelBounds();
        IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval { get; }
        ILineVisualFeedbackEvent ILineVisualFeedbackEvent { get; }
    }

    public class ObjectRepelModule : InteractiveObjectModule, IObjectRepelModuleEvent, IObjectRepelModuleDataRetrieval
    {
        [FormerlySerializedAs("RepelableObjectID")]
        public ObjectRepelID ObjectRepelID;

        #region Module Dependencies
        private ModelObjectModule ModelObjectModule;
        public ILineVisualFeedbackEvent ILineVisualFeedbackEvent { get; private set; }
        #endregion

        #region Internal Dependencies
        private BoxCollider objectRepelCollider;
        #endregion

        public IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval { get; private set; }

        private ObjectRepelTypeAnimationComponent objectRepelTypeAnimationComponent = new ObjectRepelTypeAnimationComponent();

        public override void Init(InteractiveObjectInitializationObject interactiveObjectInitializationObject, IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval,
            IInteractiveObjectTypeEvents IInteractiveObjectTypeEvents)
        {
            this.ModelObjectModule = IInteractiveObjectTypeDataRetrieval.GetModelObjectModule();
            this.ILineVisualFeedbackEvent = IInteractiveObjectTypeDataRetrieval.GetILineVisualFeedbackEvent();

            this.objectRepelCollider = GetComponent<BoxCollider>();
            this.IInteractiveObjectTypeDataRetrieval = IInteractiveObjectTypeDataRetrieval;
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
        public BoxDefinition GetObjectRepelColliderDefinition()
        {
            return new BoxDefinition(this.objectRepelCollider);
        }
        public ObjectRepelID GetObjectRepelID() { return this.ObjectRepelID; }
        public Transform GetTransform() { return this.transform; }
        #endregion
        
    }

    public class ObjectRepelTypeAnimationComponent
    {
        public bool isMoving;
        public float elapsedTime;
        public BeziersControlPoints path;
    }
}

