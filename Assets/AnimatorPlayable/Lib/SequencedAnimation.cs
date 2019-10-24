using System.Collections.Generic;
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
                    float dynamicallyCalculatedElapsedTime = 0f;
                    for (var i = 0; i < this.UniqueAnimationClips.Count; i++)
                    {
                        //Blend has already been calculated -> we set 0f weight
                        if (atLeastOneClipIsPlaying)
                        {
                            SetAnimationMixerPlayableWeight(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[i], this.UniqueAnimationClips[i].InputHandler, 0f);
                            continue;
                        }

                        this.UniqueAnimationClips[i].TransitionBlending = this.UniqueAnimationClips[i].TransitionBlending.CalculateAnimationWeightTime(this.UniqueAnimationClips[i].AnimationClip);

                        var clipNormalizedElapsedTime = elapsedTime - this.BeginTransitionTime - dynamicallyCalculatedElapsedTime;

                        //Begin calculating weight

                        //No blending
                        if (clipNormalizedElapsedTime >= 0 && clipNormalizedElapsedTime < this.UniqueAnimationClips[i].TransitionBlending.AnimationWeightStartDecreasingTime)
                        {
                            SetAnimationMixerPlayableWeight(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[i], this.UniqueAnimationClips[i].InputHandler, 1f);
                            atLeastOneClipIsPlaying = true;
                        }
                        //Blending
                        else if (clipNormalizedElapsedTime >= 0 && clipNormalizedElapsedTime < this.UniqueAnimationClips[i].TransitionBlending.AnimationWeightEndDecreasingTime)
                        {
                            if (i == this.UniqueAnimationClips.Count - 1)
                            {
                                SetAnimationMixerPlayableWeight(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[i], this.UniqueAnimationClips[i].InputHandler, 1f);
                                atLeastOneClipIsPlaying = true;
                            }
                            else
                            {
                                LinearBlending(clipNormalizedElapsedTime, this.UniqueAnimationClips[i].TransitionBlending, out float w1, out float w2);
                                SetAnimationMixerPlayableWeight(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[i], this.UniqueAnimationClips[i].InputHandler, w1);
                                SetAnimationMixerPlayableWeight(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[i + 1], this.UniqueAnimationClips[i + 1].InputHandler, w2);

                                /*
                                DoesTransitionClipAnimationsPlaying(clipNormalizedElapsedTime, this.UniqueAnimationClips[i].TransitionBlending, this.UniqueAnimationClips[i + 1].TransitionBlending, out bool clipLeftPlaying, out bool clipRightPlaying);
                                if (clipLeftPlaying)
                                {
                                    this.AssociatedAnimationClipsPlayable[i].Play();
                                }
                                else
                                {
                                    this.AssociatedAnimationClipsPlayable[i].Pause();
                                }

                                if (clipRightPlaying)
                                {
                                    this.AssociatedAnimationClipsPlayable[i + 1].Play();
                                }
                                else
                                {
                                    this.AssociatedAnimationClipsPlayable[i + 1].Pause();
                                }
*/
                                atLeastOneClipIsPlaying = true;

                                //We skip the next clip because we already updated it
                                dynamicallyCalculatedElapsedTime += this.UniqueAnimationClips[i].TransitionBlending.GetClipLength(this.UniqueAnimationClips[i].AnimationClip);
                                i += 1;
                            }
                        }
                        else
                        {
                            SetAnimationMixerPlayableWeight(this.AnimationMixerPlayable, this.AssociatedAnimationClipsPlayable[i], this.UniqueAnimationClips[i].InputHandler, 0f);
                        }
                        //End calculating weight

                        dynamicallyCalculatedElapsedTime += this.UniqueAnimationClips[i].TransitionBlending.GetClipLength(this.UniqueAnimationClips[i].AnimationClip);
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

        private static void LinearBlending(double sampledTime, LinearBlending TransitionBlending, out float w1, out float w2)
        {
            if (sampledTime <= TransitionBlending.AnimationWeightStartDecreasingTime)
            {
                w1 = 1f;
                w2 = 0f;
            }
            else
            {
                float weightFactor = (TransitionBlending.AnimationWeightEndDecreasingTime - (float) sampledTime) / (TransitionBlending.AnimationWeightEndDecreasingTime - TransitionBlending.AnimationWeightStartDecreasingTime);
                w1 = weightFactor;
                w2 = 1 - w1;
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