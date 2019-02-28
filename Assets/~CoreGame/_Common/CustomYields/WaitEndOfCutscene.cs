using UnityEngine;
using UnityEngine.Playables;

public class WaitEndOfCutscene : CustomYieldInstruction
{

    private PlayableDirector playableDirector;

    public WaitEndOfCutscene(PlayableDirector playableDirector)
    {
        this.playableDirector = playableDirector;
    }

    public override bool keepWaiting
    {
        get
        {
            return (playableDirector.state == PlayState.Playing);
        }
    }
}
