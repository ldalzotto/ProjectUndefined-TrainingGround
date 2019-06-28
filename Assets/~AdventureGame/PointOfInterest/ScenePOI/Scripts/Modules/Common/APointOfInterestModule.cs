using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{
    public abstract class APointOfInterestModule : MonoBehaviour
    {

        protected PointOfInterestType pointOfInterestTypeRef;
        protected PointOfInterestModules pointOfInteresetModules;

        public PointOfInterestType PointOfInterestTypeRef { get => pointOfInterestTypeRef; }

        public virtual void Init(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModules pointOfInteresetModules)
        {
            this.pointOfInterestTypeRef = pointOfInterestTypeRef;
            this.pointOfInteresetModules = pointOfInteresetModules;
        }
        
        public abstract void Tick(float d);
        public virtual void LateTick(float d) { }

        #region External Events
        public virtual void OnPOIInit() { }
        public virtual void OnPOIDisabled(APointOfInterestType pointOfInterestType) { }
        #endregion
    }


}
