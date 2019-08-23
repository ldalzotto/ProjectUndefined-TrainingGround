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
        public AbstractCutsceneController GetCutsceneController()
        {
            return this.pointOfInterestCutsceneController;
        }
        #endregion

        #region Logical Condition
        public bool IsDirectedByCutscene()
        {
            return this.pointOfInterestCutsceneController.IsAnimationPlaying || this.CutscenePlayerManagerV2.IsCutscenePlaying;
        }
        #endregion

        public void Init(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModelObjectModule PointOfInterestModelObjectModule)
        {
            this.CutscenePlayerManagerV2 = GameObject.FindObjectOfType<CutscenePlayerManagerV2>();
            this.pointOfInterestCutsceneController = new PointOfInterestCutsceneController(pointOfInterestTypeRef, PointOfInterestModelObjectModule);
        }

        public void Tick(float d)
        {
            this.pointOfInterestCutsceneController.Tick(d);
        }
    }

    class PointOfInterestCutsceneController : AbstractCutsceneController
    {
        public PointOfInterestCutsceneController(PointOfInterestType pointOfInterestTypeRef, PointOfInterestModelObjectModule PointOfInterestModelObjectModule)
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

            base.BaseInit(Rigidbody, Agent, Animator, PlayerInputMoveManagerComponentV2.ToV3(), PlayerAnimationDataManager);
        }
    }
}
