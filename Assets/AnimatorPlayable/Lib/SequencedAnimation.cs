﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimatorPlayable
{
    public class SequencedAnimationLayer : MyAnimationLayer
    {
        public int LayerID;
        public List<UniqueAnimationClip> UniqueAnimationClips;
        public AnimationClipPlayable[] AssociatedAnimationClipsPlayable { get; private set; }

        public AnimationMixerPlayable AnimationMixerPlayable { get; private set; }

        private bool isInfinite;
        private float BeginTransitionTime;
        private float EndTransitionTime;

        private bool IsTransitioningIn;
        private bool IsTransitioningOut;
        private float TransitioningOutStartTime;
        private bool HasEnded;

        public SequencedAnimationLayer(PlayableGraph playableGraph, AnimationLayerMixerPlayable parentAnimationLayerMixerPlayable,
            int layerId, List<UniqueAnimationClip> uniqueAnimationClips, bool isInfinite, float BeginTransitionTime, float EndTransitionTime) : base(parentAnimationLayerMixerPlayable)
        {
            this.isInfinite = isInfinite;
            this.UniqueAnimationClips = uniqueAnimationClips;
            this.LayerID = layerId;
            this.BeginTransitionTime = BeginTransitionTime;
            this.EndTransitionTime = EndTransitionTime;
            this.IsTransitioningIn = false;
            this.IsTransitioningOut = false;


            this.AnimationMixerPlayable = AnimationMixerPlayable.Create(playableGraph, 0, normalizeWeights: true);
            this.AssociatedAnimationClipsPlayable = new AnimationClipPlayable[uniqueAnimationClips.Count];

            for (var i = 0; i < uniqueAnimationClips.Count; i++)
            {
                this.AssociatedAnimationClipsPlayable[i] = AnimationClipPlayable.Create(playableGraph, uniqueAnimationClips[i].AnimationClip);
                PlayableExtensions.SetDuration(this.AssociatedAnimationClipsPlayable[i], uniqueAnimationClips[i].AnimationClip.length);
                this.AssociatedAnimationClipsPlayable[i].SetApplyFootIK(false);
                this.AssociatedAnimationClipsPlayable[i].SetApplyPlayableIK(false);
                this.AssociatedAnimationClipsPlayable[i].Pause();
                uniqueAnimationClips[i].InputHandler = PlayableExtensions.AddInput(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[i], 0);
            }


            if (this.BeginTransitionTime > 0f)
            {
                this.IsTransitioningIn = true;
            }

            //Calculate linear blendings times
            float virtualClipElapsedTime = 0f;


            for (var i = 0; i < uniqueAnimationClips.Count; i++)
            {
                if (i == 0)
                {
                    this.UniqueAnimationClips[i].TransitionBlending = this.UniqueAnimationClips[i].TransitionBlending.SetWeightTimePoints(
                        AnimationWeightStartIncreasingTime: virtualClipElapsedTime,
                        AnimationWeightEndIncreasingTime: virtualClipElapsedTime,
                        AnimationWeightStartDecreasingTime: virtualClipElapsedTime + this.UniqueAnimationClips[i].AnimationClip.length - this.UniqueAnimationClips[i].TransitionBlending.EndTransitionTime,
                        AnimationWeightEndDecreasingTime: virtualClipElapsedTime + this.UniqueAnimationClips[i].AnimationClip.length
                    );

                    virtualClipElapsedTime += this.UniqueAnimationClips[i].AnimationClip.length;
                }
                else if (i == this.UniqueAnimationClips.Count - 1)
                {
                    this.UniqueAnimationClips[i].TransitionBlending = this.UniqueAnimationClips[i].TransitionBlending.SetWeightTimePoints(
                        AnimationWeightStartIncreasingTime: virtualClipElapsedTime - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime,
                        AnimationWeightEndIncreasingTime: virtualClipElapsedTime,
                        AnimationWeightStartDecreasingTime: (virtualClipElapsedTime - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime) + this.UniqueAnimationClips[i].AnimationClip.length,
                        AnimationWeightEndDecreasingTime: (virtualClipElapsedTime - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime) + this.UniqueAnimationClips[i].AnimationClip.length
                    );

                    virtualClipElapsedTime += this.UniqueAnimationClips[i].AnimationClip.length - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime;
                }
                else
                {
                    this.UniqueAnimationClips[i].TransitionBlending = this.UniqueAnimationClips[i].TransitionBlending.SetWeightTimePoints(
                        AnimationWeightStartIncreasingTime: virtualClipElapsedTime - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime,
                        AnimationWeightEndIncreasingTime: virtualClipElapsedTime,
                        AnimationWeightStartDecreasingTime: (virtualClipElapsedTime - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime) + this.UniqueAnimationClips[i].AnimationClip.length - this.UniqueAnimationClips[i].TransitionBlending.EndTransitionTime,
                        AnimationWeightEndDecreasingTime: (virtualClipElapsedTime - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime) + this.UniqueAnimationClips[i].AnimationClip.length
                    );

                    virtualClipElapsedTime += this.UniqueAnimationClips[i].AnimationClip.length - this.UniqueAnimationClips[i - 1].TransitionBlending.EndTransitionTime;
                }
            }

            PlayableExtensions.SetTime(this.AnimationMixerPlayable, 0);
        }

        public override void Tick(float d, float weightEvaluation)
        {
            if (!this.HasEnded)
            {
                var elapsedTime = PlayableExtensions.GetTime(this.AnimationMixerPlayable);
                if (this.IsTransitioningIn)
                {
                    float weightSetted = Mathf.Clamp01((float) elapsedTime / this.BeginTransitionTime);
                    SetAnimationMixerPlayableWeight(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[0], this.UniqueAnimationClips[0].InputHandler, 1f);
                    this.AssociatedAnimationClipsPlayable[0].Pause();
                    PlayableExtensions.SetInputWeight(this.ParentAnimationLayerMixerPlayable, this.Inputhandler, weightSetted);
                    if (weightSetted == 1f)
                    {
                        this.IsTransitioningIn = false;
                        this.AssociatedAnimationClipsPlayable[0].Play();
                    }
                }
                else if (this.IsTransitioningOut)
                {
                    float weightSetted = Mathf.Clamp01(((this.EndTransitionTime - ((float) elapsedTime - this.TransitioningOutStartTime)) / this.EndTransitionTime));

                    this.AssociatedAnimationClipsPlayable[this.AssociatedAnimationClipsPlayable.Length - 1].SetTime(this.AssociatedAnimationClipsPlayable[this.AssociatedAnimationClipsPlayable.Length - 1].GetDuration());
                    PlayableExtensions.SetInputWeight(this.ParentAnimationLayerMixerPlayable, this.Inputhandler, weightSetted);
                    if (weightSetted == 0f)
                    {
                        this.HasEnded = true;
                        this.IsTransitioningOut = false;
                    }
                }
                else
                {
                    bool atLeastOneClipIsPlaying = false;
                    for (var i = 0; i < this.UniqueAnimationClips.Count; i++)
                    {
                        if (i == this.UniqueAnimationClips.Count - 1 && elapsedTime >= this.UniqueAnimationClips[i].TransitionBlending.AnimationWeightEndDecreasingTime)
                        {
                            SetAnimationMixerPlayableWeight(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[i], this.UniqueAnimationClips[i].InputHandler, 1f);
                            atLeastOneClipIsPlaying = false;
                        }
                        else
                        {
                            float weightSetted = this.UniqueAnimationClips[i].TransitionBlending.GetInterpolatedWeight((float) elapsedTime);
                            SetAnimationMixerPlayableWeight(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[i], this.UniqueAnimationClips[i].InputHandler, weightSetted);
                            if (weightSetted > 0)
                            {
                                atLeastOneClipIsPlaying = true;
                            }
                        }
                    }

                    if (!atLeastOneClipIsPlaying)
                    {
                        if (this.EndTransitionTime > 0f && !this.isInfinite)
                        {
                            //  Debug.Break();
                            this.IsTransitioningOut = true;
                            this.TransitioningOutStartTime = (float) elapsedTime;
                        }
                        else
                        {
                            this.HasEnded = true;
                        }

                        PlayableExtensions.SetInputWeight(AnimationMixerPlayable, this.UniqueAnimationClips[this.UniqueAnimationClips.Count - 1].InputHandler, 1f);
                    }
                }
            }
        }


        /*
        private static void DoesTransitionClipAnimationsPlaying(double sampledTime, LinearBlending clipLeftBlending, LinearBlending clipRightBlending, out bool clipLeftPlaying, out bool clipRightPlaying)
        {
            clipLeftPlaying = (sampledTime <= clipLeftBlending.AnimationWeightEndDecreasingTime);
            clipRightPlaying = (sampledTime >= clipLeftBlending.GetAnimationEndTimeWithoutDelay() + clipRightBlending.BeginClipDelay);
        }
        */

        private static void SetAnimationMixerPlayableWeight(AnimationMixerPlayable AnimationMixerPlayable, AnimationClipPlayable AnimationClipPlayable, int inputHandler, float weight)
        {
            if (PlayableExtensions.GetInputWeight(AnimationMixerPlayable, inputHandler) == 0f && weight > 0f)
            {
                AnimationClipPlayable.SetTime(0f);
                AnimationClipPlayable.Play();
            }
            else if (weight == 0f)
            {
                AnimationClipPlayable.Pause();
            }

            PlayableExtensions.SetInputWeight(AnimationMixerPlayable, inputHandler, weight);
        }

        public override bool AskedToBeDestoyed()
        {
            return this.HasEnded && !this.isInfinite;
        }

        public override void Destroy(AnimationLayerMixerPlayable AnimationLayerMixerPlayable)
        {
            base.Destroy(AnimationLayerMixerPlayable);
            this.AnimationMixerPlayable.Destroy();
        }
    }
}