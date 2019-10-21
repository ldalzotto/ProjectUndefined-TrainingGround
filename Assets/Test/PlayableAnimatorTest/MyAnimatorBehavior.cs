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

        BlendedAnimationLayer BlendedAnimationLayer = new BlendedAnimationLayer(this.GlobalPlayableGraph, BlendedAnimationInput.layerID, BlendedAnimationClips);
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

        var SequencedAnimationLayer = new SequencedAnimationLayer(this.GlobalPlayableGraph, SequencedAnimationInput.layerID, SequencedAnimationInput.UniqueAnimationClips, SequencedAnimationInput.isInfinite, SequencedAnimationInput.BeginTransitionTime);
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

    private bool HasEnded;

    public SequencedAnimationLayer(PlayableGraph playableGraph, int layerId, List<UniqueAnimationClip> uniqueAnimationClips, bool isInfinite, float BeginTransitionTime)
    {
        this.isInfinite = isInfinite;
        this.UniqueAnimationClips = uniqueAnimationClips;
        this.LayerID = layerId;
        this.BeginTransitionTime = BeginTransitionTime;

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
            bool atLeastOneClipIsPlaying = false;
            float dynamicallyCalculatedElapsedTime = 0f;
            for (var i = 0; i < this.UniqueAnimationClips.Count; i++)
            {
                var clipNormalizedElapsedTime = elapsedTime - dynamicallyCalculatedElapsedTime;

                //Begin calculating weight
                float weightSetted = 0f;
                if (clipNormalizedElapsedTime >= 0 && clipNormalizedElapsedTime < this.UniqueAnimationClips[i].AnimationClip.length)
                {
                    if (i == 0)
                    {
                        if (this.BeginTransitionTime == 0f)
                        {
                            weightSetted = 1f;
                        }
                        else
                        {
                            //TODO -> Begin must be applied to layer instead
                            weightSetted = Mathf.Clamp01((float) clipNormalizedElapsedTime / this.BeginTransitionTime);
                        }
                    }
                    else
                    {
                        weightSetted = 1f;
                    }

                    atLeastOneClipIsPlaying = true;
                }
                //End calculating weight

                PlayableExtensions.SetInputWeight(this.AnimationMixerPlayable, this.UniqueAnimationClips[i].InputHandler, weightSetted);
                dynamicallyCalculatedElapsedTime += this.UniqueAnimationClips[i].AnimationClip.length;
            }

            if (!atLeastOneClipIsPlaying)
            {
                this.HasEnded = true;

                if (this.isInfinite)
                {
                    PlayableExtensions.SetInputWeight(this.AnimationMixerPlayable, this.UniqueAnimationClips[this.UniqueAnimationClips.Count - 1].InputHandler, 1f);
                }
            }
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
    public List<UniqueAnimationClip> UniqueAnimationClips;
}

[Serializable]
class UniqueAnimationClip
{
    public AnimationClip AnimationClip;
    public float EndTransitionTime;

    #region Dynamically Setted

    public int InputHandler;

    #endregion
}

class BlendedAnimationLayer : MyAnimationLayer
{
    public int LayerID;
    public List<BlendedAnimationClip> BlendedAnimationClips;

    public AnimationMixerPlayable AnimationMixerPlayable { get; private set; }

    public BlendedAnimationLayer(PlayableGraph PlayableGraph, int layerId, List<BlendedAnimationClip> blendedAnimationClips)
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