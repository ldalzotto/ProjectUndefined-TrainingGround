using CoreGame;
using UnityEngine;
using UnityEngine.AI;

namespace AdventureGame
{
    public class PointOfInterestCutsceneControllerModule : APointOfInterestModule
    {
        private PointOfInterestCutsceneController pointOfInterestCutsceneController;

        #region Data Retrieval
        public AbstractCutsceneController GetCutsceneController()
        {
            return this.pointOfInterestCutsceneController;
        }
        #endregion

        public void Init(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModelObjectModule PointOfInterestModelObjectModule)
        {
            this.pointOfInterestCutsceneController = new PointOfInterestCutsceneController(pointOfInterestTypeRef, GameObject.FindObjectOfType<CutscenePlayerManagerV2>(), PointOfInterestModelObjectModule);
        }

        public void Tick(float d)
        {
            this.pointOfInterestCutsceneController.Tick(d);
        }
    }

    class PointOfInterestCutsceneController : AbstractCutsceneController
    {
        public PointOfInterestCutsceneController(PointOfInterestType pointOfInterestTypeRef, CutscenePlayerManagerV2 cutscenePlayerManagerV2, PointOfInterestModelObjectModule PointOfInterestModelObjectModule)
        {
            #region Data Components Dependencies
            var PlayerInputMoveManagerComponentV2 = pointOfInterestTypeRef.POIDataComponentContainer.GetDataComponent<TransformMoveManagerComponentV2>();
            #endregion

            var Rigidbody = pointOfInterestTypeRef.GetComponentInParent<Rigidbody>();
            var Agent = pointOfInterestTypeRef.GetComponentInParent<NavMeshAgent>();
            var Animator = PointOfInterestModelObjectModule.Animator;

            PlayerAnimationDataManager PlayerAnimationDataManager = null;

            if (!pointOfInterestTypeRef.IsPlayer())
            {
                PlayerAnimationDataManager = new PlayerAnimationDataManager(Animator);
            }

            base.BaseInit(PlayerInputMoveManagerComponentV2, cutscenePlayerManagerV2, Rigidbody, Agent, Animator, PlayerAnimationDataManager);
        }
    }
}
