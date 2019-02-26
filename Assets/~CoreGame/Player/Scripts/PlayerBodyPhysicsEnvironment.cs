using UnityEngine;

namespace CoreGame
{
    public class PlayerBodyPhysicsEnvironment
    {

        private GroundRayCaster GroundRayCaster;
        private StickGroundBodyPositioner StickGroundBodyPositioner;
        private SlopeVelocityAdjuster SlopeVelocityAdjuster;

        public PlayerBodyPhysicsEnvironment(Rigidbody rigidbody, BodyGroundStickContactDistance BodyGroundStickContactDistance)
        {
            GroundRayCaster = new GroundRayCaster(rigidbody);
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
        private RaycastHit hit;

        public GroundRayCaster(Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
        }

        public void Tick(float d)
        {
            int layerMask = 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER);
            Physics.Raycast(rigidbody.position, Vector3.down, out hit, 10f, layerMask);
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
                Debug.Log("MOVE");
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
            var slopeQuaternion = Quaternion.FromToRotation(Vector3.up, hitNormal);
            rigidbody.velocity = slopeQuaternion * rigidbody.velocity;
            Debug.DrawRay(rigidbody.position, rigidbody.velocity * 10, Color.red);
        }
    }
}