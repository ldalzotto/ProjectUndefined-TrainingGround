using UnityEngine;
using System.Collections;
using System;

namespace AnimationSystem
{
    public abstract class AbstractAnimationActorModule : MonoBehaviour
    {
        public abstract void Init(AnimationActor AnimationActorRef);

        public virtual void SeenByCameraTick(float d) {  }

        public virtual void OnVisible() { }

        public virtual void OnNotVisible() { }
    }
}
