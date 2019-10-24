using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Profiling;

namespace AnimatorPlayable
{
    public class MyAnimatorBehavior : MonoBehaviour
    {
        [Range(0f, 1f)] public float WeightValue;
        private AnimatorPlayableObject _animatorPlayableObject;

        [SerializeField] private BlendedAnimationInput BlendedAnimationInput;
        [SerializeField] private SequencedAnimationInput SequencedAnimationInput;
        public bool PlaySequence;

        private void Start()
        {
            this._animatorPlayableObject = new AnimatorPlayableObject("TEst", this.GetComponent<Animator>());
            this._animatorPlayableObject.PlayBlendedAnimation(this.BlendedAnimationInput);
        }

        private void Update()
        {
            if (this.PlaySequence)
            {
                this._animatorPlayableObject.PlaySequencedAnimation(this.SequencedAnimationInput);
                this.PlaySequence = false;
            }

            Profiler.BeginSample("MyAnimatorBehavior");
            this._animatorPlayableObject.Tick(Time.deltaTime, this.WeightValue);
            Profiler.EndSample();
        }
    }
}