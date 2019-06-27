using UnityEngine;

namespace AdventureGame
{
    public abstract class APointOfInterestModule : MonoBehaviour
    {

        protected PointOfInterestType pointOfInterestTypeRef;
        protected PointOfInterestModules pointOfInteresetModules;

        public virtual void Init(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModules pointOfInteresetModules)
        {
            this.pointOfInterestTypeRef = pointOfInterestTypeRef;
            this.pointOfInteresetModules = pointOfInteresetModules;
        }
        public abstract void Tick(float d);
    }


}
