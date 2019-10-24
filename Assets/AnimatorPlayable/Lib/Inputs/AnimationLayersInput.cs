using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AnimatorPlayable
{
    [Serializable]
    public class UniqueAnimationClip
    {
        public AnimationClip AnimationClip;
        public LinearBlending TransitionBlending;

        #region Dynamically Setted

        public int InputHandler;

        #endregion
    }


    [Serializable]
    public struct LinearBlending
    {
        public float EndTransitionTime;

        //   [FormerlySerializedAs("ArtificialBeginClipDelay")] public float BeginClipDelay;
        //  [FormerlySerializedAs("ArtificialEndClipDelay")] public float EndClipDelay;

        [MyReadOnly] public float AnimationWeightStartDecreasingTime;
        [MyReadOnly] public float AnimationWeightEndDecreasingTime;

        /*
         public float GetAnimationEndTimeWithoutDelay()
         {
             return this.AnimationWeightEndDecreasingTime - this.EndClipDelay;
         }
         */

        public float GetClipLength(AnimationClip involvedAnimationClip)
        {
            return involvedAnimationClip.length; // + BeginClipDelay + EndClipDelay;
        }

        public LinearBlending CalculateAnimationWeightTime(AnimationClip involvedAnimationClip)
        {
            this.AnimationWeightStartDecreasingTime = this.GetClipLength(involvedAnimationClip) - this.EndTransitionTime;
            this.AnimationWeightEndDecreasingTime = this.AnimationWeightStartDecreasingTime + this.EndTransitionTime;
            return this;
        }
    }


    [Serializable]
    public struct SequencedAnimationInput
    {
        public int layerID;
        public bool isInfinite;
        public float BeginTransitionTime;
        public float EndTransitionTime;
        public List<UniqueAnimationClip> UniqueAnimationClips;
    }


    public class BlendedAnimationClip
    {
        public BlendedAnimationClip(AnimationClip animationClip, AnimationCurve normalizedWeightDistribution)
        {
            AnimationClip = animationClip;
            NormalizedWeightDistribution = normalizedWeightDistribution;
        }

        public AnimationClip AnimationClip { get; private set; }
        public AnimationCurve NormalizedWeightDistribution { get; private set; }

        public int InputHandler;
    }

    [Serializable]
    public struct BlendedAnimationInput
    {
        public int layerID;
        public List<AnimationClip> BlendedClips;
        public List<AnimationCurve> NormalizedWeightDistributions;
        public bool IsInfinite;

        public BlendedAnimationInput(int layerId, List<AnimationClip> blendedClips, List<AnimationCurve> normalizedWeightDistributions, bool isInfinite)
        {
            layerID = layerId;
            BlendedClips = blendedClips;
            NormalizedWeightDistributions = normalizedWeightDistributions;
            IsInfinite = isInfinite;
        }
    }
}