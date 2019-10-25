using AnimatorPlayable;
using UnityEngine;

namespace DefaultNamespace
{
    public class SequencerLoop2 : MonoBehaviour
    {
        private AnimatorPlayableObject AnimatorPlayableObject;
        public SequencedAnimationInput SequencedAnimationInput;

        private void Start()
        {
            this.AnimatorPlayableObject = new AnimatorPlayableObject("eee", this.GetComponent<Animator>());
            this.AnimatorPlayableObject.PlaySequencedAnimation(0, this.SequencedAnimationInput);
            this.InfiniteLoop();
        }

        private void InfiniteLoop()
        {
            this.AnimatorPlayableObject.PlaySequencedAnimation(0, this.SequencedAnimationInput);
            this.AnimatorPlayableObject.AllAnimationLayersCurrentlyPlaying[0].ReigsterOnSequencedAnimationEnd(() => { this.InfiniteLoop(); });
        }

        private void Update()
        {
            this.AnimatorPlayableObject.Tick(Time.deltaTime);
        }
    }
}