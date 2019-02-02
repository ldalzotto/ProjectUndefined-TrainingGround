using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class RuntimeTimeline : MonoBehaviour
{
    public GameObject Obj2Prefab;
    public PlayableDirector playableDirector;

    private void Start()
    {
        var obj2 = Instantiate(Obj2Prefab, transform);
        var timelineAsset = (TimelineAsset)playableDirector.playableAsset;

        foreach (var trackAsset in timelineAsset.GetOutputTracks())
        {
            if (trackAsset.name == "O2Animation")
            {
                playableDirector.SetGenericBinding(trackAsset, obj2.GetComponent<Animator>());
            }
        }

    }
}
