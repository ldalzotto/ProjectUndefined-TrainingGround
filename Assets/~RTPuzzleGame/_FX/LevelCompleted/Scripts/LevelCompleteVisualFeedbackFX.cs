using InteractiveObjectTest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class LevelCompleteVisualFeedbackFX : TriggerableParticleSystemFX
    {
        public override void Init()
        {
            base.Init();
            this.transform.position = PlayerInteractiveObjectManager.Get().GetPlayerGameObject().GetTransform().WorldPosition;
        }
    }

}
