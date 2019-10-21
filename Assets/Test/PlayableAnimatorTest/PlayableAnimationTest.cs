using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class PlayableAnimationTest : MonoBehaviour
{
    private AnimationLayerMixerPlayable AnimationLayerMixerPlayable;

    public AnimationClip Clip;
    public AnimationClip Clip2;
    public AnimationClip Clip3;
    private float clip3CrossFade = 0f;
    public float clip3CrossFadeSpeed;
    private int clip3InputNb = -1;

    private AnimationClipPlayable clip3Playable;

    private float elaspedTime;
    [Range(0f, 1f)] public float LayerClip3Weight;

    private AnimationMixerPlayable mainMixer;

    private PlayableGraph PlayableGraph;

    public bool PlayClip3;
    [Range(0f, 1f)] public float RunWeight;

    private void Start()
    {
        PlayableGraph = PlayableGraph.Create(GetType().Name);
        PlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        var playableOutput = AnimationPlayableOutput.Create(PlayableGraph, "Animation", GetComponent<Animator>());
        mainMixer = AnimationMixerPlayable.Create(PlayableGraph, normalizeWeights: true);


        var animationClipPlayable1 = AnimationClipPlayable.Create(PlayableGraph, Clip);
        animationClipPlayable1.Play();
        animationClipPlayable1.SetApplyFootIK(false);
        mainMixer.AddInput(animationClipPlayable1, 0);

        var animationClipPlayable2 = AnimationClipPlayable.Create(PlayableGraph, Clip2);
        animationClipPlayable2.Play();
        animationClipPlayable2.SetApplyFootIK(false);
        mainMixer.AddInput(animationClipPlayable2, 0);

        clip3Playable = AnimationClipPlayable.Create(PlayableGraph, Clip3);
        clip3Playable.SetDuration(Clip3.length);
        clip3Playable.Play();
        clip3Playable.SetApplyFootIK(false);

        AnimationLayerMixerPlayable = AnimationLayerMixerPlayable.Create(PlayableGraph);
        var port = AnimationLayerMixerPlayable.AddInput(mainMixer, 0);
        AnimationLayerMixerPlayable.SetInputWeight(port, 1);

        //  this.clip3InputNb = this.AnimationLayerMixerPlayable.AddInput(this.clip3Playable, 0);
        //   this.AnimationLayerMixerPlayable.SetInputWeight(this.clip3InputNb, this.LayerClip3Weight);

        playableOutput.SetSourcePlayable(AnimationLayerMixerPlayable);

        PlayableGraph.Play();
    }

    private void Update()
    {
        mainMixer.SetInputWeight(0, RunWeight);
        mainMixer.SetInputWeight(1, 1f - RunWeight);

        if (clip3InputNb >= 0) AnimationLayerMixerPlayable.SetInputWeight(clip3InputNb, LayerClip3Weight);

    }

    private void LateUpdate()
    {
       
        // PlayableGraph.Evaluate(Time.deltaTime);

        if (PlayClip3)
        {
            clip3InputNb = AnimationLayerMixerPlayable.AddInput(clip3Playable, 0);
            clip3Playable.SetTime(0f);
            clip3Playable.Play();
            clip3Playable.SetDone(false);
            clip3CrossFade = 1f;

            PlayClip3 = false;
        }

        if (clip3Playable.IsDone())
        {
            clip3CrossFade = Mathf.Clamp01(clip3CrossFade - Time.deltaTime * clip3CrossFadeSpeed);

            if (clip3CrossFade == 0f && clip3InputNb >= 0)
            {
                AnimationLayerMixerPlayable.DisconnectInput(clip3InputNb);
                clip3InputNb = -1;
            }
        }
        else
        {
            clip3CrossFade = Mathf.Clamp01(clip3CrossFade + Time.deltaTime * clip3CrossFadeSpeed);
        }

        LayerClip3Weight = clip3CrossFade;
    }

    private void OnDisable()
    {
        PlayableGraph.Destroy();
    }
}