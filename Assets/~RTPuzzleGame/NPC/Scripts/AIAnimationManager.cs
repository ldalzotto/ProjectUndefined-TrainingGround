using CoreGame;
using UnityEngine;

namespace RTPuzzle
{
    public class AIAnimationManager
    {

        private PlayerAnimationDataManager NPCPAnimationDataManager;
        private InteractiveObjectType InteractiveObjectType;
        private Animator animator;
        private InteractiveObjectContainer InteractiveObjectContainer;

        private SequencedActionPlayer CurrentAnimationPlayed;

        public AIAnimationManager(Animator animator, InteractiveObjectType InteractiveObjectType, AnimationConfiguration AnimationConfiguration,
            InteractiveObjectContainer InteractiveObjectContainer)
        {
            this.animator = animator;
            this.InteractiveObjectType = InteractiveObjectType;
            this.InteractiveObjectContainer = InteractiveObjectContainer;

            this.NPCPAnimationDataManager = new PlayerAnimationDataManager(animator);

            //Initialize movement animation
            GenericAnimatorHelper.SetMovementLayer(animator, AnimationConfiguration, LevelType.PUZZLE);
        }

        public void TickAlways(float d, float normalizedCurrentSpeed)
        {
            this.NPCPAnimationDataManager.Tick(normalizedCurrentSpeed);
            if (this.CurrentAnimationPlayed != null) { this.CurrentAnimationPlayed.Tick(d); }
        }

        #region External Events
        public void OnDisarmObjectStart(IDisarmObjectModuleDataRetrieval disarmObjectModule)
        {
            this.CurrentAnimationPlayed = new SequencedActionPlayer(disarmObjectModule.GetDisarmAnimation().GetRootActions(),
                    new PuzzleCutsceneActionInput(this.InteractiveObjectContainer, PuzzleCutsceneActionInput.Build_1_Town_StartTutorial_Speaker_DisarmAnimation(this.InteractiveObjectType)));
            this.CurrentAnimationPlayed.Play();
        }
        public void OnDisarmObjectEnd()
        {
            this.CurrentAnimationPlayed.Kill();
        }
        #endregion
    }
}
