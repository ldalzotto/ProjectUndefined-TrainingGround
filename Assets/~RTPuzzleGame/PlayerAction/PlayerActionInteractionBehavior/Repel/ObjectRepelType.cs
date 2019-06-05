using UnityEngine;

namespace RTPuzzle
{
    public class ObjectRepelType : MonoBehaviour
    {
        private Collider objectRepelCollider;

        public Collider ObjectRepelCollider { get => objectRepelCollider; }

        private ObjectRepelTypeAnimationComponent objectRepelTypeAnimationComponent = new ObjectRepelTypeAnimationComponent();

        public void Init()
        {
            this.objectRepelCollider = GetComponent<Collider>();
        }

        public static ObjectRepelType FromCollisionType(CollisionType collisionType)
        {
            if (collisionType.IsRepelable)
            {
                return collisionType.GetComponent<ObjectRepelType>();
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
    }

    public class ObjectRepelTypeAnimationComponent
    {
        public bool isMoving;
        public float elapsedTime;
        public BeziersControlPoints path;
    }
}

