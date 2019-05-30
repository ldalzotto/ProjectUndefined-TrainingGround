using UnityEngine;
using System.Collections;

namespace AdventureGame
{
    [System.Serializable]
    public class AnimatorActionInherentData : AContextActionInherentData
    {
        public PlayerAnimatioNamesEnum PlayerAnimationEnum;
        public override AContextAction BuildContextAction()
        {
            return new AnimatorAction(this.PlayerAnimationEnum, null);
        }
    }

}
