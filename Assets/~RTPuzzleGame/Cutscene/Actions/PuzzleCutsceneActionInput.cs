using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleCutsceneActionInput : SequencedActionInput
    {
        private InteractiveObjectContainer interactiveObjectContainer;

        public InteractiveObjectContainer InteractiveObjectContainer { get => interactiveObjectContainer; }

        public PuzzleCutsceneActionInput(InteractiveObjectContainer interactiveObjectContainer, Dictionary<CutsceneParametersName, object> PuzzleCutsceneGraphParameters = null)
        {
            this.interactiveObjectContainer = interactiveObjectContainer;
            this.graphParameters = PuzzleCutsceneGraphParameters;
        }
        
        public static Dictionary<CutsceneParametersName, object> Build_GENERIC_AnimationWithFollowObject_Animation(Transform transformToFollow, GameObject followingGameObject, AnimationID animationPlayed)
        {
            return new Dictionary<CutsceneParametersName, object>()
            {
                { CutsceneParametersName.TRANSFORM_TO_FOLLOW, transformToFollow },
                { CutsceneParametersName.FOLLOWING_OBJECT, followingGameObject },
                { CutsceneParametersName.ANIMATION_ID_1, animationPlayed }
            };
        }

        public static Dictionary<CutsceneParametersName, object> Build_1_Town_StartTutorial_Speaker_DisarmAnimation(InteractiveObjectType animatedInteractiveObject)
        {
            return new Dictionary<CutsceneParametersName, object>()
            {
                { CutsceneParametersName.INTERACTIVE_OBJECT_0, animatedInteractiveObject },
            };
        }
    }

}
