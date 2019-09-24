using CoreGame;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class IntersectionTests : MonoBehaviour
    {

        private BoxCollider CreateBoxCollider()
        {
            var go = new GameObject();
            go.transform.position = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.rotation = Quaternion.identity;
            var box= go.AddComponent<BoxCollider>();
            box.transform.position = Vector3.zero;
            box.transform.localScale = Vector3.one;
            box.transform.rotation = Quaternion.identity;
            return box;
        }

        private void InitBox(ref BoxCollider box, Vector3 size, Quaternion quaternion)
        {
            box.size = size;
            box.transform.rotation = quaternion;
        }

        [Test]
        public void NonRotateCube()
        {
            var sphereWorldPosition = Vector3.zero;
            var sphereRadius = 0.5f;

            BoxCollider box = this.CreateBoxCollider();
            this.InitBox(ref box, Vector3.one, Quaternion.identity);
            
            this.AssertBox(Vector3.forward, 0.1f, ref box, sphereWorldPosition, sphereRadius);
            this.AssertBox(Vector3.back, 0.1f, ref box, sphereWorldPosition, sphereRadius);
            this.AssertBox(Vector3.up, 0.1f, ref box, sphereWorldPosition, sphereRadius);
            this.AssertBox(Vector3.down, 0.1f, ref box, sphereWorldPosition, sphereRadius);
            this.AssertBox(Vector3.left, 0.1f, ref box, sphereWorldPosition, sphereRadius);
            this.AssertBox(Vector3.right, 0.1f, ref box, sphereWorldPosition, sphereRadius);
        }

        [Test]
        public void WithRotateCube()
        {
            var sphereWorldPosition = Vector3.zero;
            var sphereRadius = 0.5f;

            BoxCollider box = this.CreateBoxCollider();
            this.InitBox(ref box, Vector3.one, Quaternion.Euler(90f,90f,90f));
            
            this.AssertBox(Vector3.forward, 0.1f, ref box, sphereWorldPosition, sphereRadius);
            this.AssertBox(Vector3.back, 0.1f, ref box, sphereWorldPosition, sphereRadius);
            this.AssertBox(Vector3.up, 0.1f, ref box, sphereWorldPosition, sphereRadius);
            this.AssertBox(Vector3.down, 0.1f, ref box, sphereWorldPosition, sphereRadius);
            this.AssertBox(Vector3.left, 0.1f, ref box, sphereWorldPosition, sphereRadius);
            this.AssertBox(Vector3.right, 0.1f, ref box, sphereWorldPosition, sphereRadius);
        }

        private void AssertBox(Vector3 moveDirection, float directionDelta, ref BoxCollider box, Vector3 sphereWorldPosition, float sphereRadius) {

            box.transform.position = moveDirection;
            Assert.IsTrue(Intersection.BoxIntersectsOrEntirelyContainedInSphere(box.center, box.size, box.transform.localToWorldMatrix, sphereWorldPosition, sphereRadius));

            box.transform.position = moveDirection + (moveDirection * (-1 * directionDelta));
            Assert.IsTrue(Intersection.BoxIntersectsOrEntirelyContainedInSphere(box.center, box.size, box.transform.localToWorldMatrix, sphereWorldPosition, sphereRadius));

            box.transform.position = moveDirection + (moveDirection * directionDelta);
            Assert.IsFalse(Intersection.BoxIntersectsOrEntirelyContainedInSphere(box.center, box.size, box.transform.localToWorldMatrix, sphereWorldPosition, sphereRadius));
        }

        [Test]
        public void CubeEntirelyContainedInSphere()
        {
            var sphereWorldPosition = Vector3.zero;
            var sphereRadius = 9999f;

            BoxCollider box = this.CreateBoxCollider();
            this.InitBox(ref box, Vector3.one, Quaternion.identity);
            box.transform.position = Vector3.zero;

            Assert.IsTrue(Intersection.BoxIntersectsOrEntirelyContainedInSphere(box.center, box.size, box.transform.localToWorldMatrix, sphereWorldPosition, sphereRadius));
        }

    }

}
