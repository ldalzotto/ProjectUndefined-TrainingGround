using UnityEngine;
using UnityEngine.Profiling;

namespace AnimatorPlayable
{
    public class MyAnimatorBehavior : MonoBehaviour
    {
        public float Speed = 1f;

        [Range(0f, 1f)] public float WeightValue;
        private AnimatorPlayableObject _animatorPlayableObject;
        public bool PlayBlended;

        [SerializeField] private BlendedAnimationInput BlendedAnimationInput;
        [SerializeField] private SequencedAnimationInput SequencedAnimationInput;
        public bool PlaySequence;

        [SerializeField] private SequencedAnimationInput AnotherSequencedAnimationInput;
        public bool PlaySequence2;

        private void Start()
        {
            this._animatorPlayableObject = new AnimatorPlayableObject("TEst", this.GetComponent<Animator>());
        }

        private void Update()
        {
            this._animatorPlayableObject.SetSpeed(this.Speed);
            if (this.PlayBlended)
            {
                this._animatorPlayableObject.PlayBlendedAnimation(this.BlendedAnimationInput, () => this.WeightValue);
                this.PlayBlended = false;
            }

            if (this.PlaySequence)
            {
                this._animatorPlayableObject.PlaySequencedAnimation(this.SequencedAnimationInput);
                this.PlaySequence = false;
            }

            if (this.PlaySequence2)
            {
                this._animatorPlayableObject.PlaySequencedAnimation(this.AnotherSequencedAnimationInput);
                this.PlaySequence2 = false;
            }

            Profiler.BeginSample("MyAnimatorBehavior");
            this._animatorPlayableObject.Tick(Time.deltaTime);
            Profiler.EndSample();
        }
    }
}