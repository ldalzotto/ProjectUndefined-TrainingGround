using GameConfigurationID;

namespace AdventureGame
{
    public class PointOfInterestSpecificBehaviorModule : APointOfInterestModule
    {

        private PointOfInterestSpecificBehavior PointOfInterestSpecificBehavior;

        public void Init(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModelObjectModule PointOfInterestModelObjectModule)
        {
            switch (pointOfInterestTypeRef.PointOfInterestId)
            {
                case PointOfInterestId._1_Town_Girl:
                   // this.PointOfInterestSpecificBehavior = new _1_Level_Girl_SpecificBehavior();
                   // ((_1_Level_Girl_SpecificBehavior)this.PointOfInterestSpecificBehavior).Init(PointOfInterestModelObjectModule, pointOfInterestTypeRef);
                    break;
                default:
                    break;
            }
        }

        public void Tick(float d)
        {
            if (this.PointOfInterestSpecificBehavior != null)
            {
                this.PointOfInterestSpecificBehavior.Tick(d);
            }
        }
    }

    public interface PointOfInterestSpecificBehavior
    {
        void Tick(float d);
    }

}
