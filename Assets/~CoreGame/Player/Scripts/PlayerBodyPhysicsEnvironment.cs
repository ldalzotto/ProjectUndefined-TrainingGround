using UnityEngine;

namespace CoreGame
{
    public class PlayerBodyPhysicsEnvironment
    {

        private GroundRayCaster GroundRayCaster;
        private StickGroundBodyPositioner StickGroundBodyPositioner;
        private SlopeVelocityAdjuster SlopeVelocityAdjuster;

        public PlayerBodyPhysicsEnvironment(Rigidbody rigidbody, Collider collider, BodyGroundStickContactDistance BodyGroundStickContactDistance)
        {
            GroundRayCaster = new GroundRayCaster(rigidbody, collider);
            StickGroundBodyPositioner = new StickGroundBodyPositioner(BodyGroundStickContactDistance, rigidbody);
            SlopeVelocityAdjuster = new SlopeVelocityAdjuster(rigidbody);
        }

        public void Tick(float d)
        {
            GroundRayCaster.Tick(d);
        }


        public void FixedTick(float d)
        {
            if (GroundRayCaster.HasHitted())
            {
                //slope velocity adjusted
                SlopeVelocityAdjuster.FixedTick(GroundRayCaster.GetHitNormal());

                //stick to ground
                StickGroundBodyPositioner.FixedTick(GroundRayCaster.GetHitPosition());
            }
        }

    }

    class GroundRayCaster
    {
        private Rigidbody rigidbody;
        private Collider collider;
        private RaycastHit hit;

        public GroundRayCaster(Rigidbody rigidbody, Collider collider)
        {
            this.rigidbody = rigidbody;
            this.collider = collider;
        }

        public void Tick(float d)
        {
            PhysicsHelper.RaycastToDownVertically(collider, rigidbody, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER), out hit);
        }

        public bool HasHitted()
        {
            return hit.collider != null;
        }

        public Vector3 GetHitPosition()
        {
            return hit.point;
        }

        public Vector3 GetHitNormal()
        {
            return hit.normal;
        }
    }


    class StickGroundBodyPositioner
    {
        private Rigidbody rigidbody;
        private BodyGroundStickContactDistance BodyGroundStickContactDistance;

        public StickGroundBodyPositioner(BodyGroundStickContactDistance BodyGroundStickContactDistance, Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
            this.BodyGroundStickContactDistance = BodyGroundStickContactDistance;
        }

        public void FixedTick(Vector3 hitPosition)
        {
            if (Vector3.Distance(rigidbody.position, hitPosition) > BodyGroundStickContactDistance.ContactDistance)
            {
                rigidbody.MovePosition(hitPosition);
            }
        }
    }

    [System.Serializable]
    public class BodyGroundStickContactDistance
    {
        public float ContactDistance;
    }

    class SlopeVelocityAdjuster
    {
        private Rigidbody rigidbody;

        public SlopeVelocityAdjuster(Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
        }

        public void FixedTick(Vector3 hitNormal)
        {
            // (1) - We project the normal in the current direction of rigid body -> this allow to take player orientation while getting on slope.
            var rigidBodyProjectedNormal = Vector3.ProjectOnPlane(hitNormal, rigidbody.transform.right);
            var slopeQuaternion = Quaternion.FromToRotation(rigidbody.transform.up, rigidBodyProjectedNormal);
            // velocity vector is rotated
            rigidbody.velocity = slopeQuaternion * rigidbody.velocity;

            Debug.DrawRay(rigidbody.position, rigidBodyProjectedNormal * 10, Color.green);
            Debug.DrawRay(rigidbody.position, rigidbody.velocity * 10, Color.red);

        }
    }
}