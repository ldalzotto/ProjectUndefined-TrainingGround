﻿using System;
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
        private List<MyAnimationLayer> OrderedByInputHandlerAnimationLayers = new List<MyAnimationLayer>();

        public AnimatorPlayableObject(string graphName, Animator animator)
        {
            this.GlobalPlayableGraph = PlayableGraph.Create(graphName);
            this.GlobalPlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            var playableOutput = AnimationPlayableOutput.Create(this.GlobalPlayableGraph, "Animation", animator);
            this.AnimationLayerMixerPlayable = AnimationLayerMixerPlayable.Create(this.GlobalPlayableGraph);
            PlayableOutputExtensions.SetSourcePlayable(playableOutput, this.AnimationLayerMixerPlayable);
            this.GlobalPlayableGraph.Play();
        }

        public void PlayBlendedAnimation(BlendedAnimationInput BlendedAnimationInput, Func<float> InputWeightProvider)
        {
            if (this.AllAnimationLayersCurrentlyPlaying.ContainsKey(BlendedAnimationInput.layerID))
            {
                this.DestroyLayer(BlendedAnimationInput.layerID);
            }

            BlendedAnimationLayer BlendedAnimationLayer = new BlendedAnimationLayer(this.GlobalPlayableGraph, this.AnimationLayerMixerPlayable, BlendedAnimationInput.layerID,
                BlendedAnimationInput.BlendedAnimationClips, BlendedAnimationInput.BlendedAnimationSpeedCurve, InputWeightProvider);
            BlendedAnimationLayer.Inputhandler = PlayableExtensions.AddInput(this.AnimationLayerMixerPlayable, BlendedAnimationLayer.AnimationMixerPlayable, 0);

            this.AllAnimationLayersCurrentlyPlaying[BlendedAnimationInput.layerID] = BlendedAnimationLayer;
            this.OrderedByInputHandlerAnimationLayers.Add(BlendedAnimationLayer);

            this.SortLayers();

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
            this.OrderedByInputHandlerAnimationLayers.Add(SequencedAnimationLayer);

            this.SortLayers();

            PlayableExtensions.SetInputWeight(this.AnimationLayerMixerPlayable, SequencedAnimationLayer.Inputhandler, 1f);
        }

        public void Tick(float d)
        {
            List<int> animationLayersToDestroy = null;
            foreach (var blendedAnimationLayer in AllAnimationLayersCurrentlyPlaying)
            {
                blendedAnimationLayer.Value.Tick(d);
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

        public void SetSpeed(float speed)
        {
            this.AnimationLayerMixerPlayable.SetSpeed(speed);
        }

        private void DestroyLayer(int layerID)
        {
            this.OrderedByInputHandlerAnimationLayers.Remove(this.AllAnimationLayersCurrentlyPlaying[layerID]);
            this.AllAnimationLayersCurrentlyPlaying[layerID].Destroy(this.AnimationLayerMixerPlayable);
            this.AllAnimationLayersCurrentlyPlaying.Remove(layerID);
        }

        private void SortLayers()
        {
            if (this.AllAnimationLayersCurrentlyPlaying.Count > 1)
            {
                bool layerRefreshNeeded = false;
                bool isSorted = false;
                while (!isSorted)
                {
                    isSorted = true;
                    for (var i = 0; i < this.OrderedByInputHandlerAnimationLayers.Count; i++)
                    {
                        if (i != this.OrderedByInputHandlerAnimationLayers.Count - 1)
                        {
                            if (this.OrderedByInputHandlerAnimationLayers[i].LayerID > this.OrderedByInputHandlerAnimationLayers[i + 1].LayerID)
                            {
                                var tmp = this.OrderedByInputHandlerAnimationLayers[i + 1];
                                this.OrderedByInputHandlerAnimationLayers[i + 1] = this.OrderedByInputHandlerAnimationLayers[i];
                                this.OrderedByInputHandlerAnimationLayers[i] = tmp;

                                var tmpInputHandler = this.OrderedByInputHandlerAnimationLayers[i + 1].Inputhandler;
                                this.OrderedByInputHandlerAnimationLayers[i + 1].Inputhandler = this.OrderedByInputHandlerAnimationLayers[i].Inputhandler;
                                this.OrderedByInputHandlerAnimationLayers[i].Inputhandler = tmpInputHandler;
                                isSorted = false;
                                layerRefreshNeeded = true;
                                break;
                            }
                        }
                    }
                }

                if (layerRefreshNeeded)
                {
                    foreach (var animationLayer in AllAnimationLayersCurrentlyPlaying)
                    {
                        this.AnimationLayerMixerPlayable.DisconnectInput(animationLayer.Value.Inputhandler);
                    }

                    foreach (var orderedByInputHandlerAnimationLayer in this.OrderedByInputHandlerAnimationLayers)
                    {
                        this.AnimationLayerMixerPlayable.ConnectInput(orderedByInputHandlerAnimationLayer.Inputhandler, orderedByInputHandlerAnimationLayer.GetEntryPointMixerPlayable(), 0);
                    }
                }
            }
        }

        private void SwitchLayers(int leftLayerID, int rightLayerID)
        {
            this.AnimationLayerMixerPlayable.DisconnectInput(this.AllAnimationLayersCurrentlyPlaying[leftLayerID].Inputhandler);
            this.AnimationLayerMixerPlayable.DisconnectInput(this.AllAnimationLayersCurrentlyPlaying[rightLayerID].Inputhandler);

            this.AnimationLayerMixerPlayable.ConnectInput(this.AllAnimationLayersCurrentlyPlaying[rightLayerID].Inputhandler, AllAnimationLayersCurrentlyPlaying[leftLayerID].GetEntryPointMixerPlayable(), 0);
            this.AnimationLayerMixerPlayable.ConnectInput(this.AllAnimationLayersCurrentlyPlaying[leftLayerID].Inputhandler, AllAnimationLayersCurrentlyPlaying[rightLayerID].GetEntryPointMixerPlayable(), 0);

            var leftLayerInputHandler = AllAnimationLayersCurrentlyPlaying[leftLayerID].Inputhandler;
            AllAnimationLayersCurrentlyPlaying[leftLayerID].Inputhandler = this.AllAnimationLayersCurrentlyPlaying[rightLayerID].Inputhandler;
            this.AllAnimationLayersCurrentlyPlaying[rightLayerID].Inputhandler = leftLayerInputHandler;
        }

        public void Destroy()
        {
            this.GlobalPlayableGraph.Destroy();
        }
    }

    public abstract class MyAnimationLayer
    {
        public int Inputhandler;
        public int LayerID;
        public abstract void Tick(float d);
        public abstract bool AskedToBeDestoyed();

        protected AnimationLayerMixerPlayable ParentAnimationLayerMixerPlayable;

        public abstract AnimationMixerPlayable GetEntryPointMixerPlayable();

        protected MyAnimationLayer(int LayerID, AnimationLayerMixerPlayable parentAnimationLayerMixerPlayable)
        {
            this.LayerID = LayerID;
            ParentAnimationLayerMixerPlayable = parentAnimationLayerMixerPlayable;
        }

        public virtual void Destroy(AnimationLayerMixerPlayable AnimationLayerMixerPlayable)
        {
            PlayableExtensions.DisconnectInput(AnimationLayerMixerPlayable, this.Inputhandler);
            AnimationLayerMixerPlayable.SetInputCount(AnimationLayerMixerPlayable.GetInputCount() - 1);
        }
    }
}