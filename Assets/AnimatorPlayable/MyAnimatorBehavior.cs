using UnityEngine;
using UnityEngine.Profiling;

namespace AnimatorPlayable
{
    public class MyAnimatorBehavior : MonoBehaviour
    {
        [Range(0f, 1f)] public float WeightValue;
        private AnimatorPlayableObject _animatorPlayableObject;
        public bool PlayBlended;

        [SerializeField] private BlendedAnimationInput BlendedAnimationInput;
        [SerializeField] private SequencedAnimationInput SequencedAnimationInput;
        public bool PlaySequence;

        private void Start()
        {
            this._animatorPlayableObject = new AnimatorPlayableObject("TEst", this.GetComponent<Animator>());
        }

        private void Update()
        {
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

            Profiler.BeginSample("MyAnimatorBehavior");
            this._animatorPlayableObject.Tick(Time.deltaTime);
            Profiler.EndSample();
        }
    }
}