using CoreGame;
using GameConfigurationID;

namespace AdventureGame
{

    public class _1_Level_Girl_SpecificBehavior : PointOfInterestSpecificBehavior
    {
        public void Init(PointOfInterestModelObjectModule PointOfInterestModelObjectModule, PointOfInterestType pointOfInterestTypeRef)
        {
            AnimationPlayerHelper.Play(PointOfInterestModelObjectModule.Animator, pointOfInterestTypeRef.GetCoreConfigurationManager().AnimationConfiguration().ConfigurationInherentData[AnimationID.BACK_TO_WALL_POSE], 0f);
        }

        public void Tick(float d)
        {
        }
    }

}
