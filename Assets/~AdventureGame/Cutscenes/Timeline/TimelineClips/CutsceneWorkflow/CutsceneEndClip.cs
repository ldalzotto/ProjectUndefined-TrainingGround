using UnityEngine;
using UnityEngine.Playables;

namespace AdventureGame
{
    public class CutsceneEndClip : PlayableAsset
    {

        public CutsceneEndBehavior PlayerItemHolderBehavior = new CutsceneEndBehavior();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<CutsceneEndBehavior>.Create(graph, PlayerItemHolderBehavior);
        }
    }

}
