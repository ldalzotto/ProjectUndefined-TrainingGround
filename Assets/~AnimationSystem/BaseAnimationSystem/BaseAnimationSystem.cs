using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimationSystem
{
    public class BaseAnimationSystem : MonoBehaviour
    {
        #region Internal Dependencies
        private BoxCollider Trigger;
        protected List<AnimationActor> animationActors;
        #endregion

        #region State
        private bool lastFrameSeenByCamera;
        #endregion

        private Plane[] planes;

        public void Init()
        {
            this.Trigger = GetComponent<BoxCollider>();
            this.planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

            this.animationActors = GetComponentsInChildren<AnimationActor>().ToList();
            this.animationActors.ForEach(a => a.Init());
        }

        public void Tick(float d)
        {
            GeometryUtility.CalculateFrustumPlanes(Camera.main, this.planes);

            var currentlySeenByCamera = GeometryUtility.TestPlanesAABB(planes, this.Trigger.bounds);
            if (currentlySeenByCamera)
            {
                if (!this.lastFrameSeenByCamera)
                {
                    this.OnVisible();
                }
            }
            else
            {
                if (this.lastFrameSeenByCamera)
                {
                    this.OnNotVisible();
                }
            }

            if (currentlySeenByCamera)
            {
                this.SeenByCameraTick(d);
            }

            this.lastFrameSeenByCamera = currentlySeenByCamera;
        }
        
        private void SeenByCameraTick(float d)
        {
            this.animationActors.ForEach(a => a.SeenByCameraTick(d));
        }

        private void OnVisible()
        {
            this.animationActors.ForEach(a => a.OnVisible());
        }

        private void OnNotVisible()
        {
            this.animationActors.ForEach(a => a.OnNotVisible());
        }
    }

}
