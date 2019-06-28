using CoreGame;
using GameConfigurationID;

namespace AdventureGame
{

    public class _1_Level_Girl_SpecificBehavior : PointOfInterestSpecificBehavior
    {
        public void Init(PointOfInterestSpecificBehaviorModule associatedModule, PointOfInterestType pointOfInterestTypeRef, PointOfInterestModules pointOfInteresetModules)
        {
            var PointOfInterestModelObjectModule = pointOfInteresetModules.GetModule<PointOfInterestModelObjectModule>();
            AnimationPlayerHelper.Play(PointOfInterestModelObjectModule.Animator, associatedModule.PointOfInterestTypeRef.GetCoreConfigurationManager().AnimationConfiguration().ConfigurationInherentData[AnimationID.BACK_TO_WALL_POSE], 0f);
        }

        public void Tick(float d)
        {
        }
    }

}
