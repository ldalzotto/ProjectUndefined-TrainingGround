using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Profiling;

public class MyAnimatorBehavior : MonoBehaviour
{
    [Range(0f, 1f)] public float WeightValue;
    private MyAnimator MyAnimator;

    [SerializeField] private BlendedAnimationInput BlendedAnimationInput;
    [SerializeField] private SequencedAnimationInput SequencedAnimationInput;
    public bool PlaySequence;

    private void Start()
    {
        this.MyAnimator = new MyAnimator("TEst", this.GetComponent<Animator>());
        this.MyAnimator.PlayBlendedAnimation(this.BlendedAnimationInput);
    }

    private void Update()
    {
        if (this.PlaySequence)
        {
            this.MyAnimator.PlaySequencedAnimation(this.SequencedAnimationInput);
            this.PlaySequence = false;
        }

        Profiler.BeginSample("MyAnimatorBehavior");
        this.MyAnimator.Tick(Time.deltaTime, this.WeightValue);
        Profiler.EndSample();
    }
}

class MyAnimator
{
    private PlayableGraph GlobalPlayableGraph;
    private AnimationLayerMixerPlayable AnimationLayerMixerPlayable;

    private Dictionary<int, MyAnimationLayer> AllAnimationLayersCurrentlyPlaying = new Dictionary<int, MyAnimationLayer>();

    public MyAnimator(string graphName, Animator animator)
    {
        this.GlobalPlayableGraph = PlayableGraph.Create(graphName);
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
}

abstract class MyAnimationLayer
{
    public int Inputhandler;
    public abstract void Tick(float d, float weightEvaluation);
    public abstract bool AskedToBeDestoyed();

    protected AnimationLayerMixerPlayable ParentAnimationLayerMixerPlayable;

    protected MyAnimationLayer(AnimationLayerMixerPlayable parentAnimationLayerMixerPlayable)
    {
        ParentAnimationLayerMixerPlayable = parentAnimationLayerMixerPlayable;
    }

    public void Destroy(AnimationLayerMixerPlayable AnimationLayerMixerPlayable)
    {
        PlayableExtensions.DisconnectInput(AnimationLayerMixerPlayable, this.Inputhandler);
    }
}

class SequencedAnimationLayer : MyAnimationLayer
{
    public int LayerID;
    public List<UniqueAnimationClip> UniqueAnimationClips;

    public AnimationMixerPlayable AnimationMixerPlayable { get; private set; }

    private bool isInfinite;
    private float BeginTransitionTime;
    private float EndTransitionTime;

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

        this.AnimationMixerPlayable = AnimationMixerPlayable.Create(playableGraph, uniqueAnimationClips.Count, normalizeWeights: true);

        for (var i = 0; i < uniqueAnimationClips.Count; i++)
        {
            var animationClipPlayable = AnimationClipPlayable.Create(playableGraph, uniqueAnimationClips[i].AnimationClip);
            PlayableExtensions.SetDuration(animationClipPlayable, uniqueAnimationClips[i].AnimationClip.length);
            animationClipPlayable.SetApplyFootIK(false);
            uniqueAnimationClips[i].InputHandler = PlayableExtensions.AddInput(this.AnimationMixerPlayable, animationClipPlayable, 0);
        }

        PlayableExtensions.SetTime(this.AnimationMixerPlayable, 0);
    }

    public override void Tick(float d, float weightEvaluation)
    {
        if (!this.HasEnded)
        {
            var elapsedTime = PlayableExtensions.GetTime(this.AnimationMixerPlayable);
            if (!this.IsTransitioningOut)
            {
                if (this.BeginTransitionTime == 0f)
                {
                    PlayableExtensions.SetInputWeight(this.ParentAnimationLayerMixerPlayable, this.Inputhandler, 1f);
                }
                else
                {
                    float weightSetted = Mathf.Clamp01((float) elapsedTime / this.BeginTransitionTime);
                    PlayableExtensions.SetInputWeight(this.ParentAnimationLayerMixerPlayable, this.Inputhandler, weightSetted);
                }


                bool atLeastOneClipIsPlaying = false;
                float dynamicallyCalculatedElapsedTime = 0f;
                for (var i = 0; i < this.UniqueAnimationClips.Count; i++)
                {
                    //Blend has already been calculated -> we set 0f weight
                    if (atLeastOneClipIsPlaying)
                    {
                        PlayableExtensions.SetInputWeight(this.AnimationMixerPlayable, this.UniqueAnimationClips[i].InputHandler, 0f);
                        continue;
                    }

                    this.UniqueAnimationClips[i].TransitionBlending = this.UniqueAnimationClips[i].TransitionBlending.CalculateAnimationWeightTime(this.UniqueAnimationClips[i].AnimationClip);

                    var clipNormalizedElapsedTime = elapsedTime - dynamicallyCalculatedElapsedTime;

                    //Begin calculating weight

                    //No blending
                    if (clipNormalizedElapsedTime >= 0 && clipNormalizedElapsedTime < this.UniqueAnimationClips[i].TransitionBlending.AnimationWeightStartDecreasingTime)
                    {
                        PlayableExtensions.SetInputWeight(this.AnimationMixerPlayable, this.UniqueAnimationClips[i].InputHandler, 1f);
                        atLeastOneClipIsPlaying = true;
                    }
                    //Blending
                    else if (clipNormalizedElapsedTime >= 0 && clipNormalizedElapsedTime < this.UniqueAnimationClips[i].TransitionBlending.AnimationWeightEndDecreasingTime)
                    {
                        if (i == this.UniqueAnimationClips.Count - 1)
                        {
                            PlayableExtensions.SetInputWeight(this.AnimationMixerPlayable, this.UniqueAnimationClips[i].InputHandler, 1f);
                            atLeastOneClipIsPlaying = true;
                        }
                        else
                        {
                            LinearBlending(clipNormalizedElapsedTime, this.UniqueAnimationClips[i].TransitionBlending, out float w1, out float w2);
                            PlayableExtensions.SetInputWeight(this.AnimationMixerPlayable, this.UniqueAnimationClips[i].InputHandler, w1);
                            PlayableExtensions.SetInputWeight(this.AnimationMixerPlayable, this.UniqueAnimationClips[i + 1].InputHandler, w2);

                            atLeastOneClipIsPlaying = true;

                            if (clipNormalizedElapsedTime <= this.UniqueAnimationClips[i].TransitionBlending.AnimationWeightStartDecreasingTime)
                            {
                                //We skip the next clip because we already updated it
                                i += 1;
                            }
                        }
                    }
                    else
                    {
                        PlayableExtensions.SetInputWeight(this.AnimationMixerPlayable, this.UniqueAnimationClips[i].InputHandler, 0f);
                    }
                    //End calculating weight

                    dynamicallyCalculatedElapsedTime += this.UniqueAnimationClips[i].TransitionBlending.AnimationWeightEndDecreasingTime;
                }

                if (!atLeastOneClipIsPlaying)
                {
                    if (this.EndTransitionTime > 0f && !this.isInfinite)
                    {
                        this.IsTransitioningOut = true;
                        this.TransitioningOutStartTime = (float) elapsedTime;
                    }
                    else
                    {
                        this.HasEnded = true;
                    }

                    PlayableExtensions.SetInputWeight(this.AnimationMixerPlayable, this.UniqueAnimationClips[this.UniqueAnimationClips.Count - 1].InputHandler, 1f);
                }
            }
            else
            {
                float weightSetted = Mathf.Clamp01(((this.EndTransitionTime - ((float) elapsedTime - this.TransitioningOutStartTime)) / this.EndTransitionTime));
                PlayableExtensions.SetInputWeight(this.ParentAnimationLayerMixerPlayable, this.Inputhandler, weightSetted);
                if (weightSetted == 0f)
                {
                    this.HasEnded = true;
                    this.IsTransitioningOut = false;
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

    public override bool AskedToBeDestoyed()
    {
        return this.HasEnded && !this.isInfinite;
    }
}

[Serializable]
struct SequencedAnimationInput
{
    public int layerID;
    public bool isInfinite;
    public float BeginTransitionTime;
    public float EndTransitionTime;
    public List<UniqueAnimationClip> UniqueAnimationClips;
}

[Serializable]
class UniqueAnimationClip
{
    public AnimationClip AnimationClip;
    public LinearBlending TransitionBlending;

    #region Dynamically Setted

    public int InputHandler;

    #endregion
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

class BlendedAnimationClip
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
struct BlendedAnimationInput
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

[Serializable]
struct LinearBlending
{
    public float EndTransitionTime;
    public float EndTransitionOffset;

    public float AnimationWeightStartDecreasingTime;
    public float AnimationWeightEndDecreasingTime;

    public LinearBlending CalculateAnimationWeightTime(AnimationClip involvedAnimationClip)
    {
        this.AnimationWeightStartDecreasingTime = involvedAnimationClip.length + this.EndTransitionOffset;
        this.AnimationWeightEndDecreasingTime = this.AnimationWeightStartDecreasingTime + this.EndTransitionTime;
        return this;
    }
}