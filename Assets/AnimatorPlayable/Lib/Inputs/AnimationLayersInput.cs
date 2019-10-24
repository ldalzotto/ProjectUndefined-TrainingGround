using System;
using System.Collections.Generic;
using UnityEngine;

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
        public float EndClipDelay;

        [MyReadOnly] public float AnimationWeightStartIncreasingTime;
        [MyReadOnly] public float AnimationWeightEndIncreasingTime;

        [MyReadOnly] public float AnimationWeightStartDecreasingTime;
        [MyReadOnly] public float AnimationWeightEndDecreasingTime;


        public LinearBlending SetWeightTimePoints(float AnimationWeightStartIncreasingTime, float AnimationWeightEndIncreasingTime, float AnimationWeightStartDecreasingTime, float AnimationWeightEndDecreasingTime)
        {
            this.AnimationWeightStartIncreasingTime = AnimationWeightStartIncreasingTime;
            this.AnimationWeightEndIncreasingTime = AnimationWeightEndIncreasingTime;
            this.AnimationWeightStartDecreasingTime = AnimationWeightStartDecreasingTime;
            this.AnimationWeightEndDecreasingTime = AnimationWeightEndDecreasingTime;
            return this;
        }

        public float GetInterpolatedWeight(float sampledTime)
        {
            if (sampledTime >= this.AnimationWeightStartIncreasingTime && sampledTime <= this.AnimationWeightEndIncreasingTime)
            {
                if ((this.AnimationWeightEndIncreasingTime - this.AnimationWeightStartIncreasingTime) == 0f)
                {
                    return 1f;
                }

                return (sampledTime - this.AnimationWeightStartIncreasingTime) / (this.AnimationWeightEndIncreasingTime - this.AnimationWeightStartIncreasingTime);
            }
            else if (sampledTime >= this.AnimationWeightEndIncreasingTime && sampledTime <= this.AnimationWeightStartDecreasingTime)
            {
                return 1f;
            }
            else if (sampledTime >= this.AnimationWeightStartDecreasingTime && sampledTime <= this.AnimationWeightEndDecreasingTime)
            {
                if ((this.AnimationWeightEndDecreasingTime - this.AnimationWeightStartDecreasingTime) == 0f)
                {
                    return 0f;
                }

                return 1 - ((sampledTime - this.AnimationWeightStartDecreasingTime) / (this.AnimationWeightEndDecreasingTime - this.AnimationWeightStartDecreasingTime));
            }
            else
            {
                return 0f;
            }
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