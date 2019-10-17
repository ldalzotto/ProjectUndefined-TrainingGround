using CoreGame;
using UnityEngine;
using UnityEngine.AI;

namespace AdventureGame
{
    public class PointOfInterestCutsceneControllerModule : APointOfInterestModule
    {
        #region External Dependencies

        private CutscenePlayerManagerV2 CutscenePlayerManagerV2;

        #endregion

        private PointOfInterestCutsceneController pointOfInterestCutsceneController;

        #region Data Retrieval

        public BaseCutsceneController GetCutsceneController()
        {
            return pointOfInterestCutsceneController;
        }

        #endregion

        #region Logical Condition

        public bool IsDirectedByCutscene()
        {
            return pointOfInterestCutsceneController.IsAnimationPlaying || CutscenePlayerManagerV2.IsCutscenePlaying;
        }

        #endregion

        public void Init(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModelObjectModule PointOfInterestModelObjectModule)
        {
            CutscenePlayerManagerV2 = AdventureGameSingletonInstances.CutscenePlayerManagerV2;
            pointOfInterestCutsceneController = new PointOfInterestCutsceneController(pointOfInterestTypeRef, PointOfInterestModelObjectModule);
        }

        public void Tick(float d)
        {
            pointOfInterestCutsceneController.Tick(d);
        }
    }

    internal class PointOfInterestCutsceneController : BaseCutsceneController
    {
        public PointOfInterestCutsceneController(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModelObjectModule PointOfInterestModelObjectModule)
        {
            #region Data Components Dependencies

            var PlayerInputMoveManagerComponentV3 = pointOfInterestTypeRef.PointOfInterestDefinitionInherentData.PointOfInterestSharedDataTypeInherentData.TransformMoveManagerComponent;

            #endregion

            var Rigidbody = pointOfInterestTypeRef.GetComponentInParent<Rigidbody>();
            var Agent = pointOfInterestTypeRef.GetComponentInParent<NavMeshAgent>();
            var Animator = PointOfInterestModelObjectModule.Animator;

            AnimationDataManager animationDataManager = null;

            if (!pointOfInterestTypeRef.IsPlayer()) animationDataManager = new AnimationDataManager(Animator);

            BaseInit(Rigidbody, Agent, Animator, PlayerInputMoveManagerComponentV3, animationDataManager);
        }
    }
}