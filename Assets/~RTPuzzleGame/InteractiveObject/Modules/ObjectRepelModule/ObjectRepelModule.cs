using System;
using CoreGame;
using GameConfigurationID;
using UnityEngine;
using UnityEngine.Serialization;

namespace RTPuzzle
{
    public class ObjectRepelModule : InteractiveObjectModule
    {
        [FormerlySerializedAs("RepelableObjectID")]
        public ObjectRepelID ObjectRepelID;

        #region Module Dependencies
        private ModelObjectModule ModelObjectModule;
        #endregion

        #region Internal Dependencies
        private Collider objectRepelCollider;
        #endregion

        public Collider ObjectRepelCollider { get => objectRepelCollider; }

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

        public void OnObjectRepelRepelled(BeziersControlPoints path)
        {
            this.objectRepelTypeAnimationComponent.path = path;
            objectRepelTypeAnimationComponent.isMoving = true;
            objectRepelTypeAnimationComponent.elapsedTime = 0.1f;
            this.Tick(0, 0);
        }

        #region Data Retrieval
        public IRenderBoundRetrievable GetModelBounds()
        {
            return this.ModelObjectModule;
        }
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

    public static class ObjectRepelTypeModuleEventHandling
    {
        public static void OnObjectRepelRepelled(ObjectRepelModule objectRepelType, Vector3 targetWorldPosition)
        {
            Debug.Log(MyLog.Format(Vector3.Distance(objectRepelType.transform.position, targetWorldPosition)));
            objectRepelType.OnObjectRepelRepelled(BeziersControlPoints.Build(objectRepelType.transform.position, targetWorldPosition, objectRepelType.transform.up, BeziersControlPointsShape.CURVED));
        }
    }
}

