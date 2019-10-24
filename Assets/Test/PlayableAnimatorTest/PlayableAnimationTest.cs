using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class PlayableAnimationTest : MonoBehaviour
{
    public AnimationClip Clip;

    private AnimationLayerMixerPlayable AnimationLayerMixerPlayable;

    private PlayableGraph PlayableGraph;

    public bool Create;

    private void Start()
    {
        PlayableGraph = PlayableGraph.Create(GetType().Name);
        PlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        var playableOutput = AnimationPlayableOutput.Create(PlayableGraph, "Animation", GetComponent<Animator>());


        AnimationLayerMixerPlayable = AnimationLayerMixerPlayable.Create(PlayableGraph);

        playableOutput.SetSourcePlayable(AnimationLayerMixerPlayable);

        PlayableGraph.Play();
    }

    private void Update()
    {
        if (Create)
        {
            var clipPlayable = AnimationClipPlayable.Create(this.PlayableGraph, this.Clip);
            clipPlayable.SetApplyFootIK(false);
            clipPlayable.SetApplyPlayableIK(false);
            int inputHandle = this.AnimationLayerMixerPlayable.AddInput(clipPlayable, 0, 1f);
            Create = false;
            StartCoroutine(AfterClipPlayed(clipPlayable, inputHandle));
        }
    }

    private IEnumerator AfterClipPlayed(AnimationClipPlayable AnimationClipPlayable, int inputHandle)
    {
        yield return new WaitForSeconds(AnimationClipPlayable.GetAnimationClip().length);
        AnimationClipPlayable.SetDone(true);
        this.AnimationLayerMixerPlayable.DisconnectInput(inputHandle);
        this.AnimationLayerMixerPlayable.SetInputCount(this.AnimationLayerMixerPlayable.GetInputCount() - 1);
        AnimationClipPlayable.Destroy();
    }

    private void LateUpdate()
    {
    }

    private void OnDisable()
    {
        PlayableGraph.Destroy();
    }
}