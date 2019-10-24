using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimatorPlayable
{
    public class AnimatorPlayableObject
    {
        public PlayableGraph GlobalPlayableGraph { get; private set; }
        public AnimationLayerMixerPlayable AnimationLayerMixerPlayable { get; private set; }

        public Dictionary<int, MyAnimationLayer> AllAnimationLayersCurrentlyPlaying { get; private set; } = new Dictionary<int, MyAnimationLayer>();

        public AnimatorPlayableObject(string graphName, Animator animator)
        {
            this.GlobalPlayableGraph = PlayableGraph.Create(graphName);
            this.GlobalPlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            var playableOutput = AnimationPlayableOutput.Create(this.GlobalPlayableGraph, "Animation", animator);
            this.AnimationLayerMixerPlayable = AnimationLayerMixerPlayable.Create(this.GlobalPlayableGraph);
            PlayableOutputExtensions.SetSourcePlayable(playableOutput, this.AnimationLayerMixerPlayable);
            this.GlobalPlayableGraph.Play();
        }

        public void PlayBlendedAnimation(BlendedAnimationInput BlendedAnimationInput)
        {
            if (this.AllAnimationLayersCurrentlyPlaying.ContainsKey(BlendedAnimationInput.layerID))
            {
                this.DestroyLayer(BlendedAnimationInput.layerID);
            }

            List<BlendedAnimationClip> BlendedAnimationClips = new List<BlendedAnimationClip>();
            for (var i = 0; i < BlendedAnimationInput.BlendedClips.Count; i++)
            {
                BlendedAnimationClips.Add(new BlendedAnimationClip(BlendedAnimationInput.BlendedClips[i], BlendedAnimationInput.NormalizedWeightDistributions[i]));
            }

            BlendedAnimationLayer BlendedAnimationLayer = new BlendedAnimationLayer(this.GlobalPlayableGraph, this.AnimationLayerMixerPlayable, BlendedAnimationInput.layerID, BlendedAnimationClips);
            BlendedAnimationLayer.Inputhandler = PlayableExtensions.AddInput(this.AnimationLayerMixerPlayable, BlendedAnimationLayer.AnimationMixerPlayable, 0);

            this.AllAnimationLayersCurrentlyPlaying[BlendedAnimationInput.layerID] = BlendedAnimationLayer;

            PlayableExtensions.SetInputWeight(this.AnimationLayerMixerPlayable, this.AllAnimationLayersCurrentlyPlaying[BlendedAnimationInput.layerID].Inputhandler, 1f);
        }

        public void PlaySequencedAnimation(SequencedAnimationInput SequencedAnimationInput)
        {
            if (this.AllAnimationLayersCurrentlyPlaying.ContainsKey(SequencedAnimationInput.layerID))
            {
                this.DestroyLayer(SequencedAnimationInput.layerID);
            }

            var SequencedAnimationLayer = new SequencedAnimationLayer(this.GlobalPlayableGraph, this.AnimationLayerMixerPlayable, SequencedAnimationInput.layerID, SequencedAnimationInput.UniqueAnimationClips,
                SequencedAnimationInput.isInfinite, SequencedAnimationInput.BeginTransitionTime, SequencedAnimationInput.EndTransitionTime);
            SequencedAnimationLayer.Inputhandler = PlayableExtensions.AddInput(this.AnimationLayerMixerPlayable, SequencedAnimationLayer.AnimationMixerPlayable, 0);
            this.AllAnimationLayersCurrentlyPlaying[SequencedAnimationInput.layerID] = SequencedAnimationLayer;
            PlayableExtensions.SetInputWeight(this.AnimationLayerMixerPlayable, SequencedAnimationLayer.Inputhandler, 1f);
        }

        public void Tick(float d, float weightValue)
        {
            List<int> animationLayersToDestroy = null;
            foreach (var blendedAnimationLayer in AllAnimationLayersCurrentlyPlaying)
            {
                blendedAnimationLayer.Value.Tick(d, weightValue);
                if (blendedAnimationLayer.Value.AskedToBeDestoyed())
                {
                    if (animationLayersToDestroy == null)
                    {
                        animationLayersToDestroy = new List<int>();
                    }

                    animationLayersToDestroy.Add(blendedAnimationLayer.Key);
                }
            }

            if (animationLayersToDestroy != null)
            {
                foreach (var animationLayerToDestroy in animationLayersToDestroy)
                {
                    this.DestroyLayer(animationLayerToDestroy);
                }
            }
        }

        private void DestroyLayer(int layerID)
        {
            this.AllAnimationLayersCurrentlyPlaying[layerID].Destroy(this.AnimationLayerMixerPlayable);
            this.AllAnimationLayersCurrentlyPlaying.Remove(layerID);
        }

        public void Destroy()
        {
            this.GlobalPlayableGraph.Destroy();
        }
    }

    public abstract class MyAnimationLayer
    {
        public int Inputhandler;
        public abstract void Tick(float d, float weightEvaluation);
        public abstract bool AskedToBeDestoyed();

        protected AnimationLayerMixerPlayable ParentAnimationLayerMixerPlayable;

        protected MyAnimationLayer(AnimationLayerMixerPlayable parentAnimationLayerMixerPlayable)
        {
            ParentAnimationLayerMixerPlayable = parentAnimationLayerMixerPlayable;
        }

        public virtual void Destroy(AnimationLayerMixerPlayable AnimationLayerMixerPlayable)
        {
            PlayableExtensions.DisconnectInput(AnimationLayerMixerPlayable, this.Inputhandler);
            AnimationLayerMixerPlayable.SetInputCount(AnimationLayerMixerPlayable.GetInputCount() - 1);
        }
    }

    class BlendedAnimationLayer : MyAnimationLayer
    {
        public int LayerID;
        public List<BlendedAnimationClip> BlendedAnimationClips;

        public AnimationMixerPlayable AnimationMixerPlayable { get; private set; }

        public BlendedAnimationLayer(PlayableGraph PlayableGraph, AnimationLayerMixerPlayable parentAnimationLayerMixerPlayable,
            int layerId, List<BlendedAnimationClip> blendedAnimationClips) : base(parentAnimationLayerMixerPlayable)
        {
            LayerID = layerId;
            BlendedAnimationClips = blendedAnimationClips;

            //create a playable mixer
            this.AnimationMixerPlayable = AnimationMixerPlayable.Create(PlayableGraph, blendedAnimationClips.Count, normalizeWeights: true);

            foreach (var blendedAnimationClip in blendedAnimationClips)
            {
                var animationClipPlayable = AnimationClipPlayable.Create(PlayableGraph, blendedAnimationClip.AnimationClip);
                animationClipPlayable.SetApplyFootIK(false);
                animationClipPlayable.SetApplyPlayableIK(false);
                blendedAnimationClip.InputHandler = PlayableExtensions.AddInput(this.AnimationMixerPlayable, animationClipPlayable, 0);
                PlayableExtensions.Play(animationClipPlayable);
                animationClipPlayable.SetApplyFootIK(false);
            }
        }

        private float oldWeightEvaluation = -1f;

        public override void Tick(float d, float weightEvaluation)
        {
            if (this.oldWeightEvaluation != weightEvaluation)
            {
                foreach (var blendedAnimationClip in BlendedAnimationClips)
                {
                    PlayableExtensions.SetInputWeight(this.AnimationMixerPlayable, blendedAnimationClip.InputHandler, blendedAnimationClip.NormalizedWeightDistribution.Evaluate(weightEvaluation));
                }
            }

            this.oldWeightEvaluation = weightEvaluation;
        }

        public override bool AskedToBeDestoyed()
        {
            return false;
        }
    }
}