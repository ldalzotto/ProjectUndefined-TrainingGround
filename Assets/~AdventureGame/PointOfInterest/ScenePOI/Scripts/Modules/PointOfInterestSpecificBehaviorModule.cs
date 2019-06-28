using GameConfigurationID;
using System.Collections.Generic;

namespace AdventureGame
{
    public class PointOfInterestSpecificBehaviorModule : APointOfInterestModule
    {
        private static Dictionary<PointOfInterestId, PointOfInterestSpecificBehavior> specificBehaviors = new Dictionary<PointOfInterestId, PointOfInterestSpecificBehavior>()
        {
            {PointOfInterestId._1_Town_Girl,  new _1_Level_Girl_SpecificBehavior() }
        };

        private PointOfInterestSpecificBehavior PointOfInterestSpecificBehavior;

        public override void Init(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModules pointOfInteresetModules)
        {
            base.Init(pointOfInterestTypeRef, pointOfInteresetModules);
            this.PointOfInterestSpecificBehavior = specificBehaviors[pointOfInterestTypeRef.PointOfInterestId];
            this.PointOfInterestSpecificBehavior.Init(this, pointOfInterestTypeRef, pointOfInteresetModules);
        }

        public override void Tick(float d)
        {
            this.PointOfInterestSpecificBehavior.Tick(d);
        }
    }

    public interface PointOfInterestSpecificBehavior
    {
        void Init(PointOfInterestSpecificBehaviorModule associatedModule, PointOfInterestType pointOfInterestTypeRef, PointOfInterestModules pointOfInteresetModules);
        void Tick(float d);
    }

}
